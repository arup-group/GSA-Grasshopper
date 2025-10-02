# Material Quantities
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Material Quantities](./images/MaterialQuantities.png) |

## Description

Get Quantities for Standard and Custom Materials from a GSA model

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![ModelParam](./images/ModelParam.png) |[Model](gsagh-model-parameter.md) |**Model** |Model parameter |
|![ListParam](./images/ListParam.png) |[List](gsagh-list-parameter.md) |**Element/Member filter List** |Filter the Elements or Members by list. (by default 'all')<br />Element/Member list should take the form:<br /> 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)<br />Refer to help file for definition of lists and full vocabulary. |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![GenericParam](./images/GenericParam.png) |`Generic` _Tree_ |**Steel Quantities** |Total weight of Steel Materials from GSA Model
Grafted by Material ID. |
|![GenericParam](./images/GenericParam.png) |`Generic` _Tree_ |**Concrete Quantities** |Total weight of Concrete Materials from GSA Model
Grafted by Material ID. |
|![GenericParam](./images/GenericParam.png) |`Generic` _Tree_ |**FRP Quantities** |Total weight of FRP Materials from GSA Model
Grafted by Material ID. |
|![GenericParam](./images/GenericParam.png) |`Generic` _Tree_ |**Aluminium Quantities** |Total weight of Aluminium Materials from GSA Model
Grafted by Material ID. |
|![GenericParam](./images/GenericParam.png) |`Generic` _Tree_ |**Timber Quantities** |Total weight of Timber Materials from GSA Model
Grafted by Material ID. |
|![GenericParam](./images/GenericParam.png) |`Generic` _Tree_ |**Glass Quantities** |Total weight of Glass Materials from GSA Model
Grafted by Material ID. |
|![GenericParam](./images/GenericParam.png) |`Generic` _Tree_ |**Custom Quantities** |Total weight of Custom Analysis Materials from GSA Model
Grafted by Material ID. |


