# Create Analysis Task
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Create Analysis Task](./images/CreateAnalysisTask.png) |

## Description

Create a GSA Analysis Task

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Task ID** |The Task number of the Analysis Task |
|![TextParam](./images/TextParam.png) |`Text` |**Name** |Task Name |
|![GenericParam](./images/GenericParam.png) |`Generic` _List_ |**Analysis Cases** |List of Analysis Cases (if left empty, all load cases in model will be added) |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![AnalysisTaskParam](./images/AnalysisTaskParam.png) |[Analysis Task](gsagh-analysis-task-parameter.md) |**Analysis Task** |GSA Analysis Task parameter |
