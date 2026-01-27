# Property Quantities
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                  |
| -------------------------------------------------------  |
| ![Property Quantities](./images/PropertyQuantities.png)  |

## Description

Get Quantities for Sections, and 2D Properties from a GSA model

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                 | <img width="200"/> Type           | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                                                                                                                                                  |
| -------------------------------------- | --------------------------------- | ------------------------------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  |
| ![ModelParam](./images/ModelParam.png) | [Model](gsagh-model-parameter.md) | **Model**                      | Model parameter                                                                                                                                                                                                                                                  |
| ![ListParam](./images/ListParam.png)   | [List](gsagh-list-parameter.md)   | **Element/Member filter List** | Filter the Elements or Members by list. (by default 'all')<br />Element/Member list should take the form:<br /> 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)<br />Refer to help file for definition of lists and full vocabulary.  |

### Output parameters

| <img width="20"/> Icon                     | <img width="200"/> Type        | <img width="200"/> Name        | <img width="1000"/> Description                                          |
| ------------------------------------------ | ------------------------------ | ------------------------------ | -----------------------------------------------------------------------  |
| ![GenericParam](./images/GenericParam.png) | `Generic` _Tree_               | **Section Quantities**         | Total Length per Section Property from GSA Model
Grafted by Section ID.  |
| ![GenericParam](./images/GenericParam.png) | `Generic` _Tree_               | **2D Property Quantities**     | Total Area per 2D Property from GSA Model.
Grafted by Property ID.       |
