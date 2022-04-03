using FlightRewinder.Structs;
using FlightRewinder.Classes;
using FlightRewinder.Enums;
using SimConnectWrapper.Core;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FlightRewinderRecordingLogic
{
    public class Rewind
    {
        public Connection Instance;
        public Recorder RecorderInstance;
        public List<RecordedFrame> RecordedFrames = new List<RecordedFrame>();

        public double RewindRate = 1;
        public uint PlaneID;
        public int currentFrame;
        public long? PauseTime;
        public Stopwatch Watch = Stopwatch.StartNew();

        public bool Playing;
        private bool Stopping = false;
        public PositionStruct StartingPosition;

        private TaskCompletionSource<bool>? tick;

        public Rewind(Connection instance)
        {
            Instance = instance;
            RecorderInstance = new Recorder(Instance);
        }

        public void StopRewind()
        {

        }

        public void LoadData(List<RecordedFrame> frames)
        {
            if (frames == null)
                throw new ArgumentNullException(nameof(frames), "This list is null.");
            RecordedFrames = frames;
        }

        public void GetRecords(List<RecordedFrame> records)
        {
            if (records == null)
                throw new ArgumentNullException(nameof(records), "Cannot use null records");
            RecordedFrames = records;
        }

        public void ResumeRewind()
        {
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

        public void StartReplay(uint planeID)
        {
            if (RecordedFrames == null)
                throw new NullReferenceException("Frames are not loaded.");
            Playing = true;
            PlaneID = planeID;
            currentFrame = 0;
            AddReferences();

            if (RecordedFrames.Any())
            {
                Instance.Init(0, RecordedFrames[currentFrame].Position);
            }
            Task.Run(StartReplaying);
        }

        private void Instance_FrameEvent(object? sender, EventArgs e)
        {
            Tick();
        }

        private async Task StartReplaying()
        {
            var frames = reversedFrames.GetEnumerator();
            PositionStruct? lastPosition = null;
            PositionStruct? position = null;
            long lastTime = 0;
            long frameTime = 0;
            Watch.Restart();
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
                    AbortReplay();
                    return;
                }

                long currentTime = (long)(Watch.ElapsedMilliseconds * RewindRate);

                while (currentTime > frameTime)
                {
                    var success = frames.MoveNext();
                    if (success)
                    {
                        lastPosition = position;
                        lastTime = frameTime;
                        position = frames.Current.Position;
                        frameTime = frames.Current.Time;
                        currentFrame++;
                    }
                    else
                    {
                        AbortReplay();
                        return;
                    }
                }
                if (position.HasValue && lastPosition.HasValue)
                {
                    MoveCraft(position, frameTime, lastPosition, lastTime, currentTime);
                }
            }
        }

        public void AbortReplay()
        {
            currentFrame = -1;
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
                    interpolation = 0.501;
                Instance.SetData(0, Definitions.SetLocation, PositionStructOperators.Interpolate(PositionStructOperators.ToSet(oldPosition.Value), PositionStructOperators.ToSet(newPosition.Value), interpolation));
            }
        }
    }
}
