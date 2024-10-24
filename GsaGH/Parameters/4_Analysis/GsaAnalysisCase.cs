﻿using GsaGH.Helpers;

namespace GsaGH.Parameters {
  /// <summary>
  /// Analysis Case definition, for instance `L1` for LoadCase 1 or `L1 + L2` for combining multiple load cases in one Analysis case.
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/analysiscases.html">Analysis cases</see> to read more.</para>
  /// </summary>
  public class GsaAnalysisCase {
    public string Definition { get; set; }
    public string Name { get; set; }
    internal int Id { get; set; } = 0;

    public GsaAnalysisCase() { }

    public GsaAnalysisCase(string name, string description) {
      Name = name;
      Definition = description;
    }

    internal GsaAnalysisCase(int id, string name, string description = "") {
      Id = id;
      Name = name;
      Definition = description;
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
