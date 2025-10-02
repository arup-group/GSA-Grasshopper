# Create Model
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Create Model](./images/CreateModel.png) |

## Description

Assemble a GSA Model

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![GenericParam](./images/GenericParam.png) |`Generic` _List_ |**Model(s), Lists and Grid Lines** |Existing Model(s) to append to, Lists and Grid Lines<br />If you input more than one model they will be merged<br />with first model in list taking priority for IDs |
|![GenericParam](./images/GenericParam.png) |`Generic` _List_ |**Properties** |Sections (PB), Prop2Ds (PA) and Prop3Ds (PV) to add/set in the model<br />Properties already added to Elements or Members<br />will automatically be added with Geometry input |
|![GenericParam](./images/GenericParam.png) |`Generic` _List_ |**GSA Geometry** |Nodes, Element1Ds, Element2Ds, Member1Ds, Member2Ds and Member3Ds to add/set in model |
|![GenericParam](./images/GenericParam.png) |`Generic` _List_ |**Load** |Loads to add to the model<br />You can also use this input to add Edited GridPlaneSurfaces |
|![GenericParam](./images/GenericParam.png) |`Generic` _List_ |**Analysis Tasks & Combinations** |Analysis Tasks and Combination Cases to add to the model |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![ModelParam](./images/ModelParam.png) |[Model](gsagh-model-parameter.md) |**Model** |GSA Model parameter |


