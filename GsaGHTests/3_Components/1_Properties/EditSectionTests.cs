using System.Drawing;
using Grasshopper.Kernel.Types;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.Components.Properties {

  [Collection("GrasshopperFixture collection")]
  public class EditSectionTests {

    #region Public Methods
    public static GH_OasysComponent ComponentMother() {
      var comp = new EditSection();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void EditExistingSectionTest() {
      GH_OasysComponent comp = ComponentMother();

      var section
        = (GsaSectionGoo)ComponentTestHelper.GetOutput(
          CreateSectionTests.ComponentMother("STD CH(mm) 40 30 2 1"));

      var material
        = (GsaMaterialGoo)ComponentTestHelper.GetOutput(CreateCustomMaterialTests
          .ComponentMother());

      var modifier
        = (GsaSectionModifierGoo)ComponentTestHelper.GetOutput(CreateSectionModifierTests
          .ComponentMother());

      var edit = new GsaSection {
        Id = 42,
        Profile = "STD I 300 400 10 20",
        Material = material.Value,
        Modifier = modifier.Value,
        Pool = 12,
        Name = "byggemandbob",
        Colour = Color.Red,
      };

      ComponentTestHelper.SetInput(comp, section, 0);
      ComponentTestHelper.SetInput(comp, 42, 1);
      ComponentTestHelper.SetInput(comp, "STD I 300 400 10 20", 2);
      ComponentTestHelper.SetInput(comp, material, 3);
      ComponentTestHelper.SetInput(comp, modifier, 4);
      ComponentTestHelper.SetInput(comp, 12, 5);
      ComponentTestHelper.SetInput(comp, "byggemandbob", 6);
      ComponentTestHelper.SetInput(comp, Color.Red, 7);

      int i = 0;
      var sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, i++);
      var id = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);
      var profile = (GH_String)ComponentTestHelper.GetOutput(comp, i++);
      var mat = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp, i++);
      var mod = (GsaSectionModifierGoo)ComponentTestHelper.GetOutput(comp, i++);
      var pool = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);
      var name = (GH_String)ComponentTestHelper.GetOutput(comp, i++);
      var colour = (GH_Colour)ComponentTestHelper.GetOutput(comp, i++);

      Duplicates.AreEqual(edit, sectionGoo.Value);
      Assert.Equal(42, id.Value);
      Assert.Equal("STD I 300 400 10 20", profile.Value);
      Duplicates.AreEqual(material.Value, mat.Value);
      Duplicates.AreEqual(modifier.Value, mod.Value);
      Assert.Equal(12, pool.Value);
      Assert.Equal("byggemandbob", name.Value);
      Assert.Equal(Color.Red.A, colour.Value.A);
      Assert.Equal(Color.Red.R, colour.Value.R);
      Assert.Equal(Color.Red.G, colour.Value.G);
      Assert.Equal(Color.Red.B, colour.Value.B);
    }

    [Fact]
    public void TestColour() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, Color.Blue, 7);
      var sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 0);

      Assert.Equal(Color.Blue.A, sectionGoo.Value.Colour.A);
      Assert.Equal(Color.Blue.R, sectionGoo.Value.Colour.R);
      Assert.Equal(Color.Blue.G, sectionGoo.Value.Colour.G);
      Assert.Equal(Color.Blue.B, sectionGoo.Value.Colour.B);
      var colour = (GH_Colour)ComponentTestHelper.GetOutput(comp, 7);
      Assert.Equal(Color.Blue.A, colour.Value.A);
      Assert.Equal(Color.Blue.R, colour.Value.R);
      Assert.Equal(Color.Blue.G, colour.Value.G);
      Assert.Equal(Color.Blue.B, colour.Value.B);
    }

    [Fact]
    public void TestId() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, 42, 1);
      var sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 0);
      Assert.Equal(42, sectionGoo.Value.Id);
      var id = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      Assert.Equal(42, id.Value);
    }

    [Fact]
    public void TestMaterial() {
      GH_OasysComponent comp = ComponentMother();
      var material
        = (GsaMaterialGoo)ComponentTestHelper.GetOutput(CreateCustomMaterialTests
          .ComponentMother());
      ComponentTestHelper.SetInput(comp, material, 3);
      var sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 0);
      Duplicates.AreEqual(material.Value, sectionGoo.Value.Material);
      var mat = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp, 3);
      Duplicates.AreEqual(material.Value, mat.Value);
    }

    [Fact]
    public void TestModifier() {
      GH_OasysComponent comp = ComponentMother();
      var modifier
        = (GsaSectionModifierGoo)ComponentTestHelper.GetOutput(CreateSectionModifierTests
          .ComponentMother());
      ComponentTestHelper.SetInput(comp, modifier, 4);
      var sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 0);
      Duplicates.AreEqual(modifier.Value, sectionGoo.Value.Modifier);
      var mod = (GsaSectionModifierGoo)ComponentTestHelper.GetOutput(comp, 4);
      Duplicates.AreEqual(modifier.Value, mod.Value);
    }

    [Fact]
    public void TestName() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, "John", 6);
      var sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 0);
      Assert.Equal("John", sectionGoo.Value.Name);
      var name = (GH_String)ComponentTestHelper.GetOutput(comp, 6);
      Assert.Equal("John", name.Value);
    }

    [Fact]
    public void TestPool() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, 99, 5);
      var sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 0);
      Assert.Equal(99, sectionGoo.Value.Pool);
      var pool = (GH_Integer)ComponentTestHelper.GetOutput(comp, 5);
      Assert.Equal(99, pool.Value);
    }

    [Fact]
    public void TestProfile() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, "STD I 300 400 10 20", 2);
      var sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 0);
      Assert.Equal("STD I 300 400 10 20", sectionGoo.Value.Profile);
      var profile = (GH_String)ComponentTestHelper.GetOutput(comp, 2);
      Assert.Equal("STD I 300 400 10 20", profile.Value);
    }

    #endregion Public Methods
  }
}
