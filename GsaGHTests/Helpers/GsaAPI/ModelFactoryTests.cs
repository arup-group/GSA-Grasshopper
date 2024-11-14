using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using GH_IO.Serialization;

using GsaAPI;

using GsaGH.Helpers;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;

using OasysUnits.Units;

using Xunit;

namespace GsaGHTests.Helpers.GsaApi {
  [Collection("GrasshopperFixture collection")]
  public class ModelFactoryTests {

    public GsaModel GsaModel() {
      var model = new GsaModel();
      var cases = new Dictionary<int, LoadCase> {
       { 1, new LoadCase() { CaseType= GsaAPI.LoadCaseType.Dead, Name = "DL" } },
       { 3, new LoadCase() {CaseType= GsaAPI.LoadCaseType.Live, Name = "LL" } }
     };
      model.ApiModel.SetLoadCases(new ReadOnlyDictionary<int, LoadCase>(cases));
      model.ApiModel.AddGravityLoad(new GravityLoad { Case = 1, EntityList = "all" });
      model.ApiModel.AddGravityLoad(new GravityLoad { Case = 3, EntityList = "all" });
      return model;
    }
    [Fact]
    public void AnalysisCasesAreAddedAtId() {
      GsaModel model = GsaModel();
      var task = new GsaAnalysisTask {
        Id = model.ApiModel.AddAnalysisTask(),
      };
      ModelFactory.BuildAnalysisTask(model.ApiModel, new List<GsaAnalysisTask> { task }, true);
      AnalysisTask outTask = model.ApiModel.AnalysisTasks()[task.Id];
      //Gsa model has load case at Row(Or Id) 1 and 3
      Assert.Equal(1, outTask.Cases[0]);
      Assert.Equal(3, outTask.Cases[1]);
    }

    [Fact]
    public void AnalysisCasesAreAppendedWhenAnalysisCaseIsAlreadyAttachedToAnotherTask() {
      GsaModel model = GsaModel();
      int taskId = model.ApiModel.AddAnalysisTask();
      //this will create analysis case having Id = 1
      model.ApiModel.AddAnalysisCaseToTask(taskId, "DL", "L1");
      int analysisCaseId = model.ApiModel.AnalysisTasks()[1].Cases[0];

      //now create another task
      var newTask = new GsaAnalysisTask {
        Id = model.ApiModel.AddAnalysisTask(),
      };

      //and assign same analysis case Id
      newTask.Cases.Add(new GsaAnalysisCase() { Id = analysisCaseId, Name = "DL", Definition = "L1" });

      ModelFactory.BuildAnalysisTask(model.ApiModel, new List<GsaAnalysisTask> { newTask });
      AnalysisTask outTask = model.ApiModel.AnalysisTasks()[newTask.Id];
      Assert.Equal(analysisCaseId + 1, outTask.Cases[0]);

    }

    [Fact]
    public void AnalysisCasesWillBeEmptyInAbsenseOfLoadCase() {
      var model = new GsaModel();
       var task = new GsaAnalysisTask {
        Id = model.ApiModel.AddAnalysisTask(),
      };
      ModelFactory.BuildAnalysisTask(model.ApiModel, new List<GsaAnalysisTask> { task });
      AnalysisTask outTask = model.ApiModel.AnalysisTasks()[task.Id];
      Assert.Empty(outTask.Cases);
    }
  }
}
