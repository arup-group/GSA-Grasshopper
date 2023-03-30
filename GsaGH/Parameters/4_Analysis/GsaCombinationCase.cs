namespace GsaGH.Parameters {

  public class GsaCombinationCase {

    #region Properties + Fields
    public string Description { get; set; }
    public string Name { get; set; }
    internal int Id { get; set; } = 0;
    #endregion Properties + Fields

    #region Public Constructors
    public GsaCombinationCase() {
    }

    public GsaCombinationCase(string name, string description) {
      Name = name;
      Description = description;
    }

    #endregion Public Constructors

    #region Internal Constructors
    internal GsaCombinationCase(int id, string name, string description) {
      Id = id;
      Name = name;
      Description = description;
    }

    #endregion Internal Constructors

    #region Public Methods
    public GsaCombinationCase Duplicate() => new GsaCombinationCase(Id, Name, Description);

    public override string ToString() {
      string s = "";
      if (Name != null)
        s += " '" + Name.ToString() + "'";
      if (Description != null)
        s += " " + Description.ToString();
      return string.Join(" ",
          (Id > 0
            ? "ID:" + Id
            : "").Trim(),
          s.Trim())
        .Trim()
        .Replace("  ", " ");
    }

    #endregion Public Methods
  }
}
