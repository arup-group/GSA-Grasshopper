using System;

namespace GsaGH.Parameters {
  public interface IGsaLoad {

    LoadType LoadType { get; }
    GsaList ReferenceList { get; }
    ReferenceType ReferenceType { get; }
    Guid RefObjectGuid { get; }
    GsaLoadCase LoadCase { get; }
    int CaseId();
    IGsaLoad Duplicate();
  }
}
