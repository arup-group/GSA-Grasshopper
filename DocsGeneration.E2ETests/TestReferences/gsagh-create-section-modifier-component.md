# Create Section Modifier
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Create Section Modifier](./images/CreateSectionModifier.png) |

## Description

Create a GSA Section Modifier

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Area Modifier** |[Optional] Modify the effective Area BY this decimal fraction value (Default = 1.0 -> 100%) |
|![GenericParam](./images/GenericParam.png) |`Generic` |**I11 Modifier** |[Optional] Modify the effective Iyy/Iuu BY this decimal fraction value (Default = 1.0 -> 100%) |
|![GenericParam](./images/GenericParam.png) |`Generic` |**I22 Modifier** |[Optional] Modify the effective Izz/Ivv BY this decimal fraction value (Default = 1.0 -> 100%) |
|![GenericParam](./images/GenericParam.png) |`Generic` |**J Modifier** |[Optional] Modify the effective J BY this decimal fraction value (Default = 1.0 -> 100%) |
|![GenericParam](./images/GenericParam.png) |`Generic` |**K11 Modifier** |[Optional] Modify the effective Kyy/Kuu BY this decimal fraction value (Default = 1.0 -> 100%) |
|![GenericParam](./images/GenericParam.png) |`Generic` |**K22 Modifier** |[Optional] Modify the effective Kzz/Kvv BY this decimal fraction value (Default = 1.0 -> 100%) |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Volume Modifier** |[Optional] Modify the effective Volume/Length BY this decimal fraction value (Default = 1.0 -> 100%) |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Linear Density ` |**Additional Mass** |[Optional] Additional mass per unit length (Default = 0 -> no additional mass) |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Principal Bending Axis** |[Optional] Set to 'true' to use Principal (u,v) Axis for Bending. If false (and by default), Local (y,z) Axis will be used |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Reference Point Centroid** |[Optional] Set to 'true' to use the Centroid as Analysis Reference Point. If false (and by default), the specified point will be used |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![SectionModifierParam](./images/SectionModifierParam.png) |[Section Modifier](gsagh-section-modifier-parameter.md) |**Section Modifier** |GSA Section Modifier parameter |


