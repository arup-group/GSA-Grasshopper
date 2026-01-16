# Load Properties
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                          |
| -----------------------------------------------  |
| ![Load Properties](./images/LoadProperties.png)  |

## Description

Get properties of a GSA Load

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon               | <img width="200"/> Type         | <img width="200"/> Name        | <img width="1000"/> Description |
| ------------------------------------ | ------------------------------- | ------------------------------ | ------------------------------  |
| ![LoadParam](./images/LoadParam.png) | [Load](gsagh-load-parameter.md) | **Load**                       | Load to get some info out of.   |

### Output parameters

| <img width="20"/> Icon                                       | <img width="200"/> Type                                     | <img width="200"/> Name        | <img width="1000"/> Description                                                                           |
| ------------------------------------------------------------ | ----------------------------------------------------------- | ------------------------------ | --------------------------------------------------------------------------------------------------------  |
| ![LoadCaseParam](./images/LoadCaseParam.png)                 | [Load Case](gsagh-load-case-parameter.md)                   | **Load Case**                  | GSA Load Case parameter                                                                                   |
| ![TextParam](./images/TextParam.png)                         | `Text`                                                      | **Name**                       | Load name                                                                                                 |
| ![GenericParam](./images/GenericParam.png)                   | `Generic`                                                   | **Definition**                 | Node, Element or Member list that load is applied to or Grid point / polygon definition                   |
| ![IntegerParam](./images/IntegerParam.png)                   | `Integer`                                                   | **Axis**                       | Axis Property (0 : Global // -1 : Local                                                                   |
| ![TextParam](./images/TextParam.png)                         | `Text`                                                      | **Direction**                  | Load direction                                                                                            |
| ![BooleanParam](./images/BooleanParam.png)                   | `Boolean`                                                   | **Projected**                  | Projected                                                                                                 |
| ![LoadParam](./images/LoadParam.png)                         | `Load   Value  or   Factor   X  [k N , k N /m , k N /m ²]`  | **Load Value or Factor X**     | Value at Start, Point 1 or Factor X.
Expression for Face Equation load.                                   |
| ![LoadParam](./images/LoadParam.png)                         | `Load   Value  or   Factor   Y  [k N , k N /m , k N /m ²]`  | **Load Value or Factor Y**     | Value at End, Point 2 or Factor Y.
Position X for Face Point load.
Equation Axis for Face Equation load.  |
| ![LoadParam](./images/LoadParam.png)                         | `Load   Value  or   Factor   Z  [k N , k N /m , k N /m ²]`  | **Load Value or Factor Z**     | Value at Point 3 or Factor Z.
Position Y for Face Point load.
Is Constant for Face Equation load.         |
| ![LoadParam](./images/LoadParam.png)                         | `Load   Value  [k N , k N /m , k N /m ²]`                   | **Load Value**                 | Value at Point 4.
Units of the equation for Face Equation load                                            |
| ![GridPlaneSurfaceParam](./images/GridPlaneSurfaceParam.png) | [Grid Plane Surface](gsagh-grid-plane-surface-parameter.md) | **Grid Plane Surface**         | GSA Grid Plane Surface parameter                                                                          |
