using System.Xml.Linq;

using GsaAPI;

using GsaGH.Helpers;

namespace GsaGH.Parameters {
  /// <summary>
  /// Analysis Case definition, for instance `L1` for LoadCase 1 or `L1 + L2` for combining multiple load cases in one Analysis case.
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/analysiscases.html">Analysis cases</see> to read more.</para>
  /// </summary>
  public class GsaAnalysisCase {
    private AnalysisCase ApiCase { get; set; }
    internal int Id { get; set; } = 0;
    public string Definition => ApiCase.Description;
    public string Name => ApiCase.Name;
    private GsaAnalysisCase() {
      ApiCase = new AnalysisCase(string.Empty, string.Empty);
    }
    public GsaAnalysisCase(string name, string description) {
      ApiCase = new AnalysisCase(name, description);
    }

    internal GsaAnalysisCase(int id, string name, string description) {
      Id = id;
      ApiCase = new AnalysisCase(name, description);
    }

    public GsaAnalysisCase Duplicate() {
      return new GsaAnalysisCase(Id, Name, Definition);
    }

    public override string ToString() {
      string id = Id == 0 ? string.Empty : "ID:" + Id + " ";
      string s = string.Empty;
      if (Name != null) {
        s += " '" + Name + "'";
      }

      if (Definition != null) {
        s += " " + Definition;
      }
      return string.Join(" ", id, s).TrimSpaces();
    }
  }
}
