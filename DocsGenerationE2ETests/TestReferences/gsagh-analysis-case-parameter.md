# Analysis Case
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                               |
| ----------------------------------------------------  |
| ![AnalysisCaseParam](./images/AnalysisCaseParam.png)  |

## Description

Analysis Case definition, for instance `L1` for LoadCase 1 or `L1 + L2` for combining multiple load cases in one Analysis case. Refer to [Analysis cases](/references/analysiscases.md) to read more.



## Properties

| <img width="20"/> Icon                     | <img width="200"/> Type        | <img width="200"/> Name        | <img width="1000"/> Description                                |
| ------------------------------------------ | ------------------------------ | ------------------------------ | -------------------------------------------------------------  |
| ![TextParam](./images/TextParam.png)       | `Text`                         | **Name**                       | Analysis Case Name                                             |
| ![TextParam](./images/TextParam.png)       | `Text`                         | **Definition**                 | The definition of the analysis case                            |
| ![IntegerParam](./images/IntegerParam.png) | `Integer`                      | **CaseID**                     | The Case number if the Analysis Case ever belonged to a model  |

_Note: the above properties can be retrieved using the [Analysis Case Info](gsagh-analysis-case-info-component.md) component_
