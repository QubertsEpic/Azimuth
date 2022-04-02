using FlightRewinderData.Classes;
using FlightRewinderData.StructAttributes;
using Microsoft.FlightSimulator.SimConnect;
using System.Runtime.InteropServices;

namespace FlightRewinder.Structs
{
 
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public partial struct PositionSetStruct
    {
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct PositionStruct
    {
        

        [DefinitionAttribute("PLANE ALTITUDE", UnitTypes.Foot, SIMCONNECT_DATATYPE.FLOAT64)]
        public double Altitude;
        [DefinitionAttribute("PLANE LONGITUDE", UnitTypes.Degree, SIMCONNECT_DATATYPE.FLOAT64)]
        public double Longitude;
        [DefinitionAttribute("PLANE LATITUDE", UnitTypes.Degree, SIMCONNECT_DATATYPE.FLOAT64)]
        public double Latitude;

        [DefinitionAttribute("PLANE BANK DEGREES", UnitTypes.Degree, SIMCONNECT_DATATYPE.FLOAT64)]
        public double Bank;
        [DefinitionAttribute("PLANE PITCH DEGREES", UnitTypes.Degree, SIMCONNECT_DATATYPE.FLOAT64)]
        public double Pitch;
        [DefinitionAttribute("PLANE HEADING DEGREES TRUE", UnitTypes.Degree, SIMCONNECT_DATATYPE.FLOAT64)]
        public double Heading;

        [DefinitionAttribute("GENERAL ENG THROTTLE LEVER POSITION:1", "percent", SIMCONNECT_DATATYPE.FLOAT64)]
        public double Throttle;

        [DefinitionAttribute("FLAPS HANDLE PERCENT", UnitTypes.PercentOver100, SIMCONNECT_DATATYPE.FLOAT64)]
        public double FlapsPosition;
        [DefinitionAttribute("SPOILERS HANDLE POSITION", UnitTypes.PercentOver100, SIMCONNECT_DATATYPE.FLOAT64)]
        public double SpoilerHandlePosition;
        [DefinitionAttribute("GEAR HANDLE POSITION", UnitTypes.PercentOver100, SIMCONNECT_DATATYPE.FLOAT64)]
        public double GearHandlePosition;

        [DefinitionAttribute("VELOCITY BODY X", "feet/second", SIMCONNECT_DATATYPE.FLOAT64)]
        public double VelocityX;

        [DefinitionAttribute("VELOCITY BODY Y", "feet/second", SIMCONNECT_DATATYPE.FLOAT64)]
        public double VelocityY;

        [DefinitionAttribute("VELOCITY BODY Z", "feet/second", SIMCONNECT_DATATYPE.FLOAT64)]
        public double VelocityZ;

        [DefinitionAttribute("SIM ON GROUND", "Bool", SIMCONNECT_DATATYPE.INT32)]
        public uint OnGround;

        //Unfinished.
    }

    public class PositionStructHelper
    {
        
    }
}
