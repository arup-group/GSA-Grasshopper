# Edit 3D Element
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Edit 3D Element](./images/Edit3dElement.png) |

## Description

Modify GSA 3D Element

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![Element3dParam](./images/Element3dParam.png) |[Element 3D](gsagh-element-3d-parameter.md) |**Element 3D** |3D Element(s) to get or set information for. |
|![IntegerParam](./images/IntegerParam.png) |`Integer` _List_ |**Number** |Set Element Number |
|![Property3dParam](./images/Property3dParam.png) |[Property 3D](gsagh-property-3d-parameter.md) _List_ |**3D Property** |Change 3D Property. Input either a 3D Property or an Integer to use a Property already defined in model |
|![IntegerParam](./images/IntegerParam.png) |`Integer` _List_ |**Element3d Group** |Set Element Group |
|![TextParam](./images/TextParam.png) |`Text` _List_ |**Element3d Name** |Set Name of Element |
|![ColourParam](./images/ColourParam.png) |`Colour` _List_ |**Element3d Colour** |Set Element Colour |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` _List_ |**Dummy Element** |Set Element to Dummy |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![Element3dParam](./images/Element3dParam.png) |[Element 3D](gsagh-element-3d-parameter.md) |**Element 3D** |GSA 3D Element(s) with applied changes. |
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


