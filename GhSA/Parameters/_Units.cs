using System;
using System.Collections.Generic;
using System.Linq;
using Rhino;
using UnitsNet;
using UnitsNet.Units;
using Oasys.Units;

namespace GsaGH
{
  /// <summary>
  /// Class to hold units used in Grasshopper GSA file. 
  /// </summary>
  public static class Units
  {
    public enum GsaUnits
    {
      Length,
      Area,
      AreaMomentOfInertia,
      Force,
      Moment,
      Stress,
      Strain,
      Temperature,
      Mass,
      Density,
      LinearDensity,
      Velocity,
      Acceleration,
      Ratio
    }

    internal static List<string> FilteredAngleUnits = new List<string>()
    {
      AngleUnit.Radian.ToString(),
      AngleUnit.Degree.ToString()
    };
    #region lengths
    internal static List<string> FilteredLengthUnits = new List<string>()
    {
      LengthUnit.Millimeter.ToString(),
      LengthUnit.Centimeter.ToString(),
      LengthUnit.Meter.ToString(),
      LengthUnit.Inch.ToString(),
      LengthUnit.Foot.ToString()
    };

    internal static List<string> FilteredAreaMomentOfInertiaUnits = new List<string>()
    {
      AreaMomentOfInertiaUnit.MillimeterToTheFourth.ToString(),
      AreaMomentOfInertiaUnit.CentimeterToTheFourth.ToString(),
      AreaMomentOfInertiaUnit.MeterToTheFourth.ToString(),
      AreaMomentOfInertiaUnit.InchToTheFourth.ToString(),
      AreaMomentOfInertiaUnit.FootToTheFourth.ToString()
    };

    public static Length Tolerance
    {
      get
      {
        if (useRhinoTolerance)
          return GetRhinoTolerance();
        else
          return m_tolerance;
      }
      set { m_tolerance = value; }
    }
    private static Length m_tolerance = new Length(1, LengthUnit.Centimeter);
    internal static bool useRhinoTolerance;
    public static int SignificantDigits
    {
      get { return BitConverter.GetBytes(decimal.GetBits((decimal)m_tolerance.As(LengthUnitGeometry))[3])[2]; ; }
    }

    public static LengthUnit LengthUnitGeometry
    {
      get
      {
        if (useRhinoLengthGeometryUnit)
          m_length_geometry = GetRhinoLengthUnit(RhinoDoc.ActiveDoc.ModelUnitSystem);
        return m_length_geometry;
      }
      set
      {
        useRhinoLengthGeometryUnit = false;
        m_length_geometry = value;
      }
    }
    private static LengthUnit m_length_geometry = LengthUnit.Meter;
    internal static bool useRhinoLengthGeometryUnit;

    public static LengthUnit LengthUnitSection
    {
      get
      {
        return m_length_section;
      }
      set
      {
        m_length_section = value;
      }
    }
    private static LengthUnit m_length_section = LengthUnit.Centimeter;

    internal static AreaMomentOfInertiaUnit SectionAreaMomentOfInertiaUnit = GetAreaMomentOfInertiaUnit(LengthUnitSection);
    internal static AreaMomentOfInertiaUnit GetAreaMomentOfInertiaUnit(LengthUnit unit)
    {
      switch (unit)
      {
        case LengthUnit.Millimeter:
          return AreaMomentOfInertiaUnit.MillimeterToTheFourth;
        case LengthUnit.Centimeter:
          return AreaMomentOfInertiaUnit.CentimeterToTheFourth;
        case LengthUnit.Meter:
          return AreaMomentOfInertiaUnit.MeterToTheFourth;
        case LengthUnit.Foot:
          return AreaMomentOfInertiaUnit.FootToTheFourth;
        case LengthUnit.Inch:
          return AreaMomentOfInertiaUnit.InchToTheFourth;
      }
      // fallback:
      BaseUnits baseUnits = new BaseUnits(unit, SI.Mass, SI.Time, SI.Current, SI.Temperature, SI.Amount, SI.LuminousIntensity);
      UnitsNet.UnitSystem unitSystem = new UnitsNet.UnitSystem(baseUnits);
      return new AreaMomentOfInertia(1, unitSystem).Unit;
    }

    internal static AreaUnit SectionAreaUnit = GetAreaUnit(LengthUnitSection);
    internal static AreaUnit GetAreaUnit(LengthUnit unit)
    {
      switch (unit)
      {
        case LengthUnit.Millimeter:
          return AreaUnit.SquareMillimeter;
        case LengthUnit.Centimeter:
          return AreaUnit.SquareCentimeter;
        case LengthUnit.Meter:
          return AreaUnit.SquareMeter;
        case LengthUnit.Foot:
          return AreaUnit.SquareFoot;
        case LengthUnit.Inch:
          return AreaUnit.SquareInch;
      }
      // fallback:
      BaseUnits baseUnits = new BaseUnits(unit, SI.Mass, SI.Time, SI.Current, SI.Temperature, SI.Amount, SI.LuminousIntensity);
      UnitsNet.UnitSystem unitSystem = new UnitsNet.UnitSystem(baseUnits);
      return new Area(1, unitSystem).Unit;
    }

    public static VolumePerLengthUnit VolumePerLengthUnit
    {
      get
      {
        switch (m_length_section)
        {
          case LengthUnit.Yard:
          case LengthUnit.Inch:
          case LengthUnit.Foot:
            return VolumePerLengthUnit.CubicYardPerFoot;
          default:
            return VolumePerLengthUnit.CubicMeterPerMeter;
        }
      }
    }
    internal static VolumePerLengthUnit GetVolumePerLengthUnit(LengthUnit unit)
    {
      switch (unit)
      {
        case LengthUnit.Millimeter:
        case LengthUnit.Centimeter:
        case LengthUnit.Meter:
          return VolumePerLengthUnit.CubicMeterPerMeter;
        case LengthUnit.Foot:
        case LengthUnit.Inch:
          return VolumePerLengthUnit.CubicYardPerFoot;
      }
      // fallback:
      BaseUnits baseUnits = new BaseUnits(unit, SI.Mass, SI.Time, SI.Current, SI.Temperature, SI.Amount, SI.LuminousIntensity);
      UnitsNet.UnitSystem unitSystem = new UnitsNet.UnitSystem(baseUnits);
      return new VolumePerLength(1, unitSystem).Unit;
    }
    public static LengthUnit LengthUnitResult
    {
      get
      {
        return m_length_result;
      }
      set
      {
        m_length_result = value;
      }
    }
    private static LengthUnit m_length_result = LengthUnit.Millimeter;

    #endregion

    #region force
    public static ForceUnit ForceUnit
    {
      get
      {
        return m_force;
      }
      set { m_force = value; }
    }
    private static ForceUnit m_force = ForceUnit.Kilonewton;
    internal static List<string> FilteredForceUnits = new List<string>()
    {
      ForceUnit.Newton.ToString(),
      ForceUnit.Kilonewton.ToString(),
      ForceUnit.Meganewton.ToString(),
      ForceUnit.PoundForce.ToString(),
      ForceUnit.KilopoundForce.ToString(),
      ForceUnit.TonneForce.ToString()
    };

    public static ForcePerLengthUnit ForcePerLengthUnit
    {
      get
      {
        switch (m_force)
        {
          case ForceUnit.Newton:
            switch (m_length_geometry)
            {
              case LengthUnit.Millimeter:
                return ForcePerLengthUnit.NewtonPerMillimeter;
              case LengthUnit.Centimeter:
                return ForcePerLengthUnit.NewtonPerCentimeter;
              case LengthUnit.Meter:
                return ForcePerLengthUnit.NewtonPerMeter;
            }
            break;
          case ForceUnit.Kilonewton:
            switch (m_length_geometry)
            {
              case LengthUnit.Millimeter:
                return ForcePerLengthUnit.KilonewtonPerMillimeter;
              case LengthUnit.Centimeter:
                return ForcePerLengthUnit.KilonewtonPerCentimeter;
              case LengthUnit.Meter:
                return ForcePerLengthUnit.KilonewtonPerMeter;
            }
            break;
          case ForceUnit.Meganewton:
            switch (m_length_geometry)
            {
              case LengthUnit.Millimeter:
                return ForcePerLengthUnit.MeganewtonPerMillimeter;
              case LengthUnit.Centimeter:
                return ForcePerLengthUnit.MeganewtonPerCentimeter;
              case LengthUnit.Meter:
                return ForcePerLengthUnit.MeganewtonPerMeter;
            }
            break;
          case ForceUnit.KilopoundForce:
            switch (m_length_geometry)
            {
              case LengthUnit.Inch:
                return ForcePerLengthUnit.KilopoundForcePerInch;
              case LengthUnit.Foot:
                return ForcePerLengthUnit.KilopoundForcePerFoot;
            }
            break;
          case ForceUnit.PoundForce:
            switch (m_length_geometry)
            {
              case LengthUnit.Inch:
                return ForcePerLengthUnit.PoundForcePerInch;
              case LengthUnit.Foot:
                return ForcePerLengthUnit.PoundForcePerFoot;
            }
            break;
        }

        Force force = Force.From(1, ForceUnit);
        Length length = Length.From(1, LengthUnitGeometry);
        ForcePerLength kNperM = force / length;

        return kNperM.Unit;
      }
    }

    internal static List<string> FilteredForcePerLengthUnits = new List<string>()
    {
      ForcePerLengthUnit.NewtonPerMillimeter.ToString(),
      ForcePerLengthUnit.NewtonPerCentimeter.ToString(),
      ForcePerLengthUnit.NewtonPerMeter.ToString(),

      ForcePerLengthUnit.KilonewtonPerMillimeter.ToString(),
      ForcePerLengthUnit.KilonewtonPerCentimeter.ToString(),
      ForcePerLengthUnit.KilonewtonPerMeter.ToString(),

      ForcePerLengthUnit.TonneForcePerCentimeter.ToString(),
      ForcePerLengthUnit.TonneForcePerMeter.ToString(),
      ForcePerLengthUnit.TonneForcePerMillimeter.ToString(),

      ForcePerLengthUnit.MeganewtonPerMeter.ToString(),

      ForcePerLengthUnit.PoundForcePerInch.ToString(),
      ForcePerLengthUnit.PoundForcePerFoot.ToString(),
      ForcePerLengthUnit.PoundForcePerYard.ToString(),

      ForcePerLengthUnit.KilopoundForcePerInch.ToString(),
      ForcePerLengthUnit.KilopoundForcePerFoot.ToString()
    };
    #endregion

    #region moment
    public static MomentUnit MomentUnit
    {
      get
      {
        return m_moment;
      }
      set { m_moment = value; }
    }
    private static MomentUnit m_moment = MomentUnit.KilonewtonMeter;

    internal static List<string> FilteredMomentUnits = Enum.GetNames(typeof(MomentUnit)).Skip(1).ToList();
    #endregion

    #region stress
    public static PressureUnit StressUnit
    {
      get
      {
        return m_stress;
      }
      set { m_stress = value; }
    }
    private static PressureUnit m_stress = PressureUnit.Megapascal;

    internal static List<string> FilteredStressUnits = new List<string>()
    {
      PressureUnit.Pascal.ToString(),
      PressureUnit.Kilopascal.ToString(),
      PressureUnit.Megapascal.ToString(),
      PressureUnit.Gigapascal.ToString(),
      PressureUnit.NewtonPerSquareMillimeter.ToString(),
      PressureUnit.NewtonPerSquareMeter.ToString(),
      PressureUnit.PoundForcePerSquareInch.ToString(),
      PressureUnit.PoundForcePerSquareFoot.ToString(),
      PressureUnit.KilopoundForcePerSquareInch.ToString()
    };
    internal static List<string> FilteredForcePerAreaUnits = new List<string>()
    {
      PressureUnit.NewtonPerSquareMillimeter.ToString(),
      PressureUnit.NewtonPerSquareCentimeter.ToString(),
      PressureUnit.NewtonPerSquareMeter.ToString(),
      PressureUnit.KilonewtonPerSquareCentimeter.ToString(),
      PressureUnit.KilonewtonPerSquareMillimeter.ToString(),
      PressureUnit.KilonewtonPerSquareMeter.ToString(),
      PressureUnit.PoundForcePerSquareInch.ToString(),
      PressureUnit.PoundForcePerSquareFoot.ToString(),
      PressureUnit.KilopoundForcePerSquareInch.ToString(),
    };
    #endregion

    #region strain
    public static StrainUnit StrainUnit
    {
      get
      {
        return m_strain;
      }
      set { m_strain = value; }
    }
    private static StrainUnit m_strain = StrainUnit.MilliStrain;

    internal static List<string> FilteredStrainUnits = new List<string>()
    {
      StrainUnit.Ratio.ToString(),
      StrainUnit.Percent.ToString(),
      StrainUnit.MilliStrain.ToString(),
      StrainUnit.MicroStrain.ToString()
    };
    #endregion

    #region axial stiffness
    public static AxialStiffnessUnit AxialStiffnessUnit
    {
      get
      {
        return m_axialstiffness;
      }
      set { m_axialstiffness = value; }
    }
    private static AxialStiffnessUnit m_axialstiffness = AxialStiffnessUnit.Kilonewton;

    internal static List<string> FilteredAxialStiffnessUnits = Enum.GetNames(typeof(AxialStiffnessUnit)).Skip(1).ToList();
    #endregion

    #region bending stiffness
    public static BendingStiffnessUnit BendingStiffnessUnit
    {
      get
      {
        return m_bendingstiffness;
      }
      set { m_bendingstiffness = value; }
    }
    private static BendingStiffnessUnit m_bendingstiffness = BendingStiffnessUnit.KilonewtonSquareMeter;

    internal static List<string> FilteredBendingStiffnessUnits = Enum.GetNames(typeof(BendingStiffnessUnit)).Skip(1).ToList();
    #endregion

    #region curvature
    public static CurvatureUnit CurvatureUnit
    {
      get
      {
        return m_curvature;
      }
      set { m_curvature = value; }
    }
    private static CurvatureUnit m_curvature = (CurvatureUnit)Enum.Parse(typeof(CurvatureUnit), "Per" + LengthUnitGeometry.ToString());

    internal static List<string> FilteredCurvatureUnits = new List<string>()
    {
      CurvatureUnit.PerMillimeter.ToString(),
      CurvatureUnit.PerCentimeter.ToString(),
      CurvatureUnit.PerMeter.ToString(),
      CurvatureUnit.PerInch.ToString(),
      CurvatureUnit.PerFoot.ToString()
    };
    #endregion

    #region mass
    public static MassUnit MassUnit
    {
      get
      {
        return m_mass;
      }
      set { m_mass = value; }
    }
    private static MassUnit m_mass = MassUnit.Tonne;

    internal static List<string> FilteredMassUnits = new List<string>()
    {
      MassUnit.Gram.ToString(),
      MassUnit.Kilogram.ToString(),
      MassUnit.Tonne.ToString(),
      MassUnit.Kilotonne.ToString(),
      MassUnit.Pound.ToString(),
      MassUnit.Kilopound.ToString(),
      MassUnit.LongTon.ToString(),
      MassUnit.Slug.ToString()
    };
    #endregion

    #region density
    public static DensityUnit DensityUnit
    {
      get
      {
        Mass mass = Mass.From(1, MassUnit);
        Length len = Length.From(1, LengthUnitGeometry);
        Volume vol = len * len * len;

        Density density = mass / vol;
        return density.Unit;
      }
    }
    internal static List<string> FilteredDensityUnits = new List<string>()
    {
      DensityUnit.GramPerCubicMillimeter.ToString(),
      DensityUnit.GramPerCubicCentimeter.ToString(),
      DensityUnit.GramPerCubicMeter.ToString(),
      DensityUnit.KilogramPerCubicMillimeter.ToString(),
      DensityUnit.KilogramPerCubicCentimeter.ToString(),
      DensityUnit.KilogramPerCubicMeter.ToString(),
      DensityUnit.TonnePerCubicMillimeter.ToString(),
      DensityUnit.TonnePerCubicCentimeter.ToString(),
      DensityUnit.TonnePerCubicMeter.ToString(),
      DensityUnit.PoundPerCubicFoot.ToString(),
      DensityUnit.PoundPerCubicInch.ToString(),
      DensityUnit.KilopoundPerCubicFoot.ToString(),
      DensityUnit.KilopoundPerCubicInch.ToString(),
    };
    public static LinearDensityUnit LinearDensityUnit
    {
      get
      {
        switch (m_mass)
        {
          case MassUnit.Kilogram:
            switch (m_length_geometry)
            {
              case LengthUnit.Millimeter:
                return LinearDensityUnit.KilogramPerMillimeter;
              case LengthUnit.Centimeter:
                return LinearDensityUnit.KilogramPerCentimeter;
              case LengthUnit.Meter:
                return LinearDensityUnit.KilogramPerMeter;
            }
            break;
          case MassUnit.Pound:
            switch (m_length_geometry)
            {
              case LengthUnit.Foot:
                return LinearDensityUnit.PoundPerFoot;
              case LengthUnit.Inch:
                return LinearDensityUnit.PoundPerInch;
            }
            break;
        }
        return LinearDensityUnit.KilogramPerMeter;
      }
    }
    internal static List<string> FilteredLinearDensityUnitUnits = new List<string>()
    {
      LinearDensityUnit.KilogramPerMillimeter.ToString(),
      LinearDensityUnit.KilogramPerCentimeter.ToString(),
      LinearDensityUnit.KilogramPerMeter.ToString(),
      LinearDensityUnit.PoundPerFoot.ToString(),
      LinearDensityUnit.PoundPerInch.ToString(),
    };
    #endregion

    #region temperature
    public static TemperatureUnit TemperatureUnit
    {
      get
      {
        return m_temperature;
      }
      set { m_temperature = value; }
    }
    private static TemperatureUnit m_temperature = TemperatureUnit.DegreeCelsius;

    internal static List<string> FilteredTemperatureUnits = new List<string>()
    {
      TemperatureUnit.DegreeCelsius.ToString(),
      TemperatureUnit.Kelvin.ToString(),
      TemperatureUnit.DegreeFahrenheit.ToString(),
    };

    internal static CoefficientOfThermalExpansionUnit CoefficientOfThermalExpansionUnit
    {
      get
      {
        switch (TemperatureUnit)
        {
          case TemperatureUnit.DegreeCelsius:
            return CoefficientOfThermalExpansionUnit.InverseDegreeCelsius;
          case TemperatureUnit.Kelvin:
            return CoefficientOfThermalExpansionUnit.InverseKelvin;
          case TemperatureUnit.DegreeFahrenheit:
            return CoefficientOfThermalExpansionUnit.InverseDegreeFahrenheit;
          default:
            return CoefficientOfThermalExpansionUnit.Undefined;
        }
      }
    }
    #endregion

    #region velocity
    public static SpeedUnit VelocityUnit
    {
      get
      {
        return m_velocity;
      }
      set { m_velocity = value; }
    }
    private static SpeedUnit m_velocity = SpeedUnit.MeterPerSecond;

    internal static List<string> FilteredVelocityUnits = new List<string>()
    {
      SpeedUnit.MillimeterPerSecond.ToString(),
      SpeedUnit.CentimeterPerSecond.ToString(),
      SpeedUnit.MeterPerSecond.ToString(),
      SpeedUnit.FootPerSecond.ToString(),
      SpeedUnit.InchPerSecond.ToString(),
      SpeedUnit.KilometerPerHour.ToString(),
      SpeedUnit.MilePerHour.ToString(),
    };
    #endregion

    #region acceleration
    public static AccelerationUnit AccelerationUnit
    {
      get
      {
        return m_acceleration;
      }
      set { m_acceleration = value; }
    }
    private static AccelerationUnit m_acceleration = AccelerationUnit.MeterPerSecondSquared;

    internal static List<string> FilteredAccelerationUnits = new List<string>()
    {
      AccelerationUnit.MillimeterPerSecondSquared.ToString(),
      AccelerationUnit.CentimeterPerSecondSquared.ToString(),
      AccelerationUnit.MeterPerSecondSquared.ToString(),
      AccelerationUnit.KilometerPerSecondSquared.ToString(),
      AccelerationUnit.FootPerSecondSquared.ToString(),
      AccelerationUnit.InchPerSecondSquared.ToString(),
    };
    #endregion

    #region energy
    public static EnergyUnit EnergyUnit
    {
      get
      {
        return m_energy;
      }
      set { m_energy = value; }
    }
    private static EnergyUnit m_energy = EnergyUnit.Megajoule;

    internal static List<string> FilteredEnergyUnits = new List<string>()
    {
      EnergyUnit.Joule.ToString(),
      EnergyUnit.Kilojoule.ToString(),
      EnergyUnit.Megajoule.ToString(),
      EnergyUnit.Gigajoule.ToString(),
      EnergyUnit.KilowattHour.ToString(),
      EnergyUnit.FootPound.ToString(),
      EnergyUnit.Calorie.ToString(),
      EnergyUnit.BritishThermalUnit.ToString(),
    };
    #endregion

    #region time
    internal static List<string> FilteredTimeUnits = new List<string>()
    {
      DurationUnit.Millisecond.ToString(),
      DurationUnit.Second.ToString(),
      DurationUnit.Minute.ToString(),
      DurationUnit.Hour.ToString(),
      DurationUnit.Day.ToString(),
    };
    public static DurationUnit TimeShortUnit
    {
      get
      {
        return m_time_short;
      }
      set { m_time_short = value; }
    }
    private static DurationUnit m_time_short = DurationUnit.Second;
    public static DurationUnit TimeMediumUnit
    {
      get
      {
        return m_time_medium;
      }
      set { m_time_medium = value; }
    }
    private static DurationUnit m_time_medium = DurationUnit.Minute;
    public static DurationUnit TimeLongUnit
    {
      get
      {
        return m_time_long;
      }
      set { m_time_long = value; }
    }
    private static DurationUnit m_time_long = DurationUnit.Day;

    #endregion

    private static BaseUnits SI = UnitsNet.UnitSystem.SI.BaseUnits;

    #region methods
    public static void SetupUnitsDuringLoad(bool headless = false)
    {
      bool settingsExist = ReadSettings();
      if (!settingsExist)
      {
        // get rhino document length unit
        if (headless)
          m_length_geometry = LengthUnit.Meter;
        else
          m_length_geometry = GetRhinoLengthUnit(RhinoDoc.ActiveDoc.ModelUnitSystem);
        m_length_section = LengthUnit.Centimeter;
        m_length_result = LengthUnit.Millimeter;

        SaveSettings();
      }
    }
    internal static void SaveSettings()
    {
      Grasshopper.Instances.Settings.SetValue("GsaLengthUnitGeometry", LengthUnitGeometry.ToString());
      Grasshopper.Instances.Settings.SetValue("GsaUseRhinoLengthGeometryUnit", useRhinoLengthGeometryUnit);
      Grasshopper.Instances.Settings.SetValue("GsaUseRhinoTolerance", useRhinoTolerance);

      Grasshopper.Instances.Settings.SetValue("GsaLengthUnitSection", LengthUnitSection.ToString());

      Grasshopper.Instances.Settings.SetValue("GsaLengthUnitResult", LengthUnitResult.ToString());

      Grasshopper.Instances.Settings.SetValue("GsaTolerance", Tolerance.As(LengthUnitGeometry));

      Grasshopper.Instances.Settings.SetValue("GsaForceUnit", ForceUnit.ToString());
      Grasshopper.Instances.Settings.SetValue("GsaMomentUnit", MomentUnit.ToString());
      Grasshopper.Instances.Settings.SetValue("GsaStressUnit", StressUnit.ToString());
      Grasshopper.Instances.Settings.SetValue("GsaStrainUnit", StrainUnit.ToString());
      //Grasshopper.Instances.Settings.SetValue("GsaAxialStiffnessUnit", AxialStiffnessUnit.ToString());
      //Grasshopper.Instances.Settings.SetValue("GsaCurvatureUnit", CurvatureUnit.ToString());
      //Grasshopper.Instances.Settings.SetValue("GsaBendingStiffnessUnit", BendingStiffnessUnit.ToString());

      Grasshopper.Instances.Settings.SetValue("GsaMassUnit", MassUnit.ToString());
      Grasshopper.Instances.Settings.SetValue("GsaTemperatureUnit", TemperatureUnit.ToString());
      //Grasshopper.Instances.Settings.SetValue("GsaVelocityUnit", VelocityUnit.ToString());
      //Grasshopper.Instances.Settings.SetValue("GsaAccelerationUnit", AccelerationUnit.ToString());
      Grasshopper.Instances.Settings.SetValue("GsaEnergyUnit", EnergyUnit.ToString());
      //Grasshopper.Instances.Settings.SetValue("GsaTimeShortUnit", TimeShortUnit.ToString());
      //Grasshopper.Instances.Settings.SetValue("GsaTimeMediumUnit", TimeMediumUnit.ToString());
      //Grasshopper.Instances.Settings.SetValue("GsaTimeLongUnit", TimeLongUnit.ToString());


      Grasshopper.Instances.Settings.WritePersistentSettings();
    }
    internal static bool ReadSettings()
    {
      if (!Grasshopper.Instances.Settings.ConstainsEntry("GsaLengthUnitGeometry"))
        return false;

      string lengthGeometry = Grasshopper.Instances.Settings.GetValue("GsaLengthUnitGeometry", string.Empty);
      useRhinoLengthGeometryUnit = Grasshopper.Instances.Settings.GetValue("GsaUseRhinoLengthGeometryUnit", false);
      string lengthSection = Grasshopper.Instances.Settings.GetValue("GsaLengthUnitSection", string.Empty);
      string lengthResult = Grasshopper.Instances.Settings.GetValue("GsaLengthUnitResult", string.Empty);
      
      useRhinoTolerance = useRhinoLengthGeometryUnit;
      if (Grasshopper.Instances.Settings.ConstainsEntry("GsaUseRhinoTolerance"))
        useRhinoTolerance = Grasshopper.Instances.Settings.GetValue("GsaUseRhinoTolerance", false);
      double tolerance = Grasshopper.Instances.Settings.GetValue("GsaTolerance", double.NaN);

      string force = Grasshopper.Instances.Settings.GetValue("GsaForceUnit", string.Empty);
      string moment = Grasshopper.Instances.Settings.GetValue("GsaMomentUnit", string.Empty);
      string stress = Grasshopper.Instances.Settings.GetValue("GsaStressUnit", string.Empty);
      string strain = Grasshopper.Instances.Settings.GetValue("GsaStrainUnit", string.Empty);
      //string axialstiffness = Grasshopper.Instances.Settings.GetValue("GsaAxialStiffnessUnit", string.Empty);
      //string curvature = Grasshopper.Instances.Settings.GetValue("GsaCurvatureUnit", string.Empty);
      //string bendingstiffness = Grasshopper.Instances.Settings.GetValue("GsaBendingStiffnessUnit", string.Empty);

      string mass = Grasshopper.Instances.Settings.GetValue("GsaMassUnit", string.Empty);
      string temperature = Grasshopper.Instances.Settings.GetValue("GsaTemperatureUnit", string.Empty);
      //string velocity = Grasshopper.Instances.Settings.GetValue("GsaVelocityUnit", string.Empty);
      //string acceleration = Grasshopper.Instances.Settings.GetValue("GsaAccelerationUnit", string.Empty);
      string energy = EnergyUnit.ToString();
      if (Grasshopper.Instances.Settings.ConstainsEntry("GsaEnergyUnit"))
        energy = Grasshopper.Instances.Settings.GetValue("GsaEnergyUnit", string.Empty);
      //string timeShort = Grasshopper.Instances.Settings.GetValue("GsaTimeShortUnit", string.Empty);
      //string timeMedium = Grasshopper.Instances.Settings.GetValue("GsaTimeMediumUnit", string.Empty);
      //string timeLong = Grasshopper.Instances.Settings.GetValue("GsaTimeLongUnit", string.Empty);


      if (useRhinoLengthGeometryUnit)
        m_length_geometry = GetRhinoLengthUnit(RhinoDoc.ActiveDoc.ModelUnitSystem);
      else
        m_length_geometry = (LengthUnit)Enum.Parse(typeof(LengthUnit), lengthGeometry);

      m_length_section = (LengthUnit)Enum.Parse(typeof(LengthUnit), lengthSection);
      m_length_result = (LengthUnit)Enum.Parse(typeof(LengthUnit), lengthResult);

      m_tolerance = Length.From(tolerance, m_length_geometry);

      m_force = (ForceUnit)Enum.Parse(typeof(ForceUnit), force);
      m_moment = (MomentUnit)Enum.Parse(typeof(MomentUnit), moment);
      m_stress = (PressureUnit)Enum.Parse(typeof(PressureUnit), stress);
      m_strain = (StrainUnit)Enum.Parse(typeof(StrainUnit), strain);
      //m_axialstiffness = (AxialStiffnessUnit)Enum.Parse(typeof(AxialStiffnessUnit), axialstiffness);
      //m_curvature = (CurvatureUnit)Enum.Parse(typeof(CurvatureUnit), curvature);
      //m_bendingstiffness = (BendingStiffnessUnit)Enum.Parse(typeof(BendingStiffnessUnit), bendingstiffness);

      m_mass = (MassUnit)Enum.Parse(typeof(MassUnit), mass);
      m_temperature = (TemperatureUnit)Enum.Parse(typeof(TemperatureUnit), temperature);
      //m_velocity = (SpeedUnit)Enum.Parse(typeof(SpeedUnit), velocity);
      //m_acceleration = (AccelerationUnit)Enum.Parse(typeof(AccelerationUnit), acceleration);
      m_energy = (EnergyUnit)Enum.Parse(typeof(EnergyUnit), energy);
      //m_time_short = (DurationUnit)Enum.Parse(typeof(DurationUnit), timeShort);
      //m_time_medium = (DurationUnit)Enum.Parse(typeof(DurationUnit), timeMedium);
      //m_time_long = (DurationUnit)Enum.Parse(typeof(DurationUnit), timeLong);

      return true;
    }
    internal static LengthUnit GetRhinoLengthUnit()
    {
      return GetRhinoLengthUnit(RhinoDoc.ActiveDoc.ModelUnitSystem);
    }
    internal static Length GetRhinoTolerance()
    {
      LengthUnit lengthUnit = GetRhinoLengthUnit();
      double tolerance = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
      return new Length(tolerance, lengthUnit);
    }
    internal static LengthUnit GetRhinoLengthUnit(Rhino.UnitSystem rhinoUnits)
    {
      List<int> id = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 });
      List<string> name = new List<string>(new string[] {
        "None",
        "Microns",
        "mm",
        "cm",
        "m",
        "km",
        "Microinches",
        "Mils",
        "in",
        "ft",
        "Miles",
        " ",
        "Angstroms",
        "Nanometers",
        "Decimeters",
        "Dekameters",
        "Hectometers",
        "Megameters",
        "Gigameters",
        "Yards" });
      List<LengthUnit> unit = new List<LengthUnit>(new LengthUnit[] {
        LengthUnit.Undefined,
        LengthUnit.Micrometer,
        LengthUnit.Millimeter,
        LengthUnit.Centimeter,
        LengthUnit.Meter,
        LengthUnit.Kilometer,
        LengthUnit.Microinch,
        LengthUnit.Mil,
        LengthUnit.Inch,
        LengthUnit.Foot,
        LengthUnit.Mile,
        LengthUnit.Undefined,
        LengthUnit.Undefined,
        LengthUnit.Nanometer,
        LengthUnit.Decimeter,
        LengthUnit.Undefined,
        LengthUnit.Hectometer,
        LengthUnit.Undefined,
        LengthUnit.Undefined,
        LengthUnit.Yard });
      for (int i = 0; i < id.Count; i++)
        if (rhinoUnits.GetHashCode() == id[i])
          return unit[i];
      return LengthUnit.Undefined;
    }
    #endregion
  }
}
