namespace GsaGH.Parameters {
  public enum AnalysisTaskType {
    Static = 1,
    Buckling = 3,
    StaticPDelta = 4,
    NonlinearStatic = 8,
    ModalDynamic = 2,
    ModalPDelta = 5,
    Ritz = 32,
    RitzPDelta = 33,
    ResponseSpectrum = 6,
    PseudoResponseSpectrum = 42,
    LinearTimeHistory = 15,
    Harmonic = 14,
    Footfall = 34,
    Periodic = 35,
    FormFinding = 9,
    Envelope = 37,
    ModelStability = 39,
    ModelStabilityPDelta = 40,
  }
}
