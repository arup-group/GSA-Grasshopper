# Total Loads and Reactions
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                            |
| -----------------------------------------------------------------  |
| ![Total Loads and Reactions](./images/TotalLoadsAndReactions.png)  |

## Description

Get Total Loads and Reaction Results from a GSA model

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                   | <img width="200"/> Type             | <img width="200"/> Name        | <img width="1000"/> Description |
| ---------------------------------------- | ----------------------------------- | ------------------------------ | ------------------------------  |
| ![ResultParam](./images/ResultParam.png) | [Result](gsagh-result-parameter.md) | **Result**                     | Result                          |

### Output parameters

| <img width="20"/> Icon                | <img width="200"/> Type                               | <img width="200"/> Name                | <img width="1000"/> Description                         |
| ------------------------------------- | ----------------------------------------------------- | -------------------------------------- | ------------------------------------------------------  |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Force`  | **Force X**                            | Sum of all Force Loads in GSA Model in X-direction      |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Force`  | **Force Y**                            | Sum of all Force Loads in GSA Model in Y-direction      |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Force`  | **Force Z**                            | Sum of all Force Loads in GSA Model in Z-direction      |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Force`  | **Force &#124;XYZ&#124;**              | Sum of all Force Loads in GSA Model                     |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Moment` | **Moment XX**                          | Sum of all Moment Loads in GSA Model around X-axis      |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Moment` | **Moment YY**                          | Sum of all Moment Loads in GSA Model around Y-axis      |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Moment` | **Moment ZZ**                          | Sum of all Moment Loads in GSA Model around Z-axis      |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Moment` | **Moment &#124;XXYYZZ&#124;**          | Sum of all Moment Loads in GSA Model                    |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Force`  | **Total Reaction X**                   | Sum of all Reaction Forces in GSA Model in X-direction  |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Force`  | **Total Reaction Y**                   | Sum of all Reaction Forces in GSA Model in Y-direction  |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Force`  | **Total Reaction Z**                   | Sum of all Reaction Forces in GSA Model in Z-direction  |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Force`  | **Total Reaction &#124;XYZ&#124;**     | Sum of all Reaction Forces in GSA Model                 |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Moment` | **Total Reaction XX**                  | Sum of all Reaction Moments in GSA Model around X-axis  |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Moment` | **Total Reaction XX **                 | Sum of all Reaction Moments in GSA Model around Y-axis  |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Moment` | **Total Reaction XX **                 | Sum of all Reaction Moments in GSA Model around Z-axis  |
| ![UnitNumber](./images/UnitParam.png) | [Unit Number](gsagh-unitnumber-parameter.md) `Moment` | **Total Reaction &#124;XXYYZZ&#124; ** | Sum of all Reaction Moments in GSA Model                |
