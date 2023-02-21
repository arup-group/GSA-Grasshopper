using Grasshopper.Kernel.Types;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Display;
using Rhino.Geometry;
using Xunit;
using static GsaGH.Parameters.GsaMaterial;

namespace GsaGHTests.Properties
{
  [Collection("GrasshopperFixture collection")]
  public class EditProp2dTests
  {
    public static GH_OasysComponent ComponentMother()
    {
      var comp = new EditProp2d();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void GetValuesFromExistingComponent()
    {
      GsaProp2d prop2d = new GsaProp2d(new Length(400, LengthUnit.Millimeter));

      var comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, new GsaProp2dGoo(prop2d), 0);

      int i = 0;
      GsaProp2dGoo prop2dGoo = (GsaProp2dGoo)ComponentTestHelper.GetOutput(comp, i++);
      GH_Integer id = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);
      GsaMaterialGoo mat = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber thk = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_Integer axis = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);
      GH_String name = (GH_String)ComponentTestHelper.GetOutput(comp, i++);
      GH_Colour colour = (GH_Colour)ComponentTestHelper.GetOutput(comp, i++);
      GH_String type = (GH_String)ComponentTestHelper.GetOutput(comp, i++);

      Duplicates.AreEqual(prop2d, prop2dGoo.Value);
      Assert.NotEqual(prop2d, prop2dGoo.Value);
      Assert.Equal(0, id.Value);
      GsaMaterial expectedMat = new GsaMaterial();
      Duplicates.AreEqual(expectedMat, mat.Value);
      Assert.Equal(400, thk.Value.As(LengthUnit.Millimeter), 6);
      Assert.Equal(-1, axis.Value);
      Assert.Equal("", name.Value);
      Assert.Equal(ColorRGBA.Black, colour.Value);
      Assert.Equal("Shell", type.Value);
    }

    [Fact]
    public void SetValuesFromNewComponent()
    {
      GsaProp2d prop2d = new GsaProp2d();
      var comp = ComponentMother();

      int i = 0;
      GsaProp2dGoo prop2dGoo = (GsaProp2dGoo)ComponentTestHelper.GetOutput(comp, i++);
      GH_Integer id = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);
      GsaMaterialGoo mat = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber thk = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_Integer axis = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);
      GH_String name = (GH_String)ComponentTestHelper.GetOutput(comp, i++);
      GH_Colour colour = (GH_Colour)ComponentTestHelper.GetOutput(comp, i++);
      GH_String type = (GH_String)ComponentTestHelper.GetOutput(comp, i++);

      Duplicates.AreEqual(prop2d, prop2dGoo.Value);
      Assert.NotEqual(prop2d, prop2dGoo.Value);
      Assert.Equal(0, id.Value);
      GsaMaterial expectedMat = new GsaMaterial();
      Duplicates.AreEqual(expectedMat, mat.Value);
      Assert.Equal(0, thk.Value.As(LengthUnit.Millimeter), 6);
      Assert.Equal(-1, axis.Value);
      Assert.Equal("", name.Value);
      Assert.Equal(ColorRGBA.Black, colour.Value);
      Assert.Equal("Shell", type.Value);

      i = 1;
      ComponentTestHelper.SetInput(comp, new GH_Integer(49), i++);
      expectedMat = new GsaMaterial(8);
      ComponentTestHelper.SetInput(comp, new GsaMaterialGoo(expectedMat), i++);
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new Length(40, LengthUnit.Centimeter)), i++);
      ComponentTestHelper.SetInput(comp, new GH_Integer(7), i++);
      ComponentTestHelper.SetInput(comp, new GH_String("MyPropediprop"), i++);
      ComponentTestHelper.SetInput(comp, new GH_Colour(System.Drawing.Color.White), i++);
      ComponentTestHelper.SetInput(comp, new GH_String("Curved Shell"), i++);

      i = 0;
      prop2dGoo = (GsaProp2dGoo)ComponentTestHelper.GetOutput(comp, i++);
      id = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);
      mat = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp, i++);
      thk = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      axis = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);
      name = (GH_String)ComponentTestHelper.GetOutput(comp, i++);
      colour = (GH_Colour)ComponentTestHelper.GetOutput(comp, i++);
      type = (GH_String)ComponentTestHelper.GetOutput(comp, i++);

      Assert.Equal(49, id.Value);
      Duplicates.AreEqual(expectedMat, mat.Value);
      Assert.Equal(40, thk.Value.As(LengthUnit.Centimeter), 6);
      Assert.Equal(7, axis.Value);
      Assert.Equal("MyPropediprop", name.Value);
      Assert.Equal(ColorRGBA.White, colour.Value);
      Assert.Equal("Curved Shell", type.Value);
    }

    [Fact]
    public void SetPlaneFromNewComponent()
    {
      GsaProp2d prop2d = new GsaProp2d();
      var comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, new GH_Plane(Plane.WorldYZ), 4);
      GH_Plane axisPlane = (GH_Plane)ComponentTestHelper.GetOutput(comp, 4);
      Assert.Equal(new GH_Plane(Plane.WorldYZ).ToString(), axisPlane.ToString());
    }
  }
}
