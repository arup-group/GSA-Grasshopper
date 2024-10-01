using System;
using System.Linq;

using Grasshopper.Kernel;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helper;
using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaResultParameterTests {
    private static IGH_Param ModelParam => _modelParam ?? (_modelParam = OpenModelParam());
    private static IGH_Param _modelParam = null;

    [Theory]
    [InlineData(typeof(AssemblyDisplacements))]
    [InlineData(typeof(AssemblyDrifts))]
    [InlineData(typeof(AssemblyDriftIndices))]
    [InlineData(typeof(AssemblyForcesAndMoments))]
    [InlineData(typeof(BeamDerivedStresses))]
    [InlineData(typeof(BeamDisplacements))]
    [InlineData(typeof(BeamForcesAndMoments))]
    [InlineData(typeof(BeamStrainEnergyDensity))]
    [InlineData(typeof(BeamStresses))]
    [InlineData(typeof(Element2dDisplacements))]
    [InlineData(typeof(Element2dForcesAndMoments))]
    [InlineData(typeof(Element2dStresses))]
    [InlineData(typeof(Element3dDisplacements))]
    [InlineData(typeof(Element3dStresses))]
    [InlineData(typeof(FootfallResults))]
    [InlineData(typeof(GlobalPerformanceResults))]
    [InlineData(typeof(Member1dDisplacements))]
    [InlineData(typeof(Member1dForcesAndMoments))]
    [InlineData(typeof(NodeDisplacements))]
    [InlineData(typeof(ReactionForces))]
    [InlineData(typeof(SpringReactionForces))]
    [InlineData(typeof(TotalLoadsAndReactions))]
    [InlineData(typeof(AssemblyResultDiagrams))]
    [InlineData(typeof(AssemblyResults))]
    [InlineData(typeof(Contour1dResults))]
    [InlineData(typeof(Contour2dResults))]
    [InlineData(typeof(Contour3dResults))]
    [InlineData(typeof(ContourNodeResults))]
    [InlineData(typeof(ReactionForceDiagrams))]
    [InlineData(typeof(ResultDiagrams))]
    [InlineData(typeof(PreviewDeformed3dSections))]
    [InlineData(typeof(SteelUtilisations))]
    public void GsaResultParameterPreferredCastFromModelParameterTest(Type componentType) {
      var doc = new GH_Document();
      var open = new OpenModel();
      open.CreateAttributes();
      open.Attributes.Pivot = new System.Drawing.PointF(0, 0);
      string file = GsaFile.SteelDesignComplex;
      ComponentTestHelper.SetInput(open, file);
      IGH_Param modelParam = open.Params.Output[0];
      modelParam.CollectData();
      doc.AddObject(open, true);
      Assert.Equal(1, doc.ObjectCount);

      var comp = (GH_Component)Activator.CreateInstance(componentType);
      comp.CreateAttributes();
      comp.Attributes.Pivot = new System.Drawing.PointF(200, 400);
      doc.AddObject(comp, true);
      Assert.Equal(2, doc.ObjectCount);
      ComponentTestHelper.SetInput(comp, ModelParam.VolatileData.AllData(false).First());
      comp.Params.Output[0].CollectData();
      Assert.Equal(3, doc.ObjectCount);
    }

    [Theory]
    [InlineData(typeof(BeamDerivedStresses))]
    [InlineData(typeof(BeamDisplacements))]
    [InlineData(typeof(BeamForcesAndMoments))]
    [InlineData(typeof(BeamStrainEnergyDensity))]
    [InlineData(typeof(BeamStresses))]
    [InlineData(typeof(Element2dDisplacements))]
    [InlineData(typeof(Element2dForcesAndMoments))]
    [InlineData(typeof(Element2dStresses))]
    [InlineData(typeof(Element3dDisplacements))]
    [InlineData(typeof(Element3dStresses))]
    [InlineData(typeof(FootfallResults))]
    [InlineData(typeof(GlobalPerformanceResults))]
    [InlineData(typeof(Member1dDisplacements))]
    [InlineData(typeof(Member1dForcesAndMoments))]
    [InlineData(typeof(NodeDisplacements))]
    [InlineData(typeof(ReactionForces))]
    [InlineData(typeof(SpringReactionForces))]
    [InlineData(typeof(TotalLoadsAndReactions))]
    [InlineData(typeof(Contour1dResults))]
    [InlineData(typeof(Contour2dResults))]
    [InlineData(typeof(Contour3dResults))]
    [InlineData(typeof(ContourNodeResults))]
    [InlineData(typeof(ReactionForceDiagrams))]
    [InlineData(typeof(ResultDiagrams))]
    [InlineData(typeof(PreviewDeformed3dSections))]
    [InlineData(typeof(SteelUtilisations))]
    public void GsaResultParameterNullTest(Type componentType) {
      var comp = (GH_Component)Activator.CreateInstance(componentType);
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, false);
      comp.Params.Output[0].CollectData();
      Assert.Equal(GH_RuntimeMessageLevel.Error, comp.RuntimeMessageLevel);
    }

    [Fact]
    public void GsaResultParameterPreferredCastErrorTest() {
      var param = new GsaResultParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, ModelParam);
      Assert.False(param.VolatileData.AllData(false).First().IsValid);
      Assert.Single(param.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    private static IGH_Param OpenModelParam() {
      var open = new OpenModel();
      open.CreateAttributes();
      string file = GsaFile.SteelDesignComplex;
      ComponentTestHelper.SetInput(open, file);
      IGH_Param modelParam = open.Params.Output[0];
      modelParam.CollectData();
      return modelParam;
    }
  }
}
