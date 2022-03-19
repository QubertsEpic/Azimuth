using FlightRewinderData.StructAttributes;
using Microsoft.FlightSimulator.SimConnect;
using System.Runtime.InteropServices;

namespace FlightRewinderData.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PositionStruct
    {
        [DefinitionAttribute("PLANE ALTITUDE", "feet", SIMCONNECT_DATATYPE.FLOAT64)]
        public double Altitude;
        [DefinitionAttribute("PLANE LONGITUDE", "feet", SIMCONNECT_DATATYPE.FLOAT64)]
        public double Longitude;
        [DefinitionAttribute("PLANE LATITUDE", "feet", SIMCONNECT_DATATYPE.FLOAT64)]
        public double Latitude;

        [DefinitionAttribute("PLANE BANK DEGREES", "Radians", SIMCONNECT_DATATYPE.FLOAT64)]
        public double Bank;
        [DefinitionAttribute("PLANE PITCH DEGREES", "Radians", SIMCONNECT_DATATYPE.FLOAT64)]
        public double Pitch;
        [DefinitionAttribute("PLANE HEADING DEGREES TRUE", "Radians", SIMCONNECT_DATATYPE.FLOAT64)]
        public double Heading;

        [DefinitionAttribute("GENERAL ENG THROTTLE LEVER POSITION:1", "percent", SIMCONNECT_DATATYPE.FLOAT64)]
        public int Throttle;

        [DefinitionAttribute("PLANE ALTITUDE", "feet", SIMCONNECT_DATATYPE.FLOAT64)]
        public int FlapsPosition;
        [DefinitionAttribute("PLANE ALTITUDE", "feet", SIMCONNECT_DATATYPE.FLOAT64)]
        public int SpoilerHandlePosition;
        [DefinitionAttribute("PLANE ALTITUDE", "feet", SIMCONNECT_DATATYPE.FLOAT64)]
        public int GearHandlePosition;

        [DefinitionAttribute("VELOCITY BODY X", "feet/second", SIMCONNECT_DATATYPE.FLOAT64)]
        public double VelocityX;

        [DefinitionAttribute("VELOCITY BODY Y", "feet/second", SIMCONNECT_DATATYPE.FLOAT64)]
        public double VelocityY;

        [DefinitionAttribute("VELOCITY BODY Z", "feet/second", SIMCONNECT_DATATYPE.FLOAT64)]
        public double VelocityZ;


        public static List<DefinitionAttribute> GetAllAttributes()
        {
            List<DefinitionAttribute> attributes = new List<DefinitionAttribute>();
            foreach (var value in typeof(PositionStruct).GetFields())
            {
                DefinitionAttribute[] customAttributes = (DefinitionAttribute[])value.GetCustomAttributes(typeof(DefinitionAttribute), true);
                for (int i = 0; i < customAttributes.Length; i++)
                {
                    attributes.Add(customAttributes[i]);
                }
            }
            return attributes;
        }
        //Unfinished.
    }
}
