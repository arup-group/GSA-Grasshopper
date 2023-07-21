using System.Collections.Generic;
using System.Linq;
using GsaAPI;

namespace GsaGH.Parameters {
  public class GsaCombinationCase {
    public string Definition { get; set; }
    public string Name { get; set; }
    internal int Id { get; set; } = 0;

    public GsaCombinationCase() { }

    public GsaCombinationCase(string name, string definition) {
      Name = name;
      Definition = definition;
      ValidateDefinition(name, definition);
    }

    internal GsaCombinationCase(int id, string name, string definition) 
      : this (name, definition) {
      Id = id;
    }

    internal GsaCombinationCase(KeyValuePair<int, CombinationCase> keyValuePair) : this(
      keyValuePair.Key, keyValuePair.Value.Name, keyValuePair.Value.Definition) { }

    public GsaCombinationCase Duplicate() {
      return new GsaCombinationCase(Id, Name, Definition);
    }

    public override string ToString() {
      string s = string.Empty;
      if (Name != null) {
        s += " '" + Name.ToString() + "'";
      }

      if (Definition != null) {
        s += " " + Definition.ToString();
      }

      return string.Join(" ", (Id > 0 ? "ID:" + Id : string.Empty).Trim(), s.Trim()).Trim()
       .Replace("  ", " ");
    }

    private void ValidateDefinition(string name, string definition) {
      // this should throw an exception when definition is invalid [GSA-7113]
      var combinationCase = new CombinationCase(name, definition);
    }
  }
}
