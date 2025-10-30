# Assembly Drift Indices
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Assembly Drift Indices](./images/AssemblyDriftIndices.png) |

## Description

Assembly Drift Index result values

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![ResultParam](./images/ResultParam.png) |[Result](gsagh-result-parameter.md) _List_ |**Result** |Result |
|![ListParam](./images/ListParam.png) |[List](gsagh-list-parameter.md) |**Assembly filter list** |Filter the Assemblies by list. (by default 'all') |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![GenericParam](./images/GenericParam.png) |`Generic` _Tree_ |**Drift Index X** |Drift Index in Local Assembly X-direction<br />DataTree organised as { `CaseID` ; `Permutation` ; `AssemblyID` } <br />fx. `{1;2;3}` is Case 1, Permutation 2, Assembly 3, where each <br />branch contains a list of results per assembly position/storey. |
|![GenericParam](./images/GenericParam.png) |`Generic` _Tree_ |**Drift Index Y** |Drift Index in Local Assembly Y-direction<br />DataTree organised as { `CaseID` ; `Permutation` ; `AssemblyID` } <br />fx. `{1;2;3}` is Case 1, Permutation 2, Assembly 3, where each <br />branch contains a list of results per assembly position/storey. |
|![GenericParam](./images/GenericParam.png) |`Generic` _Tree_ |**Drift Index XY** |Drift Index in Local Assembly XY-plane<br />DataTree organised as { `CaseID` ; `Permutation` ; `AssemblyID` } <br />fx. `{1;2;3}` is Case 1, Permutation 2, Assembly 3, where each <br />branch contains a list of results per assembly position/storey. |


