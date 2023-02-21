﻿using System.Drawing;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Components.Geometry
{
  [Collection("GrasshopperFixture collection")]
  public class EditMember2dTests
  {
    public static GH_OasysComponent ComponentMother()
    {
      var comp = new EditMember2d();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, ComponentTestHelper.GetOutput(CreateMember2dTests.ComponentMother()), 0);

      return comp;
    }

    [Fact]
    public void CreateComponentTest1()
    {
      // Arrange & Act
      var comp = ComponentMother();

      // Assert
      GsaMember2dGoo output0 = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp, 0);
      GH_Integer output1 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      GH_Brep output2 = (GH_Brep)ComponentTestHelper.GetOutput(comp, 2);
      //GH_Point output3 = (GH_Point)ComponentTestHelper.GetOutput(comp, 3);
      //GH_Curve output4 = (GH_Curve)ComponentTestHelper.GetOutput(comp, 4);
      GsaProp2dGoo output5 = (GsaProp2dGoo)ComponentTestHelper.GetOutput(comp, 5);
      GH_Integer output6 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 6);
      GH_String output7 = (GH_String)ComponentTestHelper.GetOutput(comp, 7);
      GH_String output8 = (GH_String)ComponentTestHelper.GetOutput(comp, 8);
      GsaOffsetGoo output9 = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, 9);
      GH_Number output10 = (GH_Number)ComponentTestHelper.GetOutput(comp, 10);
      GH_Boolean output11 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 11);
      GH_Number output12 = (GH_Number)ComponentTestHelper.GetOutput(comp, 12);
      GH_String output13 = (GH_String)ComponentTestHelper.GetOutput(comp, 13);
      GH_Colour output14 = (GH_Colour)ComponentTestHelper.GetOutput(comp, 14);
      GH_Boolean output15 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 15);
      GH_String output16 = (GH_String)ComponentTestHelper.GetOutput(comp, 16);

      Assert.Equal(100, output0.Value.Brep.GetArea());
      Assert.Equal(Property2D_Type.PLATE, output0.Value.Property.Type);
      Assert.Equal(new Length(14, LengthUnit.Inch), output0.Value.Property.Thickness);
      Assert.Equal(0.5, output0.Value.MeshSize);
      Assert.Equal(0, output1.Value);
      Assert.Equal(100, output2.Value.GetArea());
      //Assert.Equal(Null, output3.Value);
      //Assert.Equal(Null, output4.Value);
      Assert.Equal(Property2D_Type.PLATE, output5.Value.Type);
      Assert.Equal(new Length(14, LengthUnit.Inch), output5.Value.Thickness);
      Assert.Equal(0, output6.Value);
      Assert.Equal("Generic 2D", output7.Value);
      Assert.Equal("Linear", output8.Value);
      Assert.Equal(0, output9.Value.X1.Value);
      Assert.Equal(0, output9.Value.X2.Value);
      Assert.Equal(0, output9.Value.Y.Value);
      Assert.Equal(0, output9.Value.Z.Value);
      Assert.Equal(0.5, output10.Value);
      Assert.True(output11.Value);
      Assert.Equal(0.0, output12.Value);
      Assert.Equal("", output13.Value);
      Assert.Equal(0, output14.Value.R);
      Assert.Equal(0, output14.Value.G);
      Assert.Equal(0, output14.Value.B);
      Assert.False(output15.Value);
      Assert.Equal("", output16.Value);
    }

    [Fact]
    public void CreateComponentTest2()
    {
      // Arrange & Act
      var comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, 7, 1);
      ComponentTestHelper.SetInput(comp, Brep.CreateFromCornerPoints(new Point3d(0, 0, 0), new Point3d(4, 0, 0), new Point3d(4, 4, 0), new Point3d(0, 4, 0), 1), 2);
      ComponentTestHelper.SetInput(comp, new GsaProp2dGoo(new GsaProp2d(new Length(200, LengthUnit.Millimeter))), 5);
      ComponentTestHelper.SetInput(comp, 1, 6);
      ComponentTestHelper.SetInput(comp, "Ribbed Slab", 7);
      ComponentTestHelper.SetInput(comp, "Rigid Diaphragm", 8);
      ComponentTestHelper.SetInput(comp, new GsaOffsetGoo(new GsaOffset(1, 2, 3, 4)), 9);
      ComponentTestHelper.SetInput(comp, 0.7, 10);
      ComponentTestHelper.SetInput(comp, false, 11);
      ComponentTestHelper.SetInput(comp, "name", 13);
      ComponentTestHelper.SetInput(comp, new GH_Colour(Color.White), 14);
      ComponentTestHelper.SetInput(comp, true, 15);

      // Assert
      GsaMember2dGoo output0 = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp, 0);
      GH_Integer output1 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      GH_Brep output2 = (GH_Brep)ComponentTestHelper.GetOutput(comp, 2);
      //GH_Point output3 = (GH_Point)ComponentTestHelper.GetOutput(comp, 3);
      //GH_Curve output4 = (GH_Curve)ComponentTestHelper.GetOutput(comp, 4);
      GsaProp2dGoo output5 = (GsaProp2dGoo)ComponentTestHelper.GetOutput(comp, 5);
      GH_Integer output6 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 6);
      GH_String output7 = (GH_String)ComponentTestHelper.GetOutput(comp, 7);
      GH_String output8 = (GH_String)ComponentTestHelper.GetOutput(comp, 8);
      GsaOffsetGoo output9 = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, 9);
      GH_Number output10 = (GH_Number)ComponentTestHelper.GetOutput(comp, 10);
      GH_Boolean output11 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 11);
      GH_Number output12 = (GH_Number)ComponentTestHelper.GetOutput(comp, 12);
      GH_String output13 = (GH_String)ComponentTestHelper.GetOutput(comp, 13);
      GH_Colour output14 = (GH_Colour)ComponentTestHelper.GetOutput(comp, 14);
      GH_Boolean output15 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 15);
      GH_String output16 = (GH_String)ComponentTestHelper.GetOutput(comp, 16);

      Assert.Equal(16, output0.Value.Brep.GetArea());
      Assert.Equal(Property2D_Type.SHELL, output0.Value.Property.Type);
      Assert.Equal(new Length(200, LengthUnit.Millimeter), output0.Value.Property.Thickness);
      Assert.Equal(0.7, output0.Value.MeshSize);
      Assert.Equal(7, output1.Value);
      Assert.Equal(16, output2.Value.GetArea());
      //Assert.Equal(Null, output3.Value);
      //Assert.Equal(Null, output4.Value);
      Assert.Equal(Property2D_Type.SHELL, output5.Value.Type);
      Assert.Equal(new Length(200, LengthUnit.Millimeter), output5.Value.Thickness);
      Assert.Equal(1, output6.Value);
      Assert.Equal("Ribbed Slab", output7.Value);
      Assert.Equal("Rigid Diaphragm", output8.Value);
      Assert.Equal(1, output9.Value.X1.Value);
      Assert.Equal(2, output9.Value.X2.Value);
      Assert.Equal(3, output9.Value.Y.Value);
      Assert.Equal(4, output9.Value.Z.Value);
      Assert.Equal(0.7, output10.Value);
      Assert.True(output11.Value); // overridden by IsDummy
      Assert.Equal(0.0, output12.Value);
      Assert.Equal("name", output13.Value);
      Assert.Equal(255, output14.Value.R);
      Assert.Equal(255, output14.Value.G);
      Assert.Equal(255, output14.Value.B);
      Assert.True(output15.Value);
      Assert.Equal("", output16.Value);
    }

    [Fact]
    public void CreateComponentTest3()
    {
      // Arrange & Act
      var comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, Brep.CreateFromCornerPoints(new Point3d(0, 0, 0), new Point3d(4, 0, 0), new Point3d(4, 4, 0), new Point3d(0, 4, 0), 1), 2);
      ComponentTestHelper.SetInput(comp, new Point3d(2, 2, 0), 3);
      ComponentTestHelper.SetInput(comp, new Line(new Point3d(3, 0, 0), new Point3d(3, 3, 0)), 4);

      // Assert
      GsaMember2dGoo output0 = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp, 0);
      GH_Point output3 = (GH_Point)ComponentTestHelper.GetOutput(comp, 3);
      GH_Curve output4 = (GH_Curve)ComponentTestHelper.GetOutput(comp, 4);

      //Assert.Equal("1 2 3 4 P(5 6) L(7 8)", output0.Value.ApiMember.Topology);
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
  }
}

