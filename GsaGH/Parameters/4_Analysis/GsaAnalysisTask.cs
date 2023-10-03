﻿using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers;

namespace GsaGH.Parameters {
  /// <summary>
  /// <para>An analysis task is a package of work for the solver. Thus we can have a static analysis task, a modal analysis task, etc. Each analysis task has one or more analysis case(s). The distinction is that the cases corresponds to result sets and define items such as loading (in the static case) while the task describes what the solver has to do. </para>
  /// <para>In Grasshopper, it is only possible to create linear static analysis tasks.</para>
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/analysistasks.html">Analysis Tasks</see> to read more.</para>
  /// </summary>
  public class GsaAnalysisTask {
    public List<GsaAnalysisCase> Cases { get; set; } = null;
    public int Id { get; set; } = 0;
    public string Name { get; set; }
    public AnalysisTaskType Type { get; set; }

    public GsaAnalysisTask() {
      Id = 0;
      Cases = new List<GsaAnalysisCase>();
      Type = AnalysisTaskType.Static;
    }

    internal GsaAnalysisTask(int id, AnalysisTask task, Model model) {
      Id = id;
      Cases = new List<GsaAnalysisCase>();
      foreach (int caseId in task.Cases) {
        string caseName = model.AnalysisCaseName(caseId);
        string caseDescription = model.AnalysisCaseDescription(caseId);
        Cases.Add(new GsaAnalysisCase(caseId, caseName, caseDescription));
      }

      Type = (AnalysisTaskType)task.Type;
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

    internal void CreateDefaultCases(GsaModel gsaModel) {
      Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> tuple
        = gsaModel.GetAnalysisTasksAndCombinations();
      Cases = tuple.Item2.Select(x => x.Value).ToList();
    }
  }
}
