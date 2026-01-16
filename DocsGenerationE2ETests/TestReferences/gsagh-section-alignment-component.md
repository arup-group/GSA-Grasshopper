# Section Alignment
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                              |
| ---------------------------------------------------  |
| ![Section Alignment](./images/SectionAlignment.png)  |

## Description

Automatically create Offset based on desired Alignment and Section profile

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                     | <img width="200"/> Type             | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                                                                                                                           |
| ------------------------------------------ | ----------------------------------- | ------------------------------ | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  |
| ![GenericParam](./images/GenericParam.png) | `Generic`                           | **Element/Member 1D/2D**       | Element1D, Element2D, Member1D or Member2D to align. Existing Offsets will be overwritten.                                                                                                                                                |
| ![TextParam](./images/TextParam.png)       | `Text`                              | **Alignment**                  | Section alignment. This input will overwrite dropdown selection.<br />Accepted inputs are:<br />Centroid<br />Top-Left<br />Top-Centre<br />Top-Right<br />Mid-Left<br />Mid-Right<br />Bottom-Left<br />Bottom-Centre<br />Bottom-Right  |
| ![OffsetParam](./images/OffsetParam.png)   | [Offset](gsagh-offset-parameter.md) | **Offset**                     | Additional Offset (y and z values will be added to alignment setting)                                                                                                                                                                     |

### Output parameters

| <img width="20"/> Icon                     | <img width="200"/> Type                    | <img width="200"/> Name        | <img width="1000"/> Description                                                               |
| ------------------------------------------ | ------------------------------------------ | ------------------------------ | --------------------------------------------------------------------------------------------  |
| ![GenericParam](./images/GenericParam.png) | `Generic`                                  | **Element/Member 1D/2D**       | Element1D, Element2D, Member1D or Member2D with new Offset corrosponding to alignment input.  |
| ![OffsetParam](./images/OffsetParam.png)   | [Offset](gsagh-offset-parameter.md) _List_ | **Offset**                     | Applied Offset                                                                                |
