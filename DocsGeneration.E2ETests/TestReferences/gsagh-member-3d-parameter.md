# Member 3D
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Member3dParam](./images/Member3dParam.png) |

## Description

Members in GSA are geometrical objects used in the Design Layer. Members can automatically intersection with other members. Members are as such more closely related to building objects, like a beam, column, slab or wall. Elements can automatically be created from Members used for analysis. 

 A Member3D is the volumetric geometry resembling for instance soil. It can be defined geometrically by a closed Solid (either Mesh or Brep). 

 Refer to [Members](/references/hidr-data-member.md) to read more.

## Properties

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
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

_Note: the above properties can be retrieved using the [Edit 3D Member](gsagh-edit-3d-member-component.md) component_
