using FlightRewinderData.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightRewinderData.DataEventArgs
{
    public class RecorderUpdatedEventArgs : EventArgs
    {
        public RecordedFrame Frame;
        public RecorderUpdatedEventArgs(RecordedFrame frame)
        {
            Frame = frame;
        }
    }
}
