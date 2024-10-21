using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;

using GsaAPI;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;

using OasysUnits;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create Effective Length Options for a Member 1D
  /// </summary>
  public class CreateEffectiveLengthOptions : GH_OasysDropDownComponent {
    private enum FoldMode {
      Automatic,
      InternalRestraints,
      UserSpecified,
    }

    public override Guid ComponentGuid => new Guid("a477dee8-8ac6-4d3d-880c-4b2d6364d3c6");
    public override GH_Exposure Exposure => GH_Exposure.senary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateEffectiveLengthOptions;
    private FoldMode _mode = FoldMode.Automatic;
    private readonly IReadOnlyDictionary<FoldMode, string> _effectiveLengthOptions
      = new Dictionary<FoldMode, string> {
        { FoldMode.Automatic, "Automatic" },
        { FoldMode.InternalRestraints, "Internal restraints" },
        { FoldMode.UserSpecified, "User specified" },
      };
    private readonly IReadOnlyDictionary<LoadReference, string> _loadReferenceTypes
      = new Dictionary<LoadReference, string> {
        { LoadReference.ShearCentre, "Shear centre" },
        { LoadReference.TopFlange, "Top flange" },
        { LoadReference.BottomFlange, "Bottom flange" },
      };

    public CreateEffectiveLengthOptions() : base("Create Effective Length Options",
      "EffectiveLengthOptions",
      "Create 1D Member Design Options for Effective Length, Restraints and Buckling Factors",
      CategoryName.Name(), SubCategoryName.Cat2()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      FoldMode mode = GetModeBy(_selectedItems[0]);
      if (i == 0) {
        UpdateParameters(mode);
      }

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      UpdateParameters(_mode);
    }

    private void UpdateParameters(FoldMode mode) {
      if (mode == _mode) {
        return;
      }

      var fLtb = (Param_Number)Params.Input[Params.Input.Count - 1];
      var fLz = (Param_Number)Params.Input[Params.Input.Count - 2];
      var fLy = (Param_Number)Params.Input[Params.Input.Count - 3];
      var h = (Param_Number)Params.Input[Params.Input.Count - 4];

      Param_String end1 = End1Restraint();
      Param_String end2 = End2Restraint();
      if (Params.Input.Count != 7 && Params.Input.Count != 4) {
        end1 = (Param_String)Params.Input[0];
        end2 = (Param_String)Params.Input[1];
      }

      while (Params.Input.Count > 0) {
        Params.UnregisterInputParameter(Params.Input[0], false);
      }

      switch (mode) {
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

      Params.RegisterInputParam(h);
      Params.RegisterInputParam(fLy);
      Params.RegisterInputParam(fLz);
      Params.RegisterInputParam(fLtb);

      _mode = mode;
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Options",
        "Load position",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(_effectiveLengthOptions.Values.ToList());
      _selectedItems.Add(_dropDownItems[0][0]);

      _dropDownItems.Add(_loadReferenceTypes.Values.ToList());
      _selectedItems.Add(_loadReferenceTypes[0]);

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddTextParameter("Member Restraint Start", "ER1",
      "Set the Member's Start Restraint" +
      "\nUse either shortcut names ('Pinned', 'Fixed', 'Free'," +
      "\n'FullRotational', 'PartialRotational' or 'TopFlangeLateral')" +
      "\nor the " + MemberEndRestraintFactory.RestraintSyntax(), GH_ParamAccess.item);
      pManager.AddTextParameter("Member Restraint End", "ER2",
        "Set the Member's End Restraint." +
        "\nUse either shortcut names ('Pinned', 'Fixed', 'Free'," +
        "\n'FullRotational', 'PartialRotational' or 'TopFlangeLateral')" +
        "\nor the " + MemberEndRestraintFactory.RestraintSyntax(), GH_ParamAccess.item);
      pManager.AddNumberParameter("Destabilising Load Height", "h",
        "Destabilising Load Height in model units", GH_ParamAccess.item, 0);
      pManager.AddNumberParameter("Factor Lsy", "fLy", "Moment Amplification Factor, Strong Axis",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Factor Lsz", "fLz", "Moment Amplification Factor, Weak Axis",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Equivalent uniform moment factor for LTB", "fLtb",
        $"Override the automatically calculated factor to account for the shape of the moment diagram in lateral torsional buckling design equations. This override is applied for all bending segments in the member.  This override is applied to the following variable for each design code:{Environment.NewLine} AISC 360: C_b {Environment.NewLine} AS 4100: alpha_m {Environment.NewLine} BS 5950: m_LT {Environment.NewLine} CSA S16: omega_2 {Environment.NewLine} EN 1993-1-1 and EN 1993-1-2: C_1 {Environment.NewLine} Hong Kong Code of Practice: m_LT {Environment.NewLine} IS 800: C_mLT {Environment.NewLine} SANS 10162-1: omega_2",
        GH_ParamAccess.item);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
      pManager[5].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaEffectiveLengthOptionsParameter());
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var leff = new GsaEffectiveLengthOptions();
      string end1 = string.Empty;
      string end2 = string.Empty;
      int destablisingLoadIndex = 2;
      switch (_mode) {
        case FoldMode.Automatic:
          var auto = new EffectiveLengthFromEndRestraintAndGeometry();
          if (da.GetData(0, ref end1)) {
            auto.End1 = MemberEndRestraintFactory.CreateFromStrings(end1);
          }

          if (da.GetData(1, ref end2)) {
            auto.End2 = MemberEndRestraintFactory.CreateFromStrings(end2);
          }

          leff.EffectiveLength = auto;
          break;

        case FoldMode.InternalRestraints:
          destablisingLoadIndex = 4;
          var internalRes = new EffectiveLengthFromEndAndInternalRestraint();
          if (da.GetData(0, ref end1)) {
            internalRes.End1 = MemberEndRestraintFactory.CreateFromStrings(end1);
          }

          if (da.GetData(1, ref end2)) {
            internalRes.End2 = MemberEndRestraintFactory.CreateFromStrings(end2);
          }

          string continous = string.Empty;
          if (da.GetData(2, ref continous)) {
            internalRes.RestraintAlongMember = InternalContinuousRestraint(continous);
          }

          string intermed = string.Empty;
          if (da.GetData(3, ref intermed)) {
            internalRes.RestraintAtBracedPoints = InternalIntermediateRestraint(intermed);
          }

          leff.EffectiveLength = internalRes;
          break;

        case FoldMode.UserSpecified:
          destablisingLoadIndex = 3;
          var specific = new EffectiveLengthFromUserSpecifiedValue();
          if (Params.Input[0].SourceCount > 0) {
            specific.EffectiveLengthAboutY = EffectiveLengthAttribute(
              Input.LengthOrRatio(this, da, 0, LengthUnit.Meter, true));
          }

          if (Params.Input[1].SourceCount > 0) {
            specific.EffectiveLengthAboutZ = EffectiveLengthAttribute(
              Input.LengthOrRatio(this, da, 1, LengthUnit.Meter, true));
          }

          if (Params.Input[2].SourceCount > 0) {
            specific.EffectiveLengthLaterialTorsional = EffectiveLengthAttribute(
              Input.LengthOrRatio(this, da, 2, LengthUnit.Meter, true));
          }

          leff.EffectiveLength = specific;
          break;
      }

      double? input = null;
      if (da.GetData(destablisingLoadIndex, ref input)) {
        leff.EffectiveLength.DestablisingLoad = (double)input;
      }

      var fls = new GsaBucklingFactors();
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

      leff.EffectiveLength.DestablisingLoadPositionRelativeTo = GetLoadReferenceBy(_selectedItems[1]);

      da.SetData(0, new GsaEffectiveLengthOptionsGoo(leff));
    }

    protected override void UpdateUIFromSelectedItems() {
      FoldMode mode = GetModeBy(_selectedItems[0]);
      UpdateParameters(mode);

      base.UpdateUIFromSelectedItems();
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

    private FoldMode GetModeBy(string value) {
      foreach (KeyValuePair<FoldMode, string> item in _effectiveLengthOptions) {
        if (item.Value.Equals(value)) {
          return item.Key;
        }
      }
      throw new ArgumentException("Unable to convert " + value + " to Effective Length Options");
    }

    private LoadReference GetLoadReferenceBy(string value) {
      foreach (KeyValuePair<LoadReference, string> item in _loadReferenceTypes) {
        if (item.Value.Equals(value)) {
          return item.Key;
        }
      }
      throw new ArgumentException("Unable to convert " + value + " to Load Reference");
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

      if (s.Contains("0") || s.Contains("free")) {
        return GsaAPI.InternalContinuousRestraint.Free;
      }

      if (s.Contains("2") || s.Contains("pin")) {
        return GsaAPI.InternalContinuousRestraint.Pinned;
      }

      if (s.Contains("1") || s.Contains("top")) {
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
      return quantity switch {
        Length length => new EffectiveLengthAttribute(EffectiveLengthOptionType.Absolute,
                    length.Meters),
        Ratio ratio => new EffectiveLengthAttribute(EffectiveLengthOptionType.Relative,
                    ratio.DecimalFractions),
        _ => null,
      };
    }
  }
}
