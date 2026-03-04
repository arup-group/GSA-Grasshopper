# Design Task
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                           |
| ------------------------------------------------  |
| ![DesignTaskParam](./images/DesignTaskParam.png)  |

## Description

A Design Task is collection of specifications that guide the automated, iterative design or checking of members. A Design Task is analogous to an Analysis Task in that there can be multiple Design Tasks all of which are saved with the model. Design Tasks must be executed to carry out either a design or a check based on the parameters defined in the task. 

In Grasshopper, it is only possible to create steel design tasks.

Refer to [Design Tasks](/references/sbs-steeldesign/) to read more.



## Properties

| <img width="20"/> Icon                     | <img width="200"/> Type         | <img width="200"/> Name        | <img width="1000"/> Description                                                      |
| ------------------------------------------ | ------------------------------- | ------------------------------ | -----------------------------------------------------------------------------------  |
| ![TextParam](./images/TextParam.png)       | `Text`                          | **Name**                       | Task Name                                                                            |
| ![IntegerParam](./images/IntegerParam.png) | `Integer`                       | **Number**                     | Set Task Number. If ID is set it will replace any existing DesignTasks in the model  |
| ![ListParam](./images/ListParam.png)       | [List](gsagh-list-parameter.md) | **Definition**                 | Members List definition                                                              |
| ![IntegerParam](./images/IntegerParam.png) | `Integer`                       | **CombinationCase**            | Combination Case ID                                                                  |
| ![NumberParam](./images/NumberParam.png)   | `Number`                        | **Target Utilisation**         | Target overall utilisation (upper)                                                   |
| ![NumberParam](./images/NumberParam.png)   | `Number`                        | **Lower limit**                | Lower utilisation limit (inefficiency warning)                                       |
| ![BooleanParam](./images/BooleanParam.png) | `Boolean`                       | **Grouped Design**             | If true, Members with the same pool are assigned the same section                    |
| ![TextParam](./images/TextParam.png)       | `Text`                          | **Primary Objective**          | Primary design optimisation objective                                                |
| ![TextParam](./images/TextParam.png)       | `Text`                          | **Secondary Objective**        | Secondary design optimisation objective                                              |

_Note: the above properties can be retrieved using the [Design Task Info](gsagh-design-task-info-component.md) component_
