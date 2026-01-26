using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
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
  ///   Component to get geometric dimensions of a section
  /// </summary>
  public class ProfileDimensions : GH_OasysComponent, IGH_VariableParameterComponent {
    public override Guid ComponentGuid => new Guid("98765d83-2b23-47c1-ad1d-201b5a2eed8b");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.ProfileDimensions;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;

    public ProfileDimensions() : base("Profile Dimensions", "PfDims",
      "Get GSA Section Dimensions", CategoryName.Name(), SubCategoryName.Cat1()) {
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

      string profile = section.ApiSection.Profile;
      if (string.IsNullOrEmpty(profile.Trim())) {
        this.AddRuntimeError("Profile not set in Section");
        return;
      }

      string[] parts = profile.Split(' ');

      LengthUnit unit = LengthUnit.Millimeter;
      string[] type = parts[1].Split('(', ')');
      if (type.Length > 1) {
        UnitParser parser = OasysUnitsSetup.Default.UnitParser;
        unit = parser.Parse<LengthUnit>(type[1]);
      }

      var profileActions = new Dictionary<Func<string, bool>, Action> {
        { p => profile.StartsWith("STD A"), () => SetOutputForAngleProfile(da, parts, unit, type) }, {
          p => profile.StartsWith("STD CH ") || profile.StartsWith("STD CH("),
          () => SetOutputForChannelProfile(da, parts, unit, type)
        },
        { p => profile.StartsWith("STD CHS"), () => SetOutputForCircleHollowProfile(da, parts, unit, type) }, {
          p => profile.StartsWith("STD C ") || profile.StartsWith("STD C("),
          () => SetOutputForCircleProfile(da, parts, unit, type)
        },
        { p => profile.StartsWith("STD X"), () => SetOutputForICruciformSymmetricalProfile(da, parts, unit, type) },
        { p => profile.StartsWith("STD OVAL"), () => SetOutputForIEllipseHollowProfile(da, parts, unit, type) },
        { p => profile.StartsWith("STD E"), () => SetOutputForIEllipseProfile(da, parts, unit, type) },
        { p => profile.StartsWith("STD GC"), () => SetOutputForIGeneralCProfile(da, parts, unit, type) },
        { p => profile.StartsWith("STD GZ"), () => SetOutputForIGeneralZProfile(da, parts, unit, type) },
        { p => profile.StartsWith("STD GI"), () => SetOutputForIIBeamAsymmetricalProfile(da, parts, unit, type) },
        { p => profile.StartsWith("STD CB"), () => SetOutputForIIBeamCellularProfile(da, parts, unit, type) },
        { p => profile.StartsWith("STD I"), () => SetOutputForIIBeamSymmetricalProfile(da, parts, unit, type) },
        { p => profile.StartsWith("STD RHS"), () => SetOutputForIRectangleHollowProfile(da, parts, unit, type) }, {
          p => profile.StartsWith("STD R ") || profile.StartsWith("STD R("),
          () => SetOutputForIRectangleProfile(da, parts, unit, type)
        },
        { p => profile.StartsWith("STD RE"), () => SetOutputForIRectoEllipseProfile(da, parts, unit, type) },
        { p => profile.StartsWith("STD SP"), () => SetOutputForISecantPileProfile(da, parts, unit, profile, type) },
        { p => profile.StartsWith("STD SHT"), () => SetOutputForISheetPileProfile(da, parts, unit, type) },
        { p => profile.StartsWith("STD RC"), () => SetOutputForIStadiumProfile(da, parts, unit, type) },
        { p => profile.StartsWith("STD TR"), () => SetOutputForITrapezoidProfile(da, parts, unit, type) },
        { p => profile.StartsWith("STD T"), () => SetOutputForITSectionProfile(da, parts, unit, type) },
        { p => profile.StartsWith("CAT"), () => unit = SetOutputForCATProfile(da, profile) },
      };

      Action action = profileActions.FirstOrDefault(pair => pair.Key(profile)).Value;

      if (action != null) {
        action.Invoke();
      } else {
        this.AddRuntimeError("Unable to get dimensions for type " + type[0]);
      }
    }

    private LengthUnit SetOutputForCATProfile(IGH_DataAccess da, string profile) {
      LengthUnit unit;
      string prof = profile.Split(' ')[2];
      List<double> sqlValues = SqlReader.Instance.GetCatalogueProfileValues(prof,
        Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"));
      unit = LengthUnit.Meter;
      switch (sqlValues.Count) {
        case 2:
          SetOutputForCAT2ValuesProfile(da, sqlValues, unit);
          break;
        case 3:
          SetOutputForCAT3ValuesProfile(da, sqlValues, unit);
          break;
        default:
          SetOutputForCATProfile(da, sqlValues, unit);
          break;
      }

      da.SetData(9, "CAT " + profile.Split(' ')[1]);
      return unit;
    }

    private void SetOutputForCATProfile(IGH_DataAccess da, List<double> sqlValues, LengthUnit unit) {
      int i = 0;

      double?[] values = {
        sqlValues[0], // Depth
        sqlValues[1], // Width
        sqlValues[1], // Width Top
        sqlValues[1], // Width Bottom
        sqlValues[3], // Flange Thk Top
        sqlValues[3], // Flange Thk Bottom
        sqlValues[2], // Web Thk Bottom
        sqlValues.Count > 4 ? sqlValues[4] : (double?)0, // Root radius
        null, // Spacing
      };

      SetUnitOutputs(da, unit, values, ref i);
    }

    private void SetOutputForCAT3ValuesProfile(IGH_DataAccess da, List<double> sqlValues, LengthUnit unit) {
      int i = 0;

      double?[] values = {
        sqlValues[0], // Depth
        sqlValues[1], // Width
        sqlValues[1], // Width Top
        sqlValues[1], // Width Bottom
        null, // Flange Thk Top
        null, // Flange Thk Bottom
        sqlValues[2], // Web Thk Bottom
        sqlValues.Count > 4 ? sqlValues[4] : (double?)0, // Root radius
        null, // Spacing
      };

      SetUnitOutputs(da, unit, values, ref i);
    }

    private void SetOutputForCAT2ValuesProfile(IGH_DataAccess da, List<double> sqlValues, LengthUnit unit) {
      int i = 0;

      double?[] values = {
        sqlValues[0], // Depth
        sqlValues[1], // Width
        null, // Width Top
        null, // Width Bottom
        null, // Flange Thk Top
        null, // Flange Thk Bottom
        sqlValues[1], // Web Thk Bottom
        null, // Root radius
        null, // Spacing
      };

      SetUnitOutputs(da, unit, values, ref i);
    }

    private void SetOutputForITSectionProfile(IGH_DataAccess da, string[] parts, LengthUnit unit, string[] type) {
      int i = 0;
      string[] values = {
        parts[2], //Depth
        parts[3], //Width
        parts[3], //Width Top
        parts[4], //Width Bottom
        parts[5], //Flange Thk Top
        null, //Flange Thk Bottom
        parts[4], //Web Thk Bottom
        null, //Root radius
        null, //Spacing
      };
      SetUnitOutputs(da, unit, values, ref i);
      da.SetData(i, type[0]);
    }

    private void SetOutputForITrapezoidProfile(IGH_DataAccess da, string[] parts, LengthUnit unit, string[] type) {
      int i = 0;

      double top = double.Parse(parts[3], CultureInfo.InvariantCulture);
      double bottom = double.Parse(parts[4], CultureInfo.InvariantCulture);
      var width = new Length(Math.Max(top, bottom), unit);

      string[] values = {
        parts[2], // Depth
        null, // Placeholder for Width, 
        parts[3], // Width Top
        parts[4], // Width Bottom
        null, // Flange Thk Top
        null, // Flange Thk Bottom
        null, // Web Thk Bottom
        null, // Root radius
        null, // Spacing
      };

      SetUnitOutputs(da, unit, values, ref i);

      da.SetData(1, new GH_UnitNumber(width.ToUnit(_lengthUnit)));
      da.SetData(i, type[0]);
    }

    private void SetOutputForIStadiumProfile(IGH_DataAccess da, string[] parts, LengthUnit unit, string[] type) {
      int i = 0;
      string[] values = {
        parts[2], //Depth
        parts[3], //Width
        null, //Width Top
        null, //Width Bottom
        null, //Flange Thk Top
        null, //Flange Thk Bottom
        null, //Web Thk Bottom
        null, //Root radius
        null, //Spacing
      };

      SetUnitOutputs(da, unit, values, ref i);
      da.SetData(i, type[0]);
    }

    private void SetOutputForISheetPileProfile(IGH_DataAccess da, string[] parts, LengthUnit unit, string[] type) {
      int i = 0;
      string[] values = {
        parts[2], //Depth
        parts[3], //Width
        parts[4], //Width Top
        parts[5], //Width Bottom
        parts[6], //Flange Thk Top
        parts[6], //Flange Thk Bottom
        parts[7], //Web Thk Bottom
        null, //Root radius
        null, //Spacing
      };

      SetUnitOutputs(da, unit, values, ref i);

      da.SetData(i, type[0]);
    }

    private void SetOutputForISecantPileProfile(
      IGH_DataAccess da, string[] parts, LengthUnit unit, string profile, string[] type) {
      int i = 0;
      double width = 0;
      int count = int.Parse(parts[4], CultureInfo.InvariantCulture);
      double spacing = double.Parse(parts[3], CultureInfo.InvariantCulture);

      if (profile.StartsWith("STD SPW")) {
        // STD SPW 250 100 4
        width = count * spacing;
      } else {
        // STD SP 250 100 4
        double diameter = double.Parse(parts[2], CultureInfo.InvariantCulture);
        width = ((count - 1) * spacing) + diameter;
      }

      string[] values = {
        parts[2], //Depth
        null, //Width
        null, //Width Top
        null, //Width Bottom
        null, //Flange Thk Top
        null, //Flange Thk Bottom
        null, //Web Thk Bottom
        null, //Root radius
        parts[3], //Spacing
      };

      SetUnitOutputs(da, unit, values, ref i);
      SetOutput(da, 1, width, unit);
      da.SetData(i, type[0]);
    }

    private void SetOutputForIRectoEllipseProfile(IGH_DataAccess da, string[] parts, LengthUnit unit, string[] type) {
      int i = 0;
      string[] values = {
        parts[2], //Depth
        parts[4], //Width
        parts[3], //Width Top
        parts[5], //Width Bottom
        null, //Flange Thk Top
        null, //Flange Thk Bottom
        null, //Web Thk Bottom
        null, //Root radius
        null, //Spacing
      };

      SetUnitOutputs(da, unit, values, ref i);
      da.SetData(i, type[0]);
    }

    private void SetOutputForIRectangleProfile(IGH_DataAccess da, string[] parts, LengthUnit unit, string[] type) {
      int i = 0;
      string[] values = {
        parts[2], //Depth/Diameter
        parts[3], //Width
        parts[3], //Width Top
        parts[3], //Width Bottom
        null, //Flange Thk Top
        null, //Flange Thk Bottom
        null, //Web Thk Bottom
        null, //Root radius
        null, //Spacing
      };

      SetUnitOutputs(da, unit, values, ref i);
      da.SetData(i, type[0]);
    }

    private void SetOutputForIRectangleHollowProfile(
      IGH_DataAccess da, string[] parts, LengthUnit unit, string[] type) {
      SetOutputForAngleProfile(da, parts, unit, type);
    }

    private void SetOutputForIIBeamSymmetricalProfile(
      IGH_DataAccess da, string[] parts, LengthUnit unit, string[] type) {
      SetOutputForAngleProfile(da, parts, unit, type);
    }

    private void SetOutputForIIBeamCellularProfile(IGH_DataAccess da, string[] parts, LengthUnit unit, string[] type) {
      int i = 0;
      string[] values = {
        parts[2], //Depth/Diameter
        parts[3], //Width
        parts[3], //Width Top
        parts[3], //Width Bottom
        parts[5], //Flange Thk Top
        parts[5], //Flange Thk Bottom
        parts[4], //Web Thk Bottom
        parts[6], //hole size
        parts[7], //pitch
      };

      SetUnitOutputs(da, unit, values, ref i);
      da.SetData(i, type[0]);
    }

    private void SetOutputForIIBeamAsymmetricalProfile(
      IGH_DataAccess da, string[] parts, LengthUnit unit, string[] type) {
      int i = 0;
      double top = double.Parse(parts[3], CultureInfo.InvariantCulture);
      double bottom = double.Parse(parts[4], CultureInfo.InvariantCulture);

      string[] values = {
        parts[2], //Depth
        null, //Width
        parts[3], //Width Top
        parts[4], //Width Bottom
        parts[6], //Flange Thk Top
        parts[7], //Flange Thk Bottom
        parts[5], //Web Thk Bottom
        null, //Root radius
        null, //Spacing
      };

      SetUnitOutputs(da, unit, values, ref i);
      SetOutput(da, 1, Math.Max(top, bottom), unit); //Width

      da.SetData(i, type[0]);
    }

    private void SetOutputForIGeneralZProfile(IGH_DataAccess da, string[] parts, LengthUnit unit, string[] type) {
      int i = 0;
      double top = double.Parse(parts[3], CultureInfo.InvariantCulture);
      double bottom = double.Parse(parts[4], CultureInfo.InvariantCulture);
      double width = top + bottom;

      string[] values = {
        parts[2], //Depth
        null, //Width
        parts[3], //Width Top
        parts[4], //Width Bottom
        parts[5], //Flange Thk Top
        parts[6], //Flange Thk Bottom
        parts[7], //Web Thk Bottom
        null, //Root radius
        null, //Spacing
      };
      SetUnitOutputs(da, unit, values, ref i);
      SetOutput(da, 1, width, unit); //Width

      da.SetData(i, type[0]);
    }

    private void SetOutputForIGeneralCProfile(IGH_DataAccess da, string[] parts, LengthUnit unit, string[] type) {
      int i = 0;
      string[] values = {
        parts[2], //Depth
        parts[3], //Width
        parts[3], //Width Top
        parts[3], //Width Bottom
        parts[4], //Flange Thk Top
        parts[4], //Flange Thk Bottom
        parts[5], //Web Thk Bottom
        null, //Root radius
        null, //Spacing
      };
      SetUnitOutputs(da, unit, values, ref i);
      da.SetData(i, type[0]);
    }

    private void SetOutputForIEllipseProfile(IGH_DataAccess da, string[] parts, LengthUnit unit, string[] type) {
      SetOutputForIRectangleProfile(da, parts, unit, type);
    }

    private void SetOutputForIEllipseHollowProfile(IGH_DataAccess da, string[] parts, LengthUnit unit, string[] type) {
      int i = 0;
      string[] values = {
        parts[2], //Depth
        parts[3], //Width
        parts[3], //Width Top
        parts[3], //Width Bottom
        parts[4], //Flange Thk Top
        parts[4], //Flange Thk Bottom
        parts[4], //Web Thk Bottom
        null, //Root radius
        null, //Spacing
      };
      SetUnitOutputs(da, unit, values, ref i);
      da.SetData(i, type[0]);
    }

    private void SetOutputForICruciformSymmetricalProfile(
      IGH_DataAccess da, string[] parts, LengthUnit unit, string[] type) {
      int i = 0;
      string[] values = {
        parts[2], //Depth
        parts[3], //Width
        parts[3], //Width Top
        parts[3], //Width Bottom
        parts[5], //Flange Thk Top
        parts[5], //Flange Thk Bottom
        parts[4], //Web Thk Bottom
        null, //Root radius
        null, //Spacing
      };

      SetUnitOutputs(da, unit, values, ref i);
      da.SetData(i, type[0]);
    }

    private void SetOutputForCircleProfile(IGH_DataAccess da, string[] parts, LengthUnit unit, string[] type) {
      int i = 0;
      string[] values = {
        parts[2], //Depth
        null, //Width
        null, //Width Top
        null, //Width Bottom
        null, //Flange Thk Top
        null, //Flange Thk Bottom
        null, //Web Thk Bottom
        null, //Root radius
        null, //Spacing
      };

      SetUnitOutputs(da, unit, values, ref i);
      da.SetData(i, type[0]);
    }

    private void SetOutputForCircleHollowProfile(IGH_DataAccess da, string[] parts, LengthUnit unit, string[] type) {
      int i = 0;
      string[] values = {
        parts[2], //Depth
        null, //Width
        null, //Width Top
        null, //Width Bottom
        null, //Flange Thk Top
        null, //Flange Thk Bottom
        parts[3], //Web Thk Bottom
        null, //Root radius
        null, //Spacing
      };
      SetUnitOutputs(da, unit, values, ref i);
      da.SetData(i, type[0]);
    }

    private void SetOutputForAngleProfile(IGH_DataAccess da, string[] parts, LengthUnit unit, string[] type) {
      int i = 0;

      string[] values = {
        parts[2], //Depth
        parts[3], //Width
        parts[3], //Width Top
        parts[3], //Width Bottom
        parts[5], //Flange Thk Top
        parts[5], //Flange Thk Bottom
        parts[4], //Web Thk Bottom
        null, //Root radius
        null, //Spacing
      };
      SetUnitOutputs(da, unit, values, ref i);
      da.SetData(i, type[0]);
    }

    private void SetOutputForChannelProfile(IGH_DataAccess da, string[] parts, LengthUnit unit, string[] type) {
      SetOutputForAngleProfile(da, parts, unit, type);
    }

    internal void Update(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      Message = unit;
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }

    private void SetOutput(IGH_DataAccess da, int outputId, string outputValue, LengthUnit unit) {
      double val = double.Parse(outputValue, CultureInfo.InvariantCulture);
      var length = new Length(val, unit);
      da.SetData(outputId, new GH_UnitNumber(length.ToUnit(_lengthUnit)));
    }

    private void SetOutput(IGH_DataAccess da, int outputId, double value, LengthUnit unit) {
      var length = new Length(value, unit);
      da.SetData(outputId, new GH_UnitNumber(length.ToUnit(_lengthUnit)));
    }

    private void SetUnitOutputs(IGH_DataAccess da, LengthUnit unit, IList<double?> values, ref int index) {
      foreach (double? value in values) {
        if (value.HasValue) {
          SetOutput(da, index++, value.Value, unit);
        } else {
          da.SetData(index++, null);
        }
      }
    }

    private void SetUnitOutputs(IGH_DataAccess da, LengthUnit unit, IList<string> values, ref int index) {
      foreach (string value in values) {
        if (!string.IsNullOrEmpty(value)) {
          SetOutput(da, index++, value, unit);
        } else {
          da.SetData(index++, null);
        }
      }
    }

  }
}
