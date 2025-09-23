# Result
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![ResultParam](./images/ResultParam.png) |

## Description

A Result is used to select Cases from an analysed [Model](gsagh-model-parameter.md) and extract the values for post-processing or visualisation.

The following result types can be extracted if they are present in the model: 
- [Node Results](/references/dotnet-api/result-classes.md#noderesult): `Displacement` and `Reaction`.
- [1D Element Results](/references/dotnet-api/result-classes.md#element1dresult): `Displacement`, `Force` and `StrainEnergyDensity`.
- [2D Element Results](/references/dotnet-api/result-classes.md#element2dresult): `Displacement`, `Force`, `Moment`, `Shear` and `Stress`.
- [3D Element Results](/references/dotnet-api/result-classes.md#element3dresult): `Displacement` and `Stress`.
- [Global Results](/references/dotnet-api/result-classes.md#globalresult): `Frequency`, `LoadFactor`, `ModalGeometricStiffness`, `ModalMass`, `ModalStiffness`, `TotalLoad`, `TotalReaction`, `Mode`, `EffectiveInertia`, `EffectiveMass` and `Eigenvalue`.

All result values from the [.NET API](/references/dotnet-api/introduction.md) have been wrapped in [Unit Number](/references/gsagh/gsagh-unitnumber-parameter.md) and can be converted into different measures as you work. The Result parameter caches the result values.


