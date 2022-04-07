using FlightRewinder.Classes;
using FlightRewinder.StructAttributes;
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
        [Definition("PLANE HEADING DEGREES MAGNETIC", UnitTypes.Degree, SIMCONNECT_DATATYPE.FLOAT64)]
        public double TrueHeading;

        [DefinitionAttribute("GENERAL ENG THROTTLE LEVER POSITION:1", "percent", SIMCONNECT_DATATYPE.FLOAT64)]
        public double Throttle;

        [DefinitionAttribute("FLAPS HANDLE INDEX", UnitTypes.Number, SIMCONNECT_DATATYPE.INT32)]
        public uint FlapsHandlePosition;
        [Definition("LEADING EDGE FLAPS LEFT PERCENT", UnitTypes.PercentOver100, SIMCONNECT_DATATYPE.FLOAT64)]
        public double FlapsEdgeFrontLeftPercent;
        [Definition("LEADING EDGE FLAPS RIGHT PERCENT", UnitTypes.PercentOver100, SIMCONNECT_DATATYPE.FLOAT64)]
        public double FlapsEdgeFrontRightPercent;
        [Definition("TRAILING EDGE FLAPS LEFT PERCENT", UnitTypes.PercentOver100, SIMCONNECT_DATATYPE.FLOAT64)]
        public double FlapsEdgeBackLeft;
        [Definition("TRAILING EDGE FLAPS RIGHT PERCENT", UnitTypes.PercentOver100, SIMCONNECT_DATATYPE.FLOAT64)]
        public double FlapsEdgeBackRight;

        [Definition("ELEVATOR POSITION", UnitTypes.Position, SIMCONNECT_DATATYPE.FLOAT64)]
        public double ElevatorPosition;
        [Definition("ELEVATOR TRIM POSITION", UnitTypes.PercentOver100, SIMCONNECT_DATATYPE.FLOAT64)]
        public double ElevatorTrimPosition;

        [Definition("RUDDER PEDAL POSITION", UnitTypes.Position, SIMCONNECT_DATATYPE.FLOAT64)]
        public double RudderHandlePosition;
        [Definition("RUDDER TRIM PCT", UnitTypes.PercentOver100, SIMCONNECT_DATATYPE.FLOAT64)]
        public double RudderTrimPercent;

        [Definition("AILERON POSITION", UnitTypes.Position, SIMCONNECT_DATATYPE.FLOAT64)]
        public double AileronPosition;
        [Definition("AILERON TRIM PCT", UnitTypes.PercentOver100, SIMCONNECT_DATATYPE.FLOAT64)]
        public double AileronTrimPercent;

        [DefinitionAttribute("SPOILERS HANDLE POSITION", UnitTypes.PercentOver100, SIMCONNECT_DATATYPE.FLOAT64)]
        public double SpoilerHandlePosition;

        [Definition("GEAR CENTER POSITION", UnitTypes.PercentOver100, SIMCONNECT_DATATYPE.FLOAT64)]
        public double GearCentrePosition;
        [Definition("GEAR AUX POSITION", UnitTypes.PercentOver100, SIMCONNECT_DATATYPE.FLOAT64)]
        public double GearAuxPosition;
        [Definition("GEAR LEFT POSITION", UnitTypes.PercentOver100, SIMCONNECT_DATATYPE.FLOAT64)]
        public double GearLeftPosition;
        [Definition("GEAR RIGHT POSITION", UnitTypes.PercentOver100, SIMCONNECT_DATATYPE.FLOAT64)]
        public double GearRightPosition;
            
        [Definition("BRAKE LEFT POSITION", UnitTypes.Position, SIMCONNECT_DATATYPE.FLOAT64)]
        public double BrakeLeftPosition;
        [Definition("BRAKE RIGHT POSITION", UnitTypes.Position, SIMCONNECT_DATATYPE.FLOAT64)]
        public double BrakeRightPosition;

        [DefinitionAttribute("VELOCITY BODY X", "feet/second", SIMCONNECT_DATATYPE.FLOAT64)]
        public double VelocityX;
        [DefinitionAttribute("VELOCITY BODY Y", "feet/second", SIMCONNECT_DATATYPE.FLOAT64)]
        public double VelocityY;
        [DefinitionAttribute("VELOCITY BODY Z", "feet/second", SIMCONNECT_DATATYPE.FLOAT64)]
        public double VelocityZ;
        [Definition("ROTATION VELOCITY BODY X", UnitTypes.FeetPerSecond, SIMCONNECT_DATATYPE.FLOAT64)]
        public double RotVelocityX;
        [Definition("ROTATION VELOCITY BODY Y", UnitTypes.FeetPerSecond, SIMCONNECT_DATATYPE.FLOAT64)]
        public double RotVelocityY;
        [Definition("ROTATION VELOCITY BODY Z", UnitTypes.FeetPerSecond, SIMCONNECT_DATATYPE.FLOAT64)]
        public double RotVelocityZ;
        [Definition("ACCELERATION BODY X", UnitTypes.FeetPerSecondSquared, SIMCONNECT_DATATYPE.FLOAT64)]
        public double AccelerationX;
        [Definition("ACCELERATION BODY Y", UnitTypes.FeetPerSecondSquared, SIMCONNECT_DATATYPE.FLOAT64)]
        public double AccelerationY;
        [Definition("ACCELERATION BODY Z", UnitTypes.FeetPerSecondSquared, SIMCONNECT_DATATYPE.FLOAT64)]
        public double AccelerationZ;
        
        [DefinitionAttribute("SIM ON GROUND", "Bool", SIMCONNECT_DATATYPE.INT32)]
        public uint OnGround;

        //Unfinished.
    }

    public class PositionStructHelper
    {
        
    }
}
