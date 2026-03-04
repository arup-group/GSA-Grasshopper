# Grid Line Info
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                       |
| --------------------------------------------  |
| ![Grid Line Info](./images/GridLineInfo.png)  |

## Description

Get the information of a GSA Grid Line

### Input parameters

| <img width="20"/> Icon                       | <img width="200"/> Type                   | <img width="200"/> Name        | <img width="1000"/> Description |
| -------------------------------------------- | ----------------------------------------- | ------------------------------ | ------------------------------  |
| ![GridLineParam](./images/GridLineParam.png) | [Grid Line](gsagh-grid-line-parameter.md) | **Grid Line**                  | Grid Line parameter             |

### Output parameters

| <img width="20"/> Icon                   | <img width="200"/> Type        | <img width="200"/> Name        | <img width="1000"/> Description                                                   |
| ---------------------------------------- | ------------------------------ | ------------------------------ | --------------------------------------------------------------------------------  |
| ![TextParam](./images/TextParam.png)     | `Text`                         | **Label**                      | The name by which the grid line is referred                                       |
| ![PointParam](./images/PointParam.png)   | `Point`                        | **Starting Point**             | The start of a straight line or the centre of a circular arc                      |
| ![NumberParam](./images/NumberParam.png) | `Number`                       | **Length**                     | The length of a straight line or the radius of a circular arc                     |
| ![TextParam](./images/TextParam.png)     | `Text`                         | **Shape**                      | Specifies whether the grid line is a straight line or circular arc                |
| ![NumberParam](./images/NumberParam.png) | `Number`                       | **Orientation**                | The angle of inclination of a straight line or the start angle of a circular arc  |
| ![NumberParam](./images/NumberParam.png) | `Number`                       | **Angle**                      | The end angle of a circular arc (not required for straight grid lines)            |
