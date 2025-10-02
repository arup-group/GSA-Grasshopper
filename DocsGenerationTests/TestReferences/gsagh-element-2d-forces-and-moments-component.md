# Element 2D Forces and Moments
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Element 2D Forces and Moments](./images/Element2dForcesAndMoments.png) |

## Description

2D Projected Force and Moment result values

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![ResultParam](./images/ResultParam.png) |[Result](gsagh-result-parameter.md) _List_ |**Result** |Result |
|![ListParam](./images/ListParam.png) |[List](gsagh-list-parameter.md) |**Element/Member filter List** |Filter the Elements or Members by list. (by default 'all')<br />Element/Member list should take the form:<br /> 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)<br />Refer to help file for definition of lists and full vocabulary. |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Axis** |Standard Axis: Global (0), Local (-1), Natural (-2), Default (-10), XElevation (-11), YElevation (-12), GlobalCylindrical (-13), Vertical (-14) |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force Per Length ` _Tree_ |**Force X** |Element in-plane Force in X-direction<br />+ve in plane force resultant: tensile<br />DataTree organised as { CaseID ; Permutation ; ElementID } <br />fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each <br />branch contains a list of results in the following order: <br />Vertex(1), Vertex(2), ..., Vertex(i), Centre<br />Element results are NOT averaged at nodes |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force Per Length ` _Tree_ |**Force Y** |Element in-plane Force in Y-direction<br />+ve in plane force resultant: tensile<br />DataTree organised as { CaseID ; Permutation ; ElementID } <br />fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each <br />branch contains a list of results in the following order: <br />Vertex(1), Vertex(2), ..., Vertex(i), Centre<br />Element results are NOT averaged at nodes |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force Per Length ` _Tree_ |**Force Z** |Element in-plane Force in XY-direction<br />+ve in plane force resultant: tensile<br />DataTree organised as { CaseID ; Permutation ; ElementID } <br />fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each <br />branch contains a list of results in the following order: <br />Vertex(1), Vertex(2), ..., Vertex(i), Centre<br />Element results are NOT averaged at nodes |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force Per Length ` _Tree_ |**Shear X** |Element through thickness Shear in XZ-plane<br />DataTree organised as { CaseID ; Permutation ; ElementID } <br />fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each <br />branch contains a list of results in the following order: <br />Vertex(1), Vertex(2), ..., Vertex(i), Centre<br />Element results are NOT averaged at nodes |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force Per Length ` _Tree_ |**Shear Y** |Element through thickness Shear in YZ-plane<br />DataTree organised as { CaseID ; Permutation ; ElementID } <br />fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each <br />branch contains a list of results in the following order: <br />Vertex(1), Vertex(2), ..., Vertex(i), Centre<br />Element results are NOT averaged at nodes |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force ` _Tree_ |**Moment X** |Element Moment around X-axis<br />+ve moments correspond to +ve stress on the top (eg. Mx +ve if top Sxx +ve)<br />DataTree organised as { CaseID ; Permutation ; ElementID } <br />fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each <br />branch contains a list of results in the following order: <br />Vertex(1), Vertex(2), ..., Vertex(i), Centre<br />Element results are NOT averaged at nodes |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force ` _Tree_ |**Moment Y** |Element Moment around Y-axis<br />+ve moments correspond to +ve stress on the top (eg. Mx +ve if top Sxx +ve)<br />DataTree organised as { CaseID ; Permutation ; ElementID } <br />fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each <br />branch contains a list of results in the following order: <br />Vertex(1), Vertex(2), ..., Vertex(i), Centre<br />Element results are NOT averaged at nodes |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force ` _Tree_ |**Moment XY** |Element Moment around XY-axis<br />+ve moments correspond to +ve stress on the top (eg. Mx +ve if top Sxx +ve)<br />DataTree organised as { CaseID ; Permutation ; ElementID } <br />fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each <br />branch contains a list of results in the following order: <br />Vertex(1), Vertex(2), ..., Vertex(i), Centre<br />Element results are NOT averaged at nodes |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force ` _Tree_ |**Wood-Armer X** |Element Wood-Armer Moment (Mx + sgn(Mx)·&#124;Mxy&#124;) around X-axis<br />+ve moments correspond to +ve stress on the top (eg. Mx +ve if top Sxx +ve)<br />DataTree organised as { CaseID ; Permutation ; ElementID } <br />fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each <br />branch contains a list of results in the following order: <br />Vertex(1), Vertex(2), ..., Vertex(i), Centre<br />Element results are NOT averaged at nodes |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force ` _Tree_ |**Wood-Armer Y** |Element Wood-Armer Moment (My + sgn(My)·&#124;Mxy&#124;) around Y-axis<br />+ve moments correspond to +ve stress on the top (eg. Mx +ve if top Sxx +ve)<br />DataTree organised as { CaseID ; Permutation ; ElementID } <br />fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each <br />branch contains a list of results in the following order: <br />Vertex(1), Vertex(2), ..., Vertex(i), Centre<br />Element results are NOT averaged at nodes |
