﻿namespace GsaGH.Parameters {
  public enum ApiDiagramType {
    AxialForceFx,
    ShearForceFy,
    ShearForceFz,
    TorsionMxx,
    MomentMyy,
    MomentMzz,
    ResolvedShearFyz,
    ResolvedMomentMyz,
    AxialStressA,
    ShearStressSy,
    ShearStressSz,
    BendingStressByPositiveZ,
    BendingStressByNegativeZ,
    BendingStressBzPositiveY,
    BendingStressBzNegativeY,
    CombinedStressC1,
    CombinedStressC2,
    LoadGridPoint,
    LoadGridLine,
    LoadGridArea,
    LoadNodalForce,
    LoadNodalMoment,
    LoadNodalDisplacementTranslation,
    LoadNodalDisplacementRotation,
    LoadNodalSettlementTranslation,
    LoadNodalSettlementRotation,
    Load1dPointForce,
    Load1dPointMoment,
    Load1dPatchForce,
    Load1dPatchMoment,
    Load1dPrestressForce,
    Load1dPrestressMoment,
    Load1dInitialStrain,
    Load1dLackOfFit,
    Load1dDistortionTranslation,
    Load1dDistortionRotation,
    Load1dThermalUniform,
    Load1dThermalGradient,
    Load2dFacePointForce,
    Load2dFacePressure,
    Load2dEdgePressure,
    Load2dPreStressPointForce,
    Load2dPreStressMoment,
    Load2dStrain,
    Load2dThermalUniform,
    Load2dThermalGradient,
    Load3dFacePressure,
    Load3dThermalUniform,
    Load3dThermalGradient,
    TranslationUx,
    TranslationUy,
    TranslationUz,
    ResolvedTranslationU,
    RotationRxx,
    RotationRyy,
    RotationRzz,
    ResolvedRotationR,
  }
}
