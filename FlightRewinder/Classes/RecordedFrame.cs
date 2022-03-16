using FlightRewinderData.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightRewinderData.Classes
{
    public class RecordedFrame
    {
        public PositionStruct Position;
        public long deltaTime;
        public RecordedFrame(PositionStruct position, long deltaTime)
        {
            Position = position;
            this.deltaTime = deltaTime;
        }
    }
}
