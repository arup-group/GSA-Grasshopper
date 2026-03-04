# Analysis Task
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                               |
| ----------------------------------------------------  |
| ![AnalysisTaskParam](./images/AnalysisTaskParam.png)  |

## Description

An analysis task is a package of work for the solver. Thus we can have a static analysis task, a modal analysis task, etc. Each analysis task has one or more analysis case(s). The distinction is that the cases corresponds to result sets and define items such as loading (in the static case) while the task describes what the solver has to do. 

Refer to [Analysis Tasks](/references/analysistasks.md) to read more.



## Properties

| <img width="20"/> Icon                               | <img width="200"/> Type                                  | <img width="200"/> Name        | <img width="1000"/> Description       |
| ---------------------------------------------------- | -------------------------------------------------------- | ------------------------------ | ------------------------------------  |
| ![TextParam](./images/TextParam.png)                 | `Text`                                                   | **Name**                       | Analysis Task Name                    |
| ![AnalysisCaseParam](./images/AnalysisCaseParam.png) | [Analysis Case](gsagh-analysis-case-parameter.md) _List_ | **Analysis Case(s)**           | List of GSA Analysis Case             |
| ![TextParam](./images/TextParam.png)                 | `Text`                                                   | **Solver Type**                | Solver Type                           |
| ![IntegerParam](./images/IntegerParam.png)           | `Integer`                                                | **Task ID**                    | The Task number of the Analysis Task  |

_Note: the above properties can be retrieved using the [Analysis Task Info](gsagh-analysis-task-info-component.md) component_
