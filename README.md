# GSA-Grasshopper
![GitHub branch checks state](https://img.shields.io/badge/dynamic/json?color=blue&label=Downloads&query=download_count&url=https%3A%2F%2Fyak.rhino3d.com%2Fpackages%2Fgsa) [![Install plugin](https://img.shields.io/badge/Install-Food4Rhino-lightgrey)](https://www.food4rhino.com/en/app/gsa)

A Grasshopper plugin for structural analysis using Oasys GSA.

| Latest | CI Pipeline | Unit Tests | Deployment | Dependencies |
| ------ | ----------- | ---------- | ---------- | ------------ |
| [![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/arup-group/GSA-Grasshopper?include_prereleases)](https://github.com/arup-group/GSA-Grasshopper/releases) <br /> ![Yak](https://img.shields.io/badge/dynamic/json?color=blue&label=Yak&prefix=v&query=version&url=https%3A%2F%2Fyak.rhino3d.com%2Fpackages%2Fgsa) <br /> ![Nuget](https://img.shields.io/nuget/vpre/gsagh) | [![Build Status](https://dev.azure.com/oasys-software/OASYS%20libraries/_apis/build/status/arup-group.GSA-Grasshopper?branchName=main)](https://dev.azure.com/oasys-software/OASYS%20libraries/_build/latest?definitionId=139&branchName=main) <br /> ![GitHub branch checks state](https://img.shields.io/github/checks-status/arup-group/gsa-grasshopper/main) | [![codecov](https://codecov.io/gh/arup-group/GSA-Grasshopper/branch/main/graph/badge.svg?token=MB9FPYAICX)](https://codecov.io/gh/arup-group/GSA-Grasshopper) <br /> ![Azure DevOps tests](https://img.shields.io/azure-devops/tests/oasys-software/OASYS%2520libraries/139/main?compact_message) | ![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/arup-group/gsa-grasshopper/github-release-yak.yml?label=yak) <br /> ![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/arup-group/gsa-grasshopper/github-release-nuget.yml?label=nuget) | ![Libraries.io dependency status for GitHub repo](https://img.shields.io/librariesio/github/arup-group/gsa-grasshopper) |

## Documentation
[![Docs](https://img.shields.io/badge/Docs-GsaGH%20Introduction-blue)](https://docs.oasys-software.com/structural/gsa/explanations/gsagh-introduction.html?source=GsaGhGithub)

Head over to our GSA documentation site for:
- [Introduction](https://docs.oasys-software.com/structural/gsa/explanations/gsagh-introduction.html?source=GsaGhGithub)
- [Installing the plugin in Grasshopper](https://docs.oasys-software.com/structural/gsa/tutorials/gsagh-installing-grasshopper-plugin.html?source=GsaGhGithub)
- [Parameters overview](https://docs.oasys-software.com/structural/gsa/explanations/gsagh-parameters.html?source=GsaGhGithub)
- [Components overview](https://docs.oasys-software.com/structural/gsa/explanations/gsagh-components.html?source=GsaGhGithub)

Tutorials and more example files to come shortly!

## Example Files
[![Docs](https://img.shields.io/badge/Grasshopper-Example%20Files-green)](/ExampleFiles)

This repository contains a number of example files that are also used for testing on new releases, please check out the [ExampleFiles folder](/ExampleFiles) for a growing list of Grasshopper files.

## Contributing
![GitHub pull requests](https://img.shields.io/github/issues-pr-raw/arup-group/gsa-grasshopper) ![GitHub closed pull requests](https://img.shields.io/github/issues-pr-closed-raw/arup-group/gsa-grasshopper?logoColor=brightgreen)
![GitHub commit activity](https://img.shields.io/github/commit-activity/m/arup-group/gsa-grasshopper)
![GitHub contributors](https://img.shields.io/github/contributors/arup-group/gsa-grasshopper)

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

This plugin is free to use, however in order to perform any meaningful tasks you must have GSA installed on your machine. For licensing of GSA refer to Oasys Software licensing terms: https://www.oasys-software.com/support/licensing-of-oasys-software/

## About
[![GSA](https://img.shields.io/badge/Oasys-GSA-blue)](https://www.oasys-software.com/products/gsa/)

GSA-Grasshopper is a plugin for Grasshopper wrapping Oasys GSA's .NET API. The plugin allows users of Grasshopper to create, edit and analyse GSA models seemlesly. GsaGH requires [GSA](https://www.oasys-software.com/products/gsa/) to be installed.

