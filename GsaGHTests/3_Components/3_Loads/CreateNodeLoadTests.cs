using GsaGH.Components;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;

using GsaGHTests.Helpers;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Components.Loads {
  [Collection("GrasshopperFixture collection")]
  public class CreateNodeLoadTests {
    [Fact]
    public void CreateNodeForceTest() {
      var comp = new CreateNodeLoad();
      ComponentTestHelper.SetInput(comp, 7, 0);
      ComponentTestHelper.SetInput(comp, new Point3d(10, 5, -1), 1);
      ComponentTestHelper.SetInput(comp, "myNodeLoad", 2);
      ComponentTestHelper.SetInput(comp, -5, 4);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaNodeLoad)output.Value;
      Assert.Equal(7, load.LoadCase.Id);
      Assert.Equal(10, load._refPoint.X);
      Assert.Equal(5, load._refPoint.Y);
      Assert.Equal(-1, load._refPoint.Z);
      Assert.Equal("myNodeLoad", load.ApiLoad.Name);
      Assert.Equal(-5000, load.ApiLoad.Value);
      Assert.Equal(NodeLoadType.NodeLoad, load.Type);
      Assert.Equal(ReferenceType.None, load.ReferenceType);
    }

    [Fact]
    public void CreateNodeMomentTest() {
      var comp = new CreateNodeLoad();
      comp.CreateAttributes();
      comp.SetSelected(0, 1); // NodeMoment
      ComponentTestHelper.SetInput(comp, 7, 0);
      ComponentTestHelper.SetInput(comp, new Point3d(10, 5, -1), 1);
      ComponentTestHelper.SetInput(comp, "myNodeLoad", 2);
      ComponentTestHelper.SetInput(comp, -5, 4);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaNodeLoad)output.Value;
      Assert.Equal(7, load.LoadCase.Id);
      Assert.Equal(10, load._refPoint.X);
      Assert.Equal(5, load._refPoint.Y);
      Assert.Equal(-1, load._refPoint.Z);
      Assert.Equal("myNodeLoad", load.ApiLoad.Name);
      Assert.Equal(-5000, load.ApiLoad.Value);
      Assert.Equal(NodeLoadType.NodeLoad, load.Type);
      Assert.Equal(ReferenceType.None, load.ReferenceType);
    }

    [Fact]
    public void CreateAppliedDisplTest() {
      var comp = new CreateNodeLoad();
      comp.CreateAttributes();
      comp.SetSelected(0, 2); // AppliedDispl
      ComponentTestHelper.SetInput(comp, 7, 0);
      ComponentTestHelper.SetInput(comp, new Point3d(10, 5, -1), 1);
      ComponentTestHelper.SetInput(comp, "myNodeLoad", 2);
      ComponentTestHelper.SetInput(comp, -5, 4);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaNodeLoad)output.Value;
      Assert.Equal(7, load.LoadCase.Id);
      Assert.Equal(10, load._refPoint.X);
      Assert.Equal(5, load._refPoint.Y);
      Assert.Equal(-1, load._refPoint.Z);
      Assert.Equal("myNodeLoad", load.ApiLoad.Name);
      Assert.Equal(-0.005, load.ApiLoad.Value);
      Assert.Equal(NodeLoadType.AppliedDisp, load.Type);
      Assert.Equal(ReferenceType.None, load.ReferenceType);
    }

    [Fact]
    public void CreateSettlementTest() {
      var comp = new CreateNodeLoad();
      comp.CreateAttributes();
      comp.SetSelected(0, 3); // Settlement
      ComponentTestHelper.SetInput(comp, 7, 0);
      ComponentTestHelper.SetInput(comp, new Point3d(10, 5, -1), 1);
      ComponentTestHelper.SetInput(comp, "myNodeLoad", 2);
      ComponentTestHelper.SetInput(comp, -5, 4);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaNodeLoad)output.Value;
      Assert.Equal(7, load.LoadCase.Id);
      Assert.Equal(10, load._refPoint.X);
      Assert.Equal(5, load._refPoint.Y);
      Assert.Equal(-1, load._refPoint.Z);
      Assert.Equal("myNodeLoad", load.ApiLoad.Name);
      Assert.Equal(-0.005, load.ApiLoad.Value);
      Assert.Equal(NodeLoadType.Settlement, load.Type);
      Assert.Equal(ReferenceType.None, load.ReferenceType);
    }

    [Theory]
    [InlineData(0, "X")]
    [InlineData(0, "Y")]
    [InlineData(0, "Z")]
    [InlineData(0, "XX")]
    [InlineData(0, "YY")]
    [InlineData(0, "ZZ")]
    [InlineData(1, "X")]
    [InlineData(1, "Y")]
    [InlineData(1, "Z")]
    [InlineData(1, "XX")]
    [InlineData(1, "YY")]
    [InlineData(1, "ZZ")]
    [InlineData(2, "X")]
    [InlineData(2, "Y")]
    [InlineData(2, "Z")]
    [InlineData(2, "XX")]
    [InlineData(2, "YY")]
    [InlineData(2, "ZZ")]
    [InlineData(3, "X")]
    [InlineData(3, "Y")]
    [InlineData(3, "Z")]
    [InlineData(3, "XX")]
    [InlineData(3, "YY")]
    [InlineData(3, "ZZ")]
    public void DirectionTest(int j, string direction) {
      var comp = new CreateNodeLoad();
      comp.CreateAttributes();
      comp.SetSelected(0, j);
      ComponentTestHelper.SetInput(comp, new Point3d(10, 5, -1), 1);
      ComponentTestHelper.SetInput(comp, direction, 3);
      ComponentTestHelper.SetInput(comp, -5, 4);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaNodeLoad)output.Value;
      Assert.Equal(direction, load.ApiLoad.Direction.ToString());
    }

    [Fact]
    public void ParsePointWarningTest() {
      var comp = new CreateNodeLoad();
      var list = new GsaList {
        EntityType = EntityType.Member
      };
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      ComponentTestHelper.SetInput(comp, -5, 4);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
    }

    [Theory]
    [InlineData(1, "X")]
    [InlineData(1, "Y")]
    [InlineData(1, "Z")]
    [InlineData(0, "XX")]
    [InlineData(0, "YY")]
    [InlineData(0, "ZZ")]
    public void ParseDirectionWarningTest(int j, string direction) {
      var comp = new CreateNodeLoad();
      comp.CreateAttributes();
      comp.SetSelected(0, j);
      ComponentTestHelper.SetInput(comp, new Point3d(10, 5, -1), 1);
      ComponentTestHelper.SetInput(comp, direction, 3);
      ComponentTestHelper.SetInput(comp, -5, 4);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
    }
  }
}
