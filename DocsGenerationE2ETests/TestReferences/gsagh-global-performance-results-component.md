# Global Performance Results
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                               |
| --------------------------------------------------------------------  |
| ![Global Performance Results](./images/GlobalPerformanceResults.png)  |

## Description

Get Global Performance (Dynamic, Model Stability, and Buckling) Results from a GSA model

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                   | <img width="200"/> Type             | <img width="200"/> Name        | <img width="1000"/> Description |
| ---------------------------------------- | ----------------------------------- | ------------------------------ | ------------------------------  |
| ![ResultParam](./images/ResultParam.png) | [Result](gsagh-result-parameter.md) | **Result**                     | Result                          |

### Output parameters

| <img width="20"/> Icon                   | <img width="200"/> Type                                               | <img width="200"/> Name        | <img width="1000"/> Description                        |
| ---------------------------------------- | --------------------------------------------------------------------- | ------------------------------ | -----------------------------------------------------  |
| ![UnitNumber](./images/UnitParam.png)    | [Unit Number](gsagh-unitnumber-parameter.md) `Magnetic Field`         | **Effective Mass X**           | Effective Mass in GSA Model in X-direction             |
| ![UnitNumber](./images/UnitParam.png)    | [Unit Number](gsagh-unitnumber-parameter.md) `Magnetic Field`         | **Effective Mass Y**           | Effective Mass in GSA Model in Y-direction             |
| ![UnitNumber](./images/UnitParam.png)    | [Unit Number](gsagh-unitnumber-parameter.md) `Magnetic Field`         | **Effective Mass Z**           | Effective Mass in GSA Model in Z-direction             |
| ![UnitNumber](./images/UnitParam.png)    | [Unit Number](gsagh-unitnumber-parameter.md) `Area Moment Of Inertia` | **Effective Inertia X**        | Effective Inertia in GSA Model in X-direction          |
| ![UnitNumber](./images/UnitParam.png)    | [Unit Number](gsagh-unitnumber-parameter.md) `Area Moment Of Inertia` | **Effective Inertia Y**        | Effective Inertia in GSA Model in Y-direction          |
| ![UnitNumber](./images/UnitParam.png)    | [Unit Number](gsagh-unitnumber-parameter.md) `Area Moment Of Inertia` | **Effective Inertia Z**        | Effective Inertia in GSA Model in Z-direction          |
| ![NumberParam](./images/NumberParam.png) | `Number`                                                              | **Mode**                       | Mode number if LC is a dynamic task                    |
| ![UnitNumber](./images/UnitParam.png)    | [Unit Number](gsagh-unitnumber-parameter.md) `Magnetic Field`         | **Modal Mass**                 | Modal Mass of selected LoadCase / mode                 |
| ![UnitNumber](./images/UnitParam.png)    | [Unit Number](gsagh-unitnumber-parameter.md) `Force Per Length`       | **Modal Stiffness**            | Modal Stiffness of selected LoadCase / mode            |
| ![UnitNumber](./images/UnitParam.png)    | [Unit Number](gsagh-unitnumber-parameter.md) `Force Per Length`       | **Modal Geometric Stiffness**  | Modal Geometric Stiffness of selected LoadCase / mode  |
| ![UnitNumber](./images/UnitParam.png)    | [Unit Number](gsagh-unitnumber-parameter.md) `Frequency`              | **Frequency**                  | Frequency of selected LoadCase / mode                  |
| ![NumberParam](./images/NumberParam.png) | `Number`                                                              | **Load Factor**                | Load Factor for selected LoadCase / mode               |
| ![NumberParam](./images/NumberParam.png) | `Number`                                                              | **Eigenvalue**                 | Eigenvalue for selected LoadCase / mode                |
