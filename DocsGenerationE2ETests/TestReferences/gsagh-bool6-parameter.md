# Bool6
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                 |
| --------------------------------------  |
| ![Bool6Param](./images/Bool6Param.png)  |

:::info The `Bool6` icon takes inspiration from the central pin/hinge/charnier connection [Ove Arup's Kingsgate footbridge](https://www.arup.com/projects/kingsgate-footbridge).
![Kingsgate Footbridge Durham](./images/Kingsgate-Footbridge-Durham.jpg)
*(c) Giles Rocholl / Arup*

Did you know?

:::

## Description

A Bool6 contains six booleans to set releases in [Element 1D](gsagh-element-1d-parameter.md)s and [Member 1D](gsagh-member-1d-parameter.md)s, or restraints in [Node](gsagh-node-parameter.md)s.

## Properties

| <img width="20"/> Icon                     | <img width="200"/> Type        | <img width="200"/> Name        | <img width="1000"/> Description                      |
| ------------------------------------------ | ------------------------------ | ------------------------------ | ---------------------------------------------------  |
| ![BooleanParam](./images/BooleanParam.png) | `Boolean`                      | **X**                          | Release or restraint for translation in X-direction  |
| ![BooleanParam](./images/BooleanParam.png) | `Boolean`                      | **Y**                          | Release or restraint for translation in Y-direction  |
| ![BooleanParam](./images/BooleanParam.png) | `Boolean`                      | **Z**                          | Release or restraint for translation in Z-direction  |
| ![BooleanParam](./images/BooleanParam.png) | `Boolean`                      | **XX**                         | Release or restraint for rotation around X-axis      |
| ![BooleanParam](./images/BooleanParam.png) | `Boolean`                      | **YY**                         | Release or restraint for rotation around Y-axis      |
| ![BooleanParam](./images/BooleanParam.png) | `Boolean`                      | **ZZ**                         | Release or restraint for rotation around Z-axis      |

_Note: the above properties can be retrieved using the [Edit Bool6](gsagh-edit-bool6-component.md) component_
