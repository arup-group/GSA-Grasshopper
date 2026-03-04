# Steel Utilisations
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                |
| -----------------------------------------------------  |
| ![Steel Utilisations](./images/SteelUtilisations.png)  |

## Description

Steel Utilisation result values

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                   | <img width="200"/> Type                    | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                                                                                                           |
| ---------------------------------------- | ------------------------------------------ | ------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  |
| ![ResultParam](./images/ResultParam.png) | [Result](gsagh-result-parameter.md) _List_ | **Result**                     | Result                                                                                                                                                                                                                    |
| ![ListParam](./images/ListParam.png)     | [List](gsagh-list-parameter.md)            | **Member filter list**         | Filter import by list. (by default 'all')<br />Member list should take the form:<br /> 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (Z4 XY55)<br />Refer to help file for definition of lists and full vocabulary.  |

### Output parameters

| <img width="20"/> Icon                     | <img width="200"/> Type        | <img width="200"/> Name        | <img width="1000"/> Description               |
| ------------------------------------------ | ------------------------------ | ------------------------------ | --------------------------------------------  |
| ![GenericParam](./images/GenericParam.png) | `Generic` _List_               | **Overall**                    | Overall Utilisation ratio                     |
| ![GenericParam](./images/GenericParam.png) | `Generic` _List_               | **LocalCombined**              | Local Combined Utilisation ratio              |
| ![GenericParam](./images/GenericParam.png) | `Generic` _List_               | **BucklingCombined**           | Buckling Combined Utilisation ratio           |
| ![GenericParam](./images/GenericParam.png) | `Generic` _List_               | **LocalAxial**                 | Axial Utilisation ratio                       |
| ![GenericParam](./images/GenericParam.png) | `Generic` _List_               | **LocalShearU**                | Local Major Shear Utilisation ratio           |
| ![GenericParam](./images/GenericParam.png) | `Generic` _List_               | **LocalShearV**                | Local Minor Shear Utilisation ratio           |
| ![GenericParam](./images/GenericParam.png) | `Generic` _List_               | **LocalTorsion**               | Local Torsion Utilisation ratio               |
| ![GenericParam](./images/GenericParam.png) | `Generic` _List_               | **LocalMajorMoment**           | Local Major Moment Utilisation ratio          |
| ![GenericParam](./images/GenericParam.png) | `Generic` _List_               | **LocalMinorMoment**           | Local Minor Moment Utilisation ratio          |
| ![GenericParam](./images/GenericParam.png) | `Generic` _List_               | **MajorBuckling**              | Major Buckling Utilisation ratio              |
| ![GenericParam](./images/GenericParam.png) | `Generic` _List_               | **MinorBuckling**              | Minor Buckling Utilisation ratio              |
| ![GenericParam](./images/GenericParam.png) | `Generic` _List_               | **LateralTorsionalBuckling**   | Lateral Torsional Buckling Utilisation ratio  |
| ![GenericParam](./images/GenericParam.png) | `Generic` _List_               | **TorsionalBuckling**          | Torsional Buckling Utilisation ratio          |
| ![GenericParam](./images/GenericParam.png) | `Generic` _List_               | **FlexuralBuckling**           | Flexural Buckling Utilisation ratio           |
