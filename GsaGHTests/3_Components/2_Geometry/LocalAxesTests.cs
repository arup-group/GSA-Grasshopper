using Grasshopper.Kernel.Types;

using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Rhino.Geometry;

using Xunit;

using LocalAxes = GsaGH.Components.LocalAxes;
using GsaGH.Helpers;
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

      Assert.Equal(0, vectorX.Value.X, DoubleComparer.Default);
      Assert.Equal(0, vectorX.Value.Y, DoubleComparer.Default);
      Assert.Equal(1, vectorX.Value.Z, DoubleComparer.Default);

      Assert.Equal(0, vectorY.Value.X, DoubleComparer.Default);
      Assert.Equal(1, vectorY.Value.Y, DoubleComparer.Default);
      Assert.Equal(0, vectorY.Value.Z, DoubleComparer.Default);

      Assert.Equal(-1, vectorZ.Value.X, DoubleComparer.Default);
      Assert.Equal(0, vectorZ.Value.Y, DoubleComparer.Default);
      Assert.Equal(0, vectorZ.Value.Z, DoubleComparer.Default);

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

      Assert.Equal(0, vectorX.Value.X, DoubleComparer.Default);
      Assert.Equal(0, vectorX.Value.Y, DoubleComparer.Default);
      Assert.Equal(1, vectorX.Value.Z, DoubleComparer.Default);

      Assert.Equal(0, vectorY.Value.X, DoubleComparer.Default);
      Assert.Equal(1, vectorY.Value.Y, DoubleComparer.Default);
      Assert.Equal(0, vectorY.Value.Z, DoubleComparer.Default);

      Assert.Equal(-1, vectorZ.Value.X, DoubleComparer.Default);
      Assert.Equal(0, vectorZ.Value.Y, DoubleComparer.Default);
      Assert.Equal(0, vectorZ.Value.Z, DoubleComparer.Default);

      ComponentTestHelper.DrawViewportMeshesAndWiresTest(comp);
    }
  }
}
