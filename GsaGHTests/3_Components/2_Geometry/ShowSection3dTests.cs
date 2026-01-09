using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Components;

using GsaGHTests.Helpers;
using GsaGHTests.Model;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class ShowSection3dTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new Preview3dSections();
      comp.CreateAttributes();
      comp.Params.Input[0].DataMapping = GH_DataMapping.Flatten;
      ComponentTestHelper.SetInput(comp,
        ComponentTestHelper.GetOutput(CreateElement1dTests.ComponentMother()), 0);
      ComponentTestHelper.SetInput(comp,
        ComponentTestHelper.GetOutput(CreateElement2dTests.ComponentMother()), 0);
      ComponentTestHelper.SetInput(comp,
        ComponentTestHelper.GetOutput(CreateMember1dTests.ComponentMother()), 0);
      ComponentTestHelper.SetInput(comp,
        ComponentTestHelper.GetOutput(CreateMember2dTests.ComponentMother()), 0);

      return comp;
    }

    [Fact]
    public void CreateComponentTest() {
      GH_OasysComponent comp = ComponentMother();
      var analysisMesh = (GH_Mesh)ComponentTestHelper.GetOutput(comp, 0);
      var analysisOutlines = (List<GH_Line>)comp.Params.Output[1].VolatileData.get_Branch(0);
      var designMesh = (GH_Mesh)ComponentTestHelper.GetOutput(comp, 2);
      var designOutlines = (List<GH_Line>)comp.Params.Output[3].VolatileData.get_Branch(0);

      Assert.NotEmpty(analysisMesh.Value.Vertices);
      Assert.NotEmpty(analysisOutlines);
      Assert.NotEmpty(designMesh.Value.Vertices);
      Assert.NotEmpty(designOutlines);

      //Assert.Equal(26, analysisMesh.Value.Vertices.Count);
      //Assert.Equal(37, analysisOutlines.Count);
      //Assert.Equal(26, designMesh.Value.Vertices.Count);
      //Assert.Equal(41, designOutlines.Count);
    }

    [Fact]
    public void CreateComponentFromModelTest() {
      var comp = new Preview3dSections();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, ModelTests.GsaModelGooMother, 0);
      var analysisMesh = (GH_Mesh)ComponentTestHelper.GetOutput(comp, 0);
      var analysisOutlines = (List<GH_Line>)comp.Params.Output[1].VolatileData.get_Branch(0);
      var designMesh = (GH_Mesh)ComponentTestHelper.GetOutput(comp, 2);
      var designOutlines = (List<GH_Line>)comp.Params.Output[3].VolatileData.get_Branch(0);
      Assert.Equal(78, analysisMesh.Value.Vertices.Count);
      Assert.Equal(86, analysisOutlines.Count);
      Assert.Equal(78, designMesh.Value.Vertices.Count);
      Assert.Equal(86, designOutlines.Count);
    }
  }
}
