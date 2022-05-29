using Azimuth.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Azimuth.Classes
{
    public class RecordedFrame
    {
        public PositionStruct Position;
        public long Time;
        /// <summary>
        /// Span is the time between the last frame and this one.
        /// </summary>
        public RecordedFrame(PositionStruct position, long time)
        {
            Position = position;
            this.Time = time;
        }
    }
}
