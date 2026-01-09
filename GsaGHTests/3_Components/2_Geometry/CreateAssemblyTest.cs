using System.Collections.Generic;
using System.Linq;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class CreateAssemblyTests {
    public static GH_OasysDropDownComponent ByExplicitPositionsComponentMother1() {
      var comp = new CreateAssembly();
      comp.CreateAttributes();

      comp.SetSelected(0, 0);
      comp.SetSelected(1, 2); // m
      ComponentTestHelper.SetInput(comp, 1, 2);
      ComponentTestHelper.SetInput(comp, 2, 3);
      ComponentTestHelper.SetInput(comp, 3, 4);
      ComponentTestHelper.SetListInput(comp, new List<object>() { 4, 5 }, 9);

      return comp;
    }

    public static GH_OasysDropDownComponent ByExplicitPositionsComponentMother2() {
      var comp = new CreateAssembly();
      comp.CreateAttributes();

      comp.SetSelected(0, 0);
      comp.SetSelected(1, 2); // m
      ComponentTestHelper.SetInput(comp, "Name", 0);
      var entityList = new GsaList("list", "1", GsaAPI.EntityType.Member);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(entityList), 1);
      ComponentTestHelper.SetInput(comp, 1, 2);
      ComponentTestHelper.SetInput(comp, 2, 3);
      ComponentTestHelper.SetInput(comp, 3, 4);
      ComponentTestHelper.SetInput(comp, 1.0, 5);
      ComponentTestHelper.SetInput(comp, -1.0, 6);
      ComponentTestHelper.SetInput(comp, "4 5 6", 7);
      ComponentTestHelper.SetInput(comp, 1, 8);
      ComponentTestHelper.SetListInput(comp, new List<object>() { 7.7, 8.8 }, 9);

      return comp;
    }

    public static GH_OasysDropDownComponent ByNumberOfPointsComponentMother1() {
      var comp = new CreateAssembly();
      comp.CreateAttributes();

      comp.SetSelected(0, 1);
      comp.SetSelected(1, 2); // m
      ComponentTestHelper.SetInput(comp, 1, 2);
      ComponentTestHelper.SetInput(comp, 2, 3);
      ComponentTestHelper.SetInput(comp, 3, 4);

      return comp;
    }

    public static GH_OasysDropDownComponent ByNumberOfPointsComponentMother2() {
      var comp = new CreateAssembly();
      comp.CreateAttributes();

      comp.SetSelected(0, 1);
      comp.SetSelected(1, 2); // m
      ComponentTestHelper.SetInput(comp, "Name", 0);
      var entityList = new GsaList("list", "1", GsaAPI.EntityType.Member);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(entityList), 1);
      ComponentTestHelper.SetInput(comp, 1, 2);
      ComponentTestHelper.SetInput(comp, 2, 3);
      ComponentTestHelper.SetInput(comp, 3, 4);
      ComponentTestHelper.SetInput(comp, 1.0, 5);
      ComponentTestHelper.SetInput(comp, -1.0, 6);
      ComponentTestHelper.SetInput(comp, "4 5 6", 7);
      ComponentTestHelper.SetInput(comp, 1, 8);
      ComponentTestHelper.SetInput(comp, 7, 9);

      return comp;
    }

    public static GH_OasysDropDownComponent BySpacingOfPointsComponentMother1() {
      var comp = new CreateAssembly();
      comp.CreateAttributes();

      comp.SetSelected(0, 2);
      comp.SetSelected(1, 2); // m
      ComponentTestHelper.SetInput(comp, 1, 2);
      ComponentTestHelper.SetInput(comp, 2, 3);
      ComponentTestHelper.SetInput(comp, 3, 4);

      return comp;
    }

    public static GH_OasysDropDownComponent BySpacingOfPointsComponentMother2() {
      var comp = new CreateAssembly();
      comp.CreateAttributes();

      comp.SetSelected(0, 2);
      comp.SetSelected(1, 2); // m
      ComponentTestHelper.SetInput(comp, "Name", 0);
      var entityList = new GsaList("list", "1", GsaAPI.EntityType.Member);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(entityList), 1);
      ComponentTestHelper.SetInput(comp, 1, 2);
      ComponentTestHelper.SetInput(comp, 2, 3);
      ComponentTestHelper.SetInput(comp, 3, 4);
      ComponentTestHelper.SetInput(comp, 1.0, 5);
      ComponentTestHelper.SetInput(comp, -1.0, 6);
      ComponentTestHelper.SetInput(comp, "4 5 6", 7);
      ComponentTestHelper.SetInput(comp, 1, 8);
      ComponentTestHelper.SetInput(comp, 7.7, 9);

      return comp;
    }

    public static GH_OasysDropDownComponent ByStoreyComponentMother1() {
      var comp = new CreateAssembly();
      comp.CreateAttributes();

      comp.SetSelected(0, 3);
      comp.SetSelected(1, 2); // m
      ComponentTestHelper.SetInput(comp, 1, 2);
      ComponentTestHelper.SetInput(comp, 2, 3);
      ComponentTestHelper.SetInput(comp, 3, 4);
      ComponentTestHelper.SetInput(comp, "7", 7);

      return comp;
    }

    public static GH_OasysDropDownComponent ByStoreyComponentMother2() {
      var comp = new CreateAssembly();
      comp.CreateAttributes();

      comp.SetSelected(0, 3);
      comp.SetSelected(1, 2); // m
      ComponentTestHelper.SetInput(comp, "Name", 0);
      var entityList = new GsaList("list", "1", GsaAPI.EntityType.Member);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(entityList), 1);
      ComponentTestHelper.SetInput(comp, 1, 2);
      ComponentTestHelper.SetInput(comp, 2, 3);
      ComponentTestHelper.SetInput(comp, 3, 4);
      ComponentTestHelper.SetInput(comp, 1.0, 5);
      ComponentTestHelper.SetInput(comp, -1.0, 6);
      ComponentTestHelper.SetInput(comp, "7", 7);

      return comp;
    }

    [Fact]
    public void CreateByExplicitPositionsComponent1() {
      GH_OasysDropDownComponent comp = ByExplicitPositionsComponentMother1();

      var output = (GsaAssemblyGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("", output.Value.ApiAssembly.Name);
      Assert.Equal(GsaAPI.EntityType.Element, output.Value.ApiAssembly.EntityType);
      Assert.Equal("all", output.Value.ApiAssembly.EntityList);
      Assert.Equal(1, output.Value.ApiAssembly.Topology1);
      Assert.Equal(2, output.Value.ApiAssembly.Topology2);
      Assert.Equal(3, output.Value.ApiAssembly.OrientationNode);
      Assert.Equal(0, output.Value.ApiAssembly.ExtentY);
      Assert.Equal(0, output.Value.ApiAssembly.ExtentZ);
      Assert.Empty(((AssemblyByExplicitPositions)output.Value.ApiAssembly).InternalTopology);
      Assert.Equal(CurveFit.LagrangeInterpolation, ((AssemblyByExplicitPositions)output.Value.ApiAssembly).CurveFit);
      Assert.Equal(2, ((AssemblyByExplicitPositions)output.Value.ApiAssembly).Positions.Count);
      Assert.Equal(4, ((AssemblyByExplicitPositions)output.Value.ApiAssembly).Positions.First());
      Assert.Equal(5, ((AssemblyByExplicitPositions)output.Value.ApiAssembly).Positions.Last());
    }

    [Fact]
    public void CreateByExplicitPositionsComponent2() {
      GH_OasysDropDownComponent comp = ByExplicitPositionsComponentMother2();

      var output = (GsaAssemblyGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("Name", output.Value.ApiAssembly.Name);
      Assert.Equal(GsaAPI.EntityType.Member, output.Value.ApiAssembly.EntityType);
      Assert.Equal("1", output.Value.ApiAssembly.EntityList);
      Assert.Equal(1, output.Value.ApiAssembly.Topology1);
      Assert.Equal(2, output.Value.ApiAssembly.Topology2);
      Assert.Equal(3, output.Value.ApiAssembly.OrientationNode);
      Assert.Equal(1.0, output.Value.ApiAssembly.ExtentY);
      Assert.Equal(-1.0, output.Value.ApiAssembly.ExtentZ);
      Assert.Equal(3, ((AssemblyByExplicitPositions)output.Value.ApiAssembly).InternalTopology.Count);
      Assert.Equal(4, ((AssemblyByExplicitPositions)output.Value.ApiAssembly).InternalTopology[0]);
      Assert.Equal(5, ((AssemblyByExplicitPositions)output.Value.ApiAssembly).InternalTopology[1]);
      Assert.Equal(6, ((AssemblyByExplicitPositions)output.Value.ApiAssembly).InternalTopology[2]);
      Assert.Equal(CurveFit.CircularArc, ((AssemblyByExplicitPositions)output.Value.ApiAssembly).CurveFit);
      Assert.Equal(2, ((AssemblyByExplicitPositions)output.Value.ApiAssembly).Positions.Count);
      Assert.Equal(7.7, ((AssemblyByExplicitPositions)output.Value.ApiAssembly).Positions.First());
      Assert.Equal(8.8, ((AssemblyByExplicitPositions)output.Value.ApiAssembly).Positions.Last());
    }

    [Fact]
    public void CreateByNumberOfPointsComponent1() {
      GH_OasysDropDownComponent comp = ByNumberOfPointsComponentMother1();

      var output = (GsaAssemblyGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("", output.Value.ApiAssembly.Name);
      Assert.Equal(GsaAPI.EntityType.Element, output.Value.ApiAssembly.EntityType);
      Assert.Equal("all", output.Value.ApiAssembly.EntityList);
      Assert.Equal(1, output.Value.ApiAssembly.Topology1);
      Assert.Equal(2, output.Value.ApiAssembly.Topology2);
      Assert.Equal(3, output.Value.ApiAssembly.OrientationNode);
      Assert.Equal(0, output.Value.ApiAssembly.ExtentY);
      Assert.Equal(0, output.Value.ApiAssembly.ExtentZ);
      Assert.Empty(((AssemblyByNumberOfPoints)output.Value.ApiAssembly).InternalTopology);
      Assert.Equal(CurveFit.LagrangeInterpolation, ((AssemblyByNumberOfPoints)output.Value.ApiAssembly).CurveFit);
      Assert.Equal(10, ((AssemblyByNumberOfPoints)output.Value.ApiAssembly).NumberOfPoints);
    }

    [Fact]
    public void CreateByNumberOfPointsComponent2() {
      GH_OasysDropDownComponent comp = ByNumberOfPointsComponentMother2();

      var output = (GsaAssemblyGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("Name", output.Value.ApiAssembly.Name);
      Assert.Equal(GsaAPI.EntityType.Member, output.Value.ApiAssembly.EntityType);
      Assert.Equal("1", output.Value.ApiAssembly.EntityList);
      Assert.Equal(1, output.Value.ApiAssembly.Topology1);
      Assert.Equal(2, output.Value.ApiAssembly.Topology2);
      Assert.Equal(3, output.Value.ApiAssembly.OrientationNode);
      Assert.Equal(1.0, output.Value.ApiAssembly.ExtentY);
      Assert.Equal(-1.0, output.Value.ApiAssembly.ExtentZ);
      Assert.Equal(3, ((AssemblyByNumberOfPoints)output.Value.ApiAssembly).InternalTopology.Count);
      Assert.Equal(4, ((AssemblyByNumberOfPoints)output.Value.ApiAssembly).InternalTopology[0]);
      Assert.Equal(5, ((AssemblyByNumberOfPoints)output.Value.ApiAssembly).InternalTopology[1]);
      Assert.Equal(6, ((AssemblyByNumberOfPoints)output.Value.ApiAssembly).InternalTopology[2]);
      Assert.Equal(CurveFit.CircularArc, ((AssemblyByNumberOfPoints)output.Value.ApiAssembly).CurveFit);
      Assert.Equal(7, ((AssemblyByNumberOfPoints)output.Value.ApiAssembly).NumberOfPoints);
    }

    [Fact]
    public void CreateBySpacingOfPointsComponent1() {
      GH_OasysDropDownComponent comp = BySpacingOfPointsComponentMother1();

      var output = (GsaAssemblyGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("", output.Value.ApiAssembly.Name);
      Assert.Equal(GsaAPI.EntityType.Element, output.Value.ApiAssembly.EntityType);
      Assert.Equal("all", output.Value.ApiAssembly.EntityList);
      Assert.Equal(1, output.Value.ApiAssembly.Topology1);
      Assert.Equal(2, output.Value.ApiAssembly.Topology2);
      Assert.Equal(3, output.Value.ApiAssembly.OrientationNode);
      Assert.Equal(0, output.Value.ApiAssembly.ExtentY);
      Assert.Equal(0, output.Value.ApiAssembly.ExtentZ);
      Assert.Empty(((AssemblyBySpacingOfPoints)output.Value.ApiAssembly).InternalTopology);
      Assert.Equal(CurveFit.LagrangeInterpolation, ((AssemblyBySpacingOfPoints)output.Value.ApiAssembly).CurveFit);
      Assert.Equal(1, ((AssemblyBySpacingOfPoints)output.Value.ApiAssembly).Spacing);
    }

    [Fact]
    public void CreateBySpacingOfPointsComponent2() {
      GH_OasysDropDownComponent comp = BySpacingOfPointsComponentMother2();

      var output = (GsaAssemblyGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("Name", output.Value.ApiAssembly.Name);
      Assert.Equal(GsaAPI.EntityType.Member, output.Value.ApiAssembly.EntityType);
      Assert.Equal("1", output.Value.ApiAssembly.EntityList);
      Assert.Equal(1, output.Value.ApiAssembly.Topology1);
      Assert.Equal(2, output.Value.ApiAssembly.Topology2);
      Assert.Equal(3, output.Value.ApiAssembly.OrientationNode);
      Assert.Equal(1.0, output.Value.ApiAssembly.ExtentY);
      Assert.Equal(-1.0, output.Value.ApiAssembly.ExtentZ);
      Assert.Equal(3, ((AssemblyBySpacingOfPoints)output.Value.ApiAssembly).InternalTopology.Count);
      Assert.Equal(4, ((AssemblyBySpacingOfPoints)output.Value.ApiAssembly).InternalTopology[0]);
      Assert.Equal(5, ((AssemblyBySpacingOfPoints)output.Value.ApiAssembly).InternalTopology[1]);
      Assert.Equal(6, ((AssemblyBySpacingOfPoints)output.Value.ApiAssembly).InternalTopology[2]);
      Assert.Equal(CurveFit.CircularArc, ((AssemblyBySpacingOfPoints)output.Value.ApiAssembly).CurveFit);
      Assert.Equal(7.7, ((AssemblyBySpacingOfPoints)output.Value.ApiAssembly).Spacing);
    }

    [Fact]
    public void CreateByStoreyComponent1() {
      GH_OasysDropDownComponent comp = ByStoreyComponentMother1();

      var output = (GsaAssemblyGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("", output.Value.ApiAssembly.Name);
      Assert.Equal(GsaAPI.EntityType.Element, output.Value.ApiAssembly.EntityType);
      Assert.Equal("all", output.Value.ApiAssembly.EntityList);
      Assert.Equal(1, output.Value.ApiAssembly.Topology1);
      Assert.Equal(2, output.Value.ApiAssembly.Topology2);
      Assert.Equal(3, output.Value.ApiAssembly.OrientationNode);
      Assert.Equal(0, output.Value.ApiAssembly.ExtentY);
      Assert.Equal(0, output.Value.ApiAssembly.ExtentZ);
      Assert.Equal("7", ((AssemblyByStorey)output.Value.ApiAssembly).StoreyList);
    }

    [Fact]
    public void CreateByStoreyComponent2() {
      GH_OasysDropDownComponent comp = ByStoreyComponentMother2();

      var output = (GsaAssemblyGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("Name", output.Value.ApiAssembly.Name);
      Assert.Equal(GsaAPI.EntityType.Member, output.Value.ApiAssembly.EntityType);
      Assert.Equal("1", output.Value.ApiAssembly.EntityList);
      Assert.Equal(1, output.Value.ApiAssembly.Topology1);
      Assert.Equal(2, output.Value.ApiAssembly.Topology2);
      Assert.Equal(3, output.Value.ApiAssembly.OrientationNode);
      Assert.Equal(1.0, output.Value.ApiAssembly.ExtentY);
      Assert.Equal(-1.0, output.Value.ApiAssembly.ExtentZ);
      Assert.Equal("7", ((AssemblyByStorey)output.Value.ApiAssembly).StoreyList);
    }

  }
}
