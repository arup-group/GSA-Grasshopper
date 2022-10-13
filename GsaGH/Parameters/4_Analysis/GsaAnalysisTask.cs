using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Section class, this class defines the basic properties and methods for any Gsa Section
  /// </summary>
  public class GsaAnalysisTask
  {
    public enum AnalysisType
    {
      Static = 1,
      Static_P_delta = 4,
      Nonlinear_static = 8,
      Modal_dynamic = 2,
      Modal_P_delta = 5,
      Ritz = 32,
      Ritz_P_Delta = 33,
      Response_spectrum = 6,
      Pseudo_Response_spectrum = 42,
      Linear_time_history = 15,
      Harmonic = 14,
      Footfall = 34,
      Periodic = 35,
      Buckling = 3,
      Form_finding = 9,
      Envelope = 37,
      Model_stability = 39,
      Model_stability_P_delta = 40
    }

    #region fields
    private int m_idd = 0;
    #endregion

    #region properties
    public List<GsaAnalysisCase> Cases { get; set; } = null;
    public string Name { get; set; }
    public AnalysisType Type { get; set; }
    public int ID
    {
      get
      {
        return this.m_idd;
      }
    }
    internal void SetID(int id)
    {
      this.m_idd = id;
    }
    public bool IsValid
    {
      get
      {
        return true;
      }
    }
    #endregion

    #region constructors
    public GsaAnalysisTask()
    {
      this.m_idd = 0;
      this.Cases = new List<GsaAnalysisCase>();
      this.Type = AnalysisType.Static;
    }

    internal GsaAnalysisTask(int ID, AnalysisTask task, Model model)
    {
      this.m_idd = ID;
      this.Cases = new List<GsaAnalysisCase>();
      foreach (int caseID in task.Cases)
      {
        string caseName = model.AnalysisCaseName(caseID);
        string caseDescription = model.AnalysisCaseDescription(caseID);
        this.Cases.Add(new GsaAnalysisCase(caseID, caseName, caseDescription));
      }
      this.Type = (AnalysisType)task.Type;
      this.Name = task.Name;
    }

    internal void CreateDeafultCases(Model model)
    {
      Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> tuple = Util.Gsa.FromGSA.GetAnalysisTasksAndCombinations(model);
      this.Cases = tuple.Item2.Select(x => x.Value).ToList();
    }
    #endregion

    #region methods
    internal void CreateDeafultCases(GsaModel gsaModel)
    {
      Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> tuple = Util.Gsa.FromGSA.GetAnalysisTasksAndCombinations(gsaModel);
      this.Cases = tuple.Item2.Select(x => x.Value).ToList();
    }

    public GsaAnalysisTask Duplicate()
    {
      if (this == null) { return null; }
      GsaAnalysisTask dup = new GsaAnalysisTask();
      dup.m_idd = this.m_idd;
      dup.Cases = this.Cases;
      dup.Type = this.Type;
      dup.Name = this.Name;
      return dup;
    }

    public override string ToString()
    {
      return "GSA Analysis Task" + ((this.ID > 0) ? " " + this.ID.ToString() : "") + " '" + this.Name + "' {" + this.Type.ToString() + "}";
    }
    #endregion
  }
}
