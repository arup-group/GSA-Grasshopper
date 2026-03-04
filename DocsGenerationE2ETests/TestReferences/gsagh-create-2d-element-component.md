# Create 2D Element
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                             |
| --------------------------------------------------  |
| ![Create 2D Element](./images/Create2dElement.png)  |

## Description

Create GSA 2D Element

_Note: This component can preview 3D Sections, right-click the middle of the component to toggle the section preview._

### Input parameters

| <img width="20"/> Icon                           | <img width="200"/> Type                       | <img width="200"/> Name        | <img width="1000"/> Description                                                              |
| ------------------------------------------------ | --------------------------------------------- | ------------------------------ | -------------------------------------------------------------------------------------------  |
| ![IGeometricParam](./images/IGeometricParam.png) | `Geometry`                                    | **Geometry**                   | Polyline extractable geometry to create load panels and mesh geometry to create FE elements  |
| ![Property2dParam](./images/Property2dParam.png) | [Property 2D](gsagh-property-2d-parameter.md) | **Property 2D**                | 2D Property (Area) parameter                                                                 |

### Output parameters

| <img width="20"/> Icon                         | <img width="200"/> Type                     | <img width="200"/> Name        | <img width="1000"/> Description |
| ---------------------------------------------- | ------------------------------------------- | ------------------------------ | ------------------------------  |
| ![Element2dParam](./images/Element2dParam.png) | [Element 2D](gsagh-element-2d-parameter.md) | **Element 2D**                 | GSA 2D Element(s) parameter     |
