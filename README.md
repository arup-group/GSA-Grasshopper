# GSA-Grasshopper
![GitHub branch checks state](https://img.shields.io/badge/dynamic/json?color=blue&label=Downloads&query=download_count&url=https%3A%2F%2Fyak.rhino3d.com%2Fpackages%2Fgsa) [![Install plugin](https://img.shields.io/badge/Install-Food4Rhino-lightgrey)](https://www.food4rhino.com/en/app/gsa)

A Grasshopper plugin for structural analysis using Oasys GSA.

| Latest | CI Pipeline | Unit Tests | Deployment | Dependencies |
| ------ | ----------- | ---------- | ---------- | ------------ |
| [![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/arup-group/GSA-Grasshopper?include_prereleases&logo=github)](https://github.com/arup-group/GSA-Grasshopper/releases) <br /> ![Yak](https://img.shields.io/badge/dynamic/json?color=blue&label=Yak&prefix=v&query=version&url=https%3A%2F%2Fyak.rhino3d.com%2Fpackages%2Fgsa&logo=rhinoceros) <br /> ![Nuget](https://img.shields.io/nuget/vpre/gsagh?logo=nuget) | [![Build Status](https://dev.azure.com/oasys-software/OASYS%20libraries/_apis/build/status/arup-group.GSA-Grasshopper?branchName=main)](https://dev.azure.com/oasys-software/OASYS%20libraries/_build/latest?definitionId=139&branchName=main) <br /> ![GitHub branch checks state](https://img.shields.io/github/checks-status/arup-group/gsa-grasshopper/main?logo=github) | [![codecov](https://codecov.io/gh/arup-group/GSA-Grasshopper/branch/main/graph/badge.svg?token=MB9FPYAICX)](https://codecov.io/gh/arup-group/GSA-Grasshopper) <br /> ![Azure DevOps tests](https://img.shields.io/azure-devops/tests/oasys-software/OASYS%2520libraries/139/main?compact_message&logo=azurepipelines) | ![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/arup-group/gsa-grasshopper/github-release-yak.yml?label=yak&logo=rhinoceros) <br /> ![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/arup-group/gsa-grasshopper/github-release-nuget.yml?label=nuget&logo=nuget) | ![Libraries.io dependency status for GitHub repo](https://img.shields.io/librariesio/github/arup-group/gsa-grasshopper?logo=nuget) <br /> ![Sourcegraph for Repo Reference Count](https://img.shields.io/sourcegraph/rrc/github.com/arup-group/GSA-Grasshopper/?logo=nuget) |

## Documentation
[![Docs](https://img.shields.io/badge/Docs-GsaGH%20Introduction-blue?logo=readme&logoColor=white)](https://docs.oasys-software.com/structural/gsa/explanations/gsagh-introduction.html?source=GsaGhGithub)

Head over to our GSA documentation site for:
- [Introduction](https://docs.oasys-software.com/structural/gsa/explanations/gsagh-introduction.html?source=GsaGhGithub)
- [Installing the plugin in Grasshopper](https://docs.oasys-software.com/structural/gsa/tutorials/gsagh-installing-grasshopper-plugin.html?source=GsaGhGithub)
- [Parameters overview](https://docs.oasys-software.com/structural/gsa/explanations/gsagh-parameters.html?source=GsaGhGithub)
- [Components overview](https://docs.oasys-software.com/structural/gsa/explanations/gsagh-components.html?source=GsaGhGithub)

Tutorials and more example files to come shortly!

## Example Files
[![Docs](https://img.shields.io/badge/Grasshopper-Example%20Files-green?logo=rhinoceros)](/ExampleFiles)

This repository contains a number of example files that are also used for testing on new releases, please check out the [ExampleFiles folder](/ExampleFiles) for a growing list of Grasshopper files.

## Contributing
![GitHub pull requests](https://img.shields.io/github/issues-pr-raw/arup-group/gsa-grasshopper) ![GitHub closed pull requests](https://img.shields.io/github/issues-pr-closed-raw/arup-group/gsa-grasshopper?logoColor=brightgreen)
![GitHub commit activity](https://img.shields.io/github/commit-activity/m/arup-group/gsa-grasshopper)
![GitHub top language](https://img.shields.io/github/languages/top/arup-group/gsa-grasshopper?logo=dotnet)
![GitHub contributors](https://img.shields.io/github/contributors/arup-group/gsa-grasshopper)

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

This plugin is free to use, however in order to perform any meaningful tasks you must have GSA installed on your machine. For licensing of GSA refer to Oasys Software licensing terms: https://www.oasys-software.com/support/licensing-of-oasys-software/

## About
[![GSA](https://img.shields.io/badge/Oasys-GSA-blue?logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAg1SURBVHgB7VtriExvGH927eaSNO6RZXxwiQ8WWRGZf0gpGb4IYVfkbncihOyua3LZJYqUWQoply3lEmWUXMplP2A/SDsRktvfnZb1f36v805nzsyc+zm7/vzq9M68M+ed8zzv+/ze53neZ4j+4s9GFjUC7t69G8jNza1oaGgoxHtuq37+/BkZOHDgv+QzfFfA/fv3y1jYYn4Z4JZwAdxC+F0DBgwoIx/hmwIePHgQzsrKqmBBg1Lod+/eibZ169aiVRQSV1ZDNfkAzxVQW1sb5CbKwodUs0179uyhEydOiPeTJ0+mhQsXJj5TvhfjtogVEScP4ZkC6urqAvX19aUsRIkUHLh06RJt27aNPn78mPR9rILi4mIaM2YMaUyjkptyr/jBEwU8fPiwhBsIH8B7CPPkyRNauXIlxeNx3Xt79OhB69evp7y8PC0/QAmV5DJcVQALHsrOzo6q7fzDhw9UWlpKN2/epB8/fpgap1mzZjRo0CBau3ZtCj+wKcEsYuQSXFEAL/cgNyC4sNrO9+7dS8ePH6dv376RHTRv3pwmTJhACxYsSIypXNWs6Igb/OBIAbBzbuRyT/Tfu3ePli1bRm/evCE30LZtW1qzZg2xwFp+KGdFVDrhB9sKePz4cSE7MBWk7OfAp0+faPbs2fTo0SPyAt27d6fdu3cLs9DwQ2Tw4MFVZAOWFcBkFlIEz1ceQPRHIhG6evUqvDryEuCH/Px82rp1q1oJ+N0aPMaQIUNiFoYzr4Dnz58Hsa3xy0L5o8DJkydpx44d9PXrV/IT4IcZM2bQlClTSD6P8kxV/Jzlw4YNi5sZx1ABsPOcnBzYuXBf5Y+9ePGCpk+fTq9fv6bGRCAQEBMA81CRpHCr+bkN+UFXAU+fPg3TL3YP4j0G//z5My1atAgBDTUl9OnThzZt2kRt2rRRK0K41UOHDs3oVmdUwLNnzy7zzSH5HgNu3LiRTp06ZXo/9xvgh5EjR9Lq1au1u0WsoKDgn3T3pFUA2zv89stygGvXrlFJSYnvdm4X4IclS5bQ2LFjSbU9/5OOILP1BmKvizZv3kzz58//bYQH4Hht376dtmzZImTQQw4Z4NWrV+QWWrRoIVzcxYsXE+/bKZ/DcTpz5gxVV1cjfCaneP/+fUIBakdNDUMFuAHYJgKhWbNm6X6vXbt24ju4oAxscexwkV2wl2iogIwmIG/EIE7QsWNHunPnjqHwWkAZFy9eFIQGBdqBFFrPDAw5wAn69esnvEMs/XS4fft24soUN0BxZ8+eta0EKQP7BGk/z9G7MdOyMQM4JqdPn07pRz4AbnM6G4eiwNzYbtVKCwaDQgnjx4+3tAVDBqNJ9GQF4OFBZloUFRXRuHHjMhIcdhrcx46LYeLEDOTz2zYBuzh48GDKsp80aZLwJ8wAisBsSyWgtTr7Eo5WgJkBtADpabc4zLzVbQ3CQuhDhw45Et7IDFzngLlz5ya9x+yZnXktIDQcMaeALJlI0HUOkOGpBAivMWHLBDJpywiIxLS274ZHZxdmTMBVDkAQogb298aGbQ7wE1g5+/btM/VdBGbw8a1Aj888c4SsACsnXXCU6btmIWdfT44cowF+dzj2A6xAewBidla9ht42aMgBVqJB2Ca8OPVOgIDIaCdAIDRixIiU/pYtW4qIUA0ctZmFGRNw3RXGUZgaFRUVhvfA4Xn58mXKJc8FJaBcO5kpX2OBAwcOJL1HJDd8+HCyA63yrly5Qm7DMCFiFZg57f4fjUaFKVgBjtigPDU2bNhAVtBo4XC6JCpyAzjpNQMIjxSaGlAqlGsVUgm2YgE5gFWADKdOnZrSj0zt9evXM64GRJJHjx5NER7KhFLswpEnaHcVgPmRA9BmhZDrk31qU+nfv3/a1BmEx2GHXfJz5Ag5hVTCsWPH0gpn5CdI4a26vlo42gWcusNQAlJcFy5csHQfEiE4Q/BSeMCXYAgzuXTpUrEKpk2bJnIGWoYHkDyBHwEecOMkSr0L2A6GOnfuTG4BQiFfiMsPoLQG0OMBw21wxYoVYkYy5fabIvCsqBlYt26dPT+At6MYN7jEALDFmpoasRXZPaDwA4hbwuGwOIwZNWpUop9liPXq1SuW7h5d9fDBqCiQ4CsolxAivnnz5tGNGzeoKQF1Qyi/RQAFKM8b5yvSs2dP6wUSEm/fvg18//69hLVYLCs/AURwEydOtOWduYn27dvTkSNHqFOnTok+lMjwtYtfVrLw9ktk1GBFBDlqK+WBC9X958+fp1WrVvlePwA7h8cIP0NdLcaoamhoKGfB42bGsezm8YyH6JdZ5Kv7ly9fTufOnfO8fAYcFAqFaOfOnUnMjjI5VI/m5eXFyAJs57yYHwrpVwFVwiywChAD1NbWkhfo3bs3HT58mFq1apVUKMnmGenWrVsV2YCjpJ/kB35Zqu6vq6ujmTNnusYPHTp0oP379xMzeVI/ljr/vqGd68GVrCf4gR8EZhFW98PhQWmrXX6AnWPHmTNnTqJPmXlRLN2lS5c4OYSraV/wAy/HqKwrBCA8qjwQC1gplx89erSo+8O2prL1OMrlWfAYuQRP8t6sCGkWicpSbJuYSSN+6Nu3r1juWPbyXvmHia5duzbtP0yoofADlABlJGbx1q1botJUG+XhdAjmUlBQoPXbK798+VLuxM714PnJB4qsOR0V5ZchtWDIE8JfB3CCrM34oLoTf5pyw8714NvRD9xqpcw+KPskOYLsVNsa7DzCUej/429zWjA/lHGT5FYrEO4rC15GPqJRDv/AD/X19VgNhUpXVW5uboTjd9//OvsXfzr+AyzhWcdnbajCAAAAAElFTkSuQmCC)](https://www.oasys-software.com/products/gsa/)

GSA-Grasshopper is a plugin for Grasshopper wrapping Oasys GSA's .NET API. The plugin allows users of Grasshopper to create, edit and analyse GSA models seemlesly. GsaGH requires [GSA](https://www.oasys-software.com/products/gsa/) to be installed.

