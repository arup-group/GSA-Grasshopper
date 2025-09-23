# Member 2D
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Member2dParam](./images/Member2dParam.png) |

## Description

[Members](/references/hidr-data-member.md) in GSA are geometrical objects used in the Design Layer. Members can automatically intersect with other members. Members are as such more closely related to building objects, like a beam, column, slab or wall. Elements can automatically be created from Members used for analysis. 

 A Member2D is the planar/area geometry resembling for instance a slab or a wall. It can be defined geometrically from a planar Brep. 

 Refer to [2D Members](/explanations/members-2d.md) to read more. 



## Properties

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Member Number** |Member Number |
|![BrepParam](./images/BrepParam.png) |`Brep` |**Brep** |Member Brep |
|![PointParam](./images/PointParam.png) |`Point` _List_ |**Incl. Points** |Inclusion points |
|![CurveParam](./images/CurveParam.png) |`Curve` _List_ |**Incl. Curves** |Inclusion curves |
|![Property2dParam](./images/Property2dParam.png) |[Property 2D](gsagh-property-2d-parameter.md) |**2D Property** |2D Section Property |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Member Group** |Member Group |
|![TextParam](./images/TextParam.png) |`Text` |**Member Type** |2D Member Type |
|![TextParam](./images/TextParam.png) |`Text` |**2D Element Type** |Member 2D Analysis Element Type<br />0: Linear (Tri3/Quad4), 1: Quadratic (Tri6/Quad8), 2: Rigid Diaphragm |
|![OffsetParam](./images/OffsetParam.png) |[Offset](gsagh-offset-parameter.md) |**Offset** |Member Offset |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Internal Offset** |Automatic Internal Offset of Member |
|![NumberParam](./images/NumberParam.png) |`Number` |**Mesh Size in model units** |Member Mesh Size |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Mesh With Others** |if to mesh with others |
|![TextParam](./images/TextParam.png) |`Text` |**Mesh Mode** |Mesh mode: Tri-only, Mixed (quad dominant) or Quad-only |
|![NumberParam](./images/NumberParam.png) |`Number` |**Orientation Angle** |Member Orientation Angle in radians |
|![TextParam](./images/TextParam.png) |`Text` |**Member Name** |Name of Member |
|![ColourParam](./images/ColourParam.png) |`Colour` |**Member Colour** |Member Colour |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Dummy Member** |if Member is Dummy |
|![TextParam](./images/TextParam.png) |`Text` |**Topology** |the Member's original topology list referencing node IDs in Model that Model was created from |

_Note: the above properties can be retrieved using the [Edit 2D Member](gsagh-edit-2d-member-component.md) component_
