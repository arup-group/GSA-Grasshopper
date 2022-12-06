using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.GUI;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
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
      CategoryName.Name(),
      SubCategoryName.Cat0())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaModelParameter(), "Model(s)", "GSA", "Existing GSA Model(s) to append to" + Environment.NewLine +
          "If you input more than one model they will be merged" + Environment.NewLine + "with first model in list taking priority for IDs"
          , GH_ParamAccess.list);
      pManager.AddGenericParameter("Properties", "Pro", "GSA Sections (PB), Prop2Ds (PA) and Prop3Ds (PV) to add/set in the model" + Environment.NewLine +
          "Properties already added to Elements or Members" + Environment.NewLine + "will automatically be added with Geometry input", GH_ParamAccess.list);
      pManager.AddGenericParameter("GSA Geometry in [" + Length.GetAbbreviation(this.LengthUnit) + "]", "Geo", "GSA Nodes, Element1Ds, Element2Ds, Member1Ds, Member2Ds and Member3Ds to add/set in model", GH_ParamAccess.list);
      pManager.AddGenericParameter("Load", "Ld", "Loads to add to the model" + Environment.NewLine + "You can also use this input to add Edited GridPlaneSurfaces", GH_ParamAccess.list);
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
      List<GsaModel> models = Helpers.Export.GetInputsForModelAssembly.GetModels(this, DA, 0, true);

      Tuple<List<GsaSection>, List<GsaProp2d>, List<GsaProp3d>> props = Helpers.Export.GetInputsForModelAssembly.GetProperties(this, DA, 1, true);
      List<GsaSection> sections = props.Item1;
      List<GsaProp2d> prop2Ds = props.Item2;
      List<GsaProp3d> prop3Ds = props.Item3;

      Tuple<List<GsaNode>, List<GsaElement1d>, List<GsaElement2d>, List<GsaElement3d>, List<GsaMember1d>, List<GsaMember2d>, List<GsaMember3d>> geo = Helpers.Export.GetInputsForModelAssembly.GetGeometry(this, DA, 2, true);
      List<GsaNode> nodes = geo.Item1;
      List<GsaElement1d> elem1ds = geo.Item2;
      List<GsaElement2d> elem2ds = geo.Item3;
      List<GsaElement3d> elem3ds = geo.Item4;
      List<GsaMember1d> mem1ds = geo.Item5;
      List<GsaMember2d> mem2ds = geo.Item6;
      List<GsaMember3d> mem3ds = geo.Item7;

      Tuple<List<GsaLoad>, List<GsaGridPlaneSurface>> loading = Helpers.Export.GetInputsForModelAssembly.GetLoading(this, DA, 3, true);
      List<GsaLoad> loads = loading.Item1;
      List<GsaGridPlaneSurface> gridPlaneSurfaces = loading.Item2;

      Tuple<List<GsaAnalysisTask>, List<GsaCombinationCase>> analysis = Helpers.Export.GetInputsForModelAssembly.GetAnalysis(this, DA, 4, true);
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
            model = Helpers.Export.MergeModels.MergeModel(models);
          else
            model = models[0].Clone();
        }
      }

      model.Model = Helpers.Export.AssembleModel.Assemble(model, nodes, elem1ds, elem2ds, elem3ds, mem1ds, mem2ds, mem3ds, sections, prop2Ds, prop3Ds, loads, gridPlaneSurfaces, analysisTasks, combinationCases, this.LengthUnit, this._tolerance, this.ReMesh);

      this.UpdateMessage();

      DA.SetData(0, new GsaModelGoo(model));
    }

    #region Custom UI
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;
    private List<string> CheckboxTexts = new List<string>() { "ElemsFromMems" };
    private List<bool> InitialCheckState = new List<bool>() { true };
    private bool ReMesh = true;
    private double _tolerance = DefaultUnits.Tolerance.Meters;
    private string _toleranceTxt = "";

    protected override void BeforeSolveInstance()
    {
      base.BeforeSolveInstance();
      this.UpdateMessage();
    }

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

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
    {
      Menu_AppendSeparator(menu);

      ToolStripTextBox tolerance = new ToolStripTextBox();
      _toleranceTxt = new Length(_tolerance, this.LengthUnit).ToString();
      tolerance.Text = _toleranceTxt;
      tolerance.TextChanged += (s, e) => MaintainText(tolerance);

      ToolStripMenuItem toleranceMenu = new ToolStripMenuItem("Set Tolerance", Properties.Resources.Units);
      toleranceMenu.Enabled = true;
      toleranceMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;

      GH_MenuCustomControl menu2 = new GH_MenuCustomControl(toleranceMenu.DropDown, tolerance.Control, true, 200);
      toleranceMenu.DropDownItems[1].MouseUp += (s, e) =>
      {
        this.UpdateMessage();
        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
        ExpireSolution(true);
      };
      menu.Items.Add(toleranceMenu);

      Menu_AppendSeparator(menu);

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }
    private void MaintainText(ToolStripTextBox tolerance)
    {
      _toleranceTxt = tolerance.Text;
      if (Length.TryParse(_toleranceTxt, out Length res))
        tolerance.BackColor = System.Drawing.Color.FromArgb(255, 180, 255, 150);
      else
        tolerance.BackColor = System.Drawing.Color.FromArgb(255, 255, 100, 100);
    }
    private void UpdateMessage()
    {
      if (this._toleranceTxt != "")
      {
        try
        {
          Length newTolerance = Length.Parse(_toleranceTxt);
          _tolerance = newTolerance.Meters;
        }
        catch (Exception e)
        {
          MessageBox.Show(e.Message);
          return;
        }
      }
      this.Message = "Tol: " + new Length(_tolerance, this.LengthUnit).ToString();
      if (_tolerance < 0.001)
        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Set tolerance is quite small, you can change this by right-clicking the component.");
      if (_tolerance > 0.25)
        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Set tolerance is quite large, you can change this by right-clicking the component.");
    }
    #endregion

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetBoolean("ReMesh", this.ReMesh);
      writer.SetDouble("Tolerance", this._tolerance);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      this.ReMesh = reader.GetBoolean("ReMesh");
      if (reader.ItemExists("Tolerance"))
        this._tolerance = reader.GetDouble("Tolerance");
      else
        this._tolerance = DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry);
      this.UpdateMessage();
      return base.Read(reader);
    }
    #endregion
  }
}
