using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using OasysUnits;

using Xunit;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class EditSectionTests {

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
        ApiSection = new Section() {
          Profile = "STD I 300 400 10 20",
          Pool = 12,
          Name = "byggemandbob",
          Colour = Color.Red,
          BasicOffset = BasicOffset.Top,
        },
        Material = material.Value,
        Modifier = modifier.Value,
        AdditionalOffsetY = new Length(10.0, LengthUnit.Millimeter),
        AdditionalOffsetZ = new Length(-23.0, LengthUnit.Millimeter)
      };

      ComponentTestHelper.SetInput(comp, section, 0);
      ComponentTestHelper.SetInput(comp, 42, 1);
      ComponentTestHelper.SetInput(comp, "STD I 300 400 10 20", 2);
      ComponentTestHelper.SetInput(comp, material, 3);
      ComponentTestHelper.SetInput(comp, BasicOffset.Top, 4);
      ComponentTestHelper.SetInput(comp, 1.0, 5);
      ComponentTestHelper.SetInput(comp, -2.3, 6);
      ComponentTestHelper.SetInput(comp, modifier, 7);
      ComponentTestHelper.SetInput(comp, 12, 8);
      ComponentTestHelper.SetInput(comp, "byggemandbob", 9);
      ComponentTestHelper.SetInput(comp, Color.Red, 10);

      var sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 0);
      var id = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      var profile = (GH_String)ComponentTestHelper.GetOutput(comp, 2);
      var mat = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp, 3);
      var basicOffset = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 4);
      var addiontalOffsetY = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 5);
      var addiontalOffsetZ = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 6);
      var mod = (GsaSectionModifierGoo)ComponentTestHelper.GetOutput(comp, 7);
      var pool = (GH_Integer)ComponentTestHelper.GetOutput(comp, 8);
      var name = (GH_String)ComponentTestHelper.GetOutput(comp, 9);
      var colour = (GH_Colour)ComponentTestHelper.GetOutput(comp, 10);

      Duplicates.AreEqual(edit, sectionGoo.Value, new List<string>() { "Guid" });
      Assert.Equal(42, id.Value);
      Assert.Equal("STD I 300 400 10 20", profile.Value);
      Duplicates.AreEqual(material.Value, mat.Value);
      Duplicates.AreEqual(BasicOffset.Top, basicOffset.Value);
      Duplicates.AreEqual(new Length(1.0, LengthUnit.Centimeter), addiontalOffsetY.Value);
      Duplicates.AreEqual(new Length(-2.3, LengthUnit.Centimeter), addiontalOffsetZ.Value);
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
      ComponentTestHelper.SetInput(comp, Color.Blue, 10);
      var sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 0);

      Assert.Equal(Color.Blue.A, ((Color)sectionGoo.Value.ApiSection.Colour).A);
      Assert.Equal(Color.Blue.R, ((Color)sectionGoo.Value.ApiSection.Colour).R);
      Assert.Equal(Color.Blue.G, ((Color)sectionGoo.Value.ApiSection.Colour).G);
      Assert.Equal(Color.Blue.B, ((Color)sectionGoo.Value.ApiSection.Colour).B);
      var colour = (GH_Colour)ComponentTestHelper.GetOutput(comp, 10);
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
      var material = (GsaMaterialGoo)ComponentTestHelper.GetOutput(
        CreateCustomMaterialTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, material, 3);
      var sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 0);
      Duplicates.AreEqual(material.Value, sectionGoo.Value.Material);
      var mat = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp, 3);
      Duplicates.AreEqual(material.Value, mat.Value);
    }

    [Fact]
    public void TestModifier() {
      GH_OasysComponent comp = ComponentMother();
      var modifier = (GsaSectionModifierGoo)ComponentTestHelper.GetOutput(
        CreateSectionModifierTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, modifier, 7);
      var sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 0);
      Duplicates.AreEqual(modifier.Value, sectionGoo.Value.Modifier);
      var mod = (GsaSectionModifierGoo)ComponentTestHelper.GetOutput(comp, 7);
      Duplicates.AreEqual(modifier.Value, mod.Value);
    }

    [Fact]
    public void TestName() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, "John", 9);
      var sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 0);
      Assert.Equal("John", sectionGoo.Value.ApiSection.Name);
      var name = (GH_String)ComponentTestHelper.GetOutput(comp, 9);
      Assert.Equal("John", name.Value);
    }

    [Fact]
    public void TestPool() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, 99, 8);
      var sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 0);
      Assert.Equal(99, sectionGoo.Value.ApiSection.Pool);
      var pool = (GH_Integer)ComponentTestHelper.GetOutput(comp, 8);
      Assert.Equal(99, pool.Value);
    }

    [Fact]
    public void TestProfile() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, "STD I 300 400 10 20", 2);
      var sectionGoo = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 0);
      Assert.Equal("STD I 300 400 10 20", sectionGoo.Value.ApiSection.Profile);
      var profile = (GH_String)ComponentTestHelper.GetOutput(comp, 2);
      Assert.Equal("STD I 300 400 10 20", profile.Value);
    }

    [Fact]
    public void UpdateCustomUITest() {
      var comp = (EditSection)ComponentMother();
      comp.Update("ft");
      Assert.Equal("ft", comp.Message);
    }
  }
}
