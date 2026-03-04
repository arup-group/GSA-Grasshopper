# Get Model Geometry
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                               |
| ----------------------------------------------------  |
| ![Get Model Geometry](./images/GetModelGeometry.png)  |

## Description

Get nodes, elements, members and assemblies from GSA model

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                 | <img width="200"/> Type           | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                                                                                                                                                                                               |
| -------------------------------------- | --------------------------------- | ------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  |
| ![ModelParam](./images/ModelParam.png) | [Model](gsagh-model-parameter.md) | **GSA Model**                  | model containing some geometry                                                                                                                                                                                                                                                                                |
| ![ListParam](./images/ListParam.png)   | [List](gsagh-list-parameter.md)   | **Node filter list**           | Filter the Nodes by list. (by default 'all')<br />Node list should take the form:<br /> 1 11 to 72 step 2 not (XY3 31 to 45)<br />Refer to help file for definition of lists and full vocabulary.                                                                                                             |
| ![ListParam](./images/ListParam.png)   | [List](gsagh-list-parameter.md)   | **Element filter list**        | Filter the Elements by list. (by default 'all')<br />Element/Member list should take the form:<br /> 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)<br />Refer to help file for definition of lists and full vocabulary.<br />You can input a member list to get child elements.  |
| ![ListParam](./images/ListParam.png)   | [List](gsagh-list-parameter.md)   | **Member filter list**         | Filter import by list. (by default 'all')<br />Member list should take the form:<br /> 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (Z4 XY55)<br />Refer to help file for definition of lists and full vocabulary.                                                                                      |

### Output parameters

| <img width="20"/> Icon                         | <img width="200"/> Type                            | <img width="200"/> Name        | <img width="1000"/> Description                                        |
| ---------------------------------------------- | -------------------------------------------------- | ------------------------------ | ---------------------------------------------------------------------  |
| ![NodeParam](./images/NodeParam.png)           | [Node](gsagh-node-parameter.md) _List_             | **Nodes**                      | Nodes from GSA Model                                                   |
| ![Element1dParam](./images/Element1dParam.png) | [Element 1D](gsagh-element-1d-parameter.md) _List_ | **1D Elements**                | 1D Elements (Analysis Layer) from GSA Model imported to selected unit  |
| ![Element2dParam](./images/Element2dParam.png) | [Element 2D](gsagh-element-2d-parameter.md) _List_ | **2D Elements**                | 2D Elements (Analysis Layer) from GSA Model imported to selected unit  |
| ![Element3dParam](./images/Element3dParam.png) | [Element 3D](gsagh-element-3d-parameter.md) _List_ | **3D Elements**                | 3D Elements (Analysis Layer) from GSA Model imported to selected unit  |
| ![Member1dParam](./images/Member1dParam.png)   | [Member 1D](gsagh-member-1d-parameter.md) _List_   | **1D Members**                 | 1D Members (Design Layer) from GSA Model imported to selected unit     |
| ![Member2dParam](./images/Member2dParam.png)   | [Member 2D](gsagh-member-2d-parameter.md) _List_   | **2D Members**                 | 2D Members (Design Layer) from GSA Model imported to selected unit     |
| ![Member3dParam](./images/Member3dParam.png)   | [Member 3D](gsagh-member-3d-parameter.md) _List_   | **3D Members**                 | 3D Members (Design Layer) from GSA Model imported to selected unit     |
| ![AssemblyParam](./images/AssemblyParam.png)   | [Assembly](gsagh-assembly-parameter.md) _List_     | **Assemblies**                 | Assemblies from GSA Model                                              |
