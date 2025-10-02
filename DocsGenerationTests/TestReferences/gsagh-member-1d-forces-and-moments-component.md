# Member 1D Forces and Moments
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Member 1D Forces and Moments](./images/Member1dForcesAndMoments.png) |

## Description

1D Member Force and Moment result values

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![ResultParam](./images/ResultParam.png) |[Result](gsagh-result-parameter.md) _List_ |**Result** |Result |
|![ListParam](./images/ListParam.png) |[List](gsagh-list-parameter.md) |**Member filter list** |Filter import by list. (by default 'all')<br />Member list should take the form:<br /> 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (Z4 XY55)<br />Refer to help file for definition of lists and full vocabulary. |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Intermediate Points** |Number of intermediate equidistant points (default 3) |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force ` _Tree_ |**Force X** |* Member Axial Force in Local Member X-direction<br />+ve axial forces are tensile |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force ` _Tree_ |**Force Y** |* Member Shear Force in Local Member Y-direction<br />+ve axial forces are tensile |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force ` _Tree_ |**Force Z** |* Member Shear Force in Local Member Z-direction<br />+ve axial forces are tensile |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force ` _Tree_ |**Force &#124;YZ&#124;** |* Total &#124;YZ&#124; Member Shear Force |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Moment ` _Tree_ |**Moment XX** |* Member Torsional Moment around Local Member X-axis<br />Moments follow the right hand grip rule |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Moment ` _Tree_ |**Moment YY** |* Member Bending Moment around Local Member Y-axis<br />Moments follow the right hand grip rule |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Moment ` _Tree_ |**Moment ZZ** |* Member Bending Moment around Local Member Z-axis<br />Moments follow the right hand grip rule |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Moment ` _Tree_ |**Moment &#124;YYZZ&#124;** |* Total &#124;YYZZ&#124; Member Bending Moment |



_* DataTree organised as { `CaseID` ; `Permutation` ; `ElementID` } fx. `{1;2;3}` is Case 1, Permutation 2, Element 3, where each branch contains a list of results per element position._
