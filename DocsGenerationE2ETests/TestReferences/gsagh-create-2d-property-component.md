# Create 2D Property
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                               |
| ----------------------------------------------------  |
| ![Create 2D Property](./images/Create2dProperty.png)  |

## Description

Create a GSA 2D Property

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                       | <img width="200"/> Type                               | <img width="200"/> Name        | <img width="1000"/> Description                              |
| -------------------------------------------- | ----------------------------------------------------- | ------------------------------ | -----------------------------------------------------------  |
| ![UnitNumber](./images/UnitParam.png)        | [Unit Number](gsagh-unitnumber-parameter.md) `Length` | **Thickness**                  | Section thickness                                            |
| ![MaterialParam](./images/MaterialParam.png) | [Material](gsagh-material-parameter.md)               | **Material**                   | Material                                                     |
| ![GenericParam](./images/GenericParam.png)   | `Generic`                                             | **Reference Surface**          | Reference Surface Middle = 0 (default), Top = 1, Bottom = 2  |
| ![UnitNumber](./images/UnitParam.png)        | [Unit Number](gsagh-unitnumber-parameter.md) `Length` | **Offset**                     | Additional Offset                                            |

### Output parameters

| <img width="20"/> Icon                           | <img width="200"/> Type                       | <img width="200"/> Name        | <img width="1000"/> Description   |
| ------------------------------------------------ | --------------------------------------------- | ------------------------------ | --------------------------------  |
| ![Property2dParam](./images/Property2dParam.png) | [Property 2D](gsagh-property-2d-parameter.md) | **Property 2D**                | GSA 2D Property (Area) parameter  |
