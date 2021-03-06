using Azimuth.Classes;
using Azimuth.DataEventArgs;
using Azimuth.Structs;
using SimConnectWrapper.Core;
using SimConnectWrapper.Core.SimEventArgs;
using System.Diagnostics;

namespace Azimuth.RecordingLogic
{
    public class Recorder
    {
        public event EventHandler<RecorderUpdatedEventArgs>? RecorderUpdated;
        public event EventHandler? RecordingStarted;
        public event EventHandler? RecordingStopped;

        public List<RecordedFrame>? ListOfFrames = new List<RecordedFrame>();
        public Stopwatch watch = Stopwatch.StartNew();
        public bool Recording => EndingTime < 0;
        public int? MaxIndex => ListOfFrames?.Count - 1;

        private bool linked = false;

        private long EndingTime = -1;
        private long PauseTime = 0;
        private long OffsetCorrection = 0;
        private Connection connectionInstance;


        public Recorder(Connection connection)
        {
            connectionInstance = connection;
        }

        public void ResetRecording()
        {
            EndingTime = -1;
            PauseTime = 0;
            OffsetCorrection = 0;
            ListOfFrames = new List<RecordedFrame>();
            watch.Restart();
        }

        public void ChangeEvents(bool add)
        {
            if (add)
            {
                if (!linked)
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
            if (ListOfFrames?.Any() == true)
                OffsetCorrection = watch.ElapsedMilliseconds - (ListOfFrames[ListOfFrames.Count - 1].Time);
            else
                OffsetCorrection = watch.ElapsedMilliseconds;
            ChangeEvents(true);
            RecordingStarted?.Invoke(this, new());
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
            RecorderUpdated?.Invoke(this, new(nextFrame, ListOfFrames.Count - 1));
        }

        public void RemoveFrame(int index)
        {
            if (index > ListOfFrames?.Count - 1 || index < 0)
                throw new IndexOutOfRangeException("Cannot access index to make modifications.");
            if (ListOfFrames != null)
            {
                RecorderUpdated?.Invoke(this, new(ListOfFrames[index], ListOfFrames.Count-1));
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
            RecordingStopped?.Invoke(this, new());
        }

        public SaveData DumpData()
        {
            if (ListOfFrames == null)
                throw new NullReferenceException("Recording invalid.");
            return new SaveData("Title", ListOfFrames);
        }
    }
}