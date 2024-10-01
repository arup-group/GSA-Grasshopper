using System.Collections.Generic;

using GsaAPI.Materials;

using GsaGH.Parameters;

using GsaGHTests.Parameters;

using Xunit;

namespace GsaGHTests.Helpers.Import {
  [Collection("GrasshopperFixture collection")]
  public class MaterialsTest {
    [Fact]
    public void ImportSteelMaterialTest() {
      GsaMaterials materials = ImportMaterialsMother();
      Assert.True(materials.SteelMaterials.Count > 1);
      int i = 1;
      foreach (KeyValuePair<int, GsaMaterial> kvp in materials.SteelMaterials) {
        Assert.NotNull(kvp.Value);
        Assert.Equal(MatType.Steel, kvp.Value.MaterialType);
        Assert.Equal("EN 1993-1-1:2005", kvp.Value.SteelDesignCodeName);
        Assert.Equal(i++, kvp.Key);
        GsaMaterialTest.DuplicateTest(kvp.Value);
        Assert.False(kvp.Value.IsUserDefined);
      }
    }

    [Fact]
    public void ImportConcreteMaterialTest() {
      GsaMaterials materials = ImportMaterialsMother();
      Assert.True(materials.ConcreteMaterials.Count > 1);
      int i = 1;
      foreach (KeyValuePair<int, GsaMaterial> kvp in materials.ConcreteMaterials) {
        Assert.NotNull(kvp.Value);
        Assert.NotNull(((IGsaStandardMaterial)kvp.Value).StandardMaterial);
        Assert.Equal(MatType.Concrete, kvp.Value.MaterialType);
        Assert.Equal("EC2-1-1", kvp.Value.ConcreteDesignCodeName);
        Assert.Equal(i++, kvp.Key);
        GsaMaterialTest.DuplicateTest(kvp.Value);
        Assert.False(kvp.Value.IsUserDefined);
      }
    }

    [Fact]
    public void ImportFrpMaterialTest() {
      GsaMaterials materials = ImportMaterialsMother();
      Assert.True(materials.FrpMaterials.Count > 1);
      int i = 1;
      foreach (KeyValuePair<int, GsaMaterial> kvp in materials.FrpMaterials) {
        Assert.NotNull(kvp.Value);
        Assert.NotNull(((IGsaStandardMaterial)kvp.Value).StandardMaterial);
        Assert.Equal(MatType.Frp, kvp.Value.MaterialType);
        Assert.Equal(i++, kvp.Key);
        GsaMaterialTest.DuplicateTest(kvp.Value);
        Assert.False(kvp.Value.IsUserDefined);
      }
    }

    [Fact]
    public void ImportAluminiumMaterialTest() {
      GsaMaterials materials = ImportMaterialsMother();
      Assert.True(materials.AluminiumMaterials.Count > 1);
      int i = 1;
      foreach (KeyValuePair<int, GsaMaterial> kvp in materials.AluminiumMaterials) {
        Assert.NotNull(kvp.Value);
        Assert.NotNull(((IGsaStandardMaterial)kvp.Value).StandardMaterial);
        Assert.Equal(MatType.Aluminium, kvp.Value.MaterialType);
        Assert.Equal(i++, kvp.Key);
        GsaMaterialTest.DuplicateTest(kvp.Value);
        Assert.False(kvp.Value.IsUserDefined);
      }
    }

    [Fact]
    public void ImportTimberMaterialTest() {
      GsaMaterials materials = ImportMaterialsMother();
      Assert.True(materials.TimberMaterials.Count > 1);
      int i = 1;
      foreach (KeyValuePair<int, GsaMaterial> kvp in materials.TimberMaterials) {
        Assert.NotNull(kvp.Value);
        Assert.NotNull(((IGsaStandardMaterial)kvp.Value).StandardMaterial);
        Assert.Equal(MatType.Timber, kvp.Value.MaterialType);
        Assert.Equal(i++, kvp.Key);
        GsaMaterialTest.DuplicateTest(kvp.Value);
        Assert.False(kvp.Value.IsUserDefined);
      }
    }

    [Fact]
    public void ImportGlassMaterialTest() {
      GsaMaterials materials = ImportMaterialsMother();
      Assert.True(materials.GlassMaterials.Count > 1);
      int i = 1;
      foreach (KeyValuePair<int, GsaMaterial> kvp in materials.GlassMaterials) {
        Assert.NotNull(kvp.Value);
        Assert.NotNull(((IGsaStandardMaterial)kvp.Value).StandardMaterial);
        Assert.Equal(MatType.Glass, kvp.Value.MaterialType);
        Assert.Equal(i++, kvp.Key);
        GsaMaterialTest.DuplicateTest(kvp.Value);
        Assert.False(kvp.Value.IsUserDefined);
      }
    }

    [Fact]
    public void ImportFabricMaterialTest() {
      GsaMaterials materials = ImportMaterialsMother();
      Assert.True(materials.FabricMaterials.Count > 1);
      int i = 1;
      foreach (KeyValuePair<int, GsaMaterial> kvp in materials.FabricMaterials) {
        Assert.NotNull(kvp.Value);
        Assert.NotNull(((IGsaStandardMaterial)kvp.Value).StandardMaterial);
        Assert.Equal(MatType.Fabric, kvp.Value.MaterialType);
        Assert.Equal(i++, kvp.Key);
        Assert.False(kvp.Value.IsUserDefined);
        GsaMaterialTest.DuplicateTest(kvp.Value);
      }
    }

    [Fact]
    public void ImportCustomMaterialTest() {
      GsaMaterials materials = ImportMaterialsMother();
      Assert.True(materials.AnalysisMaterials.Count > 1);
      int i = 1;
      foreach (KeyValuePair<int, GsaMaterial> kvp in materials.AnalysisMaterials) {
        Assert.NotNull(kvp.Value);
        Assert.Equal(MatType.Custom, kvp.Value.MaterialType);
        GsaMaterialTest.DuplicateTest(kvp.Value);
        Assert.Equal(i++, kvp.Key);
        Assert.True(kvp.Value.IsUserDefined);
      }
    }

    internal static GsaAPI.Model ImportMaterialsMotherModel() {
      string steelCodeName = "EN 1993-1-1:2005";
      string concreteCodeName = "EC2-1-1";
      var model = new GsaAPI.Model(concreteCodeName, steelCodeName);

      foreach (string grade in GsaMaterialFactory.GetGradeNames(MatType.Steel, string.Empty, steelCodeName)) {
        model.AddSteelMaterial(model.CreateSteelMaterial(grade));
      }
      foreach (string grade in
        GsaMaterialFactory.GetGradeNames(MatType.Concrete, concreteCodeName, string.Empty)) {
        model.AddConcreteMaterial(model.CreateConcreteMaterial(grade));
      }
      foreach (string grade in GsaMaterialFactory.GetGradeNames(MatType.Frp)) {
        model.AddFrpMaterial(model.CreateFrpMaterial(grade));
      }
      foreach (string grade in GsaMaterialFactory.GetGradeNames(MatType.Aluminium)) {
        model.AddAluminiumMaterial(model.CreateAluminiumMaterial(grade));
      }
      foreach (string grade in GsaMaterialFactory.GetGradeNames(MatType.Timber)) {
        model.AddTimberMaterial(model.CreateTimberMaterial(grade));
      }
      foreach (string grade in GsaMaterialFactory.GetGradeNames(MatType.Glass)) {
        model.AddGlassMaterial(model.CreateGlassMaterial(grade));
      }
      foreach (string grade in GsaMaterialFactory.GetGradeNames(MatType.Fabric)) {
        model.AddFabricMaterial(model.CreateFabricMaterial(grade));
      }

      model.AddAnalysisMaterial(new AnalysisMaterial() {
        CoefficientOfThermalExpansion = 0.05,
        Density = 7800,
        ElasticModulus = 205000,
        Name = "mySteel",
        PoissonsRatio = 0.3,
      });

      model.AddAnalysisMaterial(new AnalysisMaterial() {
        CoefficientOfThermalExpansion = 0.025,
        Density = 2450,
        ElasticModulus = 32000,
        Name = "myConcrete",
        PoissonsRatio = 0.25,
      });

      return model;
    }

    internal static GsaMaterials ImportMaterialsMother() {
      return new GsaMaterials(ImportMaterialsMotherModel());
    }
  }
}
