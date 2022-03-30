using FlightRewinderData.Classes;
using FlightRewinderData.DataEventArgs;
using FlightRewinderData.Structs;
using SimConnectWrapper.Core;
using SimConnectWrapper.Core.SimEventArgs;
using System.Diagnostics;

namespace FlightRewinderRecordingLogic
{
    public class Recorder
    {
        public event EventHandler<RecorderUpdatedEventArgs>? RecorderUpdated;

        public uint? MaxFrames;
        public List<RecordedFrame>? ListOfFrames;
        public Stopwatch watch = new Stopwatch();
        public bool Recording => EndingTime < 0;

        private long EndingTime;
        private Connection connectionInstance;


        public Recorder(Connection connection, uint maxFrames = 2000)
        {
            connectionInstance = connection;
            MaxFrames = maxFrames;
        }

        public void StartRecording()
        {
            ListOfFrames = new List<RecordedFrame>();
            EndingTime = -1;
            watch.Restart();
            connectionInstance.LocationChanged += OnLocationUpdated;
        }

        /// <summary>
        /// Begins the recording process.
        /// </summary>
        public void RecordFrame(PositionStruct postion)
        {
            if (ListOfFrames == null)
                throw new InvalidOperationException("Recording hasn't started.");
            long currentFrame = watch.ElapsedMilliseconds;
            var nextFrame = new RecordedFrame(postion, currentFrame);
            ListOfFrames.Add(nextFrame);
            //Check to see if too many frames have been recorded.
            if (ListOfFrames.Count > MaxFrames)
                RemoveFrame(ListOfFrames.Count-1);
            RecorderUpdated?.Invoke(this, new(nextFrame));
        }

        public void RemoveFrame(int index, bool correctDeltaTime = true)
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

        public void StopRecording()
        {
            EndingTime = watch.ElapsedMilliseconds;
            connectionInstance.LocationChanged -= OnLocationUpdated;
        }

        public SaveData DumpData()
        {
            if (ListOfFrames == null)
                throw new NullReferenceException("Recording invalid.");
            return new SaveData("Title", ListOfFrames);
        }
    }
}