# Beam Displacements
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                |
| -----------------------------------------------------  |
| ![Beam Displacements](./images/BeamDisplacements.png)  |

## Description

Element1D Translation and Rotation result values

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                     | <img width="200"/> Type                    | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                                                                                                                                                  |
| ------------------------------------------ | ------------------------------------------ | ------------------------------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  |
| ![ResultParam](./images/ResultParam.png)   | [Result](gsagh-result-parameter.md) _List_ | **Result**                     | Result                                                                                                                                                                                                                                                           |
| ![ListParam](./images/ListParam.png)       | [List](gsagh-list-parameter.md)            | **Element/Member filter List** | Filter the Elements or Members by list. (by default 'all')<br />Element/Member list should take the form:<br /> 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)<br />Refer to help file for definition of lists and full vocabulary.  |
| ![IntegerParam](./images/IntegerParam.png) | `Integer`                                  | **Intermediate Points**        | Number of intermediate equidistant points (default 3)                                                                                                                                                                                                            |
| ![IntegerParam](./images/IntegerParam.png) | `Integer`                                  | **Axis**                       | Standard Axis: Global (0), Local (-1), Natural (-2), Default (-10), XElevation (-11), YElevation (-12), GlobalCylindrical (-13), Vertical (-14)                                                                                                                  |

### Output parameters

| <img width="20"/> Icon                | <img width="200"/> Type                                      | <img width="200"/> Name          | <img width="1000"/> Description          |
| ------------------------------------- | ------------------------------------------------------------ | -------------------------------- | ---------------------------------------  |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Length` _Tree_ | **Translations X**               | \* Translation in X-direction            |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Length` _Tree_ | **Translations Y**               | \* Translation in Y-direction            |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Length` _Tree_ | **Translations Z**               | \* Translation in Z-direction            |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Length` _Tree_ | **Translations &#124;XYZ&#124;** | \* Combined &#124;XYZ&#124; Translation  |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Angle` _Tree_  | **Rotation XX**                  | \* Rotation around X-axis                |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Angle` _Tree_  | **Rotation YY**                  | \* Rotation around Y-axis                |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Angle` _Tree_  | **Rotation ZZ**                  | \* Rotation around Z-axis                |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Angle` _Tree_  | **Rotation &#124;XYZ&#124;**     | \* Combined &#124;XXYYZZ&#124; Rotation  |

_\* DataTree organised as \{ `CaseID` ; `Permutation` ; `ElementID` \} fx. `{1;2;3}` is Case 1, Permutation 2, Element 3, where each branch contains a list of results per element position._
