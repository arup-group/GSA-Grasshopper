# Element 3D
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Element3dParam](./images/Element3dParam.png) |

## Description

Elements in GSA are geometrical objects used for Analysis. Elements must be split at intersections with other elements to connect to each other or 'node out'. 

In Grasshopper, an Element3D is a collection of 3D Elements (mesh solids representing [Brick, Wedge, Pyramid or Tetra Elements](/references/element-types.md#brick-wedge-pyramid-and-tetra-elements)) used for FE analysis. In GSA, a 3D Element is just a single closed mesh, but for Rhino performance reasons we have made Element3D an [Ngon Mesh](https://docs.mcneel.com/rhino/7/help/en-us/popup_moreinformation/ngon.htm) that can contain more than one closed mesh.

Refer to [Elements](/references/hidr-data-element.md) to read more.

## Properties

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![IntegerParam](./images/IntegerParam.png) |`Integer` _List_ |**Number** |Element Number |
|![MeshParam](./images/MeshParam.png) |`Mesh` _List_ |**Analysis Mesh** |Analysis Mesh. <br />This will export a list of solid meshes representing each 3D element.<br />To get a combined mesh connect a GSA Element 3D to normal Mesh Parameter component to convert on the fly |
|![Property3dParam](./images/Property3dParam.png) |[Property 3D](gsagh-property-3d-parameter.md) _List_ |**3D Property** |3D Property. Either a GSA 3D Property or an Integer representing a Property already defined in model |
|![IntegerParam](./images/IntegerParam.png) |`Integer` _List_ |**Group** |Element Group |
|![TextParam](./images/TextParam.png) |`Text` _List_ |**Element Type** |Element 3D Type.<br />Type can not be set; it is either Tetra4, Pyramid5, Wedge6 or Brick8 |
|![TextParam](./images/TextParam.png) |`Text` _List_ |**Name** |Set Element Name |
|![ColourParam](./images/ColourParam.png) |`Colour` _List_ |**Colour** |Element Colour |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` _List_ |**Dummy Element** |if Element is Dummy |
|![IntegerParam](./images/IntegerParam.png) |`Integer` _List_ |**Parent Members** |Parent Member IDs in Model that Element was created from |
|![IntegerParam](./images/IntegerParam.png) |`Integer` _List_ |**Topology** |the Element's original topology list referencing node IDs in Model that Element was created from |

_Note: the above properties can be retrieved using the [Edit 3D Element](gsagh-edit-3d-element-component.md) component_
