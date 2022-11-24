using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaAPI;
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
    /// Component to edit a Prop2d and ouput the information
    /// </summary>
    public class EditProp2d_OBSOLETE : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("ab8af109-7ebc-4e49-9f5d-d4cb8ee45557");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.EditProp2d;

    public EditProp2d_OBSOLETE() : base("Edit 2D Property",
      "Prop2dEdit",
      "Modify GSA 2D Property",
      CategoryName.Name(),
      SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaProp2dParameter(), GsaProp2dGoo.Name, GsaProp2dGoo.NickName, GsaProp2dGoo.Description + " to get or set information for. Leave blank to create a new " + GsaProp2dGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Prop2d Number", "ID", "Set 2D Property Number. If ID is set it will replace any existing 2D Property in the model", GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());
      pManager.AddGenericParameter("Thickness [" + Length.GetAbbreviation(this.LengthUnit) + "]", "Th", "Set Property Thickness", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Axis", "Ax", "Set Axis as integer: Global (0) or Topological (1)", GH_ParamAccess.item);
      pManager.AddTextParameter("Prop2d Name", "Na", "Set Name of 2D Proerty", GH_ParamAccess.item);
      pManager.AddColourParameter("Prop2d Colour", "Co", "Set 2D Property Colour", GH_ParamAccess.item);
      pManager.AddTextParameter("Type", "Ty", "Set 2D Property Type." + Environment.NewLine +
          "Input either text string or integer:"
          + Environment.NewLine + "Plane Stress : 1"
          + Environment.NewLine + "Plane Strain : 2"
          + Environment.NewLine + "Axis Symmetric : 3"
          + Environment.NewLine + "Fabric : 4"
          + Environment.NewLine + "Plate : 5"
          + Environment.NewLine + "Shell : 6"
          + Environment.NewLine + "Curved Shell : 7"
          + Environment.NewLine + "Torsion : 8"
          + Environment.NewLine + "Wall : 9"
          + Environment.NewLine + "Load : 10",
          GH_ParamAccess.item);
      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaProp2dParameter(), GsaProp2dGoo.Name, GsaProp2dGoo.NickName, GsaProp2dGoo.Description + " with applied changes.", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Prop2d Number", "ID", "2D Property Number", GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());
      pManager.AddGenericParameter("Thickness [" + Length.GetAbbreviation(this.LengthUnit) + "]", "Th", "Get Property Thickness", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Axis", "Ax", "Get Axis: Global (0) or Topological (1)", GH_ParamAccess.item);
      pManager.AddTextParameter("Prop2d Name", "Na", "Name of 2D Proerty", GH_ParamAccess.item);
      pManager.AddColourParameter("Prop2d Colour", "Co", "2D Property Colour", GH_ParamAccess.item);
      pManager.AddTextParameter("Type", "Ty", "2D Property Type", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaProp2d gsaProp2d = new GsaProp2d();
      GsaProp2d prop = new GsaProp2d();
      if (DA.GetData(0, ref gsaProp2d))
      {
        prop = gsaProp2d.Duplicate();
      }

      if (prop != null)
      {
        // #### inputs ####
        // 1 ID
        GH_Integer ghID = new GH_Integer();
        if (DA.GetData(1, ref ghID))
        {
          if (GH_Convert.ToInt32(ghID, out int id, GH_Conversion.Both))
            prop.Id = id;
        }

        // 2 Material
        GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
        if (DA.GetData(2, ref gh_typ))
        {
          GsaMaterial material = new GsaMaterial();
          if (gh_typ.Value is GsaMaterialGoo)
          {
            gh_typ.CastTo(ref material);
            prop.Material = material;
          }
          else
          {
            if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
              prop.MaterialID = idd;
            else
            {
              AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PB input to a Section Property of reference integer");
              return;
            }
          }
        }

        // 3 Thickness
        if (this.Params.Input[3].SourceCount > 0)
          prop.Thickness = (Length)Input.UnitNumber(this, DA, 3, this.LengthUnit, true);

        // 4 Axis
        GH_Integer ghax = new GH_Integer();
        if (DA.GetData(4, ref ghax))
        {
          if (GH_Convert.ToInt32(ghax, out int axis, GH_Conversion.Both))
          {
            prop.AxisProperty = axis;
          }
        }

        // 5 name
        GH_String ghnm = new GH_String();
        if (DA.GetData(5, ref ghnm))
        {
          if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both))
            prop.Name = name;
        }

        // 6 Colour
        GH_Colour ghcol = new GH_Colour();
        if (DA.GetData(6, ref ghcol))
        {
          if (GH_Convert.ToColor(ghcol, out System.Drawing.Color col, GH_Conversion.Both))
            prop.Colour = col;
        }

        // 7 type
        GH_ObjectWrapper ghType = new GH_ObjectWrapper();
        if (DA.GetData(7, ref ghType))
        {
          if (GH_Convert.ToInt32(ghType, out int number, GH_Conversion.Both))
            prop.Type = (Property2D_Type)number;
          else if (GH_Convert.ToString(ghType, out string type, GH_Conversion.Both))
            prop.Type = GsaProp2d.PropTypeFromString(type);
        }

        //#### outputs ####
        int ax = (prop.API_Prop2d == null) ? 0 : prop.AxisProperty;
        string nm = (prop.API_Prop2d == null) ? "--" : prop.Name;
        ValueType colour = (prop.API_Prop2d == null) ? null : prop.API_Prop2d.Colour;

        DA.SetData(0, new GsaProp2dGoo(prop));
        DA.SetData(1, prop.Id);
        DA.SetData(2, new GsaMaterialGoo(new GsaMaterial(prop)));
        if (prop.API_Prop2d.Description == "")
          DA.SetData(3, new GH_UnitNumber(Length.Zero));
        else
          DA.SetData(3, new GH_UnitNumber(prop.Thickness.ToUnit(this.LengthUnit)));
        DA.SetData(4, ax);
        DA.SetData(5, nm);
        DA.SetData(6, colour);

        DA.SetData(7, Mappings.Prop2dTypeMapping.FirstOrDefault(x => x.Value == prop.Type).Key);
      }
      else
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Prop2d is Null");
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
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
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
        this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("LengthUnit"));
      else
        this.LengthUnit = OasysGH.Units.DefaultUnits.LengthUnitSection;
      return base.Read(reader);
    }
    #endregion
  }
}
