# Contour Node Results
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Contour Node Results](./images/ContourNodeResults.png) |

## Description

Diplays GSA Node Results as Contours

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![ResultParam](./images/ResultParam.png) |[Result](gsagh-result-parameter.md) |**Result** |Result |
|![ListParam](./images/ListParam.png) |[List](gsagh-list-parameter.md) |**Node filter list** |Filter the Nodes by list. (by default 'all')<br />Node list should take the form:<br /> 1 11 to 72 step 2 not (XY3 31 to 45)<br />Refer to help file for definition of lists and full vocabulary. |
|![ColourParam](./images/ColourParam.png) |`Colour` _List_ |**Colour** |Optional list of colours to override default colours.<br />A new gradient will be created from the input list of colours |
|![IntervalParam](./images/IntervalParam.png) |`Interval` |**Min/Max Domain** |Optional Domain for custom Min to Max contour colours |
|![NumberParam](./images/NumberParam.png) |`Number` |**Scalar** |Scale the result display size |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![GenericParam](./images/GenericParam.png) |`Generic` _List_ |**Result Point** |Contoured Points with result values |
|![GenericParam](./images/GenericParam.png) |`Generic` _List_ |**Colours** |Legend Colours |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` _List_ |**Values** |Legend Values |


