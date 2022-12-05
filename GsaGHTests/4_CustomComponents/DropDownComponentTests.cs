using GsaGH.Components;
using GsaGHTests.Components.Properties;
using GsaGHTests.Helpers;
using OasysGH.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GsaGHTests.CustomComponent
{
  [Collection("GrasshopperFixture collection")]
  public class DropDownComponentTests
  {
    [Theory]
    [InlineData(typeof(CreateModel), true)]
    //[InlineData(typeof(GetGeometry))] GetGeometry is GH_OasysTaskCapableComponent
    [InlineData(typeof(GetLoads))]
    [InlineData(typeof(CreateCustomMaterial))]
    [InlineData(typeof(CreateMaterial))]
    [InlineData(typeof(CreateOffset))]
    [InlineData(typeof(CreateProfile), true)]
    [InlineData(typeof(CreateProp2d))]
    [InlineData(typeof(CreateSectionModifier))]
    [InlineData(typeof(ElemFromMem))]
    [InlineData(typeof(SectionAlignment))]
    [InlineData(typeof(CreateBeamLoads))]
    [InlineData(typeof(CreateFaceLoads))]
    [InlineData(typeof(CreateGridAreaLoad))]
    [InlineData(typeof(CreateGridLineLoad))]
    [InlineData(typeof(CreateGridPlane))]
    [InlineData(typeof(CreateGridPointLoad))]
    [InlineData(typeof(CreateGridSurface))]
    [InlineData(typeof(CreateNodeLoad))]
    [InlineData(typeof(LoadProp))]
    [InlineData(typeof(GH_Analyse), true)]
    [InlineData(typeof(CreateAnalysisTask))]
    [InlineData(typeof(BeamDisplacement), true)]
    [InlineData(typeof(BeamForces))]
    [InlineData(typeof(BeamStrainEnergy), true)]
    [InlineData(typeof(Elem1dContourResults), true)]
    [InlineData(typeof(Elem2dContourResults), true)]
    [InlineData(typeof(Elem2dDisplacement))]
    [InlineData(typeof(Elem2dForces))]
    [InlineData(typeof(Elem2dStress))]
    [InlineData(typeof(Elem3dDisplacement))]
    [InlineData(typeof(Elem3dStress))]
    [InlineData(typeof(GlobalPerformanceResults))]
    [InlineData(typeof(NodeContourResults), true)]
    [InlineData(typeof(NodeDisplacement))]
    [InlineData(typeof(ReactionForce))]
    //[InlineData(typeof(SelectResults))] SelectResults depends on input model to populate dropdown
    [InlineData(typeof(SpringReactionForce))]
    [InlineData(typeof(TotalLoadsAndReactionResults))]
    public void DropDownComponentTest(Type t, bool ignoreSpacerDescriptionCount = false)
    {
      // ignore space description count for components that has more UI features than just the dropdown
      GH_OasysDropDownComponent comp = (GH_OasysDropDownComponent)Activator.CreateInstance(t);
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp, ignoreSpacerDescriptionCount);
    }
  }
}
