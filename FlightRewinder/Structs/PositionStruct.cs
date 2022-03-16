using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FlightRewinderData.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PositionStruct
    {
        public double Altitude;
        public double Longitude;
        public double Latitude;
        //Unfinished.
    }
}
