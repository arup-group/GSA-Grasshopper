using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.Import;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters {
  public class GsaGridLine {
    private GridLine _gridLine;

    public GsaGridLine() { }

    internal GsaGridLine(GridLine gridLine) {
      _gridLine = gridLine;
    }

    //internal EntityList GetApiList() {
    //  return new EntityList {
    //    Name = Name,
    //    Definition = Definition,
    //    Type = GetAPIEntityType(EntityType)
    //  };
    //}

    public GsaGridLine Duplicate() {
      var dup = new GsaGridLine {
        _gridLine = _gridLine
      };

      return dup;
    }

    public override string ToString() {
      return "";
    }
  }
}
