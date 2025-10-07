# Edit 2D Member
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Edit 2D Member](./images/Edit2dMember.png) |

## Description

Modify GSA 2D Member

_Note: This component can preview 3D Sections, right-click the middle of the component to toggle the section preview._

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![Member2dParam](./images/Member2dParam.png) |[Member 2D](gsagh-member-2d-parameter.md) |**Member 2D** |2D Member to get or set information for. Leave blank to create a new Member 2D |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Member2d Number** |Set Member Number. If ID is set it will replace any existing 2d Member in the model |
|![BrepParam](./images/BrepParam.png) |`Brep` |**Brep** |Reposition Member Brep (non-planar geometry will be automatically converted to an average plane from exterior boundary control points) |
|![PointParam](./images/PointParam.png) |`Point` _List_ |**Incl. Points** |Add inclusion points (will automatically be projected onto Brep) |
|![CurveParam](./images/CurveParam.png) |`Curve` _List_ |**Incl. Curves** |Add inclusion curves (will automatically be made planar and projected onto brep, and converted to Arcs and Lines) |
|![Property2dParam](./images/Property2dParam.png) |[Property 2D](gsagh-property-2d-parameter.md) |**2D Property** |Set new 2D Property. |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Member2d Group** |Set Member 2d Group |
|![TextParam](./images/TextParam.png) |`Text` |**Member Type** |Set 2D Member Type<br />Accepted inputs are:<br />1: Generic 2D (default)<br />4: Slab<br />5: Wall<br />7: Ribbed Slab<br />12: Void-cutter |
|![TextParam](./images/TextParam.png) |`Text` |**2D Element Type** |Set Member 2D Analysis Element Type<br />Accepted inputs are:<br />0: Linear - Tri3/Quad4 Elements (default)<br />1: Quadratic - Tri6/Quad8 Elements<br />2: Rigid Diaphragm<br />3: Load Panel |
|![OffsetParam](./images/OffsetParam.png) |[Offset](gsagh-offset-parameter.md) |**Offset** |Set Member Offset |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Internal Offset** |Set Automatic Internal Offset of Member |
|![NumberParam](./images/NumberParam.png) |`Number` |**Mesh Size in model units** |Set target mesh size |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Mesh With Others** |Mesh with others? |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Mesh Mode** |Mesh mode: 3 for Tri-only, 4 for Quad-only, anything else for mixed (quad dominant) |
|![NumberParam](./images/NumberParam.png) |`Number` |**Orientation Angle** |Set Member Orientation Angle |
|![TextParam](./images/TextParam.png) |`Text` |**Member2d Name** |Set Name of Member2d |
|![ColourParam](./images/ColourParam.png) |`Colour` |**Member2d Colour** |Set Member 2d Colour |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Dummy Member** |Set Member to Dummy |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![Member2dParam](./images/Member2dParam.png) |[Member 2D](gsagh-member-2d-parameter.md) |**Member 2D** |GSA 2D Member with applied changes. |
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


