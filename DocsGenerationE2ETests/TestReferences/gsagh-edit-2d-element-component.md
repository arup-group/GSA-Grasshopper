# Edit 2D Element
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                         |
| ----------------------------------------------  |
| ![Edit 2D Element](./images/Edit2dElement.png)  |

## Description

Modify GSA 2D Element

_Note: This component can preview 3D Sections, right-click the middle of the component to toggle the section preview._

### Input parameters

| <img width="20"/> Icon                           | <img width="200"/> Type                              | <img width="200"/> Name        | <img width="1000"/> Description                                                                          |
| ------------------------------------------------ | ---------------------------------------------------- | ------------------------------ | -------------------------------------------------------------------------------------------------------  |
| ![Element2dParam](./images/Element2dParam.png)   | [Element 2D](gsagh-element-2d-parameter.md)          | **Element 2D**                 | 2D Element(s) to get or set information for.Element 2D                                                   |
| ![IntegerParam](./images/IntegerParam.png)       | `Integer` _List_                                     | **Element2d Number**           | Set Element Number. If ID is set it will replace any existing 2D Element in the model                    |
| ![Property2dParam](./images/Property2dParam.png) | [Property 2D](gsagh-property-2d-parameter.md) _List_ | **2D Property**                | Change 2D Property. Input either a 2D Property or an Integer to use a Property already defined in model  |
| ![IntegerParam](./images/IntegerParam.png)       | `Integer` _List_                                     | **Element2d Group**            | Set Element Group                                                                                        |
| ![OffsetParam](./images/OffsetParam.png)         | [Offset](gsagh-offset-parameter.md) _List_           | **Offset**                     | Set Element Offset                                                                                       |
| ![NumberParam](./images/NumberParam.png)         | `Number` _List_                                      | **Orientation Angle**          | Set Element Orientation Angle                                                                            |
| ![TextParam](./images/TextParam.png)             | `Text` _List_                                        | **Element2d Name**             | Set Name of Element                                                                                      |
| ![ColourParam](./images/ColourParam.png)         | `Colour` _List_                                      | **Element2d Colour**           | Set Element Colour                                                                                       |
| ![BooleanParam](./images/BooleanParam.png)       | `Boolean` _List_                                     | **Dummy Element**              | Set Element to Dummy                                                                                     |

### Output parameters

| <img width="20"/> Icon                           | <img width="200"/> Type                              | <img width="200"/> Name        | <img width="1000"/> Description                                                                                           |
| ------------------------------------------------ | ---------------------------------------------------- | ------------------------------ | ------------------------------------------------------------------------------------------------------------------------  |
| ![Element2dParam](./images/Element2dParam.png)   | [Element 2D](gsagh-element-2d-parameter.md)          | **Element 2D**                 | GSA 2D Element(s) with applied changes.                                                                                   |
| ![IntegerParam](./images/IntegerParam.png)       | `Integer` _List_                                     | **Number**                     | Element Number                                                                                                            |
| ![IGeometricParam](./images/IGeometricParam.png) | `Geometry`                                           | **Geometry**                   | analysis mesh for FE element and polyline for load panel                                                                  |
| ![Property2dParam](./images/Property2dParam.png) | [Property 2D](gsagh-property-2d-parameter.md) _List_ | **2D Property**                | 2D Property. Input either a GSA 2D Property or an Integer to use a Property already defined in model                      |
| ![IntegerParam](./images/IntegerParam.png)       | `Integer` _List_                                     | **Group**                      | Element Group                                                                                                             |
| ![TextParam](./images/TextParam.png)             | `Text` _List_                                        | **Element Type**               | Element 2D Type.<br />Type can not be set; it is either Tri3 or Quad4<br />depending on Rhino/Grasshopper mesh face type  |
| ![OffsetParam](./images/OffsetParam.png)         | [Offset](gsagh-offset-parameter.md) _List_           | **Offset**                     | Element Offset                                                                                                            |
| ![NumberParam](./images/NumberParam.png)         | `Number` _List_                                      | **Orientation Angle**          | Element Orientation Angle in radians                                                                                      |
| ![TextParam](./images/TextParam.png)             | `Text` _List_                                        | **Name**                       | Set Element Name                                                                                                          |
| ![ColourParam](./images/ColourParam.png)         | `Colour` _List_                                      | **Colour**                     | Element Colour                                                                                                            |
| ![BooleanParam](./images/BooleanParam.png)       | `Boolean` _List_                                     | **Dummy Element**              | if Element is Dummy                                                                                                       |
| ![IntegerParam](./images/IntegerParam.png)       | `Integer` _List_                                     | **Parent Members**             | Parent Member IDs in Model that Element was created from                                                                  |
| ![IntegerParam](./images/IntegerParam.png)       | `Integer` _List_                                     | **Topology**                   | the Element's original topology list referencing node IDs in Model that Element was created from                          |
