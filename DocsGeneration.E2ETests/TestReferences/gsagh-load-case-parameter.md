# Load Case
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![LoadCaseParam](./images/LoadCaseParam.png) |

## Description

When [Load](gsagh-load-parameter.md)s are applied to the model they are assigned to a load case. Load cases are a convenient way of grouping together a collection of loads that are to be considered acting together, for instance dead loads or live loads. In GSA, the load cases are only used to group loading and are not used directly for analysis. Refer to [Load Case Specification](/references/hidr-data-load-title.md) to read more.



## Properties

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Case ID** |Load Case number |
|![TextParam](./images/TextParam.png) |`Text` |**Name** |Load Case Name |
|![TextParam](./images/TextParam.png) |`Text` |**Type** |Load Case Type |

_Note: the above properties can be retrieved using the [Load Case Info](gsagh-load-case-info-component.md) component_
