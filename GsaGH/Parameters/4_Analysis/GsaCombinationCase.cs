using System.Collections.Generic;

using GsaAPI;

using GsaGH.Helpers;

namespace GsaGH.Parameters {
  /// <summary>
  /// Combination cases are similar to analysis cases but differ in two respects:
  /// <list type="bullet">
  /// <item><description>Results for combination cases are inferred from analysis case results and not calculated explicitly.</description></item>
  /// <item><description>Combination cases can be enveloping cases, as described in <see href="https://docs.oasys-software.com/structural/gsa/references/envelopingingsa.html">Enveloping</see> in GSA.</description></item>
  /// </list>
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-comb-case.html">Combination Cases</see> to read more.</para>
  /// </summary>
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
      : this(name, definition) {
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

      return string.Join(" ", Id > 0 ? $"ID:{Id}" : string.Empty, s).TrimSpaces();
    }

    private void ValidateDefinition(string name, string definition) {
      // this should throw an exception when definition is invalid [GSA-7113]
      var combinationCase = new CombinationCase(name, definition);
    }
  }
}
