using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaAPI;

using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;

using Xunit;

namespace GsaGHTests.Helpers.GsaApi {
  [Collection("GrasshopperFixture collection")]
  public class ModelFactoryTests {
    private GsaModel _model;
    public ModelFactoryTests() {
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
      return new List<GsaAnalysisTask>() { new GsaAnalysisTask(id, gsaSeedModel.ApiModel.AnalysisTasks()[id], gsaSeedModel.ApiModel) };
    }

    [Fact]
    public void WhenNoCasesProvidedShouldAddDefaultCaseCreatedFromLoadCase() {
      var task = new GsaAnalysisTask {
        Id = _model.ApiModel.AddAnalysisTask(),
      };
      ModelFactory.BuildAnalysisTask(_model.ApiModel, new List<GsaAnalysisTask> { task }, true);
      AnalysisTask outTask = _model.ApiModel.AnalysisTasks()[task.Id];
      //Gsa model has load case at Row(Or Id) 1 and 3
      Assert.Equal(1, outTask.Cases[0]);
      Assert.Equal(3, outTask.Cases[1]);
    }

    [Fact]
    public void AnalysisCasesAreAppendedWhenAnalysisCaseIsAlreadyAttachedToAnotherTask() {
      int taskId = _model.ApiModel.AddAnalysisTask();
      //this will create analysis case with Id = 1
      _model.ApiModel.AddAnalysisCaseToTask(taskId, "AnyName", "L1");
      int analysisCaseId = _model.ApiModel.AnalysisTasks()[taskId].Cases[0];
      Assert.Equal(1, analysisCaseId);

      //now create another task
      var newTask = new GsaAnalysisTask {
        Id = _model.ApiModel.AddAnalysisTask(),
      };
      Assert.Equal(2, newTask.Id);

      //and assign same analysis case Id
      newTask.Cases.Add(new GsaAnalysisCase() {
        Id = analysisCaseId,
        Name = "AnyName",
        Definition = "L1"
      });

      ModelFactory.BuildAnalysisTask(_model.ApiModel, new List<GsaAnalysisTask> { newTask });
      int newAnalysCaseId = _model.ApiModel.AnalysisTasks()[newTask.Id].Cases[0];
      Assert.Equal(analysisCaseId + 1, newAnalysCaseId);
    }

    [Fact]
    public void ShouldAddMissingTask() {
      ModelFactory.BuildAnalysisTask(_model.ApiModel, GsaAnalysisTasksFromSeedModel());
      Assert.Single(_model.ApiModel.AnalysisTasks());
    }

    [Fact]
    public void ShouldNotCreateDefaultCaseWhenCaseCountNotZero() {
      ModelFactory.BuildAnalysisTask(_model.ApiModel, GsaAnalysisTasksFromSeedModel(true), true);
      Assert.Single(_model.ApiModel.AnalysisTasks().First().Value.Cases);
    }

    [Fact]
    public void ShouldCreateDefaultCaseWhenCaseCountIsZero() {
      ModelFactory.BuildAnalysisTask(_model.ApiModel, GsaAnalysisTasksFromSeedModel(), true);
      Assert.Equal(2, _model.ApiModel.AnalysisTasks().First().Value.Cases.Count);
    }

    [Fact]
    public void AnalysisCasesWillBeEmptyInAbsenceOfLoadCase() {
      var noLoadModel = new GsaModel();
      ModelFactory.BuildAnalysisTask(noLoadModel.ApiModel, GsaAnalysisTasksFromSeedModel(), true);
      Assert.Empty(_model.ApiModel.AnalysisTasks().First().Value.Cases);
    }
  }
}
