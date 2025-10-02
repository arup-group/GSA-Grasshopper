# Assembly Result Diagrams
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Assembly Result Diagrams](./images/AssemblyResultDiagrams.png) |

## Description

Displays GSA Assembly Result Diagram

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![ResultParam](./images/ResultParam.png) |[Result](gsagh-result-parameter.md) |**Result** |Result |
|![ListParam](./images/ListParam.png) |[List](gsagh-list-parameter.md) |**Assembly filter list** |Filter the Assemblies by list. (by default 'all') |
|![BooleanParam](./images/BooleanParam.png) |`Boolean` |**Annotation** |Show Annotation |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Significant Digits** |Round values to significant digits |
|![ColourParam](./images/ColourParam.png) |`Colour` |**Colour** |[Optional] Colour to override default colour |
|![NumberParam](./images/NumberParam.png) |`Number` |**Scale** |Scale the result display size |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![DiagramParam](./images/DiagramParam.png) |`Diagram` _List_ |**Diagram lines** |Lines of the GSA Result Diagram |
|![AnnotationParam](./images/AnnotationParam.png) |`Annotation` _List_ |**Annotations** |Annotations for the diagram |
