# Restraint
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                 |
| --------------------------------------  |
| ![Bool6Param](./images/Bool6Param.png)  |

## Description

A Bool6 contains six booleans to set releases in [Element 1D](gsagh-element-1d-parameter.md)s and [Member 1D](gsagh-member-1d-parameter.md)s, or restraints in [Node](gsagh-node-parameter.md)s.

## Properties

| <img width="20"/> Icon               | <img width="200"/> Type        | <img width="200"/> Name                  | <img width="1000"/> Description                                                                                              |
| ------------------------------------ | ------------------------------ | ---------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------  |
| ![TextParam](./images/TextParam.png) | `Text`                         | **Top Flange (F1) Warping Restraint**    | Top Flange (F1) Warping Restraint<br />Accepted inputs are:<br />  None (0)<br />  Partial (1)<br />  Full (2)               |
| ![TextParam](./images/TextParam.png) | `Text`                         | **Bottom Flange (F2) Warping Restraint** | Bottom Flange (F1) Warping Restraint<br />Accepted inputs are:<br />  None (0)<br />  Partial (1)<br />  Full (2)            |
| ![TextParam](./images/TextParam.png) | `Text`                         | **Torsional Restraint (xx)**             | Torsional Restraint (xx)<br />Accepted inputs are:<br />  None (0)<br />  Frictional (1)<br />  Partial (2)<br />  Full (3)  |
| ![TextParam](./images/TextParam.png) | `Text`                         | **Major Axis Rotation (yy)**             | Major Axis Rotational Restraint (yy)<br />Accepted inputs are:<br />  None (0)<br />  Partial (1)<br />  Full (2)            |
| ![TextParam](./images/TextParam.png) | `Text`                         | **Minor Axis Rotation (zz)**             | Minor Axis Rotational Restraint (zz)<br />Accepted inputs are:<br />  None (0)<br />  Partial (1)<br />  Full (2)            |
| ![TextParam](./images/TextParam.png) | `Text`                         | **Top Flange (F1) Lateral Restraint**    | Top Flange (F1) Lateral Restraint<br />Accepted inputs are:<br />  None (0)<br />  Full (1)                                  |
| ![TextParam](./images/TextParam.png) | `Text`                         | **Bottom Flange (F2) Lateral Restraint** | Bottom Flange (F1) Lateral Restraint<br />Accepted inputs are:<br />  None (0)<br />  Full (1)                               |
| ![TextParam](./images/TextParam.png) | `Text`                         | **Major Axis Translation (y)**           | Major Axis Translational Restraint (y)<br />Accepted inputs are:<br />  None (0)<br />  Full (1)                             |
| ![TextParam](./images/TextParam.png) | `Text`                         | **Minor Axis Translation (z)**           | Minor Axis Translational Restraint (z)<br />Accepted inputs are:<br />  None (0)<br />  Full (1)                             |

_Note: the above properties can be retrieved using the [Member End Restraint Info](gsagh-member-end-restraint-info-component.md) component_
