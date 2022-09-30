using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Helpers;
using OasysGH.Parameters;
using System;
using System.Text.RegularExpressions;

namespace GsaGH.Parameters
{
  public class GsaAnalysisCase
  {
    public string Name { get; set; }
    public string Description { get; set; }
    internal int ID { get; set; } = 0;

    public GsaAnalysisCase()
    { }

    internal GsaAnalysisCase(int id, string name, string description = "")
    {
      this.ID = id;
      this.Name = name;
      this.Description = description;
    }

    public GsaAnalysisCase(string name, string description)
    {
      this.Name = name;
      this.Description = description;
    }

    public GsaAnalysisCase Duplicate()
    {
      return new GsaAnalysisCase(ID, Name, Description);
    }

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
      string s = "GSA Analysis Case";
      if (Name != null)
        s += " '" + Name.ToString() + "'";
      if (Description != null)
        s += " { " + Description.ToString() + " }";
      return s;
    }

    #endregion
  }
}
