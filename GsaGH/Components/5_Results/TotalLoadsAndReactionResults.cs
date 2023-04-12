using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers;
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
  ///   Component to get GSA total model load and reactions
  /// </summary>
  public class TotalLoadsAndReactionResults : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("00a195ef-b8f2-4b91-ac47-a8ae12d48b8e");
    public override GH_Exposure Exposure => GH_Exposure.septenary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    public TotalLoadsAndReactionResults() : base("Total Loads & Reactions",
      "TotalResults",
      "Get Total Loads and Reaction Results from a GSA model",
      CategoryName.Name(),
      SubCategoryName.Cat5())
      => Hidden = true;

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      switch (i) {
        case 0:
          _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[i]);
          break;

        case 1:
          _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), _selectedItems[i]);
          break;
      }

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string forceunitAbbreviation = Force.GetAbbreviation(_forceUnit);
      string momentunitAbbreviation = Moment.GetAbbreviation(_momentUnit);
      int i = 0;
      Params.Output[i++]
        .Name = "Force X [" + forceunitAbbreviation + "]";
      Params.Output[i++]
        .Name = "Force Y [" + forceunitAbbreviation + "]";
      Params.Output[i++]
        .Name = "Force Z [" + forceunitAbbreviation + "]";
      Params.Output[i++]
        .Name = "Force |XYZ| [" + forceunitAbbreviation + "]";
      Params.Output[i++]
        .Name = "Moment XX [" + momentunitAbbreviation + "]";
      Params.Output[i++]
        .Name = "Moment YY [" + momentunitAbbreviation + "]";
      Params.Output[i++]
        .Name = "Moment ZZ [" + momentunitAbbreviation + "]";
      Params.Output[i]
        .Name = "Moment |XXYYZZ| [" + momentunitAbbreviation + "]";
    }

    protected override Bitmap Icon => Resources.TotalLoadAndReaction;
    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Force Unit",
        "Moment Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force));
      _selectedItems.Add(Force.GetAbbreviation(_forceUnit));

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Moment));
      _selectedItems.Add(Moment.GetAbbreviation(_momentUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager)
      => pManager.AddParameter(new GsaResultsParameter(),
        "Result",
        "Res",
        "GSA Result",
        GH_ParamAccess.item);

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string forceunitAbbreviation = Force.GetAbbreviation(_forceUnit);
      string momentunitAbbreviation = Moment.GetAbbreviation(_momentUnit);

      pManager.AddGenericParameter("Total Force X [" + forceunitAbbreviation + "]",
        "ΣFx",
        "Sum of all Force Loads in GSA Model in X-direction",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Force Y [" + forceunitAbbreviation + "]",
        "ΣFy",
        "Sum of all Force Loads in GSA Model in Y-direction",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Force Z [" + forceunitAbbreviation + "]",
        "ΣFz",
        "Sum of all Force Loads in GSA Model in Z-direction",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Force |XYZ| [" + forceunitAbbreviation + "]",
        "Σ|F|",
        "Sum of all Force Loads in GSA Model",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Moment XX [" + momentunitAbbreviation + "]",
        "ΣMxx",
        "Sum of all Moment Loads in GSA Model around X-axis",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Moment XX  [" + momentunitAbbreviation + "]",
        "ΣMyy",
        "Sum of all Moment Loads in GSA Model around Y-axis",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Moment XX  [" + momentunitAbbreviation + "]",
        "ΣMzz",
        "Sum of all Moment Loads in GSA Model around Z-axis",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Moment |XXYYZZ|  [" + momentunitAbbreviation + "]",
        "Σ|M|",
        "Sum of all Moment Loads in GSA Model",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Reaction X [" + forceunitAbbreviation + "]",
        "ΣRx",
        "Sum of all Reaction Forces in GSA Model in X-direction",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Reaction Y [" + forceunitAbbreviation + "]",
        "ΣRy",
        "Sum of all Reaction Forces in GSA Model in Y-direction",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Reaction Z [" + forceunitAbbreviation + "]",
        "ΣRz",
        "Sum of all Reaction Forces in GSA Model in Z-direction",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Reaction |XYZ| [" + forceunitAbbreviation + "]",
        "Σ|Rf|",
        "Sum of all Reaction Forces in GSA Model",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Reaction XX [" + momentunitAbbreviation + "]",
        "ΣRxx",
        "Sum of all Reaction Moments in GSA Model around X-axis",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Reaction XX  [" + momentunitAbbreviation + "]",
        "ΣRyy",
        "Sum of all Reaction Moments in GSA Model around Y-axis",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Reaction XX  [" + momentunitAbbreviation + "]",
        "ΣRzz",
        "Sum of all Reaction Moments in GSA Model around Z-axis",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Reaction |XXYYZZ|  [" + momentunitAbbreviation + "]",
        "Σ|Rm|",
        "Sum of all Reaction Moments in GSA Model",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var result = new GsaResult();
      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp))
        return;

      #region Inputs

      switch (ghTyp?.Value) {
        case null:
          this.AddRuntimeWarning("Input is null");
          return;

        case GsaResultGoo goo: {
            result = goo.Value;
            if (result.Type == GsaResult.CaseType.Combination) {
              this.AddRuntimeError("Global Result only available for Analysis Cases");
              return;
            }

            break;
          }
        default:
          this.AddRuntimeError("Error converting input to GSA Result");
          return;
      }

      #endregion

      #region Get results from GSA

      AnalysisCaseResult analysisCaseResult = result.AnalysisCaseResult;

      #endregion

      int i = 0;
      GsaResultQuantity f
        = ResultHelper.GetQuantityResult(analysisCaseResult.Global.TotalLoad, _forceUnit);
      da.SetData(i++, new GH_UnitNumber(f.X));
      da.SetData(i++, new GH_UnitNumber(f.Y));
      da.SetData(i++, new GH_UnitNumber(f.Z));
      da.SetData(i++, new GH_UnitNumber(f.Xyz));

      GsaResultQuantity m
        = ResultHelper.GetQuantityResult(analysisCaseResult.Global.TotalLoad, _momentUnit);
      da.SetData(i++, new GH_UnitNumber(m.X));
      da.SetData(i++, new GH_UnitNumber(m.Y));
      da.SetData(i++, new GH_UnitNumber(m.Z));
      da.SetData(i++, new GH_UnitNumber(m.Xyz));

      GsaResultQuantity rf
        = ResultHelper.GetQuantityResult(analysisCaseResult.Global.TotalReaction, _forceUnit);
      da.SetData(i++, new GH_UnitNumber(rf.X));
      da.SetData(i++, new GH_UnitNumber(rf.Y));
      da.SetData(i++, new GH_UnitNumber(rf.Z));
      da.SetData(i++, new GH_UnitNumber(rf.Xyz));

      GsaResultQuantity rm
        = ResultHelper.GetQuantityResult(analysisCaseResult.Global.TotalReaction, _momentUnit);
      da.SetData(i++, new GH_UnitNumber(rm.X));
      da.SetData(i++, new GH_UnitNumber(rm.Y));
      da.SetData(i++, new GH_UnitNumber(rm.Z));
      da.SetData(i, new GH_UnitNumber(rm.Xyz));

      PostHog.Result(result.Type, -1, "Global", "TotalLoadsAndReactions");
    }

    protected override void UpdateUIFromSelectedItems() {
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[0]);
      _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), _selectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }

    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private MomentUnit _momentUnit = DefaultUnits.MomentUnit;
  }
}
