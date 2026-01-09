using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Helpers;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;
using OasysGH.Parameters;

using OasysUnits;
using OasysUnits.Units;

using Rhino.Display;
using Rhino.Geometry;

using Xunit;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Properties {
  [Collection("GrasshopperFixture collection")]
  public class EditProp2dTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new Edit2dProperty();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void LoadPanelPropertyIfNotInGlobalAxisWillThrowRunTimeWarning() {
      var prop2d = new GsaProperty2d();
      prop2d.ApiProp2d.Type = Property2D_Type.LOAD;

      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, new GsaProperty2dGoo(prop2d), 0);
      ComponentTestHelper.SetInput(comp, new GH_Integer((int)GsaAPI.StandardAxis.Local), 4);
      var id = (GH_Integer)ComponentTestHelper.GetOutput(comp, 4);
      Assert.Null(id);
      Assert.Contains("One runtime warning", comp.InstanceDescription);

      comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, new GH_Integer((int)GsaAPI.StandardAxis.Global), 4);
      id = (GH_Integer)ComponentTestHelper.GetOutput(comp, 4);
      Assert.Equal((int)GsaAPI.StandardAxis.Global, id.Value);
    }

    [Fact]
    public void GetValuesFromExistingComponent() {
      var prop2d = new GsaProperty2d(new Length(400, LengthUnit.Millimeter));

      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, new GsaProperty2dGoo(prop2d), 0);

      var prop2dGoo = (GsaProperty2dGoo)ComponentTestHelper.GetOutput(comp, 0);
      var id = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      var name = (GH_String)ComponentTestHelper.GetOutput(comp, 2);
      var colour = (GH_Colour)ComponentTestHelper.GetOutput(comp, 3);
      var axis = (GH_Integer)ComponentTestHelper.GetOutput(comp, 4);
      var type = (GH_String)ComponentTestHelper.GetOutput(comp, 5);
      var thickness = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 7);
      var referenceSurface = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 8);
      var offset = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 9);
      var modifier = (GsaProperty2dModifierGoo)ComponentTestHelper.GetOutput(comp, 10);
      var supportType = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 11);
      var referenceEdge = (GH_Integer)ComponentTestHelper.GetOutput(comp, 12);

      Duplicates.AreEqual(prop2d, prop2dGoo.Value, new List<string>() { "Guid" });
      Assert.NotEqual(prop2d, prop2dGoo.Value);
      Assert.Equal(0, id.Value);
      Assert.Equal("", name.Value);
      Assert.Equal(ColorRGBA.Black, colour.Value);
      Assert.Equal(-1, axis.Value);
      Assert.Equal(-1, axis.Value);
      Assert.Equal("Shell", type.Value);
      Assert.Equal(400, thickness.Value.As(LengthUnit.Millimeter), DoubleComparer.Default);
      Assert.Equal(ReferenceSurface.Middle, referenceSurface.Value);
      Assert.Equal(new Length(0, LengthUnit.Centimeter), offset.Value);
      Duplicates.AreEqual(new GsaProperty2dModifier(), modifier.Value);
      Assert.Equal(SupportType.Undefined, supportType.Value);
      Assert.Equal(1, referenceEdge.Value);
    }

    [Fact]
    public void SetPlaneFromNewComponent() {
      GH_OasysComponent comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, new GH_Plane(Plane.WorldYZ), 4);
      var axisPlane = (GH_Plane)ComponentTestHelper.GetOutput(comp, 4);
      Assert.Equal(new GH_Plane(Plane.WorldYZ).ToString(), axisPlane.ToString());
    }

    [Fact]
    public void SetValuesFromNewComponent() {
      var prop2d = new GsaProperty2d();
      GH_OasysComponent comp = ComponentMother();

      var prop2dGoo = (GsaProperty2dGoo)ComponentTestHelper.GetOutput(comp, 0);
      var id = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      var name = (GH_String)ComponentTestHelper.GetOutput(comp, 2);
      var colour = (GH_Colour)ComponentTestHelper.GetOutput(comp, 3);
      var axis = (GH_Integer)ComponentTestHelper.GetOutput(comp, 4);
      var type = (GH_String)ComponentTestHelper.GetOutput(comp, 5);
      var thickness = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 7);
      var referenceSurface = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 8);
      var offset = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 9);
      var modifier = (GsaProperty2dModifierGoo)ComponentTestHelper.GetOutput(comp, 10);
      var supportType = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 11);
      var referenceEdge = (GH_Integer)ComponentTestHelper.GetOutput(comp, 12);

      Duplicates.AreEqual(prop2d, prop2dGoo.Value, new List<string>() { "Guid" });
      Assert.NotEqual(prop2d, prop2dGoo.Value);
      Assert.Equal(0, id.Value);
      Assert.Equal("", name.Value);
      Assert.Equal(ColorRGBA.Black, colour.Value);
      Assert.Equal(-1, axis.Value);
      Assert.Equal("Shell", type.Value);
      Assert.Equal(0, thickness.Value.As(LengthUnit.Millimeter), 6);
      Assert.Equal(ReferenceSurface.Middle, referenceSurface.Value);
      Assert.Equal(new Length(0, LengthUnit.Centimeter), offset.Value);
      Duplicates.AreEqual(new GsaProperty2dModifier(), modifier.Value);
      Assert.Equal(SupportType.Undefined, supportType.Value);
      Assert.Equal(1, referenceEdge.Value);

      ComponentTestHelper.SetInput(comp, new GH_Integer(49), 1);
      ComponentTestHelper.SetInput(comp, "name", 2);
      ComponentTestHelper.SetInput(comp, new GH_Colour(Color.White), 3);
      ComponentTestHelper.SetInput(comp, new GH_Integer(0), 4);
      ComponentTestHelper.SetInput(comp, new GH_String("Load"), 5);
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new Length(40, LengthUnit.Centimeter)), 7);
      ComponentTestHelper.SetInput(comp, new GH_Integer(2), 8); // Bottom
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new Length(10, LengthUnit.Millimeter)), 9);
      ComponentTestHelper.SetInput(comp, new GsaProperty2dModifierGoo(new GsaProperty2dModifier()), 10);
      ComponentTestHelper.SetInput(comp, new GH_String("Cantilever"), 11);
      ComponentTestHelper.SetInput(comp, new GH_Integer(3), 12);

      prop2dGoo = (GsaProperty2dGoo)ComponentTestHelper.GetOutput(comp, 0);
      Assert.Equal(49, prop2dGoo.Value.Id);
      Assert.Equal("name", prop2dGoo.Value.ApiProp2d.Name);
      Assert.Equal(ColorRGBA.White, (Color)prop2dGoo.Value.ApiProp2d.Colour);
      Assert.Equal(0, prop2dGoo.Value.ApiProp2d.AxisProperty);
      Assert.Equal(Property2D_Type.LOAD, prop2dGoo.Value.ApiProp2d.Type);
      Assert.Equal(40, prop2dGoo.Value.Thickness.As(LengthUnit.Centimeter), 6);
      Assert.Equal(ReferenceSurface.Bottom, prop2dGoo.Value.ApiProp2d.ReferenceSurface);
      Assert.Equal(10, prop2dGoo.Value.AdditionalOffsetZ.As(LengthUnit.Millimeter));
      Assert.Equal(1, prop2dGoo.Value.ApiProp2d.PropertyModifier.InPlane.Value);
      Assert.Equal(1, prop2dGoo.Value.ApiProp2d.PropertyModifier.Bending.Value);
      Assert.Equal(1, prop2dGoo.Value.ApiProp2d.PropertyModifier.Shear.Value);
      Assert.Equal(1, prop2dGoo.Value.ApiProp2d.PropertyModifier.Volume.Value);
      Assert.Equal(0, prop2dGoo.Value.ApiProp2d.PropertyModifier.AdditionalMass);
      Assert.Equal(SupportType.Cantilever, prop2dGoo.Value.ApiProp2d.SupportType);
      Assert.Equal(3, prop2dGoo.Value.ApiProp2d.ReferenceEdge);

      id = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      name = (GH_String)ComponentTestHelper.GetOutput(comp, 2);
      colour = (GH_Colour)ComponentTestHelper.GetOutput(comp, 3);
      axis = (GH_Integer)ComponentTestHelper.GetOutput(comp, 4);
      type = (GH_String)ComponentTestHelper.GetOutput(comp, 5);
      thickness = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 7);
      referenceSurface = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 8);
      offset = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 9);
      modifier = (GsaProperty2dModifierGoo)ComponentTestHelper.GetOutput(comp, 10);
      supportType = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 11);
      referenceEdge = (GH_Integer)ComponentTestHelper.GetOutput(comp, 12);

      Assert.Equal(49, id.Value);
      Assert.Equal("name", name.Value);
      Assert.Equal(ColorRGBA.White, colour.Value);
      Assert.Equal(0, axis.Value);
      Assert.Equal("Load Panel", type.Value);
      Assert.Equal(new Length(10, LengthUnit.Millimeter), offset.Value);
      Assert.Equal(ReferenceSurface.Bottom, referenceSurface.Value);
      Assert.Equal(new Ratio(100, RatioUnit.Percent), modifier.Value.InPlane);
      Assert.Equal(new Ratio(100, RatioUnit.Percent), modifier.Value.Bending);
      Assert.Equal(new Ratio(100, RatioUnit.Percent), modifier.Value.Shear);
      Assert.Equal(new Ratio(100, RatioUnit.Percent), modifier.Value.Volume);
      Assert.Equal(new AreaDensity(0, AreaDensityUnit.KilogramPerSquareMeter), modifier.Value.AdditionalMass);
      Assert.Equal(3, referenceEdge.Value);
      Assert.Equal(SupportType.Cantilever, supportType.Value);
      Assert.Equal(3, referenceEdge.Value);
    }

    [Fact]
    public void UpdateCustomUITest() {
      var comp = (Edit2dProperty)ComponentMother();
      comp.Update("ft");
      Assert.Equal("ft", comp.Message);
    }
  }
}
