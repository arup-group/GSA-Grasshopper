# Create Node Load
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                           |
| ------------------------------------------------  |
| ![Create Node Load](./images/CreateNodeLoad.png)  |

## Description

Create GSA Node Load

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                       | <img width="200"/> Type                              | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                                                                                                                                    |
| -------------------------------------------- | ---------------------------------------------------- | ------------------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  |
| ![LoadCaseParam](./images/LoadCaseParam.png) | [Load Case](gsagh-load-case-parameter.md)            | **Load Case**                  | Load Case parameter                                                                                                                                                                                                                                |
| ![GenericParam](./images/GenericParam.png)   | `Generic`                                            | **Node list**                  | Node or Point to apply load to; either input Node, Point, or a text string.<br />Text string with Node list should take the form:<br /> 1 11 to 72 step 2 not (XY3 31 to 45)<br />Refer to help file for definition of lists and full vocabulary.  |
| ![TextParam](./images/TextParam.png)         | `Text`                                               | **Name**                       | Load Name                                                                                                                                                                                                                                          |
| ![TextParam](./images/TextParam.png)         | `Text`                                               | **Direction**                  | Load direction (default z).<br />Accepted inputs are:<br />x<br />y<br />z<br />xx<br />yy<br />zz                                                                                                                                                 |
| ![UnitNumber](./images/UnitParam.png)        | [Unit Number](gsagh-unitnumber-parameter.md) `Force` | **Value**                      | Load Value                                                                                                                                                                                                                                         |

### Output parameters

| <img width="20"/> Icon               | <img width="200"/> Type         | <img width="200"/> Name        | <img width="1000"/> Description |
| ------------------------------------ | ------------------------------- | ------------------------------ | ------------------------------  |
| ![LoadParam](./images/LoadParam.png) | [Load](gsagh-load-parameter.md) | **Node Load**                  | GSA Node Load                   |
