using System;

namespace GsaGH.Parameters {
  public interface IGsaLoad {
    LoadType LoadType { get; }
    GsaList ReferenceList { get; }
    ReferenceType ReferenceType { get; }
    Guid RefObjectGuid { get; }
    GsaLoadCase LoadCase { get; }
    int CaseId { get; set; }
    string Name { get; set; }
    IGsaLoad Duplicate();
  }
}
