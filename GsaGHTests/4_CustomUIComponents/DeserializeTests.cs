using System;
using GsaGH.Components;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.CustomComponent {
  [Collection("GrasshopperFixture collection")]
  public class DeserializeTests {

    [Theory]
    [InlineData(typeof(OpenModel))]
    [InlineData(typeof(CreateBool6))]
    [InlineData(typeof(EditOffset))]
    [InlineData(typeof(EditProp2d))]
    [InlineData(typeof(EditSectionModifier))]
    [InlineData(typeof(GetSectionDimensions))]
    [InlineData(typeof(GetSectionProperties))]
    [InlineData(typeof(GetMaterialProperties))]
    [InlineData(typeof(CreateMember1d))]
    [InlineData(typeof(CreateSupport))]
    [InlineData(typeof(EditMember1d))]
    [InlineData(typeof(EditNode))]
    [InlineData(typeof(GridPlaneSurfaceProperties))]
    public void DeSerializeComponentTest(Type t) {
      var comp = (GH_OasysComponent)Activator.CreateInstance(t);
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
