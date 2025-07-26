# Assembly
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![AssemblyParam](./images/AssemblyParam.png) |

## Description

Assemblies are a way to define an entity that is formed from a collection of elements or members and can be thought of as a superelement. This is not an analysis entity but rather a convenience for post-processing, such as cut section forces. Typical uses of assemblies include cores, where the core is modelled with 2D finite elements; trusses, where the truss is modelled with top and bottom chords; and bracing. In both these cases the assembly is identified by a list of included elements. Unlike the analysis elements, an assembly does not have a clearly define orientation and location of reference point so these must be defined explicitly. Refer to [Assemblies](/references/hidr-data-assembly.md) to read more.



## Properties

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![TextParam](./images/TextParam.png) |`Text` |**Name** |Assembly Name |
|![TextParam](./images/TextParam.png) |`Text` |**Assembly type** |Assembly type |
|![GenericParam](./images/GenericParam.png) |`Generic` |**List** |Assembly Entities |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Topology 1** |Node at the start of the Assembly |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Topology 2** |Node at the end of the Assembly |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Orientation Node** |Assembly Orientation Node |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` |**Extents y** |Extents of the Assembly in y-direction |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` |**Extents z** |Extents of the Assembly in z-direction |
|![IntegerParam](./images/IntegerParam.png) |`Integer` _List_ |**Internal Topology** |List of nodes that define the curve of the Assembly |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Curve Fit** |Curve Fit for curved elements<br />Lagrange Interpolation (2) or Circular Arc (1) |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Definition** |Assembly definition |

_Note: the above properties can be retrieved using the [Get Assembly](gsagh-get-assembly-component.md) component_
