using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysUnits;
using OasysUnits.Units;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create Effective Length properties for member 1d
  /// </summary>
  public class CreateEffectiveLength : GH_OasysDropDownComponent {
    private enum FoldMode {
      Automatic,
      InternalRestraints,
      UserSpecified,
    }
    public override Guid ComponentGuid => new Guid("3b63d584-5f61-4779-b576-14ab8682c1b9");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateEffectiveLength;
    private FoldMode _mode = FoldMode.Automatic;


    public CreateEffectiveLength() : base("Create Effective Length", "EffectiveLength",
      "Create 1D Member Design Properties for Effective Length, Restraints and Buckling Factors",
      CategoryName.Name(), SubCategoryName.Cat2()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      if (i == 0) {
        switch (_selectedItems[i]) {
          case "Automatic":
            if (_mode == FoldMode.Automatic) {
              return;
            }

            _mode = FoldMode.Automatic;
            break;

          case "InternalRestraints":
            if (_mode == FoldMode.InternalRestraints) {
              return;
            }

            _mode = FoldMode.InternalRestraints;
            break;

          case "UserSpecified":
            if (_mode == FoldMode.UserSpecified) {
              return;
            }

            _mode = FoldMode.UserSpecified;
            break;
        }
      }

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      var fLtb = (Param_Number)Params.Input[Params.Input.Count - 1];
      var fLz = (Param_Number)Params.Input[Params.Input.Count - 2];
      var fLy = (Param_Number)Params.Input[Params.Input.Count - 3];

      Param_String end1 = End1Restraint();
      Param_String end2 = End2Restraint();
      if (Params.Input.Count != 7 & Params.Input.Count != 4) { 
        end1 = (Param_String)Params.Input[1];
        end2 = (Param_String)Params.Input[2];
      }

      while (Params.Input.Count > 1) {
        Params.UnregisterInputParameter(Params.Input[1], false);
      }

      switch (_mode) {
        case FoldMode.Automatic:
          Params.RegisterInputParam(end1);
          Params.RegisterInputParam(end2);
          break;

        case FoldMode.InternalRestraints:
          Params.RegisterInputParam(end1);
          Params.RegisterInputParam(end2);
          Params.RegisterInputParam(InternalContinuousRestraint());
          Params.RegisterInputParam(InternalIntermediateRestraint());
          break;

        case FoldMode.UserSpecified:
          Params.RegisterInputParam(EffectiveLengthAboutYParam());
          Params.RegisterInputParam(EffectiveLengthAboutZParam());
          Params.RegisterInputParam(EffectiveLengthLtbParam());
          break;
      }

      Params.RegisterInputParam(fLy);
      Params.RegisterInputParam(fLz);
      Params.RegisterInputParam(fLtb);
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Lₑ calc. option",
        "Load position",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();


      _dropDownItems.Add(Enum.GetNames(typeof(FoldMode)).ToList());
      _selectedItems.Add(_dropDownItems[0][0]);

      _dropDownItems.Add(Enum.GetNames(typeof(LoadReference)).ToList());
      _selectedItems.Add(LoadReference.ShearCentre.ToString());

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddNumberParameter("Destabilising Load Height", "L",
        "Destabilising Load Height in model units", GH_ParamAccess.item, 0);
      pManager.AddNumberParameter("Factor Lsy", "fLy", "Moment Amplification Factor, Strong Axis",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Factor Lsz", "fLz", "Moment Amplification Factor, Weak Axis",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Equivalent uniform moment factor for LTB", "fLtb",
        $"Override the automatically calculated factor to account for the shape of the moment diagram in lateral torsional buckling design equations. This override is applied for all bending segments in the member.  This override is applied to the following variable for each design code:{Environment.NewLine} AISC 360: C_b {Environment.NewLine} AS 4100: alpha_m {Environment.NewLine} BS 5950: m_LT {Environment.NewLine} CSA S16: omega_2 {Environment.NewLine} EN 1993-1-1 and EN 1993-1-2: C_1 {Environment.NewLine} Hong Kong Code of Practice: m_LT {Environment.NewLine} IS 800: C_mLT {Environment.NewLine} SANS 10162-1: omega_2",
        GH_ParamAccess.item);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaEffectiveLengthParameter());
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var leff = new GsaEffectiveLength();
      string end1 = string.Empty;
      string end2 = string.Empty;
      switch (_mode) {
        case FoldMode.Automatic:
          var auto = new EffectiveLengthFromEndRestraintAndGeometry();
          if (da.GetData(1, ref end1)) {
            auto.End1 = MemberEndRestraintFactory.CreateFromStrings(end1);
          }

          if (da.GetData(2, ref end2)) {
            auto.End2 = MemberEndRestraintFactory.CreateFromStrings(end2);
          }

          leff.EffectiveLength = auto;
          break;

        case FoldMode.InternalRestraints:
          var internalRes = new EffectiveLengthFromEndAndInternalRestraint();
          if (da.GetData(1, ref end1)) {
            internalRes.End1 = MemberEndRestraintFactory.CreateFromStrings(end1);
          }

          if (da.GetData(2, ref end2)) {
            internalRes.End2 = MemberEndRestraintFactory.CreateFromStrings(end2);
          }

          string continous = string.Empty;
          if (da.GetData(3, ref continous)) {
            internalRes.RestraintAlongMember = InternalContinuousRestraint(continous);
          }

          string intermed = string.Empty;
          if (da.GetData(4, ref intermed)) {
            internalRes.RestraintAtBracedPoints = InternalIntermediateRestraint(intermed);
          }

          leff.EffectiveLength = internalRes;
          break;

        case FoldMode.UserSpecified:
          var specific = new EffectiveLengthFromUserSpecifiedValue();
          if (Params.Input[1].SourceCount > 0) {
            specific.EffectiveLengthAboutY = EffectiveLengthAttribute(
              Input.LengthOrRatio(this, da, 1, LengthUnit.Meter, true));
          }

          if (Params.Input[2].SourceCount > 0) {
            specific.EffectiveLengthAboutZ = EffectiveLengthAttribute(
              Input.LengthOrRatio(this, da, 2, LengthUnit.Meter, true));
          }

          if (Params.Input[3].SourceCount > 0) {
            specific.EffectiveLengthLaterialTorsional = EffectiveLengthAttribute(
              Input.LengthOrRatio(this, da, 3, LengthUnit.Meter, true));
          }

          leff.EffectiveLength = specific;
          break;
      }

      var fls = new GsaBucklingFactors();
      double? input = null;
      if (da.GetData(Params.Input.Count - 3, ref input)) {
        fls.MomentAmplificationFactorStrongAxis = input;
      }

      if (da.GetData(Params.Input.Count - 2, ref input)) {
        fls.MomentAmplificationFactorWeakAxis = input;
      }

      if (da.GetData(Params.Input.Count - 1, ref input)) {
        fls.EquivalentUniformMomentFactor = input;
      }

      leff.BucklingFactors = fls;

      if (da.GetData(0, ref input)) {
        leff.EffectiveLength.DestablisingLoad = (double)input;
      }

      leff.EffectiveLength.DestablisingLoadPositionRelativeTo
        = (LoadReference)Enum.Parse(typeof(LoadReference), _selectedItems[1]);

      da.SetData(0, new GsaEffectiveLengthGoo(leff));
    }

    private Param_GenericObject EffectiveLengthAboutYParam() {
      return new Param_GenericObject {
        Access = GH_ParamAccess.item,
        Name = "Effective Length About Y",
        NickName = "Lsy",
        Description = "Set the user-defined effective length about y in model units. " +
        "\nInput a negative number to set a relative (%) effective length (-0.5 = 50%).",
        Optional = true
      };
    }

    private Param_GenericObject EffectiveLengthAboutZParam() {
      return new Param_GenericObject {
        Access = GH_ParamAccess.item,
        Name = "Effective Length About Z",
        NickName = "Lsz",
        Description = "Set the user-defined effective length about z in model units. " +
        "\nInput a negative number to set a relative (%) effective length (-0.5 = 50%).",
        Optional = true
      };
    }

    private Param_GenericObject EffectiveLengthLtbParam() {
      return new Param_GenericObject {
        Access = GH_ParamAccess.item,
        Name = "Effective Length LTB",
        NickName = "Ltb",
        Description = "Set the user-defined effective length for lateral torsional buckling in " +
        "model units.\nInput a negative number to set a relative (%) effective length (-0.5 = 50%).",
        Optional = true
      };
    }

    private Param_String End1Restraint() {
      return new Param_String {
        Access = GH_ParamAccess.item,
        Name = "Member Restraint Start",
        NickName = "ER1",
        Description = "Set the Member's Start Restraint" +
        "\nUse either shortcut names ('Pinned', 'Fixed', 'Free'," +
        "\n'FullRotational', 'PartialRotational' or 'TopFlangeLateral')" +
        "\nor the " + MemberEndRestraintFactory.RestraintSyntax(),
        Optional = true
      };
    }

    private Param_String End2Restraint() {
      return new Param_String {
        Access = GH_ParamAccess.item,
        Name = "Member Restraint End",
        NickName = "ER2",
        Description = "Set the Member's End Restraint." +
        "\nUse either shortcut names ('Pinned', 'Fixed', 'Free'," +
        "\n'FullRotational', 'PartialRotational' or 'TopFlangeLateral')" +
        "\nor the " + MemberEndRestraintFactory.RestraintSyntax(),
        Optional = true
      };
    }

    private Param_String InternalContinuousRestraint() {
      return new Param_String {
        Access = GH_ParamAccess.item,
        Name = "Restraint Along Member",
        NickName = "RAM",
        Description = "Set the internal continous restraint along the member. " +
        "\nAccepted inputs are:" +
        "\n  Free (0)" +
        "\n  TopFlangeLateral (1)" +
        "\n  Pinned (2)",
        Optional = true
      };
    }

    private InternalContinuousRestraint InternalContinuousRestraint(string s) {
      if (s == "2") {
        return GsaAPI.InternalContinuousRestraint.Pinned;
      }

      if (Enum.TryParse(s, out InternalContinuousRestraint result)) {
        return result;
      }
      s = s.ToLower();

      if (s.Contains("0") || s.Contains("f")) {
        return GsaAPI.InternalContinuousRestraint.Free;
      }

      if (s.Contains("2") || s.Contains("p")) {
        return GsaAPI.InternalContinuousRestraint.Pinned;
      }

      if (s.Contains("1") || s.Contains("t")) {
        return GsaAPI.InternalContinuousRestraint.TopFlangeLateral;
      }

      throw new ArgumentException($"Unable to parse {s} to InternalContinuousRestraint");
    }

    private Param_String InternalIntermediateRestraint() {
      return new Param_String {
        Access = GH_ParamAccess.item,
        Name = "Intermediate Bracing Point Restraints",
        NickName = "IBR",
        Description = "Set the internal restraint at intermediate bracing points of the member. " +
        "\nAccepted inputs are:" +
        "\n  Free (0)" +
        "\n  TopFlangeLateral (1)" +
        "\n  BottomFlangeLateral (2)" +
        "\n  TopAndBottomFlangeLateral (3)",
        Optional = true
      };
    }

    private InternalIntermediateRestraint InternalIntermediateRestraint(string s) {
      if (s == "3") {
        return GsaAPI.InternalIntermediateRestraint.TopAndBottomFlangeLateral;
      }

      if (Enum.TryParse(s, true, out InternalIntermediateRestraint result)) {
        return result;
      }
      s = s.ToLower();

      if (s.Contains("0") || s.Contains("f")) {
        return GsaAPI.InternalIntermediateRestraint.Free;
      }

      if (s.Contains("3") || (s.Contains("top") & s.Contains("bot"))) {
        return GsaAPI.InternalIntermediateRestraint.TopAndBottomFlangeLateral;
      }

      if (s.Contains("1") || s.Contains("top")) {
        return GsaAPI.InternalIntermediateRestraint.TopFlangeLateral;
      }

      if (s.Contains("2") || s.Contains("bot")) {
        return GsaAPI.InternalIntermediateRestraint.BottomFlangeLateral;
      }

      throw new ArgumentException($"Unable to parse {s} to InternalContinuousRestraint");
    }

    private EffectiveLengthAttribute EffectiveLengthAttribute(IQuantity quantity) {
      switch (quantity) {
        case Length length:
          return new EffectiveLengthAttribute(EffectiveLengthOptionType.Absolute,
            length.Meters);
        
        case Ratio ratio:
          return new EffectiveLengthAttribute(EffectiveLengthOptionType.Relative,
            ratio.DecimalFractions);

        default:
          return null;
      }
    }
  }
}
