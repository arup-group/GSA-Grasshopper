# Element 1D
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Element1dParam](./images/Element1dParam.png) |

## Description

Elements in GSA are geometrical objects used for Analysis. Elements must be split at intersections with other elements to connect to each other or 'node out'. 

Element1Ds are one-dimensional stick elements (representing [1D Element Types](/references/element-types.md#element-types)) used by the solver for analysis.

Refer to [Elements](/references/hidr-data-element.md) to read more.



## Properties

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Number** |Element Number. If ID is set it will replace any existing 1D Element in the model |
|![LineParam](./images/LineParam.png) |`Line` |**Line** |Element Line |
|![PropertyParam](./images/PropertyParam.png) |`Property` |**Property** |GSA Section Property (Beam) or Spring Property parameter |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Group** |Element Group |
|![TextParam](./images/TextParam.png) |`Text` |**Type** |Element Type |
|![OffsetParam](./images/OffsetParam.png) |[Offset](gsagh-offset-parameter.md) |**Offset** |Element Offset |
|![Bool6Param](./images/Bool6Param.png) |[Bool6](gsagh-bool6-parameter.md) |**Start release** |Release (Bool6) at Start of Element |
|![Bool6Param](./images/Bool6Param.png) |[Bool6](gsagh-bool6-parameter.md) |**End release** |Release (Bool6) at End of Element |
|![NumberParam](./images/NumberParam.png) |`Number` |**Orientation Angle** |Element Orientation Angle |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Orientation Node** |Element Orientation Node |
|![TextParam](./images/TextParam.png) |`Text` |**Name** |Element Name |
|![ColourParam](./images/ColourParam.png) |`Colour` |**Colour** |Element Colour |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Dummy Element** |if Element is Dummy |
|![IntegerParam](./images/IntegerParam.png) |`Integer` _List_ |**Parent Members** |Parent Member IDs in Model that Element was created from |
|![IntegerParam](./images/IntegerParam.png) |`Integer` _Tree_ |**Topology** |the Element's original topology list referencing node IDs in Model that Element was created from |

_Note: the above properties can be retrieved using the [Edit 1D Element](gsagh-edit-1d-element-component.md) component_
