# Section
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![SectionParam](./images/SectionParam.png) |

## Description

A Section is used by [Element 1D](gsagh-element-1d-parameter.md) and [Member 1D](gsagh-member-1d-parameter.md) and generally contains information about it's `Profile` and [Material](gsagh-material-parameter.md). Use the [Create Profile](gsagh-create-profile-component.md) component to create Catalogue and custom profiles.

Refer to [Sections](/references/hidr-data-sect-lib.md) to read more.

## Properties

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Section Number** |Original Section number (ID) if the Section ever belonged to a GSA Model |
|![TextParam](./images/TextParam.png) |`Text` |**Section Profile** |Profile description |
|![MaterialParam](./images/MaterialParam.png) |[Material](gsagh-material-parameter.md) |**Material** |Material |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Basic Offset** |Basic Offset |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` |**Add. Offset Y** |Additional Offset Y |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` |**Add. Offset Z** |Additional Offset Z |
|![SectionModifierParam](./images/SectionModifierParam.png) |[Section Modifier](gsagh-section-modifier-parameter.md) |**Section Modifier** |Section Modifier |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Section Pool** |Section pool |
|![TextParam](./images/TextParam.png) |`Text` |**Section Name** |Section name |
|![ColourParam](./images/ColourParam.png) |`Colour` |**Section Colour** |Section colour |

_Note: the above properties can be retrieved using the [Edit Section](gsagh-edit-section-component.md) component_
