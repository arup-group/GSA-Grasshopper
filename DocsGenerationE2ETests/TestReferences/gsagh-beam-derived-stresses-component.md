# Beam Derived Stresses
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                     |
| ----------------------------------------------------------  |
| ![Beam Derived Stresses](./images/BeamDerivedStresses.png)  |

## Description

Element1D Derived Stress results like von Mises

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                     | <img width="200"/> Type                    | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                                                                                                                                                  |
| ------------------------------------------ | ------------------------------------------ | ------------------------------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  |
| ![ResultParam](./images/ResultParam.png)   | [Result](gsagh-result-parameter.md) _List_ | **Result**                     | Result                                                                                                                                                                                                                                                           |
| ![ListParam](./images/ListParam.png)       | [List](gsagh-list-parameter.md)            | **Element/Member filter List** | Filter the Elements or Members by list. (by default 'all')<br />Element/Member list should take the form:<br /> 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)<br />Refer to help file for definition of lists and full vocabulary.  |
| ![IntegerParam](./images/IntegerParam.png) | `Integer`                                  | **Intermediate Points**        | Number of intermediate equidistant points (default 3)                                                                                                                                                                                                            |

### Output parameters

| <img width="20"/> Icon                | <img width="200"/> Type                                        | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                                                                                                                                                     |
| ------------------------------------- | -------------------------------------------------------------- | ------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Pressure` _Tree_ | **Elastic Shear Y**            | The maximum elastic shear stresses in local Y-axis
+ve stresses: tensile                                                                                                                                                                                            |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Pressure` _Tree_ | **Elastic Shear Z**            | The maximum elastic shear stresses in local Z-axis
+ve stresses: tensile                                                                                                                                                                                            |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Pressure` _Tree_ | **Torsional**                  | Torsional stress: St = Mxx/Ct 
where Ct is the ‘torsion modulus’. (Refer to the GSA manual for details.)                                                                                                                                                            |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Pressure` _Tree_ | **von Mises**                  | Von Mises stress: This is calculated assuming the maximum through thickness stress and torsional stress coexist. 
In most cases this is an over-estimate of the von Mises stress. 
Von Mises stress is not calculated for cases with enveloping operators in them.  |
