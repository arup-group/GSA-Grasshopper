namespace GsaGH.Parameters {

  public class GsaAnalysisCase {

    #region Properties + Fields
    public string Description { get; set; }
    public string Name { get; set; }
    internal int Id { get; set; } = 0;
    #endregion Properties + Fields

    #region Public Constructors
    public GsaAnalysisCase() {
    }

    public GsaAnalysisCase(string name, string description) {
      Name = name;
      Description = description;
    }

    #endregion Public Constructors

    #region Internal Constructors
    internal GsaAnalysisCase(int id, string name, string description = "") {
      Id = id;
      Name = name;
      Description = description;
    }

    #endregion Internal Constructors

    #region Public Methods
    public GsaAnalysisCase Duplicate() => new GsaAnalysisCase(Id, Name, Description);

    public override string ToString() {
      string id = Id == 0
        ? ""
        : "ID:" + Id + " ";
      string s = "";
      if (Name != null)
        s += " '" + Name + "'";
      if (Description != null)
        s += " " + Description;
      return string.Join(" ", id.Trim(), s.Trim())
        .Trim()
        .Replace("  ", " ");
    }

    #endregion Public Methods
  }
}
