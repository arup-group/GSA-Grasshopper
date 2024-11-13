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

    public GsaModel GSAModel() {
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
    public void AnalysisCasesAreGettingAddedAtLoadCaseId() {
      GsaModel model = GSAModel();
      var task = new GsaAnalysisTask {
        Id = model.ApiModel.AddAnalysisTask(),
      };
      ModelFactory.BuildAnalysisTask(model, new List<GsaAnalysisTask> { task });
      AnalysisTask outTask = model.ApiModel.AnalysisTasks()[task.Id];
      Assert.Equal(1, outTask.Cases[0]);
      Assert.Equal(3, outTask.Cases[1]);
    }

    [Fact]
    public void AnalysisCasesAreGettingAppendedAtLastWhenAnalysisCaseExist() {
      GsaModel model = GSAModel();
      int taskId = model.ApiModel.AddAnalysisTask();
      //this will create analysis task of Id=1
      model.ApiModel.AddAnalysisCaseToTask(taskId, "DL", "L1");

      var newTask = new GsaAnalysisTask {
        Id = model.ApiModel.AddAnalysisTask(),
      };
      //now smae task Id = 1 is used to create another case. So, should create case at Id = 2
      newTask.Cases.Add(new GsaAnalysisCase() { Id = 1, Name = "DL", Definition = "L1" });

      ModelFactory.BuildAnalysisTask(model, new List<GsaAnalysisTask> { newTask });
      AnalysisTask outTask = model.ApiModel.AnalysisTasks()[newTask.Id];
      Assert.Equal(2, outTask.Cases[0]);

    }

    [Fact]
    public void AnalysisCasesWillBeEmptyInAbsenseOfLoadCase() {
      var model = new GsaModel();
       var task = new GsaAnalysisTask {
        Id = model.ApiModel.AddAnalysisTask(),
      };
      ModelFactory.BuildAnalysisTask(model, new List<GsaAnalysisTask> { task });
      AnalysisTask outTask = model.ApiModel.AnalysisTasks()[task.Id];
      Assert.Empty(outTask.Cases);
    }
  }
}
