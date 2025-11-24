using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using GH_IO.Serialization;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;

using Rhino.Collections;
using Rhino.Geometry;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a Node
  /// </summary>
  public class Create2dElementsFromBrep : GH_OasysDropDownComponent {
    public const string DoesNotSupportLoadPanelErrorMessage = "This component does not support creating a load panel";
    public override Guid ComponentGuid => new Guid("18c5913e-cbce-42e8-8563-18e28b079d34");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Create2dElementsFromBrep;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    internal ToleranceContextMenu ToleranceMenu { get; set; } = new ToleranceContextMenu();
    private readonly List<string> _meshMode = new List<string>() {
      "Tri-6 only",
      "Planar Quads",
      "Quad-8 only"
    };

    public Create2dElementsFromBrep() : base("Create 2D Elements from Brep", "Elem2dFromBrep", "Mesh a non-planar Brep",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      ToleranceMenu.AppendAdditionalMenuItems(this, menu, _lengthUnit);
    }

    public override bool Read(GH_IReader reader) {
      bool flag = base.Read(reader);
      _isInitialised = true;
      UpdateUIFromSelectedItems();
      GH_IReader attributes = reader.FindChunk("Attributes");
      Attributes.Bounds = (RectangleF)attributes.Items[0].InternalData;
      Attributes.Pivot = (PointF)attributes.Items[1].InternalData;
      return flag;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      if (i == 1) {
        _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
      }

      ToleranceMenu.UpdateMessage(this, _lengthUnit);
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      Params.Input[4].Name = "Mesh Size [" + Length.GetAbbreviation(_lengthUnit) + "]";
    }

    protected override void BeforeSolveInstance() {
      base.BeforeSolveInstance();
      ToleranceMenu.UpdateMessage(this, _lengthUnit);
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Mesh mode",
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(_meshMode);
      _selectedItems.Add(_meshMode[0]); // tri only

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddBrepParameter("Brep", "B", "Brep (can be non-planar)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Incl. Points or Nodes", "(P)", "Inclusion points or Nodes", GH_ParamAccess.list);
      pManager.AddGenericParameter("Incl. Curves or 1D Members", "(C)", "Inclusion curves or 1D Members",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaProperty2dParameter());
      pManager.AddGenericParameter("Mesh Size", "Ms", "Target mesh size", GH_ParamAccess.item);

      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaElement2dParameter(), "2D Elements", "E2D", "GSA 2D Elements", GH_ParamAccess.item);
      pManager.AddParameter(new GsaNodeParameter(), "Incl. Nodes", "No", "Inclusion Nodes", GH_ParamAccess.list);
      pManager.AddParameter(new GsaElement1dParameter(), "Incl. Element1Ds", "E1D",
        "Inclusion 1D Elements created from 1D Members", GH_ParamAccess.list);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var ghbrep = new GH_Brep();
      da.GetData(0, ref ghbrep);

      if (ghbrep == null || ghbrep.Value == null) {
        this.AddRuntimeError("Brep input is null");
        return;
      }

      var brep = new Brep();
      GH_Convert.ToBrep(ghbrep, ref brep, GH_Conversion.Both);

      if (brep.Surfaces.Count > 1) {
        brep.Faces.ShrinkFaces();
      }

      if (brep.Surfaces[0].IsPlanar()) {
        this.AddRuntimeRemark(
          "Input Surface is planar. You may want to use Member2D component as this component is intended to help mesh non-planar Breps and provides less functionality than Member2D.");
      }

      var ghTypes = new List<GH_ObjectWrapper>();
      var pts = new Point3dList();
      var nodes = new List<GsaNode>();
      if (da.GetDataList(1, ghTypes)) {
        foreach (GH_ObjectWrapper ghObjectWrapper in ghTypes) {
          var pt = new Point3d();
          if (ghObjectWrapper.Value is GsaNodeGoo nodeGoo) {
            nodes.Add(new GsaNode(nodeGoo.Value));
          } else if (GH_Convert.ToPoint3d(ghObjectWrapper.Value, ref pt, GH_Conversion.Both)) {
            pts.Add(new Point3d(pt));
          } else {
            string type = ghObjectWrapper.Value.GetType().ToString();
            type = type.Replace("GsaGH.Parameters.", string.Empty);
            type = type.Replace("Goo", string.Empty);
            this.AddRuntimeError("Unable to convert incl. Point/Node input parameter of type " + type
              + " to point or node");
          }
        }
      }

      ghTypes = new List<GH_ObjectWrapper>();
      var crvs = new List<Curve>();
      var elem1ds = new List<GsaElement1D>();
      var mem1ds = new List<GsaMember1D>();
      if (da.GetDataList(2, ghTypes)) {
        foreach (GH_ObjectWrapper ghType in ghTypes) {
          Curve crv = null;
          switch (ghType.Value) {
            case GsaElement1dGoo element1DGoo: {
              elem1ds.Add(new GsaElement1D(element1DGoo.Value));
              break;
            }
            case GsaMember1dGoo member1DGoo: {
              mem1ds.Add(new GsaMember1D(member1DGoo.Value));
              break;
            }
            default: {
              if (GH_Convert.ToCurve(ghType.Value, ref crv, GH_Conversion.Both)) {
                crvs.Add(crv.DuplicateCurve());
              } else {
                this.AddRuntimeError($"Unable to convert incl. Curve/Mem1D input parameter of type "
                  + $"{ghType.GetTypeName()} to curve or 1D Member");
              }

              break;
            }
          }
        }
      }

      var ghTyp = new GH_ObjectWrapper();
      var prop2d = new GsaProperty2d();
      if (da.GetData(3, ref ghTyp)) {
        if (ghTyp.Value is GsaProperty2dGoo prop2DGoo) {
          prop2d = prop2DGoo.Value;
        } else {
          if (GH_Convert.ToInt32(ghTyp.Value, out int idd, GH_Conversion.Both)) {
            prop2d.Id = idd;
          } else {
            this.AddRuntimeError("Unable to convert PA input to a 2D Property of reference integer");
            return;
          }
        }
      } else {
        prop2d.Id = 0;
      }

      var meshSize = (Length)Input.UnitNumber(this, da, 4, _lengthUnit, true);
      bool isLoadPanel = (prop2d.ApiProp2d != null) && prop2d.ApiProp2d.Type == Property2D_Type.LOAD;
      if (isLoadPanel) {
        this.AddRuntimeError(DoesNotSupportLoadPanelErrorMessage);
        return;
      }

      Tuple<GsaElement2D, List<GsaNode>, List<GsaElement1D>> tuple = GetElement2dFromBrep(brep, pts, nodes, crvs,
        elem1ds, mem1ds, meshSize.As(_lengthUnit), _lengthUnit, ToleranceMenu.Tolerance);
      GsaElement2D elem2d = tuple.Item1;

      var prop2Ds = new List<GsaProperty2d>();
      for (int i = 0; i < elem2d.ApiElements.Count; i++) {
        prop2Ds.Add(prop2d);
      }

      elem2d.Prop2ds = prop2Ds;

      da.SetData(0, new GsaElement2dGoo(elem2d));
      if (tuple.Item2 != null) {
        da.SetDataList(1, new List<GsaNodeGoo>(tuple.Item2.Select(n => new GsaNodeGoo(n))));
      }

      if (tuple.Item3 != null) {
        da.SetDataList(2, new List<GsaElement1dGoo>(tuple.Item3.Select(elem => new GsaElement1dGoo(elem))));
      }

      this.AddRuntimeRemark(
        "This component is work-in-progress and provided 'as-is'. It will unroll the surface, do the meshing, map the mesh back on the original surface. Only single surfaces will work. Surfaces of high curvature and not-unrollable geometries (like a sphere) are unlikely to produce good results");
    }

    protected override void UpdateUIFromSelectedItems() {
      int i = _selectedItems.Count == 1 ? 0 : 1;
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

      if (_selectedItems.Count == 1) {
        _dropDownItems.Insert(0, _meshMode);
        _selectedItems.Insert(0, _meshMode[1]);

        _spacerDescriptions.Insert(0, "Mesh mode");
      }

      base.UpdateUIFromSelectedItems();
    }

    private Tuple<GsaElement2D, List<GsaNode>, List<GsaElement1D>> GetElement2dFromBrep(
      Brep brep, Point3dList points, List<GsaNode> nodes, List<Curve> curves, List<GsaElement1D> elem1ds,
      List<GsaMember1D> mem1ds, double meshSize, LengthUnit unit, Length tolerance) {
      var gsaElement2D = new GsaElement2D();

      MeshMode2d meshMode2d = MeshMode2d.Tri;
      if (_selectedItems[0] == _meshMode[1]) {
        meshMode2d = MeshMode2d.Mixed;
      }

      if (_selectedItems[0] == _meshMode[2]) {
        meshMode2d = MeshMode2d.Quad;
      }

      Tuple<Mesh, List<GsaNode>, List<GsaElement1D>> tuple = RhinoConversions.ConvertBrepToMesh(brep, points, nodes,
        curves, elem1ds, mem1ds, meshSize, unit, tolerance, meshMode2d);
      gsaElement2D.Mesh = tuple.Item1;
      Tuple<List<GSAElement>, Point3dList, List<List<int>>> convertMesh
        = RhinoConversions.ConvertMeshToElem2d(gsaElement2D.Mesh, true);
      gsaElement2D.ApiElements = convertMesh.Item1;
      gsaElement2D.Topology = convertMesh.Item2;
      gsaElement2D.TopoInt = convertMesh.Item3;
      gsaElement2D.Ids = new List<int>(new int[gsaElement2D.Mesh.Faces.Count]);
      return new Tuple<GsaElement2D, List<GsaNode>, List<GsaElement1D>>(gsaElement2D, tuple.Item2, tuple.Item3);
    }
  }
}
