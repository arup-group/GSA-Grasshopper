# Edit 1D Member
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Edit 1D Member](./images/Edit1dMember.png) |

## Description

Modify GSA 1D Member

_Note: This component can preview 3D Sections, right-click the middle of the component to toggle the section preview._

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![Member1dParam](./images/Member1dParam.png) |[Member 1D](gsagh-member-1d-parameter.md) |**Member 1D** |1D Member to get or set information for. Leave blank to create a new Member 1D |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Member1d Number** |Set Member Number. If ID is set it will replace any existing 1D Member in the model. |
|![CurveParam](./images/CurveParam.png) |`Curve` |**Curve** |Member Curve |
|![PropertyParam](./images/PropertyParam.png) |`Property` |**Section** |Set new Section or Spring Property. |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Member1d Group** |Set Member 1D Group |
|![TextParam](./images/TextParam.png) |`Text` |**Member Type** |Set 1D Member Type<br />Default is 0: Generic 1D - Accepted inputs are:<br />2: Beam<br />3: Column<br />6: Cantilever<br />8: Compos<br />9: Pile<br />11: Void cutter |
|![TextParam](./images/TextParam.png) |`Text` |**1D Element Type** |Set Element 1D Type<br />Accepted inputs are:<br />1: Bar<br />2: Beam<br />3: Spring<br />9: Link<br />10: Cable<br />19: Spacer<br />20: Strut<br />21: Tie<br />23: Rod<br />24: Damper |
|![OffsetParam](./images/OffsetParam.png) |[Offset](gsagh-offset-parameter.md) |**Offset** |Set Member Offset |
|![Bool6Param](./images/Bool6Param.png) |[Bool6](gsagh-bool6-parameter.md) |**Start release** |Set Release (Bool6) at Start of Member |
|![Bool6Param](./images/Bool6Param.png) |[Bool6](gsagh-bool6-parameter.md) |**End release** |Set Release (Bool6) at End of Member |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Automatic Offset End 1** |Set Automatic Offset at End 1 of Member |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Automatic Offset End 2** |Set Automatic Offset at End 2 of Member |
|![NumberParam](./images/NumberParam.png) |`Number` |**Orientation Angle** |Set Member Orientation Angle |
|![NodeParam](./images/NodeParam.png) |[Node](gsagh-node-parameter.md) |**Orientation Node** |Set Member Orientation Node |
|![NumberParam](./images/NumberParam.png) |`Number` |**Mesh Size in model units** |Set Member Mesh Size |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Mesh With Others** |Mesh with others? |
|![EffectiveLengthOptionsParam](./images/EffectiveLengthOptionsParam.png) |[Effective Length Options](gsagh-effective-length-options-parameter.md) |**Set Effective Length Options** |1D Member Design Options for Effective Length, Restraints and Buckling Factors |
|![TextParam](./images/TextParam.png) |`Text` |**Member1d Name** |Set Name of Member1d |
|![ColourParam](./images/ColourParam.png) |`Colour` |**Member1d Colour** |Set Member 1D Colour |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Dummy Member** |Set Member to Dummy |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![Member1dParam](./images/Member1dParam.png) |[Member 1D](gsagh-member-1d-parameter.md) |**Member 1D** |GSA 1D Member with applied changes. |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Member1d Number** |Member Number |
|![CurveParam](./images/CurveParam.png) |`Curve` |**Curve** |Member Curve |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Section** |Section or Spring Property |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Member Group** |Member Group |
|![TextParam](./images/TextParam.png) |`Text` |**Member Type** |1D Member Type |
|![TextParam](./images/TextParam.png) |`Text` |**1D Element Type** |Element 1D Type |
|![OffsetParam](./images/OffsetParam.png) |[Offset](gsagh-offset-parameter.md) |**Offset** |Member Offset |
|![Bool6Param](./images/Bool6Param.png) |[Bool6](gsagh-bool6-parameter.md) |**Start release** |Release (Bool6) at Start of Member |
|![Bool6Param](./images/Bool6Param.png) |[Bool6](gsagh-bool6-parameter.md) |**End release** |Release (Bool6) at End of Member |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Automatic Offset End 1** |Automatic Offset at End 1 of Member |
|![NumberParam](./images/NumberParam.png) |`Number` |**Offset Length 1** |Automatic Offset Length at End 1 of Member |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Automatic Offset End 2** |Automatic Offset at End 2 of Member |
|![NumberParam](./images/NumberParam.png) |`Number` |**Offset Length 2** |Automatic Offset Length at End 2 of Member |
|![NumberParam](./images/NumberParam.png) |`Number` |**Orientation Angle** |Member Orientation Angle in radians |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Orientation Node** |Member Orientation Node |
|![NumberParam](./images/NumberParam.png) |`Number` |**Mesh Size in model units** |Member Mesh Size |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Mesh With Others** |if to mesh with others |
|![EffectiveLengthOptionsParam](./images/EffectiveLengthOptionsParam.png) |[Effective Length Options](gsagh-effective-length-options-parameter.md) |**Get Effective Length Options** |GSA 1D Member Design Options for Effective Length, Restraints and Buckling Factors |
|![TextParam](./images/TextParam.png) |`Text` |**Member Name** |Name of Member1d |
|![ColourParam](./images/ColourParam.png) |`Colour` |**Member Colour** |Member Colour |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Dummy Member** |it Member is Dummy |
|![TextParam](./images/TextParam.png) |`Text` |**Topology** |the Member's original topology list referencing node IDs in Model that Model was created from |
