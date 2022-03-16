using FlightRewinderData.Classes;
using FlightRewinderData.DataEventArgs;
using FlightRewinderData.Structs;
using System.Diagnostics;

namespace FlightRewinderRecordingLogic
{
    public class Recorder
    {
        public uint? MaxFrames;
        public List<RecordedFrame>? ListOfFrames;
        public Stopwatch watch = new Stopwatch();
        public bool? Recording = false;
        public event EventHandler<RecorderUpdatedEventArgs>? RecorderUpdated;

        private long PreviousFrame;

        public Recorder()
        {
        }

        public void StartRecording()
        {
            ListOfFrames = new List<RecordedFrame>();
            Recording = true;
            watch.Start();
            PreviousFrame = watch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Begins the recording process.
        /// </summary>
        public void RecordFrame(PositionStruct postion)
        {
            if (ListOfFrames == null)
                throw new InvalidOperationException("Recording hasn't started.");
            long currentFrame = watch.ElapsedMilliseconds;
            var nextFrame = new RecordedFrame(postion, watch.ElapsedMilliseconds - PreviousFrame);
            ListOfFrames.Add(nextFrame);
            PreviousFrame = currentFrame;
            RecorderUpdated?.Invoke(this, new(nextFrame));
        }

        public void RemoveFrame(int index, bool correctDeltaTime = true)
        {
            if (index > ListOfFrames?.Count - 1 || index < 0)
                throw new IndexOutOfRangeException("Cannot access index to make modifications.");
            if (ListOfFrames != null)
            {
                if (index + 1 != ListOfFrames.Count && correctDeltaTime == true)
                {
                    ListOfFrames[index + 1].deltaTime += ListOfFrames[index].deltaTime;
                }
                ListOfFrames.RemoveAt(index);
            }
        }

        public List<RecordedFrame> DumpData()
        {
            if (ListOfFrames == null)
                throw new NullReferenceException("Recording invalid.");
            return ListOfFrames;
        }
    }
}