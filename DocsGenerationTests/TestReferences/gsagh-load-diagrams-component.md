# Load Diagrams
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Load Diagrams](./images/LoadDiagrams.png) |

## Description

Displays GSA Load Diagram

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![ModelParam](./images/ModelParam.png) |[Model](gsagh-model-parameter.md) |**GSA model** |model containing some Analysis Cases and Tasks |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Case filter list** |Filter import by list.<br />The case list should take the form:<br /> 1 L1 M1 A1 C1 C2p1 A3 to A5 T1. |
|![ListParam](./images/ListParam.png) |[List](gsagh-list-parameter.md) |**Element/Member filter List** |Filter the Elements or Members by list. (by default 'all')<br />Element/Member list should take the form:<br /> 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)<br />Refer to help file for definition of lists and full vocabulary. |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Annotation** |Show Annotation |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Significant Digits** |Round values to significant digits |
|![ColourParam](./images/ColourParam.png) |`Colour` |**Colour** |[Optional] Colour to override default colour |
|![NumberParam](./images/NumberParam.png) |`Number` |**Scale** |Scale the result display size |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![DiagramParam](./images/DiagramParam.png) |`Diagram` _List_ |**Diagram lines** |Lines and arrowheads of the GSA Load Diagram |
|![AnnotationParam](./images/AnnotationParam.png) |`Annotation` _List_ |**Annotations** |Annotations for the diagram |
