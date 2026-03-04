# Annotate
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon             |
| ----------------------------------  |
| ![Annotate](./images/Annotate.png)  |

## Description

Show the ID of a Node, Element, or Member parameters, or get Result or Diagram values

### Input parameters

| <img width="20"/> Icon                     | <img width="200"/> Type        | <img width="200"/> Name                     | <img width="1000"/> Description                                                          |
| ------------------------------------------ | ------------------------------ | ------------------------------------------- | ---------------------------------------------------------------------------------------  |
| ![GenericParam](./images/GenericParam.png) | `Generic` _Tree_               | **Node/Element/Member/Load/Result/Diagram** | Node, Element, Member, Point/Line/Mesh result, Result or Load diagram or to get ID for.  |
| ![ColourParam](./images/ColourParam.png)   | `Colour`                       | **Colour**                                  | [Optional] Colour to override default colour                                             |

### Output parameters

| <img width="20"/> Icon                           | <img width="200"/> Type        | <img width="200"/> Name        | <img width="1000"/> Description                   |
| ------------------------------------------------ | ------------------------------ | ------------------------------ | ------------------------------------------------  |
| ![AnnotationParam](./images/AnnotationParam.png) | `Annotation` _Tree_            | **Annotations**                | Annotations for the GSA object                    |
| ![PointParam](./images/PointParam.png)           | `Point` _Tree_                 | **Position**                   | The (centre/mid) location(s) of the object(s)     |
| ![TextParam](./images/TextParam.png)             | `Text` _Tree_                  | **Text**                       | The objects ID(s) or the result/diagram value(s)  |
