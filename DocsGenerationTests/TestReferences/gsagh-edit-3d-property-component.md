# Edit 3D Property
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Edit 3D Property](./images/Edit3dProperty.png) |

## Description

Modify a GSA 3D Property

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![Property3dParam](./images/Property3dParam.png) |[Property 3D](gsagh-property-3d-parameter.md) |**Property 3D** |3D Property (Volumetric) to get or set information for. Leave blank to create a new Property 3D |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Prop3d Number** |Set 3D Property Number. If ID is set it will replace any existing 3D Property in the model |
|![MaterialParam](./images/MaterialParam.png) |[Material](gsagh-material-parameter.md) |**Material** |Material parameter |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Axis** |Set Axis as integer: Global (0) or Topological (-1) |
|![TextParam](./images/TextParam.png) |`Text` |**Prop3d Name** |Set Name of 3D Proerty |
|![ColourParam](./images/ColourParam.png) |`Colour` |**Prop3d Colour** |Set 3D Property Colour |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![Property3dParam](./images/Property3dParam.png) |[Property 3D](gsagh-property-3d-parameter.md) |**Property 3D** |GSA 3D Property (Volumetric) with applied changes. |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Prop2d Number** |3D Property Number |
|![MaterialParam](./images/MaterialParam.png) |[Material](gsagh-material-parameter.md) |**Material** |GSA Material parameter |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Axis** |Axis: Global (0) or Topological (-1) |
|![TextParam](./images/TextParam.png) |`Text` |**Prop3d Name** |Name of 3D Proerty |
|![ColourParam](./images/ColourParam.png) |`Colour` |**Prop3d Colour** |3D Property Colour |
