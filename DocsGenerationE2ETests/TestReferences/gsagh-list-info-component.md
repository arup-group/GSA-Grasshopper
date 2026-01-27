# List Info
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon              |
| -----------------------------------  |
| ![List Info](./images/ListInfo.png)  |

## Description

Get information of like ID, Name, Type and Definition, as well as all objects (Nodes, Elements, Members or Cases) from a GSA List

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon               | <img width="200"/> Type         | <img width="200"/> Name        | <img width="1000"/> Description |
| ------------------------------------ | ------------------------------- | ------------------------------ | ------------------------------  |
| ![ListParam](./images/ListParam.png) | [List](gsagh-list-parameter.md) | **List**                       | Entity List parameter           |

### Output parameters

| <img width="20"/> Icon                     | <img width="200"/> Type        | <img width="200"/> Name        | <img width="1000"/> Description                       |
| ------------------------------------------ | ------------------------------ | ------------------------------ | ----------------------------------------------------  |
| ![IntegerParam](./images/IntegerParam.png) | `Integer`                      | **Index**                      | List Number if the list ever belonged to a GSA Model  |
| ![TextParam](./images/TextParam.png)       | `Text`                         | **Name**                       | List Name                                             |
| ![TextParam](./images/TextParam.png)       | `Text`                         | **Type**                       | Entity Type                                           |
| ![TextParam](./images/TextParam.png)       | `Text`                         | **Definition**                 | List Definition                                       |
| ![GenericParam](./images/GenericParam.png) | `Generic` _List_               | **List Objects**               | Expanded objects contained in the input list          |
| ![IntegerParam](./images/IntegerParam.png) | `Integer` _List_               | **Expand List**                | Expanded list IDs                                     |
