# Annotate Detailed
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                              |
| ---------------------------------------------------  |
| ![Annotate Detailed](./images/AnnotateDetailed.png)  |

## Description

Show the detailed information of Element or Member parameters

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                     | <img width="200"/> Type        | <img width="200"/> Name        | <img width="1000"/> Description            |
| ------------------------------------------ | ------------------------------ | ------------------------------ | -----------------------------------------  |
| ![GenericParam](./images/GenericParam.png) | `Generic` _Tree_               | **Element/Member**             | Element or Member to annotate details of.  |
| ![NumberParam](./images/NumberParam.png)   | `Number`                       | **Size**                       | Size of annotation                         |
| ![ColourParam](./images/ColourParam.png)   | `Colour`                       | **Colour**                     | Optional colour of annotation              |

### Output parameters

| <img width="20"/> Icon                           | <img width="200"/> Type        | <img width="200"/> Name        | <img width="1000"/> Description                   |
| ------------------------------------------------ | ------------------------------ | ------------------------------ | ------------------------------------------------  |
| ![AnnotationParam](./images/AnnotationParam.png) | `Annotation` _Tree_            | **Annotations**                | Annotations for the GSA object                    |
| ![PointParam](./images/PointParam.png)           | `Point` _Tree_                 | **Position**                   | The (centre/mid) location(s) of the object(s)     |
| ![TextParam](./images/TextParam.png)             | `Text` _Tree_                  | **Text**                       | The objects ID(s) or the result/diagram value(s)  |
