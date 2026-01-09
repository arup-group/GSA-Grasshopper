using GsaAPI;

using GsaGH.Parameters;

using Xunit;

namespace GsaGHTests.Helpers.Import {
  [Collection("GrasshopperFixture collection")]
  public class PropertiesTest {
    [Fact]
    public void ImportSectionsTest() {
      GsaAPI.Model model = ImportPropertiesMotherModel();
      var gsaModel = new GsaModel(model);

      Assert.Equal("STD R 800 400", gsaModel.Sections[1].Value.ApiSection.Profile);
      Duplicates.AreEqual(gsaModel.Materials.SteelMaterials[1], gsaModel.Sections[1].Value.Material);
      Assert.Equal("STD R 800 400", gsaModel.Sections[2].Value.ApiSection.Profile);
      Duplicates.AreEqual(gsaModel.Materials.ConcreteMaterials[1], gsaModel.Sections[2].Value.Material);
      Assert.Equal("STD R 800 400", gsaModel.Sections[3].Value.ApiSection.Profile);
      Duplicates.AreEqual(gsaModel.Materials.FrpMaterials[1], gsaModel.Sections[3].Value.Material);
      Assert.Equal("STD R 800 400", gsaModel.Sections[4].Value.ApiSection.Profile);
      Duplicates.AreEqual(gsaModel.Materials.AluminiumMaterials[1], gsaModel.Sections[4].Value.Material);
      Assert.Equal("STD R 800 400", gsaModel.Sections[5].Value.ApiSection.Profile);
      Duplicates.AreEqual(gsaModel.Materials.TimberMaterials[1], gsaModel.Sections[5].Value.Material);
      Assert.Equal("STD R 800 400", gsaModel.Sections[6].Value.ApiSection.Profile);
      Duplicates.AreEqual(gsaModel.Materials.GlassMaterials[1], gsaModel.Sections[6].Value.Material);
    }

    [Fact]
    public void ImportProp2dsTest() {
      GsaAPI.Model model = ImportPropertiesMotherModel();
      var gsaModel = new GsaModel(model);

      Assert.Equal(200, gsaModel.Prop2ds[1].Value.Thickness.Millimeters);
      Duplicates.AreEqual(gsaModel.Materials.SteelMaterials[1], gsaModel.Prop2ds[1].Value.Material);
      Assert.Equal(200, gsaModel.Prop2ds[2].Value.Thickness.Millimeters);
      Duplicates.AreEqual(gsaModel.Materials.ConcreteMaterials[1], gsaModel.Prop2ds[2].Value.Material);
      Assert.Equal(200, gsaModel.Prop2ds[3].Value.Thickness.Millimeters);
      Duplicates.AreEqual(gsaModel.Materials.FrpMaterials[1], gsaModel.Prop2ds[3].Value.Material);
      Assert.Equal(200, gsaModel.Prop2ds[4].Value.Thickness.Millimeters);
      Duplicates.AreEqual(gsaModel.Materials.AluminiumMaterials[1], gsaModel.Prop2ds[4].Value.Material);
      Assert.Equal(200, gsaModel.Prop2ds[5].Value.Thickness.Millimeters);
      Duplicates.AreEqual(gsaModel.Materials.TimberMaterials[1], gsaModel.Prop2ds[5].Value.Material);
      Assert.Equal(200, gsaModel.Prop2ds[6].Value.Thickness.Millimeters);
      Duplicates.AreEqual(gsaModel.Materials.GlassMaterials[1], gsaModel.Prop2ds[6].Value.Material);
      Assert.Equal(200, gsaModel.Prop2ds[7].Value.Thickness.Millimeters);
      Assert.Equal(gsaModel.Materials.FabricMaterials[1].Name, gsaModel.Prop2ds[7].Value.Material.Name);
    }

    [Fact]
    public void ImportProp3dsTest() {
      GsaAPI.Model model = ImportPropertiesMotherModel();
      var gsaModel = new GsaModel(model);

      Duplicates.AreEqual(gsaModel.Materials.SteelMaterials[1], gsaModel.Prop3ds[1].Value.Material);
      Duplicates.AreEqual(gsaModel.Materials.ConcreteMaterials[1], gsaModel.Prop3ds[2].Value.Material);
      Duplicates.AreEqual(gsaModel.Materials.FrpMaterials[1], gsaModel.Prop3ds[3].Value.Material);
      Duplicates.AreEqual(gsaModel.Materials.AluminiumMaterials[1], gsaModel.Prop3ds[4].Value.Material);
      Duplicates.AreEqual(gsaModel.Materials.TimberMaterials[1], gsaModel.Prop3ds[5].Value.Material);
      Duplicates.AreEqual(gsaModel.Materials.GlassMaterials[1], gsaModel.Prop3ds[6].Value.Material);
    }

    internal static GsaAPI.Model ImportPropertiesMotherModel() {
      GsaAPI.Model model = MaterialsTest.ImportMaterialsMotherModel();

      model.AddSection(new Section() {
        MaterialType = MaterialType.STEEL, // 1
        MaterialGradeProperty = 1,
        MaterialAnalysisProperty = 0,
        Profile = "STD R 800 400"
      });

      model.AddSection(new Section() {
        MaterialType = MaterialType.CONCRETE, // 2
        MaterialGradeProperty = 1,
        MaterialAnalysisProperty = 0,
        Profile = "STD R 800 400"
      });

      model.AddSection(new Section() {
        MaterialType = MaterialType.FRP, // 3
        MaterialGradeProperty = 1,
        MaterialAnalysisProperty = 0,
        Profile = "STD R 800 400"
      });

      model.AddSection(new Section() {
        MaterialType = MaterialType.ALUMINIUM, // 4
        MaterialGradeProperty = 1,
        MaterialAnalysisProperty = 0,
        Profile = "STD R 800 400"
      });

      model.AddSection(new Section() {
        MaterialType = MaterialType.TIMBER, // 5
        MaterialGradeProperty = 1,
        MaterialAnalysisProperty = 0,
        Profile = "STD R 800 400"
      });

      model.AddSection(new Section() {
        MaterialType = MaterialType.GLASS, // 6
        MaterialGradeProperty = 1,
        MaterialAnalysisProperty = 0,
        Profile = "STD R 800 400"
      });

      model.AddProp2D(new Prop2D() {
        MaterialType = MaterialType.STEEL, // 1
        MaterialGradeProperty = 1,
        MaterialAnalysisProperty = 0,
        Description = "200(mm)"
      });

      model.AddProp2D(new Prop2D() {
        MaterialType = MaterialType.CONCRETE, // 2
        MaterialGradeProperty = 1,
        MaterialAnalysisProperty = 0,
        Description = "200(mm)"
      });

      model.AddProp2D(new Prop2D() {
        MaterialType = MaterialType.FRP, // 3
        MaterialGradeProperty = 1,
        MaterialAnalysisProperty = 0,
        Description = "200(mm)"
      });

      model.AddProp2D(new Prop2D() {
        MaterialType = MaterialType.ALUMINIUM, // 4
        MaterialGradeProperty = 1,
        MaterialAnalysisProperty = 0,
        Description = "200(mm)"
      });

      model.AddProp2D(new Prop2D() {
        MaterialType = MaterialType.TIMBER, // 5
        MaterialGradeProperty = 1,
        MaterialAnalysisProperty = 0,
        Description = "200(mm)"
      });

      model.AddProp2D(new Prop2D() {
        MaterialType = MaterialType.GLASS, // 6
        MaterialGradeProperty = 1,
        MaterialAnalysisProperty = 0,
        Description = "200(mm)"
      });

      model.AddProp2D(new Prop2D() {
        MaterialType = MaterialType.FABRIC, // 7
        MaterialGradeProperty = 1,
        MaterialAnalysisProperty = 0,
        Description = "200(mm)"
      });

      //property 8
      model.AddProp2D(new Prop2D() {
        Type = Property2D_Type.LOAD
      }); ;

      model.AddProp3D(new Prop3D() {
        MaterialType = MaterialType.STEEL, // 1
        MaterialGradeProperty = 1,
        MaterialAnalysisProperty = 0,
      });

      model.AddProp3D(new Prop3D() {
        MaterialType = MaterialType.CONCRETE, // 2
        MaterialGradeProperty = 1,
        MaterialAnalysisProperty = 0,
      });

      model.AddProp3D(new Prop3D() {
        MaterialType = MaterialType.FRP, // 3
        MaterialGradeProperty = 1,
        MaterialAnalysisProperty = 0,
      });

      model.AddProp3D(new Prop3D() {
        MaterialType = MaterialType.ALUMINIUM, // 4
        MaterialGradeProperty = 1,
        MaterialAnalysisProperty = 0,
      });

      model.AddProp3D(new Prop3D() {
        MaterialType = MaterialType.TIMBER, // 5
        MaterialGradeProperty = 1,
        MaterialAnalysisProperty = 0,
      });

      model.AddProp3D(new Prop3D() {
        MaterialType = MaterialType.GLASS, // 6
        MaterialGradeProperty = 1,
        MaterialAnalysisProperty = 0,
      });

      return model;
    }
  }
}
