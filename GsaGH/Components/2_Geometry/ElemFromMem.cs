using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper.GUI;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Grasshopper.Kernel.Parameters;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to edit a Node
  /// </summary>
  public class ElemFromMem : GH_OasysDropDownComponent, IGH_PreviewObject
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("3de73a08-b72c-45e4-a650-e4c6515266c5");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateElemsFromMems;

    public ElemFromMem() : base("Elements from Members",
      "ElemFromMem",
      "Create Elements from Members",
      CategoryName.Name(),
      SubCategoryName.Cat2())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

      pManager.AddGenericParameter("Nodes [" + unitAbbreviation + "]", "No", "Nodes to be included in meshing", GH_ParamAccess.list);
      pManager.AddGenericParameter("1D Members [" + unitAbbreviation + "]", "M1D", "1D Members to create 1D Elements from", GH_ParamAccess.list);
      pManager.AddGenericParameter("2D Members [" + unitAbbreviation + "]", "M2D", "2D Members to create 2D Elements from", GH_ParamAccess.list);
      pManager.AddGenericParameter("3D Members [" + unitAbbreviation + "]", "M3D", "3D Members to create 3D Elements from", GH_ParamAccess.list);

      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;

      pManager.HideParameter(0);
      pManager.HideParameter(1);
      pManager.HideParameter(2);
      pManager.HideParameter(3);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Nodes", "No", "GSA Nodes", GH_ParamAccess.list);
      pManager.HideParameter(0);
      pManager.AddGenericParameter("1D Elements", "E1D", "GSA 1D Elements", GH_ParamAccess.list);
      pManager.AddGenericParameter("2D Elements", "E2D", "GSA 2D Elements", GH_ParamAccess.list);
      pManager.AddGenericParameter("3D Elements", "E3D", "GSA 3D Elements", GH_ParamAccess.item);
      pManager.AddGenericParameter("GSA Model", "GSA", "GSA Model with Elements and Members", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      #region inputs
      // Get Member1d input
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();

      List<GsaNode> in_nodes = new List<GsaNode>();
      if (DA.GetDataList(0, gh_types))
      {
        for (int i = 0; i < gh_types.Count; i++)
        {
          gh_typ = gh_types[i];
          if (gh_typ == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Node input (index: " + i + ") is null and has been ignored"); continue; }

          if (gh_typ.Value is GsaNodeGoo)
          {
            GsaNode gsanode = new GsaNode();
            gh_typ.CastTo(ref gsanode);
            in_nodes.Add(gsanode);
          }
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in Node input");
            return;
          }
        }
      }

      List<GsaMember1d> in_mem1ds = new List<GsaMember1d>();
      if (DA.GetDataList(1, gh_types))
      {
        for (int i = 0; i < gh_types.Count; i++)
        {
          gh_typ = gh_types[i];
          if (gh_typ == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Member1D input (index: " + i + ") is null and has been ignored"); continue; }

          if (gh_typ.Value is GsaMember1dGoo)
          {
            GsaMember1d gsamem1 = new GsaMember1d();
            gh_typ.CastTo(ref gsamem1);
            in_mem1ds.Add(gsamem1);
          }
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in Mem1D input");
            return;
          }
        }
      }

      // Get Member2d input
      gh_types = new List<GH_ObjectWrapper>();
      List<GsaMember2d> in_mem2ds = new List<GsaMember2d>();
      if (DA.GetDataList(2, gh_types))
      {
        for (int i = 0; i < gh_types.Count; i++)
        {
          gh_typ = gh_types[i];
          if (gh_typ == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Member2D input (index: " + i + ") is null and has been ignored"); continue; }

          if (gh_typ.Value is GsaMember2dGoo)
          {
            GsaMember2d gsamem2 = new GsaMember2d();
            gh_typ.CastTo(ref gsamem2);
            in_mem2ds.Add(gsamem2);
          }
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in Mem2D input");
            return;
          }
        }
      }

      // Get Member3d input
      gh_types = new List<GH_ObjectWrapper>();
      List<GsaMember3d> in_mem3ds = new List<GsaMember3d>();
      if (DA.GetDataList(3, gh_types))
      {
        for (int i = 0; i < gh_types.Count; i++)
        {
          gh_typ = gh_types[i];
          if (gh_typ == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Member3D input (index: " + i + ") is null and has been ignored"); continue; }

          if (gh_typ.Value is GsaMember3dGoo)
          {
            GsaMember3d gsamem3 = new GsaMember3d();
            gh_typ.CastTo(ref gsamem3);
            in_mem3ds.Add(gsamem3);
          }
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in Mem3D input");
            return;
          }
        }
      }

      // manually add a warning if no input is set, as all three inputs are optional
      if (in_mem1ds.Count < 1 & in_mem2ds.Count < 1 & in_mem3ds.Count < 1)
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameters failed to collect data");
        return;
      }
      #endregion

      // Assemble model
      Model gsa = Helpers.Export.AssembleModel.Assemble(null, in_nodes, null, null, null, in_mem1ds, in_mem2ds, in_mem3ds, null, null, null, null, null, null, null, this.LengthUnit, this._tolerance, true, null);

      this.UpdateMessage();

      // extract nodes from model
      ConcurrentBag<GsaNodeGoo> nodes = Helpers.Import.Nodes.GetNodes(gsa.Nodes(), this.LengthUnit, null, false);

      
      // populate local axes dictionary
      ReadOnlyDictionary<int, Element> elementDict = gsa.Elements();
      Dictionary<int, ReadOnlyCollection<double>> elementLocalAxesDict = new Dictionary<int, ReadOnlyCollection<double>>();
      foreach (int id in elementDict.Keys)
        elementLocalAxesDict.Add(id, gsa.ElementDirectionCosine(id));

      // extract elements from model
      Tuple<ConcurrentBag<GsaElement1dGoo>, ConcurrentBag<GsaElement2dGoo>, ConcurrentBag<GsaElement3dGoo>> elementTuple
          = Helpers.Import.Elements.GetElements(elementDict, gsa.Nodes(), gsa.Sections(), gsa.Prop2Ds(), gsa.Prop3Ds(), gsa.AnalysisMaterials(), gsa.SectionModifiers(), elementLocalAxesDict, gsa.Axes(), this.LengthUnit, false);

      // expose internal model if anyone wants to use it
      GsaModel outModel = new GsaModel();
      outModel.Model = gsa;

      outModel.ModelUnit = this.LengthUnit;

      DA.SetDataList(0, nodes.OrderBy(item => item.Value.Id));
      DA.SetDataList(1, elementTuple.Item1.OrderBy(item => item.Value.Id));
      DA.SetDataList(2, elementTuple.Item2.OrderBy(item => item.Value.Ids.First()));
      DA.SetDataList(3, elementTuple.Item3.OrderBy(item => item.Value.Ids.First()));
      DA.SetData(4, new GsaModelGoo(outModel));

      // custom display settings for element2d mesh
      element2ds = elementTuple.Item2;
    }
    ConcurrentBag<GsaElement2dGoo> element2ds;
    public override void DrawViewportMeshes(IGH_PreviewArgs args)
    {

      base.DrawViewportMeshes(args);

      if (element2ds != null)
      {
        foreach (GsaElement2dGoo element in element2ds)
        {
          if (element == null) { continue; }
          //Draw shape.
          if (element.Value.Mesh != null)
          {
            if (!(element.Value.API_Elements[0].ParentMember.Member > 0)) // only draw mesh shading if no parent member exist.
            {
              if (this.Attributes.Selected)
                args.Display.DrawMeshShaded(element.Value.Mesh, Helpers.Graphics.Colours.Element2dFaceSelected);
              else
                args.Display.DrawMeshShaded(element.Value.Mesh, Helpers.Graphics.Colours.Element2dFace);
            }
          }
        }
      }
    }

    public override void DrawViewportWires(IGH_PreviewArgs args)
    {
      base.DrawViewportWires(args);

      if (element2ds != null)
      {
        foreach (GsaElement2dGoo element in element2ds)
        {
          if (element == null) { continue; }
          //Draw lines
          if (element.Value.Mesh != null)
          {
            if (element.Value.API_Elements[0].ParentMember.Member > 0) // only draw mesh shading if no parent member exist.
            {
              for (int i = 0; i < element.Value.Mesh.TopologyEdges.Count; i++)
              {
                if (element.Value.Mesh.TopologyEdges.GetConnectedFaces(i).Length > 1)
                  args.Display.DrawLine(element.Value.Mesh.TopologyEdges.EdgeLine(i), System.Drawing.Color.FromArgb(255, 229, 229, 229), 1);
              }
            }
            else
            {
              if (this.Attributes.Selected)
              {
                for (int i = 0; i < element.Value.Mesh.TopologyEdges.Count; i++)
                  args.Display.DrawLine(element.Value.Mesh.TopologyEdges.EdgeLine(i), Helpers.Graphics.Colours.Element2dEdgeSelected, 2);
              }
              else
              {
                for (int i = 0; i < element.Value.Mesh.TopologyEdges.Count; i++)
                  args.Display.DrawLine(element.Value.Mesh.TopologyEdges.EdgeLine(i), Helpers.Graphics.Colours.Element2dEdge, 1);
              }
            }
          }
        }
      }
    }

    #region Custom UI
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;
    private double _tolerance = DefaultUnits.Tolerance.Meters;
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
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

      int i = 0;
      Params.Input[i++].Name = "Nodes [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "1D Members [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "2D Members [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "3D Members [" + unitAbbreviation + "]";
    }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
    {
      Menu_AppendSeparator(menu);

      ToolStripTextBox tolerance = new ToolStripTextBox();
      _toleranceTxt = new Length(_tolerance, this.LengthUnit).ToString();
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

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
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
          _tolerance = newTolerance.Meters;
        }
        catch (Exception e)
        {
          MessageBox.Show(e.Message);
          return;
        }
      }
      Length tol = new Length(_tolerance, this.LengthUnit);
      this.Message = "Tol: " + tol.ToString();
      if (tol.Meters < 0.001)
        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Set tolerance is quite small, you can change this by right-clicking the component.");
      if (tol.Meters > 0.25)
        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Set tolerance is quite large, you can change this by right-clicking the component.");
    }
    #endregion

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetDouble("Tolerance", this._tolerance);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      if (reader.ItemExists("Tolerance"))
        this._tolerance = reader.GetDouble("Tolerance");
      else
        this._tolerance = DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry);
      this.UpdateMessage();
      return base.Read(reader);
    }
    #endregion
  }
}

