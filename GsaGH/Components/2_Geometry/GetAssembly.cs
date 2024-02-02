﻿using System;
using System.Drawing;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Components {
  public class GetAssembly : GH_OasysComponent, IGH_VariableParameterComponent {
    public override Guid ComponentGuid => new Guid("c54ffe55-aaa7-4edc-999a-c48f6990d254");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GetAssembly;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;

    public GetAssembly() : base("Get Assembly", "GetAs", "Get GSA Assembly", CategoryName.Name()
      , SubCategoryName.Cat2()) {
      Hidden = true;
    }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);

      var lengthUnitsMenu = new ToolStripMenuItem("Length") {
        Enabled = true,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => UpdateLength(unit)) {
          Checked = unit == Length.GetAbbreviation(_lengthUnit),
          Enabled = true,
        };
        lengthUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      var unitsMenu = new ToolStripMenuItem("Select Units", Resources.ModelUnits);
      unitsMenu.DropDownItems.AddRange(new ToolStripItem[] {
        lengthUnitsMenu
      });
      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;

      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index) {
      return false;
    }

    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index) {
      return false;
    }

    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index) {
      return null;
    }

    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) {
      return false;
    }

    public override bool Read(GH_IReader reader) {
      _lengthUnit
        = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("LengthUnit"));
      return base.Read(reader);
    }

    public virtual void VariableParameterMaintenance() {
      string lengthUnitAbr = Length.GetAbbreviation(_lengthUnit);

      Params.Output[0].Name = "Name";
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetString("LengthUnit", _lengthUnit.ToString());
      return base.Write(writer);
    }

    protected override void BeforeSolveInstance() {
      UpdateMessage();
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaAssemblyParameter(), GsaAssemblyGoo.Name,
        GsaAssemblyGoo.NickName, GsaAssemblyGoo.Description + " to get information for." + GsaAssemblyGoo.Name, GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string lengthUnitAbr = Length.GetAbbreviation(_lengthUnit);

      pManager.AddTextParameter("Name", "Na", "Assembly Name", GH_ParamAccess.item);
      pManager.AddTextParameter("Assembly type", "aT", "Assembly type", GH_ParamAccess.item);
      pManager.AddGenericParameter("List", "El", "Assembly Entities", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Topology 1", "To1", "Node at the start of the Assembly", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Topology 2", "To2", "Node at the end of the Assembly", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Orientation Node", "⭮N", "Assembly Orientation Node", GH_ParamAccess.item);
      pManager.AddGenericParameter("Extents y [" + lengthUnitAbr + "]", "Ey", "Extents of the Assembly in y-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Extents z [" + lengthUnitAbr + "]", "Ez", "Extents of the Assembly in z-direction", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Internal Topology", "IT", " List of nodes that define the curve of the Assembly", GH_ParamAccess.list);
      pManager.AddIntegerParameter("Curve Fit", "CF", "Curve Fit for curved elements" + $"{Environment.NewLine}Lagrange Interpolation (2) or Circular Arc (1)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Definition", "D", "Assembly definition", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaAssemblyGoo assemblyGoo = null;
      if (!da.GetData(0, ref assemblyGoo)) {
        this.AddRuntimeWarning("Input As failed to collect data");
        return;
      }
      GsaAssembly assembly = assemblyGoo.Value;

      var entities = new GsaList("", assembly.ApiAssembly.EntityList, assembly.ApiAssembly.EntityType);
      da.SetData(0, assembly.ApiAssembly.Name);
      da.SetData(2, new GsaListGoo(entities));
      da.SetData(3, assembly.ApiAssembly.Topology1);
      da.SetData(4, assembly.ApiAssembly.Topology2);
      da.SetData(5, assembly.ApiAssembly.OrientationNode);
      da.SetData(6, new Length(assembly.ApiAssembly.ExtentY, LengthUnit.Meter).ToUnit(_lengthUnit));
      da.SetData(7, new Length(assembly.ApiAssembly.ExtentZ, LengthUnit.Meter).ToUnit(_lengthUnit));

      switch (assembly.ApiAssembly) {
        case AssemblyByExplicitPositions byExplicitPositions:
          da.SetData(1, "By explicit positions");
          da.SetDataList(8, byExplicitPositions.InternalTopology);
          da.SetData(9, byExplicitPositions.CurveFit);
          da.SetDataList(10, byExplicitPositions.Positions);
          break;

        case AssemblyByNumberOfPoints byNumberOfPoints:
          da.SetData(1, "By number of points");
          da.SetDataList(8, byNumberOfPoints.InternalTopology);
          da.SetData(9, byNumberOfPoints.CurveFit);
          da.SetData(10, byNumberOfPoints.NumberOfPoints);
          break;

        case AssemblyBySpacingOfPoints bySpacingOfPoints:
          da.SetData(1, "By spacing of points");
          da.SetDataList(8, bySpacingOfPoints.InternalTopology);
          da.SetData(9, bySpacingOfPoints.CurveFit);
          da.SetData(10, new Length(bySpacingOfPoints.Spacing, LengthUnit.Meter).ToUnit(_lengthUnit));
          break;

        case AssemblyByStorey byStorey:
          da.SetData(1, "By storey");
          da.SetData(10, byStorey.StoreyList);
          break;
      }
    }

    internal void UpdateLength(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      Update();
    }

    private void Update() {
      UpdateMessage();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }

    private void UpdateMessage() {
      Message = Length.GetAbbreviation(_lengthUnit);
    }
  }
}
