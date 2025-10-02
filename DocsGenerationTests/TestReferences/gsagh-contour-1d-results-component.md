# Contour 1D Results
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Contour 1D Results](./images/Contour1dResults.png) |

## Description

Displays GSA 1D Element Results as Contour

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![ResultParam](./images/ResultParam.png) |[Result](gsagh-result-parameter.md) |**Result** |Result |
|![ListParam](./images/ListParam.png) |[List](gsagh-list-parameter.md) |**Element/Member filter List** |Filter the Elements or Members by list. (by default 'all')<br />Element/Member list should take the form:<br /> 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)<br />Refer to help file for definition of lists and full vocabulary. |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Intermediate Points** |Number of intermediate equidistant points (default 10) |
|![ColourParam](./images/ColourParam.png) |`Colour` _List_ |**Colour** |[Optional] List of colours to override default colours<br />A new gradient will be created from the input list of colours |
|![IntervalParam](./images/IntervalParam.png) |`Interval` |**Min/Max Domain** |Optional Domain for custom Min to Max contour colours |
|![NumberParam](./images/NumberParam.png) |`Number` |**Scale** |Scale the result display size |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![GenericParam](./images/GenericParam.png) |`Generic` _Tree_ |**Result Line** |Contoured Line segments with result values |
|![GenericParam](./images/GenericParam.png) |`Generic` _List_ |**Colours** |Legend Colours |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` _List_ |**Values** |Legend Values |
