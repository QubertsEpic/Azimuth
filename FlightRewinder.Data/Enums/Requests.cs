﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightRewinder.Enums
{
    public enum Requests : uint
    {
        PlaneLocation = 0,
        RemovePlane = 1,
        CreatePlane = 10000,
    }
}
