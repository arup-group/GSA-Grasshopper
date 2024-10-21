using System.Collections.Generic;
using System.Linq;

using OasysUnits.Units;

namespace GsaGH.Helpers.GsaApi.EnumMappings {
  internal static class UnitMapping {
    internal static LengthUnit GetUnit(GsaAPI.Model model) {
      return GetUnit(model.UiUnits().LengthLarge);
    }
    internal static LengthUnit GetUnit(GsaAPI.LengthUnit apiUnit) {
      return lengthUnitMapping[apiUnit];
    }
    internal static GsaAPI.LengthUnit GetApiUnit(LengthUnit unit) {
      return lengthUnitMapping.FirstOrDefault(x => x.Value == unit).Key;
    }

    internal static AccelerationUnit GetUnit(GsaAPI.AccelerationUnit apiUnit) {
      return accelerationUnitMapping[apiUnit];
    }
    internal static GsaAPI.AccelerationUnit GetApiUnit(AccelerationUnit unit) {
      return accelerationUnitMapping.FirstOrDefault(x => x.Value == unit).Key;
    }

    internal static AngleUnit GetUnit(GsaAPI.AngleUnit apiUnit) {
      return angleUnitMapping[apiUnit];
    }
    internal static GsaAPI.AngleUnit GetApiUnit(AngleUnit unit) {
      return angleUnitMapping.FirstOrDefault(x => x.Value == unit).Key;
    }

    internal static EnergyUnit GetUnit(GsaAPI.EnergyUnit apiUnit) {
      return energyUnitMapping[apiUnit];
    }
    internal static GsaAPI.EnergyUnit GetApiUnit(EnergyUnit unit) {
      return energyUnitMapping.FirstOrDefault(x => x.Value == unit).Key;
    }

    internal static ForceUnit GetUnit(GsaAPI.ForceUnit apiUnit) {
      return forceUnitMapping[apiUnit];
    }
    internal static GsaAPI.ForceUnit GetApiUnit(ForceUnit unit) {
      return forceUnitMapping.FirstOrDefault(x => x.Value == unit).Key;
    }

    internal static MassUnit GetUnit(GsaAPI.MassUnit apiUnit) {
      return massUnitMapping[apiUnit];
    }
    internal static GsaAPI.MassUnit GetApiUnit(MassUnit unit) {
      return massUnitMapping.FirstOrDefault(x => x.Value == unit).Key;
    }

    internal static PressureUnit GetUnit(GsaAPI.StressUnit apiUnit) {
      return stressUnitMapping[apiUnit];
    }
    internal static GsaAPI.StressUnit GetApiUnit(PressureUnit unit) {
      return stressUnitMapping.FirstOrDefault(x => x.Value == unit).Key;
    }

    internal static DurationUnit GetUnit(GsaAPI.TimeUnit apiUnit) {
      return timeUnitMapping[apiUnit];
    }
    internal static GsaAPI.TimeUnit GetApiUnit(DurationUnit unit) {
      return timeUnitMapping.FirstOrDefault(x => x.Value == unit).Key;
    }

    internal static SpeedUnit GetUnit(GsaAPI.VelocityUnit apiUnit) {
      return velocityUnitMapping[apiUnit];
    }
    internal static GsaAPI.VelocityUnit GetApiUnit(SpeedUnit unit) {
      return velocityUnitMapping.FirstOrDefault(x => x.Value == unit).Key;
    }

    private static readonly Dictionary<GsaAPI.LengthUnit, LengthUnit> lengthUnitMapping
      = new Dictionary<GsaAPI.LengthUnit, LengthUnit>() {
        {
          GsaAPI.LengthUnit.Centimeter, LengthUnit.Centimeter
        }, {
          GsaAPI.LengthUnit.Millimeter, LengthUnit.Millimeter
        }, {
          GsaAPI.LengthUnit.Foot, LengthUnit.Foot
        }, {
          GsaAPI.LengthUnit.Inch, LengthUnit.Inch
        }, {
          GsaAPI.LengthUnit.Meter, LengthUnit.Meter
        }
      };

    private static readonly Dictionary<GsaAPI.AccelerationUnit, AccelerationUnit> accelerationUnitMapping
      = new Dictionary<GsaAPI.AccelerationUnit, AccelerationUnit>() {
        {
           GsaAPI.AccelerationUnit.MeterPerSecondSquared, AccelerationUnit.MeterPerSecondSquared
        }, {
           GsaAPI.AccelerationUnit.CentimeterPerSecondSquared, AccelerationUnit.CentimeterPerSecondSquared
        }, {
           GsaAPI.AccelerationUnit.Gal, AccelerationUnit.CentimeterPerSecondSquared
        }, {
           GsaAPI.AccelerationUnit.MillimeterPerSecondSquared, AccelerationUnit.MillimeterPerSecondSquared
        }, {
           GsaAPI.AccelerationUnit.InchPerSecondSquared,  AccelerationUnit.InchPerSecondSquared
        }, {
           GsaAPI.AccelerationUnit.FootPerSecondSquared, AccelerationUnit.FootPerSecondSquared
        }, {
           GsaAPI.AccelerationUnit.Gravity, AccelerationUnit.StandardGravity
        }, {
           GsaAPI.AccelerationUnit.Milligravity, AccelerationUnit.MillistandardGravity
        }
      };

    private static readonly Dictionary<GsaAPI.AngleUnit, AngleUnit> angleUnitMapping
      = new Dictionary<GsaAPI.AngleUnit, AngleUnit>() {
        {
           GsaAPI.AngleUnit.Degree, AngleUnit.Degree
        }, {
           GsaAPI.AngleUnit.Gradian, AngleUnit.Gradian
        }, {
           GsaAPI.AngleUnit.Radian, AngleUnit.Radian
        }
      };

    private static readonly Dictionary<GsaAPI.EnergyUnit, EnergyUnit> energyUnitMapping
     = new Dictionary<GsaAPI.EnergyUnit, EnergyUnit>() {
        {
           GsaAPI.EnergyUnit.BritishThermalUnit, EnergyUnit.BritishThermalUnit
        }, {
           GsaAPI.EnergyUnit.Calorie, EnergyUnit.Calorie
        }, {
           GsaAPI.EnergyUnit.FootPound, EnergyUnit.FootPound
        }, {
           GsaAPI.EnergyUnit.Gigajoule, EnergyUnit.Gigajoule
        }, {
           GsaAPI.EnergyUnit.Joule, EnergyUnit.Joule
        }, {
           GsaAPI.EnergyUnit.Kilojoule, EnergyUnit.Kilojoule
        }, {
           GsaAPI.EnergyUnit.KilowattHour, EnergyUnit.KilowattHour
        }, {
           GsaAPI.EnergyUnit.Megajoule, EnergyUnit.Megajoule
        }
     };

    private static readonly Dictionary<GsaAPI.ForceUnit, ForceUnit> forceUnitMapping
     = new Dictionary<GsaAPI.ForceUnit, ForceUnit>() {
        {
           GsaAPI.ForceUnit.KiloNewton, ForceUnit.Kilonewton
        }, {
           GsaAPI.ForceUnit.KiloPoundForce, ForceUnit.KilopoundForce
        }, {
           GsaAPI.ForceUnit.MegaNewton, ForceUnit.Meganewton
        }, {
           GsaAPI.ForceUnit.Newton, ForceUnit.Newton
        }, {
           GsaAPI.ForceUnit.PoundForce, ForceUnit.PoundForce
        }, {
           GsaAPI.ForceUnit.TonneForce, ForceUnit.TonneForce
        }
     };

    private static readonly Dictionary<GsaAPI.MassUnit, MassUnit> massUnitMapping
     = new Dictionary<GsaAPI.MassUnit, MassUnit>() {
        {
           GsaAPI.MassUnit.Gram, MassUnit.Gram
        }, {
           GsaAPI.MassUnit.Kilogram, MassUnit.Kilogram
        }, {
           GsaAPI.MassUnit.Kilopound, MassUnit.Kilopound
        }, {
           GsaAPI.MassUnit.Kilotonne, MassUnit.Kilotonne
        }, {
           GsaAPI.MassUnit.Pound, MassUnit.Pound
        }, {
           GsaAPI.MassUnit.Slug, MassUnit.Slug
        }, {
           GsaAPI.MassUnit.Ton, MassUnit.LongTon
        }, {
           GsaAPI.MassUnit.Tonne, MassUnit.Tonne
        }
     };

    private static readonly Dictionary<GsaAPI.StressUnit, PressureUnit> stressUnitMapping
     = new Dictionary<GsaAPI.StressUnit, PressureUnit>() {
        {
           GsaAPI.StressUnit.Gigapascal, PressureUnit.Gigapascal
        }, {
           GsaAPI.StressUnit.Kilopascal, PressureUnit.Kilopascal
        }, {
           GsaAPI.StressUnit.KilopoundForcePerSquareFoot, PressureUnit.KilopoundForcePerSquareFoot
        }, {
           GsaAPI.StressUnit.KilopoundForcePerSquareInch, PressureUnit.KilopoundForcePerSquareInch
        }, {
           GsaAPI.StressUnit.Megapascal, PressureUnit.Megapascal
        }, {
           GsaAPI.StressUnit.NewtonPerSquareMillimeter, PressureUnit.NewtonPerSquareMillimeter
        }, {
           GsaAPI.StressUnit.NewtonPerSquareMeter, PressureUnit.NewtonPerSquareMeter
        }, {
           GsaAPI.StressUnit.Pascal, PressureUnit.Pascal
        }, {
           GsaAPI.StressUnit.PoundForcePerSquareFoot, PressureUnit.PoundForcePerSquareFoot
        }, {
           GsaAPI.StressUnit.PoundForcePerSquareInch, PressureUnit.PoundForcePerSquareInch
        }
     };

    private static readonly Dictionary<GsaAPI.TimeUnit, DurationUnit> timeUnitMapping
     = new Dictionary<GsaAPI.TimeUnit, DurationUnit>() {
        {
           GsaAPI.TimeUnit.Day, DurationUnit.Day
        }, {
           GsaAPI.TimeUnit.Hour, DurationUnit.Hour
        }, {
           GsaAPI.TimeUnit.Millisecond, DurationUnit.Millisecond
        }, {
           GsaAPI.TimeUnit.Minute, DurationUnit.Minute
        }, {
           GsaAPI.TimeUnit.Second, DurationUnit.Second
      }
     };

    private static readonly Dictionary<GsaAPI.VelocityUnit, SpeedUnit> velocityUnitMapping
     = new Dictionary<GsaAPI.VelocityUnit, SpeedUnit>() {
        {
           GsaAPI.VelocityUnit.CentimeterPerSecond, SpeedUnit.CentimeterPerSecond
        }, {
           GsaAPI.VelocityUnit.FootPerSecond, SpeedUnit.FootPerSecond
        }, {
           GsaAPI.VelocityUnit.InchPerSecond, SpeedUnit.InchPerSecond
        }, {
           GsaAPI.VelocityUnit.KilometerPerHour, SpeedUnit.KilometerPerHour
        }, {
           GsaAPI.VelocityUnit.MeterPerSecond, SpeedUnit.MeterPerSecond
        }, {
           GsaAPI.VelocityUnit.MilePerHour, SpeedUnit.MilePerHour
        }, {
           GsaAPI.VelocityUnit.MillimeterPerSecond, SpeedUnit.MillimeterPerSecond
    }
     };
  }
}
