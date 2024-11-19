using Grasshopper.Kernel;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Rhino.Collections;
using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Model {
  [Collection("GrasshopperFixture collection")]
  public class CreateGridLineTest {
    private static CreateGridLineTestHelper _helper;

    public CreateGridLineTest() {
      _helper = new CreateGridLineTestHelper();
    }

    [Fact]
    public void LineInputReturnNotNullGridLine() {
      _helper.CreateComponentWithLineInput();
      GsaGridLineGoo output = _helper.GetGridLineOutput();
      Assert.NotNull(output.Value.GridLine);
    }

    [Fact]
    public void ArcInputReturnNotNullGridLine() {
      _helper.CreateComponentWithArcInput();
      GsaGridLineGoo output = _helper.GetGridLineOutput();
      Assert.NotNull(output.Value.GridLine);
    }

    [Fact]
    public void ForInvalidArcInputReturnWarning() {
      var arc = new Arc(new Point3d(0, 0, 0), new Point3d(0, 0, 0), new Point3d(0, 0, 0));
      _helper.CreateComponentWithArcInput(arc);
      _helper.GetGridLineOutput();
      Assert.NotEmpty(_helper.GetComponent().RuntimeMessages(GH_RuntimeMessageLevel.Warning));
    }

    [Fact]
    public void ForInvalidLineInputReturnWarning() {
      var line = new Line(new Point3d(0, 0, 0), new Point3d(0, 0, 0));
      _helper.CreateComponentWithLineInput(line);
      _helper.GetGridLineOutput();
      Assert.NotEmpty(_helper.GetComponent().RuntimeMessages(GH_RuntimeMessageLevel.Warning));
    }

    [Fact]
    public void ForCurveInputReturnWarning() {
      var pts = new Point3dList {
        new Point3d(0, 0, 0),
        new Point3d(10, 0, 0),
        new Point3d(10, 10, 0),
        new Point3d(10, 10, 10)
      };

      var crv = Curve.CreateControlPointCurve(pts, 3);
      _helper.SetLineInput(crv);
      _helper.GetGridLineOutput();
      Assert.NotEmpty(_helper.GetComponent().RuntimeMessages(GH_RuntimeMessageLevel.Warning));
    }

    [Fact]
    public void CreateModelWithGridLineReturnsSingleGridLine() {
      GsaModelGoo model = _helper.CreateModelWithGridlines();
      Assert.Single(model.Value.ApiModel.GridLines());
    }
  }

  public class CreateGridLineTestHelper {
    private readonly GH_OasysComponent _component;

    public CreateGridLineTestHelper() {
      _component = CreateComponent();
    }

    private static GH_OasysComponent CreateComponent() {
      var comp = new CreateGridLine();
      comp.CreateAttributes();

      return comp;
    }

    public void CreateComponentWithLineInput() {
      var defaultLine = new Line {
        FromX = 0,
        FromY = 0,
        FromZ = 0,
        ToX = 1,
        ToY = 1,
        ToZ = 0,
      };
      CreateComponentWithLineInput(defaultLine);
    }

    public void CreateComponentWithLineInput(Line line) {
      SetLineInput(line);
      SetLabelInput("Line");
    }

    public void CreateComponentWithArcInput() {
      var arc = new Arc(new Point3d(0, 0, 0), new Point3d(1, 1, 0), new Point3d(2, 0, 0));
      CreateComponentWithArcInput(arc);
    }

    public void CreateComponentWithArcInput(Arc arc) {
      SetLineInput(arc);
      SetLabelInput("Arc");
    }

    public GsaModelGoo CreateModelWithGridlines() {
      var comp = new CreateModel();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, GetGridLineOutput());
      return (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
    }

    public void SetLineInput(Line line) {
      ComponentTestHelper.SetInput(_component, line, 0);
    }

    public void SetLineInput(Curve curve) {
      ComponentTestHelper.SetInput(_component, curve, 0);
    }

    public void SetLineInput(Arc arc) {
      ComponentTestHelper.SetInput(_component, arc, 0);
    }

    public void SetLabelInput(string label) {
      ComponentTestHelper.SetInput(_component, label, 1);
    }

    public GsaGridLineGoo GetGridLineOutput() {
      return (GsaGridLineGoo)ComponentTestHelper.GetOutput(_component);
    }

    public GH_OasysComponent GetComponent() {
      return _component;
    }
  }
}
