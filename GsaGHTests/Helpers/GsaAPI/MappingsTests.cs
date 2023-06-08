using System;
using GsaAPI;
using GsaGH.Helpers.GsaApi;
using GsaGH.Helpers.GsaApi.EnumMappings;
using OasysUnits.Units;
using Xunit;
using static GsaGH.Parameters.GsaMaterial;
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
    [InlineData("Linear", 0)]
    [InlineData("Quadratic", 1)]
    [InlineData("Rigid Diaphragm", 2)]
    [InlineData("rIgId diAphrAgm", 2)]
    public void GetAnalysisOrderTest(string input, int expected) {
      AnalysisOrder actual = Mappings.GetAnalysisOrder(input);

      Assert.Equal(expected, (int)actual);
    }

    [Theory]
    [InlineData("New", -1)]
    [InlineData(" New", -1)]
    [InlineData("Undefined", 0)]
    [InlineData(" Undefined  ", 0)]
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
    [InlineData("Last Type", 26)]
    [InlineData("1D", 27)]
    [InlineData("2D", 28)]
    [InlineData("3D", 29)]
    [InlineData("1D Section", 30)]
    [InlineData("2D Finite Element", 31)]
    [InlineData("2d FinitEelement", 31)]
    [InlineData("2D Load", 32)]
    [InlineData("2dLoad", 32)]
    public void GetElementTypeTest(string input, int expected) {
      ElementType actual = Mappings.GetElementType(input);

      Assert.Equal(expected, (int)actual);
    }

    [Theory]
    [InlineData("Undefined", -2)]
    [InlineData("None", -1)]
    [InlineData("Generic", 0)]
    [InlineData("Steel", 1)]
    [InlineData("Concrete", 2)]
    [InlineData("Aluminium", 3)]
    [InlineData("Glass", 4)]
    [InlineData("FRP", 5)]
    [InlineData("Rebar", 6)]
    [InlineData("Timber", 7)]
    [InlineData("Fabric", 8)]
    [InlineData("Soil", 9)]
    [InlineData("Numeric Material", 10)]
    [InlineData("Compound", 0x100)]
    [InlineData("Bar", 0x1000)]
    [InlineData("Tendon", 4352)]
    [InlineData("FRP Bar", 4608)]
    [InlineData("CFRP", 4864)]
    [InlineData("GFRP", 5120)]
    [InlineData("AFRP", 5376)]
    [InlineData("ARGFRP", 5632)]
    [InlineData("Bar Material", 65280)]
    public void GetMatTypeTest(string input, int expected) {
      MatType actual = Mappings.GetMatType(input);

      Assert.Equal(expected, (int)actual);
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
    [InlineData("Num Type", 11)]
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
      var expectedApiUnit = (GsaAPI.StressUnit)Enum.Parse(typeof(GsaAPI.StressUnit), gsaUnit, true);

      PressureUnit unit = UnitMapping.GetUnit(expectedApiUnit);
      Assert.Equal(expectedUnit.ToString(), unit.ToString());

      if (castBack) {
        GsaAPI.StressUnit apiUnit = UnitMapping.GetApiUnit(expectedUnit);
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
      var expectedApiUnit = (GsaAPI.TimeUnit)Enum.Parse(typeof(GsaAPI.TimeUnit), gsaUnit, true);

      DurationUnit unit = UnitMapping.GetUnit(expectedApiUnit);
      Assert.Equal(expectedUnit.ToString(), unit.ToString());

      if (castBack) {
        GsaAPI.TimeUnit apiUnit = UnitMapping.GetApiUnit(expectedUnit);
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
      var expectedApiUnit = (GsaAPI.VelocityUnit)Enum.Parse(typeof(GsaAPI.VelocityUnit), gsaUnit, true);

      SpeedUnit unit = UnitMapping.GetUnit(expectedApiUnit);
      Assert.Equal(expectedUnit.ToString(), unit.ToString());

      if (castBack) {
        GsaAPI.VelocityUnit apiUnit = UnitMapping.GetApiUnit(expectedUnit);
        Assert.Equal(expectedApiUnit.ToString(), apiUnit.ToString());
      }
    }
  }
}
