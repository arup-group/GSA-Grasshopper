# Grid Plane Surface
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![GridPlaneSurfaceParam](./images/GridPlaneSurfaceParam.png) |

## Description

A Grid Plane Surface is used by [Grid Loads](/references-theory/grid-loads.md).

A grid plane defines the geometry of a surface, and the load behaviour of the grid plane is defined by a grid surface.

In Grasshopper, a Grid Plane Surface contains both the information of what in GSA is known as a [Grid Plane](/references/hidr-data-grid-plane.md) and a [Grid Surface](/references/hidr-data-grid-surface.md)

The Grasshopper plugin will automatically create a fitting Grid Plane Surface when using the [Create Grid Point Load](gsagh-create-grid-point-load-component.md), [Create Grid Line Load](gsagh-create-grid-line-load-component.md) or [Create Grid Area Load](gsagh-create-grid-area-load-component.md) components. 

Grid Plane Surfaces can also be created independently using the [Create Grid Plane](gsagh-create-grid-plane-component.md) and [Create Grid Surface](gsagh-create-grid-surface-component.md) components.

## Properties

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![PlaneParam](./images/PlaneParam.png) |`Plane` |**Grid Plane** |Grid Plane (Axis + Elevation) |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Grid Plane ID** |Grid Plane ID |
|![TextParam](./images/TextParam.png) |`Text` |**Grid Plane Name** |Grid Plane Name |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**is Storey?** |Grid Plane is Storey type |
|![PlaneParam](./images/PlaneParam.png) |`Plane` |**Axis** |Grid Plane Axis as plane |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Axis ID** |Axis ID |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` |**Elevation** |Grid Plane Elevation |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` |**Grid Plane Tolerance Above** |Grid Plane Tolerance Above (for Storey Type) |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` |**Grid Plane Tolerance Below** |Grid Plane Tolerance Below (for Storey Type) |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Grid Surface ID** |Grid Surface ID |
|![TextParam](./images/TextParam.png) |`Text` |**Grid Surface Name** |Grid Surface Name |
|![TextParam](./images/TextParam.png) |`Text` |**Elements** |Elements that Grid Surface will try to expand load to |
|![TextParam](./images/TextParam.png) |`Text` |**Element Type** |Grid Surface Element Type |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` |**Grid Surface Tolerance** |Grid Surface Tolerance |
|![TextParam](./images/TextParam.png) |`Text` |**Span Type** |Grid Surface Span Type |
|![NumberParam](./images/NumberParam.png) |`Number` |**Span Direction** |Grid Surface Span Direction |
|![TextParam](./images/TextParam.png) |`Text` |**Expansion Type** |Grid Surface Expansion Type |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Simplified Tributary Area** |Grid Surface Simplified Tributary Area |

_Note: the above properties can be retrieved using the [Grid Plane Surface Properties](gsagh-grid-plane-surface-properties-component.md) component_
