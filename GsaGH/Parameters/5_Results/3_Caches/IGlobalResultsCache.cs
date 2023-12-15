using OasysUnits;

namespace GsaGH.Parameters.Results {
  public interface IGlobalResultsCache : IResultItem  {
    IApiResult ApiResult { get; }
    IEffectiveInertia EffectiveInertia { get; }
    IEffectiveMass EffectiveMass { get; }
    double? Eigenvalue { get; }
    Frequency Frequency { get; }
    Ratio LoadFactor { get; }
    ForcePerLength ModalGeometricStiffness { get; }
    Mass ModalMass { get; }
    ForcePerLength ModalStiffness { get; }
    int? Mode { get; }
    IReactionForce TotalLoad { get; }
    IReactionForce TotalReaction { get; }
  }
}
