using Grasshopper.Kernel.Types;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using OasysGH.Parameters;
using Xunit;
using static GsaGH.Parameters.GsaMaterial;

namespace GsaGHTests.Components.Properties
{
  [Collection("GrasshopperFixture collection")]
  public class EditSectionTests
  {
    public static GH_OasysComponent ComponentMother()
    {
      var comp = new EditSection();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void CreateComponentTest()
    {
      // Arrange
      var comp = ComponentMother();

      GsaSectionGoo section = (GsaSectionGoo)ComponentTestHelper.GetOutput(CreateSectionTests.ComponentMother("STD CH(mm) 40 30 2 1"));

      GsaMaterialGoo material = (GsaMaterialGoo)ComponentTestHelper.GetOutput(CreateCustomMaterialTests.ComponentMother());

      GsaSectionModifierGoo modifier = (GsaSectionModifierGoo)ComponentTestHelper.GetOutput(CreateSectionModifierTests.ComponentMother());

      GsaSection edit = new GsaSection();
      edit.Id = 42;
      edit.Profile = "STD I 300 400 10 20";
      edit.Material = material.Value;
      edit.Modifier = modifier.Value;
      edit.Pool = 12;
      edit.Name = "byggemandbob";
      edit.Colour = System.Drawing.Color.Red;

      // Act
      // 0: section
      // 1: id
      // 2: profile
      // 3: material
      // 4: modifier
      // 5: pool
      // 6: name
      // 7: colour

      ComponentTestHelper.SetInput(comp, section, 0);
      ComponentTestHelper.SetInput(comp, 42, 1);
      ComponentTestHelper.SetInput(comp, "STD I 300 400 10 20", 2);
      ComponentTestHelper.SetInput(comp, material, 3);
      ComponentTestHelper.SetInput(comp, modifier, 4);
      ComponentTestHelper.SetInput(comp, 12, 5);
      ComponentTestHelper.SetInput(comp, "byggemandbob", 6);
      ComponentTestHelper.SetInput(comp, System.Drawing.Color.Red, 7);

      int i = 0;
      GsaSectionGoo sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, i++);
      GH_Integer id = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);
      GH_String profile = (GH_String)ComponentTestHelper.GetOutput(comp, i++);
      GsaMaterialGoo mat = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp, i++);
      GsaSectionModifierGoo mod = (GsaSectionModifierGoo)ComponentTestHelper.GetOutput(comp, i++);
      GH_Integer pool = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);
      GH_String name = (GH_String)ComponentTestHelper.GetOutput(comp, i++);
      GH_Colour colour = (GH_Colour)ComponentTestHelper.GetOutput(comp, i++);

      // Assert
      Duplicates.AreEqual(edit, sectionGoo.Value);
      Assert.Equal(42, id.Value);
      Assert.Equal("STD I 300 400 10 20", profile.Value);
      Duplicates.AreEqual(material.Value, mat.Value);
      Duplicates.AreEqual(modifier.Value, mod.Value);
      Assert.Equal(12, pool.Value);
      Assert.Equal("byggemandbob", name.Value);
      Assert.Equal(System.Drawing.Color.Red.A, colour.Value.A);
      Assert.Equal(System.Drawing.Color.Red.R, colour.Value.R);
      Assert.Equal(System.Drawing.Color.Red.G, colour.Value.G);
      Assert.Equal(System.Drawing.Color.Red.B, colour.Value.B);
    }
  }
}
