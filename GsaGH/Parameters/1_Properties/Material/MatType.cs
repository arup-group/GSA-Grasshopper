﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsaGH.Parameters {
  public enum MatType {
    Undef = -2,
    None = -1,
    Generic = 0,
    Steel = 1,
    Concrete = 2,
    Aluminium = 3,
    Glass = 4,
    Frp = 5,
    Rebar = 6,
    Timber = 7,
    Fabric = 8,
    Soil = 9,
    NumMt = 10,
    Compound = 0x100,
    Bar = 0x1000,
    Tendon = 4352,
    Frpbar = 4608,
    Cfrp = 4864,
    Gfrp = 5120,
    Afrp = 5376,
    Argfrp = 5632,
    Barmat = 65280,
  }
}
