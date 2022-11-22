using Grasshopper.Kernel.Types;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Rhino.Geometry;
using System;
using System.Drawing;
using Xunit;

namespace GsaGHTests.Components.Geometry
{
  [Collection("GrasshopperFixture collection")]
  public class EditMember1dTests
  {
    public static GH_OasysComponent ComponentMother()
    {
      var comp = new EditMember1d();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, ComponentTestHelper.GetOutput(CreateMember1dTests.ComponentMother()), 0);

      return comp;
    }

    [Fact]
    public void CreateComponentTest1()
    {
      // Arrange & Act
      var comp = ComponentMother();

      // Assert
      GsaMember1dGoo output0 = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp, 0);
      GH_Integer output1 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      GH_Curve output2 = (GH_Curve)ComponentTestHelper.GetOutput(comp, 2);
      GsaSectionGoo output3 = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 3);
      GH_Integer output4 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 4);
      GH_String output5 = (GH_String)ComponentTestHelper.GetOutput(comp, 5);
      GH_String output6 = (GH_String)ComponentTestHelper.GetOutput(comp, 6);
      GsaOffsetGoo output7 = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, 7);
      GsaBool6Goo output8 = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 8);
      GsaBool6Goo output9 = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 9);
      GH_Number output10 = (GH_Number)ComponentTestHelper.GetOutput(comp, 10);
      GsaNodeGoo output11 = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp, 11);
      GH_Number output12 = (GH_Number)ComponentTestHelper.GetOutput(comp, 12);
      GH_Boolean output13 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 13);
      GsaBucklingLengthFactorsGoo output14 = (GsaBucklingLengthFactorsGoo)ComponentTestHelper.GetOutput(comp, 14);
      GH_String output15 = (GH_String)ComponentTestHelper.GetOutput(comp, 15);
      GH_Colour output16 = (GH_Colour)ComponentTestHelper.GetOutput(comp, 16);
      GH_Boolean output17 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 17);
      GH_String output18 = (GH_String)ComponentTestHelper.GetOutput(comp, 18);

      Assert.Equal(0, output0.Value.PolyCurve.PointAtStart.X);
      Assert.Equal(-1, output0.Value.PolyCurve.PointAtStart.Y);
      Assert.Equal(0, output0.Value.PolyCurve.PointAtStart.Z);
      Assert.Equal(7, output0.Value.PolyCurve.PointAtEnd.X);
      Assert.Equal(3, output0.Value.PolyCurve.PointAtEnd.Y);
      Assert.Equal(1, output0.Value.PolyCurve.PointAtEnd.Z);
      Assert.Equal("STD CH(ft) 1 2 3 4", output0.Value.Section.Profile);
      Assert.Equal(0, output1.Value);
      Assert.Equal(0, output2.Value.PointAtStart.X);
      Assert.Equal(-1, output2.Value.PointAtStart.Y);
      Assert.Equal(0, output2.Value.PointAtStart.Z);
      Assert.Equal(7, output2.Value.PointAtEnd.X);
      Assert.Equal(3, output2.Value.PointAtEnd.Y);
      Assert.Equal(1, output2.Value.PointAtEnd.Z);
      Assert.Equal("STD CH(ft) 1 2 3 4", output3.Value.Profile);
      Assert.Equal(0, output4.Value);
      Assert.Equal("Generic 1D", output5.Value);
      Assert.Equal("Beam", output6.Value);
      Assert.Equal(0, output7.Value.X1.Value);
      Assert.Equal(0, output7.Value.X2.Value);
      Assert.Equal(0, output7.Value.Y.Value);
      Assert.Equal(0, output7.Value.Z.Value);
      Assert.False(output8.Value.X);
      Assert.False(output8.Value.Y);
      Assert.False(output8.Value.Z);
      Assert.False(output8.Value.XX);
      Assert.False(output8.Value.YY);
      Assert.False(output8.Value.ZZ);
      Assert.False(output9.Value.X);
      Assert.False(output9.Value.Y);
      Assert.False(output9.Value.Z);
      Assert.False(output9.Value.XX);
      Assert.False(output9.Value.YY);
      Assert.False(output9.Value.ZZ);
      Assert.Equal(0, output10.Value);
      Assert.Null(output11.Value);
      Assert.Equal(0.5, output12.Value);
      Assert.True(output13.Value);
      Assert.Null(output14.Value.MomentAmplificationFactorStrongAxis);
      Assert.Null(output14.Value.MomentAmplificationFactorWeakAxis);
      Assert.Null(output14.Value.LateralTorsionalBucklingFactor);
      Assert.True(output14.Value.LengthIsSet);
      Assert.Equal("", output15.Value);
      Assert.Equal(0, output16.Value.R);
      Assert.Equal(0, output16.Value.G);
      Assert.Equal(0, output16.Value.B);
      Assert.False(output17.Value);
      Assert.Equal("", output18.Value);
    }

    [Fact]
    public void CreateComponentTest2()
    {
      // Arrange & Act
      var comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, 1, 1);
      ComponentTestHelper.SetInput(comp, new LineCurve(new Point3d(0, 0, 0), new Point3d(1, 2, 3)), 2);
      ComponentTestHelper.SetInput(comp, "STD CH 10 20 30 40", 3);
      ComponentTestHelper.SetInput(comp, 7, 4);
      ComponentTestHelper.SetInput(comp, "Cantilever", 5);
      ComponentTestHelper.SetInput(comp, "Damper", 6);
      ComponentTestHelper.SetInput(comp, new GsaOffsetGoo(new GsaOffset(1, 2, 3, 4)), 7);
      ComponentTestHelper.SetInput(comp, new GsaBool6Goo(new GsaBool6(true, true, true, true, true, true)), 8);
      ComponentTestHelper.SetInput(comp, new GsaBool6Goo(new GsaBool6(true, true, true, true, true, true)), 9);
      ComponentTestHelper.SetInput(comp, Math.PI, 10);
      ComponentTestHelper.SetInput(comp, new GsaNodeGoo(new GsaNode(new Point3d(1, 2, 3), 99)), 11);
      ComponentTestHelper.SetInput(comp, 0.7, 12);
      ComponentTestHelper.SetInput(comp, false, 13);
      ComponentTestHelper.SetInput(comp, new GsaBucklingLengthFactorsGoo(new GsaBucklingLengthFactors(1, 2, 3)), 14);
      ComponentTestHelper.SetInput(comp, "name", 15);
      ComponentTestHelper.SetInput(comp, new GH_Colour(Color.White), 16);
      ComponentTestHelper.SetInput(comp, true, 17);

      // Assert
      GsaMember1dGoo output0 = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp, 0);
      GH_Integer output1 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      GH_Curve output2 = (GH_Curve)ComponentTestHelper.GetOutput(comp, 2);
      GsaSectionGoo output3 = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 3);
      GH_Integer output4 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 4);
      GH_String output5 = (GH_String)ComponentTestHelper.GetOutput(comp, 5);
      GH_String output6 = (GH_String)ComponentTestHelper.GetOutput(comp, 6);
      GsaOffsetGoo output7 = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, 7);
      GsaBool6Goo output8 = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 8);
      GsaBool6Goo output9 = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 9);
      GH_Number output10 = (GH_Number)ComponentTestHelper.GetOutput(comp, 10);
      GsaNodeGoo output11 = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp, 11);
      GH_Number output12 = (GH_Number)ComponentTestHelper.GetOutput(comp, 12);
      GH_Boolean output13 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 13);
      GsaBucklingLengthFactorsGoo output14 = (GsaBucklingLengthFactorsGoo)ComponentTestHelper.GetOutput(comp, 14);
      GH_String output15 = (GH_String)ComponentTestHelper.GetOutput(comp, 15);
      GH_Colour output16 = (GH_Colour)ComponentTestHelper.GetOutput(comp, 16);
      GH_Boolean output17 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 17);
      GH_String output18 = (GH_String)ComponentTestHelper.GetOutput(comp, 18);

      Assert.Equal(0, output0.Value.PolyCurve.PointAtStart.X);
      Assert.Equal(0, output0.Value.PolyCurve.PointAtStart.Y);
      Assert.Equal(0, output0.Value.PolyCurve.PointAtStart.Z);
      Assert.Equal(1, output0.Value.PolyCurve.PointAtEnd.X);
      Assert.Equal(2, output0.Value.PolyCurve.PointAtEnd.Y);
      Assert.Equal(3, output0.Value.PolyCurve.PointAtEnd.Z);
      Assert.Equal("STD CH 10 20 30 40", output0.Value.Section.Profile);
      Assert.Equal(1, output1.Value);
      Assert.Equal(0, output2.Value.PointAtStart.X);
      Assert.Equal(0, output2.Value.PointAtStart.Y);
      Assert.Equal(0, output2.Value.PointAtStart.Z);
      Assert.Equal(1, output2.Value.PointAtEnd.X);
      Assert.Equal(2, output2.Value.PointAtEnd.Y);
      Assert.Equal(3, output2.Value.PointAtEnd.Z);
      Assert.Equal("STD CH 10 20 30 40", output3.Value.Profile);
      Assert.Equal(7, output4.Value);
      Assert.Equal("Cantilever", output5.Value);
      Assert.Equal("Damper", output6.Value);
      Assert.Equal(1, output7.Value.X1.Value);
      Assert.Equal(2, output7.Value.X2.Value);
      Assert.Equal(3, output7.Value.Y.Value);
      Assert.Equal(4, output7.Value.Z.Value);
      Assert.True(output8.Value.X);
      Assert.True(output8.Value.Y);
      Assert.True(output8.Value.Z);
      Assert.True(output8.Value.XX);
      Assert.True(output8.Value.YY);
      Assert.True(output8.Value.ZZ);
      Assert.True(output9.Value.X);
      Assert.True(output9.Value.Y);
      Assert.True(output9.Value.Z);
      Assert.True(output9.Value.XX);
      Assert.True(output9.Value.YY);
      Assert.True(output9.Value.ZZ);
      Assert.Equal(Math.PI, output10.Value);
      Assert.Equal(1, output11.Value.Point.X);
      Assert.Equal(2, output11.Value.Point.Y);
      Assert.Equal(3, output11.Value.Point.Z);
      Assert.Equal(99, output11.Value.Id);
      Assert.Equal(0.7, output12.Value);
      Assert.False(output13.Value);
      Assert.Equal(1, output14.Value.MomentAmplificationFactorStrongAxis);
      Assert.Equal(2, output14.Value.MomentAmplificationFactorWeakAxis);
      Assert.Equal(3, output14.Value.LateralTorsionalBucklingFactor);
      Assert.True(output14.Value.LengthIsSet);
      Assert.Equal("name", output15.Value);
      Assert.Equal(255, output16.Value.R);
      Assert.Equal(255, output16.Value.G);
      Assert.Equal(255, output16.Value.B);
      Assert.True(output17.Value);
      Assert.Equal("", output18.Value);
    }
  }
}
