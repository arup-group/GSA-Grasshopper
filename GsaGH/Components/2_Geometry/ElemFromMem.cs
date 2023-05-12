﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.Export;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.Graphics;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Elements = GsaGH.Helpers.Import.Elements;
using Nodes = GsaGH.Helpers.Import.Nodes;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a Node
  /// </summary>
  public class ElemFromMem : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("3de73a08-b72c-45e4-a650-e4c6515266c5");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateElemsFromMems;
    private ConcurrentBag<GsaElement2dGoo> _element2ds;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    private Length _tolerance = DefaultUnits.Tolerance;
    private string _toleranceTxt = string.Empty;

    public ElemFromMem() : base("Elements from Members", "ElemFromMem",
      "Create Elements from Members", CategoryName.Name(), SubCategoryName.Cat2()) { }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }
      
      Menu_AppendSeparator(menu);

      var tolerance = new ToolStripTextBox();
      _toleranceTxt = _tolerance.ToUnit(_lengthUnit).ToString().Replace(" ", string.Empty);
      tolerance.Text = _toleranceTxt;
      tolerance.BackColor = Color.FromArgb(255, 180, 255, 150);
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

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }

    public override void DrawViewportMeshes(IGH_PreviewArgs args) {
      base.DrawViewportMeshes(args);

      if (_element2ds == null) {
        return;
      }

      foreach (GsaElement2dGoo element in _element2ds) {
        if (element?.Value.Mesh == null || element.Value.ApiElements[0].ParentMember.Member > 0) {
          continue;
        }

        args.Display.DrawMeshShaded(element.Value.Mesh,
          Attributes.Selected ? Colours.Element2dFaceSelected : Colours.Element2dFace);
      }
    }

    public override void DrawViewportWires(IGH_PreviewArgs args) {
      base.DrawViewportWires(args);

      if (_element2ds == null) {
        return;
      }

      foreach (GsaElement2dGoo element in _element2ds) {
        if (element == null || element.Value.Mesh == null) {
          continue;
        }

        if (element.Value.ApiElements[0].ParentMember.Member
          > 0) // only draw mesh shading if no parent member exist.
        {
          for (int i = 0; i < element.Value.Mesh.TopologyEdges.Count; i++) {
            if (element.Value.Mesh.TopologyEdges.GetConnectedFaces(i).Length > 1) {
              args.Display.DrawLine(element.Value.Mesh.TopologyEdges.EdgeLine(i),
                Color.FromArgb(255, 229, 229, 229), 1);
            }
          }
        } else {
          if (Attributes.Selected) {
            for (int i = 0; i < element.Value.Mesh.TopologyEdges.Count; i++) {
              args.Display.DrawLine(element.Value.Mesh.TopologyEdges.EdgeLine(i),
                Colours.Element2dEdgeSelected, 2);
            }
          } else {
            for (int i = 0; i < element.Value.Mesh.TopologyEdges.Count; i++) {
              args.Display.DrawLine(element.Value.Mesh.TopologyEdges.EdgeLine(i),
                Colours.Element2dEdge, 1);
            }
          }
        }
      }
    }

    public override bool Read(GH_IReader reader) {
      bool flag = base.Read(reader);
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);
      if (reader.ItemExists("Tolerance")) {
        double tol = reader.GetDouble("Tolerance");
        _tolerance = new Length(tol, _lengthUnit);
      } else {
        _tolerance = DefaultUnits.Tolerance;
      }

      UpdateMessage();
      return flag;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
      UpdateMessage();
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      int i = 0;
      Params.Input[i++].Name = "Nodes [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "1D Members [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "2D Members [" + unitAbbreviation + "]";
      Params.Input[i].Name = "3D Members [" + unitAbbreviation + "]";
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetDouble("Tolerance", _tolerance.Value);
      return base.Write(writer);
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
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      pManager.AddGenericParameter("Nodes [" + unitAbbreviation + "]", "No",
        "Nodes to be included in meshing", GH_ParamAccess.list);
      pManager.AddGenericParameter("1D Members [" + unitAbbreviation + "]", "M1D",
        "1D Members to create 1D Elements from", GH_ParamAccess.list);
      pManager.AddGenericParameter("2D Members [" + unitAbbreviation + "]", "M2D",
        "2D Members to create 2D Elements from", GH_ParamAccess.list);
      pManager.AddGenericParameter("3D Members [" + unitAbbreviation + "]", "M3D",
        "3D Members to create 3D Elements from", GH_ParamAccess.list);

      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;

      pManager.HideParameter(0);
      pManager.HideParameter(1);
      pManager.HideParameter(2);
      pManager.HideParameter(3);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("Nodes", "No", "GSA Nodes", GH_ParamAccess.list);
      pManager.HideParameter(0);
      pManager.AddGenericParameter("1D Elements", "E1D", "GSA 1D Elements", GH_ParamAccess.list);
      pManager.AddGenericParameter("2D Elements", "E2D", "GSA 2D Elements", GH_ParamAccess.list);
      pManager.AddGenericParameter("3D Elements", "E3D", "GSA 3D Elements", GH_ParamAccess.item);
      pManager.AddGenericParameter("GSA Model", "GSA", "GSA Model with Elements and Members",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      #region inputs

      var ghTyp = new GH_ObjectWrapper();
      var ghTypes = new List<GH_ObjectWrapper>();

      var inNodes = new List<GsaNode>();
      if (da.GetDataList(0, ghTypes)) {
        for (int i = 0; i < ghTypes.Count; i++) {
          ghTyp = ghTypes[i];
          if (ghTyp == null) {
            Params.Owner.AddRuntimeWarning("Node input (index: " + i
              + ") is null and has been ignored");
            continue;
          }

          if (ghTyp.Value is GsaNodeGoo) {
            var gsanode = new GsaNode();
            ghTyp.CastTo(ref gsanode);
            inNodes.Add(gsanode);
          } else {
            this.AddRuntimeError("Error in Node input");
            return;
          }
        }
      }

      var inMem1ds = new List<GsaMember1d>();
      if (da.GetDataList(1, ghTypes)) {
        for (int i = 0; i < ghTypes.Count; i++) {
          ghTyp = ghTypes[i];
          if (ghTyp == null) {
            Params.Owner.AddRuntimeWarning("Member1D input (index: " + i
              + ") is null and has been ignored");
            continue;
          }

          if (ghTyp.Value is GsaMember1dGoo) {
            var gsamem1 = new GsaMember1d();
            ghTyp.CastTo(ref gsamem1);
            inMem1ds.Add(gsamem1);
          } else {
            this.AddRuntimeError("Error in Mem1D input");
            return;
          }
        }
      }

      ghTypes = new List<GH_ObjectWrapper>();
      var inMem2ds = new List<GsaMember2d>();
      if (da.GetDataList(2, ghTypes)) {
        for (int i = 0; i < ghTypes.Count; i++) {
          ghTyp = ghTypes[i];
          if (ghTyp == null) {
            Params.Owner.AddRuntimeWarning("Member2D input (index: " + i
              + ") is null and has been ignored");
            continue;
          }

          if (ghTyp.Value is GsaMember2dGoo) {
            var gsamem2 = new GsaMember2d();
            ghTyp.CastTo(ref gsamem2);
            inMem2ds.Add(gsamem2);
          } else {
            this.AddRuntimeError("Error in Mem2D input");
            return;
          }
        }
      }

      ghTypes = new List<GH_ObjectWrapper>();
      var inMem3ds = new List<GsaMember3d>();
      if (da.GetDataList(3, ghTypes)) {
        for (int i = 0; i < ghTypes.Count; i++) {
          ghTyp = ghTypes[i];
          if (ghTyp == null) {
            Params.Owner.AddRuntimeWarning("Member3D input (index: " + i
              + ") is null and has been ignored");
            continue;
          }

          if (ghTyp.Value is GsaMember3dGoo) {
            var gsamem3 = new GsaMember3d();
            ghTyp.CastTo(ref gsamem3);
            inMem3ds.Add(gsamem3);
          } else {
            this.AddRuntimeError("Error in Mem3D input");
            return;
          }
        }
      }

      // manually add a warning if no input is set, as all three inputs are optional
      if ((inMem1ds.Count < 1) & (inMem2ds.Count < 1) & (inMem3ds.Count < 1)) {
        this.AddRuntimeWarning("Input parameters failed to collect data");
        return;
      }

      #endregion

      Model gsa = AssembleModel.Assemble(null, null, inNodes, null, null, null, inMem1ds, inMem2ds,
        inMem3ds, null, null, null, null, null, null, null, _lengthUnit, _tolerance, true, this);

      UpdateMessage();

      ConcurrentBag<GsaNodeGoo> nodes = Nodes.GetNodes(gsa.Nodes(), _lengthUnit);

      ReadOnlyDictionary<int, Element> elementDict = gsa.Elements();
      var elementLocalAxesDict
        = elementDict.Keys.ToDictionary(id => id, id => gsa.ElementDirectionCosine(id));

      (ConcurrentBag<GsaElement1dGoo> e1d, ConcurrentBag<GsaElement2dGoo> e2d,
      ConcurrentBag<GsaElement3dGoo> e3d) = Elements.GetElements(elementDict,
        gsa.Nodes(), gsa.Sections(), gsa.Prop2Ds(), gsa.Prop3Ds(), gsa.AnalysisMaterials(),
        gsa.SectionModifiers(), elementLocalAxesDict, gsa.Axes(), _lengthUnit, false);

      var outModel = new GsaModel {
        Model = gsa,
        ModelUnit = _lengthUnit,
      };

      da.SetDataList(0, nodes.OrderBy(item => item.Value.Id));
      da.SetDataList(1, e1d.OrderBy(item => item.Value.Id));
      da.SetDataList(2, e2d.OrderBy(item => item.Value.Ids.First()));
      da.SetDataList(3, e3d.OrderBy(item => item.Value.Ids.First()));
      da.SetData(4, new GsaModelGoo(outModel));

      _element2ds = e2d;
    }

    private void MaintainText(ToolStripTextBox tolerance) {
      _toleranceTxt = tolerance.Text;
      tolerance.BackColor = Length.TryParse(_toleranceTxt, out Length _) ?
        Color.FromArgb(255, 180, 255, 150) : Color.FromArgb(255, 255, 100, 100);
    }

    private void UpdateMessage() {
      if (_toleranceTxt != string.Empty) {
        try {
          _tolerance = Length.Parse(_toleranceTxt);
        } catch (Exception e) {
          MessageBox.Show(e.Message);
          return;
        }
      }

      _tolerance = _tolerance.ToUnit(_lengthUnit);
      Message = "Tol: " + _tolerance.ToString().Replace(" ", string.Empty);
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
