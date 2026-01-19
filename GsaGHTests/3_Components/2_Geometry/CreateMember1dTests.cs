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
    private readonly GsaMember1dGoo _gsaMember1dGoo;
    private GsaMember1dGoo _gsaMember1dGooFromSection3d;

    public CreateMember1dTests() {
      GH_OasysComponent comp = ComponentMother();
      _gsaMember1dGoo = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp);
    }

    public static GH_OasysComponent ComponentMother() {
      var comp = new Create1dMember();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(
        comp, new LineCurve(new Point3d(0, -1, 0), new Point3d(7, 3, 1)), 0);
      ComponentTestHelper.SetInput(comp, "STD CH(ft) 1 2 3 4", 1);
      ComponentTestHelper.SetInput(comp, 0.5, 2);

      return comp;
    }

    private void SetMember1dFromSection3d() {
      if (_gsaMember1dGooFromSection3d != null) {
        return;
      }

      var section3dPreview = (Section3dPreviewDropDownComponent)ComponentMother();
      section3dPreview.Preview3dSection = true;
      section3dPreview.ExpireSolution(true);
      _gsaMember1dGooFromSection3d = (GsaMember1dGoo)ComponentTestHelper.GetOutput(section3dPreview);
    }

    [Fact]
    public void ComponentShouldReturn0AsPointAtStartXValue() {
      Assert.Equal(0, _gsaMember1dGoo.Value.PolyCurve.PointAtStart.X);
    }

    [Fact]
    public void ComponentShouldReturnMinus1AsPointAtStartYValue() {
      Assert.Equal(-1, _gsaMember1dGoo.Value.PolyCurve.PointAtStart.Y);
    }

    [Fact]
    public void ComponentShouldReturn0AsPointAtStartZValue() {
      Assert.Equal(0, _gsaMember1dGoo.Value.PolyCurve.PointAtStart.Z);
    }

    [Fact]
    public void ComponentShouldReturn7AsPointAtEndXValue() {
      Assert.Equal(7, _gsaMember1dGoo.Value.PolyCurve.PointAtEnd.X);
    }

    [Fact]
    public void ComponentShouldReturn3AsPointAtEndYValue() {
      Assert.Equal(3, _gsaMember1dGoo.Value.PolyCurve.PointAtEnd.Y);
    }

    [Fact]
    public void ComponentShouldReturn1AsPointAtEndZValue() {
      Assert.Equal(1, _gsaMember1dGoo.Value.PolyCurve.PointAtEnd.Z);
    }

    [Fact]
    public void ComponentShouldReturn05AsMeshSize() {
      Assert.Equal(0.5, _gsaMember1dGoo.Value.ApiMember.MeshSize);
    }

    [Fact]
    public void ComponentShouldReturnCorrectProfile() {
      Assert.Equal("STD CH(ft) 1 2 3 4", _gsaMember1dGoo.Value.Section.ApiSection.Profile);
    }

    [Fact]
    public void ComponentShouldReturnDefaultGroupValue() {
      Assert.Equal(1, _gsaMember1dGoo.Value.ApiMember.Group);
    }

    [Fact]
    public void CreateComponentWithSection3dPreviewShouldReturn16Vertices() {
      SetMember1dFromSection3d();
      Assert.Equal(16, _gsaMember1dGooFromSection3d.Value.Section3dPreview.Mesh.Vertices.Count);
    }

    [Fact]
    public void CreateComponentWithSection3dPreviewShouldReturn24Outlines() {
      SetMember1dFromSection3d();
      Assert.Equal(24, _gsaMember1dGooFromSection3d.Value.Section3dPreview.Outlines.Count());
    }

    [Fact]
    public void CreateComponentWithSection3dPreviewShouldCastToMesh() {
      SetMember1dFromSection3d();
      var mesh = new GH_Mesh();
      Assert.True(_gsaMember1dGooFromSection3d.CastTo(ref mesh));
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
