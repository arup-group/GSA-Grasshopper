# Material
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![MaterialParam](./images/MaterialParam.png) |

## Description

A Material is used by [Section](gsagh-section-parameter.md)s, [Property 2D](gsagh-property-2d-parameter.md)s and [Property 3D](gsagh-property-3d-parameter.md)s. It is only possible to work with elastic isotropic material types. A Material can either be created as a Standard Material from design code and grade using the [Create Material](gsagh-create-material-component.md) component, or as a custom material using the [Create Custom Material](gsagh-create-custom-material-component.md) component.

Use the [Get Model Materials](gsagh-get-model-materials-component.md) to get all materials in a [Model](gsagh-model-parameter.md) and then use [Edit Material](gsagh-edit-material-component.md) in combination with [Material Properties](gsagh-material-properties-component.md) to get information about material properties.

Refer to [Materials](/references/hidr-data-mat-steel.md) to read more.



## Properties

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Material ID** |the Material's ID in its respective table (Steel, Concrete, etc) |
|![TextParam](./images/TextParam.png) |`Text` |**Material Name** |the Material's Name |
|![MaterialParam](./images/MaterialParam.png) |[Material](gsagh-material-parameter.md) |**Custom Material** |A copy of this material as a Custom material. |
|![TextParam](./images/TextParam.png) |`Text` |**Material Type** |Material Type |

_Note: the above properties can be retrieved using the [Edit Material](gsagh-edit-material-component.md) component_
