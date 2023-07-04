using GsaGH.Components;
using GsaGH.Helpers.Export;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using GsaGHTests.Model;
using Oasys.Taxonomy.Profiles;
using OasysGH.Components;
using System.Collections.Generic;
using Xunit;
using static GsaGH.Parameters.GsaMaterial;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class CreateMaterialTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateMaterial();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void CreateComponent() {
      GH_OasysDropDownComponent comp = ComponentMother();

      var output = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(MatType.Concrete, output.Value.MaterialType);

      var expected = new GsaMaterial(MatType.Concrete, "C30/37", "EC2-1-1");

      Duplicates.AreEqual(expected, output.Value);
    }

    [Fact]
    public void StandardMaterialsSurviveRoundTrip() {
      var createMaterialComponent = (CreateMaterial)ComponentMother();

      for (int i = 0; i < createMaterialComponent._dropDownItems.Count; i++) {
        createMaterialComponent.SetSelected(i, 0);

        for (int j = 0; j < createMaterialComponent._dropDownItems[i].Count; j++) {
          createMaterialComponent.SetSelected(i, j);
          var material
            = (GsaMaterialGoo)ComponentTestHelper.GetOutput(createMaterialComponent, 0);
          TestMaterialSurvivesRoundTrip(material);
        }
      }
    }

    private void TestMaterialSurvivesRoundTrip(GsaMaterialGoo material) {
      var section = new CreateSection();
      section.CreateAttributes();
      ComponentTestHelper.SetInput(section, "STD R 400 400", 0);
      ComponentTestHelper.SetInput(section, material, 1);
      var sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(section);

      GH_OasysDropDownComponent createModel =
        CreateModelTest.CreateModelFromProperties(new List<GsaSectionGoo>() {
          sectionGoo,
        }, null, null);

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(createModel);
      
      GsaMaterial assembledMaterial = null;
      switch (material.Value.MaterialType) {
        case MatType.Steel:
          assembledMaterial = modelGoo.Value.Materials.SteelMaterials[1];
          break;

        case MatType.Concrete:
          assembledMaterial = modelGoo.Value.Materials.ConcreteMaterials[1];
          break;

        case MatType.Aluminium:
          assembledMaterial = modelGoo.Value.Materials.AluminiumMaterials[1];
          break;

        case MatType.Timber:
          assembledMaterial = modelGoo.Value.Materials.TimberMaterials[1];
          break;

        case MatType.Frp:
          assembledMaterial = modelGoo.Value.Materials.FrpMaterials[1];
          break;

        case MatType.Glass:
          assembledMaterial = modelGoo.Value.Materials.GlassMaterials[1];
          break;

        case MatType.Fabric:
          assembledMaterial = modelGoo.Value.Materials.FabricMaterials[1];
          Assert.NotNull(assembledMaterial);
          assembledMaterial.Id = 0;
          Duplicates.AreEqual(material.Value.StandardMaterial, assembledMaterial.StandardMaterial);
          return;
      }
      
      Assert.NotNull(assembledMaterial);
      assembledMaterial.Id = 0;
      Duplicates.AreEqual(material.Value, assembledMaterial, true);
    }
  }
}
