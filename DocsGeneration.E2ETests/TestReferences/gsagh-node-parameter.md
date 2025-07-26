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




