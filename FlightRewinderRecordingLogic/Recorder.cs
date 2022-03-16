using FlightRewinderData.Classes;
using FlightRewinderData.Structs;
using System.Diagnostics;

namespace FlightRewinderRecordingLogic
{
    public class Recorder
    {
        public uint MaxFrames;
        public List<RecordedFrame>? ListOfFrames;
        public Stopwatch watch = new Stopwatch();
        public bool Recording = false;

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
            ListOfFrames.Add(new RecordedFrame(postion, watch.ElapsedMilliseconds - PreviousFrame));
            PreviousFrame = currentFrame; 
        }

        public void RemoveFrame(int index)
        {
            if (index > ListOfFrames?.Count - 1 || index < 0)
                throw new IndexOutOfRangeException("Cannot access index to make modifications.");
            if (ListOfFrames == null)
                throw new InvalidOperationException("Recording hasn't started.");
            if (index + 1 != ListOfFrames?.Count)
            {
                ListOfFrames[index + 1].deltaTime += ListOfFrames[index].deltaTime;
            }
            ListOfFrames.RemoveAt(index);
        }

        public List<RecordedFrame> DumpData()
        {
            if (ListOfFrames == null)
                throw new NullReferenceException("Recording invalid.");
            return ListOfFrames;
                
        }
    }
}