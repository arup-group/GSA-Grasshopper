# Member 1D
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Member1dParam](./images/Member1dParam.png) |

## Description

[Members](/references/hidr-data-member.md) in GSA are geometrical objects used in the Design Layer. Members can automatically intersection with other members. Members are as such more closely related to building objects, like a beam, column, slab or wall. Elements can automatically be created from Members used for analysis. 

 A Member1D is the linear geometry resembling for instance a column or a beam. They can be defined geometrically by a PolyCurve consisting of either multiple line segments or a single arc. 

 Refer to [1D Members](/explanations/members-1d.md) to read more.

## Properties

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
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

_Note: the above properties can be retrieved using the [Edit 1D Member](gsagh-edit-1d-member-component.md) component_
