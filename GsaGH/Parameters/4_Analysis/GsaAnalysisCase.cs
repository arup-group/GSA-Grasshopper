namespace GsaGH.Parameters {
  public class GsaAnalysisCase {
    public string Description { get; set; }
    public string Name { get; set; }
    internal int Id { get; set; } = 0;

    public GsaAnalysisCase() { }

    public GsaAnalysisCase(string name, string description) {
      Name = name;
      Description = description;
    }

    internal GsaAnalysisCase(int id, string name, string description = "") {
      Id = id;
      Name = name;
      Description = description;
    }

    public GsaAnalysisCase Duplicate() {
      return new GsaAnalysisCase(Id, Name, Description);
    }

    public override string ToString() {
      string id = Id == 0
        ? ""
        : "ID:" + Id + " ";
      string s = "";
      if (Name != null) {
        s += " '" + Name + "'";
      }

      if (Description != null) {
        s += " " + Description;
      }

      return string.Join(" ", id.Trim(), s.Trim())
        .Trim()
        .Replace("  ", " ");
    }
  }
}
