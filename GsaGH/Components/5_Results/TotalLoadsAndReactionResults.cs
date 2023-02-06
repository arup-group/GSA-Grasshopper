using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaAPI;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to get GSA total model load and reactions
    /// </summary>
    public class TotalLoadsAndReactionResults : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("00a195ef-b8f2-4b91-ac47-a8ae12d48b8e");
    public override GH_Exposure Exposure => GH_Exposure.septenary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.TotalLoadAndReaction;

    public TotalLoadsAndReactionResults() : base("Total Loads & Reactions",
      "TotalResults",
      "Get Total Loads and Reaction Results from a GSA model",
      CategoryName.Name(),
      SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      string forceunitAbbreviation = Force.GetAbbreviation(this.ForceUnit);
      string momentunitAbbreviation = Moment.GetAbbreviation(this.MomentUnit);

      pManager.AddGenericParameter("Total Force X [" + forceunitAbbreviation + "]", "ΣFx", "Sum of all Force Loads in GSA Model in X-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Force Y [" + forceunitAbbreviation + "]", "ΣFy", "Sum of all Force Loads in GSA Model in Y-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Force Z [" + forceunitAbbreviation + "]", "ΣFz", "Sum of all Force Loads in GSA Model in Z-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Force |XYZ| [" + forceunitAbbreviation + "]", "Σ|F|", "Sum of all Force Loads in GSA Model", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Moment XX [" + momentunitAbbreviation + "]", "ΣMxx", "Sum of all Moment Loads in GSA Model around X-axis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Moment XX  [" + momentunitAbbreviation + "]", "ΣMyy", "Sum of all Moment Loads in GSA Model around Y-axis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Moment XX  [" + momentunitAbbreviation + "]", "ΣMzz", "Sum of all Moment Loads in GSA Model around Z-axis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Moment |XXYYZZ|  [" + momentunitAbbreviation + "]", "Σ|M|", "Sum of all Moment Loads in GSA Model", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Reaction X [" + forceunitAbbreviation + "]", "ΣRx", "Sum of all Reaction Forces in GSA Model in X-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Reaction Y [" + forceunitAbbreviation + "]", "ΣRy", "Sum of all Reaction Forces in GSA Model in Y-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Reaction Z [" + forceunitAbbreviation + "]", "ΣRz", "Sum of all Reaction Forces in GSA Model in Z-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Reaction |XYZ| [" + forceunitAbbreviation + "]", "Σ|Rf|", "Sum of all Reaction Forces in GSA Model", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Reaction XX [" + momentunitAbbreviation + "]", "ΣRxx", "Sum of all Reaction Moments in GSA Model around X-axis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Reaction XX  [" + momentunitAbbreviation + "]", "ΣRyy", "Sum of all Reaction Moments in GSA Model around Y-axis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Reaction XX  [" + momentunitAbbreviation + "]", "ΣRzz", "Sum of all Reaction Moments in GSA Model around Z-axis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Reaction |XXYYZZ|  [" + momentunitAbbreviation + "]", "Σ|Rm|", "Sum of all Reaction Moments in GSA Model", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // Result to work on
      GsaResult result = new GsaResult();

      // Get Model
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        #region Inputs
        if (gh_typ == null || gh_typ.Value == null)
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input is null");
          return;
        }
        if (gh_typ.Value is GsaResultGoo)
        {
          result = ((GsaResultGoo)gh_typ.Value).Value;
          if (result.Type == GsaResult.CaseType.Combination)
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Global Result only available for Analysis Cases");
            return;
          }
        }
        else
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Result");
          return;
        }
        #endregion

        #region Get results from GSA
        // ### Get results ###
        //Get analysis case from model
        AnalysisCaseResult analysisCaseResult = result.AnalysisCaseResult;
        #endregion
        int i = 0;
        GsaResultQuantity f = ResultHelper.GetQuantityResult(analysisCaseResult.Global.TotalLoad, ForceUnit);
        DA.SetData(i++, new GH_UnitNumber(f.X));
        DA.SetData(i++, new GH_UnitNumber(f.Y));
        DA.SetData(i++, new GH_UnitNumber(f.Z));
        DA.SetData(i++, new GH_UnitNumber(f.XYZ));

        GsaResultQuantity m = ResultHelper.GetQuantityResult(analysisCaseResult.Global.TotalLoad, MomentUnit);
        DA.SetData(i++, new GH_UnitNumber(m.X));
        DA.SetData(i++, new GH_UnitNumber(m.Y));
        DA.SetData(i++, new GH_UnitNumber(m.Z));
        DA.SetData(i++, new GH_UnitNumber(m.XYZ));

        GsaResultQuantity rf = ResultHelper.GetQuantityResult(analysisCaseResult.Global.TotalReaction, ForceUnit);
        DA.SetData(i++, new GH_UnitNumber(rf.X));
        DA.SetData(i++, new GH_UnitNumber(rf.Y));
        DA.SetData(i++, new GH_UnitNumber(rf.Z));
        DA.SetData(i++, new GH_UnitNumber(rf.XYZ));

        GsaResultQuantity rm = ResultHelper.GetQuantityResult(analysisCaseResult.Global.TotalReaction, MomentUnit);
        DA.SetData(i++, new GH_UnitNumber(rm.X));
        DA.SetData(i++, new GH_UnitNumber(rm.Y));
        DA.SetData(i++, new GH_UnitNumber(rm.Z));
        DA.SetData(i++, new GH_UnitNumber(rm.XYZ));

        Helpers.PostHog.Result(result.Type, -1, "Global", "TotalLoadsAndReactions");
      }
    }

    #region Custom UI
    private ForceUnit ForceUnit = DefaultUnits.ForceUnit;
    private MomentUnit MomentUnit = DefaultUnits.MomentUnit;
    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Force Unit", "Moment Unit"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // force
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force));
      this.SelectedItems.Add(Force.GetAbbreviation(this.ForceUnit));

      // moment
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Moment));
      this.SelectedItems.Add(Moment.GetAbbreviation(this.MomentUnit));

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      if (i == 0)
        this.ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), this.SelectedItems[i]);
      else if (i == 1)
        this.MomentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), this.SelectedItems[i]);
      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems()
    {
      this.ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), this.SelectedItems[0]);
      this.MomentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), this.SelectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string forceunitAbbreviation = Force.GetAbbreviation(this.ForceUnit);
      string momentunitAbbreviation = Moment.GetAbbreviation(this.MomentUnit);
      int i = 0;
      Params.Output[i++].Name = "Force X [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Force Y [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Force Z [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Force |XYZ| [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment XX [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment YY [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment ZZ [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment |XXYYZZ| [" + momentunitAbbreviation + "]";
    }
    #endregion
  }
}
