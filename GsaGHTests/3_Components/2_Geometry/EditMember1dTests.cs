using System.Drawing;

using Grasshopper.Kernel.Types;

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
  //[Collection("GrasshopperFixture collection")]
  //public class EditMember1dTests_WithInputsSet {

  //  [Fact]
  //  public void CreateComponentTest2() {
  //    GH_OasysComponent comp = ComponentMother();
  //    ComponentTestHelper.SetInput(comp, 1, 1);
  //    ComponentTestHelper.SetInput(comp, new LineCurve(new Point3d(0, 0, 0), new Point3d(1, 2, 3)), 2);
  //    ComponentTestHelper.SetInput(comp, "STD CH 10 20 30 40", 3);
  //    ComponentTestHelper.SetInput(comp, 7, 4);
  //    ComponentTestHelper.SetInput(comp, "Cantilever", 5);
  //    ComponentTestHelper.SetInput(comp, "Damper", 6);
  //    ComponentTestHelper.SetInput(comp, new GsaOffsetGoo(new GsaOffset(1, 2, 3, 4)), 7);
  //    ComponentTestHelper.SetInput(comp, new GsaBool6Goo(new GsaBool6(true, true, true, true, true, true)), 8);
  //    ComponentTestHelper.SetInput(comp, new GsaBool6Goo(new GsaBool6(true, true, true, true, true, true)), 9);
  //    ComponentTestHelper.SetInput(comp, true, 10);
  //    ComponentTestHelper.SetInput(comp, true, 11);
  //    ComponentTestHelper.SetInput(comp, Math.PI, 12);
  //    var node = new GsaNode(new Point3d(1, 2, 3)) {
  //      Id = 99,
  //    };
  //    ComponentTestHelper.SetInput(comp, new GsaNodeGoo(node), 13);
  //    ComponentTestHelper.SetInput(comp, 0.7, 14);
  //    ComponentTestHelper.SetInput(comp, false, 15);
  //    var leff = new GsaEffectiveLengthOptions(new GsaMember1d()) {
  //      BucklingFactors = new GsaBucklingFactors(1, 2, 3),
  //    };
  //    ComponentTestHelper.SetInput(comp, new GsaEffectiveLengthOptionsGoo(leff), 16);
  //    ComponentTestHelper.SetInput(comp, "name", 17);
  //    ComponentTestHelper.SetInput(comp, new GH_Colour(Color.White), 18);
  //    ComponentTestHelper.SetInput(comp, true, 19);

  //    var output0 = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp, 0);
  //    var output1 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
  //    var output2 = (GH_Curve)ComponentTestHelper.GetOutput(comp, 2);
  //    var output3 = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 3);
  //    var output4 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 4);
  //    var output5 = (GH_String)ComponentTestHelper.GetOutput(comp, 5);
  //    var output6 = (GH_String)ComponentTestHelper.GetOutput(comp, 6);
  //    var output7 = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, 7);
  //    var output8 = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 8);
  //    var output9 = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 9);
  //    var output10 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 10);

  //    var output12 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 12);

  //    var output14 = (GH_Number)ComponentTestHelper.GetOutput(comp, 14);
  //    var output15 = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp, 15);
  //    var output16 = (GH_Number)ComponentTestHelper.GetOutput(comp, 16);
  //    var output17 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 17);
  //    var output18 = (GsaEffectiveLengthOptionsGoo)ComponentTestHelper.GetOutput(comp, 18);
  //    var output19 = (GH_String)ComponentTestHelper.GetOutput(comp, 19);
  //    var output20 = (GH_Colour)ComponentTestHelper.GetOutput(comp, 20);
  //    var output21 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 21);
  //    var output22 = (GH_String)ComponentTestHelper.GetOutput(comp, 22);

  //    Assert.Equal(0, output0.Value.PolyCurve.PointAtStart.X);
  //    Assert.Equal(0, output0.Value.PolyCurve.PointAtStart.Y);
  //    Assert.Equal(0, output0.Value.PolyCurve.PointAtStart.Z);
  //    Assert.Equal(1, output0.Value.PolyCurve.PointAtEnd.X);
  //    Assert.Equal(2, output0.Value.PolyCurve.PointAtEnd.Y);
  //    Assert.Equal(3, output0.Value.PolyCurve.PointAtEnd.Z);
  //    Assert.Equal("STD CH 10 20 30 40", output0.Value.Section.ApiSection.Profile);
  //    Assert.Equal(7, output0.Value.ApiMember.Group);
  //    Assert.Equal(1, output1.Value);
  //    Assert.Equal(0, output2.Value.PointAtStart.X);
  //    Assert.Equal(0, output2.Value.PointAtStart.Y);
  //    Assert.Equal(0, output2.Value.PointAtStart.Z);
  //    Assert.Equal(1, output2.Value.PointAtEnd.X);
  //    Assert.Equal(2, output2.Value.PointAtEnd.Y);
  //    Assert.Equal(3, output2.Value.PointAtEnd.Z);
  //    Assert.Equal("STD CH 10 20 30 40", output3.Value.ApiSection.Profile);
  //    Assert.Equal(7, output4.Value);
  //    Assert.Equal("Cantilever", output5.Value);
  //    Assert.Equal("Damper", output6.Value);
  //    Assert.Equal(1, output7.Value.X1.Value);
  //    Assert.Equal(2, output7.Value.X2.Value);
  //    Assert.Equal(3, output7.Value.Y.Value);
  //    Assert.Equal(4, output7.Value.Z.Value);
  //    Assert.True(output8.Value.X);
  //    Assert.True(output8.Value.Y);
  //    Assert.True(output8.Value.Z);
  //    Assert.True(output8.Value.Xx);
  //    Assert.True(output8.Value.Yy);
  //    Assert.True(output8.Value.Zz);
  //    Assert.True(output9.Value.X);
  //    Assert.True(output9.Value.Y);
  //    Assert.True(output9.Value.Z);
  //    Assert.True(output9.Value.Xx);
  //    Assert.True(output9.Value.Yy);
  //    Assert.True(output9.Value.Zz);
  //    Assert.True(output10.Value);
  //    Assert.True(output12.Value);
  //    Assert.Equal(Math.PI, output14.Value);
  //    Assert.Equal(1, output15.Value.Point.X);
  //    Assert.Equal(2, output15.Value.Point.Y);
  //    Assert.Equal(3, output15.Value.Point.Z);
  //    Assert.Equal(99, output15.Value.Id);
  //    Assert.Equal(0.7, output16.Value);
  //    Assert.False(output17.Value);
  //    Assert.Equal(1, output18.Value.BucklingFactors.MomentAmplificationFactorStrongAxis);
  //    Assert.Equal(2, output18.Value.BucklingFactors.MomentAmplificationFactorWeakAxis);
  //    Assert.Equal(3, output18.Value.BucklingFactors.EquivalentUniformMomentFactor);
  //    Assert.Equal("name", output19.Value);
  //    Assert.Equal(255, output20.Value.R);
  //    Assert.Equal(255, output20.Value.G);
  //    Assert.Equal(255, output20.Value.B);
  //    Assert.True(output21.Value);
  //    Assert.Equal("", output22.Value);
  //  }

  //  [Fact]
  //  public void TestElementBarInstabilityWarning() {
  //    GH_OasysComponent comp = ComponentMother();
  //    ComponentTestHelper.SetInput(comp, "Bar", 6);
  //    ComponentTestHelper.SetInput(comp, 0.0, 14);

  //    var output0 = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp, 0);
  //    Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));

  //    ComponentTestHelper.SetInput(comp, 1.0, 14);
  //    output0 = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp, 0);
  //    Assert.Single(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
  //  }

  //  [Fact]
  //  public void ChangeToSpringMember() {
  //    GH_OasysComponent comp = ComponentMother();
  //    var property = new AxialSpringProperty {
  //      Stiffness = 3.0,
  //    };
  //    ComponentTestHelper.SetInput(comp, new GsaPropertyGoo(new GsaSpringProperty(property)), 3);
  //    ComponentTestHelper.SetInput(comp, "Spring", 6);

  //    var output0 = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp, 0);
  //    var output1 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
  //    var output2 = (GH_Curve)ComponentTestHelper.GetOutput(comp, 2);
  //    var output3 = (GsaSpringPropertyGoo)ComponentTestHelper.GetOutput(comp, 3);
  //    var output4 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 4);
  //    var output5 = (GH_String)ComponentTestHelper.GetOutput(comp, 5);
  //    var output6 = (GH_String)ComponentTestHelper.GetOutput(comp, 6);
  //    var output7 = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, 7);
  //    var output8 = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 8);
  //    var output9 = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 9);
  //    var output10 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 10);
  //    var output12 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 12);
  //    var output14 = (GH_Number)ComponentTestHelper.GetOutput(comp, 14);
  //    var output15 = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp, 15);
  //    var output16 = (GH_Number)ComponentTestHelper.GetOutput(comp, 16);
  //    var output17 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 17);
  //    var output18 = (GsaEffectiveLengthOptionsGoo)ComponentTestHelper.GetOutput(comp, 18);
  //    var output19 = (GH_String)ComponentTestHelper.GetOutput(comp, 19);
  //    var output20 = (GH_Colour)ComponentTestHelper.GetOutput(comp, 20);
  //    var output21 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 21);
  //    var output22 = (GH_String)ComponentTestHelper.GetOutput(comp, 22);

  //    Assert.Equal(0, output0.Value.PolyCurve.PointAtStart.X, 6);
  //    Assert.Equal(-1, output0.Value.PolyCurve.PointAtStart.Y, 6);
  //    Assert.Equal(0, output0.Value.PolyCurve.PointAtStart.Z, 6);
  //    Assert.Equal(7, output0.Value.PolyCurve.PointAtEnd.X, 6);
  //    Assert.Equal(3, output0.Value.PolyCurve.PointAtEnd.Y, 6);
  //    Assert.Equal(1, output0.Value.PolyCurve.PointAtEnd.Z, 6);
  //    Assert.Equal(1, output0.Value.ApiMember.Group);
  //    Assert.Null(output0.Value.Section);
  //    Assert.Equal(0, output1.Value);
  //    Assert.Equal(0, output2.Value.PointAtStart.X, 6);
  //    Assert.Equal(-1, output2.Value.PointAtStart.Y, 6);
  //    Assert.Equal(0, output2.Value.PointAtStart.Z, 6);
  //    Assert.Equal(7, output2.Value.PointAtEnd.X, 6);
  //    Assert.Equal(3, output2.Value.PointAtEnd.Y, 6);
  //    Assert.Equal(1, output2.Value.PointAtEnd.Z, 6);
  //    Assert.NotNull(output3.Value);
  //    Assert.Equal(1, output4.Value);
  //    Assert.Equal("Generic 1D", output5.Value);
  //    Assert.Equal("Spring", output6.Value);
  //    Assert.Equal(0, output7.Value.X1.Value, 6);
  //    Assert.Equal(0, output7.Value.X2.Value, 6);
  //    Assert.Equal(0, output7.Value.Y.Value, 6);
  //    Assert.Equal(0, output7.Value.Z.Value, 6);
  //    Assert.False(output8.Value.X);
  //    Assert.False(output8.Value.Y);
  //    Assert.False(output8.Value.Z);
  //    Assert.False(output8.Value.Xx);
  //    Assert.False(output8.Value.Yy);
  //    Assert.False(output8.Value.Zz);
  //    Assert.False(output9.Value.X);
  //    Assert.False(output9.Value.Y);
  //    Assert.False(output9.Value.Z);
  //    Assert.False(output9.Value.Xx);
  //    Assert.False(output9.Value.Yy);
  //    Assert.False(output9.Value.Zz);
  //    Assert.False(output10.Value);
  //    Assert.False(output12.Value);
  //    Assert.Equal(0, output14.Value, 6);
  //    Assert.Null(output15.Value);
  //    Assert.Equal(0.5, output16.Value, 6);
  //    Assert.True(output17.Value);
  //    Assert.Null(output18.Value.BucklingFactors.MomentAmplificationFactorStrongAxis);
  //    Assert.Null(output18.Value.BucklingFactors.MomentAmplificationFactorWeakAxis);
  //    Assert.Null(output18.Value.BucklingFactors.EquivalentUniformMomentFactor);
  //    Assert.Equal("", output19.Value);
  //    Assert.Equal(0, output20.Value.R);
  //    Assert.Equal(0, output20.Value.G);
  //    Assert.Equal(0, output20.Value.B);
  //    Assert.False(output21.Value);
  //    Assert.Equal("", output22.Value);
  //  }

  //  [Fact]
  //  public void InvalidPropertyElement1DTypeCombination1() {
  //    GH_OasysComponent comp = ComponentMother();
  //    var property = new AxialSpringProperty {
  //      Stiffness = 3.0,
  //    };
  //    ComponentTestHelper.SetInput(comp, new GsaPropertyGoo(new GsaSpringProperty(property)), 3);
  //    ComponentTestHelper.SetInput(comp, "Beam", 6);

  //    comp.Params.Output[0].ExpireSolution(true);
  //    comp.Params.Output[0].CollectData();
  //    Assert.Single(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
  //  }

  //  [Fact]
  //  public void InvalidPropertyElement1DTypeCombination2() {
  //    GH_OasysComponent comp = ComponentMother();
  //    ComponentTestHelper.SetInput(comp, "STD CH 10 20 30 40", 3);
  //    ComponentTestHelper.SetInput(comp, "Spring", 6);

  //    comp.Params.Output[0].ExpireSolution(true);
  //    comp.Params.Output[0].CollectData();
  //    Assert.Single(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
  //  }

  //  [Fact]
  //  public void InvalidPropertyElement1DTypeCombination3() {
  //    var comp = new Edit1dMember();
  //    comp.CreateAttributes();
  //    ComponentTestHelper.SetInput(comp, "Spring", 6);

  //    comp.Params.Output[0].ExpireSolution(true);
  //    comp.Params.Output[0].CollectData();
  //    Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
  //  }

  //}

  public class EditMember1dTestsHelper {
    public readonly string DefaultMemberProfile = "STD CH(ft) 1 2 3 4";
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
  }
}
