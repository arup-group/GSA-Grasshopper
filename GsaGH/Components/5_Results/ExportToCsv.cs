using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using GsaGH.Components.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;
using Newtonsoft.Json.Linq;
using OasysGH;
using OasysGH.Components;
using OasysGH.UI;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.UI;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to export GSA result values to .csv
  /// </summary>
  public class ExportToCsv : GH_OasysDropDownComponent {
    public const char ListSeparator = ',';

    private enum ResultType {
      BeamDerivedStresses,
      BeamDisplacements,
      BeamForcesAndMoments,
      BeamStrainEnergyDensity,
      BeamStresses,
      Element2dDisplacements,
      Element2dForcesAndMoments,
      Elememt2dStresses,
      Element3dDisplacements,
      Elememt3dStresses,
      FootfallResults,
      GlobalPerformanceResults,
      Member1dDisplacements,
      Member1dForcesAndMoments,
      NodeDisplacements,
      ReactionForces,
      SpringReactionForces,
      TotalLoadsAndReactions
    }

    public override Guid ComponentGuid => new Guid("c30084b1-a1e5-493c-ab5e-8b7b66b52c3d");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => null;
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private MomentUnit _momentUnit = DefaultUnits.MomentUnit;
    private string _fileNameLastSaved;
    private bool _saveInputOverride = false;

    private readonly IReadOnlyDictionary<ResultType, string> _resultTypes
      = new Dictionary<ResultType, string> {
        { ResultType.BeamDerivedStresses, "Major" },
        { ResultType.BeamDisplacements, "Minor" },
        { ResultType.BeamForcesAndMoments, "LT" },
        { ResultType.BeamStrainEnergyDensity, "LT" },
        { ResultType.BeamStresses, "BeamStresses" },
        { ResultType.Element2dDisplacements, "Element2dDisplacements" },
        { ResultType.Element2dForcesAndMoments, "Element2dForcesAndMoments" },
        { ResultType.Elememt2dStresses, "Elememt2dStresses" },
        { ResultType.Element3dDisplacements, "Element3dDisplacements" },
        { ResultType.Elememt3dStresses, "Elememt3dStresses" },
        { ResultType.FootfallResults, "FootfallResults" },
        { ResultType.GlobalPerformanceResults, "GlobalPerformanceResults" },
        { ResultType.Member1dDisplacements, "Member1dDisplacements" },
        { ResultType.Member1dForcesAndMoments, "Member1dForcesAndMoments" },
        { ResultType.NodeDisplacements, "NodeDisplacements" },
        { ResultType.ReactionForces, "ReactionForces" },
        { ResultType.SpringReactionForces, "SpringReactionForces" },
        { ResultType.TotalLoadsAndReactions, "TotalLoadsAndReactions" }
      };

    public ExportToCsv() : base("Export results to .csv", "ExportResults",
      "Export results as comma separated values", CategoryName.Name(), SubCategoryName.Cat5()) {
      Hidden = true;
    }

    //public override void CreateAttributes() {
    //  m_attributes = new ThreeButtonComponentAttributes(this, "Save", "Save As", "Open in GSA",
    //    SaveButtonClick, SaveAsButtonClick, OpenGsaExe, true, "Save GSA file");
    //}

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      switch (i) {
        case 1:
          _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[i]);
          break;

        case 2:
          _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), _selectedItems[i]);
          break;
      }

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string forceunitAbbreviation = Force.GetAbbreviation(_forceUnit);
      string momentunitAbbreviation = Moment.GetAbbreviation(_momentUnit);
      int i = 0;

    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Max/Min",
        "Force Unit",
        "Moment Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(ExtremaHelper.Vector6InternalForces.ToList());
      _selectedItems.Add(_dropDownItems[0][0]);

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force));
      _selectedItems.Add(Force.GetAbbreviation(_forceUnit));

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Moment));
      _selectedItems.Add(Moment.GetAbbreviation(_momentUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaElementListParameter());
      pManager[1].Optional = true;
      pManager.AddIntegerParameter("Intermediate Points", "nP",
        "Number of intermediate equidistant points (default 3)", GH_ParamAccess.item, 3);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string forceunitAbbreviation = Force.GetAbbreviation(_forceUnit);
      string momentunitAbbreviation = Moment.GetAbbreviation(_momentUnit);

      pManager.AddTextParameter("Foo", "", "", GH_ParamAccess.item);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      // posthog event?!


      GsaResult result;
      string elementlist = "All";
      var ghDivisions = new GH_Integer();
      da.GetData(2, ref ghDivisions);
      GH_Convert.ToInt32(ghDivisions, out int positionsCount, GH_Conversion.Both);
      positionsCount = Math.Abs(positionsCount) + 2; // taken absolute value and add 2 end points.

      double increment = 100.0 / (positionsCount - 1);
      var positions = new List<string>();
      for (int i = 0; i < positionsCount; i++) {
        positions.Add(new Ratio(increment * i, RatioUnit.Percent).ToString());
      }

      var ghTypes = new List<GH_ObjectWrapper>();
      da.GetDataList(0, ghTypes);

      string docPath = @"C:\Temp";
      using var outputFile = new StreamWriter(Path.Combine(docPath, "test.csv"));

      string forceAbr = Force.GetAbbreviation(_forceUnit);
      string momentAbr = Moment.GetAbbreviation(_momentUnit);

      var builder = new StringBuilder();
      builder.Append("Elem");
      builder.Append(ListSeparator);
      builder.Append("Case");
      builder.Append(ListSeparator);
      builder.Append("Pos");
      builder.Append(ListSeparator);
      builder.Append("Fx [");
      builder.Append(forceAbr);
      builder.Append(']');
      builder.Append(ListSeparator);
      builder.Append("Fy [");
      builder.Append(forceAbr);
      builder.Append(']'); builder.Append(ListSeparator);
      builder.Append("Fz [");
      builder.Append(forceAbr);
      builder.Append(']'); builder.Append(ListSeparator);
      builder.Append("|Fxyz| [");
      builder.Append(forceAbr);
      builder.Append(']'); builder.Append(ListSeparator);
      builder.Append("Mx [");
      builder.Append(momentAbr);
      builder.Append(']'); builder.Append(ListSeparator);
      builder.Append("My [");
      builder.Append(momentAbr);
      builder.Append(']'); builder.Append(ListSeparator);
      builder.Append("Mz [");
      builder.Append(momentAbr);
      builder.Append(']'); builder.Append(ListSeparator);
      builder.Append("|Mxyz| [");
      builder.Append(momentAbr);
      builder.Append(']'); builder.Append(ListSeparator);
      outputFile.WriteLine(builder.ToString());

      foreach (GH_ObjectWrapper ghTyp in ghTypes) {
        result = Inputs.GetResultInput(this, ghTyp);
        if (result == null) {
          return;
        }

        elementlist = Inputs.GetElementListDefinition(this, da, 1, result.Model);
        ReadOnlyCollection<int> elementIds = result.ElementIds(elementlist, 1);
        IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet =
          result.Element1dInternalForces.ResultSubset(elementIds, positionsCount);

        List<int> permutations = result.SelectedPermutationIds ?? new List<int>() {
          1,
        };
        if (permutations.Count == 1 && permutations[0] == -1) {
          permutations = Enumerable.Range(1, resultSet.Subset.Values.First().Count).ToList();
        }

        if (_selectedItems[0] == ExtremaHelper.Vector6Displacements[0]) {
          foreach (KeyValuePair<int, IList<IEntity1dQuantity<IInternalForce>>> kvp in resultSet.Subset) {
            foreach (int p in permutations) {
              var path = new GH_Path(result.CaseId, result.SelectedPermutationIds == null ? 0 : p, kvp.Key);

              int position = 0;
              foreach (IInternalForce force in kvp.Value[p - 1].Results.Values) {
                string value = BuildInternalForce(kvp.Key, result.CaseId, positions[position], force);
                outputFile.WriteLine(value);
                position++;
              }
            }
          }
        } else {
          Entity1dExtremaKey key = ExtremaHelper.InternalForceExtremaKey(resultSet, _selectedItems[0]);
          IInternalForce extrema = resultSet.GetExtrema(key);
          int perm = result.CaseType == CaseType.AnalysisCase ? 0 : 1;

          string value = BuildInternalForce(key.Id, result.CaseId, "", extrema);
          outputFile.WriteLine(value);
        }
      }

    }

    private string BuildInternalForce(int elementId, int caseId, string position, IInternalForce force) {
      var builder = new StringBuilder();
      builder.Append(elementId);
      builder.Append(ListSeparator);
      builder.Append(caseId);
      builder.Append(ListSeparator);
      builder.Append(position);
      builder.Append(ListSeparator);
      builder.Append(force.X.ToUnit(_forceUnit).Value);
      builder.Append(ListSeparator);
      builder.Append(force.Y.ToUnit(_forceUnit).Value);
      builder.Append(ListSeparator);
      builder.Append(force.Z.ToUnit(_forceUnit).Value);
      builder.Append(ListSeparator);
      builder.Append(force.Xyz.ToUnit(_forceUnit).Value);
      builder.Append(ListSeparator);
      builder.Append(force.Xx.ToUnit(_momentUnit).Value);
      builder.Append(ListSeparator);
      builder.Append(force.Yy.ToUnit(_momentUnit).Value);
      builder.Append(ListSeparator);
      builder.Append(force.Zz.ToUnit(_momentUnit).Value);
      builder.Append(ListSeparator);
      builder.Append(force.Xxyyzz.ToUnit(_momentUnit).Value);
      builder.Append(ListSeparator);
      return builder.ToString();
    }

    protected override void UpdateUIFromSelectedItems() {
      if (_selectedItems.Count == 2) {
        _spacerDescriptions.Insert(0, "Max/Min");
        _dropDownItems.Insert(0, ExtremaHelper.Vector6InternalForces.ToList());
        _selectedItems.Insert(0, _dropDownItems[0][0]);
      }

      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[1]);
      _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), _selectedItems[2]);
      base.UpdateUIFromSelectedItems();
    }

    private void OpenGsaExe() {
      if (string.IsNullOrEmpty(_fileNameLastSaved)) {
        Params.Input[0].CollectData();
        var tempModel = (GsaModelGoo)Params.Input[0].VolatileData.AllData(true).First();
        string tempPath = Path.GetTempPath() + tempModel.Value.Guid.ToString() + ".gwa";
        GsaModel gsaModel = tempModel.Value;
        Save(ref gsaModel, tempPath);
      }

      Process.Start(_fileNameLastSaved);
    }

    private void Save(ref GsaModel model, string fileNameAndPath) {
      if (!fileNameAndPath.EndsWith(".csv")) {
        fileNameAndPath += ".csv";
      }

      Directory.CreateDirectory(Path.GetDirectoryName(fileNameAndPath) ?? string.Empty);

      string mes = model.Model.SaveAs(fileNameAndPath).ToString();
      if (mes == GsaAPI.ReturnValue.GS_OK.ToString()) {
        _fileNameLastSaved = fileNameAndPath;

        //PostHog.ModelIO(GsaGH.PluginInfo.Instance, $"save{fileNameAndPath.Substring(fileNameAndPath.LastIndexOf('.') + 1).ToUpper()}",
        //  (int)(new FileInfo(fileNameAndPath).Length / 1024));

        model.FileNameAndPath = fileNameAndPath;
      } else {
        this.AddRuntimeError(mes);
      }
    }

    private void SaveButtonClick() {
      if (string.IsNullOrEmpty(_fileNameLastSaved)) {
        SaveAsButtonClick();
        return;
      }

      _saveInputOverride = true;
    }

    private void SaveAsButtonClick() {
      var fdi = new SaveFileDialog {
        Filter = "CSV (*.csv)|*.csv|Comma delimited",
      };
      bool res = fdi.ShowSaveDialog();
      if (!res) {
        return;
      }

      while (Params.Input[2].Sources.Count > 0) {
        OnPingDocument().RemoveObject(Params.Input[2].Sources[0], false);
      }

      var panel = new GH_Panel();
      panel.CreateAttributes();

      panel.Attributes.Pivot
        = new PointF(
          Attributes.DocObject.Attributes.Bounds.Left - panel.Attributes.Bounds.Width - 40,
          Attributes.DocObject.Attributes.Bounds.Bottom - panel.Attributes.Bounds.Height);
      panel.UserText = fdi.FileName;
      OnPingDocument().AddObject(panel, false);

      Params.Input[2].AddSource(panel);
      Params.OnParametersChanged();
      ExpireSolution(true);
    }
  }
}
