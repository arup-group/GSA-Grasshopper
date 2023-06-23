using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Components.GraveyardComp;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a Node
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class Elem2dFromBrep2_OBSOLETE : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("83948408-c55d-49b9-b9a7-98034bcf3ce1");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateElemsFromBreps;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    private Length _tolerance = DefaultUnits.Tolerance;
    private string _toleranceTxt = string.Empty;

    public Elem2dFromBrep2_OBSOLETE() : base("Element2d from Brep", "Elem2dFromBrep",
      "Mesh a non-planar Brep", CategoryName.Name(), SubCategoryName.Cat2()) { }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);

      var tolerance = new ToolStripTextBox {
        Text = _tolerance.ToString(),
        BackColor = Color.FromArgb(255, 180, 255, 150),
      };
      tolerance.TextChanged += (s, e) => MaintainText(tolerance);

      var toleranceMenu = new ToolStripMenuItem("Set Tolerance", Resources.Units) {
        Enabled = true,
        ImageScaling = ToolStripItemImageScaling.SizeToFit,
      };

      toleranceMenu.DropDownItems[1].MouseUp += (s, e) => {
        UpdateMessage();
        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
        ExpireSolution(true);
      };
      menu.Items.Add(toleranceMenu);

      Menu_AppendSeparator(menu);
    }

    public override bool Read(GH_IReader reader) {
      if (reader.ChunkExists("ParameterData")) {
        return base.Read(reader);
      }

      BaseReader.Read(reader, this);
      _isInitialised = true;
      UpdateUIFromSelectedItems();
      GH_IReader attributes = reader.FindChunk("Attributes");
      Attributes.Bounds = (RectangleF)attributes.Items[0].InternalData;
      Attributes.Pivot = (PointF)attributes.Items[1].InternalData;
      return true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      Params.Input[4].Name = "Mesh Size [" + Length.GetAbbreviation(_lengthUnit) + "]";
    }

    protected override void BeforeSolveInstance() {
      base.BeforeSolveInstance();
      UpdateMessage();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddBrepParameter("Brep", "B", "Brep (can be non-planar)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Incl. Points or Nodes", "(P)", "Inclusion points or Nodes",
        GH_ParamAccess.list);
      pManager.AddGenericParameter("Incl. Curves or 1D Members", "(C)",
        "Inclusion curves or 1D Members", GH_ParamAccess.list);
      pManager.AddParameter(new GsaProp2dParameter());
      pManager.AddGenericParameter("Mesh Size", "Ms", "Target mesh size", GH_ParamAccess.item);

      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
      pManager.HideParameter(0);
      pManager.HideParameter(1);
      pManager.HideParameter(2);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaElement2dParameter(), "2D Elements", "E2D", "GSA 2D Elements",
        GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var ghbrep = new GH_Brep();
      if (!da.GetData(0, ref ghbrep)) {
        return;
      }

      if (ghbrep == null) {
        this.AddRuntimeWarning("Brep input is null");
      }

      var brep = new Brep();
      if (!GH_Convert.ToBrep(ghbrep, ref brep, GH_Conversion.Both)) {
        return;
      }

      var ghTypes = new List<GH_ObjectWrapper>();
      var pts = new List<Point3d>();
      var nodes = new List<GsaNode>();
      if (da.GetDataList(1, ghTypes)) {
        foreach (GH_ObjectWrapper objectWrapper in ghTypes) {
          var pt = new Point3d();
          if (objectWrapper.Value is GsaNodeGoo nodeGoo) {
            nodes.Add(nodeGoo.Value.Clone());
          } else if (GH_Convert.ToPoint3d(objectWrapper.Value, ref pt, GH_Conversion.Both)) {
            pts.Add(new Point3d(pt));
          } else {
            string type = objectWrapper.Value.GetType().ToString();
            type = type.Replace("GsaGH.Parameters.", string.Empty);
            type = type.Replace("Goo", string.Empty);
            this.AddRuntimeError("Unable to convert incl. Point/Node input parameter of type "
              + type + " to point or node");
          }
        }
      }

      ghTypes = new List<GH_ObjectWrapper>();
      var curves = new List<Curve>();
      var member1ds = new List<GsaMember1d>();
      if (da.GetDataList(2, ghTypes)) {
        foreach (GH_ObjectWrapper objectWrapper in ghTypes) {
          Curve crv = null;
          if (objectWrapper.Value is GsaMember1dGoo member1dGoo) {
            member1ds.Add(member1dGoo.Value.Clone());
          } else if (GH_Convert.ToCurve(objectWrapper.Value, ref crv, GH_Conversion.Both)) {
            curves.Add(crv.DuplicateCurve());
          } else {
            string type = objectWrapper.Value.GetType().ToString();
            type = type.Replace("GsaGH.Parameters.", string.Empty);
            type = type.Replace("Goo", string.Empty);
            this.AddRuntimeError("Unable to convert incl. Curve/Mem1D input parameter of type "
              + type + " to curve or 1D Member");
          }
        }
      }

      var meshSize = (Length)Input.UnitNumber(this, da, 4, _lengthUnit, true);
      var elem2d = new GsaElement2d(brep, curves, pts, meshSize.As(_lengthUnit), member1ds, nodes,
        _lengthUnit, _tolerance);
      var ghTyp = new GH_ObjectWrapper();
      var prop2d = new GsaProp2d();
      if (da.GetData(3, ref ghTyp)) {
        if (ghTyp.Value is GsaProp2dGoo prop2DGoo) {
          prop2d = prop2DGoo.Value.Duplicate();
        } else {
          if (GH_Convert.ToInt32(ghTyp.Value, out int idd, GH_Conversion.Both)) {
            prop2d.Id = idd;
          } else {
            this.AddRuntimeError(
              "Unable to convert PA input to a 2D Property of reference integer");
            return;
          }
        }
      } else {
        prop2d.Id = 0;
      }

      var prop2Ds = new List<GsaProp2d>();
      for (int i = 0; i < elem2d.ApiElements.Count; i++) {
        prop2Ds.Add(prop2d);
      }

      elem2d.Prop2ds = prop2Ds;

      da.SetData(0, new GsaElement2dGoo(elem2d, false));

      this.AddRuntimeRemark(
        "This component is work-in-progress and provided 'as-is'. It will unroll the surface, do the meshing, map the mesh back on the original surface. Only single surfaces will work. Surfaces of high curvature and not-unrollable geometries (like a sphere) is unlikely to produce good results");
    }

    protected override void UpdateUIFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }

    private void MaintainText(ToolStripItem tolerance) {
      _toleranceTxt = tolerance.Text;
      tolerance.BackColor = Length.TryParse(_toleranceTxt, out Length _) ?
        Color.FromArgb(255, 180, 255, 150) : Color.FromArgb(255, 255, 100, 100);
    }

    private void UpdateMessage() {
      if (_toleranceTxt != string.Empty) {
        try {
          var newTolerance = Length.Parse(_toleranceTxt);
          _tolerance = newTolerance;
        }
        catch (Exception e) {
          MessageBox.Show(e.Message);
          return;
        }
      }

      Message = "Tol: " + _tolerance;
      if (_tolerance.Meters < 0.001) {
        this.AddRuntimeRemark(
          "Set tolerance is quite small, you can change this by right-clicking the component.");
      }

      if (_tolerance.Meters > 0.25) {
        this.AddRuntimeRemark(
          "Set tolerance is quite large, you can change this by right-clicking the component.");
      }
    }
  }
}
