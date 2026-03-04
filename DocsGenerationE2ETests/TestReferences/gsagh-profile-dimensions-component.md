# Profile Dimensions
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                |
| -----------------------------------------------------  |
| ![Profile Dimensions](./images/ProfileDimensions.png)  |

## Description

Get GSA Section Dimensions

### Input parameters

| <img width="20"/> Icon                     | <img width="200"/> Type               | <img width="200"/> Name        | <img width="1000"/> Description                         |
| ------------------------------------------ | ------------------------------------- | ------------------------------ | ------------------------------------------------------  |
| ![SectionParam](./images/SectionParam.png) | [Section](gsagh-section-parameter.md) | **Section**                    | Section Property (Beam) to get a bit more info out of.  |

### Output parameters

| <img width="20"/> Icon                | <img width="200"/> Type                               | <img width="200"/> Name        | <img width="1000"/> Description                                                                          |
| ------------------------------------- | ----------------------------------------------------- | ------------------------------ | -------------------------------------------------------------------------------------------------------  |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Length` | **Depth**                      | Section Depth or Diameter)                                                                               |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Length` | **Width**                      | Section Width                                                                                            |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Length` | **Width Top**                  | Section Width Top (will be equal to width if profile is symmetric)                                       |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Length` | **Width Bottom**               | Section Width Bottom (will be equal to width if profile is symmetric)                                    |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Length` | **Flange Thk Top**             | Section Top Flange Thickness                                                                             |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Length` | **Flange Thk Bottom**          | Section Bottom Flange Thickness                                                                          |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Length` | **Web Thk**                    | Section Web Thickness                                                                                    |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Length` | **Radius**                     | Section Root Radius (only applicable to catalogue profiles) or hole size for cellular/castellated beams  |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Length` | **Spacing**                    | Spacing/pitch                                                                                            |
| ![TextParam](./images/TextParam.png)  | `Text`                                                | **Type**                       | Profile type description                                                                                 |
