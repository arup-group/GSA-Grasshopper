using System.Drawing;

using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Components.Properties;
using GsaGHTests.Helpers;

using OasysGH.Components;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class EditNodeTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new EditNode();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp,
        ComponentTestHelper.GetOutput(CreateSupportTests.ComponentMother()), 0);

      return comp;
    }

    [Fact]
    public void GetValuesTest() {
      GH_OasysComponent comp = ComponentMother();

      var output0 = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp, 0);
      var output1 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      var output2 = (GH_Point)ComponentTestHelper.GetOutput(comp, 2);
      var output3 = (GH_Plane)ComponentTestHelper.GetOutput(comp, 3);
      var output4 = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 4);
      var output5 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 5);
      var output6 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 6);
      var output7 = (GsaSpringPropertyGoo)ComponentTestHelper.GetOutput(comp, 7);
      var output8 = (GH_String)ComponentTestHelper.GetOutput(comp, 8);
      var output9 = (GH_Colour)ComponentTestHelper.GetOutput(comp, 9);

      Assert.Equal(0, output0.Value.ApiNode.Position.X);
      Assert.Equal(-1, output0.Value.ApiNode.Position.Y);
      Assert.Equal(0, output0.Value.ApiNode.Position.Z);
      Assert.Equal(0, output0.Value.Point.X);
      Assert.Equal(-1, output0.Value.Point.Y);
      Assert.Equal(0, output0.Value.Point.Z);
      Assert.Equal(0, output2.Value.X);
      Assert.Equal(-1, output2.Value.Y);
      Assert.Equal(0, output2.Value.Z);

      Assert.False(output0.Value.IsSupport);
      Duplicates.AreEqual(new GsaBool6(), output0.Value.Restraint);
      Duplicates.AreEqual(new GsaBool6(), output4.Value);

      Assert.Equal(0, output0.Value.Id);
      Assert.Equal(0, output1.Value);
      Duplicates.AreEqual(Plane.WorldXY, output0.Value.LocalAxis);
      Duplicates.AreEqual(Plane.WorldXY, output3.Value);

      Assert.Equal(0, output0.Value.ApiNode.DamperProperty);
      Assert.Equal(0, output5.Value);
      Assert.Equal(0, output0.Value.ApiNode.MassProperty);
      Assert.Equal(0, output6.Value);
      Assert.Equal(0, output0.Value.ApiNode.SpringProperty);
      Assert.Null(output7.Value);

      Assert.Equal("", output8.Value);
      Assert.Equal(0, output9.Value.R);
      Assert.Equal(0, output9.Value.G);
      Assert.Equal(0, output9.Value.B);
    }

    [Fact]
    public void SetValuesTest() {
      GH_OasysComponent comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, 42, 1);
      ComponentTestHelper.SetInput(comp, new Point3d(1, 2, 3), 2);
      ComponentTestHelper.SetInput(comp, Plane.WorldYZ, 3);
      ComponentTestHelper.SetInput(comp,
        ComponentTestHelper.GetOutput(CreateBool6Tests.ComponentMother()), 4);
      ComponentTestHelper.SetInput(comp, 3, 5);
      ComponentTestHelper.SetInput(comp, 6, 6);
      ComponentTestHelper.SetInput(comp, 9, 7);
      ComponentTestHelper.SetInput(comp, "name", 8);
      ComponentTestHelper.SetInput(comp, new GH_Colour(Color.White), 9);

      var output0 = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp, 0);
      var output1 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      var output2 = (GH_Point)ComponentTestHelper.GetOutput(comp, 2);
      var output3 = (GH_Plane)ComponentTestHelper.GetOutput(comp, 3);
      var output4 = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 4);
      var output5 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 5);
      var output6 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 6);
      var output7 = (GsaSpringPropertyGoo)ComponentTestHelper.GetOutput(comp, 7);
      var output8 = (GH_String)ComponentTestHelper.GetOutput(comp, 8);
      var output9 = (GH_Colour)ComponentTestHelper.GetOutput(comp, 9);

      Assert.Equal(1, output0.Value.ApiNode.Position.X);
      Assert.Equal(2, output0.Value.ApiNode.Position.Y);
      Assert.Equal(3, output0.Value.ApiNode.Position.Z);
      Assert.Equal(1, output0.Value.Point.X);
      Assert.Equal(2, output0.Value.Point.Y);
      Assert.Equal(3, output0.Value.Point.Z);
      Assert.Equal(1, output2.Value.X);
      Assert.Equal(2, output2.Value.Y);
      Assert.Equal(3, output2.Value.Z);

      Assert.True(output0.Value.IsSupport);
      Duplicates.AreEqual(new GsaBool6(true, true, true, true, true, true),
        output0.Value.Restraint);
      Duplicates.AreEqual(new GsaBool6(true, true, true, true, true, true), output4.Value);

      Assert.Equal(42, output0.Value.Id);
      Assert.Equal(42, output1.Value);
      Duplicates.AreEqual(Plane.WorldYZ, output0.Value.LocalAxis);
      Duplicates.AreEqual(Plane.WorldYZ, output3.Value);

      Assert.Equal(3, output0.Value.ApiNode.DamperProperty);
      Assert.Equal(3, output5.Value);
      Assert.Equal(6, output0.Value.ApiNode.MassProperty);
      Assert.Equal(6, output6.Value);
      Assert.Equal(9, output7.Value.Id);

      Assert.Equal("name", output8.Value);
      Assert.Equal(255, output9.Value.R);
      Assert.Equal(255, output9.Value.G);
      Assert.Equal(255, output9.Value.B);
    }

    [Fact]
    public void FlipModeTest() {
      var comp = (EditNode)ComponentMother();
      comp.FlipMode(null, null);
      Assert.Equal(12, comp.Params.Output.Count);
    }
  }
}
