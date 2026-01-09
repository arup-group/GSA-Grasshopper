using System;

using GsaAPI;

using GsaGH.Helpers.GsaApi;
using GsaGH.Helpers.GsaApi.EnumMappings;
using GsaGH.Parameters;

using OasysUnits.Units;

using Xunit;

using AccelerationUnit = OasysUnits.Units.AccelerationUnit;
using AngleUnit = OasysUnits.Units.AngleUnit;
using EnergyUnit = OasysUnits.Units.EnergyUnit;
using ForceUnit = OasysUnits.Units.ForceUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;
using MassUnit = OasysUnits.Units.MassUnit;

namespace GsaGHTests.Helpers.GsaAPITests {
  [Collection("GrasshopperFixture collection")]
  public class MappingsTests {

    [Theory]
    [InlineData("Linear")]
    [InlineData("Quadratic")]
    [InlineData("Rigid Diaphragm")]
    [InlineData("rIgId diAphrAgm")]
    public void GetAnalysisOrderTest(string input) {
      AnalysisOrder actual = Mappings.GetAnalysisOrder(input);
      Assert.Equal(input.ToLower(), actual.ToString().ToLower().Replace("_", " "));
    }

    [Theory]
    [InlineData("Bar", 1)]
    [InlineData("bar", 1)]
    [InlineData("Beam", 2)]
    [InlineData("bEAM", 2)]
    [InlineData("Spring", 3)]
    [InlineData("SPRING", 3)]
    [InlineData("Quad-4", 5)]
    [InlineData("qUaD-4", 5)]
    [InlineData("Quad-8", 6)]
    [InlineData("Quad8", 6)]
    [InlineData("Tri-3", 7)]
    [InlineData("tri3", 7)]
    [InlineData("Tri-6", 8)]
    [InlineData("TRI6", 8)]
    [InlineData("Link", 9)]
    [InlineData("Cable", 10)]
    [InlineData("Brick-8", 12)]
    [InlineData("Wedge-6", 14)]
    [InlineData("Tetra-4", 16)]
    [InlineData("Spacer", 19)]
    [InlineData("Strut", 20)]
    [InlineData("Tie", 21)]
    [InlineData("Rod", 23)]
    [InlineData("Damper", 24)]
    [InlineData("Pyramid-5", 26)]
    public void GetElementTypeTest(string input, int expected) {
      ElementType actual = Mappings.GetElementType(input);

      Assert.Equal(expected, (int)actual);
    }

    [Theory]
    [InlineData("Custom")]
    [InlineData("Steel")]
    [InlineData("Concrete")]
    [InlineData("Aluminium")]
    [InlineData("Glass")]
    [InlineData("Frp")]
    [InlineData("Timber")]
    [InlineData("Fabric")]
    public void GetMatTypeTest(string input) {
      MatType actual = Mappings.GetMatType(input);
      Assert.Equal(input, actual.ToString());
    }

    [Theory]
    [InlineData("Undefined", -1)]
    [InlineData("Generic 1D", 0)]
    [InlineData("Generic 2D", 1)]
    [InlineData("Beam", 2)]
    [InlineData("Column", 3)]
    [InlineData("Slab", 4)]
    [InlineData("Wall", 5)]
    [InlineData("Cantilever", 6)]
    [InlineData("Ribbed Slab", 7)]
    [InlineData("Composite", 8)]
    [InlineData("Pile", 9)]
    [InlineData("Explicit", 10)]
    [InlineData("1D Void Cutter", 11)]
    [InlineData("2D Void Cutter", 12)]
    [InlineData("Generic 3D", 13)]
    public void GetMemberType(string input, int expected) {
      MemberType actual = Mappings.GetMemberType(input);

      Assert.Equal(expected, (int)actual);
    }

    [Theory]
    [InlineData("Undefined", 0)]
    [InlineData("Plane Stress", 1)]
    [InlineData("Plane Strain", 2)]
    [InlineData("Axis Symmetric", 3)]
    [InlineData("Fabric", 4)]
    [InlineData("Plate", 5)]
    [InlineData("Shell", 6)]
    [InlineData("Curved Shell", 7)]
    [InlineData("Torsion", 8)]
    [InlineData("Load Panel", 10)]
    public void GetProperty2D_TypeTest(string input, int expected) {
      Property2D_Type actual = Mappings.GetProperty2D_Type(input);

      Assert.Equal(expected, (int)actual);
    }

    [Fact]
    public void LengthUnitMappingTest() {
      foreach (GsaAPI.LengthUnit apiLengthUnit in
        (GsaAPI.LengthUnit[])Enum.GetValues(typeof(GsaAPI.LengthUnit))) {

        LengthUnit lengthUnit = UnitMapping.GetUnit(apiLengthUnit);
        Assert.Equal(lengthUnit.ToString(), apiLengthUnit.ToString());

        GsaAPI.LengthUnit apiLengthUnitFromOasysUnit = UnitMapping.GetApiUnit(lengthUnit);
        Assert.Equal(lengthUnit.ToString(), apiLengthUnitFromOasysUnit.ToString());
      }
    }

    [Theory]
    [InlineData("MeterPerSecondSquared", "MeterPerSecondSquared")]
    [InlineData("CentimeterPerSecondSquared", "CentimeterPerSecondSquared")]
    [InlineData("Gal", "CentimeterPerSecondSquared", false)]
    [InlineData("MillimeterPerSecondSquared", "MillimeterPerSecondSquared")]
    [InlineData("InchPerSecondSquared", "InchPerSecondSquared")]
    [InlineData("FootPerSecondSquared", "FootPerSecondSquared")]
    [InlineData("Gravity", "StandardGravity")]
    [InlineData("Milligravity", "MillistandardGravity")]
    public void AccelerationUnitMappingTest(string gsaUnit, string oasysUnit, bool castBack = true) {
      var expectedUnit = (AccelerationUnit)Enum.Parse(typeof(AccelerationUnit), oasysUnit, true);
      var expectedApiUnit = (GsaAPI.AccelerationUnit)Enum.Parse(typeof(GsaAPI.AccelerationUnit), gsaUnit, true);

      AccelerationUnit unit = UnitMapping.GetUnit(expectedApiUnit);
      Assert.Equal(expectedUnit.ToString(), unit.ToString());

      if (castBack) {
        GsaAPI.AccelerationUnit apiUnit = UnitMapping.GetApiUnit(expectedUnit);
        Assert.Equal(expectedApiUnit.ToString(), apiUnit.ToString());
      }
    }

    [Theory]
    [InlineData("Degree", "Degree")]
    [InlineData("Gradian", "Gradian")]
    [InlineData("Radian", "Radian")]
    public void AngleUnitMappingTest(string gsaUnit, string oasysUnit, bool castBack = true) {
      var expectedUnit = (AngleUnit)Enum.Parse(typeof(AngleUnit), oasysUnit, true);
      var expectedApiUnit = (GsaAPI.AngleUnit)Enum.Parse(typeof(GsaAPI.AngleUnit), gsaUnit, true);

      AngleUnit unit = UnitMapping.GetUnit(expectedApiUnit);
      Assert.Equal(expectedUnit.ToString(), unit.ToString());

      if (castBack) {
        GsaAPI.AngleUnit apiUnit = UnitMapping.GetApiUnit(expectedUnit);
        Assert.Equal(expectedApiUnit.ToString(), apiUnit.ToString());
      }
    }

    [Theory]
    [InlineData("BritishThermalUnit", "BritishThermalUnit")]
    [InlineData("Calorie", "Calorie")]
    [InlineData("FootPound", "FootPound")]
    [InlineData("Gigajoule", "Gigajoule")]
    [InlineData("Joule", "Joule")]
    [InlineData("Kilojoule", "Kilojoule")]
    [InlineData("KilowattHour", "KilowattHour")]
    [InlineData("Megajoule", "Megajoule")]
    public void EnergyUnitMappingTest(string gsaUnit, string oasysUnit, bool castBack = true) {
      var expectedUnit = (EnergyUnit)Enum.Parse(typeof(EnergyUnit), oasysUnit, true);
      var expectedApiUnit = (GsaAPI.EnergyUnit)Enum.Parse(typeof(GsaAPI.EnergyUnit), gsaUnit, true);

      EnergyUnit unit = UnitMapping.GetUnit(expectedApiUnit);
      Assert.Equal(expectedUnit.ToString(), unit.ToString());

      if (castBack) {
        GsaAPI.EnergyUnit apiUnit = UnitMapping.GetApiUnit(expectedUnit);
        Assert.Equal(expectedApiUnit.ToString(), apiUnit.ToString());
      }
    }

    [Theory]
    [InlineData("KiloNewton", "Kilonewton")]
    [InlineData("KiloPoundForce", "KilopoundForce")]
    [InlineData("MegaNewton", "Meganewton")]
    [InlineData("Newton", "Newton")]
    [InlineData("PoundForce", "PoundForce")]
    [InlineData("TonneForce", "TonneForce")]
    public void ForceUnitMappingTest(string gsaUnit, string oasysUnit, bool castBack = true) {
      var expectedUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), oasysUnit, true);
      var expectedApiUnit = (GsaAPI.ForceUnit)Enum.Parse(typeof(GsaAPI.ForceUnit), gsaUnit, true);

      ForceUnit unit = UnitMapping.GetUnit(expectedApiUnit);
      Assert.Equal(expectedUnit.ToString(), unit.ToString());

      if (castBack) {
        GsaAPI.ForceUnit apiUnit = UnitMapping.GetApiUnit(expectedUnit);
        Assert.Equal(expectedApiUnit.ToString(), apiUnit.ToString());
      }
    }

    [Theory]
    [InlineData("Gram", "Gram")]
    [InlineData("Kilogram", "Kilogram")]
    [InlineData("Kilopound", "Kilopound")]
    [InlineData("Kilotonne", "Kilotonne")]
    [InlineData("Pound", "Pound")]
    [InlineData("Slug", "Slug")]
    [InlineData("Ton", "LongTon")]
    [InlineData("Tonne", "Tonne")]
    public void MassUnitMappingTest(string gsaUnit, string oasysUnit, bool castBack = true) {
      var expectedUnit = (MassUnit)Enum.Parse(typeof(MassUnit), oasysUnit, true);
      var expectedApiUnit = (GsaAPI.MassUnit)Enum.Parse(typeof(GsaAPI.MassUnit), gsaUnit, true);

      MassUnit unit = UnitMapping.GetUnit(expectedApiUnit);
      Assert.Equal(expectedUnit.ToString(), unit.ToString());

      if (castBack) {
        GsaAPI.MassUnit apiUnit = UnitMapping.GetApiUnit(expectedUnit);
        Assert.Equal(expectedApiUnit.ToString(), apiUnit.ToString());
      }
    }

    [Theory]
    [InlineData("Gigapascal", "Gigapascal")]
    [InlineData("Kilopascal", "Kilopascal")]
    [InlineData("KilopoundForcePerSquareFoot", "KilopoundForcePerSquareFoot")]
    [InlineData("KilopoundForcePerSquareInch", "KilopoundForcePerSquareInch")]
    [InlineData("Megapascal", "Megapascal")]
    [InlineData("NewtonPerSquareMillimeter", "NewtonPerSquareMillimeter")]
    [InlineData("NewtonPerSquareMeter", "NewtonPerSquareMeter")]
    [InlineData("Pascal", "Pascal")]
    [InlineData("PoundForcePerSquareFoot", "PoundForcePerSquareFoot")]
    [InlineData("PoundForcePerSquareInch", "PoundForcePerSquareInch")]
    public void StressUnitMappingTest(string gsaUnit, string oasysUnit, bool castBack = true) {
      var expectedUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), oasysUnit, true);
      var expectedApiUnit = (StressUnit)Enum.Parse(typeof(StressUnit), gsaUnit, true);

      PressureUnit unit = UnitMapping.GetUnit(expectedApiUnit);
      Assert.Equal(expectedUnit.ToString(), unit.ToString());

      if (castBack) {
        StressUnit apiUnit = UnitMapping.GetApiUnit(expectedUnit);
        Assert.Equal(expectedApiUnit.ToString(), apiUnit.ToString());
      }
    }

    [Theory]
    [InlineData("Day", "Day")]
    [InlineData("Hour", "Hour")]
    [InlineData("Millisecond", "Millisecond")]
    [InlineData("Minute", "Minute")]
    [InlineData("Second", "Second")]
    public void TimeUnitMappingTest(string gsaUnit, string oasysUnit, bool castBack = true) {
      var expectedUnit = (DurationUnit)Enum.Parse(typeof(DurationUnit), oasysUnit, true);
      var expectedApiUnit = (TimeUnit)Enum.Parse(typeof(TimeUnit), gsaUnit, true);

      DurationUnit unit = UnitMapping.GetUnit(expectedApiUnit);
      Assert.Equal(expectedUnit.ToString(), unit.ToString());

      if (castBack) {
        TimeUnit apiUnit = UnitMapping.GetApiUnit(expectedUnit);
        Assert.Equal(expectedApiUnit.ToString(), apiUnit.ToString());
      }
    }

    [Theory]
    [InlineData("CentimeterPerSecond", "CentimeterPerSecond")]
    [InlineData("FootPerSecond", "FootPerSecond")]
    [InlineData("InchPerSecond", "InchPerSecond")]
    [InlineData("KilometerPerHour", "KilometerPerHour")]
    [InlineData("MeterPerSecond", "MeterPerSecond")]
    [InlineData("MilePerHour", "MilePerHour")]
    [InlineData("MillimeterPerSecond", "MillimeterPerSecond")]
    public void VelocityUnitMappingTest(string gsaUnit, string oasysUnit, bool castBack = true) {
      var expectedUnit = (SpeedUnit)Enum.Parse(typeof(SpeedUnit), oasysUnit, true);
      var expectedApiUnit = (VelocityUnit)Enum.Parse(typeof(VelocityUnit), gsaUnit, true);

      SpeedUnit unit = UnitMapping.GetUnit(expectedApiUnit);
      Assert.Equal(expectedUnit.ToString(), unit.ToString());

      if (castBack) {
        VelocityUnit apiUnit = UnitMapping.GetApiUnit(expectedUnit);
        Assert.Equal(expectedApiUnit.ToString(), apiUnit.ToString());
      }
    }
  }
}
