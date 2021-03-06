using Azimuth.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azimuth.DataEventArgs
{
    public class RecorderUpdatedEventArgs : EventArgs
    {
        public RecordedFrame Frame;
        public int index;
        public RecorderUpdatedEventArgs(RecordedFrame frame, int index)
        {
            Frame = frame;
            this.index = index;
        }
    }
}
