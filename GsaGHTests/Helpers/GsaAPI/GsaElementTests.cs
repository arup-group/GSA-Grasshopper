using GsaAPI;

using Xunit;

namespace GsaGHTests.Helpers.GsaAPITests {
  [Collection("GrasshopperFixture collection")]
  public class GsaElementTests {
    [Fact]
    public void GroupsShouldDefaultToOne() {
      var element = new Element();
      var gsaElement = new GSAElement(element);
      Assert.Equal(1, gsaElement.Group);
    }

    [Fact]
    public void NonDefaultGroupsShouldNotBeModified() {
      var element = new Element() {
        Group = 2,
      };
      var gsaElement = new GSAElement(element);
      Assert.Equal(2, gsaElement.Group);
    }
  }
}
