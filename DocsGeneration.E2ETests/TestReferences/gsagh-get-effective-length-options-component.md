# Get Effective Length Options
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Get Effective Length Options](./images/GetEffectiveLengthOptions.png) |

## Description

Get information of a 1D Member's Design Options for Effective Length, Restraints and Buckling Factors

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![EffectiveLengthOptionsParam](./images/EffectiveLengthOptionsParam.png) |[Effective Length Options](gsagh-effective-length-options-parameter.md) |**Effective Length Options** |1D Member Design Options for Effective Length, Restraints and Buckling Factors parameter |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![TextParam](./images/TextParam.png) |`Text` |**Calculation Option** |The option of the Effective Length calculation. |
|![TextParam](./images/TextParam.png) |`Text` |**Member Restraint Start** |Restraint Description Syntax for Member End Restraint at Member Start |
|![TextParam](./images/TextParam.png) |`Text` |**Member Restraint Start** |Restraint Description Syntax for Member End Restraint at Member End |
|![TextParam](./images/TextParam.png) |`Text` |**Restraint Along Member** |The internal continous restraint along the member. |
|![TextParam](./images/TextParam.png) |`Text` |**Intermediate Bracing Point Restraints** |The internal restraint at intermediate bracing points of the member. |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Effective Length About Y** |The user-defined effective length about y. |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Effective Length About Z** |The user-defined effective length about y. |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Effective Length LTB** |The user-defined effective length for lateral torsional buckling. |
|![NumberParam](./images/NumberParam.png) |`Number` |**Destabilising Load Height** |Destabilising Load Height in model units |
|![TextParam](./images/TextParam.png) |`Text` |**Load Position** |The destabilising load height is relative to this reference position. |
|![NumberParam](./images/NumberParam.png) |`Number` |**Factor Lsy** |Moment Amplification Factor, Strong Axis |
|![NumberParam](./images/NumberParam.png) |`Number` |**Factor Lsz** |Moment Amplification Factor, Weak Axis |
|![NumberParam](./images/NumberParam.png) |`Number` |**Equivalent uniform moment factor for LTB** |Override the automatically calculated factor to account for the shape of the moment diagram in lateral torsional buckling design equations. This override is applied for all bending segments in the member.  This override is applied to the following variable for each design code:<br /> AISC 360: C_b <br /> AS 4100: alpha_m <br /> BS 5950: m_LT <br /> CSA S16: omega_2 <br /> EN 1993-1-1 and EN 1993-1-2: C_1 <br /> Hong Kong Code of Practice: m_LT <br /> IS 800: C_mLT <br /> SANS 10162-1: omega_2 |


