using System;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters {
  public class GsaLoadCase {
    public int Id { get; private set; }
    public LoadCase LoadCase;
    private GsaLoadCase() { }
    public GsaLoadCase(int id) {
      Id = id;
    }
    public GsaLoadCase(int id, Enums.LoadCase.LoadCaseType type, string name) {
      LoadCase = new LoadCase() {
        CaseType = (LoadCaseType)Enum.Parse(typeof(LoadCaseType), type.ToString()),
        Name = name
      };
      Id = id;
    }
    internal GsaLoadCase(int id, ReadOnlyDictionary<int, LoadCase> loadCases) {
      Id = id;
      if (loadCases != null && loadCases.ContainsKey(Id)) {
        LoadCase = loadCases[id];
      }
    }

    public GsaLoadCase Duplicate() {
      return new GsaLoadCase() {
        LoadCase = new LoadCase() {
          CaseType = LoadCase.CaseType,
          Name = LoadCase.Name
        },
        Id = Id
      };
    }

    public override string ToString() {
      if (LoadCase == null) {
        return $"ID:{Id}";
      }

      return $"ID:{Id} {LoadCase.Name} - {LoadCase.CaseType}";
    }
  }
}
