using GsaGH.Parameters;
using GsaGH.Helpers.Import;
using Xunit;
using System.Collections.Generic;
using GsaGHTests.Parameters;
using System;

namespace GsaGHTests.Helpers.Import {
  [Collection("GrasshopperFixture collection")]
  public class MaterialsTest {
    [Fact]
    public void ImportSteelMaterialTest() {
      Materials materials = ImportMaterialsMother();
      int i = 1;
      foreach (KeyValuePair<int, GsaMaterial> kvp in materials.SteelMaterials) {
        Assert.NotNull(kvp.Value);
        Assert.Equal(GsaMaterial.MatType.Steel, kvp.Value.MaterialType);
        Assert.Equal("EN 1993-1-1:2005", kvp.Value.SteelDesignCodeName);
        Assert.Equal(i++, kvp.Key);
        GsaMaterialTest.DuplicateTest(kvp.Value);
      }
    }

    [Fact]
    public void ImportConcreteMaterialTest() {
      Materials materials = ImportMaterialsMother();
      int i = 1;
      foreach (KeyValuePair<int, GsaMaterial> kvp in materials.ConcreteMaterials) {
        Assert.NotNull(kvp.Value);
        Assert.NotNull(kvp.Value.StandardMaterial);
        Assert.Equal(GsaMaterial.MatType.Concrete, kvp.Value.MaterialType);
        Assert.Equal("EC2-1-1", kvp.Value.ConcreteDesignCodeName);
        Assert.Equal(i++, kvp.Key);
        GsaMaterialTest.DuplicateTest(kvp.Value);
      }
    }

    [Fact]
    public void ImportFrpMaterialTest() {
      Materials materials = ImportMaterialsMother();
      int i = 1;
      foreach (KeyValuePair<int, GsaMaterial> kvp in materials.FrpMaterials) {
        Assert.NotNull(kvp.Value);
        Assert.NotNull(kvp.Value.StandardMaterial);
        Assert.Equal(GsaMaterial.MatType.Frp, kvp.Value.MaterialType);
        Assert.Equal(i++, kvp.Key);
        GsaMaterialTest.DuplicateTest(kvp.Value);
      }
    }

    [Fact]
    public void ImportAluminiumMaterialTest() {
      Materials materials = ImportMaterialsMother();
      int i = 1;
      foreach (KeyValuePair<int, GsaMaterial> kvp in materials.AluminiumMaterials) {
        Assert.NotNull(kvp.Value);
        Assert.NotNull(kvp.Value.StandardMaterial);
        Assert.Equal(GsaMaterial.MatType.Aluminium, kvp.Value.MaterialType);
        Assert.Equal(i++, kvp.Key);
        GsaMaterialTest.DuplicateTest(kvp.Value);
      }
    }

    [Fact]
    public void ImportTimberMaterialTest() {
      Materials materials = ImportMaterialsMother();
      int i = 1;
      foreach (KeyValuePair<int, GsaMaterial> kvp in materials.TimberMaterials) {
        Assert.NotNull(kvp.Value);
        Assert.NotNull(kvp.Value.StandardMaterial);
        Assert.Equal(GsaMaterial.MatType.Timber, kvp.Value.MaterialType);
        Assert.Equal(i++, kvp.Key);
        GsaMaterialTest.DuplicateTest(kvp.Value);
      }
    }

    [Fact]
    public void ImportGlassMaterialTest() {
      Materials materials = ImportMaterialsMother();
      int i = 1;
      foreach (KeyValuePair<int, GsaMaterial> kvp in materials.GlassMaterials) {
        Assert.NotNull(kvp.Value);
        Assert.NotNull(kvp.Value.StandardMaterial);
        Assert.Equal(GsaMaterial.MatType.Glass, kvp.Value.MaterialType);
        Assert.Equal(i++, kvp.Key);
        GsaMaterialTest.DuplicateTest(kvp.Value);
      }
    }

    [Fact]
    public void ImportFabricMaterialTest() {
      Materials materials = ImportMaterialsMother();
      int i = 1;
      foreach (KeyValuePair<int, GsaMaterial> kvp in materials.FabricMaterials) {
        Assert.NotNull(kvp.Value);
        Assert.NotNull(kvp.Value.StandardMaterial);
        Assert.Equal(GsaMaterial.MatType.Fabric, kvp.Value.MaterialType);
        Assert.Equal(i++, kvp.Key);
        Assert.Throws<System.Reflection.TargetInvocationException>(
          () => GsaMaterialTest.DuplicateTest(kvp.Value));
      }
    }

    [Fact]
    public void ImportCustomMaterialTest() {
      Materials materials = ImportMaterialsMother();
      int i = 1;
      foreach (KeyValuePair<int, GsaMaterial> kvp in materials.AnalysisMaterials) {
        Assert.NotNull(kvp.Value);
        Assert.Equal(GsaMaterial.MatType.Generic, kvp.Value.MaterialType);
        Assert.Throws<Exception>(() => kvp.Value.StandardMaterial);
        Assert.Equal(i++, kvp.Key);
        GsaMaterialTest.DuplicateTest(kvp.Value);
      }
    }

    private static Materials ImportMaterialsMother() {
      return new Materials(ImportMaterialsMotherModel());
    }
    private static GsaAPI.Model ImportMaterialsMotherModel() {
      string steelCodeName = "EN 1993-1-1:2005";
      string concreteCodeName = "EC2-1-1";
      var model = new GsaAPI.Model(concreteCodeName, steelCodeName);

      foreach (string grade in GsaMaterial.GetGradeNames(GsaMaterial.MatType.Steel, steelCodeName)) {
        model.AddSteelMaterial(model.CreateSteelMaterial(grade));
      }
      foreach (string grade in
        GsaMaterial.GetGradeNames(GsaMaterial.MatType.Concrete, "", concreteCodeName)) {
        model.AddConcreteMaterial(model.CreateConcreteMaterial(grade));
      }
      foreach (string grade in GsaMaterial.GetGradeNames(GsaMaterial.MatType.Frp)) {
        model.AddFrpMaterial(model.CreateFrpMaterial(grade));
      }
      foreach (string grade in GsaMaterial.GetGradeNames(GsaMaterial.MatType.Aluminium)) {
        model.AddAluminiumMaterial(model.CreateAluminiumMaterial(grade));
      }
      foreach (string grade in GsaMaterial.GetGradeNames(GsaMaterial.MatType.Timber)) {
        model.AddTimberMaterial(model.CreateTimberMaterial(grade));
      }
      foreach (string grade in GsaMaterial.GetGradeNames(GsaMaterial.MatType.Glass)) {
        model.AddGlassMaterial(model.CreateGlassMaterial(grade));
      }
      foreach (string grade in GsaMaterial.GetGradeNames(GsaMaterial.MatType.Fabric)) {
        model.AddFabricMaterial(model.CreateFabricMaterial(grade));
      }

      return model;
    }
  }
}
