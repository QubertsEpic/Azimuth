using FlightRewinderData.StructAttributes;
using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightRewinder.Structs
{
    public partial class PositionStructOperators
    {
        public static partial PositionSetStruct ToSet(PositionStruct values);
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
