# Section Properties
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                |
| -----------------------------------------------------  |
| ![Section Properties](./images/SectionProperties.png)  |

## Description

Get GSA Section Properties

### Input parameters

| <img width="20"/> Icon                     | <img width="200"/> Type               | <img width="200"/> Name        | <img width="1000"/> Description                         |
| ------------------------------------------ | ------------------------------------- | ------------------------------ | ------------------------------------------------------  |
| ![SectionParam](./images/SectionParam.png) | [Section](gsagh-section-parameter.md) | **Section**                    | Section Property (Beam) to get a bit more info out of.  |

### Output parameters

| <img width="20"/> Icon                     | <img width="200"/> Type                                                   | <img width="200"/> Name        | <img width="1000"/> Description                       |
| ------------------------------------------ | ------------------------------------------------------------------------- | ------------------------------ | ----------------------------------------------------  |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Area`                       | **Area**                       | Section Area                                          |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Area Moment Of Inertia`     | **Moment of Inertia y-y**      | Section Moment of Intertia around local y-y axis      |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Area Moment Of Inertia`     | **Moment of Inertia z-z**      | Section Moment of Intertia around local z-z axis      |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Area Moment Of Inertia`     | **Moment of Inertia y-z**      | Section Moment of Intertia around local y-z axis      |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Area Moment Of Inertia`     | **Moment of Inertia u-u**      | Section Moment of Intertia around principal u-u axis  |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Area Moment Of Inertia`     | **Moment of Inertia v-v**      | Section Moment of Intertia around principal v-v axis  |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Angle`                      | **Angle**                      | Angle between local and principal axis                |
| ![GenericParam](./images/GenericParam.png) | `Generic`                                                                 | **Shear Area Factor y-y**      | Section Shear Area Factor around local y-y axis       |
| ![GenericParam](./images/GenericParam.png) | `Generic`                                                                 | **Shear Area Factor z-z**      | Section Shear Area Factor around local z-z axis       |
| ![GenericParam](./images/GenericParam.png) | `Generic`                                                                 | **Shear Area Factor u-u**      | Section Shear Area Factor around local u-u axis       |
| ![GenericParam](./images/GenericParam.png) | `Generic`                                                                 | **Shear Area Factor v-v**      | Section Shear Area Factor around local v-v axis       |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Area Moment Of Inertia`     | **Torsion Constant J**         | Section Torsion constant J                            |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Section Modulus`            | **Torsion Constant C**         | Section Torsion constant C                            |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Section Modulus`            | **Section Modulus in y**       | Section Modulus in y-direction                        |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Section Modulus`            | **Section Modulus in z**       | Section Modulus in z-direction                        |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Section Modulus`            | **Plastic Modulus in y**       | Plastic Section Modulus in y-direction                |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Section Modulus`            | **Plastic Modulus in z**       | Plastic Section Modulus in z-direction                |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Length`                     | **Elastic Centroid in y**      | Elastic Centroid in y-direction                       |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Length`                     | **Elastic Centroid in z**      | Elastic Centroid in z-direction                       |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Length`                     | **Radius of Gyration in y**    | Radius of Gyration in y-direction                     |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Length`                     | **Radius of Gyration in z**    | Radius of Gyration in z-direction                     |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Surface Area / Unit Length` | **Surface Area / Unit Length** | Section Surface Area per Unit Length                  |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Volume / Unit Length`       | **Volume / Unit Length**       | Section Volume per Unit Length                        |
