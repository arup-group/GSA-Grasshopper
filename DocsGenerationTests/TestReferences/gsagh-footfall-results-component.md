# Footfall Results
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Footfall Results](./images/FootfallResults.png) |

## Description

Node Resonant or Transient Footfall result values

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![ResultParam](./images/ResultParam.png) |[Result](gsagh-result-parameter.md) _List_ |**Result** |Result |
|![ListParam](./images/ListParam.png) |[List](gsagh-list-parameter.md) |**Node filter list** |Filter the Nodes by list. (by default 'all')<br />Node list should take the form:<br /> 1 11 to 72 step 2 not (XY3 31 to 45)<br />Refer to help file for definition of lists and full vocabulary. |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![NumberParam](./images/NumberParam.png) |`Number` _Tree_ |**Maximum Response Factor** |* The maximum response factor. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Speed ` _Tree_ |**Peak Velocity** |* The peak velocity. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Speed ` _Tree_ |**RMS Velocity** |* The root mean square (rms) veloocity. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Speed ` _Tree_ |**Peak Acceleration** |* The peak velocity. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Speed ` _Tree_ |**RMS Acceleration** |* The root mean square (rms) acceleration. |
|![IntegerParam](./images/IntegerParam.png) |`Integer` _Tree_ |**Critical Node** |* The node ID of the critical frequency |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Frequency ` _Tree_ |**Critical Frequency** |* The critical frequency |
|![IntegerParam](./images/IntegerParam.png) |`Integer` _Tree_ |**Node IDs** |Node IDs for each result value |
