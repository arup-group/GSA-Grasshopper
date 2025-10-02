# Node
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![NodeParam](./images/NodeParam.png) |

## Description

A Node generally contains `X`, `Y`, and `Z` coordinates as well as [Support](/explanations/supports.md) condition.

In GSA, Nodes are the only objects containing spacial geometrical information (X, Y, Z coordinates). The node numbers are referred to by elements and members in their topology lists and therefore only contains reference to nodes, not the actualy node. In GSA this works because everything belongs to a single model, and the information does not need to be duplicated in elements and members. 

In Grasshopper, on the other hand, all parameters (nodes, elements and members) exist independently from each other. For instance, an [Element 1D](gsagh-element-1d-parameter.md) in GsaGH keeps a copy of its start and end points, which is automatically taken care of by the plugin. Therefore, Nodes almost only need to be used for defining supports, as all other nodes in a model will be included as part of the Elements or Members.

Refer to [Node](/references/hidr-data-node.md) to read more.



## Properties

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Node number** |Original Node number (ID) if Node ever belonged to a GSA Model |
|![PointParam](./images/PointParam.png) |`Point` |**Node Position** |Position (x, y, z) of Node. Setting a new position will clear any existing ID |
|![PlaneParam](./images/PlaneParam.png) |`Plane` |**Node local axis** |Local axis (Plane) of Node |
|![Bool6Param](./images/Bool6Param.png) |[Bool6](gsagh-bool6-parameter.md) |**Node Restraints** |Restraints (Bool6) of Node |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Damper Property** |Damper Property reference |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Mass Property** |Mass Property reference |
|![SpringPropertyParam](./images/SpringPropertyParam.png) |[Spring Property](gsagh-spring-property-parameter.md) |**Spring Property** |GSA Spring Property parameter |
|![TextParam](./images/TextParam.png) |`Text` |**Node Name** |Name of Node |
|![ColourParam](./images/ColourParam.png) |`Colour` |**Node Colour** |colour of node |

_Note: the above properties can be retrieved using the [Edit Node](gsagh-edit-node-component.md) component_
