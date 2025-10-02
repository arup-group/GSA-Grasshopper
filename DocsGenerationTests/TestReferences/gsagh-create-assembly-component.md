# Create Assembly
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Create Assembly](./images/CreateAssembly.png) |

## Description

Create a GSA Assembly

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![TextParam](./images/TextParam.png) |`Text` |**Name** |[Optional] Assembly Name |
|![ListParam](./images/ListParam.png) |[List](gsagh-list-parameter.md) |**List** |List of Assembly Entities (default: 'all')<br />Element/Member list should take the form:<br /> 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)<br />Refer to help file for definition of lists and full vocabulary. |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Topology 1** |Node at the start of the Assembly |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Topology 2** |Node at the end of the Assembly |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Orientation Node** |Assembly Orientation Node |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` |**Extents y** |[Optional] Extents of the Assembly in y-direction |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` |**Extents y** |[Optional] Extents of the Assembly in y-direction |
|![ListParam](./images/ListParam.png) |[List](gsagh-list-parameter.md) |**Internal Topology** | List of nodes that define the curve of the Assembly |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Curve Fit** |[Optional] Curve Fit for curved elements (default: 2)<br />Lagrange Interpolation (2) or Circular Arc (1) |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Number** |Number of points (default: 10) |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![AssemblyParam](./images/AssemblyParam.png) |[Assembly](gsagh-assembly-parameter.md) |**Assembly** |GSA Assembly parameter |
