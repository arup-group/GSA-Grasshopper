using System;
using System.Collections.Generic;
using GsaAPI;
using GsaAPI.Materials;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

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
      Assert.Equal(0.05, material.AnalysisMaterial.CoefficientOfThermalExpansion);
      Assert.Equal(7800, material.AnalysisMaterial.Density);
      Assert.Equal(205000, material.AnalysisMaterial.ElasticModulus);
      Assert.Equal(0.3, material.AnalysisMaterial.PoissonsRatio);
      DuplicateTest(material);
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
      Assert.Equal(0.05, material.AnalysisMaterial.CoefficientOfThermalExpansion);
      Assert.Equal(7800, material.AnalysisMaterial.Density);
      Assert.Equal(205000, material.AnalysisMaterial.ElasticModulus);
      Assert.Equal(0.3, material.AnalysisMaterial.PoissonsRatio);
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
