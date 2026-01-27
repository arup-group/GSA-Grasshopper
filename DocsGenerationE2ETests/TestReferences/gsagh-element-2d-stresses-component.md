# Element 2D Stresses
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                 |
| ------------------------------------------------------  |
| ![Element 2D Stresses](./images/Element2dStresses.png)  |

## Description

2D Projected Stress result values

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                     | <img width="200"/> Type                    | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                                                                                                                                                  |
| ------------------------------------------ | ------------------------------------------ | ------------------------------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  |
| ![ResultParam](./images/ResultParam.png)   | [Result](gsagh-result-parameter.md) _List_ | **Result**                     | Result                                                                                                                                                                                                                                                           |
| ![ListParam](./images/ListParam.png)       | [List](gsagh-list-parameter.md)            | **Element/Member filter List** | Filter the Elements or Members by list. (by default 'all')<br />Element/Member list should take the form:<br /> 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)<br />Refer to help file for definition of lists and full vocabulary.  |
| ![IntegerParam](./images/IntegerParam.png) | `Integer`                                  | **Stress Layer**               | Layer within the cross-section to get results.<br />Input an integer between -1 and 1, representing the normalised thickness,<br />default value is zero => middle of the element.                                                                               |
| ![IntegerParam](./images/IntegerParam.png) | `Integer`                                  | **Axis**                       | Standard Axis: Global (0), Local (-1), Natural (-2), Default (-10), XElevation (-11), YElevation (-12), GlobalCylindrical (-13), Vertical (-14)                                                                                                                  |

### Output parameters

| <img width="20"/> Icon                | <img width="200"/> Type                                        | <img width="200"/> Name        | <img width="1000"/> Description |
| ------------------------------------- | -------------------------------------------------------------- | ------------------------------ | ------------------------------  |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Pressure` _Tree_ | **Stress XX**                  | \* Stress in XX-direction       |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Pressure` _Tree_ | **Stress YY**                  | \* Stress in YY-direction       |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Pressure` _Tree_ | **Stress ZZ**                  | \* Stress in ZZ-direction       |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Pressure` _Tree_ | **Stress XY**                  | \* Stress in XY-direction       |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Pressure` _Tree_ | **Stress YZ**                  | \* Stress in YZ-direction       |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Pressure` _Tree_ | **Stress ZX**                  | \* Stress in ZX-direction       |

_\* DataTree organised as \{ `CaseID` ; `Permutation` ; `ElementID` \} fx. `{1;2;3}` is Case 1, Permutation 2, Element 3, where each branch contains a list of results in the following order: `Vertex(1)`, `Vertex(2)`, ..., `Vertex(i)`, `Centre` +ve in-plane stresses: tensile(ie. + ve direct strain). +ve bending stress gives rise to tension on the top surface. +ve shear stresses: +ve shear strain._
