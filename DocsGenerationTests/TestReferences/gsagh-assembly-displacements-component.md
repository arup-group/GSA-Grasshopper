# Assembly Displacements
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Assembly Displacements](./images/AssemblyDisplacements.png) |

## Description

Assembly Translation and Rotation result values

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![ResultParam](./images/ResultParam.png) |[Result](gsagh-result-parameter.md) _List_ |**Result** |Result |
|![ListParam](./images/ListParam.png) |[List](gsagh-list-parameter.md) |**Assembly filter list** |Filter the Assemblies by list. (by default 'all') |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` _Tree_ |**Translations X** |Translation in Local Assembly X-direction<br />DataTree organised as { `CaseID` ; `Permutation` ; `AssemblyID` } <br />fx. `{1;2;3}` is Case 1, Permutation 2, Assembly 3, where each <br />branch contains a list of results per assembly position/storey. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` _Tree_ |**Translations Y** |Translation in Local Assembly Y-direction<br />DataTree organised as { `CaseID` ; `Permutation` ; `AssemblyID` } <br />fx. `{1;2;3}` is Case 1, Permutation 2, Assembly 3, where each <br />branch contains a list of results per assembly position/storey. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` _Tree_ |**Translations Z** |Translation in Local Assembly Z-direction<br />DataTree organised as { `CaseID` ; `Permutation` ; `AssemblyID` } <br />fx. `{1;2;3}` is Case 1, Permutation 2, Assembly 3, where each <br />branch contains a list of results per assembly position/storey. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` _Tree_ |**Translations &#124;XYZ&#124;** |Combined &#124;XYZ&#124; Translation.<br />DataTree organised as { `CaseID` ; `Permutation` ; `AssemblyID` } <br />fx. `{1;2;3}` is Case 1, Permutation 2, Assembly 3, where each <br />branch contains a list of results per assembly position/storey. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Angle ` _Tree_ |**Rotation XX** |Rotation around Local Assembly X-axis<br />DataTree organised as { `CaseID` ; `Permutation` ; `AssemblyID` } <br />fx. `{1;2;3}` is Case 1, Permutation 2, Assembly 3, where each <br />branch contains a list of results per assembly position/storey. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Angle ` _Tree_ |**Rotation YY** |Rotation around Local Assembly Y-axis<br />DataTree organised as { `CaseID` ; `Permutation` ; `AssemblyID` } <br />fx. `{1;2;3}` is Case 1, Permutation 2, Assembly 3, where each <br />branch contains a list of results per assembly position/storey. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Angle ` _Tree_ |**Rotation ZZ** |Rotation around Local Assembly Z-axis<br />DataTree organised as { `CaseID` ; `Permutation` ; `AssemblyID` } <br />fx. `{1;2;3}` is Case 1, Permutation 2, Assembly 3, where each <br />branch contains a list of results per assembly position/storey. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Angle ` _Tree_ |**Rotation &#124;XYZ&#124;** |Combined &#124;XXYYZZ&#124; Rotation.<br />DataTree organised as { `CaseID` ; `Permutation` ; `AssemblyID` } <br />fx. `{1;2;3}` is Case 1, Permutation 2, Assembly 3, where each <br />branch contains a list of results per assembly position/storey. |


