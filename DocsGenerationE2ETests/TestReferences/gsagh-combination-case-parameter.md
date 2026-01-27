# Combination Case
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                     |
| ----------------------------------------------------------  |
| ![CombinationCaseParam](./images/CombinationCaseParam.png)  |

## Description

Combination cases are similar to analysis cases but differ in two respects: 
- Results for combination cases are inferred from analysis case results and not calculated explicitly.
- Combination cases can be enveloping cases, as described in [Enveloping](/references/envelopingingsa.md) in GSA.

Refer to [Combination Cases](/references/hidr-data-comb-case.md) to read more.



## Properties

| <img width="20"/> Icon                     | <img width="200"/> Type        | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                                            |
| ------------------------------------------ | ------------------------------ | ------------------------------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------  |
| ![IntegerParam](./images/IntegerParam.png) | `Integer`                      | **ID**                         | Combination Case number                                                                                                                                    |
| ![TextParam](./images/TextParam.png)       | `Text`                         | **Name**                       | Combination Case Name                                                                                                                                      |
| ![TextParam](./images/TextParam.png)       | `Text`                         | **Description**                | The description should take the form: 1.5A1 + 0.4A3.
Use 'or' for enveloping cases eg (1 or -1.4)A1,
'to' for enveloping a range of cases eg (C1 to C3)  |

_Note: the above properties can be retrieved using the [Combination Case Info](gsagh-combination-case-info-component.md) component_
