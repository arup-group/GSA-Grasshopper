# Edit Node
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Edit Node](./images/EditNode.png) |

## Description

Modify GSA Node

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![NodeParam](./images/NodeParam.png) |[Node](gsagh-node-parameter.md) |**Node** |Node to get or set information for. Leave blank to create a new Node |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Node number** |Set Node number (ID) - if Node ID is set it will replace any existing nodes in the model |
|![PointParam](./images/PointParam.png) |`Point` |**Node Position** |Set new Position (x, y, z) of Node |
|![PlaneParam](./images/PlaneParam.png) |`Plane` |**Node local axis** |Set Local axis (Plane) of Node |
|![Bool6Param](./images/Bool6Param.png) |[Bool6](gsagh-bool6-parameter.md) |**Node Restraints** |Set Restraints (Bool6) of Node |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Damper Property** |Set Damper Property by reference |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Mass Property** |Set Mass Property by reference |
|![SpringPropertyParam](./images/SpringPropertyParam.png) |[Spring Property](gsagh-spring-property-parameter.md) |**Spring Property** |Spring Property parameter |
|![TextParam](./images/TextParam.png) |`Text` |**Node Name** |Set Name of Node |
|![ColourParam](./images/ColourParam.png) |`Colour` |**Node Colour** |Set colour of node |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![NodeParam](./images/NodeParam.png) |[Node](gsagh-node-parameter.md) |**Node** |GSA Node with applied changes. |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Node number** |Original Node number (ID) if Node ever belonged to a GSA Model |
|![PointParam](./images/PointParam.png) |`Point` |**Node Position** |Position (x, y, z) of Node. Setting a new position will clear any existing ID |
|![PlaneParam](./images/PlaneParam.png) |`Plane` |**Node local axis** |Local axis (Plane) of Node |
|![Bool6Param](./images/Bool6Param.png) |[Bool6](gsagh-bool6-parameter.md) |**Node Restraints** |Restraints (Bool6) of Node |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Damper Property** |Damper Property reference |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Mass Property** |Mass Property reference |
|![SpringPropertyParam](./images/SpringPropertyParam.png) |[Spring Property](gsagh-spring-property-parameter.md) |**Spring Property** |GSA Spring Property parameter |
|![TextParam](./images/TextParam.png) |`Text` |**Node Name** |Name of Node |
|![ColourParam](./images/ColourParam.png) |`Colour` |**Node Colour** |colour of node |


