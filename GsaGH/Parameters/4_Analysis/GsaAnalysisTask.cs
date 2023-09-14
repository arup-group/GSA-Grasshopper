using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers;
using GsaGH.Helpers.Import;

namespace GsaGH.Parameters {
  /// <summary>
  /// <para>An analysis task is a package of work for the solver. Thus we can have a static analysis task, a modal analysis task, etc. Each analysis task has one or more analysis case(s). The distinction is that the cases corresponds to result sets and define items such as loading (in the static case) while the task describes what the solver has to do. </para>
  /// <para>In Grasshopper, it is only possible to create linear static analysis tasks.</para>
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/analysistasks.html">Analysis Tasks</see> to read more.</para>
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
    public int Id { get; set; } = 0;
    public string Name { get; set; }
    public AnalysisType Type { get; set; }

    public GsaAnalysisTask() {
      Id = 0;
      Cases = new List<GsaAnalysisCase>();
      Type = AnalysisType.Static;
    }

    internal GsaAnalysisTask(int id, AnalysisTask task, Model model) {
      Id = id;
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
        Id = Id,
      };
      if (Cases != null) {
        dup.Cases = Cases.ToList();
      }

      dup.Type = Type;
      dup.Name = Name;
      return dup;
    }

    public override string ToString() {
      return (Id > 0 ? $"ID:{Id} " : string.Empty) + $"'{Name}' {Type}".Replace("_", " ")
        .TrimSpaces();
    }

    internal void CreateDeafultCases(GsaModel gsaModel) {
      Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> tuple
        = Analyses.GetAnalysisTasksAndCombinations(gsaModel);
      Cases = tuple.Item2.Select(x => x.Value).ToList();
    }

    internal void CreateDefaultCases(Model model) {
      Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> tuple
        = Analyses.GetAnalysisTasksAndCombinations(model);
      Cases = tuple.Item2.Select(x => x.Value).ToList();
    }
  }
}
