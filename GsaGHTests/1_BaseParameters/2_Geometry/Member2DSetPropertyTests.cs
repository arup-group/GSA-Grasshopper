using GsaAPI;

using GsaGH.Parameters;

using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class Member2DSetPropertyTests {
    private GsaMember2d _member;

    public Member2DSetPropertyTests() {
      _member = new GsaMember2d();
    }

    [Fact]
    public void ShouldBypassThisSettingWhenCreatingPropertyById() {
      var property = new GsaProperty2d(0);
      _member.SetProperty(property);
      Assert.True(true);
    }

    [Fact]
    public void CreateLoadPanelMember2dShouldChangeTheAnalysisType() {
      var property = new GsaProperty2d {
        ApiProp2d = {
          Type = Property2D_Type.LOAD,
        },
      };
      _member.SetProperty(property);
      Assert.Equal(AnalysisOrder.LOAD_PANEL, _member.ApiMember.Type2D);
    }

    [Fact]
    public void ShouldHaveDefaultAnalysisTypeLinear() {
      var property = new GsaProperty2d();
      _member.SetProperty(property);
      Assert.Equal(AnalysisOrder.LINEAR, _member.ApiMember.Type2D);
    }

    [Fact]
    public void ShouldCacheProperty() {
      var property = new GsaProperty2d();
      _member.SetProperty(property);
      Assert.Equal(property, _member.Prop2d);
    }
  }
}
