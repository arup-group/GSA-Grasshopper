# Preview 3D Sections
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Preview 3D Sections](./images/Preview3dSections.png) |

## Description

Show the 3D cross-section of 1D/2D GSA Elements and Members in a GSA model.

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![GenericParam](./images/GenericParam.png) |`Generic` _List_ |**Element/Member 1D/2D** |Element1D, Element2D, Member1D or Member2D to preview the 3D cross-section for.<br />You can also input models or lists with geometry to preview. |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![MeshParam](./images/MeshParam.png) |`Mesh` |**AnalysisLayer Mesh** |Analysis layer 3D Section Mesh |
|![LineParam](./images/LineParam.png) |`Line` _List_ |**AnalysisLayer Outlines** |The Analyis layer 3D Sections' outlines |
|![MeshParam](./images/MeshParam.png) |`Mesh` |**DesignLayer Mesh** |Design layer 3D Section Mesh |
|![LineParam](./images/LineParam.png) |`Line` _List_ |**DesignLayer Outlines** |The Design layer 3D Sections' outlines |


