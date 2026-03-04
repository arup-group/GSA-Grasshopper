# Property 2D
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                           |
| ------------------------------------------------  |
| ![Property2dParam](./images/Property2dParam.png)  |

## Description

A 2D property is used by [Element 2D](gsagh-element-2d-parameter.md) and [Member 2D](gsagh-member-2d-parameter.md) and generally contains information about it's the Area Property's `Thickness` and [Material](gsagh-material-parameter.md). 2D Properties can also be used to create LoadPanels, use the [Create 2D Property](gsagh-create-2d-property-component.md) component and select `LoadPanel` from the dropdown list. 

Refer to [2D Element Properties](/references/hidr-data-pr-2d.md) to read more.



## Properties

| <img width="20"/> Icon                                           | <img width="200"/> Type                                         | <img width="200"/> Name        | <img width="1000"/> Description                                                                                          |
| ---------------------------------------------------------------- | --------------------------------------------------------------- | ------------------------------ | -----------------------------------------------------------------------------------------------------------------------  |
| ![IntegerParam](./images/IntegerParam.png)                       | `Integer`                                                       | **Prop2d ID**                  | 2D Property ID                                                                                                           |
| ![TextParam](./images/TextParam.png)                             | `Text`                                                          | **Name**                       | Name of 2D Proerty                                                                                                       |
| ![ColourParam](./images/ColourParam.png)                         | `Colour`                                                        | **Colour**                     | 2D Property Colour                                                                                                       |
| ![GenericParam](./images/GenericParam.png)                       | `Generic`                                                       | **Axis**                       | Local Axis either as `Plane` for custom local axis or an `Integer` (Global: 0 or Topological: 1) for a referenced Axis.  |
| ![TextParam](./images/TextParam.png)                             | `Text`                                                          | **Type**                       | 2D Property Type                                                                                                         |
| ![MaterialParam](./images/MaterialParam.png)                     | [Material](gsagh-material-parameter.md)                         | **Material**                   | GSA Material parameter                                                                                                   |
| ![UnitNumber](./images/UnitParam.png)                            | [Unit Number](gsagh-unitnumber-parameter.md) `Length`           | **Thickness**                  | Property Thickness                                                                                                       |
| ![GenericParam](./images/GenericParam.png)                       | `Generic`                                                       | **Reference Surface**          | Reference Surface Middle (default) = 0, Top = 1, Bottom = 2                                                              |
| ![UnitNumber](./images/UnitParam.png)                            | [Unit Number](gsagh-unitnumber-parameter.md) `Length`           | **Offset**                     | Additional Offset                                                                                                        |
| ![Property2dModifierParam](./images/Property2dModifierParam.png) | [Property 2D Modifier](gsagh-property-2d-modifier-parameter.md) | **Property 2D Modifier**       | GSA Property 2D Modifier parameter                                                                                       |
| ![GenericParam](./images/GenericParam.png)                       | `Generic`                                                       | **Support Type**               | Support Type                                                                                                             |
| ![IntegerParam](./images/IntegerParam.png)                       | `Integer`                                                       | **Reference Edge**             | Reference Edge for Load Panels with support type other than Auto and All Edges                                           |

_Note: the above properties can be retrieved using the [Edit 2D Property](gsagh-edit-2d-property-component.md) component_
