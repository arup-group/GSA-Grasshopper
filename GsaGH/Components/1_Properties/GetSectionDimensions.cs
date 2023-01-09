using System;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units.Helpers;
using OasysGH.Units;
using OasysUnits.Units;
using OasysUnits;
using GsaGH.Helpers.GsaAPI;
using System.Globalization;
using OasysGH.Helpers;
using System.Collections.Generic;
using System.IO;
using GsaGH.Helpers;
using GsaGH.Helpers.GH;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to get geometric dimensions of a section
    /// </summary>
    public class GetSectionDimensions : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("98765d83-2b23-47c1-ad1d-201b5a2eed8b");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.SectionDimensions;

    public GetSectionDimensions() : base("Section Dimensions",
      "SectDims",
      "Get GSA Section Dimensions",
      CategoryName.Name(),
      SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaSectionParameter(), GsaSectionGoo.Name, GsaSectionGoo.NickName, GsaSectionGoo.Description + " to get a bit more info out of.", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      string abb = Length.GetAbbreviation(this.LengthUnit);

      pManager.AddGenericParameter("Depth [" + abb + "]", "D", "Section Depth or Diameter)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Width [" + abb + "]", "W", "Section Width", GH_ParamAccess.item);
      pManager.AddGenericParameter("Width Top [" + abb + "]", "Wt", "Section Width Top (will be equal to width if profile is symmetric)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Width Bottom [" + abb + "]", "Wb", "Section Width Bottom (will be equal to width if profile is symmetric)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Flange Thk Top [" + abb + "]", "Ftt", "Section Top Flange Thickness", GH_ParamAccess.item);
      pManager.AddGenericParameter("Flange Thk Bottom [" + abb + "]", "Ftb", "Section Bottom Flange Thickness", GH_ParamAccess.item);
      pManager.AddGenericParameter("Web Thk [" + abb + "]", "Wt", "Section Web Thickness", GH_ParamAccess.item);
      pManager.AddGenericParameter("Radius [" + abb + "]", "r", "Section Root Radius (only applicable to catalogue profiles) or hole size for cellular/castellated beams", GH_ParamAccess.item);
      pManager.AddGenericParameter("Spacing [" + abb + "]", "s", "Spacing/pitch", GH_ParamAccess.item);
      pManager.AddTextParameter("Type", "typ", "Profile type description", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaSection gsaSection = new GsaSection();
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        if (gh_typ.Value is GsaSectionGoo)
          gh_typ.CastTo(ref gsaSection);
        else
        {
          string profileIn = "";
          gh_typ.CastTo(ref profileIn);
          if (GsaSection.ValidProfile(profileIn))
            gsaSection = new GsaSection(profileIn);
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Invalid profile syntax: " + profileIn);
            return;
          }
        }

        string profile = gsaSection.Profile;
        if (profile.Trim() == "")
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Profile not set in Section");
          return;
        }
        string[] parts = profile.Split(' ');

        LengthUnit unit = LengthUnit.Millimeter; // default unit for sections is mm
        string[] type = parts[1].Split('(', ')');
        if (type.Length > 1)
        {
          var parser = UnitParser.Default;
          unit = parser.Parse<LengthUnit>(type[1]);
        }

        int i = 0;

        // angle
        if (profile.StartsWith("STD A"))
        {
          SetOutput(DA, i++, parts[2], unit); //Depth
          SetOutput(DA, i++, parts[3], unit); //Width
          SetOutput(DA, i++, parts[3], unit); //Width Top
          SetOutput(DA, i++, parts[3], unit); //Width Bottom
          SetOutput(DA, i++, parts[5], unit); //Flange Thk Top
          SetOutput(DA, i++, parts[5], unit); //Flange Thk Bottom
          SetOutput(DA, i++, parts[4], unit); //Web Thk Bottom
          DA.SetData(i++, null);              //Root radius
          DA.SetData(i++, null);              //Spacing
          DA.SetData(i, type[0]);
        }

        // channel
        else if (profile.StartsWith("STD CH ") || profile.StartsWith("STD CH("))
        {
          SetOutput(DA, i++, parts[2], unit); //Depth
          SetOutput(DA, i++, parts[3], unit); //Width
          SetOutput(DA, i++, parts[3], unit); //Width Top
          SetOutput(DA, i++, parts[3], unit); //Width Bottom
          SetOutput(DA, i++, parts[5], unit); //Flange Thk Top
          SetOutput(DA, i++, parts[5], unit); //Flange Thk Bottom
          SetOutput(DA, i++, parts[4], unit); //Web Thk Bottom
          DA.SetData(i++, null);              //Root radius
          DA.SetData(i++, null);              //Spacing
          DA.SetData(i, type[0]);
        }

        // circle hollow
        else if (profile.StartsWith("STD CHS"))
        {
          SetOutput(DA, i++, parts[2], unit); //Depth
          DA.SetData(i++, null);              //Width
          DA.SetData(i++, null);              //Width Top
          DA.SetData(i++, null);              //Width Bottom
          DA.SetData(i++, null);              //Flange Thk Top
          DA.SetData(i++, null);              //Flange Thk Bottom
          SetOutput(DA, i++, parts[3], unit); //Web Thk Bottom
          DA.SetData(i++, null);              //Root radius
          DA.SetData(i++, null);              //Spacing
          DA.SetData(i, type[0]);
        }

        // circle
        else if (profile.StartsWith("STD C ") || profile.StartsWith("STD C("))
        {
          SetOutput(DA, i++, parts[2], unit); //Depth
          DA.SetData(i++, null);              //Width
          DA.SetData(i++, null);              //Width Top
          DA.SetData(i++, null);              //Width Bottom
          DA.SetData(i++, null);              //Flange Thk Top
          DA.SetData(i++, null);              //Flange Thk Bottom
          DA.SetData(i++, null);              //Web Thk Bottom
          DA.SetData(i++, null);              //Root radius
          DA.SetData(i++, null);              //Spacing
          DA.SetData(i, type[0]);
        }

        // ICruciformSymmetricalProfile
        else if (profile.StartsWith("STD X"))
        {
          SetOutput(DA, i++, parts[2], unit); //Depth
          SetOutput(DA, i++, parts[3], unit); //Width
          SetOutput(DA, i++, parts[3], unit); //Width Top
          SetOutput(DA, i++, parts[3], unit); //Width Bottom
          SetOutput(DA, i++, parts[5], unit); //Flange Thk Top
          SetOutput(DA, i++, parts[5], unit); //Flange Thk Bottom
          SetOutput(DA, i++, parts[4], unit); //Web Thk Bottom
          DA.SetData(i++, null);              //Root radius
          DA.SetData(i++, null);              //Spacing
          DA.SetData(i, type[0]);
        }

        // IEllipseHollowProfile
        else if (profile.StartsWith("STD OVAL"))
        {
          SetOutput(DA, i++, parts[2], unit); //Depth
          SetOutput(DA, i++, parts[3], unit); //Width
          SetOutput(DA, i++, parts[3], unit); //Width Top
          SetOutput(DA, i++, parts[3], unit); //Width Bottom
          SetOutput(DA, i++, parts[4], unit); //Flange Thk Top
          SetOutput(DA, i++, parts[4], unit); //Flange Thk Bottom
          SetOutput(DA, i++, parts[4], unit); //Web Thk Bottom
          DA.SetData(i++, null);              //Root radius
          DA.SetData(i++, null);              //Spacing
          DA.SetData(i, type[0]);
        }

        // IEllipseProfile
        else if (profile.StartsWith("STD E"))
        {
          SetOutput(DA, i++, parts[2], unit); //Depth
          SetOutput(DA, i++, parts[3], unit); //Width
          SetOutput(DA, i++, parts[3], unit); //Width Top
          SetOutput(DA, i++, parts[3], unit); //Width Bottom
          DA.SetData(i++, null);              //Flange Thk Top
          DA.SetData(i++, null);              //Flange Thk Bottom
          DA.SetData(i++, null);              //Web Thk Bottom
          DA.SetData(i++, null);              //Root radius
          DA.SetData(i++, null);              //Spacing
          DA.SetData(i, type[0]);
        }

        // IGeneralCProfile
        else if (profile.StartsWith("STD GC"))
        {
          SetOutput(DA, i++, parts[2], unit); //Depth/Diameter
          SetOutput(DA, i++, parts[3], unit); //Width
          SetOutput(DA, i++, parts[3], unit); //Width Top
          SetOutput(DA, i++, parts[3], unit); //Width Bottom
          SetOutput(DA, i++, parts[4], unit); //Flange Thk Top
          SetOutput(DA, i++, parts[4], unit); //Flange Thk Bottom
          SetOutput(DA, i++, parts[5], unit); //Web Thk Bottom
          DA.SetData(i++, null);              //Root radius
          DA.SetData(i++, null);              //Spacing
          DA.SetData(i, type[0]);
        }

        // IGeneralZProfile
        else if (profile.StartsWith("STD GZ"))
        {
          double top = double.Parse(parts[3], CultureInfo.InvariantCulture);
          double bottom = double.Parse(parts[4], CultureInfo.InvariantCulture);
          Length length = new Length(top + bottom, unit);
          SetOutput(DA, i++, parts[2], unit); //Depth
          DA.SetData(i++, new GH_UnitNumber(length.ToUnit(this.LengthUnit))); //Width
          SetOutput(DA, i++, parts[3], unit); //Width Top
          SetOutput(DA, i++, parts[4], unit); //Width Bottom
          SetOutput(DA, i++, parts[5], unit); //Flange Thk Top
          SetOutput(DA, i++, parts[6], unit); //Flange Thk Bottom
          SetOutput(DA, i++, parts[7], unit); //Web Thk Bottom
          DA.SetData(i++, null);              //Root radius
          DA.SetData(i++, null);              //Spacing
          DA.SetData(i, type[0]);
        }

        // IIBeamAsymmetricalProfile
        else if (profile.StartsWith("STD GI"))
        {
          double top = double.Parse(parts[3], CultureInfo.InvariantCulture);
          double bottom = double.Parse(parts[4], CultureInfo.InvariantCulture);
          Length length = new Length(Math.Max(top, bottom), unit);
          SetOutput(DA, i++, parts[2], unit); //Depth
          DA.SetData(i++, new GH_UnitNumber(length.ToUnit(this.LengthUnit))); //Width
          SetOutput(DA, i++, parts[3], unit); //Width Top
          SetOutput(DA, i++, parts[4], unit); //Width Bottom
          SetOutput(DA, i++, parts[6], unit); //Flange Thk Top
          SetOutput(DA, i++, parts[7], unit); //Flange Thk Bottom
          SetOutput(DA, i++, parts[5], unit); //Web Thk Bottom
          DA.SetData(i++, null);              //Root radius
          DA.SetData(i++, null);              //Spacing
          DA.SetData(i, type[0]);
        }

        // IIBeamCellularProfile
        else if (profile.StartsWith("STD CB"))
        {
          SetOutput(DA, i++, parts[2], unit); //Depth/Diameter
          SetOutput(DA, i++, parts[3], unit); //Width
          SetOutput(DA, i++, parts[3], unit); //Width Top
          SetOutput(DA, i++, parts[3], unit); //Width Bottom
          SetOutput(DA, i++, parts[5], unit); //Flange Thk Top
          SetOutput(DA, i++, parts[5], unit); //Flange Thk Bottom
          SetOutput(DA, i++, parts[4], unit); //Web Thk Bottom
          SetOutput(DA, i++, parts[6], unit); //hole size
          SetOutput(DA, i++, parts[7], unit); //pitch
          DA.SetData(i, type[0]);
        }

        // IIBeamSymmetricalProfile
        else if (profile.StartsWith("STD I"))
        {
          SetOutput(DA, i++, parts[2], unit); //Depth/Diameter
          SetOutput(DA, i++, parts[3], unit); //Width
          SetOutput(DA, i++, parts[3], unit); //Width Top
          SetOutput(DA, i++, parts[3], unit); //Width Bottom
          SetOutput(DA, i++, parts[5], unit); //Flange Thk Top
          SetOutput(DA, i++, parts[5], unit); //Flange Thk Bottom
          SetOutput(DA, i++, parts[4], unit); //Web Thk Bottom
          DA.SetData(i++, null);              //Root radius
          DA.SetData(i++, null);              //Spacing
          DA.SetData(i, type[0]);
        }

        // IRectangleHollowProfile
        else if (profile.StartsWith("STD RHS"))
        {
          SetOutput(DA, i++, parts[2], unit); //Depth/Diameter
          SetOutput(DA, i++, parts[3], unit); //Width
          SetOutput(DA, i++, parts[3], unit); //Width Top
          SetOutput(DA, i++, parts[3], unit); //Width Bottom
          SetOutput(DA, i++, parts[5], unit); //Flange Thk Top
          SetOutput(DA, i++, parts[5], unit); //Flange Thk Bottom
          SetOutput(DA, i++, parts[4], unit); //Web Thk Bottom
          DA.SetData(i++, null);              //Root radius
          DA.SetData(i++, null);              //Spacing
          DA.SetData(i, type[0]);
        }

        // IRectangleProfile
        else if (profile.StartsWith("STD R ") || profile.StartsWith("STD R("))
        {
          SetOutput(DA, i++, parts[2], unit); //Depth/Diameter
          SetOutput(DA, i++, parts[3], unit); //Width
          SetOutput(DA, i++, parts[3], unit); //Width Top
          SetOutput(DA, i++, parts[3], unit); //Width Bottom
          DA.SetData(i++, null);              //Flange Thk Top
          DA.SetData(i++, null);              //Flange Thk Bottom
          DA.SetData(i++, null);              //Web Thk Bottom
          DA.SetData(i++, null);              //Root radius
          DA.SetData(i++, null);              //Spacing
          DA.SetData(i, type[0]);
        }

        // IRectoEllipseProfile
        else if (profile.StartsWith("STD RE"))
        {
          SetOutput(DA, i++, parts[2], unit); //Depth
          SetOutput(DA, i++, parts[4], unit); //Width
          SetOutput(DA, i++, parts[3], unit); //Width Top
          SetOutput(DA, i++, parts[5], unit); //Width Bottom
          DA.SetData(i++, null);              //Flange Thk Top
          DA.SetData(i++, null);              //Flange Thk Bottom
          DA.SetData(i++, null);              //Web Thk Bottom
          DA.SetData(i++, null);              //Root radius
          DA.SetData(i++, null);              //Spacing
          DA.SetData(i, type[0]);
        }

        // ISecantPileProfile
        else if (profile.StartsWith("STD SP"))
        {
          SetOutput(DA, i++, parts[2], unit); //Depth/Diameter
          Length length = Length.Zero;
          if (profile.StartsWith("STD SPW"))
          {
            // STD SPW 250 100 4
            int count = int.Parse(parts[4], CultureInfo.InvariantCulture);
            double spacing = double.Parse(parts[3], CultureInfo.InvariantCulture);
            length = new Length(count * spacing, unit);
          }
          else
          {
            // STD SP 250 100 4
            int count = int.Parse(parts[4], CultureInfo.InvariantCulture);
            double spacing = double.Parse(parts[3], CultureInfo.InvariantCulture);
            double diameter = double.Parse(parts[2], CultureInfo.InvariantCulture);
            length = new Length((count - 1) * spacing + diameter, unit);
          }
          DA.SetData(i++, new GH_UnitNumber(length.ToUnit(this.LengthUnit))); //Width
          DA.SetData(i++, null);              //Width Top
          DA.SetData(i++, null);              //Width Bottom
          DA.SetData(i++, null);              //Flange Thk Top
          DA.SetData(i++, null);              //Flange Thk Bottom
          DA.SetData(i++, null);              //Web Thk Bottom
          DA.SetData(i++, null);              //Root radius
          SetOutput(DA, i++, parts[3], unit); //Spacing
          DA.SetData(i, type[0]);
        }

        // ISheetPileProfile
        else if (profile.StartsWith("STD SHT"))
        {
          SetOutput(DA, i++, parts[2], unit); //Depth
          SetOutput(DA, i++, parts[3], unit); //Width
          SetOutput(DA, i++, parts[4], unit); //Width Top
          SetOutput(DA, i++, parts[5], unit); //Width Bottom
          SetOutput(DA, i++, parts[6], unit); //Flange Thk Top
          SetOutput(DA, i++, parts[6], unit); //Flange Thk Bottom
          SetOutput(DA, i++, parts[7], unit); //Web Thk Bottom
          DA.SetData(i++, null);              //Root radius
          DA.SetData(i++, null);              //Spacing
          DA.SetData(i, type[0]);
        }

        // IStadiumProfile
        else if (profile.StartsWith("STD RC"))
        {
          SetOutput(DA, i++, parts[2], unit); //Depth
          SetOutput(DA, i++, parts[3], unit); //Width
          DA.SetData(i++, null);              //Width Top
          DA.SetData(i++, null);              //Width Bottom
          DA.SetData(i++, null);              //Flange Thk Top
          DA.SetData(i++, null);              //Flange Thk Bottom
          DA.SetData(i++, null);              //Web Thk Bottom
          DA.SetData(i++, null);              //Root radius
          DA.SetData(i++, null);              //Spacing
          DA.SetData(i, type[0]);
        }

        // ITrapezoidProfile
        else if (profile.StartsWith("STD TR"))
        {
          double top = double.Parse(parts[3], CultureInfo.InvariantCulture);
          double bottom = double.Parse(parts[4], CultureInfo.InvariantCulture);
          Length length = new Length(Math.Max(top, bottom), unit);
          SetOutput(DA, i++, parts[2], unit); //Depth
          DA.SetData(i++, new GH_UnitNumber(length.ToUnit(this.LengthUnit))); //Width
          SetOutput(DA, i++, parts[3], unit); //Width Top
          SetOutput(DA, i++, parts[4], unit); //Width Bottom
          DA.SetData(i++, null);              //Flange Thk Top
          DA.SetData(i++, null);              //Flange Thk Bottom
          DA.SetData(i++, null);              //Web Thk Bottom
          DA.SetData(i++, null);              //Root radius
          DA.SetData(i++, null);              //Spacing
          DA.SetData(i, type[0]);
        }

        // ITSectionProfile
        else if (profile.StartsWith("STD T"))
        {
          SetOutput(DA, i++, parts[2], unit); //Depth
          SetOutput(DA, i++, parts[3], unit); //Width
          SetOutput(DA, i++, parts[3], unit); //Width Top
          SetOutput(DA, i++, parts[4], unit); //Width Bottom
          SetOutput(DA, i++, parts[5], unit); //Flange Thk Top
          DA.SetData(i++, null);              //Flange Thk Bottom
          SetOutput(DA, i++, parts[4], unit); //Web Thk Bottom
          DA.SetData(i++, null);              //Root radius
          DA.SetData(i++, null);              //Spacing
          DA.SetData(i, type[0]);
        }
        else if (profile.StartsWith("CAT"))
        {
          string prof = profile.Split(' ')[2];
          List<double> sqlValues = SqlReader.reader.GetCatalogueProfileValues(prof, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"));
          unit = LengthUnit.Meter;

          DA.SetData(i++, new GH_UnitNumber(new Length(sqlValues[0], unit).ToUnit(this.LengthUnit))); //Depth
          DA.SetData(i++, new GH_UnitNumber(new Length(sqlValues[1], unit).ToUnit(this.LengthUnit))); //Width
          DA.SetData(i++, new GH_UnitNumber(new Length(sqlValues[1], unit).ToUnit(this.LengthUnit))); //Width Top
          DA.SetData(i++, new GH_UnitNumber(new Length(sqlValues[1], unit).ToUnit(this.LengthUnit))); //Width Bottom
          DA.SetData(i++, new GH_UnitNumber(new Length(sqlValues[3], unit).ToUnit(this.LengthUnit))); //Flange Thk Top
          DA.SetData(i++, new GH_UnitNumber(new Length(sqlValues[3], unit).ToUnit(this.LengthUnit))); //Flange Thk Bottom
          DA.SetData(i++, new GH_UnitNumber(new Length(sqlValues[2], unit).ToUnit(this.LengthUnit))); //Web Thk Bottom
          DA.SetData(i++, new GH_UnitNumber(new Length(sqlValues[4], unit).ToUnit(this.LengthUnit))); //Root radius
          DA.SetData(i++, null); //Spacing
          DA.SetData(i, "CAT");
        }
        else
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to get dimensions for type " + type[0]);
      }
    }

    private void SetOutput(IGH_DataAccess DA, int outputID, string outputValue, LengthUnit unit)
    {
      double val = double.Parse(outputValue, CultureInfo.InvariantCulture);
      Length length = new Length(val, unit);
      DA.SetData(outputID, new GH_UnitNumber(length.ToUnit(this.LengthUnit)));
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
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetString("LengthUnit", this.LengthUnit.ToString());
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("LengthUnit"));
      return base.Read(reader);
    }
    public virtual void VariableParameterMaintenance()
    {
      string abb = Length.GetAbbreviation(this.LengthUnit);

      this.Params.Output[0].Name = "Depth [" + abb + "]";
      this.Params.Output[1].Name = "Width [" + abb + "]";
      this.Params.Output[2].Name = "Width Top [" + abb + "]";
      this.Params.Output[3].Name = "Width Bottom [" + abb + "]";
      this.Params.Output[4].Name = "Flange Thk Top [" + abb + "]";
      this.Params.Output[5].Name = "Flange Thk Bottom [" + abb + "]";
      this.Params.Output[6].Name = "Web Thk [" + abb + "]";
      this.Params.Output[7].Name = "Radius [" + abb + "]";
      this.Params.Output[8].Name = "Spacing [" + abb + "]";
    }

    #region IGH_VariableParameterComponent null implementation

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index) => false;
    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index) => false;
    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index) => null;
    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) => false;
    #endregion
    #endregion
  }
}

