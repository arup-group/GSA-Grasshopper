# GSA-Grasshopper

GSA-Grasshopper is a plugin for Grasshopper wrapping Oasys GSA's .NET API. The plugin allows users of Grasshopper to create, edit and analyse GSA models seemlesly. 

![image](https://user-images.githubusercontent.com/25223248/118804859-7fe8a600-b8a5-11eb-8c6d-e8abb30fdcea.png)

  - [Installation](#installation)
  - [List of components by category](#list-of-components-by-category)
    * [Model](#model)
    * [Properties](#properties)
    * [Geometry](#geometry)
    * [Loads](#loads)
    * [Analysis](#analysis)
    * [Results](#results)
    * [Parameters](#parameters)
  - [Contributing](#contributing)
  - [License](#license)

<br/>


## Installation
### Install GSA-Grasshopper using the Package Manager in Rhino 7 for Windows:

  1. To install [GSA-Grasshopper, click in this link](http://rhino://package/search?name=gsa) 
  1. Or, type `PackageManager` on the Rhino command line.
  1. Search for “GSA”
  1. Select GSA and then Install

<img src="https://user-images.githubusercontent.com/25223248/118797502-f208bd00-b89c-11eb-9d5d-d1f934ab92d0.PNG" alt="Rhino7 Package Manager" width="600"/>

<br/>
### Rhino 6
Use the command "TestPackageManager" and search for GSA.

## List of components by category

### Model

|Component |Name |Description|
| ----------- | ----------- | ------------- |
|![OpenModel@4x](https://user-images.githubusercontent.com/25223248/118806051-ea4e1600-b8a6-11eb-8892-215013fdd9ee.png)| **Open** | Open an existing GSA model in .gwb format
|![SaveModel@4x](https://user-images.githubusercontent.com/25223248/118806071-f043f700-b8a6-11eb-8300-38e7e36df1b2.png)| **Save** | Save a GSA-Grasshopper model to .gwb
|![GetGeometry@4x](https://user-images.githubusercontent.com/25223248/118806118-fe921300-b8a6-11eb-875d-9e91db13e943.png)| **GetGeometry** | Get geometrical objects from a GSA model (Nodes, Element1Ds, Element2Ds, Element3Ds, Member1Ds, Member2Ds, Member3Ds)
|![GetProperties@4x](https://user-images.githubusercontent.com/25223248/118806142-02be3080-b8a7-11eb-8571-654445d0bac8.png)| **GetProperties** | Get properties from a GSA model (Sections, 2D Properties)
|![GetLoads@4x](https://user-images.githubusercontent.com/25223248/118806148-05208a80-b8a7-11eb-8bfe-99eafd54654d.png)| **GetLoads** | Get Loads and Grid Planes/Surfaces from GSA model
|![EditTitle@4x](https://user-images.githubusercontent.com/25223248/118806166-08b41180-b8a7-11eb-8373-67edc5e9154a.png)| **EditTitles (WIP)** | Edit the GSA model titles (currently titles cannot be set back into GSA model)
|![EditUnits@4x](https://user-images.githubusercontent.com/25223248/118806183-0c479880-b8a7-11eb-85fd-42083f995748.png)| **EditUnits (WIP)** | Edit the working document units (currently units cannot be changed in GSA model, use geometry in meters, sections in mm and force in kN for now)
|![GsaVersion@4x](https://user-images.githubusercontent.com/25223248/118806329-36995600-b8a7-11eb-9d38-b82c023fda89.png)| **Version** | Get the plugin version and location of .gha file

<br/>

### Properties

|Component |Name |Description|
| ----------- | ----------- | ------------- |
|![CreateMaterial@4x](https://user-images.githubusercontent.com/25223248/118807820-0eaaf200-b8a9-11eb-9a9d-300b543f0769.png)| **CreateMaterial (WIP)** | Create a new material (currently material must already exist in the GSA model, this component will for now only create the reference to existing materials only)
|![CreateProfile@4x](https://user-images.githubusercontent.com/25223248/118807841-15396980-b8a9-11eb-8ac2-18e71e7b7579.png)| **CreateProfile** | Create a profile (cross-section) for new Section. This component can look up Catalogue sections or create new shapes from scratch, either from a a closed polygon or from a range of typical structural engineering cross-sections (Rectangle, Circle, I section, Tee, Channel, Angle).
|![CreateSection@4x](https://user-images.githubusercontent.com/25223248/118807863-1a96b400-b8a9-11eb-94d2-9055734da2e3.png)| **CreateSection** | Create a new Section for Elem1D and Mem1D using Profile and Material inputs.
|![CreateProp2D@4x](https://user-images.githubusercontent.com/25223248/118807897-25514900-b8a9-11eb-97e8-c7bae3327281.png)| **CreateProp2D** | Create a new 2D property for Elem2D and Mem2D using Thickness and Material inputs
|![CreateBool6@4x](https://user-images.githubusercontent.com/25223248/118807916-2c785700-b8a9-11eb-9c93-00302614d227.png)| **CreateBool6** | Create a new Bool6 which can be used for either setting releases or support restraints (x, y, z, xx, yy or zz)
|![CreateOffset@4x](https://user-images.githubusercontent.com/25223248/118807939-369a5580-b8a9-11eb-801b-bbb5266fb0f9.png)| **CreateOffset** | Create a new offset used for both 1D and 2D elements
|![CreateSpring@4x](https://user-images.githubusercontent.com/25223248/118807970-3dc16380-b8a9-11eb-9cdc-bb00cfdd7307.png)| **CreateSpring (WIP)** | Create a new spring. Springs attach to Nodes (springs are currently not implemented)
|![EditSection@4x](https://user-images.githubusercontent.com/25223248/118808011-47e36200-b8a9-11eb-9b50-5591975c26db.png)| **EditSection** | Edit an existing Section or use this component to view an existing Section's values
|![EditProp2D@4x](https://user-images.githubusercontent.com/25223248/118808029-4d40ac80-b8a9-11eb-96fc-d62cfc2434cf.png)| **EditProp2D** | Edit an existing 2D property or use this component to view an existing Prop2D's values
|![SectionProperties@4x](https://user-images.githubusercontent.com/25223248/118808067-5893d800-b8a9-11eb-94f0-d085902cf093.png)| **SectionProperties** | Get the mechanical cross-section properties of a Section (or profile) such as Area, Moment of Inertia, etc.
|![EditBool6@4x](https://user-images.githubusercontent.com/25223248/118808086-5f224f80-b8a9-11eb-94fb-df36a290e964.png)| **EditBool6** | Edit an existing Bool6 or use this component to view an existing Bool6's settings
|![EditOffset@4x](https://user-images.githubusercontent.com/25223248/118808102-647f9a00-b8a9-11eb-8a84-1249597ceec4.png)| **EditOffset** | Edit an existing Offset or use this component to view an existing Offset's values
|![EditSpring@4x](https://user-images.githubusercontent.com/25223248/118808173-78c39700-b8a9-11eb-97ad-9bb9dce37bf7.png)| **EditSpring (WIP)** | Edit an existing spring (currently not implemented)

<br/>

### Geometry

|Component |Name |Description|
| ----------- | ----------- | ------------- |
|![CreateSupport@4x](https://user-images.githubusercontent.com/25223248/118812905-e9b97d80-b8ae-11eb-8b1f-4ecd66d074de.png)| **Create Support** | Create a Node with support restraints (Rhino/GH: Point -- GSA: Node)
|![CreateElem1D@4x](https://user-images.githubusercontent.com/25223248/118812998-ffc73e00-b8ae-11eb-9305-280f414d184d.png)| **Create Element1D** | Create a new 1D Element in analysis layer (Rhino/GH: Line -- GSA: Beam Element)
|![CreateElem2D@4x](https://user-images.githubusercontent.com/25223248/118813069-1077b400-b8af-11eb-9b12-77a655cec4af.png)| **Create Element2D** | Create a new 2D Element in analysis layer (Rhino/GH: Mesh -- GSA: Tri/Quad Element). It is possible to provide Tri6 and Quad8 using Rhino Ngons currently not natively supported in Grasshopper but can be referenced from Rhino.
|![CreateMem1D@4x](https://user-images.githubusercontent.com/25223248/118813180-2e451900-b8af-11eb-9e93-d8e588bca867.png)| **Create Member1D** | Create a new 1D Member in Design layer (Rhino/GH: Arc or PolyLine -- GSA: Beam)
|![CreateMem2D@4x](https://user-images.githubusercontent.com/25223248/118813250-3dc46200-b8af-11eb-82f1-9fad8ae1defe.png)| **Create Member2D** | Create a new 2D Member in Design layer (Rhino/GH: Brep/Surface -- GSA: Slab). Voids in the geometry will automatically be detected. Input Points or Curves to be included in the analysis mesh - intersections with other Elements and Members will automatically be detected when a model is assembled and is therefore not required to be inputted here.
|![CreateMem3D@4x](https://user-images.githubusercontent.com/25223248/118813274-41f07f80-b8af-11eb-8727-d33f69303ba2.png)| **Create Member3D** | Create a new 3D Member in Design layer (Rhino/GH: Solid Brep/Surface -- GSA: Solid)
|![EditNode@4x](https://user-images.githubusercontent.com/25223248/118813309-49178d80-b8af-11eb-9087-d656a41d6f0b.png)| **EditNode** | Edit an existing node or use this component to view an existing Node's parameters like point, orientation plane, restraint, etc.
|![EditElem1D@4x](https://user-images.githubusercontent.com/25223248/118812924-ef16c800-b8ae-11eb-8ded-72aee4ded16b.png)| **EditElement1D** | Edit an existing 1D Element 1D or use this component to view an existing Element1D's parameters. Use this component to set more advanced settings like Element Analysis Type (Beam/Bar/Spring/Link/Cable/Spacer/Strut/Tie/Damper)
|![EditElem2D@4x](https://user-images.githubusercontent.com/25223248/118813335-4f0d6e80-b8af-11eb-94fb-c6a78b597f93.png)| **EditElement2D** | Edit an existing 2D Element or use this component to view an existing Element2D's parameters. The analysis type can not be set for Element2Ds as this is determined by the underlaying Mesh geometry where triangular faces will be a Tri3 element, quad faces will be a Quad4. Geometry cannot be edited directly for Element2Ds; either create new or use Grasshopper to transform Element2D (move/mirror/morhp etc).
|![EditElem3D@4x](https://user-images.githubusercontent.com/25223248/118813357-53398c00-b8af-11eb-9a54-d7b241eab043.png)| **EditElement3D** | Edit an existing 3D Element or use this component to view an existing Element3D's parameters. The analysis type can not be set for Element3Ds as this is determined by the underlaying NgonMesh geometry where number of verticies in each Ngon face will determine the analysis type element (Tetra4, Pyramid5, Wedge6 or Brick8). Geometry cannot be edited directly for Element3Ds; either create new from Member3D or use Grasshopper to transform existing Element3D (move/mirror/morhp etc).
|![EditMem1D@4x](https://user-images.githubusercontent.com/25223248/118813928-ea9edf00-b8af-11eb-8aeb-1cb7aa1d3798.png)| **EditMember1D** | Edit an existing 1D Member or use this component to view an existing Member1D's parameters. Use this component to set more advanced settings like Member Type (Beam/Column/Cantilever/Compos/Pile/Void-cutter) and Element Analysis Type (Beam/Bar/Spring/Link/Cable/Spacer/Strut/Tie/Damper)
|![EditMem2D@4x](https://user-images.githubusercontent.com/25223248/118813947-effc2980-b8af-11eb-932b-db78bc2dc8ef.png)| **EditMember2D** | Edit an existing 2D Member or use this component to view an existing Element3D's parameters. Use this component to set more advanced settings like Member Type (Slab/Wall/Ribbed-slab/Void-cutter) and Element Analysis Type (Linear (Tri3/Quad8), Quadratic (Tri6/Quad8) or Rigid-diaphragm)
|![EditMem3D@4x](https://user-images.githubusercontent.com/25223248/118814016-ff7b7280-b8af-11eb-8a17-8f93c6e3cb63.png)| **EditMember3D** | Edit an existing 3D Member or use this component to view an existing Element3D's parameters.
|![2dElemsFromBrep@4x](https://user-images.githubusercontent.com/25223248/118814059-0a360780-b8b0-11eb-9959-5042e3b56917.png)| **Elem2dFromBrep (WIP)** | Create 2D Elements from non-planar BReps. Use this component to create analysis mesh (2D elements) from non-planar surfaces. The output will be quadratic elements (Tri6/Quad8). The component works by unrolling the input to a planar surface, perform the normal remeshing using GSA's remeshing algorithm, re-map the analysis mesh back to the original input geometry.
|![CreateElementsFromMembers@4x](https://user-images.githubusercontent.com/25223248/118814081-0f935200-b8b0-11eb-8e51-93d805630811.png)| **ElemFromMem** | Create Elements from Members. This component is essentially doing remeshing using GSA's remeshing algorithm. 

<br/>

### Loads

|Component |Name |Description|
| ----------- | ----------- | ------------- |
|![GravityLoad@4x](https://user-images.githubusercontent.com/25223248/118814600-9c3e1000-b8b0-11eb-96e2-791a554f98a8.png)| **CreateGravityLoad** | Create a new Gravity Load
|![NodeLoad@4x](https://user-images.githubusercontent.com/25223248/118814640-a6600e80-b8b0-11eb-9384-c23dab07779f.png)| **CreateNodeLoad** | Create a new Node Load - applies to nodes
|![BeamLoad@4x](https://user-images.githubusercontent.com/25223248/118814660-ac55ef80-b8b0-11eb-93c2-a194688c2064.png)| **CreateBeamLoad** | Create a new Beam Load applies to Elem1D
|![FaceLoad@4x](https://user-images.githubusercontent.com/25223248/118814674-afe97680-b8b0-11eb-82a5-e8299dd720b1.png)| **CreateFaceLoad** | Create a new Face Load applies to Elem2D
|![GridPointLoad@4x](https://user-images.githubusercontent.com/25223248/118814719-bb3ca200-b8b0-11eb-9c13-ad111954ad75.png)| **CreateGridPointLoad** | Create a new Grid Point Load, free form point load
|![GridLineLoad@4x](https://user-images.githubusercontent.com/25223248/118814745-c099ec80-b8b0-11eb-9fe2-c2cf44f384ce.png)| **CreateGridLineLoad** | Create a new Grid Line Load, free form line load
|![GridAreaLoad@4x](https://user-images.githubusercontent.com/25223248/118814773-ca235480-b8b0-11eb-8534-2991666b2b65.png)| **CreateGridAreaLoad** | Create a new Grid Area Load, free form area load
|![GridPlane@4x](https://user-images.githubusercontent.com/25223248/118814795-cee80880-b8b0-11eb-9704-60400a578852.png)| **CreateGridPlane** | Create a new Grid Plane; a plane used for Grid Plane Surface
|![GridSurface@4x](https://user-images.githubusercontent.com/25223248/118814996-0e165980-b8b1-11eb-9fbd-51ce332e57bc.png)| **CreateGridSurface** | Create a new Grid Surface; used for all Grid Load types
|![GridPlaneProperties@4x](https://user-images.githubusercontent.com/25223248/118814926-f9d25c80-b8b0-11eb-8515-769b8eda381e.png)| **GridPlaneSurface Properties** | Get the properties of a Grid Plane and Grid Surface
|![LoadProp@4x](https://user-images.githubusercontent.com/25223248/118814936-fd65e380-b8b0-11eb-9381-8ae8e3a291a0.png)| **Load Properties** | Get the properties of a Load

<br/>

### Analysis

|Component |Name |Description|
| ----------- | ----------- | ------------- |
|![Analyse@4x](https://user-images.githubusercontent.com/25223248/118826392-c0531e80-b8bb-11eb-96c3-664643a06f30.png)| **Analyse** | Assemble all objects and run analysis
|![AsyncAnalyse@4x](https://user-images.githubusercontent.com/25223248/118826959-38b9df80-b8bc-11eb-9dcb-222c1c35f0ca.png)| **AsyncAnalyse (WIP)** | Assemble all objects and run analysis using asyncronious component

<br/>

### Results

|Component |Name |Description|
| ----------- | ----------- | ------------- |
|![NodeResults@4x](https://user-images.githubusercontent.com/25223248/118821615-b3343080-b8b7-11eb-9495-17c55a419520.png)| **Node results:** | View and get typical node results as displacement or reaction forces
|![Elem1dResults@4x](https://user-images.githubusercontent.com/25223248/118821633-b7f8e480-b8b7-11eb-8d40-28046b6f9784.png)| **Element1D Results:** | View and get 1D Element results for displacement and internal forces
|![Elem2dResults@4x](https://user-images.githubusercontent.com/25223248/118821641-baf3d500-b8b7-11eb-973c-045f97b7a0c0.png)| **Element2D Results:** | View and get 2D Element surface plot results for displacement, forces and moments and stress
|![Elem3dResults@4x](https://user-images.githubusercontent.com/25223248/118821655-bdeec580-b8b7-11eb-89ad-ca7aa7f5a055.png)| **Element3D Results:** | View and get 3D Element surface plot results for displacement and stress
|![GlobalResults@4x](https://user-images.githubusercontent.com/25223248/118821675-c2b37980-b8b7-11eb-966b-848655fb2c52.png)| **Global Results:** | Get global model results as sum of forces, mass and inertia as well as dynamic factors for dynamic analysis tasks

<br/>

### Parameters

|Component |Name |Description|
| ----------- | ----------- | ------------- |
|![GsaModel@4x](https://user-images.githubusercontent.com/25223248/118825762-402cb900-b8bb-11eb-82ca-793d6fee4c7b.png)| **GsaModel** | Parameter to pass around a GSA model between components
|![GsaMaterial@4x](https://user-images.githubusercontent.com/25223248/118825789-46bb3080-b8bb-11eb-834f-b9ffac02f59b.png)| **Material (WIP)** | GSA Material parameter. Current implementation only used to refer to existing materials in a GSA model
|![GsaSection@4x](https://user-images.githubusercontent.com/25223248/118825807-4c187b00-b8bb-11eb-997d-a1005ebafbc8.png)| **Section** | GSA Section parameter. A Section contains information for ID (in GSA: "PB1"), Material, and Profile
|![GsaProp2D@4x](https://user-images.githubusercontent.com/25223248/118825821-4e7ad500-b8bb-11eb-9e79-f6f86ff2fc30.png)| **Prop2D** | GSA 2D Property parameter. A Prop2D contains information for ID (in GSA: "PA1"), Material, and Thickness
|![GsaBool6@4x](https://user-images.githubusercontent.com/25223248/118825848-5470b600-b8bb-11eb-86ee-cc52b1f52d85.png)| **Bool6** | GSA Bool6 parameter. A Bool6 contains six booleans (X, Y, Z, XX, YY, ZZ) and can be used to set node restriants or Element1D/Member1D releases
|![GsaOffset@4x](https://user-images.githubusercontent.com/25223248/118825863-576ba680-b8bb-11eb-88bb-00871cd0807a.png)| **Offset** | GSA Offset parameter. An Offset contains four values (X1, X2, Y and Z) and can be used to set offset in elements and members
|![GsaSpring@4x](https://user-images.githubusercontent.com/25223248/118825879-5a669700-b8bb-11eb-8773-e2a12859e300.png)| **Spring (WIP)** | GSA Spring parameter. Not implemented yet.
|![GsaNode@4x](https://user-images.githubusercontent.com/25223248/118825896-5e92b480-b8bb-11eb-9822-cd00c603e8df.png)| **Node** | GSA Node parameter. A Node contains position (Point), local orientation (Plane), Restraint (Bool6), ID (node number in GSA) and spring when implemented.
|![GsaElem1D@4x](https://user-images.githubusercontent.com/25223248/118825920-63576880-b8bb-11eb-9bdc-38aa63848dd8.png)| **Element1D** | GSA 1D Element parameter. An Element1D contains geometry (Line), Section property (Section or PB ID), start and end releases (not yet implemented in GsaAPI) and ID.
|![GsaElement2D@4x](https://user-images.githubusercontent.com/25223248/118825946-67838600-b8bb-11eb-94b8-fd4ddfb3db41.png)| **Element2D** | GSA 2D Element parameter. An Element2D contains geometry (Mesh), List of 2D Properties (Prop2Ds or PA IDs) and a List of IDs.
|![GsaElement3D@4x](https://user-images.githubusercontent.com/25223248/118825971-69e5e000-b8bb-11eb-82f8-adc2d1852365.png)| **Element3D** | GSA 3D Element parameter. An Element3D contains geometry (NgonMesh) and a List of IDs. 3D properties yet to be implemented.
|![GsaMem1D@4x](https://user-images.githubusercontent.com/25223248/118826044-736f4800-b8bb-11eb-8eb8-1c6cfe2a6eba.png)| **Member1D** | GSA 1D Member parameter. A Member1D contains geometry (Arc or PolyLine), Section property (Section or PB ID), start and end releases (not yet implemented in GsaAPI) and ID.
|![GsaMem2D@4x](https://user-images.githubusercontent.com/25223248/118826073-7702cf00-b8bb-11eb-9080-899d4125b487.png)| **Member2D** | GSA 2D Member parameter. A Member2D contains geometry (Planar BRep), 2D Property (Prop2D or PA ID), inclusion points and lines, and ID.
|![GsaMem3D@4x](https://user-images.githubusercontent.com/25223248/118826104-7a965600-b8bb-11eb-8713-b1924b9e19d5.png)| **Member3D** | GSA 3D Member parameter. A Member3D contains geometry (Closed Mesh) and ID. 3D properties yet to be implemented.
|![GsaLoad@4x](https://user-images.githubusercontent.com/25223248/118826124-7f5b0a00-b8bb-11eb-9177-b61ab22a940e.png)| **Load** | GSA Load parameter. A Load can contain any of the load types (Gravity, Node, Beam, Face, GridPoint, GridLine or GridArea) and a GridPlaneSurface if the instance is a Grid load type
|![GsaGridPlane@4x](https://user-images.githubusercontent.com/25223248/118826142-841fbe00-b8bb-11eb-8603-6adfed8a1333.png)| **GridPlaneSurface** | GSA Grid Plane Surface parameter. A GridPlaneSurface contains what is know in GSA as an Axis, GridPlane and GridSurface. 

<br/>

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.
<br/>

## License
This plugin is free to use, however in order to perform any meaningful tasks you must have GSA installed on your machine. For licensing of GSA refer to Oasys Software licensing terms: https://www.oasys-software.com/support/licensing-of-oasys-software/
