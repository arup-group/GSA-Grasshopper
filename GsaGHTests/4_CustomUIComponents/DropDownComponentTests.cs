using System;
using System.IO;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;

using GsaGH.Components;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.CustomComponent {
  [Collection("GrasshopperFixture collection")]
  public class DropDownComponentTests {

    [Theory]
    [InlineData(typeof(CreateList))]
    [InlineData(typeof(CreateModel), true)]
    [InlineData(typeof(GetModelLoads))]
    [InlineData(typeof(ListInfo))]
    [InlineData(typeof(PropertyQuantities))]
    [InlineData(typeof(MaterialQuantities))]
    // Properties
    [InlineData(typeof(CreateCustomMaterial))]
    [InlineData(typeof(CreateMaterial))]
    [InlineData(typeof(Create2dMember))]
    [InlineData(typeof(CreateOffset))]
    [InlineData(typeof(Create2dProperty))]
    [InlineData(typeof(Create2dPropertyModifier))]
    [InlineData(typeof(CreateSection))]
    [InlineData(typeof(CreateSectionModifier))]
    [InlineData(typeof(CreateSpringProperty))]
    [InlineData(typeof(CreateProfile), true)]
    // Geometry
    [InlineData(typeof(Create2dElementsFromBrep))]
    [InlineData(typeof(CreateElementsFromMembers))]
    [InlineData(typeof(SectionAlignment))]
    [InlineData(typeof(CreateEffectiveLengthOptions))]
    [InlineData(typeof(CreateMemberEndRestraint))]
    [InlineData(typeof(CreateAssembly))]
    // Loads
    [InlineData(typeof(CreateBeamLoad))]
    [InlineData(typeof(CreateBeamThermalLoad))]
    [InlineData(typeof(CreateFaceLoad))]
    [InlineData(typeof(CreateFaceThermalLoad))]
    [InlineData(typeof(CreateGridAreaLoad))]
    [InlineData(typeof(CreateGridLineLoad))]
    [InlineData(typeof(CreateGridPlane))]
    [InlineData(typeof(CreateGridPointLoad))]
    [InlineData(typeof(CreateGridSurface))]
    [InlineData(typeof(CreateNodeLoad))]
    [InlineData(typeof(LoadProperties))]
    // Analysis
    [InlineData(typeof(AnalyseModel), true)]
    [InlineData(typeof(CreateAnalysisTask))]
    [InlineData(typeof(CreateSteelDesignTask))]
    // Results
    [InlineData(typeof(AssemblyDisplacements))]
    [InlineData(typeof(AssemblyDriftIndices))]
    [InlineData(typeof(AssemblyDrifts))]
    [InlineData(typeof(AssemblyForcesAndMoments))]
    [InlineData(typeof(BeamDerivedStresses), true)]
    [InlineData(typeof(BeamDisplacements), true)]
    [InlineData(typeof(BeamForcesAndMoments))]
    [InlineData(typeof(BeamStrainEnergyDensity), true)]
    [InlineData(typeof(BeamStresses), true)]
    [InlineData(typeof(Element2dDisplacements))]
    [InlineData(typeof(Element2dForcesAndMoments))]
    [InlineData(typeof(Element2dStresses))]
    [InlineData(typeof(Element3dDisplacements))]
    [InlineData(typeof(Element3dStresses))]
    [InlineData(typeof(FootfallResults))]
    [InlineData(typeof(GlobalPerformanceResults_OBSOLETE))]
    [InlineData(typeof(GlobalPerformanceResults))]
    [InlineData(typeof(Member1dDisplacements))]
    [InlineData(typeof(Member1dForcesAndMoments))]
    [InlineData(typeof(NodalForcesAndMoments))]
    [InlineData(typeof(NodeDisplacements))]
    [InlineData(typeof(ReactionForces))]
    [InlineData(typeof(SpringReactionForces))]
    [InlineData(typeof(TotalLoadsAndReactions))]
    [InlineData(typeof(SteelUtilisations))]
    // Display
    [InlineData(typeof(AssemblyResultDiagrams))]
    [InlineData(typeof(AssemblyResults), true)]
    [InlineData(typeof(Contour1dResults), true)]
    [InlineData(typeof(Contour2dResults), true)]
    [InlineData(typeof(Contour3dResults), true)]
    [InlineData(typeof(ContourNodeResults), true)]
    [InlineData(typeof(LoadDiagrams), true)]
    [InlineData(typeof(ReactionForceDiagrams))]
    [InlineData(typeof(ResultDiagrams))]
    [InlineData(typeof(CreateAnalysisTask_OBSOLETE))]
    public void ToggleDropDownsTest(Type t, bool ignoreSpacerDescriptionCount = false) {
      var comp = (GH_OasysDropDownComponent)Activator.CreateInstance(t);
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp, ignoreSpacerDescriptionCount);
      OasysDropDownComponentTestHelper.TestDeserialize(comp, verifyDropDownSelection: true);
    }

    [Fact]
    public void DropdownSelectionAndInputConnectionSurviveReopen() {
      var component = new CreateList();
      var source = new Param_String();
      component.CreateAttributes();
      source.CreateAttributes();

      string directory = Path.Combine(Environment.CurrentDirectory, "GH-Test-Files");
      Directory.CreateDirectory(directory);
      string path = Path.Combine(directory, Guid.NewGuid() + ".gh");

      using (var document = new GH_Document()) {
        document.AddObject(source, false);
        document.AddObject(component, false);
        component.Params.Input[1].AddSource(source);
        component.SetSelected(0, 2);
        string selected = component.SelectedItems[0];
        Guid componentId = component.InstanceGuid;
        Guid sourceId = source.InstanceGuid;

        try {
          var writer = new GH_DocumentIO { Document = document };
          Assert.True(writer.SaveQuiet(path));
          var reader = new GH_DocumentIO();
          Assert.True(reader.Open(path));
          using (reader.Document) {
            var loaded = Assert.IsType<CreateList>(reader.Document.FindComponent(componentId));
            Assert.Equal(selected, loaded.SelectedItems[0]);
            Assert.Single(loaded.Params.Input[1].Sources);
            Assert.Equal(sourceId, loaded.Params.Input[1].Sources[0].InstanceGuid);
          }
        } finally {
          File.Delete(path);
        }
      }
    }
  }
}
