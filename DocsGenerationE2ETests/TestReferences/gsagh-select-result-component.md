# Select Result
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                      |
| -------------------------------------------  |
| ![Select Result](./images/SelectResult.png)  |

## Description

Select AnalysisCase or Combination Result from an analysed GSA model

_Note: This is a dropdown component and input/output may vary depending on the selected dropdown_

### Input parameters

| <img width="20"/> Icon                     | <img width="200"/> Type           | <img width="200"/> Name        | <img width="1000"/> Description                                                |
| ------------------------------------------ | --------------------------------- | ------------------------------ | -----------------------------------------------------------------------------  |
| ![ModelParam](./images/ModelParam.png)     | [Model](gsagh-model-parameter.md) | **GSA Model**                  | model containing some results                                                  |
| ![TextParam](./images/TextParam.png)       | `Text`                            | **Result Type**                | Result type. <br />Accepted inputs are: <br />'AnalysisCase' or 'Combination'  |
| ![IntegerParam](./images/IntegerParam.png) | `Integer`                         | **Case**                       | Case ID(s)                                                                     |
| ![IntegerParam](./images/IntegerParam.png) | `Integer` _List_                  | **Permutation**                | Permutations (only applicable for combination cases).                          |

### Output parameters

| <img width="20"/> Icon                   | <img width="200"/> Type             | <img width="200"/> Name        | <img width="1000"/> Description |
| ---------------------------------------- | ----------------------------------- | ------------------------------ | ------------------------------  |
| ![ResultParam](./images/ResultParam.png) | [Result](gsagh-result-parameter.md) | **Result**                     | GSA Result                      |
