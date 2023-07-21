using System.Collections.Generic;
using GsaAPI;

namespace GsaGH.Parameters {
  public class GsaCombinationCase {
    public string Description { get; set; }
    public string Name { get; set; }
    internal int Id { get; set; } = 0;

    public GsaCombinationCase() { }

    public GsaCombinationCase(string name, string description) {
      Name = name;
      Description = description;
    }

    internal GsaCombinationCase(int id, string name, string description) {
      Id = id;
      Name = name;
      Description = description;
    }

    internal GsaCombinationCase(KeyValuePair<int, CombinationCase> keyValuePair) : this(
      keyValuePair.Key, keyValuePair.Value.Name, keyValuePair.Value.Definition) { }

    public GsaCombinationCase Duplicate() {
      return new GsaCombinationCase(Id, Name, Description);
    }

    public override string ToString() {
      string s = string.Empty;
      if (Name != null) {
        s += " '" + Name.ToString() + "'";
      }

      if (Description != null) {
        s += " " + Description.ToString();
      }

      return string.Join(" ", (Id > 0 ? "ID:" + Id : string.Empty).Trim(), s.Trim()).Trim()
       .Replace("  ", " ");
    }
  }
}
