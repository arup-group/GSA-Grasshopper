using GsaGH.Components;

using Xunit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class GetSpringPropertyTests {

    [Fact]
    public void UpdateLengthTest() {
      var comp = new GetSpringProperty();
      comp.UpdateLength("mm");
      Assert.Equal("kN/m, N·m/rad, mm", comp.Message);
    }

    [Fact]
    public void UpdateStiffnessTest() {
      var comp = new GetSpringProperty();
      comp.UpdateStiffness("N/m");
      Assert.Equal("N/m, N·m/rad, cm", comp.Message);
    }

    [Fact]
    public void UpdateRotationalStiffnessTest() {
      var comp = new GetSpringProperty();
      comp.UpdateRotationalStiffness("kN·m/rad");
      Assert.Equal("kN/m, kN·m/rad, cm", comp.Message);
    }
  }
}
