# Element 2D
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Element2dParam](./images/Element2dParam.png) |

## Description

Elements in GSA are geometrical objects used for Analysis. Elements must be split at intersections with other elements to connect to each other or 'node out'.

In Grasshopper, an Element2D parameter is a collection of 2D Elements (mesh faces representing [Quad or Triangle Elements](/references/element-types.md#quad-and-triangle-elements)) used for FE analysis. In GSA a 2D element is just a single face, but for Rhino performance reasons we have made the Element2D parameter a mesh that can contain more than one Element/Face.

Refer to [Elements](/references/hidr-data-element.md) to read more.



## Properties

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![IntegerParam](./images/IntegerParam.png) |`Integer` _List_ |**Number** |Element Number |
|![IGeometricParam](./images/IGeometricParam.png) |`Geometry` |**Geometry** |analysis mesh for FE element and polyline for load panel |
|![Property2dParam](./images/Property2dParam.png) |[Property 2D](gsagh-property-2d-parameter.md) _List_ |**2D Property** |2D Property. Input either a GSA 2D Property or an Integer to use a Property already defined in model |
|![IntegerParam](./images/IntegerParam.png) |`Integer` _List_ |**Group** |Element Group |
|![TextParam](./images/TextParam.png) |`Text` _List_ |**Element Type** |Element 2D Type.<br />Type can not be set; it is either Tri3 or Quad4<br />depending on Rhino/Grasshopper mesh face type |
|![OffsetParam](./images/OffsetParam.png) |[Offset](gsagh-offset-parameter.md) _List_ |**Offset** |Element Offset |
|![NumberParam](./images/NumberParam.png) |`Number` _List_ |**Orientation Angle** |Element Orientation Angle in radians |
|![TextParam](./images/TextParam.png) |`Text` _List_ |**Name** |Set Element Name |
|![ColourParam](./images/ColourParam.png) |`Colour` _List_ |**Colour** |Element Colour |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` _List_ |**Dummy Element** |if Element is Dummy |
|![IntegerParam](./images/IntegerParam.png) |`Integer` _List_ |**Parent Members** |Parent Member IDs in Model that Element was created from |
|![IntegerParam](./images/IntegerParam.png) |`Integer` _List_ |**Topology** |the Element's original topology list referencing node IDs in Model that Element was created from |

_Note: the above properties can be retrieved using the [Edit 2D Element](gsagh-edit-2d-element-component.md) component_
