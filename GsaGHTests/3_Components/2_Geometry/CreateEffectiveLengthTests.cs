using GsaAPI;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class CreateEffectiveLengthTests {

    [Fact]
    public void ChangeCalculationTypeDropdownTest() {
      var comp = new CreateEffectiveLength();
      comp.CreateAttributes();
      
      comp.SetSelected(0, 0);
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
  }
}
