using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Components.GraveyardComp;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to edit a Node
  /// </summary>
  public class Elem2dFromBrep2_OBSOLETE : GH_OasysDropDownComponent, IGH_PreviewObject
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("83948408-c55d-49b9-b9a7-98034bcf3ce1");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateElemsFromBreps;

    public Elem2dFromBrep2_OBSOLETE() : base("Element2d from Brep",
      "Elem2dFromBrep",
      "Mesh a non-planar Brep",
      CategoryName.Name(),
      SubCategoryName.Cat2())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddBrepParameter("Brep", "B", "Brep (can be non-planar)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Incl. Points or Nodes", "(P)", "Inclusion points or Nodes", GH_ParamAccess.list);
      pManager.AddGenericParameter("Incl. Curves or 1D Members", "(C)", "Inclusion curves or 1D Members", GH_ParamAccess.list);
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

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaElement2dParameter(), "2D Elements", "E2D", "GSA 2D Elements", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_Brep ghbrep = new GH_Brep();
      if (DA.GetData(0, ref ghbrep))
      {
        if (ghbrep == null) { this.AddRuntimeWarning("Brep input is null"); }
        Brep brep = new Brep();
        if (GH_Convert.ToBrep(ghbrep, ref brep, GH_Conversion.Both))
        {
          // 1 Points
          List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
          List<Point3d> pts = new List<Point3d>();
          List<GsaNode> nodes = new List<GsaNode>();
          if (DA.GetDataList(1, gh_types))
          {
            for (int i = 0; i < gh_types.Count; i++)
            {
              Point3d pt = new Point3d();
              if (gh_types[i].Value is GsaNodeGoo)
              {
                GsaNode gsanode = new GsaNode();
                gh_types[i].CastTo(ref gsanode);
                nodes.Add(gsanode.Duplicate(true));
              }
              else if (GH_Convert.ToPoint3d(gh_types[i].Value, ref pt, GH_Conversion.Both))
              {
                pts.Add(new Point3d(pt));
              }
              else
              {
                string type = gh_types[i].Value.GetType().ToString();
                type = type.Replace("GsaGH.Parameters.", "");
                type = type.Replace("Goo", "");
                this.AddRuntimeError("Unable to convert incl. Point/Node input parameter of type " +
                    type + " to point or node");
              }
            }
          }

          // 2 Curves
          gh_types = new List<GH_ObjectWrapper>();
          List<Curve> crvs = new List<Curve>();
          List<GsaMember1d> mem1ds = new List<GsaMember1d>();
          if (DA.GetDataList(2, gh_types))
          {
            for (int i = 0; i < gh_types.Count; i++)
            {
              Curve crv = null;
              if (gh_types[i].Value is GsaMember1dGoo)
              {
                GsaMember1d gsamem1d = new GsaMember1d();
                gh_types[i].CastTo(ref gsamem1d);
                mem1ds.Add(gsamem1d.Duplicate(true));
              }
              else if (GH_Convert.ToCurve(gh_types[i].Value, ref crv, GH_Conversion.Both))
              {
                crvs.Add(crv.DuplicateCurve());
              }
              else
              {
                string type = gh_types[i].Value.GetType().ToString();
                type = type.Replace("GsaGH.Parameters.", "");
                type = type.Replace("Goo", "");
                this.AddRuntimeError("Unable to convert incl. Curve/Mem1D input parameter of type " +
                    type + " to curve or 1D Member");
              }
            }
          }

          // 4 mesh size
          Length meshSize = (Length)Input.UnitNumber(this, DA, 4, this.LengthUnit, true);

          // build new element2d with brep, crv and pts
          GsaElement2d elem2d = new GsaElement2d(brep, crvs, pts, meshSize.As(this.LengthUnit), mem1ds, nodes, this.LengthUnit, this.Tolerance);

          // 3 section
          GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
          GsaProp2d prop2d = new GsaProp2d();
          if (DA.GetData(3, ref gh_typ))
          {
            if (gh_typ.Value is GsaProp2dGoo)
              gh_typ.CastTo(ref prop2d);
            else
            {
              if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
                prop2d.Id = idd;
              else
              {
                this.AddRuntimeError("Unable to convert PA input to a 2D Property of reference integer");
                return;
              }
            }
          }
          else
            prop2d.Id = 0;
          List<GsaProp2d> prop2Ds = new List<GsaProp2d>();
          for (int i = 0; i < elem2d.API_Elements.Count; i++)
            prop2Ds.Add(prop2d);
          elem2d.Properties = prop2Ds;

          DA.SetData(0, new GsaElement2dGoo(elem2d, false));

          this.AddRuntimeRemarkMsg("This component is work-in-progress and provided 'as-is'. It will unroll the surface, do the meshing, map the mesh back on the original surface. Only single surfaces will work. Surfaces of high curvature and not-unrollable geometries (like a sphere) is unlikely to produce good results");
        }
      }
    }

    #region Custom UI
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;
    private Length Tolerance = DefaultUnits.Tolerance;
    private string _toleranceTxt = "";

    protected override void BeforeSolveInstance()
    {
      base.BeforeSolveInstance();
      this.UpdateMessage();
    }

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Unit"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Length
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      this.SelectedItems.Add(Length.GetAbbreviation(this.LengthUnit));

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[i]);
      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems()
    {
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      Params.Input[4].Name = "Mesh Size [" + Length.GetAbbreviation(this.LengthUnit) + "]";
    }
    public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
    {
      Menu_AppendSeparator(menu);

      ToolStripTextBox tolerance = new ToolStripTextBox();
      _toleranceTxt = Tolerance.ToString();
      tolerance.Text = _toleranceTxt;
      tolerance.BackColor = System.Drawing.Color.FromArgb(255, 180, 255, 150);
      tolerance.TextChanged += (s, e) => MaintainText(tolerance);

      ToolStripMenuItem toleranceMenu = new ToolStripMenuItem("Set Tolerance", Properties.Resources.Units);
      toleranceMenu.Enabled = true;
      toleranceMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;

      GH_MenuCustomControl menu2 = new GH_MenuCustomControl(toleranceMenu.DropDown, tolerance.Control, true, 200);
      toleranceMenu.DropDownItems[1].MouseUp += (s, e) =>
      {
        this.UpdateMessage();
        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
        ExpireSolution(true);
      };
      menu.Items.Add(toleranceMenu);

      Menu_AppendSeparator(menu);
    }
    private void MaintainText(ToolStripTextBox tolerance)
    {
      _toleranceTxt = tolerance.Text;
      if (Length.TryParse(_toleranceTxt, out Length res))
        tolerance.BackColor = System.Drawing.Color.FromArgb(255, 180, 255, 150);
      else
        tolerance.BackColor = System.Drawing.Color.FromArgb(255, 255, 100, 100);
    }
    private void UpdateMessage()
    {
      if (this._toleranceTxt != "")
      {
        try
        {
          Length newTolerance = Length.Parse(_toleranceTxt);
          Tolerance = newTolerance;
        }
        catch (Exception e)
        {
          MessageBox.Show(e.Message);
          return;
        }
      }
      this.Message = "Tol: " + Tolerance.ToString();
      if (Tolerance.Meters < 0.001)
        this.AddRuntimeRemarkMsg("Set tolerance is quite small, you can change this by right-clicking the component.");
      if (Tolerance.Meters > 0.25)
        this.AddRuntimeRemarkMsg("Set tolerance is quite large, you can change this by right-clicking the component.");
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      if (reader.ChunkExists("ParameterData"))
        return base.Read(reader);
      else
      {
        BaseReader.Read(reader, this);
        IsInitialised = true;
        UpdateUIFromSelectedItems();
        GH_IReader attributes = reader.FindChunk("Attributes");
        this.Attributes.Bounds = (System.Drawing.RectangleF)attributes.Items[0].InternalData;
        this.Attributes.Pivot = (System.Drawing.PointF)attributes.Items[1].InternalData;
        return true;
      }
    }
    #endregion
  }
}
