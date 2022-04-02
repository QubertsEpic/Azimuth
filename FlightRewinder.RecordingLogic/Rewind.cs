using FlightRewinder.Data.Structs;
using FlightRewinderData.Classes;
using FlightRewinderData.Enums;
using FlightRewinderData.Structs;
using SimConnectWrapper.Core;
using System.Diagnostics;
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
        public long? OffsetCorrection;
        public Stopwatch? Watch;

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
            if (PauseTime.HasValue)
            {
                OffsetCorrection = Watch?.ElapsedMilliseconds - PauseTime.Value;
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
                PauseTime = Watch?.ElapsedMilliseconds;
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
            OffsetCorrection = OffsetCorrection ?? 0;
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
            var frames = RecordedFrames.GetEnumerator();
            RecordedFrame? previousFrame = null;
            RecordedFrame? frame = null;
            long lastTime = 0;
            Watch = Stopwatch.StartNew();
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

                long? currentTime = (long?)((Watch?.ElapsedMilliseconds - OffsetCorrection) * RewindRate);

                while (currentTime > lastTime)
                {
                    var success = frames.MoveNext();
                    previousFrame = frames.Current;
                    if (success)
                    {
                        lastTime = frames.Current.Time;
                        frame = frames.Current;
                        currentFrame++;
                    }
                    else
                    {
                        AbortReplay();
                        return;
                    }
                }
                if (frame != null)
                {
                    MoveCraft(frame);
                }
            }
        }

        public void AbortReplay()
        {
            Watch = null;
            currentFrame = -1;
        }

        private void Tick()
        {
            if (Playing)
            {
                tick?.SetResult(true);
            }
        }

        public void MoveCraft(RecordedFrame newPosition)
        {
            Instance.SetData(0, Definitions.SetLocation, PositionStructOperators.ToSet(newPosition.Position));
        }
    }
}
