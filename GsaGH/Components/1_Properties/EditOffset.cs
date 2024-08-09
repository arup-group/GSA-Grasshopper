using System;
using System.Drawing;
using System.Windows.Forms;

using GH_IO.Serialization;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit an Offset and ouput the information
  /// </summary>
  public class EditOffset : GH_OasysComponent, IGH_VariableParameterComponent {
    public override Guid ComponentGuid => new Guid("dd2b4e77-c1c7-4a0e-9d12-fe7a8982f9ea");
    public override GH_Exposure Exposure => GH_Exposure.septenary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditOffset;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;

    public EditOffset() : base("Edit Offset", "EditOff",
      "Modify GSA Offset or just get information about existing", CategoryName.Name(),
      SubCategoryName.Cat1()) {
      Hidden = true;
    }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);

      var unitsMenu = new ToolStripMenuItem("Select unit", Resources.ModelUnits) {
        Enabled = true,
        ImageScaling = ToolStripItemImageScaling.SizeToFit,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => Update(unit)) {
          Checked = unit == Length.GetAbbreviation(_lengthUnit),
          Enabled = true,
        };
        unitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

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
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      Params.Input[1].Name = "X1 [" + unitAbbreviation + "]";
      Params.Input[2].Name = "X2 [" + unitAbbreviation + "]";
      Params.Input[3].Name = "Y [" + unitAbbreviation + "]";
      Params.Input[4].Name = "Z [" + unitAbbreviation + "]";
      Params.Output[1].Name = "X1 [" + unitAbbreviation + "]";
      Params.Output[2].Name = "X2 [" + unitAbbreviation + "]";
      Params.Output[3].Name = "Y [" + unitAbbreviation + "]";
      Params.Output[4].Name = "Z [" + unitAbbreviation + "]";
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetString("LengthUnit", _lengthUnit.ToString());
      return base.Write(writer);
    }

    protected override void BeforeSolveInstance() {
      Message = Length.GetAbbreviation(_lengthUnit);
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      pManager.AddParameter(new GsaOffsetParameter(), GsaOffsetGoo.Name, GsaOffsetGoo.NickName,
        GsaOffsetGoo.Description + " to get or set information for. Leave blank to create a new "
        + GsaOffsetGoo.Name, GH_ParamAccess.item);
      pManager.AddGenericParameter("X1 [" + unitAbbreviation + "]", "X1",
        "`X1` - Start axial offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("X2 [" + unitAbbreviation + "]", "X2",
        "`X2` - End axial offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Y [" + unitAbbreviation + "]", "Y", "`Y` Offset",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Z [" + unitAbbreviation + "]", "Z", "`Z` Offset",
        GH_ParamAccess.item);
      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      pManager.AddParameter(new GsaOffsetParameter(), GsaOffsetGoo.Name, GsaOffsetGoo.NickName,
        GsaOffsetGoo.Description + " with applied changes.", GH_ParamAccess.item);
      pManager.AddGenericParameter("X1 [" + unitAbbreviation + "]", "X1",
        "`X1` - Start axial offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("X2 [" + unitAbbreviation + "]", "X2",
        "`X2` - End axial offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Y [" + unitAbbreviation + "]", "Y", "`Y` Offset",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset Z [" + unitAbbreviation + "]", "Z", "`Z` Offset",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var offset = new GsaOffset();

      GsaOffsetGoo offsetGoo = null;
      if (da.GetData(0, ref offsetGoo)) {
        offset = offsetGoo.Value.Duplicate();
      }

      int inp = 1;
      if (Params.Input[inp].SourceCount != 0) {
        offset.X1 = (Length)Input.UnitNumber(this, da, inp++, _lengthUnit, true);
      }

      if (Params.Input[inp].SourceCount != 0) {
        offset.X2 = (Length)Input.UnitNumber(this, da, inp++, _lengthUnit, true);
      }

      if (Params.Input[inp].SourceCount != 0) {
        offset.Y = (Length)Input.UnitNumber(this, da, inp++, _lengthUnit, true);
      }

      if (Params.Input[inp].SourceCount != 0) {
        offset.Z = (Length)Input.UnitNumber(this, da, inp, _lengthUnit, true);
      }

      int outp = 0;
      da.SetData(outp++, new GsaOffsetGoo(offset));
      da.SetData(outp++, new GH_UnitNumber(offset.X1.ToUnit(_lengthUnit)));
      da.SetData(outp++, new GH_UnitNumber(offset.X2.ToUnit(_lengthUnit)));
      da.SetData(outp++, new GH_UnitNumber(offset.Y.ToUnit(_lengthUnit)));
      da.SetData(outp, new GH_UnitNumber(offset.Z.ToUnit(_lengthUnit)));
    }

    private void Update(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      Message = unit;
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }
  }
}
