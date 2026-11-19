using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

using GsaAPI;

using GsaGH.Helpers;
using GsaGH.Helpers.Assembly;
using GsaGH.Helpers.GsaApi.EnumMappings;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;

using GsaGHTests.Helper;
using GsaGHTests.Helpers;

using OasysUnits;

using Rhino.Geometry;

using Xunit;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaModelTest {
    [Fact]
    public void TestCreateModelFromModel() {
      var original = new GsaModel();
      original.ApiModel.Open(GsaFile.SteelDesignSimple);
      Utility.DeleteResults(original.ApiModel);
      var assembly = new ModelAssembly(original, null, null, null, null, null, null,
        LengthUnit.Meter, Length.Zero, false, null);
      var assembled = new GsaModel() {
        ApiModel = assembly.GetModel()
      };

      Duplicates.AreEqual(original, assembled, new List<string>() { "Guid" });
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void PreviewAssemblyWithoutGeometryPreservesResults(bool emptyCollections) {
      var model = new GsaModel();
      model.ApiModel.Open(GsaFile.SteelDesignSimple);
      int resultCount = model.ApiModel.Results().Count;
      int combinationResultCount = model.ApiModel.CombinationCaseResults().Count;
      Assert.True(resultCount > 0);

      var assembly = new ModelAssembly(model,
        emptyCollections ? new List<GsaList>() : null,
        emptyCollections ? new List<GsaElement1d>() : null,
        emptyCollections ? new List<GsaElement2d>() : null,
        emptyCollections ? new List<GsaMember1d>() : null,
        emptyCollections ? new List<GsaMember2d>() : null,
        LengthUnit.Meter);

      Assert.Same(model.ApiModel, assembly.GetModel());
      Assert.Equal(resultCount, assembly.GetModel().Results().Count);
      Assert.Equal(combinationResultCount, assembly.GetModel().CombinationCaseResults().Count);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void PreviewAssemblyWithGeometryDeletesClonedResults(bool geometryInList) {
      var original = new GsaModel();
      original.ApiModel.Open(GsaFile.SteelDesignSimple);
      int resultCount = original.ApiModel.Results().Count;
      int combinationResultCount = original.ApiModel.CombinationCaseResults().Count;
      int elementCount = original.ApiModel.Elements().Count;
      Assert.True(resultCount > 0);
      GsaModel model = original.Clone();
      Assert.Equal(resultCount, model.ApiModel.Results().Count);

      var element = new GsaElement1d(new LineCurve(Point3d.Origin, new Point3d(0, 0, 10))) {
        Section = new GsaSection("STD R 200 100")
      };
      var lists = new List<GsaList>();
      var elements = new List<GsaElement1d>();
      if (geometryInList) {
        lists.Add(new GsaList {
          EntityType = GsaGH.Parameters.EntityType.Element,
          _elements = (new ConcurrentBag<GsaElement1dGoo> { new GsaElement1dGoo(element) },
            new ConcurrentBag<GsaElement2dGoo>(), new ConcurrentBag<GsaElement3dGoo>())
        });
      } else {
        elements.Add(element);
      }

      var assembly = new ModelAssembly(model, lists, elements, null, null, null, LengthUnit.Meter);

      Assert.Empty(assembly.GetModel().Results());
      Assert.Equal(elementCount + 1, assembly.GetModel().Elements().Count);
      Assert.Equal(resultCount, original.ApiModel.Results().Count);
      Assert.Equal(combinationResultCount, original.ApiModel.CombinationCaseResults().Count);
      Assert.Equal(elementCount, original.ApiModel.Elements().Count);
    }

    [Fact]
    public void TestDuplicateModel() {
      var m = new GsaModel();

      Guid originalGuid = m.Guid;
      GsaModel clone = m.Clone();
      Guid cloneGuid = clone.Guid;
      Assert.NotEqual(cloneGuid, originalGuid);
      GsaModel dup = m;
      Guid dupGuid = dup.Guid;
      Assert.Equal(dupGuid, originalGuid);
    }

    [Theory]
    [InlineData(LengthUnit.Meter, 12800.0)]
    [InlineData(LengthUnit.Foot, 452027.734035)]
    public void TestGetBoundingBox(LengthUnit modelUnit, double expectedVolume) {
      var model = new GsaModel();
      model.ApiModel.Open(GsaFile.SteelDesignComplex);
      model.ModelUnit = modelUnit;
      BoundingBox bbox = model.BoundingBox;

      Assert.Equal(expectedVolume, bbox.Volume, 6);
    }

    [Fact]
    public void NativeApiSavesExistingModelTest() {
      var m = new GsaModel();
      string file = GsaFile.SteelDesignSimple;
      m.ApiModel.Open(file);

      string tempfilename = Path.GetTempPath() + "GSA-Grasshopper_temp.gwb";
      ReturnValue returnValue = m.ApiModel.SaveAs(tempfilename);

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

      Assert.Equal(LengthUnit.Meter, m.ModelUnit);

      m.ModelUnit = LengthUnit.Foot;
      Assert.Equal(LengthUnit.Foot, m.ModelUnit);

      Assert.Equal(LengthUnit.Foot, UnitMapping.GetUnit(m.ApiModel.UiUnits().LengthLarge));
    }

    [Fact]
    public void ShouldNotHaveAnyTasks() {
      GsaAnalysis gsaAnalysis = null;
      var assembly = new ModelAssembly(null, null, null, null, null, null, gsaAnalysis, LengthUnit.Meter, Length.Zero,
        false, null);
      var model = new GsaModel(assembly.GetModel());
      List<GsaAnalysisTaskGoo> importedTasks = model.GetAnalysisTasksAndCombinations().Item1;
      Assert.Empty(importedTasks);
      Assert.Empty(importedTasks);
    }

    [Fact]
    public void AnalysisTaskAndCasesCanBeImportedFromSeedModel() {
      //seed model to read existing analysis task
      var seedModel = new GsaModel();
      seedModel.ApiModel.Open(GsaFile.SteelDesignComplex);
      List<GsaAnalysisTaskGoo> seedTasks = seedModel.GetAnalysisTasksAndCombinations().Item1;
      var analysis = new GsaAnalysis();
      foreach (GsaAnalysisTaskGoo task in seedTasks) {
        analysis.AnalysisTasks.Add(task.Value);
      }

      //import into new model
      var assembly = new ModelAssembly(null, null, null, null, null, null, analysis,
        LengthUnit.Meter, Length.Zero, false, null);
      var model = new GsaModel(assembly.GetModel());

      List<GsaAnalysisTaskGoo> importedTasks = model.GetAnalysisTasksAndCombinations().Item1;

      Assert.True(Duplicates.AreEqual(seedTasks, importedTasks));

    }
  }
}
