# Reaction Force Diagrams
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                         |
| --------------------------------------------------------------  |
| ![Reaction Force Diagrams](./images/ReactionForceDiagrams.png)  |

## Description

Diplays GSA Node Reaction Force Results as Vector Diagrams

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                     | <img width="200"/> Type             | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                                                                                    |
| ------------------------------------------ | ----------------------------------- | ------------------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  |
| ![ResultParam](./images/ResultParam.png)   | [Result](gsagh-result-parameter.md) | **Result**                     | Result                                                                                                                                                                                             |
| ![ListParam](./images/ListParam.png)       | [List](gsagh-list-parameter.md)     | **Node filter list**           | Filter the Nodes by list. (by default 'all')<br />Node list should take the form:<br /> 1 11 to 72 step 2 not (XY3 31 to 45)<br />Refer to help file for definition of lists and full vocabulary.  |
| ![BooleanParam](./images/BooleanParam.png) | `Boolean`                           | **Annotation**                 | Show Annotation                                                                                                                                                                                    |
| ![IntegerParam](./images/IntegerParam.png) | `Integer`                           | **Significant Digits**         | Round values to significant digits                                                                                                                                                                 |
| ![ColourParam](./images/ColourParam.png)   | `Colour`                            | **Colour**                     | [Optional] Colour to override default colour                                                                                                                                                       |
| ![NumberParam](./images/NumberParam.png)   | `Number`                            | **Scalar**                     | Scale the result vectors to a specific size. If left empty, automatic scaling based on model size and maximum result by load cases will be computed.                                               |

### Output parameters

| <img width="20"/> Icon                           | <img width="200"/> Type        | <img width="200"/> Name        | <img width="1000"/> Description    |
| ------------------------------------------------ | ------------------------------ | ------------------------------ | ---------------------------------  |
| ![DiagramParam](./images/DiagramParam.png)       | `Diagram` _List_               | **Diagram lines**              | Vectors of the GSA Result Diagram  |
| ![AnnotationParam](./images/AnnotationParam.png) | `Annotation` _List_            | **Annotations**                | Annotations for the diagram        |
