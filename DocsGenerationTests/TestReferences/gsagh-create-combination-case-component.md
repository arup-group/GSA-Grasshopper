# Create Combination Case
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Create Combination Case](./images/CreateCombinationCase.png) |

## Description

Create a new GSA Combination Case

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**ID** |Combination Case number (use '0' to append Combination case) |
|![TextParam](./images/TextParam.png) |`Text` |**Name** |Case Name |
|![TextParam](./images/TextParam.png) |`Text` |**Description** |The description should take the form: 1.5A1 + 0.4A3.<br />Use 'or' for enveloping cases eg (1 or -1.4)A1,<br />'to' for enveloping a range of cases eg (C1 to C3) |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![CombinationCaseParam](./images/CombinationCaseParam.png) |[Combination Case](gsagh-combination-case-parameter.md) |**Combination Case** |GSA Combination Case parameter |
