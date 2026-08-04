using Grasshopper.Kernel.Types;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;

using Xunit;

namespace GsaGHTests.Helpers.GH {
  [Collection("GrasshopperFixture collection")]
  public class ReplaceParamTests {

    [Fact]
    public void UnsupportedValueShouldPresentType() {
      var typeHint = ReplaceParam.UnsupportedValue(new GH_ObjectWrapper() { Value = "unsupported type" });
      Assert.Equal("System.String", typeHint);
    }

    [Fact]
    public void UnsupportedValueShouldCleanParameterNamespace() {
      var typeHint
        = ReplaceParam.UnsupportedValue(new GH_ObjectWrapper() {
          Value = new GsaGH.Parameters.GsaAnalysisTaskParameter()
        });
      Assert.Equal("GsaAnalysisTaskParameter", typeHint);
    }

    [Fact]
    public void UnsupportedValueShouldCleanGooSuffix() {
      var typeHint = ReplaceParam.UnsupportedValue(new GH_ObjectWrapper() {
        Value = new GsaGH.Parameters.GsaAnalysisTaskGoo(new GsaAnalysisTask())
      });
      Assert.Equal("GsaAnalysisTask", typeHint);
    }
  }
}
