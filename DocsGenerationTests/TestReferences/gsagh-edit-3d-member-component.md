# Edit 3D Member
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Edit 3D Member](./images/Edit3dMember.png) |

## Description

Modify GSA 3D Member

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![Member3dParam](./images/Member3dParam.png) |[Member 3D](gsagh-member-3d-parameter.md) |**Member 3D** |3D Member to get or set information for. Leave blank to create a new Member 3D |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Member3d Number** |Set Member Number. If ID is set it will replace any existing 3d Member in the model |
|![IGeometricParam](./images/IGeometricParam.png) |`Geometry` |**Solid** |Reposition Solid Geometry - Closed Brep or Mesh |
|![Property3dParam](./images/Property3dParam.png) |[Property 3D](gsagh-property-3d-parameter.md) |**3D Property** |Set new 3D Property. |
|![NumberParam](./images/NumberParam.png) |`Number` |**Mesh Size in model units** |Set Member Mesh Size |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Mesh With Others** |Mesh with others? |
|![TextParam](./images/TextParam.png) |`Text` |**Member3d Name** |Set Name of Member3d |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Member3d Group** |Set Member 3d Group |
|![ColourParam](./images/ColourParam.png) |`Colour` |**Member3d Colour** |Set Member 3d Colour |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Dummy Member** |Set Member to Dummy |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![Member3dParam](./images/Member3dParam.png) |[Member 3D](gsagh-member-3d-parameter.md) |**Member 3D** |GSA 3D Member with applied changes. |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Member Number** |Member Number |
|![MeshParam](./images/MeshParam.png) |`Mesh` |**Solid Mesh** |Member Solid Mesh |
|![Property3dParam](./images/Property3dParam.png) |[Property 3D](gsagh-property-3d-parameter.md) |**3D Property** |3D Property |
|![NumberParam](./images/NumberParam.png) |`Number` |**Mesh Size in model units** |Target mesh size |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Mesh With Others** |if to mesh with others |
|![TextParam](./images/TextParam.png) |`Text` |**Member Name** |Name of Member |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Member Group** |Member Group |
|![ColourParam](./images/ColourParam.png) |`Colour` |**Member Colour** |Member Colour |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Dummy Member** |if Member is Dummy |
|![TextParam](./images/TextParam.png) |`Text` |**Topology** |the Member's original topology list referencing node IDs in Model that Model was created from |
