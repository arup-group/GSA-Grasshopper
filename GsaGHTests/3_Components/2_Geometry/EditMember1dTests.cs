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
  public class EditMember1dTests {

    [Fact]
    public void CreateComponentTest1() {
      GH_OasysComponent comp = ComponentMother();

      var output0 = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp, 0);
      var output1 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      var output2 = (GH_Curve)ComponentTestHelper.GetOutput(comp, 2);
      var output3 = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 3);
      var output4 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 4);
      var output5 = (GH_String)ComponentTestHelper.GetOutput(comp, 5);
      var output6 = (GH_String)ComponentTestHelper.GetOutput(comp, 6);
      var output7 = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, 7);
      var output8 = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 8);
      var output9 = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 9);
      var output10 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 10);
      var output12 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 12);
      var output14 = (GH_Number)ComponentTestHelper.GetOutput(comp, 14);
      var output15 = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp, 15);
      var output16 = (GH_Number)ComponentTestHelper.GetOutput(comp, 16);
      var output17 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 17);
      var output18 = (GsaEffectiveLengthOptionsGoo)ComponentTestHelper.GetOutput(comp, 18);
      var output19 = (GH_String)ComponentTestHelper.GetOutput(comp, 19);
      var output20 = (GH_Colour)ComponentTestHelper.GetOutput(comp, 20);
      var output21 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 21);
      var output22 = (GH_String)ComponentTestHelper.GetOutput(comp, 22);

      Assert.Equal(0, output0.Value.PolyCurve.PointAtStart.X, 6);
      Assert.Equal(-1, output0.Value.PolyCurve.PointAtStart.Y, 6);
      Assert.Equal(0, output0.Value.PolyCurve.PointAtStart.Z, 6);
      Assert.Equal(7, output0.Value.PolyCurve.PointAtEnd.X, 6);
      Assert.Equal(3, output0.Value.PolyCurve.PointAtEnd.Y, 6);
      Assert.Equal(1, output0.Value.PolyCurve.PointAtEnd.Z, 6);
      Assert.Equal("STD CH(ft) 1 2 3 4", output0.Value.Section.ApiSection.Profile);
      Assert.Equal(1, output0.Value.ApiMember.Group);
      Assert.Equal(0, output1.Value);
      Assert.Equal(0, output2.Value.PointAtStart.X, 6);
      Assert.Equal(-1, output2.Value.PointAtStart.Y, 6);
      Assert.Equal(0, output2.Value.PointAtStart.Z, 6);
      Assert.Equal(7, output2.Value.PointAtEnd.X, 6);
      Assert.Equal(3, output2.Value.PointAtEnd.Y, 6);
      Assert.Equal(1, output2.Value.PointAtEnd.Z, 6);
      Assert.Equal("STD CH(ft) 1 2 3 4", output3.Value.ApiSection.Profile);
      Assert.Equal(1, output4.Value);
      Assert.Equal("Generic 1D", output5.Value);
      Assert.Equal("Beam", output6.Value);
      Assert.Equal(0, output7.Value.X1.Value, 6);
      Assert.Equal(0, output7.Value.X2.Value, 6);
      Assert.Equal(0, output7.Value.Y.Value, 6);
      Assert.Equal(0, output7.Value.Z.Value, 6);
      Assert.False(output8.Value.X);
      Assert.False(output8.Value.Y);
      Assert.False(output8.Value.Z);
      Assert.False(output8.Value.Xx);
      Assert.False(output8.Value.Yy);
      Assert.False(output8.Value.Zz);
      Assert.False(output9.Value.X);
      Assert.False(output9.Value.Y);
      Assert.False(output9.Value.Z);
      Assert.False(output9.Value.Xx);
      Assert.False(output9.Value.Yy);
      Assert.False(output9.Value.Zz);
      Assert.False(output10.Value);
      Assert.False(output12.Value);
      Assert.Equal(0, output14.Value, 6);
      Assert.Null(output15.Value);
      Assert.Equal(0.5, output16.Value, 6);
      Assert.True(output17.Value);
      Assert.Null(output18.Value.BucklingFactors.MomentAmplificationFactorStrongAxis);
      Assert.Null(output18.Value.BucklingFactors.MomentAmplificationFactorWeakAxis);
      Assert.Null(output18.Value.BucklingFactors.EquivalentUniformMomentFactor);
      Assert.Equal("", output19.Value);
      Assert.Equal(0, output20.Value.R);
      Assert.Equal(0, output20.Value.G);
      Assert.Equal(0, output20.Value.B);
      Assert.False(output21.Value);
      Assert.Equal("", output22.Value);
    }

    [Fact]
    public void CreateComponentTest2() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, 1, 1);
      ComponentTestHelper.SetInput(comp, new LineCurve(new Point3d(0, 0, 0), new Point3d(1, 2, 3)),
        2);
      ComponentTestHelper.SetInput(comp, "STD CH 10 20 30 40", 3);
      ComponentTestHelper.SetInput(comp, 7, 4);
      ComponentTestHelper.SetInput(comp, "Cantilever", 5);
      ComponentTestHelper.SetInput(comp, "Damper", 6);
      ComponentTestHelper.SetInput(comp, new GsaOffsetGoo(new GsaOffset(1, 2, 3, 4)), 7);
      ComponentTestHelper.SetInput(comp,
        new GsaBool6Goo(new GsaBool6(true, true, true, true, true, true)), 8);
      ComponentTestHelper.SetInput(comp,
        new GsaBool6Goo(new GsaBool6(true, true, true, true, true, true)), 9);
      ComponentTestHelper.SetInput(comp, true, 10);
      ComponentTestHelper.SetInput(comp, true, 11);
      ComponentTestHelper.SetInput(comp, Math.PI, 12);
      var node = new GsaNode(new Point3d(1, 2, 3)) {
        Id = 99,
      };
      ComponentTestHelper.SetInput(comp, new GsaNodeGoo(node), 13);
      ComponentTestHelper.SetInput(comp, 0.7, 14);
      ComponentTestHelper.SetInput(comp, false, 15);
      var leff = new GsaEffectiveLengthOptions(new GsaMember1d()) {
        BucklingFactors = new GsaBucklingFactors(1, 2, 3)
      };
      ComponentTestHelper.SetInput(comp,
        new GsaEffectiveLengthOptionsGoo(leff), 16);
      ComponentTestHelper.SetInput(comp, "name", 17);
      ComponentTestHelper.SetInput(comp, new GH_Colour(Color.White), 18);
      ComponentTestHelper.SetInput(comp, true, 19);

      var output0 = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp, 0);
      var output1 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      var output2 = (GH_Curve)ComponentTestHelper.GetOutput(comp, 2);
      var output3 = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 3);
      var output4 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 4);
      var output5 = (GH_String)ComponentTestHelper.GetOutput(comp, 5);
      var output6 = (GH_String)ComponentTestHelper.GetOutput(comp, 6);
      var output7 = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, 7);
      var output8 = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 8);
      var output9 = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 9);
      var output10 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 10);

      var output12 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 12);

      var output14 = (GH_Number)ComponentTestHelper.GetOutput(comp, 14);
      var output15 = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp, 15);
      var output16 = (GH_Number)ComponentTestHelper.GetOutput(comp, 16);
      var output17 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 17);
      var output18 = (GsaEffectiveLengthOptionsGoo)ComponentTestHelper.GetOutput(comp, 18);
      var output19 = (GH_String)ComponentTestHelper.GetOutput(comp, 19);
      var output20 = (GH_Colour)ComponentTestHelper.GetOutput(comp, 20);
      var output21 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 21);
      var output22 = (GH_String)ComponentTestHelper.GetOutput(comp, 22);

      Assert.Equal(0, output0.Value.PolyCurve.PointAtStart.X);
      Assert.Equal(0, output0.Value.PolyCurve.PointAtStart.Y);
      Assert.Equal(0, output0.Value.PolyCurve.PointAtStart.Z);
      Assert.Equal(1, output0.Value.PolyCurve.PointAtEnd.X);
      Assert.Equal(2, output0.Value.PolyCurve.PointAtEnd.Y);
      Assert.Equal(3, output0.Value.PolyCurve.PointAtEnd.Z);
      Assert.Equal("STD CH 10 20 30 40", output0.Value.Section.ApiSection.Profile);
      Assert.Equal(7, output0.Value.ApiMember.Group);
      Assert.Equal(1, output1.Value);
      Assert.Equal(0, output2.Value.PointAtStart.X);
      Assert.Equal(0, output2.Value.PointAtStart.Y);
      Assert.Equal(0, output2.Value.PointAtStart.Z);
      Assert.Equal(1, output2.Value.PointAtEnd.X);
      Assert.Equal(2, output2.Value.PointAtEnd.Y);
      Assert.Equal(3, output2.Value.PointAtEnd.Z);
      Assert.Equal("STD CH 10 20 30 40", output3.Value.ApiSection.Profile);
      Assert.Equal(7, output4.Value);
      Assert.Equal("Cantilever", output5.Value);
      Assert.Equal("Damper", output6.Value);
      Assert.Equal(1, output7.Value.X1.Value);
      Assert.Equal(2, output7.Value.X2.Value);
      Assert.Equal(3, output7.Value.Y.Value);
      Assert.Equal(4, output7.Value.Z.Value);
      Assert.True(output8.Value.X);
      Assert.True(output8.Value.Y);
      Assert.True(output8.Value.Z);
      Assert.True(output8.Value.Xx);
      Assert.True(output8.Value.Yy);
      Assert.True(output8.Value.Zz);
      Assert.True(output9.Value.X);
      Assert.True(output9.Value.Y);
      Assert.True(output9.Value.Z);
      Assert.True(output9.Value.Xx);
      Assert.True(output9.Value.Yy);
      Assert.True(output9.Value.Zz);
      Assert.True(output10.Value);
      Assert.True(output12.Value);
      Assert.Equal(Math.PI, output14.Value);
      Assert.Equal(1, output15.Value.Point.X);
      Assert.Equal(2, output15.Value.Point.Y);
      Assert.Equal(3, output15.Value.Point.Z);
      Assert.Equal(99, output15.Value.Id);
      Assert.Equal(0.7, output16.Value);
      Assert.False(output17.Value);
      Assert.Equal(1, output18.Value.BucklingFactors.MomentAmplificationFactorStrongAxis);
      Assert.Equal(2, output18.Value.BucklingFactors.MomentAmplificationFactorWeakAxis);
      Assert.Equal(3, output18.Value.BucklingFactors.EquivalentUniformMomentFactor);
      Assert.Equal("name", output19.Value);
      Assert.Equal(255, output20.Value.R);
      Assert.Equal(255, output20.Value.G);
      Assert.Equal(255, output20.Value.B);
      Assert.True(output21.Value);
      Assert.Equal("", output22.Value);
    }

    [Fact]
    public void TestElementBarInstabilityWarning() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, "Bar", 6);
      ComponentTestHelper.SetInput(comp, 0.0, 14);

      var output0 = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp, 0);
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));

      ComponentTestHelper.SetInput(comp, 1.0, 14);
      output0 = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp, 0);
      Assert.Single(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
    }

    [Fact]
    public void ChangeToSpringMember() {
      GH_OasysComponent comp = ComponentMother();
      var property = new AxialSpringProperty {
        Stiffness = 3.0
      };
      ComponentTestHelper.SetInput(comp, new GsaPropertyGoo(new GsaSpringProperty(property)), 3);
      ComponentTestHelper.SetInput(comp, "Spring", 6);

      var output0 = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp, 0);
      var output1 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      var output2 = (GH_Curve)ComponentTestHelper.GetOutput(comp, 2);
      var output3 = (GsaSpringPropertyGoo)ComponentTestHelper.GetOutput(comp, 3);
      var output4 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 4);
      var output5 = (GH_String)ComponentTestHelper.GetOutput(comp, 5);
      var output6 = (GH_String)ComponentTestHelper.GetOutput(comp, 6);
      var output7 = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, 7);
      var output8 = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 8);
      var output9 = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 9);
      var output10 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 10);
      var output12 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 12);
      var output14 = (GH_Number)ComponentTestHelper.GetOutput(comp, 14);
      var output15 = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp, 15);
      var output16 = (GH_Number)ComponentTestHelper.GetOutput(comp, 16);
      var output17 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 17);
      var output18 = (GsaEffectiveLengthOptionsGoo)ComponentTestHelper.GetOutput(comp, 18);
      var output19 = (GH_String)ComponentTestHelper.GetOutput(comp, 19);
      var output20 = (GH_Colour)ComponentTestHelper.GetOutput(comp, 20);
      var output21 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 21);
      var output22 = (GH_String)ComponentTestHelper.GetOutput(comp, 22);

      Assert.Equal(0, output0.Value.PolyCurve.PointAtStart.X, 6);
      Assert.Equal(-1, output0.Value.PolyCurve.PointAtStart.Y, 6);
      Assert.Equal(0, output0.Value.PolyCurve.PointAtStart.Z, 6);
      Assert.Equal(7, output0.Value.PolyCurve.PointAtEnd.X, 6);
      Assert.Equal(3, output0.Value.PolyCurve.PointAtEnd.Y, 6);
      Assert.Equal(1, output0.Value.PolyCurve.PointAtEnd.Z, 6);
      Assert.Equal(1, output0.Value.ApiMember.Group);
      Assert.Null(output0.Value.Section);
      Assert.Equal(0, output1.Value);
      Assert.Equal(0, output2.Value.PointAtStart.X, 6);
      Assert.Equal(-1, output2.Value.PointAtStart.Y, 6);
      Assert.Equal(0, output2.Value.PointAtStart.Z, 6);
      Assert.Equal(7, output2.Value.PointAtEnd.X, 6);
      Assert.Equal(3, output2.Value.PointAtEnd.Y, 6);
      Assert.Equal(1, output2.Value.PointAtEnd.Z, 6);
      Assert.NotNull(output3.Value);
      Assert.Equal(1, output4.Value);
      Assert.Equal("Generic 1D", output5.Value);
      Assert.Equal("Spring", output6.Value);
      Assert.Equal(0, output7.Value.X1.Value, 6);
      Assert.Equal(0, output7.Value.X2.Value, 6);
      Assert.Equal(0, output7.Value.Y.Value, 6);
      Assert.Equal(0, output7.Value.Z.Value, 6);
      Assert.False(output8.Value.X);
      Assert.False(output8.Value.Y);
      Assert.False(output8.Value.Z);
      Assert.False(output8.Value.Xx);
      Assert.False(output8.Value.Yy);
      Assert.False(output8.Value.Zz);
      Assert.False(output9.Value.X);
      Assert.False(output9.Value.Y);
      Assert.False(output9.Value.Z);
      Assert.False(output9.Value.Xx);
      Assert.False(output9.Value.Yy);
      Assert.False(output9.Value.Zz);
      Assert.False(output10.Value);
      Assert.False(output12.Value);
      Assert.Equal(0, output14.Value, 6);
      Assert.Null(output15.Value);
      Assert.Equal(0.5, output16.Value, 6);
      Assert.True(output17.Value);
      Assert.Null(output18.Value.BucklingFactors.MomentAmplificationFactorStrongAxis);
      Assert.Null(output18.Value.BucklingFactors.MomentAmplificationFactorWeakAxis);
      Assert.Null(output18.Value.BucklingFactors.EquivalentUniformMomentFactor);
      Assert.Equal("", output19.Value);
      Assert.Equal(0, output20.Value.R);
      Assert.Equal(0, output20.Value.G);
      Assert.Equal(0, output20.Value.B);
      Assert.False(output21.Value);
      Assert.Equal("", output22.Value);
    }

    [Fact]
    public void InvalidPropertyElement1DTypeCombination1() {
      GH_OasysComponent comp = ComponentMother();
      var property = new AxialSpringProperty {
        Stiffness = 3.0
      };
      ComponentTestHelper.SetInput(comp, new GsaPropertyGoo(new GsaSpringProperty(property)), 3);
      ComponentTestHelper.SetInput(comp, "Beam", 6);

      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Single(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
    }

    [Fact]
    public void InvalidPropertyElement1DTypeCombination2() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, "STD CH 10 20 30 40", 3);
      ComponentTestHelper.SetInput(comp, "Spring", 6);

      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Single(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void InvalidPropertyElement1DTypeCombination3() {
      var comp = new Edit1dMember();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, "Spring", 6);

      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    private static GH_OasysComponent ComponentMother() {
      var comp = new Edit1dMember();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, ComponentTestHelper.GetOutput(CreateMember1dTests.ComponentMother()), 0);

      return comp;
    }
  }

  public class EditMember1dTestsHelper {
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

    public GsaMember1dGoo GetMemberOutput() {
      var element = (GsaMember1dGoo)ComponentTestHelper.GetOutput(_component, 0);
      return element;
    }

    public GH_Integer GetIdOutput() {
      var id = (GH_Integer)ComponentTestHelper.GetOutput(_component, 1);
      return id;
    }

    public GH_Curve GetMemberCurveOutput() {
      var curve = (GH_Curve)ComponentTestHelper.GetOutput(_component, 2);
      return curve;
    }

    public GsaPropertyGoo GetSectionOutput() {
      var section = (GsaPropertyGoo)ComponentTestHelper.GetOutput(_component, 3);
      return section;
    }

    public GsaSpringPropertyGoo GetSpringPropertyOutput() {
      var property = (GsaSpringPropertyGoo)ComponentTestHelper.GetOutput(_component, 3);
      return property;
    }

    public GH_Integer GetMemberGroupOutput() {
      var group = (GH_Integer)ComponentTestHelper.GetOutput(_component, 4);
      return group;
    }

    public GH_String GetMemberTypeOutput() {
      var type = (GH_String)ComponentTestHelper.GetOutput(_component, 5);
      return type;
    }

    public GH_String GetElementTypeOutput() {
      var type = (GH_String)ComponentTestHelper.GetOutput(_component, 6);
      return type;
    }

    public GsaOffsetGoo GetOffsetOutput() {
      var offset = (GsaOffsetGoo)ComponentTestHelper.GetOutput(_component, 7);
      return offset;
    }

    public GsaBool6Goo GetStartReleaseOutput() {
      var startRelease = (GsaBool6Goo)ComponentTestHelper.GetOutput(_component, 8);
      return startRelease;
    }

    public GsaBool6Goo GetEndReleaseOutput() {
      var endRelease = (GsaBool6Goo)ComponentTestHelper.GetOutput(_component, 9);
      return endRelease;
    }

    public GH_Boolean GetAutomaticOffsetEnd1Output() {
      var autoBoolean = (GH_Boolean)ComponentTestHelper.GetOutput(_component, 10);
      return autoBoolean;
    }

    public GH_Number GetAutomaticOffsetX1Output() {
      var offsetX1 = (GH_Number)ComponentTestHelper.GetOutput(_component, 11);
      return offsetX1;
    }

    public GH_Boolean GetAutomaticOffsetEnd2Output() {
      var autoBoolean = (GH_Boolean)ComponentTestHelper.GetOutput(_component, 12);
      return autoBoolean;
    }

    public GH_Number GetAutomaticOffsetX2Output() {
      var offsetX1 = (GH_Number)ComponentTestHelper.GetOutput(_component, 13);
      return offsetX1;
    }

    public GH_Number GetAngleOutput() {
      var angle = (GH_Number)ComponentTestHelper.GetOutput(_component, 14);
      return angle;
    }

    public GsaNodeGoo GetOrientationOutput() {
      var orientation = (GsaNodeGoo)ComponentTestHelper.GetOutput(_component, 15);
      return orientation;
    }

    public GH_Number GetMeshSizeOutput() {
      var mesh = (GH_Number)ComponentTestHelper.GetOutput(_component, 16);
      return mesh;
    }

    public GH_Boolean GetIntersectorOutput() {
      var intersector = (GH_Boolean)ComponentTestHelper.GetOutput(_component, 17);
      return intersector;
    }

    public GsaEffectiveLengthOptionsGoo GetEffectiveLengthOutput() {
      var effectiveLength = (GsaEffectiveLengthOptionsGoo)ComponentTestHelper.GetOutput(_component, 18);
      return effectiveLength;
    }

    public GH_String GetNameOutput() {
      var name = (GH_String)ComponentTestHelper.GetOutput(_component, 19);
      return name;
    }

    public GH_Colour GetColorOutput() {
      var colour = (GH_Colour)ComponentTestHelper.GetOutput(_component, 20);
      return colour;
    }

    public GH_Boolean GetDummyOutput() {
      var dummy = (GH_Boolean)ComponentTestHelper.GetOutput(_component, 21);
      return dummy;
    }

    public GH_String GetTopologyOutput() {
      var topology = (GH_String)ComponentTestHelper.GetOutput(_component, 22);
      return topology;
    }
  }
}
