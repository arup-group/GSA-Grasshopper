# Beam Strain Energy Density
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                              |
| -------------------------------------------------------------------  |
| ![Beam Strain Energy Density](./images/BeamStrainEnergyDensity.png)  |

## Description

Element1D Strain Energy Density result values

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                     | <img width="200"/> Type                    | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                                                                                                                                      |
| ------------------------------------------ | ------------------------------------------ | ------------------------------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  |
| ![ResultParam](./images/ResultParam.png)   | [Result](gsagh-result-parameter.md) _List_ | **Result**                     | Result                                                                                                                                                                                                                                               |
| ![GenericParam](./images/GenericParam.png) | `Generic`                                  | **Element filter list**        | Filter results by list (by default 'all')<br />Input a List or a text string taking the form:<br /> 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)<br />Refer to help file for definition of lists and full vocabulary.  |

### Output parameters

| <img width="20"/> Icon                | <img width="200"/> Type                                              | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                                                                                     |
| ------------------------------------- | -------------------------------------------------------------------- | ------------------------------ | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Energy Density` _Tree_ | **Strain energy density**      | \* Strain energy density. The strain energy density for a beam is a measure of how hard the beam is working. The average strain energy density is the average density along the element or member.  |

_\* DataTree organised as \{ `CaseID` ; `Permutation` ; `ElementID` \} fx. `{1;2;3}` is Case 1, Permutation 2, Element 3, where each branch contains a list of results per element position._
