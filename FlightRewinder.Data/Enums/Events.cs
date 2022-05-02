using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightRewinder.Data.Enums
{
    public enum Events : uint
    {
        FRAME,
        InputEventRewindDown,
        InputEventRewindUp,
        Unused = uint.MaxValue
    }
}
