# Create Effective Length Options
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                                        |
| -----------------------------------------------------------------------------  |
| ![Create Effective Length Options](./images/CreateEffectiveLengthOptions.png)  |

## Description

Create 1D Member Design Options for Effective Length, Restraints and Buckling Factors

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                   | <img width="200"/> Type        | <img width="200"/> Name                      | <img width="1000"/> Description                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                |
| ---------------------------------------- | ------------------------------ | -------------------------------------------- | -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  |
| ![TextParam](./images/TextParam.png)     | `Text`                         | **Member Restraint Start**                   | Set the Member's Start Restraint
Use either shortcut names ('Pinned', 'Fixed', 'Free',
'FullRotational', 'PartialRotational' or 'TopFlangeLateral')
or the Restraint Description Syntax:

Parts of the description are separated by spaces or commas.
The parts that can be restrained are:
F1, F2: flanges
with suffices:
    L: lateral
    W: full warping
    P: part warping
T: torsion
with suffices
    R: full restraint
    P: part restraint
    F: friction only
MAJ, MIN major and minor axis bending and shear
with suffices
    R: full rotational restraint
    P: part rotational restraint
    V: Translational restraint
Examples:
    F1LP, TP, MAJRV:	Lateral and partial warping restraint to flange 1	    F12W:		Flanges 1&2 full warping restraint...
  |
| ![TextParam](./images/TextParam.png)     | `Text`                         | **Member Restraint End**                     | Set the Member's End Restraint.
Use either shortcut names ('Pinned', 'Fixed', 'Free',
'FullRotational', 'PartialRotational' or 'TopFlangeLateral')
or the Restraint Description Syntax:

Parts of the description are separated by spaces or commas.
The parts that can be restrained are:
F1, F2: flanges
with suffices:
    L: lateral
    W: full warping
    P: part warping
T: torsion
with suffices
    R: full restraint
    P: part restraint
    F: friction only
MAJ, MIN major and minor axis bending and shear
with suffices
    R: full rotational restraint
    P: part rotational restraint
    V: Translational restraint
Examples:
    F1LP, TP, MAJRV:	Lateral and partial warping restraint to flange 1	    F12W:		Flanges 1&2 full warping restraint...
   |
| ![NumberParam](./images/NumberParam.png) | `Number`                       | **Destabilising Load Height**                | Destabilising Load Height in model units                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       |
| ![NumberParam](./images/NumberParam.png) | `Number`                       | **Factor Lsy**                               | Moment Amplification Factor, Strong Axis                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       |
| ![NumberParam](./images/NumberParam.png) | `Number`                       | **Factor Lsz**                               | Moment Amplification Factor, Weak Axis                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| ![NumberParam](./images/NumberParam.png) | `Number`                       | **Equivalent uniform moment factor for LTB** | Override the automatically calculated factor to account for the shape of the moment diagram in lateral torsional buckling design equations. This override is applied for all bending segments in the member.  This override is applied to the following variable for each design code:<br /> AISC 360: C_b <br /> AS 4100: alpha_m <br /> BS 5950: m_LT <br /> CSA S16: omega_2 <br /> EN 1993-1-1 and EN 1993-1-2: C_1 <br /> Hong Kong Code of Practice: m_LT <br /> IS 800: C_mLT <br /> SANS 10162-1: omega_2                                                                                                                                                                                                                                                              |

### Output parameters

| <img width="20"/> Icon                                                   | <img width="200"/> Type                                                 | <img width="200"/> Name        | <img width="1000"/> Description                                                               |
| ------------------------------------------------------------------------ | ----------------------------------------------------------------------- | ------------------------------ | --------------------------------------------------------------------------------------------  |
| ![EffectiveLengthOptionsParam](./images/EffectiveLengthOptionsParam.png) | [Effective Length Options](gsagh-effective-length-options-parameter.md) | **Effective Length Options**   | GSA 1D Member Design Options for Effective Length, Restraints and Buckling Factors parameter  |
