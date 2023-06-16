using GsaAPI;
using GsaGH.Helpers.Import;
using Xunit;

namespace GsaGHTests.Helpers.Import {
  [Collection("GrasshopperFixture collection")]
  public class PropertiesTest {
    [Fact]
    public void ImportSectionsTest() {
      GsaAPI.Model model = ImportPropertiesMotherModel();
      Materials materials = MaterialsTest.ImportMaterialsMother();
      var properties = new GsaGH.Helpers.Import.Properties(model, materials);

      Assert.Equal("STD R 800 400", properties.Sections[1].Value.Profile);
      Duplicates.AreEqual(materials.SteelMaterials[1], properties.Sections[1].Value.Material);
      Assert.Equal("STD R 800 400", properties.Sections[2].Value.Profile);
      Duplicates.AreEqual(materials.ConcreteMaterials[1], properties.Sections[2].Value.Material);
      Assert.Equal("STD R 800 400", properties.Sections[3].Value.Profile);
      Duplicates.AreEqual(materials.FrpMaterials[1], properties.Sections[3].Value.Material);
      Assert.Equal("STD R 800 400", properties.Sections[4].Value.Profile);
      Duplicates.AreEqual(materials.AluminiumMaterials[1], properties.Sections[4].Value.Material);
      Assert.Equal("STD R 800 400", properties.Sections[5].Value.Profile);
      Duplicates.AreEqual(materials.TimberMaterials[1], properties.Sections[5].Value.Material);
      Assert.Equal("STD R 800 400", properties.Sections[6].Value.Profile);
      Duplicates.AreEqual(materials.GlassMaterials[1], properties.Sections[6].Value.Material);
    }

    [Fact]
    public void ImportProp2dsTest() {
      GsaAPI.Model model = ImportPropertiesMotherModel();
      Materials materials = MaterialsTest.ImportMaterialsMother();
      var properties = new GsaGH.Helpers.Import.Properties(model, materials);

      Assert.Equal(200, properties.Prop2ds[1].Value.Thickness.Millimeters);
      Duplicates.AreEqual(materials.SteelMaterials[1], properties.Prop2ds[1].Value.Material);
      Assert.Equal(200, properties.Prop2ds[2].Value.Thickness.Millimeters);
      Duplicates.AreEqual(materials.ConcreteMaterials[1], properties.Prop2ds[2].Value.Material);
      Assert.Equal(200, properties.Prop2ds[3].Value.Thickness.Millimeters);
      Duplicates.AreEqual(materials.FrpMaterials[1], properties.Prop2ds[3].Value.Material);
      Assert.Equal(200, properties.Prop2ds[4].Value.Thickness.Millimeters);
      Duplicates.AreEqual(materials.AluminiumMaterials[1], properties.Prop2ds[4].Value.Material);
      Assert.Equal(200, properties.Prop2ds[5].Value.Thickness.Millimeters);
      Duplicates.AreEqual(materials.TimberMaterials[1], properties.Prop2ds[5].Value.Material);
      Assert.Equal(200, properties.Prop2ds[6].Value.Thickness.Millimeters);
      Duplicates.AreEqual(materials.GlassMaterials[1], properties.Prop2ds[6].Value.Material);
      Assert.Equal(200, properties.Prop2ds[7].Value.Thickness.Millimeters);
      Assert.Equal(materials.FabricMaterials[1].Name, properties.Prop2ds[7].Value.Material.Name);
    }

    [Fact]
    public void ImportProp3dsTest() {
      GsaAPI.Model model = ImportPropertiesMotherModel();
      Materials materials = MaterialsTest.ImportMaterialsMother();
      var properties = new GsaGH.Helpers.Import.Properties(model, materials);

      Duplicates.AreEqual(materials.SteelMaterials[1], properties.Prop3ds[1].Value.Material);
      Duplicates.AreEqual(materials.ConcreteMaterials[1], properties.Prop3ds[2].Value.Material);
      Duplicates.AreEqual(materials.FrpMaterials[1], properties.Prop3ds[3].Value.Material);
      Duplicates.AreEqual(materials.AluminiumMaterials[1], properties.Prop3ds[4].Value.Material);
      Duplicates.AreEqual(materials.TimberMaterials[1], properties.Prop3ds[5].Value.Material);
      Duplicates.AreEqual(materials.GlassMaterials[1], properties.Prop3ds[6].Value.Material);
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
