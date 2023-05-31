using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers;
using GsaGH.Helpers.GH;
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
  ///   Component to get GSA node displacement values
  /// </summary>
  public class NodeDisplacement : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("83844063-3da9-4d96-95d3-ea39f96f3e2a");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.NodeDisplacement;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitResult;

    public NodeDisplacement() : base("Node Displacements", "NodeDisp",
      "Node Translation and Rotation result values", CategoryName.Name(), SubCategoryName.Cat5()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      int i = 0;
      Params.Output[i++].Name = "Translations X [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Translations Y [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Translations Z [" + unitAbbreviation + "]";
      Params.Output[i].Name = "Translations |XYZ| [" + unitAbbreviation + "]";
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.list);
      pManager.AddGenericParameter("Node filter list", "No",
        "Filter results by list (by default 'all')" + Environment.NewLine
        + "Input a GSA List or a text string taking the form:" + Environment.NewLine
        + " 1 11 to 72 step 2 not (XY3 31 to 45)" + Environment.NewLine
        + "Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item);
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      string note = Environment.NewLine + "DataTree organised as { CaseID ; Permutation } "
        + Environment.NewLine + "fx. {1;2} is Case 1, Permutation 2, where each branch "
        + Environment.NewLine + "contains a list matching the NodeIDs in the ID output.";

      pManager.AddGenericParameter("Translations X [" + unitAbbreviation + "]", "Ux",
        "Translations in X-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations Y [" + unitAbbreviation + "]", "Uy",
        "Translations in Y-direction in Global Axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations Z [" + unitAbbreviation + "]", "Uz",
        "Translations in Z-direction in Global Axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations |XYZ| [" + unitAbbreviation + "]", "|U|",
        "Combined |XYZ| Translations in Global Axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations XX [rad]", "Rxx",
        "Rotations around X-axis in Global Axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations YY [rad]", "Ryy",
        "Rotations around Y-axis in Global Axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations ZZ [rad]", "Rzz",
        "Rotations around Z-axis in Global Axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations |XYZ| [rad]", "|R|",
        "Combined |XXYYZZ| Rotations in Global Axis" + note, GH_ParamAccess.tree);
      pManager.AddTextParameter("Nodes IDs", "ID", "Node IDs for each result value",
        GH_ParamAccess.tree);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var result = new GsaResult();

      string nodeList = Inputs.GetNodeListNameForesults(this, da, 1);
      if (string.IsNullOrEmpty(nodeList)) {
        return;
      }

      var outTransX = new DataTree<GH_UnitNumber>();
      var outTransY = new DataTree<GH_UnitNumber>();
      var outTransZ = new DataTree<GH_UnitNumber>();
      var outTransXyz = new DataTree<GH_UnitNumber>();
      var outRotX = new DataTree<GH_UnitNumber>();
      var outRotY = new DataTree<GH_UnitNumber>();
      var outRotZ = new DataTree<GH_UnitNumber>();
      var outRotXyz = new DataTree<GH_UnitNumber>();
      var outIDs = new DataTree<int>();

      var ghTypes = new List<GH_ObjectWrapper>();
      if (!da.GetDataList(0, ghTypes)) {
        return;
      }

      foreach (GH_ObjectWrapper ghTyp in ghTypes) {
        switch (ghTyp?.Value) {
          case null:
            this.AddRuntimeWarning("Input is null");
            return;

          case GsaResultGoo goo:
            result = goo.Value;
            break;

          default:
            this.AddRuntimeError("Error converting input to GSA Result");
            return;
        }

        (List<GsaResultsValues> vals, List<int> sortedIDs)
          = result.NodeDisplacementValues(nodeList, _lengthUnit);

        List<int> permutations = result.SelectedPermutationIds ?? new List<int>() {
          1,
        };
        if (permutations.Count == 1 && permutations[0] == -1) {
          permutations = Enumerable.Range(1, vals.Count).ToList();
        }

        foreach (int perm in permutations) {
          var path = new GH_Path(result.CaseId, result.SelectedPermutationIds == null ? 0 : perm);

          var transX = new List<GH_UnitNumber>();
          var transY = new List<GH_UnitNumber>();
          var transZ = new List<GH_UnitNumber>();
          var transXyz = new List<GH_UnitNumber>();
          var rotX = new List<GH_UnitNumber>();
          var rotY = new List<GH_UnitNumber>();
          var rotZ = new List<GH_UnitNumber>();
          var rotXyz = new List<GH_UnitNumber>();
          var ids = new List<int>();

          Parallel.For(0, 2, item => // split into two tasks
          {
            switch (item) {
              case 0: {
                foreach (int id in sortedIDs) {
                  ids.Add(id);
                  ConcurrentDictionary<int, GsaResultQuantity> res = vals[perm - 1].XyzResults[id];
                  GsaResultQuantity values = res[0]; // there is only one result per node
                  transX.Add(
                    new GH_UnitNumber(
                      values.X.ToUnit(_lengthUnit))); // use ToUnit to capture changes in dropdown
                  transY.Add(new GH_UnitNumber(values.Y.ToUnit(_lengthUnit)));
                  transZ.Add(new GH_UnitNumber(values.Z.ToUnit(_lengthUnit)));
                  transXyz.Add(new GH_UnitNumber(values.Xyz.ToUnit(_lengthUnit)));
                }

                break;
              }
              case 1: {
                foreach (GsaResultQuantity values in sortedIDs
                 .Select(id => vals[perm - 1].XxyyzzResults[id]).Select(res => res[0])) {
                  rotX.Add(new GH_UnitNumber(values.X));
                  rotY.Add(new GH_UnitNumber(values.Y));
                  rotZ.Add(new GH_UnitNumber(values.Z));
                  rotXyz.Add(new GH_UnitNumber(values.Xyz));
                }

                break;
              }
            }
          });

          outTransX.AddRange(transX, path);
          outTransY.AddRange(transY, path);
          outTransZ.AddRange(transZ, path);
          outTransXyz.AddRange(transXyz, path);
          outRotX.AddRange(rotX, path);
          outRotY.AddRange(rotY, path);
          outRotZ.AddRange(rotZ, path);
          outRotXyz.AddRange(rotXyz, path);
          outIDs.AddRange(ids, path);
        }
      }

      da.SetDataTree(0, outTransX);
      da.SetDataTree(1, outTransY);
      da.SetDataTree(2, outTransZ);
      da.SetDataTree(3, outTransXyz);
      da.SetDataTree(4, outRotX);
      da.SetDataTree(5, outRotY);
      da.SetDataTree(6, outRotZ);
      da.SetDataTree(7, outRotXyz);
      da.SetDataTree(8, outIDs);

      PostHog.Result(result.Type, 0, GsaResultsValues.ResultType.Displacement);
    }

    protected override void UpdateUIFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
