# Get Result Cases
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                           |
| ------------------------------------------------  |
| ![Get Result Cases](./images/GetResultCases.png)  |

## Description

Get Analysis or Combination Case IDs from a GSA model with Results

### Input parameters

| <img width="20"/> Icon                 | <img width="200"/> Type           | <img width="200"/> Name        | <img width="1000"/> Description |
| -------------------------------------- | --------------------------------- | ------------------------------ | ------------------------------  |
| ![ModelParam](./images/ModelParam.png) | [Model](gsagh-model-parameter.md) | **GSA Model**                  | model containing some results   |

### Output parameters

| <img width="20"/> Icon                     | <img width="200"/> Type        | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                     |
| ------------------------------------------ | ------------------------------ | ------------------------------ | ----------------------------------------------------------------------------------------------------------------------------------  |
| ![TextParam](./images/TextParam.png)       | `Text` _List_                  | **Result Type**                | Result type                                                                                                                         |
| ![IntegerParam](./images/IntegerParam.png) | `Integer` _List_               | **Case**                       | Case ID(s) - to be read in conjunction with Type output                                                                             |
| ![IntegerParam](./images/IntegerParam.png) | `Integer` _Tree_               | **Permutation**                | Permutations (only applicable for combination cases). Data as a Tree where each path `{i}` corrosponds to the Combination Case ID.  |
