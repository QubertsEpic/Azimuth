using FlightRewinder.Structs;
using FlightRewinder.Classes;
using FlightRewinder.DataEventArgs;
using SimConnectWrapper.Core;
using SimConnectWrapper.Core.SimEventArgs;
using System.Diagnostics;

namespace FlightRewinderRecordingLogic
{
    public class Recorder
    {
        public event EventHandler<RecorderUpdatedEventArgs>? RecorderUpdated;

        public List<RecordedFrame>? ListOfFrames = new List<RecordedFrame>();
        public Stopwatch watch = Stopwatch.StartNew();
        public bool Recording => EndingTime < 0;
        public int? MaxIndex => ListOfFrames?.Count - 1;

        private bool linked = false;

        private long EndingTime;
        private long PauseTime = 0;
        private long OffsetCorrection = 0;
        private Connection connectionInstance;


        public Recorder(Connection connection)
        {
            connectionInstance = connection;
        }

        public void RestartRecording()
        {
            EndingTime = -1;
            PauseTime = 0;
            OffsetCorrection = 0;
            ListOfFrames = new List<RecordedFrame>();
            watch.Restart();
            StartRecording();
        }

        public void ChangeEvents(bool add)
        {
            if (add)
            {
                if(!linked)
                {
                    connectionInstance.LocationChanged += OnLocationUpdated;
                    linked = true;
                }
            }
            else
            {
                if (linked)
                {
                    connectionInstance.LocationChanged -= OnLocationUpdated;
                    linked = false;
                }
            }
        }

        public void StartRecording()
        {
            OffsetCorrection += watch.ElapsedMilliseconds - PauseTime;
            ChangeEvents(true);
        }

        public void Seek(int index, bool removeAhead = true)
        {
            if (ListOfFrames != null)
            {
                if (index > ListOfFrames?.Count - 1 || index < 0)
                    throw new IndexOutOfRangeException("Cannot use index that is out of range.");
                ListOfFrames?.RemoveRange(index, ListOfFrames.Count - index - 1);
            }
        }

        /// <summary>
        /// Begins the recording process.
        /// </summary>
        public void RecordFrame(PositionStruct postion)
        {
            if (ListOfFrames == null)
                throw new InvalidOperationException("Recording hasn't started.");
            long currentFrame = watch.ElapsedMilliseconds - OffsetCorrection;
            var nextFrame = new RecordedFrame(postion, currentFrame);
            ListOfFrames.Add(nextFrame);
            RecorderUpdated?.Invoke(this, new(nextFrame, ListOfFrames.Count-1));
        }

        public void RemoveFrame(int index)
        {
            if (index > ListOfFrames?.Count - 1 || index < 0)
                throw new IndexOutOfRangeException("Cannot access index to make modifications.");
            if (ListOfFrames != null)
            {
                ListOfFrames.RemoveAt(index);
            }
        }

        private void OnLocationUpdated(object? sender, LocationChangedEventArgs args)
        {
            if (!Recording)
                throw new InvalidOperationException("Not Recording.");
            RecordFrame(args.Position);
        }

        public void FinishRecording()
        {
            StopRecording();
            EndingTime = watch.ElapsedMilliseconds;
        }

        public void StopRecording()
        {
            PauseTime = watch.ElapsedMilliseconds;
            ChangeEvents(false);
        }

        public SaveData DumpData()
        {
            if (ListOfFrames == null)
                throw new NullReferenceException("Recording invalid.");
            return new SaveData("Title", ListOfFrames);
        }
    }
}