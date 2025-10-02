# Create 2D Property Modifier
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Create 2D Property Modifier](./images/Create2dPropertyModifier.png) |

## Description

Create GSA 2D Property Modifier

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![GenericParam](./images/GenericParam.png) |`Generic` |**In-plane Modifier** |[Optional] Modify the effective in-plane stiffness BY this decimal fraction value (Default = 1.0 -> 100%) |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Bending Modifier** |[Optional] Modify the effective bending stiffness BY this decimal fraction value (Default = 1.0 -> 100%) |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Shear Modifier** |[Optional] Modify the effective shear stiffness BY thisdecimal fraction value (Default = 1.0 -> 100%) |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Volume Modifier** |[Optional] Modify the effective volume BY this decimal fraction value (Default = 1.0 -> 100%) |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Area Density ` |**Additional Mass** |[Optional] Additional mass per unit length (Default = 0 -> no additional mass) |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![Property2dModifierParam](./images/Property2dModifierParam.png) |[Property 2D Modifier](gsagh-property-2d-modifier-parameter.md) |**Property 2D Modifier** |GSA Property 2D Modifier parameter |
