# Create Grid Point Load
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Create Grid Point Load](./images/CreateGridPointLoad.png) |

## Description

Create GSA Grid Point Load

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![LoadCaseParam](./images/LoadCaseParam.png) |[Load Case](gsagh-load-case-parameter.md) |**Load Case** |Load Case parameter |
|![PointParam](./images/PointParam.png) |`Point` |**Point** |Point. If you input grid plane below only x and y coordinates will be used from this point, but if not a new Grid Plane Surface (xy-plane) will be created at the z-elevation of this point. |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Grid Plane Surface** |Grid Plane Surface or Plane [Optional]. If no input here then the point's z-coordinate will be used for an xy-plane at that elevation. |
|![TextParam](./images/TextParam.png) |`Text` |**Direction** |Load direction (default z).<br />Accepted inputs are:<br />x<br />y<br />z |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Axis** |Load axis (default Global). <br />Accepted inputs are:<br />0 : Global<br />-1 : Local |
|![TextParam](./images/TextParam.png) |`Text` |**Name** |Load Name |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force ` |**Value** |Load Value |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![LoadParam](./images/LoadParam.png) |[Load](gsagh-load-parameter.md) |**Grid Point Load** |GSA Grid Point Load |
