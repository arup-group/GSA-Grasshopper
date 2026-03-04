# Create Grid Area Load
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                    |
| ---------------------------------------------------------  |
| ![Create Grid Area Load](./images/CreateGridAreaLoad.png)  |

## Description

Create GSA Grid Area Load

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                       | <img width="200"/> Type                   | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                                                       |
| -------------------------------------------- | ----------------------------------------- | ------------------------------ | --------------------------------------------------------------------------------------------------------------------------------------------------------------------  |
| ![LoadCaseParam](./images/LoadCaseParam.png) | [Load Case](gsagh-load-case-parameter.md) | **Load Case**                  | Load Case parameter                                                                                                                                                   |
| ![BrepParam](./images/BrepParam.png)         | `Brep`                                    | **Brep**                       | [Optional] Brep. If no input the whole plane method will be used. If both Grid Plane Surface and Brep are inputted, this Brep will be projected onto the Grid Plane.  |
| ![GenericParam](./images/GenericParam.png)   | `Generic`                                 | **Grid Plane Surface**         | Grid Plane Surface or Plane [Optional]. If no input here then the brep's best-fit plane will be used                                                                  |
| ![TextParam](./images/TextParam.png)         | `Text`                                    | **Direction**                  | Load direction (default z).<br />Accepted inputs are:<br />x<br />y<br />z                                                                                            |
| ![IntegerParam](./images/IntegerParam.png)   | `Integer`                                 | **Axis**                       | Load axis (default Global). <br />Accepted inputs are:<br />0 : Global<br />-1 : Local                                                                                |
| ![BooleanParam](./images/BooleanParam.png)   | `Boolean`                                 | **Projected**                  | Projected (default not)                                                                                                                                               |
| ![TextParam](./images/TextParam.png)         | `Text`                                    | **Name**                       | Load Name                                                                                                                                                             |
| ![NumberParam](./images/NumberParam.png)     | `Number`                                  | **Value**                      | Load Value                                                                                                                                                            |

### Output parameters

| <img width="20"/> Icon               | <img width="200"/> Type         | <img width="200"/> Name        | <img width="1000"/> Description |
| ------------------------------------ | ------------------------------- | ------------------------------ | ------------------------------  |
| ![LoadParam](./images/LoadParam.png) | [Load](gsagh-load-parameter.md) | **Grid Area Load**             | GSA Grid Area Load              |
