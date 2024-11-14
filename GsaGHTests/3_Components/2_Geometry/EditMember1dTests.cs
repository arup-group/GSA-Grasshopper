using System;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class EditMember1dTests_WithoutSettingInputs {
    private readonly EditMember1dTestsHelper _helper;

    public EditMember1dTests_WithoutSettingInputs() {
      _helper = new EditMember1dTestsHelper();
    }

    [Fact]
    public void ComponentReturnValidMemberPolyCurvePointAtStartXValue() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(0, output.PolyCurve.PointAtStart.X, 6);
    }

    [Fact]
    public void ComponentReturnValidMemberPolyCurvePointAtStartYValue() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(-1, output.PolyCurve.PointAtStart.Y, 6);
    }

    [Fact]
    public void ComponentReturnValidMemberPolyCurvePointAtStartZValue() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(0, output.PolyCurve.PointAtStart.Z, 6);
    }

    [Fact]
    public void ComponentReturnValidMemberPolyCurvePointAtEndXValue() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(7, output.PolyCurve.PointAtEnd.X, 6);
    }

    [Fact]
    public void ComponentReturnValidMemberPolyCurvePointAtEndYValue() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(3, output.PolyCurve.PointAtEnd.Y, 6);
    }

    [Fact]
    public void ComponentReturnValidMemberPolyCurvePointAtEndZValue() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(1, output.PolyCurve.PointAtEnd.Z, 6);
    }

    [Fact]
    public void ComponentReturnValidMemberProfile() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(_helper.DefaultMemberProfile, output.Section.ApiSection.Profile);
    }

    [Fact]
    public void ComponentReturnDefaultMemberGroupForMember() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(1, output.ApiMember.Group);
    }

    [Fact]
    public void ComponentReturnValidId() {
      int output = _helper.GetIdOutput();
      Assert.Equal(0, output);
    }

    [Fact]
    public void ComponentReturnValidMemberCurvePointAtStartXValue() {
      Curve output = _helper.GetMemberCurveOutput();
      Assert.Equal(0, output.PointAtStart.X, 6);
    }

    [Fact]
    public void ComponentReturnValidMemberCurvePointAtStartYValue() {
      Curve output = _helper.GetMemberCurveOutput();
      Assert.Equal(-1, output.PointAtStart.Y, 6);
    }

    [Fact]
    public void ComponentReturnValidMemberCurvePointAtStartZValue() {
      Curve output = _helper.GetMemberCurveOutput();
      Assert.Equal(0, output.PointAtStart.Z, 6);
    }

    [Fact]
    public void ComponentReturnValidMemberCurvePointAtEndXValue() {
      Curve output = _helper.GetMemberCurveOutput();
      Assert.Equal(7, output.PointAtEnd.X, 6);
    }

    [Fact]
    public void ComponentReturnValidMemberCurvePointAtEndYValue() {
      Curve output = _helper.GetMemberCurveOutput();
      Assert.Equal(3, output.PointAtEnd.Y, 6);
    }

    [Fact]
    public void ComponentReturnValidMemberCurvePointAtEndZValue() {
      Curve output = _helper.GetMemberCurveOutput();
      Assert.Equal(1, output.PointAtEnd.Z, 6);
    }

    [Fact]
    public void ComponentReturnValidSection() {
      GsaSection output = _helper.GetSectionOutput();
      Assert.Equal(_helper.DefaultMemberProfile, output.ApiSection.Profile);
    }

    [Fact]
    public void ComponentReturnValidDefaultMemberGroupValue() {
      int output = _helper.GetMemberGroupOutput();
      Assert.Equal(1, output);
    }

    [Fact]
    public void ComponentReturnValidDefaultMemberType() {
      string output = _helper.GetMemberTypeOutput();
      Assert.Equal("Generic 1D", output);
    }

    [Fact]
    public void ComponentReturnValidDefaultElementType() {
      string output = _helper.GetElementTypeOutput();
      Assert.Equal("Beam", output);
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
    public void ComponentReturnValidStartReleaseXValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.False(output.X);
    }

    [Fact]
    public void ComponentReturnValidStartReleaseYValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.False(output.Y);
    }

    [Fact]
    public void ComponentReturnValidStartReleaseZValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.False(output.Z);
    }

    [Fact]
    public void ComponentReturnValidStartReleaseXxValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.False(output.Xx);
    }

    [Fact]
    public void ComponentReturnValidStartReleaseYYValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.False(output.Yy);
    }

    [Fact]
    public void ComponentReturnValidStartReleaseZzValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.False(output.Zz);
    }

    [Fact]
    public void ComponentReturnValidEndReleaseXValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.False(output.X);
    }

    [Fact]
    public void ComponentReturnValidEndReleaseYValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.False(output.Y);
    }

    [Fact]
    public void ComponentReturnValidEndReleaseZValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.False(output.Z);
    }

    [Fact]
    public void ComponentReturnValidEndReleaseXxValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.False(output.Xx);
    }

    [Fact]
    public void ComponentReturnValidEndReleaseYYValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.False(output.Yy);
    }

    [Fact]
    public void ComponentReturnValidEndReleaseZzValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.False(output.Zz);
    }

    [Fact]
    public void ComponentReturnValidAutomaticOffsetEnd1Value() {
      bool output = _helper.GetAutomaticOffsetEnd1Output();
      Assert.False(output);
    }

    [Fact]
    public void ComponentReturnValidAutomaticOffsetX1Value() {
      double output = _helper.GetAutomaticOffsetX1Output();
      Assert.Equal(0, output);
    }

    [Fact]
    public void ComponentReturnValidAutomaticOffsetEnd2Value() {
      bool output = _helper.GetAutomaticOffsetEnd2Output();
      Assert.False(output);
    }

    [Fact]
    public void ComponentReturnValidAutomaticOffsetX2Value() {
      double output = _helper.GetAutomaticOffsetX2Output();
      Assert.Equal(0, output);
    }

    [Fact]
    public void ComponentReturnValidAngle() {
      double output = _helper.GetAngleOutput();
      Assert.Equal(0, output, 6);
    }

    [Fact]
    public void ComponentReturnValidOrientation() {
      GsaNode output = _helper.GetOrientationOutput();
      Assert.Null(output);
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
    public void ComponentReturnValidBucklingFactorsMomentAmplificationFactorStrongAxis() {
      GsaEffectiveLengthOptions output = _helper.GetEffectiveLengthOutput();
      Assert.Null(output.BucklingFactors.MomentAmplificationFactorStrongAxis);
    }

    [Fact]
    public void ComponentReturnValidBucklingFactorsMomentAmplificationFactorWeakAxis() {
      GsaEffectiveLengthOptions output = _helper.GetEffectiveLengthOutput();
      Assert.Null(output.BucklingFactors.MomentAmplificationFactorWeakAxis);
    }

    [Fact]
    public void ComponentReturnValidBucklingFactorsEquivalentUniformMomentFactor() {
      GsaEffectiveLengthOptions output = _helper.GetEffectiveLengthOutput();
      Assert.Null(output.BucklingFactors.EquivalentUniformMomentFactor);
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
  public class EditMember1dTests_WithInputsSet {
    private readonly EditMember1dTestsHelper _helper;

    public EditMember1dTests_WithInputsSet() {
      _helper = new EditMember1dTestsHelper();
      _helper.SetIdInput(1);
      _helper.SetMemberCurveInput(new LineCurve(new Point3d(0, 0, 0), new Point3d(1, 2, 3)));
      _helper.SetSectionInput(_helper.MemberProfile);
      _helper.SetMemberGroupInput(7);
      _helper.SetMemberTypeInput("Cantilever");
      _helper.SetElementTypeInput("Damper");
      _helper.SetOffsetInput(new GsaOffsetGoo(new GsaOffset(1, 2, 3, 4)));
      _helper.SetStartReleaseInput(new GsaBool6Goo(new GsaBool6(true, true, true, true, true, true)));
      _helper.SetEndReleaseInput(new GsaBool6Goo(new GsaBool6(true, true, true, true, true, true)));
      _helper.SetAutomaticOffsetEnd1Input(true);
      _helper.SetAutomaticOffsetEnd2Input(true);
      _helper.SetAngleInput(Math.PI);
      var node = new GsaNode(new Point3d(1, 2, 3)) {
        Id = 99,
      };
      _helper.SetOrientationInput(new GsaNodeGoo(node));
      _helper.SetMeshSizeInput(0.7);
      _helper.SetIntersectorInput(false);
      var leff = new GsaEffectiveLengthOptions(new GsaMember1d()) {
        BucklingFactors = new GsaBucklingFactors(1, 2, 3),
      };
      _helper.SetEffectiveLengthInput(new GsaEffectiveLengthOptionsGoo(leff));
      _helper.SetNameInput("name");
      _helper.SetColorInput(new GH_Colour(Color.White));
      _helper.SetDummyInput(true);
    }

    [Fact]
    public void EditMember1dReturnValidMemberPolyCurvePointAtStartXValue() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(0, output.PolyCurve.PointAtStart.X, 6);
    }

    [Fact]
    public void EditMember1dReturnValidMemberPolyCurvePointAtStartYValue() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(0, output.PolyCurve.PointAtStart.Y, 6);
    }

    [Fact]
    public void EditMember1dReturnValidMemberPolyCurvePointAtStartZValue() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(0, output.PolyCurve.PointAtStart.Z, 6);
    }

    [Fact]
    public void EditMember1dReturnValidMemberPolyCurvePointAtEndXValue() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(1, output.PolyCurve.PointAtEnd.X, 6);
    }

    [Fact]
    public void EditMember1dReturnValidMemberPolyCurvePointAtEndYValue() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(2, output.PolyCurve.PointAtEnd.Y, 6);
    }

    [Fact]
    public void EditMember1dReturnValidMemberPolyCurvePointAtEndZValue() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(3, output.PolyCurve.PointAtEnd.Z, 6);
    }

    [Fact]
    public void EditMember1dReturnValidMemberProfile() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(_helper.MemberProfile, output.Section.ApiSection.Profile);
    }

    [Fact]
    public void EditMember1dReturnMemberGroupForMember() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(7, output.ApiMember.Group);
    }

    [Fact]
    public void EditMember1dReturnValidId() {
      int output = _helper.GetIdOutput();
      Assert.Equal(1, output);
    }

    [Fact]
    public void EditMember1dReturnValidMemberCurvePointAtStartXValue() {
      Curve output = _helper.GetMemberCurveOutput();
      Assert.Equal(0, output.PointAtStart.X, 6);
    }

    [Fact]
    public void EditMember1dReturnValidMemberCurvePointAtStartYValue() {
      Curve output = _helper.GetMemberCurveOutput();
      Assert.Equal(0, output.PointAtStart.Y, 6);
    }

    [Fact]
    public void EditMember1dReturnValidMemberCurvePointAtStartZValue() {
      Curve output = _helper.GetMemberCurveOutput();
      Assert.Equal(0, output.PointAtStart.Z, 6);
    }

    [Fact]
    public void EditMember1dReturnValidMemberCurvePointAtEndXValue() {
      Curve output = _helper.GetMemberCurveOutput();
      Assert.Equal(1, output.PointAtEnd.X, 6);
    }

    [Fact]
    public void EditMember1dReturnValidMemberCurvePointAtEndYValue() {
      Curve output = _helper.GetMemberCurveOutput();
      Assert.Equal(2, output.PointAtEnd.Y, 6);
    }

    [Fact]
    public void EditMember1dReturnValidMemberCurvePointAtEndZValue() {
      Curve output = _helper.GetMemberCurveOutput();
      Assert.Equal(3, output.PointAtEnd.Z, 6);
    }

    [Fact]
    public void EditMember1dReturnValidSection() {
      GsaSection output = _helper.GetSectionOutput();
      Assert.Equal(_helper.MemberProfile, output.ApiSection.Profile);
    }

    [Fact]
    public void EditMember1dReturnValidMemberGroupValue() {
      int output = _helper.GetMemberGroupOutput();
      Assert.Equal(7, output);
    }

    [Fact]
    public void EditMember1dReturnValidMemberType() {
      string output = _helper.GetMemberTypeOutput();
      Assert.Equal("Cantilever", output);
    }

    [Fact]
    public void EditMember1dReturnValidElementType() {
      string output = _helper.GetElementTypeOutput();
      Assert.Equal("Damper", output);
    }

    [Fact]
    public void EditMember1dReturnValidOffsetX1Value() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(1, output.X1.Value, 6);
    }

    [Fact]
    public void EditMember1dReturnValidOffsetX2Value() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(2, output.X2.Value, 6);
    }

    [Fact]
    public void EditMember1dReturnValidOffsetYValue() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(3, output.Y.Value, 6);
    }

    [Fact]
    public void EditMember1dReturnValidOffsetZValue() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(4, output.Z.Value, 6);
    }

    [Fact]
    public void EditMember1dReturnValidStartReleaseXValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.True(output.X);
    }

    [Fact]
    public void EditMember1dReturnValidStartReleaseYValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.True(output.Y);
    }

    [Fact]
    public void EditMember1dReturnValidStartReleaseZValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.True(output.Z);
    }

    [Fact]
    public void EditMember1dReturnValidStartReleaseXxValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.True(output.Xx);
    }

    [Fact]
    public void EditMember1dReturnValidStartReleaseYYValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.True(output.Yy);
    }

    [Fact]
    public void EditMember1dReturnValidStartReleaseZzValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.True(output.Zz);
    }

    [Fact]
    public void EditMember1dReturnValidEndReleaseXValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.True(output.X);
    }

    [Fact]
    public void EditMember1dReturnValidEndReleaseYValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.True(output.Y);
    }

    [Fact]
    public void EditMember1dReturnValidEndReleaseZValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.True(output.Z);
    }

    [Fact]
    public void EditMember1dReturnValidEndReleaseXxValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.True(output.Xx);
    }

    [Fact]
    public void EditMember1dReturnValidEndReleaseYYValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.True(output.Yy);
    }

    [Fact]
    public void EditMember1dReturnValidEndReleaseZzValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.True(output.Zz);
    }

    [Fact]
    public void EditMember1dReturnValidAutomaticOffsetEnd1Value() {
      bool output = _helper.GetAutomaticOffsetEnd1Output();
      Assert.True(output);
    }

    [Fact]
    public void EditMember1dReturnValidAutomaticOffsetX1Value() {
      double output = _helper.GetAutomaticOffsetX1Output();
      Assert.Equal(0, output);
    }

    [Fact]
    public void EditMember1dReturnValidAutomaticOffsetEnd2Value() {
      bool output = _helper.GetAutomaticOffsetEnd2Output();
      Assert.True(output);
    }

    [Fact]
    public void EditMember1dReturnValidAutomaticOffsetX2Value() {
      double output = _helper.GetAutomaticOffsetX2Output();
      Assert.Equal(0, output);
    }

    [Fact]
    public void EditMember1dReturnValidAngle() {
      double output = _helper.GetAngleOutput();
      Assert.Equal(Math.PI, output, 6);
    }

    [Fact]
    public void EditMember1dReturnValidOrientationX() {
      GsaNode output = _helper.GetOrientationOutput();
      Assert.Equal(1, output.Point.X);
    }

    [Fact]
    public void EditMember1dReturnValidOrientationY() {
      GsaNode output = _helper.GetOrientationOutput();
      Assert.Equal(2, output.Point.Y);
    }

    [Fact]
    public void EditMember1dReturnValidOrientationZ() {
      GsaNode output = _helper.GetOrientationOutput();
      Assert.Equal(3, output.Point.Z);
    }

    [Fact]
    public void EditMember1dReturnValidOrientationId() {
      GsaNode output = _helper.GetOrientationOutput();
      Assert.Equal(99, output.Id);
    }

    [Fact]
    public void EditMember1dReturnValidMeshSize() {
      double output = _helper.GetMeshSizeOutput();
      Assert.Equal(0.7, output, 6);
    }

    [Fact]
    public void EditMember1dReturnValidIntersectorValue() {
      bool output = _helper.GetIntersectorOutput();
      Assert.False(output);
    }

    [Fact]
    public void EditMember1dReturnValidBucklingFactorsMomentAmplificationFactorStrongAxis() {
      GsaEffectiveLengthOptions output = _helper.GetEffectiveLengthOutput();
      Assert.Equal(1, output.BucklingFactors.MomentAmplificationFactorStrongAxis);
    }

    [Fact]
    public void EditMember1dReturnValidBucklingFactorsMomentAmplificationFactorWeakAxis() {
      GsaEffectiveLengthOptions output = _helper.GetEffectiveLengthOutput();
      Assert.Equal(2, output.BucklingFactors.MomentAmplificationFactorWeakAxis);
    }

    [Fact]
    public void EditMember1dReturnValidBucklingFactorsEquivalentUniformMomentFactor() {
      GsaEffectiveLengthOptions output = _helper.GetEffectiveLengthOutput();
      Assert.Equal(3, output.BucklingFactors.EquivalentUniformMomentFactor);
    }

    [Fact]
    public void EditMember1dReturnValidName() {
      string output = _helper.GetNameOutput();
      Assert.Equal("name", output);
    }

    [Fact]
    public void EditMember1dReturnValidColor() {
      Color output = _helper.GetColorOutput();
      Assert.Equal("ffffffff", output.Name);
    }

    [Fact]
    public void EditMember1dReturnValidDummyValue() {
      bool output = _helper.GetDummyOutput();
      Assert.True(output);
    }

    [Fact]
    public void EditMember1dReturnValidTopology() {
      string output = _helper.GetTopologyOutput();
      Assert.Empty(output);
    }
  }

  [Collection("GrasshopperFixture collection")]
  public class EditMember1dTests_ChangeToSpringMember {
    private EditMember1dTestsHelper _helper;

    public EditMember1dTests_ChangeToSpringMember() {
      _helper = new EditMember1dTestsHelper();
      var property = new AxialSpringProperty {
        Stiffness = 3.0,
      };
      _helper.SetSpringPropertyInput(new GsaPropertyGoo(new GsaSpringProperty(property)));
      _helper.SetElementTypeInput("Spring");
    }

    [Fact]
    public void EditMember1dReturnValidMemberPolyCurvePointAtStartXValue() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(0, output.PolyCurve.PointAtStart.X, 6);
    }

    [Fact]
    public void EditMember1dReturnValidMemberPolyCurvePointAtStartYValue() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(-1, output.PolyCurve.PointAtStart.Y, 6);
    }

    [Fact]
    public void EditMember1dReturnValidMemberPolyCurvePointAtStartZValue() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(0, output.PolyCurve.PointAtStart.Z, 6);
    }

    [Fact]
    public void EditMember1dReturnValidMemberPolyCurvePointAtEndXValue() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(7, output.PolyCurve.PointAtEnd.X, 6);
    }

    [Fact]
    public void EditMember1dReturnValidMemberPolyCurvePointAtEndYValue() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(3, output.PolyCurve.PointAtEnd.Y, 6);
    }

    [Fact]
    public void EditMember1dReturnValidMemberPolyCurvePointAtEndZValue() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(1, output.PolyCurve.PointAtEnd.Z, 6);
    }

    [Fact]
    public void EditMember1dReturnValidMemberProfile() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Null(output.Section);
    }

    [Fact]
    public void EditMember1dReturnDefaultMemberGroupForMember() {
      GsaMember1d output = _helper.GetMemberOutput();
      Assert.Equal(1, output.ApiMember.Group);
    }

    [Fact]
    public void EditMember1dReturnValidId() {
      int output = _helper.GetIdOutput();
      Assert.Equal(0, output);
    }

    [Fact]
    public void EditMember1dReturnValidMemberCurvePointAtStartXValue() {
      Curve output = _helper.GetMemberCurveOutput();
      Assert.Equal(0, output.PointAtStart.X, 6);
    }

    [Fact]
    public void EditMember1dReturnValidMemberCurvePointAtStartYValue() {
      Curve output = _helper.GetMemberCurveOutput();
      Assert.Equal(-1, output.PointAtStart.Y, 6);
    }

    [Fact]
    public void EditMember1dReturnValidMemberCurvePointAtStartZValue() {
      Curve output = _helper.GetMemberCurveOutput();
      Assert.Equal(0, output.PointAtStart.Z, 6);
    }

    [Fact]
    public void EditMember1dReturnValidMemberCurvePointAtEndXValue() {
      Curve output = _helper.GetMemberCurveOutput();
      Assert.Equal(7, output.PointAtEnd.X, 6);
    }

    [Fact]
    public void EditMember1dReturnValidMemberCurvePointAtEndYValue() {
      Curve output = _helper.GetMemberCurveOutput();
      Assert.Equal(3, output.PointAtEnd.Y, 6);
    }

    [Fact]
    public void EditMember1dReturnValidMemberCurvePointAtEndZValue() {
      Curve output = _helper.GetMemberCurveOutput();
      Assert.Equal(1, output.PointAtEnd.Z, 6);
    }

    [Fact]
    public void EditMember1dReturnValidSection() {
      GsaSpringProperty output = _helper.GetSpringPropertyOutput();
      Assert.NotNull(output);
    }

    [Fact]
    public void EditMember1dReturnValidDefaultMemberGroupValue() {
      int output = _helper.GetMemberGroupOutput();
      Assert.Equal(1, output);
    }

    [Fact]
    public void EditMember1dReturnValidMemberType() {
      string output = _helper.GetMemberTypeOutput();
      Assert.Equal("Generic 1D", output);
    }

    [Fact]
    public void EditMember1dReturnValidElementType() {
      string output = _helper.GetElementTypeOutput();
      Assert.Equal("Spring", output);
    }

    [Fact]
    public void EditMember1dReturnValidOffsetX1Value() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(0, output.X1.Value, 6);
    }

    [Fact]
    public void EditMember1dReturnValidOffsetX2Value() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(0, output.X2.Value, 6);
    }

    [Fact]
    public void EditMember1dReturnValidOffsetYValue() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(0, output.Y.Value, 6);
    }

    [Fact]
    public void EditMember1dReturnValidOffsetZValue() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(0, output.Z.Value, 6);
    }

    [Fact]
    public void EditMember1dReturnValidStartReleaseXValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.False(output.X);
    }

    [Fact]
    public void EditMember1dReturnValidStartReleaseYValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.False(output.Y);
    }

    [Fact]
    public void EditMember1dReturnValidStartReleaseZValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.False(output.Z);
    }

    [Fact]
    public void EditMember1dReturnValidStartReleaseXxValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.False(output.Xx);
    }

    [Fact]
    public void EditMember1dReturnValidStartReleaseYYValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.False(output.Yy);
    }

    [Fact]
    public void EditMember1dReturnValidStartReleaseZzValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.False(output.Zz);
    }

    [Fact]
    public void EditMember1dReturnValidEndReleaseXValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.False(output.X);
    }

    [Fact]
    public void EditMember1dReturnValidEndReleaseYValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.False(output.Y);
    }

    [Fact]
    public void EditMember1dReturnValidEndReleaseZValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.False(output.Z);
    }

    [Fact]
    public void EditMember1dReturnValidEndReleaseXxValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.False(output.Xx);
    }

    [Fact]
    public void EditMember1dReturnValidEndReleaseYYValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.False(output.Yy);
    }

    [Fact]
    public void EditMember1dReturnValidEndReleaseZzValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.False(output.Zz);
    }

    [Fact]
    public void EditMember1dReturnValidAutomaticOffsetEnd1Value() {
      bool output = _helper.GetAutomaticOffsetEnd1Output();
      Assert.False(output);
    }

    [Fact]
    public void EditMember1dReturnValidAutomaticOffsetX1Value() {
      double output = _helper.GetAutomaticOffsetX1Output();
      Assert.Equal(0, output);
    }

    [Fact]
    public void EditMember1dReturnValidAutomaticOffsetEnd2Value() {
      bool output = _helper.GetAutomaticOffsetEnd2Output();
      Assert.False(output);
    }

    [Fact]
    public void EditMember1dReturnValidAutomaticOffsetX2Value() {
      double output = _helper.GetAutomaticOffsetX2Output();
      Assert.Equal(0, output);
    }

    [Fact]
    public void EditMember1dReturnValidAngle() {
      double output = _helper.GetAngleOutput();
      Assert.Equal(0, output, 6);
    }

    [Fact]
    public void EditMember1dReturnValidOrientationX() {
      GsaNode output = _helper.GetOrientationOutput();
      Assert.Null(output);
    }

    [Fact]
    public void EditMember1dReturnValidMeshSize() {
      double output = _helper.GetMeshSizeOutput();
      Assert.Equal(0.5, output, 6);
    }

    [Fact]
    public void EditMember1dReturnValidIntersectorValue() {
      bool output = _helper.GetIntersectorOutput();
      Assert.True(output);
    }

    [Fact]
    public void EditMember1dReturnValidBucklingFactorsMomentAmplificationFactorStrongAxis() {
      GsaEffectiveLengthOptions output = _helper.GetEffectiveLengthOutput();
      Assert.Null(output.BucklingFactors.MomentAmplificationFactorStrongAxis);
    }

    [Fact]
    public void EditMember1dReturnValidBucklingFactorsMomentAmplificationFactorWeakAxis() {
      GsaEffectiveLengthOptions output = _helper.GetEffectiveLengthOutput();
      Assert.Null(output.BucklingFactors.MomentAmplificationFactorWeakAxis);
    }

    [Fact]
    public void EditMember1dReturnValidBucklingFactorsEquivalentUniformMomentFactor() {
      GsaEffectiveLengthOptions output = _helper.GetEffectiveLengthOutput();
      Assert.Null(output.BucklingFactors.EquivalentUniformMomentFactor);
    }

    [Fact]
    public void EditMember1dReturnValidName() {
      string output = _helper.GetNameOutput();
      Assert.Empty(output);
    }

    [Fact]
    public void EditMember1dReturnValidColor() {
      Color output = _helper.GetColorOutput();
      Assert.Equal("ff000000", output.Name);
    }

    [Fact]
    public void EditMember1dReturnValidDummyValue() {
      bool output = _helper.GetDummyOutput();
      Assert.False(output);
    }

    [Fact]
    public void EditMember1dReturnValidTopology() {
      string output = _helper.GetTopologyOutput();
      Assert.Empty(output);
    }
  }

  [Collection("GrasshopperFixture collection")]
  public class EditMember1dTests_ErrorsHandling {
    private EditMember1dTestsHelper _helper;

    public EditMember1dTests_ErrorsHandling() {
      _helper = new EditMember1dTestsHelper();
    }

    [Fact]
    public void ComponentNotReturnWarningWhenElementIsBarAndMeshSizeIsIs0() {
      _helper.SetElementTypeInput("Bar");
      _helper.SetMeshSizeInput(0.0);
      _helper.GetMemberOutput();
      Assert.Empty(_helper.GetComponent().RuntimeMessages(GH_RuntimeMessageLevel.Warning));
    }

    [Fact]
    public void ComponentReturnWarningWhenElementIsBarAndMeshSizeIsIs1() {
      _helper.SetElementTypeInput("Bar");
      _helper.SetMeshSizeInput(1.0);
      _helper.GetMemberOutput();
      Assert.Single(_helper.GetComponent().RuntimeMessages(GH_RuntimeMessageLevel.Warning));
    }

    [Fact]
    public void InvalidPropertyElement1DTypeCombination1() {
      var property = new AxialSpringProperty {
        Stiffness = 3.0,
      };
      _helper.SetSpringPropertyInput(new GsaPropertyGoo(new GsaSpringProperty(property)));
      _helper.SetElementTypeInput("Beam");

      _helper.GetComponent().Params.Output[0].ExpireSolution(true);
      _helper.GetComponent().Params.Output[0].CollectData();
      Assert.Single(_helper.GetComponent().RuntimeMessages(GH_RuntimeMessageLevel.Remark));
    }

    [Fact]
    public void InvalidPropertyElement1DTypeCombination2() {
      _helper.SetSectionInput(_helper.MemberProfile);
      _helper.SetElementTypeInput("Spring");

      _helper.GetComponent().Params.Output[0].ExpireSolution(true);
      _helper.GetComponent().Params.Output[0].CollectData();
      Assert.Single(_helper.GetComponent().RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void InvalidPropertyElement1DTypeCombination3() {
      var comp = new Edit1dMember();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, "Spring", 6);

      _helper.GetComponent().Params.Output[0].ExpireSolution(true);
      _helper.GetComponent().Params.Output[0].CollectData();
      Assert.Empty(_helper.GetComponent().RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }
  }

  public class EditMember1dTestsHelper {
    public readonly string DefaultMemberProfile = "STD CH(ft) 1 2 3 4";
    public readonly string MemberProfile = "STD CH 10 20 30 40";
    public readonly int DefaultId = 0;
    private readonly GH_OasysComponent _component;

    public EditMember1dTestsHelper() {
      _component = ComponentMother();
    }

    private static GH_OasysComponent ComponentMother() {
      var comp = new Edit1dMember();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, ComponentTestHelper.GetOutput(CreateMember1dTests.ComponentMother()), 0);

      return comp;
    }

    public GH_OasysComponent GetComponent() {
      return _component;
    }

    public GsaMember1d GetMemberOutput() {
      var element = (GsaMember1dGoo)ComponentTestHelper.GetOutput(_component, 0);
      return element.Value;
    }

    public int GetIdOutput() {
      var id = (GH_Integer)ComponentTestHelper.GetOutput(_component, 1);
      return id.Value;
    }

    public Curve GetMemberCurveOutput() {
      var curve = (GH_Curve)ComponentTestHelper.GetOutput(_component, 2);
      return curve.Value;
    }

    public GsaSection GetSectionOutput() {
      var section = (GsaSectionGoo)ComponentTestHelper.GetOutput(_component, 3);
      return section.Value;
    }

    public GsaSpringProperty GetSpringPropertyOutput() {
      var property = (GsaSpringPropertyGoo)ComponentTestHelper.GetOutput(_component, 3);
      return property.Value;
    }

    public int GetMemberGroupOutput() {
      var group = (GH_Integer)ComponentTestHelper.GetOutput(_component, 4);
      return group.Value;
    }

    public string GetMemberTypeOutput() {
      var type = (GH_String)ComponentTestHelper.GetOutput(_component, 5);
      return type.Value;
    }

    public string GetElementTypeOutput() {
      var type = (GH_String)ComponentTestHelper.GetOutput(_component, 6);
      return type.Value;
    }

    public GsaOffset GetOffsetOutput() {
      var offset = (GsaOffsetGoo)ComponentTestHelper.GetOutput(_component, 7);
      return offset.Value;
    }

    public GsaBool6 GetStartReleaseOutput() {
      var startRelease = (GsaBool6Goo)ComponentTestHelper.GetOutput(_component, 8);
      return startRelease.Value;
    }

    public GsaBool6 GetEndReleaseOutput() {
      var endRelease = (GsaBool6Goo)ComponentTestHelper.GetOutput(_component, 9);
      return endRelease.Value;
    }

    public bool GetAutomaticOffsetEnd1Output() {
      var autoBoolean = (GH_Boolean)ComponentTestHelper.GetOutput(_component, 10);
      return autoBoolean.Value;
    }

    public double GetAutomaticOffsetX1Output() {
      var offsetX1 = (GH_Number)ComponentTestHelper.GetOutput(_component, 11);
      return offsetX1.Value;
    }

    public bool GetAutomaticOffsetEnd2Output() {
      var autoBoolean = (GH_Boolean)ComponentTestHelper.GetOutput(_component, 12);
      return autoBoolean.Value;
    }

    public double GetAutomaticOffsetX2Output() {
      var offsetX1 = (GH_Number)ComponentTestHelper.GetOutput(_component, 13);
      return offsetX1.Value;
    }

    public double GetAngleOutput() {
      var angle = (GH_Number)ComponentTestHelper.GetOutput(_component, 14);
      return angle.Value;
    }

    public GsaNode GetOrientationOutput() {
      var orientation = (GsaNodeGoo)ComponentTestHelper.GetOutput(_component, 15);
      return orientation.Value;
    }

    public double GetMeshSizeOutput() {
      var mesh = (GH_Number)ComponentTestHelper.GetOutput(_component, 16);
      return mesh.Value;
    }

    public bool GetIntersectorOutput() {
      var intersector = (GH_Boolean)ComponentTestHelper.GetOutput(_component, 17);
      return intersector.Value;
    }

    public GsaEffectiveLengthOptions GetEffectiveLengthOutput() {
      var effectiveLength = (GsaEffectiveLengthOptionsGoo)ComponentTestHelper.GetOutput(_component, 18);
      return effectiveLength.Value;
    }

    public string GetNameOutput() {
      var name = (GH_String)ComponentTestHelper.GetOutput(_component, 19);
      return name.Value;
    }

    public Color GetColorOutput() {
      var colour = (GH_Colour)ComponentTestHelper.GetOutput(_component, 20);
      return colour.Value;
    }

    public bool GetDummyOutput() {
      var dummy = (GH_Boolean)ComponentTestHelper.GetOutput(_component, 21);
      return dummy.Value;
    }

    public string GetTopologyOutput() {
      var topology = (GH_String)ComponentTestHelper.GetOutput(_component, 22);
      return topology.Value;
    }

    public void SetIdInput() {
      SetIdInput(DefaultId);
    }

    public void SetSectionInput() {
      SetSectionInput(DefaultMemberProfile);
    }

    public void SetIdInput(int input) {
      ComponentTestHelper.SetInput(_component, input, 1);
    }

    public void SetMemberCurveInput(LineCurve input) {
      ComponentTestHelper.SetInput(_component, input, 2);
    }

    public void SetSectionInput(string input) {
      ComponentTestHelper.SetInput(_component, input, 3);
    }

    public void SetSpringPropertyInput(GsaPropertyGoo input) {
      ComponentTestHelper.SetInput(_component, input, 3);
    }

    public void SetMemberGroupInput(int input) {
      ComponentTestHelper.SetInput(_component, input, 4);
    }

    public void SetMemberTypeInput(string input) {
      ComponentTestHelper.SetInput(_component, input, 5);
    }

    public void SetElementTypeInput(string input) {
      ComponentTestHelper.SetInput(_component, input, 6);
    }

    public void SetOffsetInput(GsaOffsetGoo input) {
      ComponentTestHelper.SetInput(_component, input, 7);
    }

    public void SetStartReleaseInput(GsaBool6Goo input) {
      ComponentTestHelper.SetInput(_component, input, 8);
    }

    public void SetEndReleaseInput(GsaBool6Goo input) {
      ComponentTestHelper.SetInput(_component, input, 9);
    }

    public void SetAutomaticOffsetEnd1Input(bool input) {
      ComponentTestHelper.SetInput(_component, input, 10);
    }

    public void SetAutomaticOffsetEnd2Input(bool input) {
      ComponentTestHelper.SetInput(_component, input, 11);
    }

    public void SetAngleInput(double input) {
      ComponentTestHelper.SetInput(_component, input, 12);
    }

    public void SetOrientationInput(GsaNodeGoo input) {
      ComponentTestHelper.SetInput(_component, input, 13);
    }

    public void SetMeshSizeInput(double input) {
      ComponentTestHelper.SetInput(_component, input, 14);
    }

    public void SetIntersectorInput(bool input) {
      ComponentTestHelper.SetInput(_component, input, 15);
    }

    public void SetEffectiveLengthInput(GsaEffectiveLengthOptionsGoo input) {
      ComponentTestHelper.SetInput(_component, input, 16);
    }

    public void SetNameInput(string input) {
      ComponentTestHelper.SetInput(_component, input, 17);
    }

    public void SetColorInput(GH_Colour input) {
      ComponentTestHelper.SetInput(_component, input, 18);
    }

    public void SetDummyInput(bool input) {
      ComponentTestHelper.SetInput(_component, input, 19);
    }
  }
}
