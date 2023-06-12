﻿using System;
using System.IO;
using GsaAPI;
using GsaGH.Components;
using GsaGH.Helpers.Export;
using GsaGH.Helpers.GsaApi.EnumMappings;
using GsaGH.Parameters;
using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;
using Xunit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaModelTest {

    [Fact]
    public void GsaModelEqualsTest() {
      var original = new GsaModel();
      GsaModel duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void TestCreateModelFromModel() {
      var original = new GsaModel();
      original.Model.Open(GsaFile.SteelDesignSimple);

      var assembled = new GsaModel {
        Model = AssembleModel.Assemble(original, null, null, null, null, null, null, null, null, null,
          null, null, null, null, null, null, LengthUnit.Meter, Length.Zero, false, null), };

      Duplicates.AreEqual(original, assembled, true);
    }

    [Fact]
    public void TestDuplicateModel() {
      var m = new GsaModel();

      Guid originalGuid = m.Guid;
      GsaModel clone = m.Clone();
      Guid cloneGuid = clone.Guid;
      Assert.NotEqual(cloneGuid, originalGuid);
      GsaModel dup = m.Duplicate();
      Guid dupGuid = dup.Guid;
      Assert.Equal(dupGuid, originalGuid);
    }

    [Theory]
    [InlineData(LengthUnit.Meter, 12800.0)]
    [InlineData(LengthUnit.Foot, 452027.734035)]
    public void TestGetBoundingBox(LengthUnit modelUnit, double expectedVolume) {
      var model = new GsaModel();
      model.Model.Open(GsaFile.SteelDesignComplex);
      model.ModelUnit = modelUnit;
      BoundingBox bbox = model.BoundingBox;

      Assert.Equal(expectedVolume, bbox.Volume, 6);
    }

    [Fact]
    public void TestSaveModel() {
      var m = new GsaModel();
      string file = GsaFile.SteelDesignSimple;
      m.Model.Open(file);

      string tempfilename = Path.GetTempPath() + "GSA-Grasshopper_temp.gwb";
      ReturnValue returnValue = m.Model.SaveAs(tempfilename);

      Assert.Same(ReturnValue.GS_OK.ToString(), returnValue.ToString());
    }

    [Fact]
    public void TestModelUiUnit() {
      var m = new GsaModel();
      UiUnits uiUnits = m.Units;

      Assert.Equal(
        OasysGH.Units.DefaultUnits.AccelerationUnit.ToString(),
        UnitMapping.GetUnit(uiUnits.Acceleration).ToString());
      Assert.Equal(
        OasysGH.Units.DefaultUnits.AngleUnit.ToString(),
        UnitMapping.GetUnit(uiUnits.Angle).ToString());
      Assert.Equal(
        OasysGH.Units.DefaultUnits.EnergyUnit.ToString(),
        UnitMapping.GetUnit(uiUnits.Energy).ToString());
      Assert.Equal(
        OasysGH.Units.DefaultUnits.ForceUnit.ToString(),
        UnitMapping.GetUnit(uiUnits.Force).ToString());
      Assert.Equal(
        OasysGH.Units.DefaultUnits.LengthUnitGeometry.ToString(),
        UnitMapping.GetUnit(uiUnits.LengthLarge).ToString());
      Assert.Equal(
        OasysGH.Units.DefaultUnits.LengthUnitSection.ToString(),
        UnitMapping.GetUnit(uiUnits.LengthSections).ToString());
      Assert.Equal(
        OasysGH.Units.DefaultUnits.LengthUnitResult.ToString(),
        UnitMapping.GetUnit(uiUnits.LengthSmall).ToString());
      Assert.Equal(
        OasysGH.Units.DefaultUnits.MassUnit.ToString(),
        UnitMapping.GetUnit(uiUnits.Mass).ToString());
      Assert.Equal(
        OasysGH.Units.DefaultUnits.StressUnitResult.ToString(),
        UnitMapping.GetUnit(uiUnits.Stress).ToString());
      Assert.Equal(
        OasysGH.Units.DefaultUnits.TimeLongUnit.ToString(),
        UnitMapping.GetUnit(uiUnits.TimeLong).ToString());
      Assert.Equal(
        OasysGH.Units.DefaultUnits.TimeMediumUnit.ToString(),
        UnitMapping.GetUnit(uiUnits.TimeMedium).ToString());
      Assert.Equal(
        OasysGH.Units.DefaultUnits.TimeShortUnit.ToString(),
        UnitMapping.GetUnit(uiUnits.TimeShort).ToString());
      Assert.Equal(
        OasysGH.Units.DefaultUnits.VelocityUnit.ToString(),
        UnitMapping.GetUnit(uiUnits.Velocity).ToString());
    }

    [Fact]
    public void TestModelLengthUnit() {
      var m = new GsaModel();

      Assert.Equal(LengthUnit.Undefined, m.ModelUnit);

      m.ModelUnit = LengthUnit.Foot;
      Assert.Equal(LengthUnit.Foot, m.ModelUnit);

      Assert.Equal(LengthUnit.Foot, UnitMapping.GetUnit(m.Model.UiUnits().LengthLarge));
    }
  }
}
