using System;

using Grasshopper.Kernel;

using GsaGH.Parameters;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GH_OasysPersistentParamTest {

    [Theory]
    // 0_Model
    [InlineData(typeof(GsaListParameter))]
    [InlineData(typeof(GsaElementListParameter), true)]
    [InlineData(typeof(GsaMemberListParameter), true)]
    [InlineData(typeof(GsaNodeListParameter), true)]
    [InlineData(typeof(GsaGridLineParameter))]
    [InlineData(typeof(GsaModelParameter))]
    // 1_Properties
    [InlineData(typeof(GsaBool6Parameter))]
    [InlineData(typeof(GsaMaterialParameter))]
    [InlineData(typeof(GsaOffsetParameter))]
    [InlineData(typeof(GsaPropertyParameter), true)]
    [InlineData(typeof(GsaProperty2dParameter))]
    [InlineData(typeof(GsaProperty3dParameter))]
    [InlineData(typeof(GsaSectionModifierParameter))]
    [InlineData(typeof(GsaSectionParameter))]
    [InlineData(typeof(GsaSpringPropertyParameter))]
    // 2_Geometry
    [InlineData(typeof(GsaElement1dParameter))]
    [InlineData(typeof(GsaElement2dParameter))]
    [InlineData(typeof(GsaElement3dParameter))]
    [InlineData(typeof(GsaMember1dParameter))]
    [InlineData(typeof(GsaMember2dParameter))]
    [InlineData(typeof(GsaMember3dParameter))]
    [InlineData(typeof(GsaNodeParameter))]
    [InlineData(typeof(GsaEffectiveLengthOptionsParameter))]
    [InlineData(typeof(GsaAssemblyParameter))]
    // 3_Loads
    [InlineData(typeof(GsaLoadParameter))]
    [InlineData(typeof(GsaLoadCaseParameter))]
    [InlineData(typeof(GsaGridPlaneSurfaceParameter))]
    // 4_Analysis
    [InlineData(typeof(GsaAnalysisCaseParameter))]
    [InlineData(typeof(GsaAnalysisTaskParameter))]
    [InlineData(typeof(GsaCombinationCaseParameter))]
    // 5_Results
    [InlineData(typeof(GsaResultParameter))]
    // 6_Display
    [InlineData(typeof(GsaAnnotationParameter), true)]
    [InlineData(typeof(GsaDiagramParameter), true)]
    public void GH_OasysComponentTest(Type t, bool isHidden = false) {
      var param = (IGH_Param)Activator.CreateInstance(t);
      Assert.NotNull(param.Icon_24x24);
      if (isHidden) {
        Assert.Equal(GH_Exposure.hidden, param.Exposure);
      } else {
        Assert.NotEqual(GH_Exposure.hidden, param.Exposure);
      }

      Assert.NotEqual(new Guid(), param.ComponentGuid);
      Assert.NotNull(param.InstanceDescription);
      Assert.NotNull(param.TypeName);
    }
  }
}
