# Create 1D Member
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                           |
| ------------------------------------------------  |
| ![Create 1D Member](./images/Create1dMember.png)  |

## Description

Create GSA 1D Member

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_
_This component can preview 3D Sections, right-click the middle of the component to toggle the section preview._

### Input parameters

| <img width="20"/> Icon                       | <img width="200"/> Type        | <img width="200"/> Name        | <img width="1000"/> Description                                                                  |
| -------------------------------------------- | ------------------------------ | ------------------------------ | -----------------------------------------------------------------------------------------------  |
| ![CurveParam](./images/CurveParam.png)       | `Curve`                        | **Curve**                      | Curve (a NURBS curve will automatically be converted in to a Polyline of Arc and Line segments)  |
| ![PropertyParam](./images/PropertyParam.png) | `Property`                     | **Property**                   | Section Property (Beam) or Spring Property parameter                                             |
| ![NumberParam](./images/NumberParam.png)     | `Number`                       | **Mesh Size in model units**   | Target mesh size                                                                                 |

### Output parameters

| <img width="20"/> Icon                       | <img width="200"/> Type                   | <img width="200"/> Name        | <img width="1000"/> Description |
| -------------------------------------------- | ----------------------------------------- | ------------------------------ | ------------------------------  |
| ![Member1dParam](./images/Member1dParam.png) | [Member 1D](gsagh-member-1d-parameter.md) | **Member 1D**                  | GSA 1D Member parameter         |
