using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using OasysUnits.Units;
using AccelerationUnit = GsaAPI.AccelerationUnit;
using AngleUnit = GsaAPI.AngleUnit;
using EnergyUnit = GsaAPI.EnergyUnit;
using ForceUnit = GsaAPI.ForceUnit;
using LengthUnit = GsaAPI.LengthUnit;
using MassUnit = GsaAPI.MassUnit;

namespace GsaGH.Helpers.GsaApi.EnumMappings {
  internal static class UnitMapping {

    private static readonly Dictionary<LengthUnit, OasysUnits.Units.LengthUnit> lengthUnitMapping
      = new Dictionary<LengthUnit, OasysUnits.Units.LengthUnit>() {
        {
          LengthUnit.Centimeter, OasysUnits.Units.LengthUnit.Centimeter
        }, {
          LengthUnit.Millimeter, OasysUnits.Units.LengthUnit.Millimeter
        }, {
          LengthUnit.Foot, OasysUnits.Units.LengthUnit.Foot
        }, {
          LengthUnit.Inch, OasysUnits.Units.LengthUnit.Inch
        }, {
          LengthUnit.Meter, OasysUnits.Units.LengthUnit.Meter
        },
      };

    private static readonly Dictionary<AccelerationUnit, OasysUnits.Units.AccelerationUnit>
      accelerationUnitMapping
        = new Dictionary<AccelerationUnit, OasysUnits.Units.AccelerationUnit>() {
          {
            AccelerationUnit.MeterPerSecondSquared,
            OasysUnits.Units.AccelerationUnit.MeterPerSecondSquared
          }, {
            AccelerationUnit.CentimeterPerSecondSquared,
            OasysUnits.Units.AccelerationUnit.CentimeterPerSecondSquared
          }, {
            AccelerationUnit.Gal, OasysUnits.Units.AccelerationUnit.CentimeterPerSecondSquared
          }, {
            AccelerationUnit.MillimeterPerSecondSquared,
            OasysUnits.Units.AccelerationUnit.MillimeterPerSecondSquared
          }, {
            AccelerationUnit.InchPerSecondSquared,
            OasysUnits.Units.AccelerationUnit.InchPerSecondSquared
          }, {
            AccelerationUnit.FootPerSecondSquared,
            OasysUnits.Units.AccelerationUnit.FootPerSecondSquared
          }, {
            AccelerationUnit.Gravity, OasysUnits.Units.AccelerationUnit.StandardGravity
          }, {
            AccelerationUnit.Milligravity, OasysUnits.Units.AccelerationUnit.MillistandardGravity
          },
        };

    private static readonly Dictionary<AngleUnit, OasysUnits.Units.AngleUnit> angleUnitMapping
      = new Dictionary<AngleUnit, OasysUnits.Units.AngleUnit>() {
        {
          AngleUnit.Degree, OasysUnits.Units.AngleUnit.Degree
        }, {
          AngleUnit.Gradian, OasysUnits.Units.AngleUnit.Gradian
        }, {
          AngleUnit.Radian, OasysUnits.Units.AngleUnit.Radian
        },
      };

    private static readonly Dictionary<EnergyUnit, OasysUnits.Units.EnergyUnit> energyUnitMapping
      = new Dictionary<EnergyUnit, OasysUnits.Units.EnergyUnit>() {
        {
          EnergyUnit.BritishThermalUnit, OasysUnits.Units.EnergyUnit.BritishThermalUnit
        }, {
          EnergyUnit.Calorie, OasysUnits.Units.EnergyUnit.Calorie
        }, {
          EnergyUnit.FootPound, OasysUnits.Units.EnergyUnit.FootPound
        }, {
          EnergyUnit.Gigajoule, OasysUnits.Units.EnergyUnit.Gigajoule
        }, {
          EnergyUnit.Joule, OasysUnits.Units.EnergyUnit.Joule
        }, {
          EnergyUnit.Kilojoule, OasysUnits.Units.EnergyUnit.Kilojoule
        }, {
          EnergyUnit.KilowattHour, OasysUnits.Units.EnergyUnit.KilowattHour
        }, {
          EnergyUnit.Megajoule, OasysUnits.Units.EnergyUnit.Megajoule
        },
      };

    private static readonly Dictionary<ForceUnit, OasysUnits.Units.ForceUnit> forceUnitMapping
      = new Dictionary<ForceUnit, OasysUnits.Units.ForceUnit>() {
        {
          ForceUnit.KiloNewton, OasysUnits.Units.ForceUnit.Kilonewton
        }, {
          ForceUnit.KiloPoundForce, OasysUnits.Units.ForceUnit.KilopoundForce
        }, {
          ForceUnit.MegaNewton, OasysUnits.Units.ForceUnit.Meganewton
        }, {
          ForceUnit.Newton, OasysUnits.Units.ForceUnit.Newton
        }, {
          ForceUnit.PoundForce, OasysUnits.Units.ForceUnit.PoundForce
        }, {
          ForceUnit.TonneForce, OasysUnits.Units.ForceUnit.TonneForce
        },
      };

    private static readonly Dictionary<MassUnit, OasysUnits.Units.MassUnit> massUnitMapping
      = new Dictionary<MassUnit, OasysUnits.Units.MassUnit>() {
        {
          MassUnit.Gram, OasysUnits.Units.MassUnit.Gram
        }, {
          MassUnit.Kilogram, OasysUnits.Units.MassUnit.Kilogram
        }, {
          MassUnit.Kilopound, OasysUnits.Units.MassUnit.Kilopound
        }, {
          MassUnit.Kilotonne, OasysUnits.Units.MassUnit.Kilotonne
        }, {
          MassUnit.Pound, OasysUnits.Units.MassUnit.Pound
        }, {
          MassUnit.Slug, OasysUnits.Units.MassUnit.Slug
        }, {
          MassUnit.Ton, OasysUnits.Units.MassUnit.LongTon
        }, {
          MassUnit.Tonne, OasysUnits.Units.MassUnit.Tonne
        },
      };

    private static readonly Dictionary<StressUnit, PressureUnit> stressUnitMapping
      = new Dictionary<StressUnit, PressureUnit>() {
        {
          StressUnit.Gigapascal, PressureUnit.Gigapascal
        }, {
          StressUnit.Kilopascal, PressureUnit.Kilopascal
        }, {
          StressUnit.KilopoundForcePerSquareFoot, PressureUnit.KilopoundForcePerSquareFoot
        }, {
          StressUnit.KilopoundForcePerSquareInch, PressureUnit.KilopoundForcePerSquareInch
        }, {
          StressUnit.Megapascal, PressureUnit.Megapascal
        }, {
          StressUnit.NewtonPerSquareMillimeter, PressureUnit.NewtonPerSquareMillimeter
        }, {
          StressUnit.NewtonPerSquareMeter, PressureUnit.NewtonPerSquareMeter
        }, {
          StressUnit.Pascal, PressureUnit.Pascal
        }, {
          StressUnit.PoundForcePerSquareFoot, PressureUnit.PoundForcePerSquareFoot
        }, {
          StressUnit.PoundForcePerSquareInch, PressureUnit.PoundForcePerSquareInch
        },
      };

    private static readonly Dictionary<TimeUnit, DurationUnit> timeUnitMapping
      = new Dictionary<TimeUnit, DurationUnit>() {
        {
          TimeUnit.Day, DurationUnit.Day
        }, {
          TimeUnit.Hour, DurationUnit.Hour
        }, {
          TimeUnit.Millisecond, DurationUnit.Millisecond
        }, {
          TimeUnit.Minute, DurationUnit.Minute
        }, {
          TimeUnit.Second, DurationUnit.Second
        },
      };

    private static readonly Dictionary<VelocityUnit, SpeedUnit> velocityUnitMapping
      = new Dictionary<VelocityUnit, SpeedUnit>() {
        {
          VelocityUnit.CentimeterPerSecond, SpeedUnit.CentimeterPerSecond
        }, {
          VelocityUnit.FootPerSecond, SpeedUnit.FootPerSecond
        }, {
          VelocityUnit.InchPerSecond, SpeedUnit.InchPerSecond
        }, {
          VelocityUnit.KilometerPerHour, SpeedUnit.KilometerPerHour
        }, {
          VelocityUnit.MeterPerSecond, SpeedUnit.MeterPerSecond
        }, {
          VelocityUnit.MilePerHour, SpeedUnit.MilePerHour
        }, {
          VelocityUnit.MillimeterPerSecond, SpeedUnit.MillimeterPerSecond
        },
      };

    internal static OasysUnits.Units.LengthUnit GetUnit(Model model) {
      return GetUnit(model.UiUnits().LengthLarge);
    }

    internal static OasysUnits.Units.LengthUnit GetUnit(LengthUnit apiUnit) {
      return lengthUnitMapping[apiUnit];
    }

    internal static LengthUnit GetApiUnit(OasysUnits.Units.LengthUnit unit) {
      return lengthUnitMapping.FirstOrDefault(x => x.Value == unit).Key;
    }

    internal static OasysUnits.Units.AccelerationUnit GetUnit(AccelerationUnit apiUnit) {
      return accelerationUnitMapping[apiUnit];
    }

    internal static AccelerationUnit GetApiUnit(OasysUnits.Units.AccelerationUnit unit) {
      return accelerationUnitMapping.FirstOrDefault(x => x.Value == unit).Key;
    }

    internal static OasysUnits.Units.AngleUnit GetUnit(AngleUnit apiUnit) {
      return angleUnitMapping[apiUnit];
    }

    internal static AngleUnit GetApiUnit(OasysUnits.Units.AngleUnit unit) {
      return angleUnitMapping.FirstOrDefault(x => x.Value == unit).Key;
    }

    internal static OasysUnits.Units.EnergyUnit GetUnit(EnergyUnit apiUnit) {
      return energyUnitMapping[apiUnit];
    }

    internal static EnergyUnit GetApiUnit(OasysUnits.Units.EnergyUnit unit) {
      return energyUnitMapping.FirstOrDefault(x => x.Value == unit).Key;
    }

    internal static OasysUnits.Units.ForceUnit GetUnit(ForceUnit apiUnit) {
      return forceUnitMapping[apiUnit];
    }

    internal static ForceUnit GetApiUnit(OasysUnits.Units.ForceUnit unit) {
      return forceUnitMapping.FirstOrDefault(x => x.Value == unit).Key;
    }

    internal static OasysUnits.Units.MassUnit GetUnit(MassUnit apiUnit) {
      return massUnitMapping[apiUnit];
    }

    internal static MassUnit GetApiUnit(OasysUnits.Units.MassUnit unit) {
      return massUnitMapping.FirstOrDefault(x => x.Value == unit).Key;
    }

    internal static PressureUnit GetUnit(StressUnit apiUnit) {
      return stressUnitMapping[apiUnit];
    }

    internal static StressUnit GetApiUnit(PressureUnit unit) {
      return stressUnitMapping.FirstOrDefault(x => x.Value == unit).Key;
    }

    internal static DurationUnit GetUnit(TimeUnit apiUnit) {
      return timeUnitMapping[apiUnit];
    }

    internal static TimeUnit GetApiUnit(DurationUnit unit) {
      return timeUnitMapping.FirstOrDefault(x => x.Value == unit).Key;
    }

    internal static SpeedUnit GetUnit(VelocityUnit apiUnit) {
      return velocityUnitMapping[apiUnit];
    }

    internal static VelocityUnit GetApiUnit(SpeedUnit unit) {
      return velocityUnitMapping.FirstOrDefault(x => x.Value == unit).Key;
    }
  }
}
