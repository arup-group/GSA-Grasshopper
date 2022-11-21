using GH_IO.Serialization;

namespace GsaGH.Parameters
{
  public class GsaAnalysisCase
  {
    internal int ID { get; set; } = 0;
    public string Name { get; set; }
    public string Description { get; set; }

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
      string id = this.ID == 0 ? "" : "ID:" + ID + " ";
      string s = "";
      if (this.Name != null)
        s += " '" + this.Name + "'";
      if (this.Description != null)
        s += " " + this.Description;
      return string.Join(" ", id.Trim(), s.Trim()).Trim().Replace("  ", " ");
    }
    #endregion
  }
}
