using System.Drawing;

using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using OasysUnits;

using Rhino.Geometry;

using Xunit;

using LengthUnit = OasysUnits.Units.LengthUnit;
using Line = Rhino.Geometry.Line;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class EditMember2dTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new Edit2dMember();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp,
        ComponentTestHelper.GetOutput(CreateMember2dTests.ComponentMother()), 0);

      return comp;
    }

    [Fact]
    public void CreateComponentTest1() {
      GH_OasysComponent comp = ComponentMother();

      var output0 = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp, 0);
      var output1 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      var output2 = (GH_Brep)ComponentTestHelper.GetOutput(comp, 2);
      var output5 = (GsaProperty2dGoo)ComponentTestHelper.GetOutput(comp, 5);
      var output6 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 6);
      var output7 = (GH_String)ComponentTestHelper.GetOutput(comp, 7);
      var output8 = (GH_String)ComponentTestHelper.GetOutput(comp, 8);
      var output9 = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, 9);
      var output10 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 10);
      var output11 = (GH_Number)ComponentTestHelper.GetOutput(comp, 11);
      var output12 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 12);
      var output13 = (GH_String)ComponentTestHelper.GetOutput(comp, 13);
      var output14 = (GH_Number)ComponentTestHelper.GetOutput(comp, 14);
      var output15 = (GH_String)ComponentTestHelper.GetOutput(comp, 15);
      var output16 = (GH_Colour)ComponentTestHelper.GetOutput(comp, 16);
      var output17 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 17);
      var output18 = (GH_String)ComponentTestHelper.GetOutput(comp, 18);

      Assert.Equal(100, output0.Value.Brep.GetArea());
      Assert.Equal(Property2D_Type.PLATE, output0.Value.Prop2d.ApiProp2d.Type);
      Assert.Equal(new Length(14, LengthUnit.Inch), output0.Value.Prop2d.Thickness);
      Assert.Equal(0.5, output0.Value.ApiMember.MeshSize);
      Assert.Equal(0, output1.Value);
      Assert.Equal(100, output2.Value.GetArea());
      Assert.Equal(Property2D_Type.PLATE, output5.Value.ApiProp2d.Type);
      Assert.Equal(new Length(14, LengthUnit.Inch), output5.Value.Thickness);
      Assert.Equal(0, output6.Value);
      Assert.Equal("Generic 2D", output7.Value);
      Assert.Equal("Linear", output8.Value);
      Assert.Equal(0, output9.Value.X1.Value);
      Assert.Equal(0, output9.Value.X2.Value);
      Assert.Equal(0, output9.Value.Y.Value);
      Assert.Equal(0, output9.Value.Z.Value);
      Assert.True(output10.Value);
      Assert.Equal(0.5, output11.Value);
      Assert.True(output12.Value);
      Assert.Equal("Mixed", output13.Value);
      Assert.Equal(0.0, output14.Value);
      Assert.Equal("", output15.Value);
      Assert.Equal(0, output16.Value.R);
      Assert.Equal(0, output16.Value.G);
      Assert.Equal(0, output16.Value.B);
      Assert.False(output17.Value);
      Assert.Equal("", output18.Value);
    }

    [Fact]
    public void CreateComponentTest2() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, 7, 1);
      ComponentTestHelper.SetInput(comp,
        Brep.CreateFromCornerPoints(new Point3d(0, 0, 0), new Point3d(4, 0, 0),
          new Point3d(4, 4, 0), new Point3d(0, 4, 0), 1), 2);
      ComponentTestHelper.SetInput(comp,
        new GsaProperty2dGoo(new GsaProperty2d(new Length(200, LengthUnit.Millimeter))), 5);
      ComponentTestHelper.SetInput(comp, 1, 6);
      ComponentTestHelper.SetInput(comp, "Ribbed Slab", 7);
      ComponentTestHelper.SetInput(comp, "Rigid Diaphragm", 8);
      ComponentTestHelper.SetInput(comp, new GsaOffsetGoo(new GsaOffset(1, 2, 3, 4)), 9);
      ComponentTestHelper.SetInput(comp, true, 10);
      ComponentTestHelper.SetInput(comp, 0.7, 11);
      ComponentTestHelper.SetInput(comp, false, 12); // is intersector
      ComponentTestHelper.SetInput(comp, 3, 13); // tri mode
      ComponentTestHelper.SetInput(comp, "name", 15);
      ComponentTestHelper.SetInput(comp, new GH_Colour(Color.White), 16);
      ComponentTestHelper.SetInput(comp, true, 17);

      var output0 = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp, 0);
      var output1 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      var output2 = (GH_Brep)ComponentTestHelper.GetOutput(comp, 2);
      var output5 = (GsaProperty2dGoo)ComponentTestHelper.GetOutput(comp, 5);
      var output6 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 6);
      var output7 = (GH_String)ComponentTestHelper.GetOutput(comp, 7);
      var output8 = (GH_String)ComponentTestHelper.GetOutput(comp, 8);
      var output9 = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, 9);
      var output10 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 10);
      var output11 = (GH_Number)ComponentTestHelper.GetOutput(comp, 11);
      var output12 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 12);
      var output13 = (GH_String)ComponentTestHelper.GetOutput(comp, 13);
      var output14 = (GH_Number)ComponentTestHelper.GetOutput(comp, 14);
      var output15 = (GH_String)ComponentTestHelper.GetOutput(comp, 15);
      var output16 = (GH_Colour)ComponentTestHelper.GetOutput(comp, 16);
      var output17 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 17);
      var output18 = (GH_String)ComponentTestHelper.GetOutput(comp, 18);

      Assert.Equal(16, output0.Value.Brep.GetArea());
      Assert.Equal(Property2D_Type.SHELL, output0.Value.Prop2d.ApiProp2d.Type);
      Assert.Equal(new Length(200, LengthUnit.Millimeter), output0.Value.Prop2d.Thickness);
      Assert.Equal(0.7, output0.Value.ApiMember.MeshSize);
      Assert.Equal(7, output1.Value);
      Assert.Equal(16, output2.Value.GetArea());
      Assert.Equal(Property2D_Type.SHELL, output5.Value.ApiProp2d.Type);
      Assert.Equal(new Length(200, LengthUnit.Millimeter), output5.Value.Thickness);
      Assert.Equal(1, output6.Value);
      Assert.Equal("Ribbed Slab", output7.Value);
      Assert.Equal("Rigid Diaphragm", output8.Value);
      Assert.Equal(1, output9.Value.X1.Value);
      Assert.Equal(2, output9.Value.X2.Value);
      Assert.Equal(3, output9.Value.Y.Value);
      Assert.Equal(4, output9.Value.Z.Value);
      Assert.True(output10.Value);
      Assert.Equal(0.7, output11.Value);
      Assert.False(output12.Value);
      Assert.Equal("Tri", output13.Value);
      Assert.Equal("name", output15.Value);
      Assert.Equal(255, output16.Value.R);
      Assert.Equal(255, output16.Value.G);
      Assert.Equal(255, output16.Value.B);
      Assert.True(output17.Value);
      Assert.Equal("", output18.Value);
    }

    [Theory]
    [InlineData(1, "Mixed")]
    [InlineData(3, "Tri")]
    [InlineData(4, "Quad")]
    public void SetMeshModeTest(int mode, string expected) {
      GH_OasysComponent comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, mode, 13);
      var output13 = (GH_String)ComponentTestHelper.GetOutput(comp, 13);
      Assert.Equal(expected, output13.Value);
    }

    [Fact]
    public void CreateComponentTest3() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp,
        Brep.CreateFromCornerPoints(new Point3d(0, 0, 0), new Point3d(4, 0, 0),
          new Point3d(4, 4, 0), new Point3d(0, 4, 0), 1), 2);
      ComponentTestHelper.SetInput(comp, new Point3d(2, 2, 0), 3);
      ComponentTestHelper.SetInput(comp, new Line(new Point3d(3, 0, 0), new Point3d(3, 3, 0)), 4);

      var output0 = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp, 0);
      var output3 = (GH_Point)ComponentTestHelper.GetOutput(comp, 3);
      var output4 = (GH_Curve)ComponentTestHelper.GetOutput(comp, 4);

      Assert.Single(output0.Value.InclusionPoints);
      Assert.Single(output0.Value.InclusionLines);
      Assert.Equal(2, output3.Value.X);
      Assert.Equal(2, output3.Value.Y);
      Assert.Equal(0, output3.Value.Z);
      Assert.Equal(3, output4.Value.PointAtStart.X);
      Assert.Equal(0, output4.Value.PointAtStart.Y);
      Assert.Equal(0, output4.Value.PointAtStart.Z);
      Assert.Equal(3, output4.Value.PointAtEnd.X);
      Assert.Equal(3, output4.Value.PointAtEnd.Y);
      Assert.Equal(0, output4.Value.PointAtEnd.Z);
    }

    [Theory]
    [InlineData((int)GsaAPI.AnalysisOrder.LINEAR)]
    [InlineData((int)GsaAPI.AnalysisOrder.QUADRATIC)]
    [InlineData((int)GsaAPI.AnalysisOrder.RIGID_DIAPHRAGM)]
    [InlineData((int)GsaAPI.AnalysisOrder.LOAD_PANEL)]
    public void CheckAnaysisOrderIsWorkingAsExpected(int analysisOrder) {
      GH_OasysComponent comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, analysisOrder, 8);
      var output8 = (GH_String)ComponentTestHelper.GetOutput(comp, 8);
      Assert.Equal((GsaAPI.AnalysisOrder)analysisOrder, Mappings.GetAnalysisOrder(output8.Value));
    }
  }
}
