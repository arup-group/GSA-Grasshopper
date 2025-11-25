using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;
using GsaGHTests.Model;

using OasysGH.Components;

using Rhino;
using Rhino.Geometry;

using Xunit;

using LengthUnit = OasysUnits.Units.LengthUnit;
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

    [Fact]
    public void CheckUnitIsConsistent() {
      var componenet = new Preview3dSections();
      componenet.CreateAttributes();
      var element1d = new GsaElement1d {
        LengthUnit = LengthUnit.Meter
      };

      var element2d = new GsaElement2d {
        LengthUnit = LengthUnit.Centimeter
      };

      ComponentTestHelper.SetInput(componenet, new GsaElement1dGoo(element1d));
      ComponentTestHelper.SetInput(componenet, new GsaElement2dGoo(element2d));
      ComponentTestHelper.GetOutput(componenet);
      IList<string> message = componenet.RuntimeMessages(GH_RuntimeMessageLevel.Error);
      Assert.Single(message);
      Assert.Contains("Multiple length units have been detected", message[0]);
    }

    [Fact]
    public void CheckZoomIscalled() {
      var line = new Line(new Point3d(0, 0, 0), new Point3d(0, 0, 10));
      var element = new GsaElement1d(new LineCurve(line)) {
        Section = new GsaSection("STD R 200 100")
      };
      var rhinoDoc = RhinoDoc.CreateHeadless(null);
      rhinoDoc.Views.DefaultViewLayout();
      RhinoDoc.ActiveDoc = rhinoDoc;
      var preview = new Section3dPreview(element);
      preview.Scale(LengthUnit.Meter);
      Assert.True(true);
      rhinoDoc.Dispose();
    }
  }
}
