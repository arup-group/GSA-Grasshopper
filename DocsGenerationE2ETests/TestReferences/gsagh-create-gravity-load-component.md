# Create Gravity Load
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                 |
| ------------------------------------------------------  |
| ![Create Gravity Load](./images/CreateGravityLoad.png)  |

## Description

Create GSA Gravity Load

### Input parameters

| <img width="20"/> Icon                       | <img width="200"/> Type                   | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                                                                                                                                                                                                                                                                                          |
| -------------------------------------------- | ----------------------------------------- | ------------------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  |
| ![LoadCaseParam](./images/LoadCaseParam.png) | [Load Case](gsagh-load-case-parameter.md) | **Load Case**                  | Load Case parameter                                                                                                                                                                                                                                                                                                                                                                                      |
| ![GenericParam](./images/GenericParam.png)   | `Generic`                                 | **Loadable Objects**           | Lists, Custom Materials, Properties, Elements or Members to apply load to; either input Section, Prop2d, Prop3d, Element1d, Element2d, Member1d, Member2d or Member3d, or a text string.<br />Text string with Element list should take the form:<br /> 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)<br />Refer to help file for definition of lists and full vocabulary.  |
| ![TextParam](./images/TextParam.png)         | `Text`                                    | **Name**                       | Load Name                                                                                                                                                                                                                                                                                                                                                                                                |
| ![VectorParam](./images/VectorParam.png)     | `Vector`                                  | **Gravity factor**             | Gravity vector factor (default z = -1)                                                                                                                                                                                                                                                                                                                                                                   |

### Output parameters

| <img width="20"/> Icon               | <img width="200"/> Type         | <img width="200"/> Name        | <img width="1000"/> Description |
| ------------------------------------ | ------------------------------- | ------------------------------ | ------------------------------  |
| ![LoadParam](./images/LoadParam.png) | [Load](gsagh-load-parameter.md) | **Gravity load**               | GSA Gravity Load                |
