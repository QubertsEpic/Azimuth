using Azimuth.Classes;
using Azimuth.Data.DataEventArgs;
using Azimuth.RecordingLogic;
using Azimuth.Structs;
using SimConnectWrapper.Core;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace AzimuthRecordingLogic
{
    public class Rewind
    {
        public Connection Instance;
        public Recorder RecorderInstance;

        public event EventHandler<ReplayFrameChangedEventArgs>? FrameFinished;
        public event EventHandler? ReplayStarted;
        public event EventHandler? ReplayStopped;

        public double RewindRate = 1;
        public long? PauseTime;
        public long Correction = 0;
        public int CurrentFrame = 0;
        public Stopwatch Watch = Stopwatch.StartNew();

        public bool Playing;
        private bool Stopping = false;
        public PositionStruct StartingPosition;

        private TaskCompletionSource<bool>? tick;

        public List<RecordedFrame>? RecordedFrames { get; private set; }

        public Rewind(Connection instance)
        {
            Instance = instance;
            RecorderInstance = new Recorder(Instance);
        }

        public void ResumeRewind()
        {
            if (!Playing && PauseTime.HasValue)
            {
                Correction += Watch.ElapsedMilliseconds - PauseTime.Value;
            }
        }

        public void AddReferences()
        {
            Instance.FrameEvent += Instance_FrameEvent;
        }

        public void RemoveReferences()
        {
            Instance.FrameEvent -= Instance_FrameEvent;
        }

        public void PauseRewind()
        {
            if (Playing)
            {
                PauseTime = Watch.ElapsedMilliseconds;
                Playing = false;
            }
        }

        public void StopReplay()
        {
            if (Playing)
            {
                Stopping = true;
            }
        }

        public void LoadFrames(List<RecordedFrame> frames)
        {
            if (frames == null)
            {
                throw new ArgumentNullException(nameof(frames), "Cannot use null frame set.");
            }
            if (CurrentFrame < 0 || CurrentFrame > frames.Count)
                CurrentFrame = 0;
            RecordedFrames = frames;
        }

        public void SeekRewind(int framePosition)
        {
            CurrentFrame = framePosition;
        }

        public void StartRewind()
        {
            if (RecordedFrames == null)
            {
                throw new NullReferenceException("Cannot replay while frames are null.");
            }
            if(CurrentFrame < 0 || CurrentFrame > RecordedFrames.Count)
            {
                throw new IndexOutOfRangeException("Cannot use null currentFrame index.");
            }
            AddReferences();
            Stopping = false;
            Watch.Restart();


            new Thread(StartReplaying).Start();
            ReplayStarted?.Invoke(this, new());
        }

        private void Instance_FrameEvent(object? sender, EventArgs e)
        {
            Tick();
        }

        private async void StartReplaying()
        {
            if (RecordedFrames == null)
                return;

            long startingTime = RecordedFrames[CurrentFrame].Time;
            PositionStruct? lastPosition = null;
            PositionStruct? position = null;
            long lastTime = 0;
            long frameTime = startingTime;
            Playing = true;
            while (true)
            {
                //Wait for a frame to pass.
                tick = new TaskCompletionSource<bool>();
                await tick.Task;
                tick = null;

                if (!Playing)
                {
                    //Paused.
                    continue;
                }

                if (Stopping)
                {
                    Stopping = false;
                    AbortReplay();
                    return;
                }

                long currentTime = (long)((startingTime - (Watch.ElapsedMilliseconds - Correction)) * RewindRate);
                try
                {
                    while (currentTime < frameTime)
                    {
                        FrameFinished?.Invoke(this, new(CurrentFrame, currentTime));
                        CurrentFrame -= 1;

                        if (CurrentFrame < 0)
                        {
                            //Final available frame has been played.
                            AbortReplay();
                            return;
                        }

                        lastPosition = position;
                        lastTime = frameTime;
                        position = RecordedFrames[CurrentFrame].Position;
                        frameTime = RecordedFrames[CurrentFrame].Time;

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Replay Failed :(" + ex.Message);
                    AbortReplay();
                }
                
                if (position.HasValue && lastPosition.HasValue)
                {
                    MoveCraft(position, frameTime, lastPosition, lastTime, currentTime);
                }
            }
        }

        public void AbortReplay()
        {
            Playing = false;
            ReplayStopped?.Invoke(this, new());
            RemoveReferences();
        }

        private void Tick()
        {
            if (Playing)
            {
                tick?.SetResult(true);
            }
        }

        public void MoveCraft(PositionStruct? newPosition, long time, PositionStruct? oldPosition, long oldTime, long currenTime)
        {
            if (newPosition.HasValue && oldPosition.HasValue)
            {
                double interpolation = (double)(currenTime - oldTime) / (time - oldTime);
                if (interpolation == 0.5)
                {
                    interpolation = 0.501;
                }
                Instance.SetPos(Connection.UserPlane, PositionStructOperators.Interpolate(PositionStructOperators.ToSet(oldPosition.Value), PositionStructOperators.ToSet(newPosition.Value), interpolation));
            }
        }
    }
}
