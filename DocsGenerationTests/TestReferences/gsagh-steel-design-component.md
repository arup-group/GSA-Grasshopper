# Steel Design
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Steel Design](./images/SteelDesign.png) |

## Description

Optimise or check the steel sections in a Model

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![ModelParam](./images/ModelParam.png) |[Model](gsagh-model-parameter.md) |**Model** |Model parameter |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Task number** |[Optional] The ID of the Task to Design or Check.By default the first steel task will be run. |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Iterations** |Set this input iterate through the steps 
A) Design -> B) ElementsFromMembers -> C) Analyse -> A) Design
To only run the above loop once set the input to 1. |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Check only** |Set to true to only perform a check of the section capacities |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![ModelParam](./images/ModelParam.png) |[Model](gsagh-model-parameter.md) |**Model** |GSA Model parameter |
|![TextParam](./images/TextParam.png) |`Text` _List_ |**Errors** |Analysis Task Errors |
|![TextParam](./images/TextParam.png) |`Text` _List_ |**Warnings** |Analysis Task Warnings |
|![TextParam](./images/TextParam.png) |`Text` _List_ |**Remarks** |Analysis Task Notes and Remarks |
|![TextParam](./images/TextParam.png) |`Text` _List_ |**Logs** |Analysis Task logs |


