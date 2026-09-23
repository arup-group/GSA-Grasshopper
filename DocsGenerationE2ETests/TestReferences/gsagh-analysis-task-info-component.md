# Analysis Task Info
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                               |
| ----------------------------------------------------  |
| ![Analysis Task Info](./images/AnalysisTaskInfo.png)  |

## Description

Get information about a GSA Analysis Task

### Input parameters

| <img width="20"/> Icon                               | <img width="200"/> Type                           | <img width="200"/> Name        | <img width="1000"/> Description |
| ---------------------------------------------------- | ------------------------------------------------- | ------------------------------ | ------------------------------  |
| ![AnalysisTaskParam](./images/AnalysisTaskParam.png) | [Analysis Task](gsagh-analysis-task-parameter.md) | **Analysis Task**              | Analysis Task                   |

### Output parameters

| <img width="20"/> Icon                               | <img width="200"/> Type                                  | <img width="200"/> Name        | <img width="1000"/> Description       |
| ---------------------------------------------------- | -------------------------------------------------------- | ------------------------------ | ------------------------------------  |
| ![TextParam](./images/TextParam.png)                 | `Text`                                                   | **Name**                       | Analysis Task Name                    |
| ![AnalysisCaseParam](./images/AnalysisCaseParam.png) | [Analysis Case](gsagh-analysis-case-parameter.md) _List_ | **Analysis Case(s)**           | List of GSA Analysis Case             |
| ![TextParam](./images/TextParam.png)                 | `Text`                                                   | **Solver Type**                | Solver Type                           |
| ![IntegerParam](./images/IntegerParam.png)           | `Integer`                                                | **Task ID**                    | The Task number of the Analysis Task  |
