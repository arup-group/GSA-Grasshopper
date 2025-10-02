# Create 2D Member
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Create 2D Member](./images/Create2dMember.png) |

## Description

Create GSA Member 2D

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_
_This component can preview 3D Sections, right-click the middle of the component to toggle the section preview._

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![BrepParam](./images/BrepParam.png) |`Brep` |**Brep** |Planar Brep (non-planar geometry will be automatically converted to an average plane of exterior boundary control points)) |
|![PointParam](./images/PointParam.png) |`Point` _List_ |**Incl. Points** |Inclusion points (will automatically be projected onto Brep) |
|![CurveParam](./images/CurveParam.png) |`Curve` _List_ |**Incl. Curves** |Inclusion curves (will automatically be made planar and projected onto brep, and converted to Arcs and Lines) |
|![Property2dParam](./images/Property2dParam.png) |[Property 2D](gsagh-property-2d-parameter.md) |**Property 2D** |2D Property (Area) parameter |
|![NumberParam](./images/NumberParam.png) |`Number` |**Mesh Size in model units** |Target mesh size |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Internal Offset** |Set Automatic Internal Offset of Member |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![Member2dParam](./images/Member2dParam.png) |[Member 2D](gsagh-member-2d-parameter.md) |**Member 2D** |GSA 2D Member parameter |
