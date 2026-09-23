# Create Grid Line Load
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                    |
| ---------------------------------------------------------  |
| ![Create Grid Line Load](./images/CreateGridLineLoad.png)  |

## Description

Create GSA Grid Line Load

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                       | <img width="200"/> Type                                         | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                                                                                                |
| -------------------------------------------- | --------------------------------------------------------------- | ------------------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  |
| ![LoadCaseParam](./images/LoadCaseParam.png) | [Load Case](gsagh-load-case-parameter.md)                       | **Load Case**                  | Load Case parameter                                                                                                                                                                                            |
| ![CurveParam](./images/CurveParam.png)       | `Curve`                                                         | **PolyLine**                   | PolyLine. If you input grid plane below only x and y coordinate positions will be used from this polyline, but if not a new Grid Plane Surface (best-fit plane) will be created from PolyLine control points.  |
| ![GenericParam](./images/GenericParam.png)   | `Generic`                                                       | **Grid Plane Surface**         | Grid Plane Surface or Plane [Optional]. If no input here then the line's best-fit plane will be used                                                                                                           |
| ![TextParam](./images/TextParam.png)         | `Text`                                                          | **Direction**                  | Load direction (default z).<br />Accepted inputs are:<br />x<br />y<br />z                                                                                                                                     |
| ![IntegerParam](./images/IntegerParam.png)   | `Integer`                                                       | **Axis**                       | Load axis (default Global). <br />Accepted inputs are:<br />0 : Global<br />-1 : Local                                                                                                                         |
| ![BooleanParam](./images/BooleanParam.png)   | `Boolean`                                                       | **Projected**                  | Projected (default not)                                                                                                                                                                                        |
| ![TextParam](./images/TextParam.png)         | `Text`                                                          | **Name**                       | Load Name                                                                                                                                                                                                      |
| ![UnitNumber](./images/UnitParam.png)        | [Unit Number](gsagh-unitnumber-parameter.md) `Force Per Length` | **Value Start**                | Load Value at Start of Line                                                                                                                                                                                    |
| ![UnitNumber](./images/UnitParam.png)        | [Unit Number](gsagh-unitnumber-parameter.md) `Force Per Length` | **Value End**                  | Load Value at End of Line (default : Start Value)                                                                                                                                                              |

### Output parameters

| <img width="20"/> Icon               | <img width="200"/> Type         | <img width="200"/> Name        | <img width="1000"/> Description |
| ------------------------------------ | ------------------------------- | ------------------------------ | ------------------------------  |
| ![LoadParam](./images/LoadParam.png) | [Load](gsagh-load-parameter.md) | **Grid Line Load**             | GSA Grid Line Load              |
