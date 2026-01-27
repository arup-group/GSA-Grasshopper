# Preview Deformed 3D Sections
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                                  |
| -----------------------------------------------------------------------  |
| ![Preview Deformed 3D Sections](./images/PreviewDeformed3dSections.png)  |

## Description

Show the deformed 3D cross-section of 1D/2D GSA Elements and Members from a GSA Result.

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                   | <img width="200"/> Type             | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                                                                                                                                                  |
| ---------------------------------------- | ----------------------------------- | ------------------------------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  |
| ![ResultParam](./images/ResultParam.png) | [Result](gsagh-result-parameter.md) | **Result**                     | Result                                                                                                                                                                                                                                                           |
| ![ListParam](./images/ListParam.png)     | [List](gsagh-list-parameter.md)     | **Element/Member filter List** | Filter the Elements or Members by list. (by default 'all')<br />Element/Member list should take the form:<br /> 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)<br />Refer to help file for definition of lists and full vocabulary.  |

### Output parameters

| <img width="20"/> Icon               | <img width="200"/> Type        | <img width="200"/> Name        | <img width="1000"/> Description          |
| ------------------------------------ | ------------------------------ | ------------------------------ | ---------------------------------------  |
| ![MeshParam](./images/MeshParam.png) | `Mesh`                         | **Mesh**                       | Analysis layer 3D Section Mesh           |
| ![LineParam](./images/LineParam.png) | `Line` _List_                  | **Outlines**                   | The Analyis layer 3D Sections' outlines  |
