# Design Task Info
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Design Task Info](./images/DesignTaskInfo.png) |

## Description

Get GSA Steel Design Tasks information

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![DesignTaskParam](./images/DesignTaskParam.png) |[Design Task](gsagh-design-task-parameter.md) |**Design Task** |Design Task parameter |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![TextParam](./images/TextParam.png) |`Text` |**Name** |Task Name |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Number** |Set Task Number. If ID is set it will replace any existing DesignTasks in the model |
|![ListParam](./images/ListParam.png) |[List](gsagh-list-parameter.md) |**Definition** |Members List definition |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**CombinationCase** |Combination Case ID |
|![NumberParam](./images/NumberParam.png) |`Number` |**Target Utilisation** |Target overall utilisation (upper) |
|![NumberParam](./images/NumberParam.png) |`Number` |**Lower limit** |Lower utilisation limit (inefficiency warning) |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Grouped Design** |If true, Members with the same pool are assigned the same section |
|![TextParam](./images/TextParam.png) |`Text` |**Primary Objective** |Primary design optimisation objective |
|![TextParam](./images/TextParam.png) |`Text` |**Secondary Objective** |Secondary design optimisation objective |


