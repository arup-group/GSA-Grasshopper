using System;
using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public partial class GsaResultTests {
    public static IGsaResult AnalysisCaseResult(string file, int caseId) {
      var apiModel = new GsaAPI.Model(file);
      var model = new GsaModel(apiModel);
      ReadOnlyDictionary<int, AnalysisCaseResult> analysisCaseResults = model.Model.Results();
      return new GsaResult(model, analysisCaseResults[caseId], caseId);
    }

    public static IGsaResult CombinationCaseResult(string file, int caseId, IEnumerable<int> permutations = null) {
      var apiModel = new GsaAPI.Model(file);
      var model = new GsaModel(apiModel);
      ReadOnlyDictionary<int, CombinationCaseResult> combinationCaseResults = model.Model.CombinationCaseResults();
      if (permutations == null) {
        permutations = new List<int>() { 1, 2 };
      }

      return new GsaResult(model, combinationCaseResults[caseId], caseId, permutations);
    }

    [Fact]
    public void ClassHasValidNumberOfDeclaredElements() {
      TypeInfo t = typeof(GsaResult).GetTypeInfo();
      int propertiesCount = t.DeclaredProperties.Count();
      int methodsCount = t.DeclaredMethods.Count(); // decalredProperties x 2 (get and set) + others 
      int constructorsCount = t.DeclaredConstructors.Count();
      int eventCount = t.DeclaredEvents.Count();
      int fieldCount = t.DeclaredFields.Count();
      int memberCount = t.DeclaredMembers.Count();
      int nestedTypesCount = t.DeclaredNestedTypes.Count();
      int interfacesCount = t.ImplementedInterfaces.Count();
      int genericTypesCount = t.GenericTypeParameters.Count();

      //if this test will fail, that means, you need to remember of adding new tests for stuff you added/removed
      Assert.Equal(26, propertiesCount);
      Assert.Equal(58, methodsCount);
      Assert.Equal(2, constructorsCount);
      Assert.Equal(0, eventCount);
      Assert.Equal(propertiesCount, fieldCount);
      Assert.Equal(1, nestedTypesCount);
      Assert.Equal(1, interfacesCount);
      Assert.Equal(0, genericTypesCount);

      int sum = propertiesCount + methodsCount + constructorsCount + eventCount + fieldCount
        + nestedTypesCount + genericTypesCount;
      Assert.Equal(sum, memberCount);
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
        : new GsaResult(model, combinationCaseResult, caseId, permutationEmpty ? null : new List<int>(){1});
      
      Assert.NotNull(results);
      if (model == null) {
        Assert.Null(results.Model);
      } else {
        Assert.NotNull(results.Model);
      }
      switch (resultsAreNull) {
        case true when analysisCase:
          Assert.Equal("DL", results.CaseName);
          break;
        case true when !analysisCase:
          Assert.Equal("ULS", results.CaseName);
          break;
        default:
          Assert.Null(results.CaseName);
          break;
      }

      Assert.Equal(caseId, results.CaseId);
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
      Assert.NotNull(results.NodeResonantFootfalls);
      Assert.NotNull(results.NodeTransientFootfalls);
      Assert.NotNull(results.Member1dInternalForces);
      Assert.NotNull(results.Member1dDisplacements);

      if(analysisCase) {
        Assert.NotNull(results.GlobalResults);
      } else {
        Assert.Null(results.GlobalResults);
      }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ToStringReturnsValidString(bool isAnalysisCase) {
      GsaResult result = isAnalysisCase ? (GsaResult)AnalysisCaseResult(GsaFile.SteelDesignSimple, 1) :
        (GsaResult)CombinationCaseResult(GsaFile.SteelDesignSimple, 1, new List<int>(){1,2,3,});

      string expectedString = isAnalysisCase ? "A1": "C1 P:3";

      Assert.Equal(expectedString, result.ToString());
    }

    [Fact]
    public void NodeIdsReturnsValidNumbers() {
      var result = (GsaResult)AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      
      Assert.Equal(new ReadOnlyCollection<int>(new List<int>(){1, 2}) ,result.NodeIds("all"));
      Assert.Equal(new ReadOnlyCollection<int>(new List<int>(){1}) ,result.NodeIds("1"));
      Assert.Equal(new ReadOnlyCollection<int>(new List<int>(){1, 2}), result.NodeIds("1 2"));
      Assert.Equal(new ReadOnlyCollection<int>(new List<int>(){}), result.NodeIds("10 20"));
    }
  }
}
