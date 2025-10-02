# GWA Command
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![GWA Command](./images/GwaCommand.png) |

## Description

Create a model from a GWA string, inject data into a model using GWA command, or retrieve model data or results through a GWA command.

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![ModelParam](./images/ModelParam.png) |[Model](gsagh-model-parameter.md) |**GSA Model** |[Optional] Existing model to inject GWA command(s) into. Leave this input empty to create a new model from GWA string. |
|![TextParam](./images/TextParam.png) |`Text` _List_ |**GWA string** |GWA string from GSA. Right-click on any data, and select copy all. Paste into notepad to check the data. <br />This input takes a a list of text strings that will automatically be joined. Construct a tree structure if you want to create multiple files. <br />The syntax of the command is based on GWA syntax and the units follow the GWA unit syntax; –<br />Refer to the “Keywords” document for details.<br />Note that locale is set to use '.' as decimal separator.<br />Right-click -> Help for further infor on GWA Commands. |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![ModelParam](./images/ModelParam.png) |[Model](gsagh-model-parameter.md) |**Model** |GSA Model parameter |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Returned result** |The 'variant' return value from executing a GWA command issued to GSA. <br />The syntax of the command is based on GWA syntax and the units follow the GWA unit syntax; –<br />Refer to the “GSA Keywords” document for details.<br />Note that locale is set to use '.' as decimal separator.<br />Right-click -> Help for further infor on GWA Commands. |
