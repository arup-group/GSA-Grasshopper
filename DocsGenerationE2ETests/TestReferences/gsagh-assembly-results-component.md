# Assembly Results
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                            |
| -------------------------------------------------  |
| ![Assembly Results](./images/AssemblyResults.png)  |

## Description

Displays GSA Assembly Results as Contour

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                       | <img width="200"/> Type             | <img width="200"/> Name        | <img width="1000"/> Description                                                                                            |
| -------------------------------------------- | ----------------------------------- | ------------------------------ | -------------------------------------------------------------------------------------------------------------------------  |
| ![ResultParam](./images/ResultParam.png)     | [Result](gsagh-result-parameter.md) | **Result**                     | Result                                                                                                                     |
| ![ListParam](./images/ListParam.png)         | [List](gsagh-list-parameter.md)     | **Assembly filter list**       | Filter the Assemblies by list. (by default 'all')                                                                          |
| ![ColourParam](./images/ColourParam.png)     | `Colour` _List_                     | **Colour**                     | [Optional] List of colours to override default colours<br />A new gradient will be created from the input list of colours  |
| ![IntervalParam](./images/IntervalParam.png) | `Interval`                          | **Min/Max Domain**             | Optional Domain for custom Min to Max contour colours                                                                      |
| ![NumberParam](./images/NumberParam.png)     | `Number`                            | **Scale**                      | Scale the result display size                                                                                              |

### Output parameters

| <img width="20"/> Icon                     | <img width="200"/> Type                                      | <img width="200"/> Name        | <img width="1000"/> Description             |
| ------------------------------------------ | ------------------------------------------------------------ | ------------------------------ | ------------------------------------------  |
| ![GenericParam](./images/GenericParam.png) | `Generic` _Tree_                                             | **Result Line**                | Contoured Line segments with result values  |
| ![GenericParam](./images/GenericParam.png) | `Generic` _List_                                             | **Colours**                    | Legend Colours                              |
| ![UnitNumber](./images/UnitParam.png)      | [Unit Number](gsagh-unitnumber-parameter.md) `Length` _List_ | **Values**                     | Legend Values                               |
