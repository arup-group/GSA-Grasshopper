# Create Face Thermal Load
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                          |
| ---------------------------------------------------------------  |
| ![Create Face Thermal Load](./images/CreateFaceThermalLoad.png)  |

## Description

Create GSA Face Thermal Load

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                       | <img width="200"/> Type                   | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                                                                                                                                                                                                                                                |
| -------------------------------------------- | ----------------------------------------- | ------------------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  |
| ![LoadCaseParam](./images/LoadCaseParam.png) | [Load Case](gsagh-load-case-parameter.md) | **Load Case**                  | Load Case parameter                                                                                                                                                                                                                                                                                                                                            |
| ![GenericParam](./images/GenericParam.png)   | `Generic`                                 | **Loadable 2D Objects**        | List, Custom Material, 2D Property, 2D Elements or 2D Members to apply load to; either input Prop2d, Element2d, or Member2d, or a text string.<br />Text string with Element list should take the form:<br /> 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)<br />Refer to help file for definition of lists and full vocabulary.  |
| ![TextParam](./images/TextParam.png)         | `Text`                                    | **Name**                       | Load Name                                                                                                                                                                                                                                                                                                                                                      |
| ![NumberParam](./images/NumberParam.png)     | `Number`                                  | **Value**                      | Load Value                                                                                                                                                                                                                                                                                                                                                     |

### Output parameters

| <img width="20"/> Icon               | <img width="200"/> Type         | <img width="200"/> Name        | <img width="1000"/> Description |
| ------------------------------------ | ------------------------------- | ------------------------------ | ------------------------------  |
| ![LoadParam](./images/LoadParam.png) | [Load](gsagh-load-parameter.md) | **Face Thermal Load**          | GSA Face Thermal Load           |
