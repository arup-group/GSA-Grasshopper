using System;
using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters {
  /// <summary>
  /// When <see cref="IGsaLoad"/>s are applied to the model they are assigned to a load case. Load cases are a convenient way of grouping together a collection of loads that are to be considered acting together, for instance dead loads or live loads. In GSA, the load cases are only used to group loading and are not used directly for analysis.
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-load-title.html">Load Case Specification</see> to read more.</para>
  /// </summary>
  public class GsaLoadCase {
    public int Id { get; private set; }
    public LoadCase LoadCase;

    public GsaLoadCase() { }

    public GsaLoadCase(int id) {
      if (id < 1) {
        throw new ArgumentException("LoadCase ID cannot be zero or negative");
      }

      Id = id;
    }

    public GsaLoadCase(int id, LoadCaseType type, string name) : this(id) {
      LoadCase = new LoadCase() {
        CaseType = (GsaAPI.LoadCaseType)Enum.Parse(typeof(GsaAPI.LoadCaseType), type.ToString()),
        Name = name
      };
    }

    internal GsaLoadCase(int id, ReadOnlyDictionary<int, LoadCase> loadCases) : this(id) {
      if (loadCases != null && loadCases.ContainsKey(Id)) {
        LoadCase = loadCases[id];
      }
    }

    public GsaLoadCase Duplicate() {
      return LoadCase == null
        ? new GsaLoadCase() {
          Id = Id
        }
        : new GsaLoadCase() {
          LoadCase = DuplicateApiObject(),
          Id = Id
        };
    }

    internal LoadCase DuplicateApiObject() {
      return new LoadCase() {
        CaseType = LoadCase.CaseType,
        Name = LoadCase.Name
      };
    }

    public override string ToString() {
      if (LoadCase == null) {
        return $"LC{Id}";
      }

      return $"LC{Id} {LoadCase.Name} - {LoadCase.CaseType}";
    }
  }
}
