using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to edit an Offset and ouput the information
    /// </summary>
    public class EditOffset_OBSOLETE : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("1e094fcd-8f5f-4047-983c-e0e57a83ae52");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.EditOffset;

    public EditOffset_OBSOLETE() : base("Edit Offset",
      "OffsetEdit",
      "Modify GSA Offset or just get information about existing",
      CategoryName.Name(),
      SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

      pManager.AddParameter(new GsaOffsetParameter(), GsaOffsetGoo.Name, GsaOffsetGoo.NickName, GsaOffsetGoo.Description + " to get or set information for. Leave blank to create a new " + GsaOffsetGoo.Name, GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset X1 [" + unitAbbreviation + "]", "X1", "X1 - Start axial offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset X2 [" + unitAbbreviation + "]", "X2", "X2 - End axial offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset Y [" + unitAbbreviation + "]", "Y", "Y Offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset Z [" + unitAbbreviation + "]", "Z", "Z Offset", GH_ParamAccess.item);
      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

      pManager.AddParameter(new GsaOffsetParameter(), GsaOffsetGoo.Name, GsaOffsetGoo.NickName, GsaOffsetGoo.Description + " with applied changes.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset X1 [" + unitAbbreviation + "]", "X1", "X1 - Start axial offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset X2 [" + unitAbbreviation + "]", "X2", "X2 - End axial offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset Y [" + unitAbbreviation + "]", "Y", "Y Offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset Z [" + unitAbbreviation + "]", "Z", "Z Offset", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaOffset offset = new GsaOffset();
      GsaOffset gsaoffset = new GsaOffset();
      if (DA.GetData(0, ref gsaoffset))
      {
        offset = gsaoffset.Duplicate();
      }

      if (offset != null)
      {
        int inp = 1;
        if (this.Params.Input[inp].SourceCount != 0)
          offset.X1 = (Length)Input.UnitNumber(this, DA, inp++, this.LengthUnit, true);

        if (this.Params.Input[inp].SourceCount != 0)
          offset.X2 = (Length)Input.UnitNumber(this, DA, inp++, this.LengthUnit, true);

        if (this.Params.Input[inp].SourceCount != 0)
          offset.Y = (Length)Input.UnitNumber(this, DA, inp++, this.LengthUnit, true);

        if (this.Params.Input[inp].SourceCount != 0)
          offset.Z = (Length)Input.UnitNumber(this, DA, inp++, this.LengthUnit, true);

        //outputs
        int outp = 0;
        DA.SetData(outp++, new GsaOffsetGoo(offset));

        DA.SetData(outp++, new GH_UnitNumber(offset.X1.ToUnit(this.LengthUnit)));
        DA.SetData(outp++, new GH_UnitNumber(offset.X2.ToUnit(this.LengthUnit)));
        DA.SetData(outp++, new GH_UnitNumber(offset.Y.ToUnit(this.LengthUnit)));
        DA.SetData(outp++, new GH_UnitNumber(offset.Z.ToUnit(this.LengthUnit)));
      }
    }

    #region Custom UI
    protected override void BeforeSolveInstance()
    {
      this.Message = Length.GetAbbreviation(this.LengthUnit);
    }

    LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;
    public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
    {
      Menu_AppendSeparator(menu);

      ToolStripMenuItem unitsMenu = new ToolStripMenuItem("Select unit", Properties.Resources.Units);
      unitsMenu.Enabled = true;
      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length))
      {
        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { Update(unit); });
        toolStripMenuItem.Checked = unit == Length.GetAbbreviation(this.LengthUnit);
        toolStripMenuItem.Enabled = true;
        unitsMenu.DropDownItems.Add(toolStripMenuItem);
      }
      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }
    private void Update(string unit)
    {
      this.LengthUnit = Length.ParseUnit(unit);
      this.Message = unit;
      ExpireSolution(true);
    }
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetString("LengthUnit", this.LengthUnit.ToString());
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      if (reader.ItemExists("LengthUnit"))
      {
        this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("LengthUnit"));
        bool flag = base.Read(reader);
        return flag & this.Params.ReadAllParameterData(reader);
      }
      else
      {
        this.LengthUnit = DefaultUnits.LengthUnitSection;
        return base.Read(reader);
      }
    }
    #endregion
  }
}
