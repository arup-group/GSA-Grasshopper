using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Util.GH;
using GsaGH.Util.Gsa;
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
  /// Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  public class TotalLoadsAndReactionResults : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("00a195ef-b8f2-4b91-ac47-a8ae12d48b8e");
    public override GH_Exposure Exposure => GH_Exposure.septenary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.TotalLoadAndReaction;

    public TotalLoadsAndReactionResults() : base("Total Loads & Reactions",
      "TotalResults",
      "Get Total Loads and Reaction Results from a GSA model",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    #endregion
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        dropdownitems.Add(FilteredUnits.FilteredForceUnits);
        dropdownitems.Add(FilteredUnits.FilteredMomentUnits);

        selecteditems = new List<string>();
        selecteditems.Add(DefaultUnits.ForceUnit.ToString());
        selecteditems.Add(DefaultUnits.MomentUnit.ToString());

        first = false;
      }

      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      switch (i)
      {
        case 0:
          forceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selecteditems[i]);
          break;
        case 1:
          momentUnit = (MomentUnit)Enum.Parse(typeof(MomentUnit), selecteditems[i]);
          break;

      }
        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    private void UpdateUIFromSelectedItems()
    {
      forceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selecteditems[0]);
      momentUnit = (MomentUnit)Enum.Parse(typeof(MomentUnit), selecteditems[1]);

      CreateAttributes();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    // list of lists with all dropdown lists conctent
    List<List<string>> dropdownitems;
    // list of selected items
    List<string> selecteditems;
    // list of descriptions 
    List<string> spacerDescriptions = new List<string>(new string[]
    {
            "Force Unit",
            "Moment Unit",
    });

    private ForceUnit forceUnit = DefaultUnits.ForceUnit;
    private MomentUnit momentUnit = DefaultUnits.MomentUnit;
    bool first = true;
    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Result", "Res", "GSA Result", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      string forceUnitAbbreviation = Force.GetAbbreviation(forceUnit);
      string momentunitAbbreviation = Moment.GetAbbreviation(momentUnit);

      pManager.AddGenericParameter("Total Force X [" + forceUnitAbbreviation + "]", "ΣFx", "Sum of all Force Loads in GSA Model in X-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Force Y [" + forceUnitAbbreviation + "]", "ΣFy", "Sum of all Force Loads in GSA Model in Y-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Force Z [" + forceUnitAbbreviation + "]", "ΣFz", "Sum of all Force Loads in GSA Model in Z-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Force |XYZ| [" + forceUnitAbbreviation + "]", "Σ|F|", "Sum of all Force Loads in GSA Model", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Moment XX [" + momentunitAbbreviation + "]", "ΣMxx", "Sum of all Moment Loads in GSA Model around X-axis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Moment XX  [" + momentunitAbbreviation + "]", "ΣMyy", "Sum of all Moment Loads in GSA Model around Y-axis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Moment XX  [" + momentunitAbbreviation + "]", "ΣMzz", "Sum of all Moment Loads in GSA Model around Z-axis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Moment |XXYYZZ|  [" + momentunitAbbreviation + "]", "Σ|M|", "Sum of all Moment Loads in GSA Model", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Reaction X [" + forceUnitAbbreviation + "]", "ΣRx", "Sum of all Reaction Forces in GSA Model in X-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Reaction Y [" + forceUnitAbbreviation + "]", "ΣRy", "Sum of all Reaction Forces in GSA Model in Y-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Reaction Z [" + forceUnitAbbreviation + "]", "ΣRz", "Sum of all Reaction Forces in GSA Model in Z-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Total Reaction |XYZ| [" + forceUnitAbbreviation + "]", "Σ|Rf|", "Sum of all Reaction Forces in GSA Model", GH_ParamAccess.item);
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
        if (gh_typ.Value is GsaResultGoo)
        {
          result = ((GsaResultGoo)gh_typ.Value).Value;
          if (result.Type == GsaResult.ResultType.Combination)
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
        GsaResultQuantity f = ResultHelper.GetQuantityResult(analysisCaseResult.Global.TotalLoad, forceUnit);
        DA.SetData(i++, new GH_UnitNumber(f.X));
        DA.SetData(i++, new GH_UnitNumber(f.Y));
        DA.SetData(i++, new GH_UnitNumber(f.Z));
        DA.SetData(i++, new GH_UnitNumber(f.XYZ));

        GsaResultQuantity m = ResultHelper.GetQuantityResult(analysisCaseResult.Global.TotalLoad, momentUnit);
        DA.SetData(i++, new GH_UnitNumber(m.X));
        DA.SetData(i++, new GH_UnitNumber(m.Y));
        DA.SetData(i++, new GH_UnitNumber(m.Z));
        DA.SetData(i++, new GH_UnitNumber(m.XYZ));

        GsaResultQuantity rf = ResultHelper.GetQuantityResult(analysisCaseResult.Global.TotalReaction, forceUnit);
        DA.SetData(i++, new GH_UnitNumber(rf.X));
        DA.SetData(i++, new GH_UnitNumber(rf.Y));
        DA.SetData(i++, new GH_UnitNumber(rf.Z));
        DA.SetData(i++, new GH_UnitNumber(rf.XYZ));

        GsaResultQuantity rm = ResultHelper.GetQuantityResult(analysisCaseResult.Global.TotalReaction, momentUnit);
        DA.SetData(i++, new GH_UnitNumber(rm.X));
        DA.SetData(i++, new GH_UnitNumber(rm.Y));
        DA.SetData(i++, new GH_UnitNumber(rm.Z));
        DA.SetData(i++, new GH_UnitNumber(rm.XYZ));
      }
    }
    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);

      first = false;
      UpdateUIFromSelectedItems();
      return base.Read(reader);
    }
    #endregion
    #region IGH_VariableParameterComponent null implementation

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
    {
      return null;
    }
    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
      string forceUnitAbbreviation = Force.GetAbbreviation(forceUnit);
      string momentunitAbbreviation = Moment.GetAbbreviation(momentUnit);

      int i = 0;
      Params.Output[i++].Name = "Total Force X [" + forceUnitAbbreviation + "]";
      Params.Output[i++].Name = "Total Force Y [" + forceUnitAbbreviation + "]";
      Params.Output[i++].Name = "Total Force Z [" + forceUnitAbbreviation + "]";
      Params.Output[i++].Name = "Total Force |XYZ| [" + forceUnitAbbreviation + "]";
      Params.Output[i++].Name = "Total Moment XX [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Total Moment YY [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Total Moment ZZ [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Total Moment |XXYYZZ| [" + momentunitAbbreviation + "]";

      Params.Output[i++].Name = "Total Reaction X [" + forceUnitAbbreviation + "]";
      Params.Output[i++].Name = "Total Reaction Y [" + forceUnitAbbreviation + "]";
      Params.Output[i++].Name = "Total Reaction Z [" + forceUnitAbbreviation + "]";
      Params.Output[i++].Name = "Total Reaction |XYZ| [" + forceUnitAbbreviation + "]";
      Params.Output[i++].Name = "Total Reaction XX [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Total Reaction YY [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Total Reaction ZZ [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Total Reaction |XXYYZZ| [" + momentunitAbbreviation + "]";
    }
    #endregion
  }
}

