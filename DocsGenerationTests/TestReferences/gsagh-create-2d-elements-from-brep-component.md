# Create 2D Elements from Brep
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Create 2D Elements from Brep](./images/Create2dElementsFromBrep.png) |

## Description

Mesh a non-planar Brep

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![BrepParam](./images/BrepParam.png) |`Brep` |**Brep** |Brep (can be non-planar) |
|![GenericParam](./images/GenericParam.png) |`Generic` _List_ |**Incl. Points or Nodes** |Inclusion points or Nodes |
|![GenericParam](./images/GenericParam.png) |`Generic` _List_ |**Incl. Curves or 1D Members** |Inclusion curves or 1D Members |
|![Property2dParam](./images/Property2dParam.png) |[Property 2D](gsagh-property-2d-parameter.md) |**Property 2D** |2D Property (Area) parameter |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` |**Mesh Size** |Target mesh size |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![Element2dParam](./images/Element2dParam.png) |[Element 2D](gsagh-element-2d-parameter.md) |**2D Elements** |GSA 2D Elements |
|![NodeParam](./images/NodeParam.png) |[Node](gsagh-node-parameter.md) _List_ |**Incl. Nodes** |Inclusion Nodes |
|![Element1dParam](./images/Element1dParam.png) |[Element 1D](gsagh-element-1d-parameter.md) _List_ |**Incl. Element1Ds** |Inclusion 1D Elements created from 1D Members |


