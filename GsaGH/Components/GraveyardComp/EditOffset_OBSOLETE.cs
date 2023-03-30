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
  // ReSharper disable once InconsistentNaming
  public class EditOffset_OBSOLETE : GH_OasysComponent {

    #region Properties + Fields
    public override Guid ComponentGuid => new Guid("1e094fcd-8f5f-4047-983c-e0e57a83ae52");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditOffset;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;
    #endregion Properties + Fields

    #region Public Constructors
    public EditOffset_OBSOLETE() : base("Edit Offset",
      "OffsetEdit",
      "Modify GSA Offset or just get information about existing",
      CategoryName.Name(),
      SubCategoryName.Cat1())
      => Hidden = true;

    #endregion Public Constructors

    #region Public Methods
    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      Menu_AppendSeparator(menu);

      var unitsMenu = new ToolStripMenuItem("Select unit", Resources.Units) {
        Enabled = true,
        ImageScaling = ToolStripItemImageScaling.SizeToFit,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { Update(unit); }) {
          Checked = unit == Length.GetAbbreviation(_lengthUnit),
          Enabled = true,
        };
        unitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }

    public override bool Read(GH_IReader reader) {
      if (reader.ItemExists("LengthUnit")) {
        _lengthUnit
          = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("LengthUnit"));
        bool flag = base.Read(reader);
        return flag & Params.ReadAllParameterData(reader);
      }
      else {
        _lengthUnit = DefaultUnits.LengthUnitSection;
        return base.Read(reader);
      }
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetString("LengthUnit", _lengthUnit.ToString());
      return base.Write(writer);
    }

    #endregion Public Methods

    #region Protected Methods
    protected override void BeforeSolveInstance() => Message = Length.GetAbbreviation(_lengthUnit);

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      pManager.AddParameter(new GsaOffsetParameter(),
        GsaOffsetGoo.Name,
        GsaOffsetGoo.NickName,
        GsaOffsetGoo.Description
        + " to get or set information for. Leave blank to create a new "
        + GsaOffsetGoo.Name,
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset X1 [" + unitAbbreviation + "]",
        "X1",
        "X1 - Start axial offset",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset X2 [" + unitAbbreviation + "]",
        "X2",
        "X2 - End axial offset",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset Y [" + unitAbbreviation + "]",
        "Y",
        "Y Offset",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset Z [" + unitAbbreviation + "]",
        "Z",
        "Z Offset",
        GH_ParamAccess.item);
      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i]
          .Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      pManager.AddParameter(new GsaOffsetParameter(),
        GsaOffsetGoo.Name,
        GsaOffsetGoo.NickName,
        GsaOffsetGoo.Description + " with applied changes.",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset X1 [" + unitAbbreviation + "]",
        "X1",
        "X1 - Start axial offset",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset X2 [" + unitAbbreviation + "]",
        "X2",
        "X2 - End axial offset",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset Y [" + unitAbbreviation + "]",
        "Y",
        "Y Offset",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset Z [" + unitAbbreviation + "]",
        "Z",
        "Z Offset",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var offset = new GsaOffset();
      var gsaoffset = new GsaOffset();
      if (da.GetData(0, ref gsaoffset))
        offset = gsaoffset.Duplicate();

      if (offset == null)
        return;

      int inp = 1;
      if (Params.Input[inp]
          .SourceCount
        != 0)
        offset.X1 = (Length)Input.UnitNumber(this, da, inp++, _lengthUnit, true);

      if (Params.Input[inp]
          .SourceCount
        != 0)
        offset.X2 = (Length)Input.UnitNumber(this, da, inp++, _lengthUnit, true);

      if (Params.Input[inp]
          .SourceCount
        != 0)
        offset.Y = (Length)Input.UnitNumber(this, da, inp++, _lengthUnit, true);

      if (Params.Input[inp]
          .SourceCount
        != 0)
        offset.Z = (Length)Input.UnitNumber(this, da, inp, _lengthUnit, true);

      int outp = 0;
      da.SetData(outp++, new GsaOffsetGoo(offset));

      da.SetData(outp++, new GH_UnitNumber(offset.X1.ToUnit(_lengthUnit)));
      da.SetData(outp++, new GH_UnitNumber(offset.X2.ToUnit(_lengthUnit)));
      da.SetData(outp++, new GH_UnitNumber(offset.Y.ToUnit(_lengthUnit)));
      da.SetData(outp, new GH_UnitNumber(offset.Z.ToUnit(_lengthUnit)));
    }

    #endregion Protected Methods

    #region Private Methods
    private void Update(string unit) {
      _lengthUnit = Length.ParseUnit(unit);
      Message = unit;
      ExpireSolution(true);
    }

    #endregion Private Methods
  }
}
