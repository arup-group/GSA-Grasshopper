# Get Assembly
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                    |
| -----------------------------------------  |
| ![Get Assembly](./images/GetAssembly.png)  |

## Description

Get GSA Assembly

### Input parameters

| <img width="20"/> Icon                       | <img width="200"/> Type                 | <img width="200"/> Name        | <img width="1000"/> Description           |
| -------------------------------------------- | --------------------------------------- | ------------------------------ | ----------------------------------------  |
| ![AssemblyParam](./images/AssemblyParam.png) | [Assembly](gsagh-assembly-parameter.md) | **Assembly**                   | Assembly to get information for.Assembly  |

### Output parameters

| <img width="20"/> Icon                     | <img width="200"/> Type                               | <img width="200"/> Name        | <img width="1000"/> Description                                                    |
| ------------------------------------------ | ----------------------------------------------------- | ------------------------------ | ---------------------------------------------------------------------------------  |
| ![TextParam](./images/TextParam.png)       | `Text`                                                | **Name**                       | Assembly Name                                                                      |
| ![TextParam](./images/TextParam.png)       | `Text`                                                | **Assembly type**              | Assembly type                                                                      |
| ![GenericParam](./images/GenericParam.png) | `Generic`                                             | **List**                       | Assembly Entities                                                                  |
| ![IntegerParam](./images/IntegerParam.png) | `Integer`                                             | **Topology 1**                 | Node at the start of the Assembly                                                  |
| ![IntegerParam](./images/IntegerParam.png) | `Integer`                                             | **Topology 2**                 | Node at the end of the Assembly                                                    |
| ![IntegerParam](./images/IntegerParam.png) | `Integer`                                             | **Orientation Node**           | Assembly Orientation Node                                                          |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Length` | **Extents y**                  | Extents of the Assembly in y-direction                                             |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Length` | **Extents z**                  | Extents of the Assembly in z-direction                                             |
| ![IntegerParam](./images/IntegerParam.png) | `Integer` _List_                                      | **Internal Topology**          | List of nodes that define the curve of the Assembly                                |
| ![IntegerParam](./images/IntegerParam.png) | `Integer`                                             | **Curve Fit**                  | Curve Fit for curved elements<br />Lagrange Interpolation (2) or Circular Arc (1)  |
| ![GenericParam](./images/GenericParam.png) | `Generic`                                             | **Definition**                 | Assembly definition                                                                |
