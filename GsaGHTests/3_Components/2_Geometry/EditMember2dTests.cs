using System.Drawing;

using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using OasysUnits;

using Rhino.Geometry;

using Xunit;

using LengthUnit = OasysUnits.Units.LengthUnit;
using Line = Rhino.Geometry.Line;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class EditMember2dTests_WithoutSettingInputs {
    private readonly EditMember2dTestsHelper _helper;

    public EditMember2dTests_WithoutSettingInputs() {
      _helper = new EditMember2dTestsHelper();
    }

    [Fact]
    public void ComponentReturnValidMemberBrepAreaValue() {
      GsaMember2D output = _helper.GetMemberOutput();
      Assert.Equal(100, output.Brep.GetArea());
    }

    [Fact]
    public void ComponentReturnPlateTypeForMember() {
      GsaMember2D output = _helper.GetMemberOutput();
      Assert.Equal(Property2D_Type.PLATE, output.Prop2d.ApiProp2d.Type);
    }

    [Fact]
    public void ComponentReturnValidMemberThickness() {
      GsaMember2D output = _helper.GetMemberOutput();
      Assert.Equal(new Length(14, LengthUnit.Inch), output.Prop2d.Thickness);
    }

    [Fact]
    public void ComponentReturnValidMemberMeshSize() {
      GsaMember2D output = _helper.GetMemberOutput();
      Assert.Equal(0.5, output.ApiMember.MeshSize);
    }

    [Fact]
    public void ComponentReturnDefaultMemberGroupValue() {
      GsaMember2D output = _helper.GetMemberOutput();
      Assert.Equal(1, output.ApiMember.Group);
    }

    [Fact]
    public void ComponentReturnValidId() {
      int output = _helper.GetIdOutput();
      Assert.Equal(0, output);
    }

    [Fact]
    public void ComponentReturnValidBrepArea() {
      Brep output = _helper.GetBrepOutput();
      Assert.Equal(100, output.GetArea());
    }

    [Fact]
    public void ComponentReturnProperty2dPlateType() {
      GsaProperty2d output = _helper.Get2dPropertyOutput();
      Assert.Equal(Property2D_Type.PLATE, output.ApiProp2d.Type);
    }

    [Fact]
    public void ComponentReturnValidProperty2dThickness() {
      GsaProperty2d output = _helper.Get2dPropertyOutput();
      Assert.Equal(new Length(14, LengthUnit.Inch), output.Thickness);
    }

    [Fact]
    public void ComponentReturnValidDefaultMemberGroupValue() {
      int output = _helper.GetGroupOutput();
      Assert.Equal(1, output);
    }

    [Fact]
    public void ComponentReturnValidDefaultElementType() {
      string output = _helper.Get2dElementTypeOutput();
      Assert.Equal("Generic 2D", output);
    }

    [Fact]
    public void ComponentReturnValidDefaultMemberType() {
      string output = _helper.GetMemberTypeOutput();
      Assert.Equal("Linear", output);
    }

    [Fact]
    public void ComponentReturnValidOffsetX1Value() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(0, output.X1.Value, 6);
    }

    [Fact]
    public void ComponentReturnValidOffsetX2Value() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(0, output.X2.Value, 6);
    }

    [Fact]
    public void ComponentReturnValidOffsetYValue() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(0, output.Y.Value, 6);
    }

    [Fact]
    public void ComponentReturnValidOffsetZValue() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(0, output.Z.Value, 6);
    }

    [Fact]
    public void ComponentReturnValidInternalOffset() {
      bool output = _helper.GetInternalOffsetOutput();
      Assert.True(output);
    }

    [Fact]
    public void ComponentReturnValidMeshSize() {
      double output = _helper.GetMeshSizeOutput();
      Assert.Equal(0.5, output, 6);
    }

    [Fact]
    public void ComponentReturnValidIntersectorValue() {
      bool output = _helper.GetIntersectorOutput();
      Assert.True(output);
    }

    [Fact]
    public void ComponentReturnValidMeshMode() {
      string output = _helper.GetMeshModeOutput();
      Assert.Equal("Mixed", output);
    }

    [Fact]
    public void ComponentReturnValidAngle() {
      double output = _helper.GetAngleOutput();
      Assert.Equal(0.0, output, 6);
    }

    [Fact]
    public void ComponentReturnValidName() {
      string output = _helper.GetNameOutput();
      Assert.Empty(output);
    }

    [Fact]
    public void ComponentReturnValidColor() {
      Color output = _helper.GetColorOutput();
      Assert.Equal("ff000000", output.Name);
    }

    [Fact]
    public void ComponentReturnValidDummyValue() {
      bool output = _helper.GetDummyOutput();
      Assert.False(output);
    }

    [Fact]
    public void ComponentReturnValidTopology() {
      string output = _helper.GetTopologyOutput();
      Assert.Empty(output);
    }
  }

  [Collection("GrasshopperFixture collection")]
  public class EditMember2dTests_ForInputsSet {
    private readonly EditMember2dTestsHelper _helper;

    public EditMember2dTests_ForInputsSet() {
      _helper = new EditMember2dTestsHelper();
      _helper.SetIdInput(7);
      _helper.SetBrepInput(Brep.CreateFromCornerPoints(new Point3d(0, 0, 0), new Point3d(4, 0, 0), new Point3d(4, 4, 0),
        new Point3d(0, 4, 0), 1));
      _helper.SetPropertyInput(new GsaProperty2dGoo(new GsaProperty2d(new Length(200, LengthUnit.Millimeter))));
      _helper.SetGroupInput(1);
      _helper.Set2dElementTypeInput("Ribbed Slab");
      _helper.Set2dMemberTypeInput("Rigid Diaphragm");
      _helper.SetOffsetInput(new GsaOffsetGoo(new GsaOffset(1, 2, 3, 4)));
      _helper.SetInternalOffsetInput(true);
      _helper.SetMeshSizeInput(0.7);
      _helper.SetIntersectorInput(false);
      _helper.SetMeshModeInput(3);
      _helper.SetNameInput("name");
      _helper.SetColorInput(new GH_Colour(Color.White));
      _helper.SetDummyInput(true);
    }

    [Fact]
    public void EditMemberReturnValidMemberBrepAreaValue() {
      GsaMember2D output = _helper.GetMemberOutput();
      Assert.Equal(16, output.Brep.GetArea());
    }

    [Fact]
    public void EditMemberReturnShellTypeForMember() {
      GsaMember2D output = _helper.GetMemberOutput();
      Assert.Equal(Property2D_Type.SHELL, output.Prop2d.ApiProp2d.Type);
    }

    [Fact]
    public void EditMemberReturnValidMemberThickness() {
      GsaMember2D output = _helper.GetMemberOutput();
      Assert.Equal(new Length(200, LengthUnit.Millimeter), output.Prop2d.Thickness);
    }

    [Fact]
    public void EditMemberReturnValidMemberMeshSize() {
      GsaMember2D output = _helper.GetMemberOutput();
      Assert.Equal(0.7, output.ApiMember.MeshSize);
    }

    [Fact]
    public void EditMemberReturnDefaultMemberGroupValue() {
      GsaMember2D output = _helper.GetMemberOutput();
      Assert.Equal(1, output.ApiMember.Group);
    }

    [Fact]
    public void EditMemberReturnValidId() {
      int output = _helper.GetIdOutput();
      Assert.Equal(7, output);
    }

    [Fact]
    public void EditMemberReturnValidBrepArea() {
      Brep output = _helper.GetBrepOutput();
      Assert.Equal(16, output.GetArea());
    }

    [Fact]
    public void EditMemberReturnProperty2dShellType() {
      GsaProperty2d output = _helper.Get2dPropertyOutput();
      Assert.Equal(Property2D_Type.SHELL, output.ApiProp2d.Type);
    }

    [Fact]
    public void EditMemberReturnValidProperty2dThickness() {
      GsaProperty2d output = _helper.Get2dPropertyOutput();
      Assert.Equal(new Length(200, LengthUnit.Millimeter), output.Thickness);
    }

    [Fact]
    public void EditMemberReturnValidDefaultMemberGroupValue() {
      int output = _helper.GetGroupOutput();
      Assert.Equal(1, output);
    }

    [Fact]
    public void EditMemberReturnValidDefaultElementType() {
      string output = _helper.Get2dElementTypeOutput();
      Assert.Equal("Ribbed Slab", output);
    }

    [Fact]
    public void EditMemberReturnValidDefaultMemberType() {
      string output = _helper.GetMemberTypeOutput();
      Assert.Equal("Rigid Diaphragm", output);
    }

    [Fact]
    public void EditMemberReturnValidOffsetX1Value() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(1, output.X1.Value, 6);
    }

    [Fact]
    public void EditMemberReturnValidOffsetX2Value() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(2, output.X2.Value, 6);
    }

    [Fact]
    public void EditMemberReturnValidOffsetYValue() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(3, output.Y.Value, 6);
    }

    [Fact]
    public void EditMemberReturnValidOffsetZValue() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(4, output.Z.Value, 6);
    }

    [Fact]
    public void EditMemberReturnValidInternalOffset() {
      bool output = _helper.GetInternalOffsetOutput();
      Assert.True(output);
    }

    [Fact]
    public void EditMemberReturnValidMeshSize() {
      double output = _helper.GetMeshSizeOutput();
      Assert.Equal(0.7, output, 6);
    }

    [Fact]
    public void EditMemberReturnValidIntersectorValue() {
      bool output = _helper.GetIntersectorOutput();
      Assert.False(output);
    }

    [Fact]
    public void EditMemberReturnValidMeshMode() {
      string output = _helper.GetMeshModeOutput();
      Assert.Equal("Tri", output);
    }

    [Fact]
    public void EditMemberReturnValidAngle() {
      double output = _helper.GetAngleOutput();
      Assert.Equal(0.0, output, 6);
    }

    [Fact]
    public void EditMemberReturnValidName() {
      string output = _helper.GetNameOutput();
      Assert.Equal("name", output);
    }

    [Fact]
    public void EditMemberReturnValidColor() {
      Color output = _helper.GetColorOutput();
      Assert.Equal("ffffffff", output.Name);
    }

    [Fact]
    public void EditMemberReturnValidDummyValue() {
      bool output = _helper.GetDummyOutput();
      Assert.True(output);
    }

    [Fact]
    public void EditMemberReturnValidTopology() {
      string output = _helper.GetTopologyOutput();
      Assert.Empty(output);
    }
  }

  [Collection("GrasshopperFixture collection")]
  public class EditMember2dTests_InclusionMembers {
    private EditMember2dTestsHelper _helper;

    public EditMember2dTests_InclusionMembers() {
      _helper = new EditMember2dTestsHelper();
      _helper.SetBrepInput(Brep.CreateFromCornerPoints(new Point3d(0, 0, 0), new Point3d(4, 0, 0), new Point3d(4, 4, 0),
        new Point3d(0, 4, 0), 1));
      _helper.SetInclusionPointsInput(new Point3d(2, 2, 0));
      _helper.SetInclusionCurvesInput(new Line(new Point3d(3, 0, 0), new Point3d(3, 3, 0)));
    }

    [Fact]
    public void EditMemberReturnOneElementInInclusionPointsList() {
      GsaMember2D output = _helper.GetMemberOutput();
      Assert.Single(output.InclusionPoints);
    }

    [Fact]
    public void EditMemberReturnOneElementInInclusionLineList() {
      GsaMember2D output = _helper.GetMemberOutput();
      Assert.Single(output.InclusionLines);
    }

    [Fact]
    public void EditMemberReturnDefaultGroupValue() {
      GsaMember2D output = _helper.GetMemberOutput();
      Assert.Equal(1, output.ApiMember.Group);
    }

    [Fact]
    public void EditMemberReturnValidInclusionPointXValue() {
      Point3d output = _helper.GetInclusionPointsOutput();
      Assert.Equal(2, output.X);
    }

    [Fact]
    public void EditMemberReturnValidInclusionPointYValue() {
      Point3d output = _helper.GetInclusionPointsOutput();
      Assert.Equal(2, output.Y);
    }

    [Fact]
    public void EditMemberReturnValidInclusionPointZValue() {
      Point3d output = _helper.GetInclusionPointsOutput();
      Assert.Equal(0, output.Z);
    }

    [Fact]
    public void EditMemberReturnValidInclusionLinePointAtStartXValue() {
      Curve output = _helper.GetInclusionCurveOutput();
      Assert.Equal(3, output.PointAtStart.X);
    }

    [Fact]
    public void EditMemberReturnValidInclusionLinePointAtStartYValue() {
      Curve output = _helper.GetInclusionCurveOutput();
      Assert.Equal(0, output.PointAtStart.Y);
    }

    [Fact]
    public void EditMemberReturnValidInclusionLinePointAtStartZValue() {
      Curve output = _helper.GetInclusionCurveOutput();
      Assert.Equal(0, output.PointAtStart.Z);
    }

    [Fact]
    public void EditMemberReturnValidInclusionLinePointAtEndXValue() {
      Curve output = _helper.GetInclusionCurveOutput();
      Assert.Equal(3, output.PointAtEnd.X);
    }

    [Fact]
    public void EditMemberReturnValidInclusionLinePointAtEndYValue() {
      Curve output = _helper.GetInclusionCurveOutput();
      Assert.Equal(3, output.PointAtEnd.Y);
    }

    [Fact]
    public void EditMemberReturnValidInclusionLinePointAtEndZValue() {
      Curve output = _helper.GetInclusionCurveOutput();
      Assert.Equal(0, output.PointAtEnd.Z);
    }
  }

  [Collection("GrasshopperFixture collection")]
  public class EditMember2dTests_Extras {
    private EditMember2dTestsHelper _helper;

    public EditMember2dTests_Extras() {
      _helper = new EditMember2dTestsHelper();
    }

    [Theory]
    [InlineData(1, "Mixed")]
    [InlineData(3, "Tri")]
    [InlineData(4, "Quad")]
    public void EditMember2sReturnValidMeshModeWhenInputSet(int mode, string expected) {
      _helper.SetMeshModeInput(mode);
      string output = _helper.GetMeshModeOutput();
      Assert.Equal(expected, output);
    }

    [Theory]
    [InlineData((int)AnalysisOrder.LINEAR)]
    [InlineData((int)AnalysisOrder.QUADRATIC)]
    [InlineData((int)AnalysisOrder.RIGID_DIAPHRAGM)]
    [InlineData((int)AnalysisOrder.LOAD_PANEL)]
    public void CheckAnaysisOrderIsWorkingAsExpected(int analysisOrder) {
      _helper.Set2dMemberTypeInput(analysisOrder);
      string output = _helper.GetMemberTypeOutput();
      Assert.Equal((AnalysisOrder)analysisOrder, Mappings.GetAnalysisOrder(output));
    }
  }

  public class EditMember2dTestsHelper {
    private readonly GH_OasysComponent _component;

    public EditMember2dTestsHelper() {
      _component = ComponentMother();
    }

    private GH_OasysComponent ComponentMother() {
      var comp = new Edit2dMember();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, ComponentTestHelper.GetOutput(CreateMember2dTests.ComponentMother()), 0);

      return comp;
    }

    public GH_OasysComponent GetComponent() {
      return _component;
    }

    public GsaMember2D GetMemberOutput() {
      return ComponentTestHelper.GetMember2dOutput(_component, 0);
    }

    public int GetIdOutput() {
      return ComponentTestHelper.GetIntOutput(_component, 1);
    }

    public Brep GetBrepOutput() {
      return ComponentTestHelper.GetBrepOutput(_component, 2);
    }

    public Point3d GetInclusionPointsOutput() {
      return ComponentTestHelper.GetPointOutput(_component, 3);
    }

    public Curve GetInclusionCurveOutput() {
      return ComponentTestHelper.GetCurveOutput(_component, 4);
    }

    public GsaProperty2d Get2dPropertyOutput() {
      return ComponentTestHelper.Get2dPropertyOutput(_component, 5);
    }

    public int GetGroupOutput() {
      return ComponentTestHelper.GetIntOutput(_component, 6);
    }

    public string Get2dElementTypeOutput() {
      return ComponentTestHelper.GetStringOutput(_component, 7);
    }

    public string GetMemberTypeOutput() {
      return ComponentTestHelper.GetStringOutput(_component, 8);
    }

    public GsaOffset GetOffsetOutput() {
      return ComponentTestHelper.GetOffsetOutput(_component, 9);
    }

    public bool GetInternalOffsetOutput() {
      return ComponentTestHelper.GetBoolOutput(_component, 10);
    }

    public double GetMeshSizeOutput() {
      return ComponentTestHelper.GetNumberOutput(_component, 11);
    }

    public bool GetIntersectorOutput() {
      return ComponentTestHelper.GetBoolOutput(_component, 12);
    }

    public string GetMeshModeOutput() {
      return ComponentTestHelper.GetStringOutput(_component, 13);
    }

    public double GetAngleOutput() {
      return ComponentTestHelper.GetNumberOutput(_component, 14);
    }

    public string GetNameOutput() {
      return ComponentTestHelper.GetStringOutput(_component, 15);
    }

    public Color GetColorOutput() {
      return ComponentTestHelper.GetColorOutput(_component, 16);
    }

    public bool GetDummyOutput() {
      return ComponentTestHelper.GetBoolOutput(_component, 17);
    }

    public string GetTopologyOutput() {
      return ComponentTestHelper.GetStringOutput(_component, 18);
    }

    public void SetIdInput(int input) {
      ComponentTestHelper.SetInput(_component, input, 1);
    }

    public void SetBrepInput(Brep input) {
      ComponentTestHelper.SetInput(_component, input, 2);
    }

    public void SetInclusionPointsInput(Point3d input) {
      ComponentTestHelper.SetInput(_component, input, 3);
    }

    public void SetInclusionCurvesInput(Line input) {
      ComponentTestHelper.SetInput(_component, input, 4);
    }

    public void SetPropertyInput(GsaProperty2dGoo input) {
      ComponentTestHelper.SetInput(_component, input, 5);
    }

    public void SetGroupInput(int input) {
      ComponentTestHelper.SetInput(_component, input, 6);
    }

    public void Set2dElementTypeInput(string input) {
      ComponentTestHelper.SetInput(_component, input, 7);
    }

    public void Set2dMemberTypeInput(string input) {
      ComponentTestHelper.SetInput(_component, input, 8);
    }

    public void Set2dMemberTypeInput(int input) {
      ComponentTestHelper.SetInput(_component, input, 8);
    }

    public void SetOffsetInput(GsaOffsetGoo input) {
      ComponentTestHelper.SetInput(_component, input, 9);
    }

    public void SetInternalOffsetInput(bool input) {
      ComponentTestHelper.SetInput(_component, input, 10);
    }

    public void SetMeshSizeInput(double input) {
      ComponentTestHelper.SetInput(_component, input, 11);
    }

    public void SetIntersectorInput(bool input) {
      ComponentTestHelper.SetInput(_component, input, 12);
    }

    public void SetMeshModeInput(int input) {
      ComponentTestHelper.SetInput(_component, input, 13);
    }

    public void SetNameInput(string input) {
      ComponentTestHelper.SetInput(_component, input, 15);
    }

    public void SetColorInput(GH_Colour input) {
      ComponentTestHelper.SetInput(_component, input, 16);
    }

    public void SetDummyInput(bool input) {
      ComponentTestHelper.SetInput(_component, input, 17);
    }
  }
}
