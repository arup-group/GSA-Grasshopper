# Get Section Modifier
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                   |
| --------------------------------------------------------  |
| ![Get Section Modifier](./images/GetSectionModifier.png)  |

## Description

Get GSA Section Modifier

### Input parameters

| <img width="20"/> Icon                                     | <img width="200"/> Type                                 | <img width="200"/> Name        | <img width="1000"/> Description                           |
| ---------------------------------------------------------- | ------------------------------------------------------- | ------------------------------ | --------------------------------------------------------  |
| ![SectionModifierParam](./images/SectionModifierParam.png) | [Section Modifier](gsagh-section-modifier-parameter.md) | **Section Modifier**           | Section Modifier to get information for.Section Modifier  |

### Output parameters

| <img width="20"/> Icon                                     | <img width="200"/> Type                                               | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                   |
| ---------------------------------------------------------- | --------------------------------------------------------------------- | ------------------------------ | --------------------------------------------------------------------------------------------------------------------------------  |
| ![SectionModifierParam](./images/SectionModifierParam.png) | [Section Modifier](gsagh-section-modifier-parameter.md)               | **Section Modifier**           | GSA Section Modifier                                                                                                              |
| ![UnitNumber](./images/UnitParam.png)                      | [Unit Number](gsagh-unitnumber-parameter.md) `Area`                   | **Area Modifier**              | Effective Area                                                                                                                    |
| ![UnitNumber](./images/UnitParam.png)                      | [Unit Number](gsagh-unitnumber-parameter.md) `Area Moment Of Inertia` | **I11 Modifier**               | Effective Iyy/Iuu                                                                                                                 |
| ![UnitNumber](./images/UnitParam.png)                      | [Unit Number](gsagh-unitnumber-parameter.md) `Area Moment Of Inertia` | **I22 Modifier**               | Effective Izz/Ivv                                                                                                                 |
| ![GenericParam](./images/GenericParam.png)                 | `Generic`                                                             | **J Modifier**                 | Effective J                                                                                                                       |
| ![GenericParam](./images/GenericParam.png)                 | `Generic`                                                             | **K11 Modifier**               | Effective Kyy/Kuu                                                                                                                 |
| ![GenericParam](./images/GenericParam.png)                 | `Generic`                                                             | **K22 Modifier**               | Effective Kzz/Kvv                                                                                                                 |
| ![UnitNumber](./images/UnitParam.png)                      | [Unit Number](gsagh-unitnumber-parameter.md) `Volume Per Length`      | **Volume Modifier**            | Effective Volume/Length                                                                                                           |
| ![UnitNumber](./images/UnitParam.png)                      | [Unit Number](gsagh-unitnumber-parameter.md) `Linear Density`         | **Additional Mass**            | Additional mass per unit length                                                                                                   |
| ![BooleanParam](./images/BooleanParam.png)                 | `Boolean`                                                             | **Principal Bending Axis**     | If 'true' GSA will use Principal (u,v) Axis for Bending. If false, Local (y,z) Axis will be used                                  |
| ![BooleanParam](./images/BooleanParam.png)                 | `Boolean`                                                             | **Reference Point Centroid**   | If 'true' GSA will use the Centroid as Analysis Reference Point. If false, the specified point will be used                       |
| ![GenericParam](./images/GenericParam.png)                 | `Generic`                                                             | **Stress Option Type**         | the Stress Option Type:<br />0: No Calculation<br />1: Use Modified section properties<br />2: Use Unmodified section properties  |
