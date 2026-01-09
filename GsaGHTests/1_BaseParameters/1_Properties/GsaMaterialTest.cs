using System;
using System.Collections.Generic;

using GsaAPI;
using GsaAPI.Materials;

using GsaGH.Helpers;
using GsaGH.Helpers.Assembly;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysUnits;

using Xunit;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaMaterialTest {
    [Fact]
    public void CreateSteelStandardMaterialsTest() {
      foreach (string codeName in DesignCode.GetSteelDesignCodeNames()) {
        List<string> gradeNames = GsaMaterialFactory.GetGradeNames(MatType.Steel, string.Empty, codeName);

        Assert.NotEmpty(gradeNames);

        foreach (string grade in gradeNames) {
          var material = (GsaMaterial)GsaMaterialFactory.CreateStandardMaterial(MatType.Steel, grade, codeName);
          Assert.NotNull(material);
          Assert.Equal(MatType.Steel, material.MaterialType);
          DuplicateTest(material);
          EditSteelStandardMaterialsAnalysisMaterialTest(material);
          material.Name = "customName";
          Assert.Equal("customName", material.Name);
          var apiMaterial = (SteelMaterial)((IGsaStandardMaterial)material).StandardMaterial;
          Assert.Equal("customName", apiMaterial.Name);
        }
      }
    }

    [Fact]
    public void CreateConcreteStandardMaterialsTest() {
      foreach (string codeName in DesignCode.GetConcreteDesignCodeNames()) {
        List<string> gradeNames = GsaMaterialFactory.GetGradeNames(
          MatType.Concrete, codeName, string.Empty);

        Assert.NotEmpty(gradeNames);

        foreach (string grade in gradeNames) {
          var material = (GsaMaterial)GsaMaterialFactory.CreateStandardMaterial(MatType.Concrete, grade, codeName);
          Assert.NotNull(material);
          Assert.Equal(MatType.Concrete, material.MaterialType);
          DuplicateTest(material);
          EditConcreteStandardMaterialsAnalysisMaterialTest(material);
          material.Name = "customName";
          Assert.Equal("customName", material.Name);
          var apiMaterial = (ConcreteMaterial)((IGsaStandardMaterial)material).StandardMaterial;
          Assert.Equal("customName", apiMaterial.Name);
        }
      }
    }

    [Fact]
    public void CreateFabricStandardMaterialsTest() {
      List<string> gradeNames = GsaMaterialFactory.GetGradeNames(MatType.Fabric);
      Assert.NotEmpty(gradeNames);

      foreach (string grade in gradeNames) {
        var material = (GsaMaterial)GsaMaterialFactory.CreateStandardMaterial(MatType.Fabric, grade);
        Assert.NotNull(material);
        Assert.Equal(MatType.Fabric, material.MaterialType);
        material.Name = "customName";
        Assert.Equal("customName", material.Name);
        var apiMaterial = (FabricMaterial)((IGsaStandardMaterial)material).StandardMaterial;
        Assert.Equal("customName", apiMaterial.Name);
        Assert.Throws<GsaApiException>(() => DuplicateTest(material));
      }
    }

    [Theory]
    [InlineData(MatType.Frp)]
    [InlineData(MatType.Aluminium)]
    [InlineData(MatType.Timber)]
    [InlineData(MatType.Glass)]
    public void CreateOtherStandardMaterialsTest(MatType type) {
      List<string> gradeNames = GsaMaterialFactory.GetGradeNames(type);
      Assert.NotEmpty(gradeNames);

      foreach (string grade in gradeNames) {
        var material = (GsaMaterial)GsaMaterialFactory.CreateStandardMaterial(type, grade);
        Assert.NotNull(material);
        Assert.Equal(type, material.MaterialType);
        DuplicateTest(material);
        EditOtherStandardMaterialsTest(material);
        material.Name = "customName";
        Assert.Equal("customName", material.Name);
        switch (type) {
          case MatType.Aluminium:
            var aluminium = (AluminiumMaterial)((IGsaStandardMaterial)material).StandardMaterial;
            Assert.Equal("customName", aluminium.Name);
            break;

          case MatType.Frp:
            var frp = (FrpMaterial)((IGsaStandardMaterial)material).StandardMaterial;
            Assert.Equal("customName", frp.Name);
            break;

          case MatType.Glass:
            var glass = (GlassMaterial)((IGsaStandardMaterial)material).StandardMaterial;
            Assert.Equal("customName", glass.Name);
            break;

          case MatType.Timber:
            var timber = (TimberMaterial)((IGsaStandardMaterial)material).StandardMaterial;
            Assert.Equal("customName", timber.Name);
            break;

          default:
            Assert.True(false);
            break;
        }
      }
    }

    [Fact]
    public void CreateCustomMaterialTest() {
      var material = new GsaCustomMaterial(TestAnalysisMaterial(), 99);

      Assert.Equal(MatType.Custom, material.MaterialType);
      Assert.Equal(99, material.Id);
      Assert.Equal("myMat", material.Name);
      Assert.Equal(0.05, material.AnalysisMaterial.CoefficientOfThermalExpansion, DoubleComparer.Default);
      Assert.Equal(7800, material.AnalysisMaterial.Density, DoubleComparer.Default);
      Assert.Equal(205000, material.AnalysisMaterial.ElasticModulus, DoubleComparer.Default);
      Assert.Equal(0.3, material.AnalysisMaterial.PoissonsRatio, DoubleComparer.Default);
      DuplicateTest(material);
    }

    [Fact]
    public void GetMatTypeTest() {
      GsaAPI.MaterialType concrete = MaterialType.CONCRETE;
      Assert.Equal(MatType.Concrete, GsaMaterialFactory.GetMatType(concrete));
      GsaAPI.MaterialType steel = MaterialType.STEEL;
      Assert.Equal(MatType.Steel, GsaMaterialFactory.GetMatType(steel));
      GsaAPI.MaterialType custom = MaterialType.GENERIC;
      Assert.Equal(MatType.Custom, GsaMaterialFactory.GetMatType(custom));
      GsaAPI.MaterialType other = MaterialType.NONE;
      Assert.Equal(MatType.Custom, GsaMaterialFactory.GetMatType(other));
    }

    [Fact]
    public void NonStandardMaterialException() {
      Assert.Throws<Exception>(() => {
        var mat = (GsaMaterial)GsaMaterialFactory.CreateStandardMaterial(MatType.Custom, "custom");
      });
    }

    [Fact]
    public void NonStandardMaterialGradeNameException() {
      Assert.Throws<Exception>(() => {
        List<string> grades = GsaMaterialFactory.GetGradeNames(MatType.Custom);
      });
    }

    [Fact]
    public void ReferenceMaterialsTest() {
      var section = new GsaAPI.Section() {
        MaterialType = MaterialType.ALUMINIUM,
        MaterialGradeProperty = 99
      };
      var prop2d = new GsaAPI.Prop2D() {
        MaterialType = MaterialType.TIMBER,
        MaterialGradeProperty = 7
      };
      var prop3d = new GsaAPI.Prop3D() {
        MaterialType = MaterialType.GENERIC,
        MaterialGradeProperty = 42
      };
      var model = new GsaAPI.Model();
      model.AddSection(section);
      model.AddProp2D(prop2d);
      model.AddProp3D(prop3d);

      var gsaModel = new GsaModel(model);
      GsaSection modelSection = gsaModel.Sections[1].Value;
      GsaProperty2d modelProp2d = gsaModel.Prop2ds[1].Value;
      GsaProperty3d modelProp3d = gsaModel.Prop3ds[1].Value;
      Assert.Equal(99, modelSection.Material.Id);
      Assert.Equal(7, modelProp2d.Material.Id);
      Assert.Equal(42, modelProp3d.Material.Id);
      Assert.Equal(MatType.Aluminium, modelSection.Material.MaterialType);
      Assert.Equal(MatType.Timber, modelProp2d.Material.MaterialType);
      Assert.Equal(MatType.Custom, modelProp3d.Material.MaterialType);

      var properties = new GsaProperties {
        Property2ds = new List<GsaProperty2d> { modelProp2d },
        Property3ds = new List<GsaProperty3d> { modelProp3d },
        Sections = new List<GsaSection> { modelSection },
      };
      var assembly = new ModelAssembly(null, null, null, null, properties, null, null,
        LengthUnit.Meter, Length.Zero, false, null);
      GsaAPI.Model assembled = assembly.GetModel();
      Assert.Equal(99, assembled.Sections()[1].MaterialGradeProperty);
      Assert.Equal(7, assembled.Prop2Ds()[1].MaterialGradeProperty);
      Assert.Equal(42, assembled.Prop3Ds()[1].MaterialAnalysisProperty);
    }

    internal static void DuplicateTest(GsaMaterial original) {
      GsaMaterial duplicate = GsaMaterialFactory.CreateMaterial(original);
      Assert.NotSame(duplicate, original);

      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid", "IsFromApi" });
    }

    private void EditOtherStandardMaterialsTest(GsaMaterial material) {
      Assert.NotNull(material);
      material.AnalysisMaterial = TestAnalysisMaterial();
      AnalysisMaterialIsEqualTest(material);
    }

    private void EditSteelStandardMaterialsAnalysisMaterialTest(GsaMaterial material) {
      material.AnalysisMaterial = TestAnalysisMaterial();
      AnalysisMaterialIsEqualTest(material);
    }

    private void EditConcreteStandardMaterialsAnalysisMaterialTest(GsaMaterial material) {
      Assert.NotNull(material);
      material.AnalysisMaterial = TestAnalysisMaterial();
      AnalysisMaterialIsEqualTest(material);
    }

    private void AnalysisMaterialIsEqualTest(GsaMaterial material) {
      Assert.Equal("myMat", material.AnalysisMaterial.Name);
      Assert.Equal(0.05, material.AnalysisMaterial.CoefficientOfThermalExpansion, DoubleComparer.Default);
      Assert.Equal(7800, material.AnalysisMaterial.Density, DoubleComparer.Default);
      Assert.Equal(205000, material.AnalysisMaterial.ElasticModulus, DoubleComparer.Default);
      Assert.Equal(0.3, material.AnalysisMaterial.PoissonsRatio, DoubleComparer.Default);
      DuplicateTest(material);
    }

    internal static AnalysisMaterial TestAnalysisMaterial() {
      return new AnalysisMaterial() {
        CoefficientOfThermalExpansion = 0.05,
        Density = 7800,
        ElasticModulus = 205000,
        Name = "myMat",
        PoissonsRatio = 0.3,
      };
    }
  }
}
