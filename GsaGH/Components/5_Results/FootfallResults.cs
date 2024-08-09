using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using GsaGH.Components.Helpers;
using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  public class FootfallResults : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("224074e5-2fdc-48fa-a042-89a2c62f0c88");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.FootfallResults;

    public FootfallResults() : base("Footfall Results", "Footfall",
      "Node Resonant or Transient Footfall result values", CategoryName.Name(), SubCategoryName.Cat5()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Type",
        "Max/Min",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(new List<string>(new[] {
        "Resonant",
        "Transient",
      }));
      _selectedItems.Add(_dropDownItems[0][0]);

      _dropDownItems.Add(ExtremaHelper.Footfall.ToList());
      _selectedItems.Add(_dropDownItems[1][0]);

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaNodeListParameter());
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string hz = Frequency.GetAbbreviation(FrequencyUnit.Hertz);
      string mps2 = Acceleration.GetAbbreviation(AccelerationUnit.MeterPerSecondSquared);
      string mps = Speed.GetAbbreviation(SpeedUnit.MeterPerSecond);
      string note = ResultNotes.NoteNodeResults;

      pManager.AddNumberParameter("Maximum Response Factor", "RF",
        "The maximum response factor." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Peak Velocity [" + mps + "]", "PkV",
        "The peak velocity." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("RMS Velocity [" + mps + "]", "RmV",
        "The root mean square (rms) veloocity." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Peak Acceleration [" + mps + "]", "PkA",
        "The peak velocity." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("RMS Acceleration [" + mps + "]", "RmA",
        "The root mean square (rms) acceleration." + note, GH_ParamAccess.tree);
      pManager.AddIntegerParameter("Critical Node", "CN",
        "The node ID of the critical frequency" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Critical Frequency [" + hz + "]", "Cf",
        "The critical frequency" + note, GH_ParamAccess.tree);
      pManager.AddIntegerParameter("Node IDs", "ID", "Node IDs for each result value",
        GH_ParamAccess.tree);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      GsaResult result;
      string nodeList = "All";

      var ghTypes = new List<GH_ObjectWrapper>();
      da.GetDataList(0, ghTypes);

      var rf = new DataTree<double>();
      var peakVelo = new DataTree<GH_UnitNumber>();
      var rmsVelo = new DataTree<GH_UnitNumber>();
      var peakAcc = new DataTree<GH_UnitNumber>();
      var rmsAcc = new DataTree<GH_UnitNumber>();
      var critId = new DataTree<int>();
      var critfreq = new DataTree<GH_UnitNumber>();
      var outIDs = new DataTree<int>();

      foreach (GH_ObjectWrapper ghTyp in ghTypes) {
        result = Inputs.GetResultInput(this, ghTyp);
        if (result == null) {
          return;
        }

        if (result.CaseType == CaseType.CombinationCase) {
          this.AddRuntimeError("Footfall Result only available for Analysis Cases");
          return;
        }

        nodeList = Inputs.GetNodeListDefinition(this, da, 1, result.Model);
        ReadOnlyCollection<int> nodeIds = result.NodeIds(nodeList);
        IEntity0dResultSubset<IFootfall, ResultFootfall<Entity0dExtremaKey>> resultSet
          = _selectedItems[0] == _dropDownItems[0][0]
          ? result.NodeResonantFootfalls.ResultSubset(nodeIds)
          : result.NodeTransientFootfalls.ResultSubset(nodeIds);

        if (resultSet.Ids.Count == 0) {
          this.AddRuntimeWarning($"{result.CaseType} {result.CaseId} contains no Footfall results.");
          return;
        }

        List<int> permutations = result.SelectedPermutationIds ?? new List<int>() {
          1,
        };
        if (permutations.Count == 1 && permutations[0] == -1) {
          permutations = Enumerable.Range(1, resultSet.Subset.Values.First().Count).ToList();
        }

        if (_selectedItems[1] == ExtremaHelper.Footfall[0]) {
          foreach (int id in resultSet.Ids) {
            foreach (int p in permutations) {
              var path = new GH_Path(result.CaseId, result.SelectedPermutationIds == null ? 0 : p);
              IFootfall res = resultSet.Subset[id][p - 1];
              rf.Add(res.MaximumResponseFactor, path);
              peakVelo.Add(new GH_UnitNumber(res.PeakVelocity), path);
              rmsVelo.Add(new GH_UnitNumber(res.RmsVelocity), path);
              peakAcc.Add(new GH_UnitNumber(res.PeakAcceleration), path);
              rmsAcc.Add(new GH_UnitNumber(res.RmsAcceleration), path);
              critId.Add(res.CriticalNode, path);
              critfreq.Add(new GH_UnitNumber(res.CriticalFrequency), path);
              outIDs.Add(id, path);
            }
          }
        } else {
          Entity0dExtremaKey key = ExtremaHelper.FootfallExtremaKey(resultSet, _selectedItems[1]);
          IFootfall extrema = resultSet.GetExtrema(key);
          int perm = result.CaseType == CaseType.AnalysisCase ? 0 : 1;
          var path = new GH_Path(result.CaseId, key.Permutation + perm);
          rf.Add(extrema.MaximumResponseFactor, path);
          peakVelo.Add(new GH_UnitNumber(extrema.PeakVelocity), path);
          rmsVelo.Add(new GH_UnitNumber(extrema.RmsVelocity), path);
          peakAcc.Add(new GH_UnitNumber(extrema.PeakAcceleration), path);
          rmsAcc.Add(new GH_UnitNumber(extrema.RmsAcceleration), path);
          critId.Add(extrema.CriticalNode, path);
          critfreq.Add(new GH_UnitNumber(extrema.CriticalFrequency), path);
          outIDs.Add(key.Id, path);
        }

        PostHog.Result(result.CaseType, 0, "Footfall");
      }

      da.SetDataTree(0, rf);
      da.SetDataTree(1, peakVelo);
      da.SetDataTree(2, rmsVelo);
      da.SetDataTree(3, peakAcc);
      da.SetDataTree(4, rmsAcc);
      da.SetDataTree(5, critId);
      da.SetDataTree(6, critfreq);
      da.SetDataTree(7, outIDs);
    }
  }
}
