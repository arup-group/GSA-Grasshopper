using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Parameters;

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
    public string Name { get; set; }
    public AnalysisType Type { get; set; }
    public int ID { get { return m_idd; } }
    internal void SetID(int id)
    {
      m_idd = id;
    }
    #region fields
    private int m_idd = 0;
    public List<GsaAnalysisCase> Cases { get; set; } = null;
    #endregion

    #region constructors
    public GsaAnalysisTask()
    {
      m_idd = 0;
      Cases = new List<GsaAnalysisCase>();
      Type = AnalysisType.Static;
    }
    internal GsaAnalysisTask(int ID, AnalysisTask task, Model model)
    {
      m_idd = ID;
      Cases = new List<GsaAnalysisCase>();
      foreach (int caseID in task.Cases)
      {
        string caseName = model.AnalysisCaseName(caseID);
        string caseDescription = model.AnalysisCaseDescription(caseID);
        Cases.Add(new GsaAnalysisCase(caseID, caseName, caseDescription));
      }
      Type = (AnalysisType)task.Type;
      Name = task.Name;
    }
    internal void CreateDeafultCases(Model model)
    {
      Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> tuple = Util.Gsa.FromGSA.GetAnalysisTasksAndCombinations(model);
      this.Cases = tuple.Item2.Select(x => x.Value).ToList();
    }
    internal void CreateDeafultCases(GsaModel gsaModel)
    {
      Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> tuple = Util.Gsa.FromGSA.GetAnalysisTasksAndCombinations(gsaModel);
      this.Cases = tuple.Item2.Select(x => x.Value).ToList();
    }

    public GsaAnalysisTask Duplicate()
    {
      if (this == null) { return null; }
      GsaAnalysisTask dup = new GsaAnalysisTask();
      dup.m_idd = m_idd;
      dup.Cases = Cases;
      dup.Type = Type;
      dup.Name = Name;
      return dup;
    }
    #endregion

    #region properties
    public bool IsValid
    {
      get
      {
        return true;
      }
    }
    #endregion

    #region methods
    public override string ToString()
    {
      return "GSA Analysis Task" + ((ID > 0) ? " " + ID.ToString() : "") + " '" + Name + "' {" + Type.ToString() + "}";
    }
    #endregion
  }
}
