namespace GsaGH.Parameters {
  public class GsaCombinationCase {
    public string Description { get; set; }
    public string Name { get; set; }
    public GsaCombinationCase() { }

    public GsaCombinationCase(string name, string description) {
      Name = name;
      Description = description;
    }

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

    internal int Id { get; set; } = 0;
    internal GsaCombinationCase(int id, string name, string description) {
      Id = id;
      Name = name;
      Description = description;
    }
  }
}
