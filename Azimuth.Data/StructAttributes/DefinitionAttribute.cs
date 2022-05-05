using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azimuth.StructAttributes
{
    public class DefinitionAttribute : Attribute
    {
        public string VariableName;
        public string Unit;
        public SIMCONNECT_DATATYPE DataType;
        public DefinitionAttribute(string variableName, string unit, SIMCONNECT_DATATYPE dataType)
        {
            VariableName = variableName;
            Unit = unit;
            DataType = dataType;
        }
    }
}
