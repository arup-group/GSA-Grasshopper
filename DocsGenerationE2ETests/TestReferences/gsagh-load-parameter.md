# Load
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![LoadParam](./images/LoadParam.png) |

## Description

A Load parameter can contain [Node Loads](/references/nodalloading-data.md), [Beam Loads](/references/beamloading-data.md), [2D Element Loads](/references/2delementloading-data.md), [Grid Loads](/references/gridloading-data.md), and [Gravity Loads](/references/hidr-data-gravity.md), 

GSA provides a number of different ways to apply loads to a model.

The simplest option is use the [Create Node Load](gsagh-create-node-load-component.md) component to create nodal loading where forces are applied directly to nodes. This is not recommended for 2D and 3D elements. 

The next level of loading applies loads to the elements, either use the [Create Beam Load](gsagh-create-beam-load-component.md) component, or [Create Face Load](gsagh-create-face-load-component.md) component. In the solver these use shape functions to give loading on the nodes compatible with the elements to which the load is applied. 

Grid loading is a different type of loading which is applied to a [Grid Plane Surface](gsagh-grid-plane-surface-parameter.md). An algorithm then distributes this loading from the grid surface to the surrounding elements. This can be useful for models where floor slabs are not modelled explicitly. 

Gravity is the final load type create with the [Create Gravity Load](gsagh-create-gravity-load-component.md) component. This is different from the other load types as it is specified as an acceleration (in g). This is normally used to model the dead weight of the structure by specifying a gravity load of −1 × g in the z direction.



## Properties

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![LoadCaseParam](./images/LoadCaseParam.png) |[Load Case](gsagh-load-case-parameter.md) |**Load Case** |GSA Load Case parameter |
|![TextParam](./images/TextParam.png) |`Text` |**Name** |Load name |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Definition** |Node, Element or Member list that load is applied to or Grid point / polygon definition |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Axis** |Axis Property (0 : Global // -1 : Local |
|![TextParam](./images/TextParam.png) |`Text` |**Direction** |Load direction |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Projected** |Projected |
|![LoadParam](./images/LoadParam.png) |`Load   Value  or   Factor   X  [k N , k N /m , k N /m ²]` |**Load Value or Factor X** |Value at Start, Point 1 or Factor X.
Expression for Face Equation load. |
|![LoadParam](./images/LoadParam.png) |`Load   Value  or   Factor   Y  [k N , k N /m , k N /m ²]` |**Load Value or Factor Y** |Value at End, Point 2 or Factor Y.
Position X for Face Point load.
Equation Axis for Face Equation load. |
|![LoadParam](./images/LoadParam.png) |`Load   Value  or   Factor   Z  [k N , k N /m , k N /m ²]` |**Load Value or Factor Z** |Value at Point 3 or Factor Z.
Position Y for Face Point load.
Is Constant for Face Equation load. |
|![LoadParam](./images/LoadParam.png) |`Load   Value  [k N , k N /m , k N /m ²]` |**Load Value** |Value at Point 4.
Units of the equation for Face Equation load |
|![GridPlaneSurfaceParam](./images/GridPlaneSurfaceParam.png) |[Grid Plane Surface](gsagh-grid-plane-surface-parameter.md) |**Grid Plane Surface** |GSA Grid Plane Surface parameter |

_Note: the above properties can be retrieved using the [Load Properties](gsagh-load-properties-component.md) component_
