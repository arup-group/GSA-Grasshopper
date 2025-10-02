# Beam Stresses
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![Beam Stresses](./images/BeamStresses.png) |

## Description

Element1D Stress result values

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![ResultParam](./images/ResultParam.png) |[Result](gsagh-result-parameter.md) _List_ |**Result** |Result |
|![ListParam](./images/ListParam.png) |[List](gsagh-list-parameter.md) |**Element/Member filter List** |Filter the Elements or Members by list. (by default 'all')<br />Element/Member list should take the form:<br /> 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)<br />Refer to help file for definition of lists and full vocabulary. |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Intermediate Points** |Number of intermediate equidistant points (default 3) |

### Output parameters

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Pressure ` _Tree_ |**Axial** |Axial stress:
A = Fx/Area
+ve stresses: tensile |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Pressure ` _Tree_ |**Shear Y** |Shear stresses:
Sy = Fy/Ay
where Ay and Az are the shear areas calculated in accordance with selected code. Note that torsional stresses are ignored. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Pressure ` _Tree_ |**Shear Z** |Shear stresses:
Sz = Fz/Az
where Ay and Az are the shear areas calculated in accordance with selected code. Note that torsional stresses are ignored. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Pressure ` _Tree_ |**Bending Y +ve z** |Bending stresses:
For sections that have Iyz=0:
By = Myy/Iyy x Dz
- where Dz & Dy are the distances from the centroid to the edge of the
section in the +ve z and y directions respectively.
For sections that have non-zero Iyz: refer to the GSA manual. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Pressure ` _Tree_ |**Bending Y -ve z** |Bending stresses:
For sections that have Iyz=0:
By = Myy/Iyy x Dz
- where Dz & Dy are the distances from the centroid to the edge of the
section in the +ve z and y directions respectively.
For sections that have non-zero Iyz: refer to the GSA manual. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Pressure ` _Tree_ |**Bending Z +ve y** |Bending stresses:
For sections that have Iyz=0:
Bz = -Mzz/Izz x Dy
- where Dz & Dy are the distances from the centroid to the edge of the
section in the +ve z and y directions respectively.
For sections that have non-zero Iyz: refer to the GSA manual. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Pressure ` _Tree_ |**Bending Z -ve y** |Bending stresses:
For sections that have Iyz=0:
Bz = -Mzz/Izz x Dy
- where Dz & Dy are the distances from the centroid to the edge of the
section in the +ve z and y directions respectively.
For sections that have non-zero Iyz: refer to the GSA manual. |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Pressure ` _Tree_ |**Combined C1** |C1 is the maximum extreme fibre longitudinal stress due to axial forces and transverse bending
C1 and C2 stresses are not calculated for cases with enveloping operators in them.
+ve stresses: tensile |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Pressure ` _Tree_ |**Combined C2** |C2 is the minimum extreme fibre longitudinal stress due to axial forces and transverse bending
C1 and C2 stresses are not calculated for cases with enveloping operators in them.
+ve stresses: tensile |
