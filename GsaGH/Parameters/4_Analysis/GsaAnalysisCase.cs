﻿namespace GsaGH.Parameters
{
  public class GsaAnalysisCase
  {
    internal int ID { get; set; } = 0;
    public string Name { get; set; }
    public string Description { get; set; }

    public bool IsValid
    {
      get
      {
        return true;
      }
    }

    public GsaAnalysisCase()
    {
    }

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

    #region methods
    public GsaAnalysisCase Duplicate()
    {
      return new GsaAnalysisCase(ID, this.Name, this.Description);
    }

    public override string ToString()
    {
      string s = "GSA Analysis Case";
      if (this.Name != null)
        s += " '" + this.Name.ToString() + "'";
      if (this.Description != null)
        s += " { " + this.Description.ToString() + " }";
      return s;
    }
    #endregion
  }
}
