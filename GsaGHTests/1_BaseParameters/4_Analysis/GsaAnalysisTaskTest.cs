using GsaAPI;
using GsaGH.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaGH.Parameters;

using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]

  public class GsaAnalysisTaskTests {

    private GsaModel _model;
    public GsaAnalysisTaskTests() {
      _model = new GsaModel();
      var cases = new Dictionary<int, LoadCase> {
       { 1, new LoadCase() { CaseType= GsaAPI.LoadCaseType.Dead, Name = "DL" } },
       { 3, new LoadCase() {CaseType= GsaAPI.LoadCaseType.Live, Name = "LL" } }
     };
      _model.ApiModel.SetLoadCases(new ReadOnlyDictionary<int, LoadCase>(cases));
      _model.ApiModel.AddGravityLoad(new GravityLoad { Case = 1, EntityList = "all" });
      _model.ApiModel.AddGravityLoad(new GravityLoad { Case = 3, EntityList = "all" });
    }

    private List<GsaAnalysisTask> GsaAnalysisTasksFromSeedModel(bool withCase = false) {
      var gsaSeedModel = new GsaModel();
      int id = gsaSeedModel.ApiModel.AddAnalysisTask();
      if (withCase) {
        gsaSeedModel.ApiModel.AddAnalysisCaseToTask(id, "AnyName", "L1");
      }
      return new List<GsaAnalysisTask>() { new GsaAnalysisTask(id, gsaSeedModel.ApiModel) };
    }

    [Fact]
    public void WhenNoCasesProvidedShouldAddDefaultCaseCreatedFromLoadCase() {
      GsaAPI.Model apiModel = _model.ApiModel;
      TaskHelper.ImportAnalysisTask(new GsaAnalysisTask(apiModel.AddAnalysisTask(), apiModel), ref apiModel);
      AnalysisTask outTask = _model.ApiModel.AnalysisTasks().Last().Value;
      //Gsa model has load case at Row(Or Id) 1 and 3
      Assert.Equal(1, outTask.Cases[0]);
      Assert.Equal(3, outTask.Cases[1]);
    }

    //[Fact]
    //public void AnalysisCasesAreAppendedWhenAnalysisCaseIsAlreadyAttachedToAnotherTask() {
    //  GsaAPI.Model apiModel = _model.ApiModel;
    //  int taskId = apiModel.AddAnalysisTask();
    //  //this will create analysis case with Id = 1
    //  apiModel.AddAnalysisCaseToTask(taskId, "AnyName", "L1");
    //  int analysisCaseId = apiModel.AnalysisTasks()[taskId].Cases[0];
    //  Assert.Equal(1, analysisCaseId);

    //  //now create another task
    //  var newTask = new GsaAnalysisTask(apiModel.AddAnalysisTask(), apiModel);
    //  //and assign same analysis case Id
    //  newTask.Cases.Add(new GsaAnalysisCase() {
    //    Id = analysisCaseId,
    //    ApiCase = new AnalysisCase("AnyName", "L1")
    //  });

    //  ReadOnlyDictionary<int, AnalysisTask> ttasks = apiModel.AnalysisTasks();
    //  TaskHelper.AddAnalysisTask(newTask, ref apiModel);
    // ttasks = apiModel.AnalysisTasks();
    //  Assert.Equal(2, ttasks.Keys.Max());
    //  int newAnalysCaseId = apiModel.AnalysisTasks()[newTask.Id].Cases[0];
    //  Assert.Equal(analysisCaseId + 1, newAnalysCaseId);
    //}

    [Fact]
    public void ShouldCreateDefaultTask() {
      TaskHelper.CreateDefaultStaticAnalysisTask(ref _model);
      Assert.Single(_model.ApiModel.AnalysisTasks());
    }

    [Fact]
    public void ShouldCreateDefaultCaseWhenCaseCountIsZero() {
      GsaAPI.Model apiModel = _model.ApiModel;
      TaskHelper.ImportAnalysisTask(GsaAnalysisTasksFromSeedModel().First(), ref apiModel);
      Assert.Equal(2, apiModel.AnalysisTasks().First().Value.Cases.Count);
    }

    [Fact]
    public void AnalysisCasesWillBeEmptyInAbsenceOfLoadCase() {
      var noLoadModel = new GsaModel();
      GsaAPI.Model apiModel = noLoadModel.ApiModel;
      TaskHelper.ImportAnalysisTask(GsaAnalysisTasksFromSeedModel().First(), ref apiModel);
      Assert.Empty(apiModel.AnalysisTasks().First().Value.Cases);
    }

    [Fact]
    public void EmptyConstructorTest() {
      var task = new GsaAnalysisTask();

      Assert.Equal(0, task.Id);
      Assert.Null(task.ApiTask);
      Assert.Empty(task.Cases);
    }
  }
}
