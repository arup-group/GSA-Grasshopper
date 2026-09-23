# Documentation Generator CLI

`DocsGeneratorCLI` loads a Grasshopper plugin assembly and generates Markdown
documentation from its components and parameters. It supports the in-repository
`GsaGH` project and an opt-in checkout of
[AdSec-Grasshopper](https://github.com/arup-group/AdSec-Grasshopper).

## Prerequisites

1. Use 64-bit Windows with Rhino 7 or 8, Grasshopper, and the .NET Framework
   4.8 developer pack installed.
2. Install GSA in `C:\Program Files\Oasys\GSA 10.3`. This is currently the
   path hard-coded in `Fixtures\GrasshopperFixture.cs`.
3. Build from the root of this repository, `GSA-GH`.
4. For AdSecGH, install a licensed AdSec API and clone
   `arup-group/AdSec-Grasshopper` alongside this repository:

   ```text
   D:\REPOS\
   ├── GSA-GH\
   └── AdSec-Grasshopper\
   ```

## Build the CLI

Build the CLI and its normal GsaGH dependency:

```powershell
dotnet build .\DocsGeneratorCLI\DocsGeneratorCLI.csproj -c Debug -p:Platform=x64
```

For AdSecGH, build the CLI with `AdSecGHProjectPath` set to the external
project. The conditional project reference copies `AdSecGH.dll` and its build
dependencies to the CLI output directory, where Grasshopper can load them.

```powershell
$adSecProject = (Resolve-Path ..\AdSec-Grasshopper\AdSecGH\AdSecGH.csproj).Path
dotnet build .\DocsGeneratorCLI\DocsGeneratorCLI.csproj -c Debug -p:Platform=x64 `
  "-p:AdSecGHProjectPath=$adSecProject"
```

If AdSec-Grasshopper is elsewhere, set `$adSecProject` to its absolute
`AdSecGH.csproj` path instead.

## Generate GsaGH documentation

From the `GSA-GH` repository root, run:

```powershell
.\DocsGeneratorCLI\bin\x64\Debug\net48\DocsGeneratorCLI.exe -p GsaGH -o gsagh-docs
```

The generated Markdown files are written to `.\gsagh-docs`.

## Generate AdSecGH documentation

After the AdSec-aware build above, run:

```powershell
.\DocsGeneratorCLI\bin\x64\Debug\net48\DocsGeneratorCLI.exe -p AdSecGH -o adsecgh-docs
```

The generated Markdown files are written to `.\adsecgh-docs`.

Before running it, this should succeed:

```powershell
Test-Path .\DocsGeneratorCLI\bin\x64\Debug\net48\AdSecGH.dll
```

## Why the AdSec command previously failed

`DocsGeneratorCLI.csproj` always references `GsaGH`, so its build places
`GsaGH.dll` beside `DocsGeneratorCLI.exe`. AdSecGH is in a separate repository,
and the original AdSec support did not add a project reference or copy its
output. Passing `-p AdSecGH` selects the assembly at runtime; it does not build
or locate a separate AdSec-Grasshopper checkout. Consequently the loader could
not find `AdSecGH.dll` and then failed with `Assembly not loaded`.

Always rebuild with `AdSecGHProjectPath` after changing AdSec-Grasshopper.
Copying only `AdSecGH.dll` is not sufficient because its dependent assemblies
must be available in the same output directory.
