﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get geometric dimensions of a section
  /// </summary>
  public class GetSectionDimensions : GH_OasysComponent, IGH_VariableParameterComponent {
    public override Guid ComponentGuid => new Guid("98765d83-2b23-47c1-ad1d-201b5a2eed8b");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.SectionDimensions;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;

    public GetSectionDimensions() : base("Section Dimensions", "SectDims",
      "Get GSA Section Dimensions", CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);

      var unitsMenu = new ToolStripMenuItem("Select unit", Resources.Units) {
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
      string abb = Length.GetAbbreviation(_lengthUnit);

      Params.Output[0].Name = "Depth [" + abb + "]";
      Params.Output[1].Name = "Width [" + abb + "]";
      Params.Output[2].Name = "Width Top [" + abb + "]";
      Params.Output[3].Name = "Width Bottom [" + abb + "]";
      Params.Output[4].Name = "Flange Thk Top [" + abb + "]";
      Params.Output[5].Name = "Flange Thk Bottom [" + abb + "]";
      Params.Output[6].Name = "Web Thk [" + abb + "]";
      Params.Output[7].Name = "Radius [" + abb + "]";
      Params.Output[8].Name = "Spacing [" + abb + "]";
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetString("LengthUnit", _lengthUnit.ToString());
      return base.Write(writer);
    }

    protected override void BeforeSolveInstance() {
      Message = Length.GetAbbreviation(_lengthUnit);
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaSectionParameter(), GsaSectionGoo.Name, GsaSectionGoo.NickName,
        GsaSectionGoo.Description + " to get a bit more info out of.", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string abb = Length.GetAbbreviation(_lengthUnit);

      pManager.AddGenericParameter("Depth [" + abb + "]", "D", "Section Depth or Diameter)",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Width [" + abb + "]", "W", "Section Width",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Width Top [" + abb + "]", "Wt",
        "Section Width Top (will be equal to width if profile is symmetric)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Width Bottom [" + abb + "]", "Wb",
        "Section Width Bottom (will be equal to width if profile is symmetric)",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Flange Thk Top [" + abb + "]", "Ftt",
        "Section Top Flange Thickness", GH_ParamAccess.item);
      pManager.AddGenericParameter("Flange Thk Bottom [" + abb + "]", "Ftb",
        "Section Bottom Flange Thickness", GH_ParamAccess.item);
      pManager.AddGenericParameter("Web Thk [" + abb + "]", "Wt", "Section Web Thickness",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Radius [" + abb + "]", "r",
        "Section Root Radius (only applicable to catalogue profiles) or hole size for cellular/castellated beams",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Spacing [" + abb + "]", "s", "Spacing/pitch",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Type", "typ", "Profile type description", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaSectionGoo sectionGoo = null;
      da.GetData(0, ref sectionGoo);
      GsaSection section = sectionGoo.Value;

      string profile = section.Profile;
      if (profile.Trim() == string.Empty) {
        this.AddRuntimeError("Profile not set in Section");
        return;
      }

      string[] parts = profile.Split(' ');

      LengthUnit unit = LengthUnit.Millimeter;
      string[] type = parts[1].Split('(', ')');
      if (type.Length > 1) {
        UnitParser parser = UnitParser.Default;
        unit = parser.Parse<LengthUnit>(type[1]);
      }

      int i = 0;

      // angle
      if (profile.StartsWith("STD A")) {
        SetOutput(da, i++, parts[2], unit); //Depth
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[3], unit); //Width Bottom
        SetOutput(da, i++, parts[5], unit); //Flange Thk Top
        SetOutput(da, i++, parts[5], unit); //Flange Thk Bottom
        SetOutput(da, i++, parts[4], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // channel
      else if (profile.StartsWith("STD CH ") || profile.StartsWith("STD CH(")) {
        SetOutput(da, i++, parts[2], unit); //Depth
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[3], unit); //Width Bottom
        SetOutput(da, i++, parts[5], unit); //Flange Thk Top
        SetOutput(da, i++, parts[5], unit); //Flange Thk Bottom
        SetOutput(da, i++, parts[4], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // circle hollow
      else if (profile.StartsWith("STD CHS")) {
        SetOutput(da, i++, parts[2], unit); //Depth
        da.SetData(i++, null); //Width
        da.SetData(i++, null); //Width Top
        da.SetData(i++, null); //Width Bottom
        da.SetData(i++, null); //Flange Thk Top
        da.SetData(i++, null); //Flange Thk Bottom
        SetOutput(da, i++, parts[3], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // circle
      else if (profile.StartsWith("STD C ") || profile.StartsWith("STD C(")) {
        SetOutput(da, i++, parts[2], unit); //Depth
        da.SetData(i++, null); //Width
        da.SetData(i++, null); //Width Top
        da.SetData(i++, null); //Width Bottom
        da.SetData(i++, null); //Flange Thk Top
        da.SetData(i++, null); //Flange Thk Bottom
        da.SetData(i++, null); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // ICruciformSymmetricalProfile
      else if (profile.StartsWith("STD X")) {
        SetOutput(da, i++, parts[2], unit); //Depth
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[3], unit); //Width Bottom
        SetOutput(da, i++, parts[5], unit); //Flange Thk Top
        SetOutput(da, i++, parts[5], unit); //Flange Thk Bottom
        SetOutput(da, i++, parts[4], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // IEllipseHollowProfile
      else if (profile.StartsWith("STD OVAL")) {
        SetOutput(da, i++, parts[2], unit); //Depth
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[3], unit); //Width Bottom
        SetOutput(da, i++, parts[4], unit); //Flange Thk Top
        SetOutput(da, i++, parts[4], unit); //Flange Thk Bottom
        SetOutput(da, i++, parts[4], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // IEllipseProfile
      else if (profile.StartsWith("STD E")) {
        SetOutput(da, i++, parts[2], unit); //Depth
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[3], unit); //Width Bottom
        da.SetData(i++, null); //Flange Thk Top
        da.SetData(i++, null); //Flange Thk Bottom
        da.SetData(i++, null); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // IGeneralCProfile
      else if (profile.StartsWith("STD GC")) {
        SetOutput(da, i++, parts[2], unit); //Depth/Diameter
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[3], unit); //Width Bottom
        SetOutput(da, i++, parts[4], unit); //Flange Thk Top
        SetOutput(da, i++, parts[4], unit); //Flange Thk Bottom
        SetOutput(da, i++, parts[5], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // IGeneralZProfile
      else if (profile.StartsWith("STD GZ")) {
        double top = double.Parse(parts[3], CultureInfo.InvariantCulture);
        double bottom = double.Parse(parts[4], CultureInfo.InvariantCulture);
        var length = new Length(top + bottom, unit);
        SetOutput(da, i++, parts[2], unit); //Depth
        da.SetData(i++, new GH_UnitNumber(length.ToUnit(_lengthUnit))); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[4], unit); //Width Bottom
        SetOutput(da, i++, parts[5], unit); //Flange Thk Top
        SetOutput(da, i++, parts[6], unit); //Flange Thk Bottom
        SetOutput(da, i++, parts[7], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // IIBeamAsymmetricalProfile
      else if (profile.StartsWith("STD GI")) {
        double top = double.Parse(parts[3], CultureInfo.InvariantCulture);
        double bottom = double.Parse(parts[4], CultureInfo.InvariantCulture);
        var length = new Length(Math.Max(top, bottom), unit);
        SetOutput(da, i++, parts[2], unit); //Depth
        da.SetData(i++, new GH_UnitNumber(length.ToUnit(_lengthUnit))); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[4], unit); //Width Bottom
        SetOutput(da, i++, parts[6], unit); //Flange Thk Top
        SetOutput(da, i++, parts[7], unit); //Flange Thk Bottom
        SetOutput(da, i++, parts[5], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // IIBeamCellularProfile
      else if (profile.StartsWith("STD CB")) {
        SetOutput(da, i++, parts[2], unit); //Depth/Diameter
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[3], unit); //Width Bottom
        SetOutput(da, i++, parts[5], unit); //Flange Thk Top
        SetOutput(da, i++, parts[5], unit); //Flange Thk Bottom
        SetOutput(da, i++, parts[4], unit); //Web Thk Bottom
        SetOutput(da, i++, parts[6], unit); //hole size
        SetOutput(da, i++, parts[7], unit); //pitch
        da.SetData(i, type[0]);
      }

      // IIBeamSymmetricalProfile
      else if (profile.StartsWith("STD I")) {
        SetOutput(da, i++, parts[2], unit); //Depth/Diameter
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[3], unit); //Width Bottom
        SetOutput(da, i++, parts[5], unit); //Flange Thk Top
        SetOutput(da, i++, parts[5], unit); //Flange Thk Bottom
        SetOutput(da, i++, parts[4], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // IRectangleHollowProfile
      else if (profile.StartsWith("STD RHS")) {
        SetOutput(da, i++, parts[2], unit); //Depth/Diameter
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[3], unit); //Width Bottom
        SetOutput(da, i++, parts[5], unit); //Flange Thk Top
        SetOutput(da, i++, parts[5], unit); //Flange Thk Bottom
        SetOutput(da, i++, parts[4], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // IRectangleProfile
      else if (profile.StartsWith("STD R ") || profile.StartsWith("STD R(")) {
        SetOutput(da, i++, parts[2], unit); //Depth/Diameter
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[3], unit); //Width Bottom
        da.SetData(i++, null); //Flange Thk Top
        da.SetData(i++, null); //Flange Thk Bottom
        da.SetData(i++, null); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // IRectoEllipseProfile
      else if (profile.StartsWith("STD RE")) {
        SetOutput(da, i++, parts[2], unit); //Depth
        SetOutput(da, i++, parts[4], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[5], unit); //Width Bottom
        da.SetData(i++, null); //Flange Thk Top
        da.SetData(i++, null); //Flange Thk Bottom
        da.SetData(i++, null); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // ISecantPileProfile
      else if (profile.StartsWith("STD SP")) {
        SetOutput(da, i++, parts[2], unit); //Depth/Diameter
        Length length;
        if (profile.StartsWith("STD SPW")) {
          // STD SPW 250 100 4
          int count = int.Parse(parts[4], CultureInfo.InvariantCulture);
          double spacing = double.Parse(parts[3], CultureInfo.InvariantCulture);
          length = new Length(count * spacing, unit);
        } else {
          // STD SP 250 100 4
          int count = int.Parse(parts[4], CultureInfo.InvariantCulture);
          double spacing = double.Parse(parts[3], CultureInfo.InvariantCulture);
          double diameter = double.Parse(parts[2], CultureInfo.InvariantCulture);
          length = new Length(((count - 1) * spacing) + diameter, unit);
        }

        da.SetData(i++, new GH_UnitNumber(length.ToUnit(_lengthUnit))); //Width
        da.SetData(i++, null); //Width Top
        da.SetData(i++, null); //Width Bottom
        da.SetData(i++, null); //Flange Thk Top
        da.SetData(i++, null); //Flange Thk Bottom
        da.SetData(i++, null); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        SetOutput(da, i++, parts[3], unit); //Spacing
        da.SetData(i, type[0]);
      }

      // ISheetPileProfile
      else if (profile.StartsWith("STD SHT")) {
        SetOutput(da, i++, parts[2], unit); //Depth
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[4], unit); //Width Top
        SetOutput(da, i++, parts[5], unit); //Width Bottom
        SetOutput(da, i++, parts[6], unit); //Flange Thk Top
        SetOutput(da, i++, parts[6], unit); //Flange Thk Bottom
        SetOutput(da, i++, parts[7], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // IStadiumProfile
      else if (profile.StartsWith("STD RC")) {
        SetOutput(da, i++, parts[2], unit); //Depth
        SetOutput(da, i++, parts[3], unit); //Width
        da.SetData(i++, null); //Width Top
        da.SetData(i++, null); //Width Bottom
        da.SetData(i++, null); //Flange Thk Top
        da.SetData(i++, null); //Flange Thk Bottom
        da.SetData(i++, null); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // ITrapezoidProfile
      else if (profile.StartsWith("STD TR")) {
        double top = double.Parse(parts[3], CultureInfo.InvariantCulture);
        double bottom = double.Parse(parts[4], CultureInfo.InvariantCulture);
        var length = new Length(Math.Max(top, bottom), unit);
        SetOutput(da, i++, parts[2], unit); //Depth
        da.SetData(i++, new GH_UnitNumber(length.ToUnit(_lengthUnit))); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[4], unit); //Width Bottom
        da.SetData(i++, null); //Flange Thk Top
        da.SetData(i++, null); //Flange Thk Bottom
        da.SetData(i++, null); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // ITSectionProfile
      else if (profile.StartsWith("STD T")) {
        SetOutput(da, i++, parts[2], unit); //Depth
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[4], unit); //Width Bottom
        SetOutput(da, i++, parts[5], unit); //Flange Thk Top
        da.SetData(i++, null); //Flange Thk Bottom
        SetOutput(da, i++, parts[4], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      } else if (profile.StartsWith("CAT")) {
        string prof = profile.Split(' ')[2];
        List<double> sqlValues = MicrosoftSQLiteReader.Instance.GetCatalogueProfileValues(prof,
          Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"));
        unit = LengthUnit.Meter;
        if (sqlValues.Count == 2) {
          da.SetData(i++,
            new GH_UnitNumber(new Length(sqlValues[0], unit).ToUnit(_lengthUnit))); //Depth
          da.SetData(i++,
            new GH_UnitNumber(new Length(sqlValues[0], unit).ToUnit(_lengthUnit))); //Width
          da.SetData(i++, null); //Width Top
          da.SetData(i++, null); //Width Bottom
          da.SetData(i++, null); //Flange Thk Top
          da.SetData(i++, null); //Flange Thk Bottom
          da.SetData(i++,
            new GH_UnitNumber(
              new Length(sqlValues[1], unit).ToUnit(_lengthUnit))); //Web Thk Bottom
          da.SetData(i++, null); //root radius
          da.SetData(i++, null); //Spacing
        } else {
          da.SetData(i++,
            new GH_UnitNumber(new Length(sqlValues[0], unit).ToUnit(_lengthUnit))); //Depth
          da.SetData(i++,
            new GH_UnitNumber(new Length(sqlValues[1], unit).ToUnit(_lengthUnit))); //Width
          da.SetData(i++,
            new GH_UnitNumber(new Length(sqlValues[1], unit).ToUnit(_lengthUnit))); //Width Top
          da.SetData(i++,
            new GH_UnitNumber(new Length(sqlValues[1], unit).ToUnit(_lengthUnit))); //Width Bottom
          da.SetData(i++,
            new GH_UnitNumber(
              new Length(sqlValues[3], unit).ToUnit(_lengthUnit))); //Flange Thk Top
          da.SetData(i++,
            new GH_UnitNumber(
              new Length(sqlValues[3], unit).ToUnit(_lengthUnit))); //Flange Thk Bottom
          da.SetData(i++,
            new GH_UnitNumber(
              new Length(sqlValues[2], unit).ToUnit(_lengthUnit))); //Web Thk Bottom
          da.SetData(i++,
            sqlValues.Count > 4 ?
              new GH_UnitNumber(new Length(sqlValues[4], unit).ToUnit(_lengthUnit)) :
              new GH_UnitNumber(
                Length.Zero.ToUnit(_lengthUnit))); // welded section don´t have a root radius
                                                   //Root radius
          da.SetData(i++, null); //Spacing
        }

        da.SetData(i, "CAT " + profile.Split(' ')[1]);
      } else {
        this.AddRuntimeError("Unable to get dimensions for type " + type[0]);
      }
    }

    private void SetOutput(IGH_DataAccess da, int outputId, string outputValue, LengthUnit unit) {
      double val = double.Parse(outputValue, CultureInfo.InvariantCulture);
      var length = new Length(val, unit);
      da.SetData(outputId, new GH_UnitNumber(length.ToUnit(_lengthUnit)));
    }

    private void Update(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      Message = unit;
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }
  }
}
