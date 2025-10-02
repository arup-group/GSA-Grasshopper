# Create List
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Create List](./images/CreateList.png) |

## Description

Create a GSA List with Name, Type and Definition or reference objects (Nodes, Elements, Members).
You can add a GSA List to a model through the 'GSA' input.

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Index** |[Optional] List Number - set this to 0 to append it to the end of the list of lists, or set the ID to overwrite an existing list. |
|![TextParam](./images/TextParam.png) |`Text` |**Name** |[Optional] List Name |
|![GenericParam](./images/GenericParam.png) |`Generic` _List_ |**Definition** |Definition as text or list of object (Nodes, Elements, Members) |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![ListParam](./images/ListParam.png) |[List](gsagh-list-parameter.md) |**GSA List** |GSA Entity List parameter.<br />You can add a GSA List to a model through the 'GSA' input. |
