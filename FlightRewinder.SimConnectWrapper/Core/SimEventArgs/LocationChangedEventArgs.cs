using FlightRewinderData.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimConnectWrapper.Core.SimEventArgs
{
    public class LocationChangedEventArgs : EventArgs
    {
        public LocationChangedEventArgs(PositionStruct position)
        {
            Position = position;
        }

        public PositionStruct Position;
    }
}
