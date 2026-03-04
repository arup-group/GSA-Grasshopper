# Nodal Forces and Moments
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                          |
| ---------------------------------------------------------------  |
| ![Nodal Forces and Moments](./images/NodalForcesAndMoments.png)  |

## Description

Nodal Force and Moment result values

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                     | <img width="200"/> Type                    | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                                                                                    |
| ------------------------------------------ | ------------------------------------------ | ------------------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  |
| ![ResultParam](./images/ResultParam.png)   | [Result](gsagh-result-parameter.md) _List_ | **Result**                     | Result                                                                                                                                                                                             |
| ![ListParam](./images/ListParam.png)       | [List](gsagh-list-parameter.md)            | **Node filter list**           | Filter the Nodes by list. (by default 'all')<br />Node list should take the form:<br /> 1 11 to 72 step 2 not (XY3 31 to 45)<br />Refer to help file for definition of lists and full vocabulary.  |
| ![IntegerParam](./images/IntegerParam.png) | `Integer`                                  | **Axis**                       | Standard Axis: Global (0), Local (-1), Natural (-2), Default (-10), XElevation (-11), YElevation (-12), GlobalCylindrical (-13), Vertical (-14)                                                    |

### Output parameters

| <img width="20"/> Icon                     | <img width="200"/> Type                                      | <img width="200"/> Name        | <img width="1000"/> Description              |
| ------------------------------------------ | ------------------------------------------------------------ | ------------------------------ | -------------------------------------------  |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Force` _Tree_  | **Force X**                    | \* Nodal Force in X-direction                |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Force` _Tree_  | **Force Y**                    | \* Nodal Force in Y-direction                |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Force` _Tree_  | **Force Z**                    | \* Nodal Force in Z-direction                |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Force` _Tree_  | **Force &#124;F&#124;**        | \* Combined &#124;XYZ&#124; Nodal Force      |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Moment` _Tree_ | **Moment XX**                  | \* Nodal Moment around X-axis                |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Moment` _Tree_ | **Moment YY**                  | \* Nodal Moment around Y-axis                |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Moment` _Tree_ | **Moment ZZ**                  | \* Nodal Moment around Z-axis                |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Moment` _Tree_ | **Moment &#124;M&#124;**       | \* Combined &#124;XXYYZZ&#124; Nodal Moment  |
| ![IntegerParam](./images/IntegerParam.png) | `Integer` _List_                                             | **Element IDs**                | Element IDs for each result value            |
