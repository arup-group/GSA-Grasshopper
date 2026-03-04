# Edit Section
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                    |
| -----------------------------------------  |
| ![Edit Section](./images/EditSection.png)  |

## Description

Modify GSA Section

### Input parameters

| <img width="20"/> Icon                                     | <img width="200"/> Type                                 | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                                |
| ---------------------------------------------------------- | ------------------------------------------------------- | ------------------------------ | ---------------------------------------------------------------------------------------------------------------------------------------------  |
| ![SectionParam](./images/SectionParam.png)                 | [Section](gsagh-section-parameter.md)                   | **Section**                    | Section Property (Beam) to get or set information for. Leave blank to create a new Section                                                     |
| ![IntegerParam](./images/IntegerParam.png)                 | `Integer`                                               | **Section Number**             | Set Section Number. If ID is set it will replace any existing 2D Property in the model                                                         |
| ![TextParam](./images/TextParam.png)                       | `Text`                                                  | **Section Profile**            | Profile name following naming convention (eg 'STD I 1000 500 15 25')                                                                           |
| ![MaterialParam](./images/MaterialParam.png)               | [Material](gsagh-material-parameter.md)                 | **Material**                   | Set Material                                                                                                                                   |
| ![GenericParam](./images/GenericParam.png)                 | `Generic`                                               | **Basic Offset**               | Set Basic Offset Centroid = 0 (default), Top = 1, TopLeft = 2, TopRight = 3, Left = 4, Right = 5, Bottom = 6, BottomLeft = 7, BottomRight = 8  |
| ![UnitNumber](./images/UnitParam.png)                      | [Unit Number](gsagh-unitnumber-parameter.md) `Length`   | **Add. Offset Y**              | Set Additional Offset Y                                                                                                                        |
| ![UnitNumber](./images/UnitParam.png)                      | [Unit Number](gsagh-unitnumber-parameter.md) `Length`   | **Add. Offset Z**              | Set Additional Offset Z                                                                                                                        |
| ![SectionModifierParam](./images/SectionModifierParam.png) | [Section Modifier](gsagh-section-modifier-parameter.md) | **Section Modifier**           | Set Section Modifier                                                                                                                           |
| ![IntegerParam](./images/IntegerParam.png)                 | `Integer`                                               | **Section Pool**               | Set Section pool                                                                                                                               |
| ![TextParam](./images/TextParam.png)                       | `Text`                                                  | **Section Name**               | Set Section name                                                                                                                               |
| ![ColourParam](./images/ColourParam.png)                   | `Colour`                                                | **Section Colour**             | Set Section colour                                                                                                                             |

### Output parameters

| <img width="20"/> Icon                                     | <img width="200"/> Type                                 | <img width="200"/> Name        | <img width="1000"/> Description                                           |
| ---------------------------------------------------------- | ------------------------------------------------------- | ------------------------------ | ------------------------------------------------------------------------  |
| ![SectionParam](./images/SectionParam.png)                 | [Section](gsagh-section-parameter.md)                   | **Section**                    | GSA Section Property (Beam) with applied changes.                         |
| ![IntegerParam](./images/IntegerParam.png)                 | `Integer`                                               | **Section Number**             | Original Section number (ID) if the Section ever belonged to a GSA Model  |
| ![TextParam](./images/TextParam.png)                       | `Text`                                                  | **Section Profile**            | Profile description                                                       |
| ![MaterialParam](./images/MaterialParam.png)               | [Material](gsagh-material-parameter.md)                 | **Material**                   | Material                                                                  |
| ![GenericParam](./images/GenericParam.png)                 | `Generic`                                               | **Basic Offset**               | Basic Offset                                                              |
| ![UnitNumber](./images/UnitParam.png)                      | [Unit Number](gsagh-unitnumber-parameter.md) `Length`   | **Add. Offset Y**              | Additional Offset Y                                                       |
| ![UnitNumber](./images/UnitParam.png)                      | [Unit Number](gsagh-unitnumber-parameter.md) `Length`   | **Add. Offset Z**              | Additional Offset Z                                                       |
| ![SectionModifierParam](./images/SectionModifierParam.png) | [Section Modifier](gsagh-section-modifier-parameter.md) | **Section Modifier**           | Section Modifier                                                          |
| ![IntegerParam](./images/IntegerParam.png)                 | `Integer`                                               | **Section Pool**               | Section pool                                                              |
| ![TextParam](./images/TextParam.png)                       | `Text`                                                  | **Section Name**               | Section name                                                              |
| ![ColourParam](./images/ColourParam.png)                   | `Colour`                                                | **Section Colour**             | Section colour                                                            |
