using Grasshopper.Kernel.Types;
using GsaGH.Components;
using GsaGHTests.Helpers;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysUnits.Units;
using Rhino;
using System;
using Xunit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class EditProfileTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new EditProfile();
      comp.CreateAttributes();
      return comp;
    }

    [Theory]
    [InlineData("STD A(cm) 25 30 4.5 6.7", 33.33, false, false, 
      "STD A(cm) 25 30 4.5 6.7 [R(33.33)]")]
    [InlineData("STD CH 100 200 10 15", 0, true, true,
      "STD CH 100 200 10 15 [R(0)HV]")]
    public void CreateComponent(
      string profile, double rotation, bool horizontal, bool vertical, string expectedString) {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, profile, 0);
      rotation = RhinoMath.ToRadians(rotation);
      ComponentTestHelper.SetInput(comp, rotation, 1);
      ComponentTestHelper.SetInput(comp, horizontal, 2);
      ComponentTestHelper.SetInput(comp, vertical, 3);

      var profileOut = (GH_String)ComponentTestHelper.GetOutput(comp, 0);

      Assert.Equal(expectedString, profileOut.Value);
    }
  }
}
