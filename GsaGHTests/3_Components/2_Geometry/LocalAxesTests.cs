using System.Collections.Generic;

using Grasshopper.Kernel.Types;

using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Rhino.Geometry;

using Xunit;

using LocalAxes = GsaGH.Components.LocalAxes;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class LocalAxesTests {
    [Fact]
    public void MemberLocalAxesTest() {
      var comp = new LocalAxes();
      comp.CreateAttributes();
      var member = new GsaMember1d(new LineCurve(new Point3d(0, 0, 0), new Point3d(0, 0, 10)));
      var goo = new GsaMember1dGoo(member);
      ComponentTestHelper.SetInput(comp, goo);

      var vectorX = (GH_Vector)ComponentTestHelper.GetOutput(comp, 0);
      var vectorY = (GH_Vector)ComponentTestHelper.GetOutput(comp, 1);
      var vectorZ = (GH_Vector)ComponentTestHelper.GetOutput(comp, 2);

      Assert.Equal(0, vectorX.Value.X);
      Assert.Equal(0, vectorX.Value.Y);
      Assert.Equal(1, vectorX.Value.Z);

      Assert.Equal(0, vectorY.Value.X);
      Assert.Equal(1, vectorY.Value.Y);
      Assert.Equal(0, vectorY.Value.Z);

      Assert.Equal(-1, vectorZ.Value.X);
      Assert.Equal(0, vectorZ.Value.Y);
      Assert.Equal(0, vectorZ.Value.Z);

      ComponentTestHelper.DrawViewportMeshesAndWiresTest(comp);
    }

    [Fact]
    public void ElementLocalAxesTest() {
      var comp = new LocalAxes();
      comp.CreateAttributes();
      var element = new GsaElement1d(new LineCurve(new Point3d(0, 0, 0), new Point3d(0, 0, 10)));
      var goo = new GsaElement1dGoo(element);
      ComponentTestHelper.SetInput(comp, goo);

      var vectorX = (GH_Vector)ComponentTestHelper.GetOutput(comp, 0);
      var vectorY = (GH_Vector)ComponentTestHelper.GetOutput(comp, 1);
      var vectorZ = (GH_Vector)ComponentTestHelper.GetOutput(comp, 2);

      Assert.Equal(0, vectorX.Value.X);
      Assert.Equal(0, vectorX.Value.Y);
      Assert.Equal(1, vectorX.Value.Z);

      Assert.Equal(0, vectorY.Value.X);
      Assert.Equal(1, vectorY.Value.Y);
      Assert.Equal(0, vectorY.Value.Z);

      Assert.Equal(-1, vectorZ.Value.X);
      Assert.Equal(0, vectorZ.Value.Y);
      Assert.Equal(0, vectorZ.Value.Z);

      ComponentTestHelper.DrawViewportMeshesAndWiresTest(comp);
    }

    [Fact]
    public void MemberLocalAxesTestWithMultipleInputs() {
      var comp = new LocalAxes();
      comp.CreateAttributes();

      // Create two members with different lines
      var member1 = new GsaMember1d(new LineCurve(new Point3d(0, 0, 0), new Point3d(0, 0, 10)));
      var member2 = new GsaMember1d(new LineCurve(new Point3d(0, 0, 0), new Point3d(10, 0, 0)));

      var goo1 = new GsaMember1dGoo(member1);
      var goo2 = new GsaMember1dGoo(member2);

      // Test first input
      ComponentTestHelper.SetInput(comp, goo1);
      ComponentTestHelper.SetInput(comp, goo2);
      List<IGH_Goo> vectorsX1 = ComponentTestHelper.GetResultOutputAllData(comp, 0);
      List<IGH_Goo> vectorsY1 = ComponentTestHelper.GetResultOutputAllData(comp, 1);
      List<IGH_Goo> vectorsZ1 = ComponentTestHelper.GetResultOutputAllData(comp, 2);

      Assert.NotNull(vectorsX1);
      Assert.NotNull(vectorsY1);
      Assert.NotNull(vectorsZ1);

      Assert.Equal(2, vectorsX1.Count);
      Assert.Equal(2, vectorsY1.Count);
      Assert.Equal(2, vectorsZ1.Count);

    }
  }
}
