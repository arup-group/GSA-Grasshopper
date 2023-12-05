using GsaAPI;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class CreateEffectiveLengthTests {

    [Fact]
    public void ChangeCalculationTypeDropdownTest() {
      var comp = new CreateEffectiveLength();
      comp.CreateAttributes();
      
      var output = (GsaEffectiveLengthGoo)ComponentTestHelper.GetOutput(comp);
      GsaEffectiveLength leff = output.Value;
      Assert.True(leff.EffectiveLength is EffectiveLengthFromEndRestraintAndGeometry);

      comp.SetSelected(0, 1);
      output = (GsaEffectiveLengthGoo)ComponentTestHelper.GetOutput(comp);
      leff = output.Value;
      Assert.True(leff.EffectiveLength is EffectiveLengthFromEndAndInternalRestraint);

      comp.SetSelected(0, 2);
      output = (GsaEffectiveLengthGoo)ComponentTestHelper.GetOutput(comp);
      leff = output.Value;
      Assert.True(leff.EffectiveLength is EffectiveLengthFromUserSpecifiedValue);

      comp.SetSelected(0, 0);
      output = (GsaEffectiveLengthGoo)ComponentTestHelper.GetOutput(comp);
      leff = output.Value;
      Assert.True(leff.EffectiveLength is EffectiveLengthFromEndRestraintAndGeometry);
    }

    [Fact]
    public void ChangeLoadReferenceDropdownTest() {
      var comp = new CreateEffectiveLength();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, 0.15, 0);

      comp.SetSelected(1, 0);
      var output = (GsaEffectiveLengthGoo)ComponentTestHelper.GetOutput(comp);
      LoadReference refr = output.Value.EffectiveLength.DestablisingLoadPositionRelativeTo;
      Assert.Equal(LoadReference.ShearCentre, refr);
      Assert.Equal(0.15, output.Value.EffectiveLength.DestablisingLoad);

      comp.SetSelected(1, 1);
      output = (GsaEffectiveLengthGoo)ComponentTestHelper.GetOutput(comp);
      refr = output.Value.EffectiveLength.DestablisingLoadPositionRelativeTo;
      Assert.Equal(LoadReference.TopFlange, refr);
      Assert.Equal(0.15, output.Value.EffectiveLength.DestablisingLoad);

      comp.SetSelected(1, 2);
      output = (GsaEffectiveLengthGoo)ComponentTestHelper.GetOutput(comp);
      refr = output.Value.EffectiveLength.DestablisingLoadPositionRelativeTo;
      Assert.Equal(LoadReference.BottomFlange, refr);
      Assert.Equal(0.15, output.Value.EffectiveLength.DestablisingLoad);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void EndReleaseInputTests(int index) {
      var comp = new CreateEffectiveLength();
      comp.CreateAttributes();
      comp.SetSelected(0, index);
      ComponentTestHelper.SetInput(comp, "Pinned", 1);
      ComponentTestHelper.SetInput(comp, "Fixed", 2);

      var output = (GsaEffectiveLengthGoo)ComponentTestHelper.GetOutput(comp);
      var leff = (EffectiveLengthFromEndRestraintAndGeometry)output.Value.EffectiveLength;
      var expected = new EffectiveLengthFromEndRestraintAndGeometry {
        End1 = new MemberEndRestraint(StandardRestraint.Pinned),
        End2 = new MemberEndRestraint(StandardRestraint.Fixed),
      };
      Assert.Equal(expected.End1.ToString(), leff.End1.ToString());
      Assert.Equal(expected.End2.ToString(), leff.End2.ToString());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void BucklingFactorsTests(int index) {
      var comp = new CreateEffectiveLength();
      comp.CreateAttributes();
      comp.SetSelected(0, index);
      int max = comp.Params.Input.Count - 1;
      ComponentTestHelper.SetInput(comp, 0.5, max - 2);
      ComponentTestHelper.SetInput(comp, 1.2, max - 1);
      ComponentTestHelper.SetInput(comp, 9.9, max);

      var output = (GsaEffectiveLengthGoo)ComponentTestHelper.GetOutput(comp);
      GsaBucklingFactors bf = output.Value.BucklingFactors;
      Assert.Equal(0.5, bf.MomentAmplificationFactorStrongAxis);
      Assert.Equal(1.2, bf.MomentAmplificationFactorWeakAxis);
      Assert.Equal(9.9, bf.EquivalentUniformMomentFactor);
    }

    [Fact]
    public void UserSpecifiedLengthAsNumberInputTest() {
      var comp = new CreateEffectiveLength();
      comp.CreateAttributes();
      comp.SetSelected(0, 2);
      ComponentTestHelper.SetInput(comp, 0.1, 1);
      ComponentTestHelper.SetInput(comp, 0.2, 2);
      ComponentTestHelper.SetInput(comp, 1.5, 3);

      var output = (GsaEffectiveLengthGoo)ComponentTestHelper.GetOutput(comp);
      var specific = (EffectiveLengthFromUserSpecifiedValue)output.Value.EffectiveLength;
      Assert.Equal(0.1, specific.EffectiveLengthAboutY.Value);
      Assert.Equal(EffectiveLengthOptionType.Absolute, specific.EffectiveLengthAboutY.Option);
      Assert.Equal(0.2, specific.EffectiveLengthAboutZ.Value);
      Assert.Equal(EffectiveLengthOptionType.Absolute, specific.EffectiveLengthAboutZ.Option);
      Assert.Equal(1.5, specific.EffectiveLengthLaterialTorsional.Value);
      Assert.Equal(EffectiveLengthOptionType.Absolute, specific.EffectiveLengthLaterialTorsional.Option);
    }

    [Fact]
    public void UserSpecifiedLengthAsPercentInputTest() {
      var comp = new CreateEffectiveLength();
      comp.CreateAttributes();
      comp.SetSelected(0, 2);
      ComponentTestHelper.SetInput(comp, -0.1, 1);
      ComponentTestHelper.SetInput(comp, -0.2, 2);
      ComponentTestHelper.SetInput(comp, -1.5, 3);

      var output = (GsaEffectiveLengthGoo)ComponentTestHelper.GetOutput(comp);
      var specific = (EffectiveLengthFromUserSpecifiedValue)output.Value.EffectiveLength;
      Assert.Equal(0.1, specific.EffectiveLengthAboutY.Value);
      Assert.Equal(EffectiveLengthOptionType.Relative, specific.EffectiveLengthAboutY.Option);
      Assert.Equal(0.2, specific.EffectiveLengthAboutZ.Value);
      Assert.Equal(EffectiveLengthOptionType.Relative, specific.EffectiveLengthAboutZ.Option);
      Assert.Equal(1.5, specific.EffectiveLengthLaterialTorsional.Value);
      Assert.Equal(EffectiveLengthOptionType.Relative, specific.EffectiveLengthLaterialTorsional.Option);
    }

    [Theory]
    [InlineData("2", "3", "Pinned", "TopAndBottomFlangeLateral")]
    [InlineData("0", "0", "Free", "Free")]
    [InlineData("free", "free", "Free", "Free")]
    [InlineData("2s", "3s", "Pinned", "TopAndBottomFlangeLateral")]
    [InlineData("pin", "top and bottom", "Pinned", "TopAndBottomFlangeLateral")]
    [InlineData("1", "top", "TopFlangeLateral", "TopFlangeLateral")]
    [InlineData("top", "bot", "TopFlangeLateral", "BottomFlangeLateral")]
    [InlineData("top", "2", "TopFlangeLateral", "BottomFlangeLateral")]
    public void IntermediateRestraintStringInputTest(
        string contin, string interm, string expectedContin, string expectedInterm) {
      var comp = new CreateEffectiveLength();
      comp.CreateAttributes();
      comp.SetSelected(0, 1);
      ComponentTestHelper.SetInput(comp, contin, 3);
      ComponentTestHelper.SetInput(comp, interm, 4);

      var output = (GsaEffectiveLengthGoo)ComponentTestHelper.GetOutput(comp);
      var intermediate = (EffectiveLengthFromEndAndInternalRestraint)output.Value.EffectiveLength;
      Assert.Equal(expectedContin, intermediate.RestraintAlongMember.ToString());
      Assert.Equal(expectedInterm, intermediate.RestraintAtBracedPoints.ToString());
    }

    [Theory]
    [InlineData("asd", 3)]
    [InlineData("asd", 4)]
    public void IntermediateRestraintStringInputErrorTest(string input, int id) {
      var comp = new CreateEffectiveLength();
      comp.CreateAttributes();
      comp.SetSelected(0, 1);
      ComponentTestHelper.SetInput(comp, input, id);
      comp.Params.Output[0].CollectData();
      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }
  }
}
