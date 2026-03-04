# Property 2D Modifier
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                           |
| ----------------------------------------------------------------  |
| ![Property2dModifierParam](./images/Property2dModifierParam.png)  |

## Description

A 2D Property Modifier is part of a [Property 2D](gsagh-property-2d-parameter.md) and can be used to modify property's analytical properties without changing the `Thickness` or [Material](gsagh-material-parameter.md). By default the 2D Property Modifier is unmodified.

## Properties

| <img width="20"/> Icon                     | <img width="200"/> Type        | <img width="200"/> Name        | <img width="1000"/> Description |
| ------------------------------------------ | ------------------------------ | ------------------------------ | ------------------------------  |
| ![GenericParam](./images/GenericParam.png) | `Generic`                      | **In-plane Modifier**          | Effective in-plane stiffness    |
| ![GenericParam](./images/GenericParam.png) | `Generic`                      | **Bending Modifier**           | Effective bending stiffness     |
| ![GenericParam](./images/GenericParam.png) | `Generic`                      | **Shear Modifier**             | Effective shear stiffness       |
| ![GenericParam](./images/GenericParam.png) | `Generic`                      | **Volume Modifier**            | Effective volume                |
| ![GenericParam](./images/GenericParam.png) | `Generic`                      | **Additional Mass**            | Additional mass per unit area   |

_Note: the above properties can be retrieved using the [Get 2D Property Modifier](gsagh-get-2d-property-modifier-component.md) component_
