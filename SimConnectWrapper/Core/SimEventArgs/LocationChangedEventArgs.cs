using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimConnectWrapper.Core.SimEventArgs
{
    public class LocationChangedEventArgs : EventArgs
    {
        public LocationChangedEventArgs(Structs.PositionStruct position)
        {
            Position = position;
        }

        public Structs.PositionStruct Position;
    }
}
