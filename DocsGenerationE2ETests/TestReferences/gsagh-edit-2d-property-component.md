# Edit 2D Property
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Edit 2D Property](./images/Edit2dProperty.png) |

## Description

Modify GSA 2D Property

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![Property2dParam](./images/Property2dParam.png) |[Property 2D](gsagh-property-2d-parameter.md) |**Property 2D** |2D Property (Area) to get or set information for. Leave blank to create a new Property 2D |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Prop2d Number** |Set 2D Property Number. If ID is set it will replace any existing 2D Property in the model |
|![TextParam](./images/TextParam.png) |`Text` |**Prop2d Name** |Set Name of 2D Proerty |
|![ColourParam](./images/ColourParam.png) |`Colour` |**Prop2d Colour** |Set 2D Property Colour |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Axis** |Input a Plane to set a custom Axis or input an integer (Global (0) or Topological (-1)) to reference a predefined Axis in the model |
|![TextParam](./images/TextParam.png) |`Text` |**Type** |Set 2D Property Type.<br />Input either text string or integer:<br />Plane Stress : 1<br />Plane Strain : 2<br />Axis Symmetric : 3<br />Fabric : 4<br />Plate : 5<br />Shell : 6<br />Curved Shell : 7<br />Torsion : 8<br />Wall : 9<br />Load : 10 |
|![MaterialParam](./images/MaterialParam.png) |[Material](gsagh-material-parameter.md) |**Material** |Material parameter |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` |**Thickness** |Set Property Thickness |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Reference Surface** |Reference Surface Middle (default) = 0, Top = 1, Bottom = 2 |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` |**Offset** |Additional Offset |
|![Property2dModifierParam](./images/Property2dModifierParam.png) |[Property 2D Modifier](gsagh-property-2d-modifier-parameter.md) |**Property 2D Modifier** |Property 2D Modifier parameter |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Support Type** |Set Load Panel Support Type.<br />Input either text string or integer:<br />Auto : 1<br />All Edges : 2<br />Three Edges : 3<br />Two Edges : 4<br />Two Adjacent Edges : 5<br />One Edge : 6<br />Cantilever : 7 |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Reference Edge** |Reference Edge for Load Panels with support type other than Auto and All Edges |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![Property2dParam](./images/Property2dParam.png) |[Property 2D](gsagh-property-2d-parameter.md) |**Property 2D** |GSA 2D Property (Area) with applied changes. |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Prop2d ID** |2D Property ID |
|![TextParam](./images/TextParam.png) |`Text` |**Name** |Name of 2D Proerty |
|![ColourParam](./images/ColourParam.png) |`Colour` |**Colour** |2D Property Colour |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Axis** |Local Axis either as `Plane` for custom local axis or an `Integer` (Global: 0 or Topological: 1) for a referenced Axis. |
|![TextParam](./images/TextParam.png) |`Text` |**Type** |2D Property Type |
|![MaterialParam](./images/MaterialParam.png) |[Material](gsagh-material-parameter.md) |**Material** |GSA Material parameter |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` |**Thickness** |Property Thickness |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Reference Surface** |Reference Surface Middle (default) = 0, Top = 1, Bottom = 2 |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` |**Offset** |Additional Offset |
|![Property2dModifierParam](./images/Property2dModifierParam.png) |[Property 2D Modifier](gsagh-property-2d-modifier-parameter.md) |**Property 2D Modifier** |GSA Property 2D Modifier parameter |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Support Type** |Support Type |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Reference Edge** |Reference Edge for Load Panels with support type other than Auto and All Edges |


