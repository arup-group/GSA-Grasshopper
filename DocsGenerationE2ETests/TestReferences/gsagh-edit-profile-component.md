# Edit Profile
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                    |
| -----------------------------------------  |
| ![Edit Profile](./images/EditProfile.png)  |

## Description

Transform a Profile by rotation or reflection.

### Input parameters

| <img width="20"/> Icon                     | <img width="200"/> Type        | <img width="200"/> Name        | <img width="1000"/> Description                               |
| ------------------------------------------ | ------------------------------ | ------------------------------ | ------------------------------------------------------------  |
| ![TextParam](./images/TextParam.png)       | `Text`                         | **Profile**                    | Profile to edit                                               |
| ![NumberParam](./images/NumberParam.png)   | `Number`                       | **Orientation Angle**          | Set Profile Orientation Angle in counter-clockwise direction  |
| ![BooleanParam](./images/BooleanParam.png) | `Boolean`                      | **Reflect Horizontal**         | True to reflect the profile about the local y-axis            |
| ![BooleanParam](./images/BooleanParam.png) | `Boolean`                      | **Reflect Vertical**           | True to reflect the profile about the local z-axis            |

### Output parameters

| <img width="20"/> Icon               | <img width="200"/> Type        | <img width="200"/> Name        | <img width="1000"/> Description              |
| ------------------------------------ | ------------------------------ | ------------------------------ | -------------------------------------------  |
| ![TextParam](./images/TextParam.png) | `Text`                         | **Profile**                    | Edited Profile with applied transformations  |
