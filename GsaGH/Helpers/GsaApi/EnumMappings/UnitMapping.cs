using System;
using OasysUnits.Units;

namespace GsaGH.Helpers.GsaApi.EnumMappings {
  internal static class UnitMapping {
    internal static LengthUnit GetLengthUnit(GsaAPI.LengthUnit apiLengthUnit) {
      return (LengthUnit)Enum.Parse(typeof(LengthUnit), apiLengthUnit.ToString(), ignoreCase: true);
    }

    internal static LengthUnit GetLengthUnit(GsaAPI.Model model) {
      return GetLengthUnit(model.UiUnits().LengthLarge);
    }

    internal static GsaAPI.LengthUnit GetAPILengthUnit(LengthUnit unit) {
      switch (unit) {
        case LengthUnit.Centimeter:
          return GsaAPI.LengthUnit.Centimeter;

          case LengthUnit.Millimeter:
          return GsaAPI.LengthUnit.Millimeter;

        case LengthUnit.Foot:
          return GsaAPI.LengthUnit.Foot;

        case LengthUnit.Inch:
          return GsaAPI.LengthUnit.Inch;

        default:
          return GsaAPI.LengthUnit.Meter;
      }
    }
  }
}
