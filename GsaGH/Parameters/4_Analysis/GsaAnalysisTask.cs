using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.Import;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Section class, this class defines the basic properties and methods for any Gsa Section
  /// </summary>
  public class GsaAnalysisTask {
    public enum AnalysisType {
      Static = 1,
      Buckling = 3,
      StaticPDelta = 4,
      NonlinearStatic = 8,
      ModalDynamic = 2,
      ModalPDelta = 5,
      Ritz = 32,
      RitzPDelta = 33,
      ResponseSpectrum = 6,
      PseudoResponseSpectrum = 42,
      LinearTimeHistory = 15,
      Harmonic = 14,
      Footfall = 34,
      Periodic = 35,
      FormFinding = 9,
      Envelope = 37,
      ModelStability = 39,
      ModelStabilityPDelta = 40,
    }

    public List<GsaAnalysisCase> Cases { get; set; } = null;
    public int Id {
      get => _id;
      set => _id = value;
    }
    public string Name { get; set; }
    public AnalysisType Type { get; set; }
    private int _id = 0;

    public GsaAnalysisTask() {
      _id = 0;
      Cases = new List<GsaAnalysisCase>();
      Type = AnalysisType.Static;
    }

    internal GsaAnalysisTask(int id, AnalysisTask task, Model model) {
      _id = id;
      Cases = new List<GsaAnalysisCase>();
      foreach (int caseId in task.Cases) {
        string caseName = model.AnalysisCaseName(caseId);
        string caseDescription = model.AnalysisCaseDescription(caseId);
        Cases.Add(new GsaAnalysisCase(caseId, caseName, caseDescription));
      }

      Type = (AnalysisType)task.Type;
      Name = task.Name;
    }

    public GsaAnalysisTask Duplicate() {
      var dup = new GsaAnalysisTask {
        _id = _id,
      };
      if (Cases != null)
        dup.Cases = Cases.ToList();
      dup.Type = Type;
      dup.Name = Name;
      return dup;
    }

    public override string ToString()
      => (Id > 0
          ? "ID:" + Id
          : ""
          + " '"
          + Name
          + "' "
          + Type.ToString()
            .Replace("_", " ")).Trim()
        .Replace("  ", " ");

    internal void CreateDeafultCases(GsaModel gsaModel) {
      Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> tuple
        = Analyses.GetAnalysisTasksAndCombinations(gsaModel);
      Cases = tuple.Item2.Select(x => x.Value)
        .ToList();
    }

    internal void CreateDefaultCases(Model model) {
      Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> tuple
        = Analyses.GetAnalysisTasksAndCombinations(model);
      Cases = tuple.Item2.Select(x => x.Value)
        .ToList();
    }
  }
}
