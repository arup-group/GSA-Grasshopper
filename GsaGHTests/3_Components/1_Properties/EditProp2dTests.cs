﻿using System.Drawing;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysUnits;
using Rhino.Display;
using Rhino.Geometry;
using Xunit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Properties {
  [Collection("GrasshopperFixture collection")]
  public class EditProp2dTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new EditProp2d();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void GetValuesFromExistingComponent() {
      var prop2d = new GsaProp2d(new Length(400, LengthUnit.Millimeter));

      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, new GsaProp2dGoo(prop2d), 0);

      var prop2dGoo = (GsaProp2dGoo)ComponentTestHelper.GetOutput(comp, 0);
      var id = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      var name = (GH_String)ComponentTestHelper.GetOutput(comp, 2);
      var colour = (GH_Colour)ComponentTestHelper.GetOutput(comp, 3);
      var axis = (GH_Integer)ComponentTestHelper.GetOutput(comp, 4);
      var type = (GH_String)ComponentTestHelper.GetOutput(comp, 5);
      var mat = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp, 6);
      var thk = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 7);
      var referenceSurface = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 8);
      var offset = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 9);
      var modifier = (GsaProp2dModifierGoo)ComponentTestHelper.GetOutput(comp, 10);
      var supportType = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 11);
      var referenceEdge = (GH_Integer)ComponentTestHelper.GetOutput(comp, 12);

      Duplicates.AreEqual(prop2d, prop2dGoo.Value);
      Assert.NotEqual(prop2d, prop2dGoo.Value);
      Assert.Equal(0, id.Value);
      var expectedMat = new GsaMaterial();
      Duplicates.AreEqual(expectedMat, mat.Value);
      Assert.Equal(400, thk.Value.As(LengthUnit.Millimeter), 6);
      Assert.Equal(-1, axis.Value);
      Assert.Equal(SupportType.Undefined, supportType.Value);
      Assert.Equal(1, referenceEdge.Value);
      Assert.Equal("", name.Value);
      Assert.Equal(ColorRGBA.Black, colour.Value);
      Assert.Equal("Shell", type.Value);
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
      var prop2d = new GsaProp2d();
      GH_OasysComponent comp = ComponentMother();

      int i = 0;
      var prop2dGoo = (GsaProp2dGoo)ComponentTestHelper.GetOutput(comp, i++);
      var id = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);
      var mat = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp, i++);
      var thk = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var axis = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);
      var supportType = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, i++);
      var referenceEdge = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);
      var name = (GH_String)ComponentTestHelper.GetOutput(comp, i++);
      var colour = (GH_Colour)ComponentTestHelper.GetOutput(comp, i++);
      var type = (GH_String)ComponentTestHelper.GetOutput(comp, i);

      Duplicates.AreEqual(prop2d, prop2dGoo.Value);
      Assert.NotEqual(prop2d, prop2dGoo.Value);
      Assert.Equal(0, id.Value);
      var expectedMat = new GsaMaterial();
      Duplicates.AreEqual(expectedMat, mat.Value);
      Assert.Equal(0, thk.Value.As(LengthUnit.Millimeter), 6);
      Assert.Equal(-1, axis.Value);
      Assert.Equal(SupportType.Undefined, supportType.Value);
      Assert.Equal(1, referenceEdge.Value);
      Assert.Equal("", name.Value);
      Assert.Equal(ColorRGBA.Black, colour.Value);
      Assert.Equal("Shell", type.Value);

      i = 1;
      ComponentTestHelper.SetInput(comp, new GH_Integer(49), i++);
      expectedMat = new GsaMaterial(8);
      ComponentTestHelper.SetInput(comp, new GsaMaterialGoo(expectedMat), i++);
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new Length(40, LengthUnit.Centimeter)),
        i++);
      ComponentTestHelper.SetInput(comp, new GH_Integer(7), i++);
      ComponentTestHelper.SetInput(comp, new GH_String("Load Panel"), 9);
      ComponentTestHelper.SetInput(comp, new GH_String("Cantilever"), i++);
      ComponentTestHelper.SetInput(comp, new GH_Integer(3), i++);
      ComponentTestHelper.SetInput(comp, new GH_String("MyPropediprop"), i++);
      ComponentTestHelper.SetInput(comp, new GH_Colour(Color.White), i++);

      i = 0;
      prop2dGoo = (GsaProp2dGoo)ComponentTestHelper.GetOutput(comp, i++);
      Assert.Equal(49, prop2dGoo.Value.Id);
      Duplicates.AreEqual(expectedMat, prop2dGoo.Value.Material);
      Assert.Equal(40, prop2dGoo.Value.Thickness.As(LengthUnit.Centimeter), 6);
      Assert.Equal(7, prop2dGoo.Value.AxisProperty);
      Assert.Equal(SupportType.Cantilever, prop2dGoo.Value.SupportType);
      Assert.Equal(3, prop2dGoo.Value.ReferenceEdge);
      Assert.Equal("MyPropediprop", prop2dGoo.Value.Name);
      Assert.Equal("LOAD", prop2dGoo.Value.Type.ToString());
      id = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);
      mat = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp, i++);
      thk = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      axis = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);
      supportType = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, i++);
      referenceEdge = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);
      name = (GH_String)ComponentTestHelper.GetOutput(comp, i++);
      colour = (GH_Colour)ComponentTestHelper.GetOutput(comp, i++);
      type = (GH_String)ComponentTestHelper.GetOutput(comp, i);

      Assert.Equal(49, id.Value);
      Duplicates.AreEqual(expectedMat, mat.Value);
      Assert.Equal(40, thk.Value.As(LengthUnit.Centimeter), 6);
      Assert.Equal(7, axis.Value);

      Assert.Equal(SupportType.Cantilever, (SupportType)supportType.Value);
      Assert.Equal(3, referenceEdge.Value);
      Assert.Equal("MyPropediprop", name.Value);
      Assert.Equal(ColorRGBA.White, colour.Value);
      Assert.Equal("Load Panel", type.Value);
    }
  }
}
