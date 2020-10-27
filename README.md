# GSA-Grasshopper

GSA-Grasshopper is a .NET component library giving Grasshopper access to Oasys GSA files allowing interfacing with existing models as well as creating and editing new ones.

![Alt text](readme-screenshot.JPG?raw=true "Title")

  - [Installation](#installation)
  - [List of components by category](#list-of-components-by-category)
    * [Model](#model)
    * [Properties](#properties)
    * [Geometry](#geometry)
    * [Loads](#loads)
    * [Analysis](#analysis)
    * [Results](#results)
  - [Contributing](#contributing)
  - [License](#license)

<br/>

## Installation

Use the Rhino package manager [yak](https://developer.rhino3d.com/guides/yak/what-is-yak/) to install GSA-Grasshopper. You can either use the Rhino interface or the yak CLI tool. For the Rhino interface, simply type TestPackageManager in the Rhino command line and search for GSA. In the future, when you share a file using this plugin with a colleague who doesnt have it installed, they will automatically be prompted to download the [latest version](https://developer.rhino3d.com/guides/yak/package-restore-in-grasshopper/). <br/>

Alternatively, to install via the yak CLI:

```bash
yak install GSA
```
<br/>

## List of components by category

### Model

|Component |Description|
| ----------- | ------------- |
| **Open:** | Open an existing GSA model in .gwb format
| **Save:** | Save a GSA-Grasshopper model to .gwb
| **Get Geometry:** | Get geometrical objects from a model (Nodes, Element1D, Element2D, Member1D, Member2D)
| **Get Properties:** | Get properties from a model
| **Edit titles:** | Edit the model titles
| **Edit units:** | Edit the working document units
<br/>

### Properties

|Component |Description|
| ----------- | ------------- |
| **Create Prop2D:** | Create a new 2D property for Elem2D and Mem2D
| **Create Section:** | Create a new Section for Elem1D and Mem1D
| **Create Profile:** | Create a profile (cross-section) for new Section
| **Create Bool6:** | Create a new Bool6 which can be used for either setting releases or support restraints (x, y, z, xx, yy or zz)
| **Create Offset:** | Create a new offset used for both 1D and 2D elements
| **Create Spring:** | Create a new spring. Springs attach to Nodes
| **Edit Prop2D:** | Edit an existing 2D property
| **Edit Section:** | Edit an existing Section
| **Edit Bool6:** | Edit an existing Bool6 
| **Edit Offset:** | Edit an existing offset
| **Edit Spring:** | Edit an existing spring 
| **Section Properties:** | Get the properties of a section or profile (Area, Moment of Inertia, etc.)
<br/>

### Geometry

|Component |Description|
| ----------- | ------------- |
| **Create Element1D:** | Create a new Element 1D in analysis layer (Line)
| **Create Element2D:** | Create a new Element 2D in analysis layer (Mesh)
| **Create Member1D:** | Create a new Member 1D in Design layer (Arc or Line)
| **Create Member2D:** | Create a new Member 2D in Design layer (Brep/Surface)
| **Create Support:** | Create a node with support restraints
| **Edit Element1D:** | Edit an existing Element 1D
| **Edit Element2D:** | Edit an existing Element 2D
| **Edit Member1D:** | Edit an existing Member 1D
| **Edit Member2D:** | Edit an existing Member 2D
| **Edit Node:** | Edit an existing node
<br/>

### Loads

|Component |Description|
| ----------- | ------------- |
| **Create Gravity Load:** | Create a new Gravity Load - applies to everything in model
| **Create Node Load:** | Create a new Node Load - applies to nodes
| **Create Beam Load:** | Create a new Beam Load applies to Elem1D
| **Create Face Load:** | Create a new Face Load applies to Elem2D
| **Create Grid Point Load:** | Create a new Grid Point Load, free form point load
| **Create Grid Line Load:** | Create a new Grid Line Load, free form line load
| **Create Grid Area Load:** | Create a new Grid Area Load, free form area load
| **Create Grid Plane:** | Create a new Grid Plane; a plane used for Grid Plane Surface
| **Create Grid Surface:** | Create a new Grid Surface; used for all Grid Load types
| **Grid Plane Surface Properties:** | Get the properties of a Grid Plane and Grid Surface
| Analysis Case: | Create analysis cases (to be done)
<br/>

### Analysis

|Component |Description|
| ----------- | ------------- |
| **Analyse:** | Assemble all objects and run analysis
| Analysis tasks: | Create/Set Analysis tasks and analysis settings (to be done)
<br/>

### Results

|Component |Description|
| ----------- | ------------- |
| **Node results:** | Get typical node results as displacement or reaction forces
| **Element 1D Results:** | Get typically bending moments or other internal force types
| **Element 2D Results:** | Get surface plot with relevant internal forces
| Member 1D Results: | Gets more design type results (utilisation) (to be done)
| Member 2D Results: | Gets typically design or reinforcement plot (to be done)
| Design Tasks: | Get design task results (to be done)
<br/>

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.
<br/>

## License
This plugin is free to use, however in order to perform any meaningful tasks you must have GSA installed on your machine. For licensing of GSA refer to Oasys Software licensing terms: https://www.oasys-software.com/support/licensing-of-oasys-software/
