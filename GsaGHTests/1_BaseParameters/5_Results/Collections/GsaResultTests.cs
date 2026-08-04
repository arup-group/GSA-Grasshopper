using System.Collections.Generic;
using System.Collections.ObjectModel;

using GsaAPI;

using GsaGH.Parameters;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;

using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public partial class GsaResultTests {
    public static IGsaResult AnalysisCaseResult(string file, int caseId) {
      var apiModel = new GsaAPI.Model(file);
      var model = new GsaModel(apiModel);
      ReadOnlyDictionary<int, AnalysisCaseResult> analysisCaseResults = model.ApiModel.Results();
      return new GsaResult(model, analysisCaseResults[caseId], caseId);
    }

    public static IGsaResult CombinationCaseResult(string file, int caseId, IEnumerable<int> permutations = null) {
      var apiModel = new GsaAPI.Model(file);
      var model = new GsaModel(apiModel);
      ReadOnlyDictionary<int, CombinationCaseResult> combinationCaseResults = model.ApiModel.CombinationCaseResults();
      if (permutations == null) {
        permutations = new List<int>() { 1, 2 };
      }

      return new GsaResult(model, combinationCaseResults[caseId], caseId, permutations);
    }

    [Theory]
    [InlineData(true, false, false, true)]
    [InlineData(false, true, false, true)]
    [InlineData(false, false, true, true)]
    [InlineData(true, false, false, false)]
    [InlineData(false, true, false, false)]
    [InlineData(false, false, true, false)]
    [InlineData(true, false, false, false, true)]
    [InlineData(false, true, false, false, true)]
    [InlineData(false, false, true, false, true)]
    public void CreatingResultsIsInvalid(bool modelIsNull, bool resultsAreNull, bool caseIdIsInvalid, bool analysisCase, bool permutationEmpty = false) {

      GsaModel model = modelIsNull ? null : new GsaModel(new GsaAPI.Model(GsaFile.SteelDesignSimple));
      AnalysisCaseResult analysisCaseResult
        = resultsAreNull ? null : new GsaAPI.Model(GsaFile.SteelDesignSimple).Results()[1];
      CombinationCaseResult combinationCaseResult
        = resultsAreNull ? null : new GsaAPI.Model(GsaFile.SteelDesignSimple).CombinationCaseResults()[1];
      int caseId = caseIdIsInvalid ? 0 : 1;

      GsaResult results = analysisCase
        ? new GsaResult(model, analysisCaseResult, caseId)
        : new GsaResult(model, combinationCaseResult, caseId, permutationEmpty ? null : new List<int>() { 1 });

      Assert.NotNull(results);
      if (modelIsNull || resultsAreNull) {
        Assert.Null(results.Model);
        return;
      } else {
        Assert.NotNull(results.Model);
      }

      if (!caseIdIsInvalid) {
        Assert.Null(results.CaseName);
        return;
      } else {
        Assert.Equal(caseId, results.CaseId);
      }

      Assert.Equal(analysisCase ? CaseType.AnalysisCase : CaseType.CombinationCase, results.CaseType);
      if (!permutationEmpty && !analysisCase) {
        Assert.Single(results.SelectedPermutationIds);
      } else {
        Assert.Null(results.SelectedPermutationIds);
      }
      //fields
      Assert.NotNull(results.Element1dAverageStrainEnergyDensities);
      Assert.NotNull(results.Element1dDisplacements);
      Assert.NotNull(results.Element1dInternalForces);
      Assert.NotNull(results.Element1dDerivedStresses);
      Assert.NotNull(results.Element1dStrainEnergyDensities);
      Assert.NotNull(results.Element1dStresses);
      Assert.NotNull(results.Element2dDisplacements);
      Assert.NotNull(results.Element2dForces);
      Assert.NotNull(results.Element2dMoments);
      Assert.NotNull(results.Element2dShearForces);
      Assert.NotNull(results.Element2dStresses);
      Assert.NotNull(results.Element3dDisplacements);
      Assert.NotNull(results.Element3dStresses);
      Assert.NotNull(results.NodeDisplacements);
      Assert.NotNull(results.NodeReactionForces);
      Assert.NotNull(results.NodeSpringForces);
      Assert.NotNull(results.Member1dInternalForces);
      Assert.NotNull(results.Member1dDisplacements);

      if (analysisCase) {
        Assert.NotNull(results.GlobalResults);
        Assert.NotNull(results.NodeTransientFootfalls);
        Assert.NotNull(results.NodeResonantFootfalls);
      } else {
        Assert.Null(results.GlobalResults);
        Assert.Null(results.NodeTransientFootfalls);
        Assert.Null(results.NodeResonantFootfalls);
      }

      if (caseId == 0) {
        return;
      }

      if (analysisCase) {
        Assert.Equal("DL", results.CaseName);
      } else {
        Assert.Equal("ULS", results.CaseName);
      }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ToStringReturnsValidString(bool isAnalysisCase) {
      GsaResult result = isAnalysisCase ? (GsaResult)AnalysisCaseResult(GsaFile.SteelDesignSimple, 1) :
        (GsaResult)CombinationCaseResult(GsaFile.SteelDesignSimple, 1, new List<int>() { 1, 2, 3, });

      string expectedString = isAnalysisCase ? "A1 'DL'" : "C1 (3 permutations) 'ULS'";

      Assert.Equal(expectedString, result.ToString());
    }

    [Fact]
    public void NodeIdsReturnsValidNumbers() {
      var result = (GsaResult)AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);

      Assert.Equal(new ReadOnlyCollection<int>(new List<int>() { 1, 2 }), result.NodeIds("all"));
      Assert.Equal(new ReadOnlyCollection<int>(new List<int>() { 1 }), result.NodeIds("1"));
      Assert.Equal(new ReadOnlyCollection<int>(new List<int>() { 1, 2 }), result.NodeIds("1 2"));
      Assert.Equal(new ReadOnlyCollection<int>(new List<int>() { }), result.NodeIds("10 20"));
    }
    [Fact]
    public void ElementIdsReturnsValidNumbers() {
      var result = (GsaResult)AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);

      Assert.Equal(new ReadOnlyCollection<int>(new List<int>() { 1, }), result.ElementIds("all", 1));
      Assert.Equal(new ReadOnlyCollection<int>(new List<int>() { 1, }), result.ElementIds("1", 1));
      Assert.Equal(new ReadOnlyCollection<int>(new List<int>() { 1, }), result.ElementIds("1 2", 1));
      Assert.Equal(new ReadOnlyCollection<int>(new List<int>() { }), result.ElementIds("10 20", 1));
    }
    [Fact]
    public void MemberIdsReturnsValidNumbers() {
      var result = (GsaResult)AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);

      Assert.Equal(new ReadOnlyCollection<int>(new List<int>() { 1, }), result.MemberIds("all"));
      Assert.Equal(new ReadOnlyCollection<int>(new List<int>() { 1, }), result.MemberIds("1"));
      Assert.Equal(new ReadOnlyCollection<int>(new List<int>() { 1, }), result.MemberIds("1 2"));
      Assert.Equal(new ReadOnlyCollection<int>(new List<int>() { }), result.MemberIds("10 20"));
    }
  }
}
