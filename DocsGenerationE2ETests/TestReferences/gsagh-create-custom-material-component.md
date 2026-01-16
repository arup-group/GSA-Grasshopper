# Create Custom Material
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                       |
| ------------------------------------------------------------  |
| ![Create Custom Material](./images/CreateCustomMaterial.png)  |

## Description

Create a Custom GSA Analysis Material

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                     | <img width="200"/> Type                                          | <img width="200"/> Name        | <img width="1000"/> Description                                  |
| ------------------------------------------ | ---------------------------------------------------------------- | ------------------------------ | ---------------------------------------------------------------  |
| ![IntegerParam](./images/IntegerParam.png) | `Integer`                                                        | **Analysis Property Number**   | Analysis Property Number (do not use 0 -> 'from Grade')          |
| ![TextParam](./images/TextParam.png)       | `Text`                                                           | **Material Name**              | Material Name of Custom Material                                 |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Pressure`          | **Elastic Modulus**            | Elastic Modulus of the elastic isotropic material                |
| ![NumberParam](./images/NumberParam.png)   | `Number`                                                         | **Poisson's Ratio**            | Poisson's Ratio of the elastic isotropic material                |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Density`           | **Density**                    | Density of the elastic isotropic material                        |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Thermal Expansion` | **Thermal Expansion**          | Thermal Expansion Coefficient of the elastic isotropic material  |

### Output parameters

| <img width="20"/> Icon                       | <img width="200"/> Type                 | <img width="200"/> Name        | <img width="1000"/> Description |
| -------------------------------------------- | --------------------------------------- | ------------------------------ | ------------------------------  |
| ![MaterialParam](./images/MaterialParam.png) | [Material](gsagh-material-parameter.md) | **Material**                   | GSA Custom Material             |
