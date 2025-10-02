# Assembly Forces and Moments
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Assembly Forces and Moments](./images/AssemblyForcesAndMoments.png) |

## Description

Assembly Force and Moment result values

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![ResultParam](./images/ResultParam.png) |[Result](gsagh-result-parameter.md) _List_ |**Result** |Result |
|![ListParam](./images/ListParam.png) |[List](gsagh-list-parameter.md) |**Assembly filter list** |Filter the Assemblies by list. (by default 'all') |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force ` _Tree_ |**Force X** |Assembly Axial Force in Local Element X-direction<br />+ve axial forces are tensile<br />DataTree organised as { `CaseID` ; `Permutation` ; `AssemblyID` } <br />fx. `{1;2;3}` is Case 1, Permutation 2, Assembly 3, where each <br />branch contains a list of results per assembly position/storey. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force ` _Tree_ |**Force Y** |Assembly Shear Force in Local Element Y-direction<br />+ve axial forces are tensile<br />DataTree organised as { `CaseID` ; `Permutation` ; `AssemblyID` } <br />fx. `{1;2;3}` is Case 1, Permutation 2, Assembly 3, where each <br />branch contains a list of results per assembly position/storey. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force ` _Tree_ |**Force Z** |Assembly Shear Force in Local Element Z-direction<br />+ve axial forces are tensile<br />DataTree organised as { `CaseID` ; `Permutation` ; `AssemblyID` } <br />fx. `{1;2;3}` is Case 1, Permutation 2, Assembly 3, where each <br />branch contains a list of results per assembly position/storey. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force ` _Tree_ |**Force &#124;YZ&#124;** |Total &#124;YZ&#124; Assembly Shear Force<br />DataTree organised as { `CaseID` ; `Permutation` ; `AssemblyID` } <br />fx. `{1;2;3}` is Case 1, Permutation 2, Assembly 3, where each <br />branch contains a list of results per assembly position/storey. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Moment ` _Tree_ |**Moment XX** |Assembly Torsional Moment around Local Element X-axis<br />Moments follow the right hand grip rule<br />DataTree organised as { `CaseID` ; `Permutation` ; `AssemblyID` } <br />fx. `{1;2;3}` is Case 1, Permutation 2, Assembly 3, where each <br />branch contains a list of results per assembly position/storey. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Moment ` _Tree_ |**Moment YY** |Assembly Bending Moment around Local Element Y-axis<br />Moments follow the right hand grip rule<br />DataTree organised as { `CaseID` ; `Permutation` ; `AssemblyID` } <br />fx. `{1;2;3}` is Case 1, Permutation 2, Assembly 3, where each <br />branch contains a list of results per assembly position/storey. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Moment ` _Tree_ |**Moment ZZ** |Assembly Bending Moment around Local Element Z-axis<br />Moments follow the right hand grip rule<br />DataTree organised as { `CaseID` ; `Permutation` ; `AssemblyID` } <br />fx. `{1;2;3}` is Case 1, Permutation 2, Assembly 3, where each <br />branch contains a list of results per assembly position/storey. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Moment ` _Tree_ |**Moment &#124;YYZZ&#124;** |Total &#124;YYZZ&#124; Assembly Bending Moment<br />DataTree organised as { `CaseID` ; `Permutation` ; `AssemblyID` } <br />fx. `{1;2;3}` is Case 1, Permutation 2, Assembly 3, where each <br />branch contains a list of results per assembly position/storey. |
