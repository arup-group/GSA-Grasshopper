using GsaAPI;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class CreateEffectiveLengthOptionsTests {

    [Fact]
    public void ChangeCalculationTypeDropdownTest() {
      var comp = new CreateEffectiveLengthOptions();
      comp.CreateAttributes();

      var output = (GsaEffectiveLengthOptionsGoo)ComponentTestHelper.GetOutput(comp);
      GsaEffectiveLengthOptions leff = output.Value;
      Assert.True(leff.EffectiveLength is EffectiveLengthFromEndRestraintAndGeometry);

      comp.SetSelected(0, 1);
      output = (GsaEffectiveLengthOptionsGoo)ComponentTestHelper.GetOutput(comp);
      leff = output.Value;
      Assert.True(leff.EffectiveLength is EffectiveLengthFromEndAndInternalRestraint);

      comp.SetSelected(0, 2);
      output = (GsaEffectiveLengthOptionsGoo)ComponentTestHelper.GetOutput(comp);
      leff = output.Value;
      Assert.True(leff.EffectiveLength is EffectiveLengthFromUserSpecifiedValue);

      comp.SetSelected(0, 0);
      output = (GsaEffectiveLengthOptionsGoo)ComponentTestHelper.GetOutput(comp);
      leff = output.Value;
      Assert.True(leff.EffectiveLength is EffectiveLengthFromEndRestraintAndGeometry);
    }

    [Fact]
    public void ChangeLoadReferenceDropdownTest() {
      var comp = new CreateEffectiveLengthOptions();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, 0.15, 2);

      comp.SetSelected(1, 0);
      var output = (GsaEffectiveLengthOptionsGoo)ComponentTestHelper.GetOutput(comp);
      LoadReference refr = output.Value.EffectiveLength.DestablisingLoadPositionRelativeTo;
      Assert.Equal(LoadReference.ShearCentre, refr);
      Assert.Equal(0.15, output.Value.EffectiveLength.DestablisingLoad);

      comp.SetSelected(1, 1);
      output = (GsaEffectiveLengthOptionsGoo)ComponentTestHelper.GetOutput(comp);
      refr = output.Value.EffectiveLength.DestablisingLoadPositionRelativeTo;
      Assert.Equal(LoadReference.TopFlange, refr);
      Assert.Equal(0.15, output.Value.EffectiveLength.DestablisingLoad);

      comp.SetSelected(1, 2);
      output = (GsaEffectiveLengthOptionsGoo)ComponentTestHelper.GetOutput(comp);
      refr = output.Value.EffectiveLength.DestablisingLoadPositionRelativeTo;
      Assert.Equal(LoadReference.BottomFlange, refr);
      Assert.Equal(0.15, output.Value.EffectiveLength.DestablisingLoad);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void EndReleaseInputTests(int index) {
      var comp = new CreateEffectiveLengthOptions();
      comp.CreateAttributes();
      comp.SetSelected(0, index);
      ComponentTestHelper.SetInput(comp, "Pinned", 0);
      ComponentTestHelper.SetInput(comp, "Fixed", 1);

      var output = (GsaEffectiveLengthOptionsGoo)ComponentTestHelper.GetOutput(comp);
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
      var comp = new CreateEffectiveLengthOptions();
      comp.CreateAttributes();
      comp.SetSelected(0, index);
      int max = comp.Params.Input.Count - 1;
      ComponentTestHelper.SetInput(comp, 0.5, max - 2);
      ComponentTestHelper.SetInput(comp, 1.2, max - 1);
      ComponentTestHelper.SetInput(comp, 9.9, max);

      var output = (GsaEffectiveLengthOptionsGoo)ComponentTestHelper.GetOutput(comp);
      GsaBucklingFactors bf = output.Value.BucklingFactors;
      Assert.Equal(0.5, bf.MomentAmplificationFactorStrongAxis);
      Assert.Equal(1.2, bf.MomentAmplificationFactorWeakAxis);
      Assert.Equal(9.9, bf.EquivalentUniformMomentFactor);
    }

    [Fact]
    public void UserSpecifiedLengthAsNumberInputTest() {
      var comp = new CreateEffectiveLengthOptions();
      comp.CreateAttributes();
      comp.SetSelected(0, 2);
      ComponentTestHelper.SetInput(comp, 0.1, 0);
      ComponentTestHelper.SetInput(comp, 0.2, 1);
      ComponentTestHelper.SetInput(comp, 1.5, 2);

      var output = (GsaEffectiveLengthOptionsGoo)ComponentTestHelper.GetOutput(comp);
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
      var comp = new CreateEffectiveLengthOptions();
      comp.CreateAttributes();
      comp.SetSelected(0, 2);
      ComponentTestHelper.SetInput(comp, -0.1, 0);
      ComponentTestHelper.SetInput(comp, -0.2, 1);
      ComponentTestHelper.SetInput(comp, -1.5, 2);

      var output = (GsaEffectiveLengthOptionsGoo)ComponentTestHelper.GetOutput(comp);
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
      var comp = new CreateEffectiveLengthOptions();
      comp.CreateAttributes();
      comp.SetSelected(0, 1);
      ComponentTestHelper.SetInput(comp, contin, 2);
      ComponentTestHelper.SetInput(comp, interm, 3);

      var output = (GsaEffectiveLengthOptionsGoo)ComponentTestHelper.GetOutput(comp);
      var intermediate = (EffectiveLengthFromEndAndInternalRestraint)output.Value.EffectiveLength;
      Assert.Equal(expectedContin, intermediate.RestraintAlongMember.ToString());
      Assert.Equal(expectedInterm, intermediate.RestraintAtBracedPoints.ToString());
    }

    [Theory]
    [InlineData("asd", 3)]
    [InlineData("asd", 4)]
    public void IntermediateRestraintStringInputErrorTest(string input, int id) {
      var comp = new CreateEffectiveLengthOptions();
      comp.CreateAttributes();
      comp.SetSelected(0, 1);
      ComponentTestHelper.SetInput(comp, input, id);
      comp.Params.Output[0].CollectData();
      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }

    [Theory]
    [InlineData("F1L F2L TR MAJV MINV", "Pinned")]
    [InlineData("F1L", "TopFlangeLateral")]
    [InlineData("", "Free")]
    [InlineData("F1LW F2W MAJVW MINV", "F1LW F2W MAJVW MINV")]
    [InlineData("F1LP F2P MAJVP MINV", "F1LP F2P MAJVP MINV")]
    [InlineData("F1W", "F1W")]
    [InlineData("F1P", "F1P")]
    [InlineData("F2W", "F2W")]
    [InlineData("F2P", "F2P")]
    [InlineData("TR", "TR")]
    [InlineData("MAJV", "MAJV")]
    [InlineData("MINV", "MINV")]
    public void MemberEndRestraintCreateFromStringsTests(string s, string expected) {
      MemberEndRestraint restraint = MemberEndRestraintFactory.CreateFromStrings(s);
      string value = MemberEndRestraintFactory.MemberEndRestraintToString(restraint);
      Assert.Equal(expected, value);
    }
  }
}
