using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightRewinder.Data.DataEventArgs
{
    public class ReplayFrameChangedEventArgs : EventArgs
    {
        public int? FrameIndex;
        public long? CurrentTime;
        public ReplayFrameChangedEventArgs(int? frameIndex, long? currentTime) 
        {
            FrameIndex = frameIndex;
            CurrentTime = currentTime;
        }
    }
}
