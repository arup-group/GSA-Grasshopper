# Edit 1D Element
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Edit 1D Element](./images/Edit1dElement.png) |

## Description

Modify GSA 1D Element

_Note: This component can preview 3D Sections, right-click the middle of the component to toggle the section preview._

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![Element1dParam](./images/Element1dParam.png) |[Element 1D](gsagh-element-1d-parameter.md) |**Element 1D** |1D Element to get or set information for. Leave blank to create a new Element 1D |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Number** |Set Element Number. If ID is set it will replace any existing 1D Element in the model |
|![LineParam](./images/LineParam.png) |`Line` |**Line** |Reposition Element Line |
|![PropertyParam](./images/PropertyParam.png) |`Property` |**Section** |Set new Section or Spring Property |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Group** |Set Element Group |
|![TextParam](./images/TextParam.png) |`Text` |**Type** |Set Element Type<br />Accepted inputs are:<br />1: Bar<br />2: Beam<br />3: Spring<br />9: Link<br />10: Cable<br />19: Spacer<br />20: Strut<br />21: Tie<br />23: Rod<br />24: Damper |
|![OffsetParam](./images/OffsetParam.png) |[Offset](gsagh-offset-parameter.md) |**Offset** |Set Element Offset |
|![Bool6Param](./images/Bool6Param.png) |[Bool6](gsagh-bool6-parameter.md) |**Start release** |Set Release (Bool6) at Start of Element |
|![Bool6Param](./images/Bool6Param.png) |[Bool6](gsagh-bool6-parameter.md) |**End release** |Set Release (Bool6) at End of Element |
|![NumberParam](./images/NumberParam.png) |`Number` |**Orientation Angle** |Set Element Orientation Angle |
|![NodeParam](./images/NodeParam.png) |[Node](gsagh-node-parameter.md) |**Orientation Node** |Set Element Orientation Node |
|![TextParam](./images/TextParam.png) |`Text` |**Name** |Set Element Name |
|![ColourParam](./images/ColourParam.png) |`Colour` |**Colour** |Set Element Colour |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Dummy Element** |Set Element to Dummy |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![Element1dParam](./images/Element1dParam.png) |[Element 1D](gsagh-element-1d-parameter.md) |**Element 1D** |GSA 1D Element with applied changes. |
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
