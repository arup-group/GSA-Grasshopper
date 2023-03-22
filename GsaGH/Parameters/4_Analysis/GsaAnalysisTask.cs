using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;

namespace GsaGH.Parameters {
  /// <summary>
  /// Section class, this class defines the basic properties and methods for any Gsa Section
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

    #region fields
    private int m_idd = 0;
    #endregion

    #region properties
    public List<GsaAnalysisCase> Cases { get; set; } = null;
    public string Name {
      get; set;
    }
    public AnalysisType Type {
      get; set;
    }
    public int ID {
      get {
        return m_idd;
      }
      set {
        m_idd = value;
      }
    }
    #endregion

    #region constructors
    public GsaAnalysisTask() {
      m_idd = 0;
      Cases = new List<GsaAnalysisCase>();
      Type = AnalysisType.Static;
    }

    internal GsaAnalysisTask(int ID, AnalysisTask task, Model model) {
      m_idd = ID;
      Cases = new List<GsaAnalysisCase>();
      foreach (int caseID in task.Cases) {
        string caseName = model.AnalysisCaseName(caseID);
        string caseDescription = model.AnalysisCaseDescription(caseID);
        Cases.Add(new GsaAnalysisCase(caseID, caseName, caseDescription));
      }
      Type = (AnalysisType)task.Type;
      Name = task.Name;
    }
    #endregion

    #region methods
    internal void CreateDefaultCases(Model model) {
      Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> tuple = Helpers.Import.Analyses.GetAnalysisTasksAndCombinations(model);
      Cases = tuple.Item2.Select(x => x.Value).ToList();
    }
    internal void CreateDeafultCases(GsaModel gsaModel) {
      Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> tuple = Helpers.Import.Analyses.GetAnalysisTasksAndCombinations(gsaModel);
      Cases = tuple.Item2.Select(x => x.Value).ToList();
    }

    public GsaAnalysisTask Duplicate() {
      if (this == null) {
        return null;
      }
      var dup = new GsaAnalysisTask();
      dup.m_idd = m_idd;
      if (Cases != null)
        dup.Cases = Cases.ToList();
      dup.Type = Type;
      dup.Name = Name;
      return dup;
    }

    public override string ToString() {
      return (ID > 0 ? "ID:" + ID : "" + " '" + Name + "' " + Type.ToString().Replace("_", " ")).Trim().Replace("  ", " ");
    }
    #endregion
  }
}
