# Create Grid Plane
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Create Grid Plane](./images/CreateGridPlane.png) |

## Description

Create GSA Grid Plane

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Plane** |Plane for Axis and Grid Plane definition. Note that an XY-plane will be created with an axis origin Z = 0 and the height location will be controlled by Grid Plane elevation. For all none-XY plane inputs, the Grid Plane elevation will be 0 |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Grid Plane ID** |Grid Plane ID. Setting this will replace any existing Grid Planes in model |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Grid Elevation in model units** |Grid Elevation [Optional]. Note that this value will be added to Plane origin location in the plane's normal axis direction. |
|![TextParam](./images/TextParam.png) |`Text` |**Name** |Grid Plane Name |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![GridPlaneSurfaceParam](./images/GridPlaneSurfaceParam.png) |[Grid Plane Surface](gsagh-grid-plane-surface-parameter.md) |**Grid Plane** |GSA Grid Plane |


