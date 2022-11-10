using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to assemble and analyse a GSA model
  /// </summary>
  public class CreateModel : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("326aa021-10b3-45a0-8286-eefb3dc3e2e1");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateModel;
    public CreateModel() : base("Create Model",
      "Model",
      "Assemble a GSA Model",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat0())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaModelParameter(), "Model(s)", "GSA", "Existing GSA Model(s) to append to" + System.Environment.NewLine +
          "If you input more than one model they will be merged" + System.Environment.NewLine + "with first model in list taking priority for IDs"
          , GH_ParamAccess.list);
      pManager.AddGenericParameter("Properties", "Pro", "GSA Sections (PB), Prop2Ds (PA) and Prop3Ds (PV) to add/set in the model" + System.Environment.NewLine +
          "Properties already added to Elements or Members" + System.Environment.NewLine + "will automatically be added with Geometry input", GH_ParamAccess.list);
      pManager.AddGenericParameter("GSA Geometry in [" + Length.GetAbbreviation(this.LengthUnit) + "]", "Geo", "GSA Nodes, Element1Ds, Element2Ds, Member1Ds, Member2Ds and Member3Ds to add/set in model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaLoadParameter(), "Load", "Ld", "Loads to add to the model" + System.Environment.NewLine + "You can also use this input to add Edited GridPlaneSurfaces", GH_ParamAccess.list);
      pManager.AddGenericParameter("Analysis Tasks & Combinations", "ΣT", "GSA Analysis Tasks and Combination Cases to add to the model", GH_ParamAccess.list);
      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaModelParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      #region GetData
      List<GsaModel> models = Helpers.Export.ModelAssembly.GetModels(this, DA, 0, true);

      Tuple<List<GsaSection>, List<GsaProp2d>, List<GsaProp3d>> props = Helpers.Export.ModelAssembly.GetProperties(this, DA, 1, true);
      List<GsaSection> sections = props.Item1;
      List<GsaProp2d> prop2Ds = props.Item2;
      List<GsaProp3d> prop3Ds = props.Item3;

      Tuple<List<GsaNode>, List<GsaElement1d>, List<GsaElement2d>, List<GsaElement3d>, List<GsaMember1d>, List<GsaMember2d>, List<GsaMember3d>> geo = Helpers.Export.ModelAssembly.GetGeometry(this, DA, 2, true);
      List<GsaNode> nodes = geo.Item1;
      List<GsaElement1d> elem1ds = geo.Item2;
      List<GsaElement2d> elem2ds = geo.Item3;
      List<GsaElement3d> elem3ds = geo.Item4;
      List<GsaMember1d> mem1ds = geo.Item5;
      List<GsaMember2d> mem2ds = geo.Item6;
      List<GsaMember3d> mem3ds = geo.Item7;

      Tuple<List<GsaLoad>, List<GsaGridPlaneSurface>> loading = Helpers.Export.ModelAssembly.GetLoading(this, DA, 3, true);
      List<GsaLoad> loads = loading.Item1;
      List<GsaGridPlaneSurface> gridPlaneSurfaces = loading.Item2;

      Tuple<List<GsaAnalysisTask>, List<GsaCombinationCase>> analysis = Helpers.Export.ModelAssembly.GetAnalysis(this, DA, 4, true);
      List<GsaAnalysisTask> analysisTasks = analysis.Item1;
      List<GsaCombinationCase> combinationCases = analysis.Item2;

      // manually add a warning if no input is set, as all inputs are optional
      if (models == null & nodes == null & elem1ds == null & elem2ds == null &
          mem1ds == null & mem2ds == null & mem3ds == null & sections == null
          & prop2Ds == null & loads == null & gridPlaneSurfaces == null)
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameters failed to collect data");
        return;
      }
      #endregion

      GsaModel model = new GsaModel();
      if (models != null)
      {
        if (models.Count > 0)
        {
          if (models.Count > 1)
            model = Util.Gsa.ToGSA.Models.MergeModel(models);
          else
            model = models[0].Clone();
        }
      }

      model.Model = Util.Gsa.ToGSA.Assemble.AssembleModel(model, nodes, elem1ds, elem2ds, elem3ds, mem1ds, mem2ds, mem3ds, sections, prop2Ds, prop3Ds, loads, gridPlaneSurfaces, analysisTasks, combinationCases, LengthUnit);

      if (this.ReMesh)
        model.Model.CreateElementsFromMembers();

      DA.SetData(0, new GsaModelGoo(model));
    }

    #region Custom UI
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;
    private List<string> CheckboxTexts = new List<string>() { "ElemsFromMems" };
    private List<bool> InitialCheckState = new List<bool>() { true };
    private bool ReMesh = true;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Unit",
          "Settings"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Length
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      this.SelectedItems.Add(Length.GetAbbreviation(this.LengthUnit));

      this.IsInitialised = true;
    }
    public override void CreateAttributes()
    {
      if (!IsInitialised)
        InitialiseDropdowns();
      m_attributes = new OasysGH.UI.DropDownCheckBoxesComponentAttributes(this, this.SetSelected, this.DropDownItems, this.SelectedItems, SetAnalysis, this.InitialCheckState, this.CheckboxTexts, this.SpacerDescriptions);
    }
    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[i]);
      base.UpdateUI();
    }
    public void SetAnalysis(List<bool> value)
    {
      this.ReMesh = value[0];
    }
    
    public override void UpdateUIFromSelectedItems()
    {
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      Params.Input[2].Name = "GSA Geometry in [" + Length.GetAbbreviation(this.LengthUnit) + "]";
    }
    #endregion

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetBoolean("ReMesh", ReMesh);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      this.ReMesh = reader.GetBoolean("ReMesh");
      return base.Read(reader);
    }
    #endregion
  }
}
