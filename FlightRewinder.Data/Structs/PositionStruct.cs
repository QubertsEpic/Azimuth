using FlightRewinderData.Classes;
using FlightRewinderData.StructAttributes;
using Microsoft.FlightSimulator.SimConnect;
using System.Runtime.InteropServices;

namespace FlightRewinderData.Structs
{
 
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct PositionSetStruct
    {
        public double Altitude;
        public double Longitude;
        public double Latitude;

        public double Bank;
        public double Pitch;
        public double Heading;

        public double Throttle;

        public double FlapsPosition;
        public double SpoilerHandlePosition;
        public double GearHandlePosition;

        public double VelocityX;

        public double VelocityY;

        public double VelocityZ;

        public uint OnGround;
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
        public static SIMCONNECT_DATA_INITPOSITION PosStructToInitPos(PositionStruct data)
        {
            SIMCONNECT_DATA_INITPOSITION pos = new SIMCONNECT_DATA_INITPOSITION()
            {
                Latitude = data.Latitude,
                Longitude = data.Longitude,
                Altitude = data.Altitude,
                Pitch = data.Pitch,
                Bank = data.Bank,
                Heading = data.Heading,
                OnGround = data.OnGround,
                Airspeed = 0
            };
            return pos;
        }

        public static PositionSetStruct ToSet(PositionStruct posStruct)
        {
            return new PositionSetStruct()
            {
                Altitude = posStruct.Altitude,
                Latitude = posStruct.Latitude,
                Longitude = posStruct.Longitude,
                Bank = posStruct.Bank,
                FlapsPosition = posStruct.FlapsPosition,
                GearHandlePosition = posStruct.GearHandlePosition,
                Heading = posStruct.Heading,
                OnGround = posStruct.OnGround,
                Pitch = posStruct.Pitch,
                SpoilerHandlePosition = posStruct.SpoilerHandlePosition,
                Throttle = posStruct.Throttle,
                VelocityX = posStruct.VelocityX,
                VelocityY = posStruct.VelocityY,
                VelocityZ = posStruct.VelocityZ
            };
        }

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
    }
}
