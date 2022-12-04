using GH_IO.Serialization;
using Grasshopper.Kernel.Types;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.UI;
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
    public void TestID()
    {
      var comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, 42, 1);
      
      GsaSectionGoo sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 0);
      Assert.Equal(42, sectionGoo.Value.Id);
      GH_Integer id = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      Assert.Equal(42, id.Value);
    }

    [Fact]
    public void TestProfile()
    {
      var comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, "STD I 300 400 10 20", 2);

      GsaSectionGoo sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 0);
      Assert.Equal("STD I 300 400 10 20", sectionGoo.Value.Profile);
      GH_String profile = (GH_String)ComponentTestHelper.GetOutput(comp, 2);
      Assert.Equal("STD I 300 400 10 20", profile.Value);
    }

    [Fact]
    public void TestMaterial()
    {
      var comp = ComponentMother();

      GsaMaterialGoo material = (GsaMaterialGoo)ComponentTestHelper.GetOutput(CreateCustomMaterialTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, material, 3);

      GsaSectionGoo sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 0);
      Duplicates.AreEqual(material.Value, sectionGoo.Value.Material);
      GsaMaterialGoo mat = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp, 3);
      Duplicates.AreEqual(material.Value, mat.Value);
    }

    [Fact]
    public void TestModifier()
    {
      var comp = ComponentMother();

      GsaSectionModifierGoo modifier = (GsaSectionModifierGoo)ComponentTestHelper.GetOutput(CreateSectionModifierTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, modifier, 4);

      GsaSectionGoo sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 0);
      Duplicates.AreEqual(modifier.Value, sectionGoo.Value.Modifier);
      GsaSectionModifierGoo mod = (GsaSectionModifierGoo)ComponentTestHelper.GetOutput(comp, 4);
      Duplicates.AreEqual(modifier.Value, mod.Value);
    }

    [Fact]
    public void TestPool()
    {
      var comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, 99, 5);

      GsaSectionGoo sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 0);
      Assert.Equal(99, sectionGoo.Value.Pool);
      GH_Integer pool = (GH_Integer)ComponentTestHelper.GetOutput(comp, 5);
      Assert.Equal(99, pool.Value);
    }

    [Fact]
    public void TestName()
    {
      var comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, "John", 6);

      GsaSectionGoo sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 0);
      Assert.Equal("John", sectionGoo.Value.Name);
      GH_String name = (GH_String)ComponentTestHelper.GetOutput(comp, 6);
      Assert.Equal("John", name.Value);
    }

    [Fact]
    public void TestColour()
    {
      var comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, System.Drawing.Color.Blue, 7);

      GsaSectionGoo sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 0);
      Assert.Equal(System.Drawing.Color.Blue.A, sectionGoo.Value.Colour.A);
      Assert.Equal(System.Drawing.Color.Blue.R, sectionGoo.Value.Colour.R);
      Assert.Equal(System.Drawing.Color.Blue.G, sectionGoo.Value.Colour.G);
      Assert.Equal(System.Drawing.Color.Blue.B, sectionGoo.Value.Colour.B);
      GH_Colour colour = (GH_Colour)ComponentTestHelper.GetOutput(comp, 7);
      Assert.Equal(System.Drawing.Color.Blue.A, colour.Value.A);
      Assert.Equal(System.Drawing.Color.Blue.R, colour.Value.R);
      Assert.Equal(System.Drawing.Color.Blue.G, colour.Value.G);
      Assert.Equal(System.Drawing.Color.Blue.B, colour.Value.B);
    }

    [Fact]
    public void EditExistingSectionTest()
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
