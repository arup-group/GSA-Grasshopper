# GSA-Grasshopper
![Downloads](https://img.shields.io/badge/dynamic/json?color=success&label=downloads&query=download_count&url=https%3A%2F%2Fyak.rhino3d.com%2Fpackages%2Fgsa&style=flat-square&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAACXBIWXMAAAsTAAALEwEAmpwYAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAB3SURBVHgB7ZRBDoAgDASr8SH8/1PwE6yHJogEd5UeNMyNpN2hhFTEmQUtzMqpUUH6VnFmCqbASaBfPuaKRk1NhAXu6G1Ca4oOR21gHIyEDyckz8MByfvwjoQLty5Qcht+yUMEDGXe91fFVh5GPpNhEyQZT5JfsAN5UByV3bhHmAAAAABJRU5ErkJggg==) [![Install plugin](https://img.shields.io/badge/install-Food4Rhino-green?style=flat-square&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAACXBIWXMAAAsTAAALEwEAmpwYAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAADxSURBVHgB3ZTtDcIgEIaPxgHqBnUCV3CEbmA3cAVXcIJ2A0foCm7QOoHdAF8SGgFpORB/6JO8oSUH3Acc0ZcRHCMpZYmhgSYhREe5wQGtfNHHrOVGIK1FgJgUTLvLwnceVA3m/FAkG44RMjJh74kSsHKZ4qHhhLcu3Bok838HjHq8QVtop7/zgBpX0Fm3hnnuIHmU2r7RKmMO5nD1tZTgk1dRYRgoAXV1C2ezEzRAtTF9pDTGtxkn5NYJOYTKe6UdfMxOxr7kDrpDe8iMktdhAx7Wjq11u5b2ZEfg89C0/7gXKY/X/rmbrNE7ti0nRb/PEyfNcxAV2WX+AAAAAElFTkSuQmCC)](https://www.food4rhino.com/en/app/gsa)

A Grasshopper plugin for structural analysis using Oasys GSA.

| Latest | CI Pipeline | Unit Tests | Deployment | Dependencies |
| ------ | ----------- | ---------- | ---------- | ------------ |
| [![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/arup-group/GSA-Grasshopper?include_prereleases&logo=github&style=flat-square)](https://github.com/arup-group/GSA-Grasshopper/releases) <br /> [![Yak](https://img.shields.io/badge/dynamic/json?color=blue&label=yak&prefix=v&query=version&url=https%3A%2F%2Fyak.rhino3d.com%2Fpackages%2Fgsa&logo=rhinoceros&style=flat-square)](https://yak.rhino3d.com/packages/gsa) <br /> [![Nuget](https://img.shields.io/nuget/vpre/gsagh?logo=nuget&style=flat-square)](https://www.nuget.org/packages/GsaGH) | [![Azure DevOps builds](https://img.shields.io/azure-devops/build/oasys-software/89fd051d-5c77-48bf-9b0e-05bca3e3e596/139?logo=azurepipelines&style=flat-square&label=Azure%20Pipelines)](https://dev.azure.com/oasys-software/OASYS%20libraries/_build?definitionId=139) <br /> [![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/arup-group/gsa-grasshopper/jira-check-branch.yml?label=jira-branch&style=flat-square)](https://github.com/arup-group/GSA-Grasshopper/actions/workflows/jira-check-branch.yml) <br /> [![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/arup-group/gsa-grasshopper/jira-pr-check.yml?label=jira-pr&style=flat-square)](https://github.com/arup-group/GSA-Grasshopper/actions/workflows/jira-pr-check.yml) | [![codecov](https://img.shields.io/codecov/c/github/arup-group/gsa-grasshopper?logo=codecov&logoColor=white&token=MB9FPYAICX&style=flat-square)](https://codecov.io/gh/arup-group/GSA-Grasshopper) | [![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/arup-group/gsa-grasshopper/github-release-yak.yml?label=Push%20Yak%20package&logo=rhinoceros&style=flat-square)](https://github.com/arup-group/GSA-Grasshopper/actions/workflows/github-release-yak.yml) <br /> [![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/arup-group/gsa-grasshopper/github-release-nuget.yml?label=Push%20NuGet%20package&logo=nuget&style=flat-square)](https://github.com/arup-group/GSA-Grasshopper/actions/workflows/github-release-nuget.yml) | [![Libraries.io dependency status for GitHub repo](https://img.shields.io/librariesio/github/arup-group/gsa-grasshopper?logo=nuget&style=flat-square)](https://libraries.io/nuget/GsaGH) <br /> [![Dependents (via libraries.io)](https://img.shields.io/librariesio/dependents/nuget/gsagh?logo=librariesdotio&logoColor=white)](https://libraries.io/nuget/GsaGH) |

## Documentation
[![Docs](https://img.shields.io/badge/Docs-GsaGH%20Docs-125DA9?logo=readme&logoColor=white&style=flat-square)](https://docs.oasys-software.com/structural/gsa/explanations/gsagh-introduction.html?source=GsaGhGithub)

Head over to our GSA documentation site for:
- [Introduction](https://docs.oasys-software.com/structural/gsa/explanations/gsagh-introduction.html?source=GsaGhGithub)
- [Installing the plugin in Grasshopper](https://docs.oasys-software.com/structural/gsa/tutorials/gsagh-installing-grasshopper-plugin.html?source=GsaGhGithub)
- [Parameters overview](https://docs.oasys-software.com/structural/gsa/explanations/gsagh-parameters.html?source=GsaGhGithub)
- [Components overview](https://docs.oasys-software.com/structural/gsa/explanations/gsagh-components.html?source=GsaGhGithub)

Tutorials and more example files to come shortly!

## Example Files
[![ExampleFiles](https://img.shields.io/badge/Grasshopper-Example%20Files-green?logo=data:image/png;base64,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&style=flat-square)](/ExampleFiles)

This repository contains a number of example files that are also used for testing on new releases, please check out the [ExampleFiles folder](/ExampleFiles) for a growing list of Grasshopper files.

## Contributing
![GitHub pull requests](https://img.shields.io/github/issues-pr-raw/arup-group/gsa-grasshopper?style=flat-square&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAACXBIWXMAAAsTAAALEwEAmpwYAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAADNSURBVHgB7VXbDcMgDHSjDpCRMgojZIOwQdMJ6CYZiRFco7qSi3gYSqV+5KSTcOTzEZAOgH8EIm7QgyAkeuat0IPQijAQX3DEB69tanivQdi1E3Uw8anhEQ6iyc2dSp5Rfcn0LURXvZdwHOKIHK/3TA9yPRNX8TdLzWQXzTbTs8Z3IIwPqEHusNBjonpmmYcRBi26CX6M0+A0GGwg8wQzcZ2CSoeKuP5Kh5W4LhgUdS1xrcWH7irWd+Im4sTwtxr0Oo7r95tsQYle3RA8AW71bDMxGCXJAAAAAElFTkSuQmCC) 
![GitHub closed pull requests](https://img.shields.io/github/issues-pr-closed-raw/arup-group/gsa-grasshopper?style=flat-square&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAPCAYAAADtc08vAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAACbSURBVHgBnVJbEYAgELwIRCCCEYxgBJtoBBsYwQhGuQhEOL2Zc1wRFNgZPoB9sABRA0TEncNTC1QoN1ytWJMZDHyNXg1WEM+xs/8RjyDeKNOpy4iVE4zDjzBLv7BnDBg4Q4qwAKGP9qZk74jk8IiwPoCY6QvqjknWm20epOTJIsEGhiOVQPvLGyvVQF8Ce0vDd+3AwFML7AKLkg/1iD3k8fl0tQAAAABJRU5ErkJggg==)
![GitHub commit activity](https://img.shields.io/github/commit-activity/m/arup-group/gsa-grasshopper?style=flat-square&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAACXBIWXMAAAsTAAALEwEAmpwYAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAC7SURBVHgB7VWBDYMwDMt2wU7gBE7ZCftg/WD9YPuAE3bCTukJOyEkUpBCRdsUgRAISxUlqe1ErVqAE2sDEQMmwPkr7AVUcBN3YyE5Gp7JFeLBZED5tyqGSa1RvCkaUO6uxL9q7kriEk8bRCQvMa9MXjlxyfF/SBl0QvpFcRd1NSmeBS185ki8Dzg+KVXi3PJfiI/Cuo+MG1ihKutgafDGqZbtVVUYDCeihROHw2WYJO+OucIE/u7nwdkMPUCUeIAsCfP+AAAAAElFTkSuQmCC)
![GitHub top language](https://img.shields.io/github/languages/top/arup-group/gsa-grasshopper?logo=dotnet&style=flat-square)
![GitHub contributors](https://img.shields.io/github/contributors/arup-group/gsa-grasshopper?style=flat-square&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAACXBIWXMAAAsTAAALEwEAmpwYAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAADnSURBVHgB7ZTRDcIwDETdTgAblAlgAzoCI7AJ2YBswAgdoWzACB2hI5i4ciULYseqkOhHn2Q1anKOfYkCsLEqEPGWYuS4/1RHE/hNcCT36Xj3T0bHBqauBidL7ZMJQqaSyHOqDZYut0kUVQZw2qDpiFos6tPnWVXVPn0PNE7/uhQ7sDsv69hfjYtmQ4rW0AW5waAsejjs6xXtOHXBFWo0IsmVBBxUecfjBvNnRLRzZWr1aNsXRHc5otXiqWAfMdmA+lm8tCs4cHLLvpnWKMR+CVBvXRKtHKWn4ghlzrAU1G+H34aNv/MGGsRggTYOMdwAAAAASUVORK5CYII=)

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?&style=flat-square&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAACXBIWXMAAAsTAAALEwEAmpwYAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAADLSURBVHgB7ZPRDcIwDESvnYAROgIbkA1ghG7CKDACTNBs0m5QNgiO5I/IuLGLhJCgT7JUWXe5OHKBvyIxazytckhIC8gghQDHLYf0PoN1eFe7jZzE45NPdC6+T/Bj+yh5J8adc09oXiawfG0lOYt62FR9ZcBRMQfY+Hw8mmQWGu2Jqr6mNEOhIaRG6y35yieKiu4Gm+jy8S5feeRcF+cWmT43WoBFiw+zBXw/oNGavGY91YFqz+1OyB5UE9edKtK/NcEDBYxpPSN+kidmAJvClBsULQAAAABJRU5ErkJggg==)](/LICENSE)

This plugin is free to use, however in order to perform any meaningful tasks you must have GSA installed on your machine. For licensing of GSA refer to Oasys Software [licensing terms](https://www.oasys-software.com/support/licensing-of-oasys-software/).

For third party packages used by GSA-Grasshopper, the individual licenses apply. See a list of these in the ['License'](/GsaGH/licenses) subfolder of each project.

## About
[![GSA](https://img.shields.io/badge/Oasys-GSA-125DA9?style=flat-square&logoWidth=35&logo=data:image/png;base64,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)](https://www.oasys-software.com/products/gsa/)

GSA-Grasshopper is a plugin for Grasshopper wrapping Oasys GSA's .NET API. The plugin allows users of Grasshopper to create, edit and analyse GSA models seemlesly. GsaGH requires [GSA](https://www.oasys-software.com/products/gsa/) to be installed.

