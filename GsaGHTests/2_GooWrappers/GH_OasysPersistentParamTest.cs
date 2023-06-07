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
    [InlineData(typeof(GsaModelParameter))]
    // 1_Properties
    [InlineData(typeof(GsaBool6Parameter))]
    [InlineData(typeof(GsaBucklingLengthFactorsParameter))]
    [InlineData(typeof(GsaMaterialParameter))]
    [InlineData(typeof(GsaOffsetParameter))]
    [InlineData(typeof(GsaProp2dParameter))]
    [InlineData(typeof(GsaProp3dParameter))]
    [InlineData(typeof(GsaSectionModifierParameter))]
    [InlineData(typeof(GsaSectionParameter))]
    // 2_Geometry
    [InlineData(typeof(GsaElement1dParameter))]
    [InlineData(typeof(GsaElement2dParameter))]
    [InlineData(typeof(GsaElement3dParameter))]
    [InlineData(typeof(GsaMember1dParameter))]
    [InlineData(typeof(GsaMember2dParameter))]
    [InlineData(typeof(GsaMember3dParameter))]
    [InlineData(typeof(GsaNodeParameter))]
    // 3_Loads
    [InlineData(typeof(GsaLoadParameter))]
    [InlineData(typeof(GsaGridPlaneSurfaceParameter))]
    // 4_Analysis
    [InlineData(typeof(GsaAnalysisCaseParameter))]
    [InlineData(typeof(GsaAnalysisTaskParameter))]
    [InlineData(typeof(GsaCombinationCaseParameter))]
    // 5_Results
    [InlineData(typeof(GsaResultParameter))]
    public void GH_OasysComponentTest(Type t) {
      var param = (IGH_Param)Activator.CreateInstance(t);
      Assert.NotNull(param.Icon_24x24);
      Assert.NotEqual(GH_Exposure.hidden, param.Exposure);
      Assert.NotEqual(new Guid(), param.ComponentGuid);
      Assert.NotNull(param.InstanceDescription);
      Assert.NotNull(param.TypeName);
    }
  }
}
