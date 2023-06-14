using GsaAPI;
using GsaAPI.Materials;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using System;
using System.Collections.Generic;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaMaterialTest {

    [Fact]
    public void DuplicateTest1() {
      var original = new GsaMaterial {
        MaterialType = GsaMaterial.MatType.Aluminium,
      };

      GsaMaterial duplicate = original.Duplicate();
      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void DuplicateTest2() {
      var analysisMaterial = new AnalysisMaterial() {
        CoefficientOfThermalExpansion = 1,
        Density = 2,
        ElasticModulus = 3,
        PoissonsRatio = 4,
        Name = "name"
      };
      var original = new GsaMaterial {
        Id = 7,
        MaterialType = GsaMaterial.MatType.Generic,
        AnalysisMaterial = analysisMaterial,
      };

      GsaMaterial duplicate = original.Duplicate();
      Duplicates.AreEqual(original, duplicate);
    }

    [Theory]
    [InlineData(GsaMaterial.MatType.Frp)]
    [InlineData(GsaMaterial.MatType.Aluminium)]
    [InlineData(GsaMaterial.MatType.Timber)]
    [InlineData(GsaMaterial.MatType.Glass)]
    [InlineData(GsaMaterial.MatType.Fabric)]
    public void CreateOtherStandardMaterialsTest(GsaMaterial.MatType type) {
      List<string> gradeNames = GsaMaterial.GetGradeNames(type);
      
      Assert.NotEmpty(gradeNames);

      foreach (string grade in gradeNames) {
        var mat = new GsaMaterial(type, grade);
        Assert.NotNull(mat); 
      }
    }

    [Fact]
    public void CreateSteelStandardMaterialsTest() {
      foreach (string codeName in DesignCode.GetSteelDesignCodeNames()) {
        List<string> gradeNames = GsaMaterial.GetGradeNames(GsaMaterial.MatType.Steel, codeName);

        Assert.NotEmpty(gradeNames);

        foreach (string grade in gradeNames) {
          var mat = new GsaMaterial(GsaMaterial.MatType.Steel, grade, codeName);
          Assert.NotNull(mat);
        }
      }
    }

    [Fact]
    public void CreateConcreteStandardMaterialsTest() {
      foreach (string codeName in DesignCode.GetConcreteDesignCodeNames()) {
        List<string> gradeNames = GsaMaterial.GetGradeNames(
          GsaMaterial.MatType.Concrete, "", codeName);

        Assert.NotEmpty(gradeNames);

        foreach (string grade in gradeNames) {
          var mat = new GsaMaterial(GsaMaterial.MatType.Concrete, grade, "", codeName);
          Assert.NotNull(mat);
        }
      }
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
  }
}
