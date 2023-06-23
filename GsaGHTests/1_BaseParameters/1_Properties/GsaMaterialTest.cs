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
        List<string> gradeNames = GsaMaterial.GetGradeNames(GsaMaterial.MatType.Steel, codeName);

        Assert.NotEmpty(gradeNames);

        foreach (string grade in gradeNames) {
          var material = new GsaMaterial(GsaMaterial.MatType.Steel, grade, codeName);
          Assert.NotNull(material);
          Assert.Equal(GsaMaterial.MatType.Steel, material.MaterialType);
          DuplicateTest(material);
          EditSteelStandardMaterialsAnalysisMaterialTest(material);
          material.Name = "customName";
          Assert.Equal("customName", material.Name);
          var apiMaterial = (SteelMaterial)material.StandardMaterial;
          Assert.Equal("customName", apiMaterial.Name);
        }
      }
    }

    [Fact]
    public void CreateConcreteStandardMaterialsTest() {
      foreach (string codeName in DesignCode.GetConcreteDesignCodeNames()) {
        List<string> gradeNames = GsaMaterial.GetGradeNames(
          GsaMaterial.MatType.Concrete, string.Empty, codeName);

        Assert.NotEmpty(gradeNames);

        foreach (string grade in gradeNames) {
          var material
            = new GsaMaterial(GsaMaterial.MatType.Concrete, grade, string.Empty, codeName);
          Assert.NotNull(material);
          Assert.Equal(GsaMaterial.MatType.Concrete, material.MaterialType);
          DuplicateTest(material);
          EditConcreteStandardMaterialsAnalysisMaterialTest(material);
          material.Name = "customName";
          Assert.Equal("customName", material.Name);
          var apiMaterial = (ConcreteMaterial)material.StandardMaterial;
          Assert.Equal("customName", apiMaterial.Name);
        }
      }
    }

    [Fact]
    public void CreateFabricStandardMaterialsTest() {
      List<string> gradeNames = GsaMaterial.GetGradeNames(GsaMaterial.MatType.Fabric);
      Assert.NotEmpty(gradeNames);

      foreach (string grade in gradeNames) {
        var material = new GsaMaterial(GsaMaterial.MatType.Fabric, grade);
        Assert.NotNull(material);
        Assert.Equal(GsaMaterial.MatType.Fabric, material.MaterialType);
        material.Name = "customName";
        Assert.Equal("customName", material.Name);
        var apiMaterial = (FabricMaterial)material.StandardMaterial;
        Assert.Equal("customName", apiMaterial.Name);
        Assert.Throws<Exception>(() => DuplicateTest(material));
      }
    }

    [Theory]
    [InlineData(GsaMaterial.MatType.Frp)]
    [InlineData(GsaMaterial.MatType.Aluminium)]
    [InlineData(GsaMaterial.MatType.Timber)]
    [InlineData(GsaMaterial.MatType.Glass)]
    public void CreateOtherStandardMaterialsTest(GsaMaterial.MatType type) {
      List<string> gradeNames = GsaMaterial.GetGradeNames(type);
      Assert.NotEmpty(gradeNames);

      foreach (string grade in gradeNames) {
        var material = new GsaMaterial(type, grade);
        Assert.NotNull(material);
        Assert.Equal(type, material.MaterialType);
        DuplicateTest(material);
        EditOtherStandardMaterialsTest(material);
        material.Name = "customName";
        Assert.Equal("customName", material.Name);
        switch (type) {
          case GsaMaterial.MatType.Aluminium:
            var aluminium = (AluminiumMaterial)material.StandardMaterial;
            Assert.Equal("customName", aluminium.Name);
            break;

          case GsaMaterial.MatType.Frp:
            var frp = (FrpMaterial)material.StandardMaterial;
            Assert.Equal("customName", frp.Name);
            break;

          case GsaMaterial.MatType.Glass:
            var glass = (GlassMaterial)material.StandardMaterial;
            Assert.Equal("customName", glass.Name);
            break;

          case GsaMaterial.MatType.Timber:
            var timber = (TimberMaterial)material.StandardMaterial;
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
      var material = new GsaMaterial(TestAnalysisMaterial(), 99);

      Assert.Equal(GsaMaterial.MatType.Generic, material.MaterialType);
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
        var mat = new GsaMaterial(GsaMaterial.MatType.Generic, "custom");
      });
    }

    [Fact]
    public void NonStandardMaterialGradeNameException() {
      Assert.Throws<Exception>(() => {
        List<string> grades = GsaMaterial.GetGradeNames(GsaMaterial.MatType.Generic);
      });
    }

    internal static void DuplicateTest(GsaMaterial original) {
      GsaMaterial duplicate = original.Clone();
      Assert.NotSame(duplicate, original);
      Duplicates.AreEqual(original, duplicate);
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
      Assert.Equal("myMat", material.Name);
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
