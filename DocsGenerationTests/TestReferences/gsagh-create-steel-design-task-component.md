# Create Steel Design Task
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Create Steel Design Task](./images/CreateSteelDesignTask.png) |

## Description

Create a GSA Steel Design Task

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![TextParam](./images/TextParam.png) |`Text` |**Name** |Task Name |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Number** |Set Task Number. If ID is set it will replace any existing DesignTasks in the model |
|![GenericParam](./images/GenericParam.png) |`Generic` _List_ |**Definition** |[Default = 'All'] Definition as text or list of object (Materials, Sections, Members) |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**CombinationCase** |Combination Case ID |
|![NumberParam](./images/NumberParam.png) |`Number` |**Target Utilisation** |Target overall utilisation (upper) |
|![NumberParam](./images/NumberParam.png) |`Number` |**Lower limit** |Lower utilisation limit (inefficiency warning) |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Grouped Design** |If true, Members with the same pool are assigned the same section |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![DesignTaskParam](./images/DesignTaskParam.png) |[Design Task](gsagh-design-task-parameter.md) |**Design Task** |GSA Design Task parameter |


