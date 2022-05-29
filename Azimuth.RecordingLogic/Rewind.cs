using Azimuth.Classes;
using Azimuth.Data.DataEventArgs;
using Azimuth.Data.Enums;
using Azimuth.Structs;
using SimConnectWrapper.Core;
using System.Diagnostics;


namespace Azimuth.RecordingLogic
{
    public class Rewind
    {
        public Connection Instance;

        public event EventHandler<ReplayFrameChangedEventArgs>? FrameFinished;
        public event EventHandler? ReplayStarted;
        public event EventHandler? ReplayStopped;

        private RecordedFrame? PositionOne;
        private RecordedFrame? PositionTwo;
        private long CurrentReplayTime = 0;

        public bool Subscribed { get; private set; }
        public long CurrentSpan = 0;
        public double RewindRate = -1;
        public long Correction = 0;
        public int PositionTwoIndex = 0;
        public long? ReplayTime;
        public Stopwatch Watch = Stopwatch.StartNew();

        public bool Playing = true;
        private bool Stopping = false;

        private TaskCompletionSource<bool>? tick;
        public List<RecordedFrame>? RecordedFrames { get; private set; }

        public Rewind(Connection instance)
        {
            Instance = instance;
        }

        public void AddReferences()
        {
            Instance.FrameEvent += Instance_FrameEvent;
        }

        public void RemoveReferences()
        {
            Instance.FrameEvent -= Instance_FrameEvent;
        }

        public void SetRewindSpeed(double rewindRate)
        {
            if (Playing && Watch != null)
            {
                Correction = (long)(Watch.ElapsedMilliseconds / rewindRate);
            }
            RewindRate = rewindRate;
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
            if (PositionTwoIndex < 0 || PositionTwoIndex > frames.Count)
                PositionTwoIndex = 0;
            RecordedFrames = frames;
            SeekRewind(PositionTwoIndex);
        }

        public void SeekRewind(int framePosition)
        {
            if (framePosition < RecordedFrames?.Count || framePosition > -1)
            {
                PositionTwoIndex = framePosition;
                ShiftPositions(PositionTwoIndex);
                if (PositionTwo != null)
                    CurrentReplayTime = PositionTwo.Time;
            }
        }

        public void StartRewind()
        {
            if (RecordedFrames == null || PositionTwo == null)
                throw new InvalidOperationException("Frames are not loaded correctly.");
            
            SetFrameEventHandler(true);

            Task.Run(Update);
            Playing = true;
            ReplayStarted?.Invoke(this, new());
        }


        public async void Update()
        {
            if (PositionTwo == null)
                throw new NullReferenceException("Frames are not loaded correctly.");

            Watch.Restart();
            long previousTime = 0;
            try
            { 
                while (true)
                {
                    if (Stopping)
                    {
                        Stopping = false;
                        AbortReplay();
                        return;
                    }

                    tick = new TaskCompletionSource<bool>();
                    await tick.Task;
                    tick = null;

                    long currentPreviousTime = previousTime;
                    CurrentReplayTime += (long)(((previousTime = Watch.ElapsedMilliseconds) - currentPreviousTime) * RewindRate);

                    Collision result;
                    while ((result = Colliding) != Collision.Zero)
                    {
                        FrameFinished?.Invoke(this, new(PositionTwoIndex, CurrentReplayTime));
                        if (result == Collision.Before)
                        {
                            PositionTwoIndex -= 1;
                        }
                        else
                        {
                            PositionTwoIndex += 1;
                        }

                        if (PositionTwoIndex > RecordedFrames?.Count - 1 || PositionTwoIndex < 0)
                        {
                            AbortReplay();
                            return;
                        }

                        ShiftPositions(PositionTwoIndex);
                    }
                    MoveCraft();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Replay Failed: " + ex.Message);
                AbortReplay();
            }
        }

        public void ShiftPositions(int index)
        {
            if (index < 0 || index > RecordedFrames?.Count - 1)
                throw new IndexOutOfRangeException("Cannot use out of range index.");
            PositionTwo = RecordedFrames?[index];
            if (index < 1)
                PositionOne = null;
            else
                PositionOne = RecordedFrames?[index - 1];
        }

        public void MoveCraft()
        {
            if (PositionOne != null && PositionTwo != null)
            {
                double interpolation = (double)(CurrentReplayTime - PositionOne.Time) / (PositionTwo.Time - PositionOne.Time);

                if (interpolation == 0.5)
                {
                    interpolation = 0.501;
                }
                Instance.SetPos(Connection.UserPlane, PositionStructOperators.Interpolate(PositionStructOperators.ToSet(PositionOne.Position), PositionStructOperators.ToSet(PositionTwo.Position), interpolation));
            }
        }

        public void AbortReplay()
        {
            Playing = false;
            ReplayTime = null;
            SetFrameEventHandler(false);
            ReplayStopped?.Invoke(this, new());
        }

        private void SetFrameEventHandler(bool state)
        {
            if(Instance != null)
            {
                if(Subscribed == true)
                {
                    if(state == false)
                    {
                        Instance.FrameEvent -= Instance_FrameEvent;
                        Subscribed = false;
                    }
                }
                else
                {
                    if (state == true)
                    {
                        Instance.FrameEvent += Instance_FrameEvent;
                        Subscribed = true;
                    }

                }
            }
        }

        private void Instance_FrameEvent(object? sender, EventArgs e)
        {
            if (Playing)
            {
                if (tick != null)
                {
                    tick.TrySetResult(true);
                }
            }
        }
        public Collision Colliding
        {
            get
            {
                if (PositionTwo != null)
                {
                    if (CurrentReplayTime > PositionTwo.Time)
                        return Collision.After;
                    if (PositionOne != null)
                    {
                        CurrentSpan = PositionTwo.Time - PositionOne.Time;
                    }
                    
                    if (CurrentReplayTime < PositionTwo.Time - CurrentSpan)
                        return Collision.Before;
                }
                    return Collision.Zero;
            }
        }

     
    }
}
