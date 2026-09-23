# Create Grid Surface
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                 |
| ------------------------------------------------------  |
| ![Create Grid Surface](./images/CreateGridSurface.png)  |

## Description

Create GSA Grid Surface

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                     | <img width="200"/> Type        | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                                                                                                                                                                                                                                                                                                               |
| ------------------------------------------ | ------------------------------ | ------------------------------ | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  |
| ![GenericParam](./images/GenericParam.png) | `Generic`                      | **Grid Plane**                 | Grid Plane. If no input, Global XY-plane will be used                                                                                                                                                                                                                                                                                                                                                                         |
| ![IntegerParam](./images/IntegerParam.png) | `Integer`                      | **Grid Surface ID**            | Grid Surface ID. Setting this will replace any existing Grid Surfaces in model                                                                                                                                                                                                                                                                                                                                                |
| ![GenericParam](./images/GenericParam.png) | `Generic`                      | **Loadable Objects**           | Lists, Custom Materials, Properties, Elements or Members to which load should be expanded to (by default 'All'); either input Section, Prop2d, Prop3d, Element1d, Element2d, Member1d, Member2d or Member3d, or a text string.<br />Element list should take the form:<br /> 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)<br />Refer to help file for definition of lists and full vocabulary.  |
| ![TextParam](./images/TextParam.png)       | `Text`                         | **Name**                       | Grid Surface Name                                                                                                                                                                                                                                                                                                                                                                                                             |
| ![GenericParam](./images/GenericParam.png) | `Generic`                      | **Tolerance in model units**   | Tolerance for Load Expansion (default 10mm)                                                                                                                                                                                                                                                                                                                                                                                   |
| ![NumberParam](./images/NumberParam.png)   | `Number`                       | **Span Direction**             | Span Direction between -Pi and Pi                                                                                                                                                                                                                                                                                                                                                                                             |

### Output parameters

| <img width="20"/> Icon                                       | <img width="200"/> Type                                     | <img width="200"/> Name        | <img width="1000"/> Description |
| ------------------------------------------------------------ | ----------------------------------------------------------- | ------------------------------ | ------------------------------  |
| ![GridPlaneSurfaceParam](./images/GridPlaneSurfaceParam.png) | [Grid Plane Surface](gsagh-grid-plane-surface-parameter.md) | **Grid Surface**               | GSA Grid Surface                |
