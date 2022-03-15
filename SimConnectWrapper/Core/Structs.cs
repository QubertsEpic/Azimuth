using System;
using System.Runtime.InteropServices;

namespace SimConnectWrapper.Core
{
    public class Structs
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
}
