# Edit Material
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Edit Material](./images/EditMaterial.png) |

## Description

Modify a GSA Material

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![MaterialParam](./images/MaterialParam.png) |[Material](gsagh-material-parameter.md) |**Material** |Material parameter |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Material ID** |[Optional] Set Material ID corrosponding to the desired ID in the material type's table (Steel, Concrete, etc). |
|![TextParam](./images/TextParam.png) |`Text` |**Material Name** |[Optional] Set Material Name |
|![MaterialParam](./images/MaterialParam.png) |[Material](gsagh-material-parameter.md) |**Analysis Material** |Material[Optional] Input another Material to overwrite the analysis material properties.Material |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Material Type** |[Optional] Set Material Type for a Custom Material (only).<br />Input either text string or integer:<br />Generic : 0<br />Steel : 1<br />Concrete : 2<br />Aluminium : 3<br />Glass : 4<br />FRP : 5<br />Timber : 7<br />Fabric : 8 |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![MaterialParam](./images/MaterialParam.png) |[Material](gsagh-material-parameter.md) |**Material** |GSA Material with applied changes. |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Material ID** |the Material's ID in its respective table (Steel, Concrete, etc) |
|![TextParam](./images/TextParam.png) |`Text` |**Material Name** |the Material's Name |
|![MaterialParam](./images/MaterialParam.png) |[Material](gsagh-material-parameter.md) |**Custom Material** |A copy of this material as a Custom material. |
|![TextParam](./images/TextParam.png) |`Text` |**Material Type** |Material Type |
