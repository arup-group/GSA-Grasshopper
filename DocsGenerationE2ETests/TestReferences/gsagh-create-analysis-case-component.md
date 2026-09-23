# Create Analysis Case
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon                                   |
| --------------------------------------------------------  |
| ![Create Analysis Case](./images/CreateAnalysisCase.png)  |

## Description

Create a GSA Analysis Case

### Input parameters

| <img width="20"/> Icon               | <img width="200"/> Type        | <img width="200"/> Name        | <img width="1000"/> Description                                                                                                                                                                     |
| ------------------------------------ | ------------------------------ | ------------------------------ | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  |
| ![TextParam](./images/TextParam.png) | `Text`                         | **Name**                       | Case Name                                                                                                                                                                                           |
| ![TextParam](./images/TextParam.png) | `Text`                         | **Description**                | The description should take the form: 1.4L1 + 0.8L3.<br />It may also take the form: 1.4A4 or 1.6C2.<br />The referenced loads (L#), analysis (A#), and combination (C#) cases must exist in model  |

### Output parameters

| <img width="20"/> Icon                               | <img width="200"/> Type                           | <img width="200"/> Name        | <img width="1000"/> Description |
| ---------------------------------------------------- | ------------------------------------------------- | ------------------------------ | ------------------------------  |
| ![AnalysisCaseParam](./images/AnalysisCaseParam.png) | [Analysis Case](gsagh-analysis-case-parameter.md) | **Analysis Case**              | GSA Analysis Case parameter     |
