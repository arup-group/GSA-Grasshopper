using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class CreateMember1dTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new Create1dMember();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(
        comp, new LineCurve(new Point3d(0, -1, 0), new Point3d(7, 3, 1)), 0);
      ComponentTestHelper.SetInput(comp, "STD CH(ft) 1 2 3 4", 1);
      ComponentTestHelper.SetInput(comp, 0.5, 2);

      return comp;
    }

    [Fact]
    public void CreateComponentTest() {
      GH_OasysComponent comp = ComponentMother();

      var output = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(0, output.Value.PolyCurve.PointAtStart.X);
      Assert.Equal(-1, output.Value.PolyCurve.PointAtStart.Y);
      Assert.Equal(0, output.Value.PolyCurve.PointAtStart.Z);
      Assert.Equal(7, output.Value.PolyCurve.PointAtEnd.X);
      Assert.Equal(3, output.Value.PolyCurve.PointAtEnd.Y);
      Assert.Equal(1, output.Value.PolyCurve.PointAtEnd.Z);
      Assert.Equal("STD CH(ft) 1 2 3 4", output.Value.Section.ApiSection.Profile);
      Assert.Equal(0.5, output.Value.ApiMember.MeshSize);
      Assert.Equal(1, output.Value.ApiMember.Group);
    }

    [Fact]
    public void CreateComponentWithSection3dPreviewTest() {
      var comp = (Section3dPreviewDropDownComponent)ComponentMother();
      comp.Preview3dSection = true;
      comp.ExpireSolution(true);

      var output = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(16, output.Value.Section3dPreview.Mesh.Vertices.Count);
      Assert.Equal(24, output.Value.Section3dPreview.Outlines.Count());

      var mesh = new GH_Mesh();
      Assert.True(output.CastTo(ref mesh));
    }

    [Theory]
    [InlineData(new bool[] {
      true, true, true, false, false, false, true, true, true, false, false, false})]
    [InlineData(new bool[] {
      false, false, false, true, true, true, false, false, false, true, true, true})]
    [InlineData(new bool[] {
      false, false, false, false, false, false, false, false, false, true, true, true})]
    [InlineData(new bool[] {
      true, true, true, false, false, false, false, false, false, false, false, false})]
    [InlineData(new bool[] {
      true, false, true, false, true, false, true, false, true, false, true, false})]
    [InlineData(new bool[] {
      false, true, false, true, false, true, false, true, false, true, false, true})]
    public void CanToggleReleases(bool[] releases) {
      var comp = (Create1dMember)ComponentMother();
      int i = 0;
      var restraints = new List<List<bool>> {
        new List<bool> {
          releases[i++],
          releases[i++],
          releases[i++],
          releases[i++],
          releases[i++],
          releases[i++],
        },
        new List<bool> {
          releases[i++],
          releases[i++],
          releases[i++],
          releases[i++],
          releases[i++],
          releases[i++],
        }
      };
      comp.SetReleases(restraints);

      var output = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp);
      i = 0;
      Assert.Equal(releases[i++], output.Value.ReleaseStart.X);
      Assert.Equal(releases[i++], output.Value.ReleaseStart.Y);
      Assert.Equal(releases[i++], output.Value.ReleaseStart.Z);
      Assert.Equal(releases[i++], output.Value.ReleaseStart.Xx);
      Assert.Equal(releases[i++], output.Value.ReleaseStart.Yy);
      Assert.Equal(releases[i++], output.Value.ReleaseStart.Zz);
      Assert.Equal(releases[i++], output.Value.ReleaseEnd.X);
      Assert.Equal(releases[i++], output.Value.ReleaseEnd.Y);
      Assert.Equal(releases[i++], output.Value.ReleaseEnd.Z);
      Assert.Equal(releases[i++], output.Value.ReleaseEnd.Xx);
      Assert.Equal(releases[i++], output.Value.ReleaseEnd.Yy);
      Assert.Equal(releases[i++], output.Value.ReleaseEnd.Zz);
    }

    [Fact]
    public void TestShortLineWarning() {
      var comp = new Create1dMember();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(
        comp, new LineCurve(new Point3d(0, 0, 0), new Point3d(0, 0, 0.001)), 0);

      var output = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Single(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
    }
  }
}
