using FlightRewinderData.Classes;
using FlightRewinderData.StructAttributes;
using Microsoft.FlightSimulator.SimConnect;
using System.Runtime.InteropServices;

namespace FlightRewinderData.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PositionStruct
    {
        [DefinitionAttribute("PLANE ALTITUDE", UnitTypes.Radian, SIMCONNECT_DATATYPE.FLOAT64)]
        public double Altitude;
        [DefinitionAttribute("PLANE LONGITUDE", UnitTypes.Radian, SIMCONNECT_DATATYPE.FLOAT64)]
        public double Longitude;
        [DefinitionAttribute("PLANE LATITUDE", "feet", SIMCONNECT_DATATYPE.FLOAT64)]
        public double Latitude;

        [DefinitionAttribute("PLANE BANK DEGREES", UnitTypes.Radian, SIMCONNECT_DATATYPE.FLOAT64)]
        public double Bank;
        [DefinitionAttribute("PLANE PITCH DEGREES", UnitTypes.Radian, SIMCONNECT_DATATYPE.FLOAT64)]
        public double Pitch;
        [DefinitionAttribute("PLANE HEADING DEGREES TRUE", UnitTypes.Degree, SIMCONNECT_DATATYPE.FLOAT64)]
        public double Heading;

        [DefinitionAttribute("GENERAL ENG THROTTLE LEVER POSITION:1", "percent", SIMCONNECT_DATATYPE.FLOAT64)]
        public int Throttle;

        [DefinitionAttribute("FLAPS HANDLE PERCENT", UnitTypes.PercentOver100, SIMCONNECT_DATATYPE.FLOAT64)]
        public int FlapsPosition;
        [DefinitionAttribute("SPOILERS HANDLE POSITION", UnitTypes.PercentOver100, SIMCONNECT_DATATYPE.FLOAT64)]
        public int SpoilerHandlePosition;
        [DefinitionAttribute("GEAR HANDLE POSITION", UnitTypes.PercentOver100, SIMCONNECT_DATATYPE.FLOAT64)]
        public int GearHandlePosition;

        [DefinitionAttribute("VELOCITY BODY X", "feet/second", SIMCONNECT_DATATYPE.FLOAT64)]
        public double VelocityX;

        [DefinitionAttribute("VELOCITY BODY Y", "feet/second", SIMCONNECT_DATATYPE.FLOAT64)]
        public double VelocityY;

        [DefinitionAttribute("VELOCITY BODY Z", "feet/second", SIMCONNECT_DATATYPE.FLOAT64)]
        public double VelocityZ;

        [DefinitionAttribute("SIM ON GROUND", UnitTypes.Bool, SIMCONNECT_DATATYPE.INT32)]
        public uint OnGround;

        public static SIMCONNECT_DATA_INITPOSITION PosStructToInitPos(PositionStruct data)
        {
            return new SIMCONNECT_DATA_INITPOSITION()
            {
                Airspeed = 0,
                Bank = data.Bank,
                Altitude = data.Altitude,
                Heading = data.Heading,
                Latitude = data.Latitude,
                Longitude = data.Longitude,
                OnGround = data.OnGround,
                Pitch = data.Pitch
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
        //Unfinished.
    }
}
