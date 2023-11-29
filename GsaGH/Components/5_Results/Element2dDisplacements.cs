﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
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
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to GSA 2D Element displacement values
  /// </summary>
  public class Element2dDisplacements : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("22f87d33-4f9a-49d6-9f3d-b366e446a75f");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Element2dDisplacements;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitResult;

    public Element2dDisplacements() : base("Element 2D Displacements", "Disp2D",
      "2D Translation and Rotation result values", CategoryName.Name(), SubCategoryName.Cat5()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);
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
        "Envelope",
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(ExtremaHelper.Vector6Displacements.ToList());
      _selectedItems.Add(_dropDownItems[0][0]);

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaElementMemberListParameter());
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      string note = ResultNotes.Note2dResults;

      pManager.AddGenericParameter("Translations X [" + unitAbbreviation + "]", "Ux",
        "Translations in X-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations Y [" + unitAbbreviation + "]", "Uy",
        "Translations in Y-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations Z [" + unitAbbreviation + "]", "Uz",
        "Translations in Z-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations |XYZ| [" + unitAbbreviation + "]", "|U|",
        "Combined |XYZ| Translations." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations XX [rad]", "Rxx",
        "Rotations around X-axis in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations YY [rad]", "Ryy",
        "Rotations around Y-axis in Global Axiss." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations ZZ [rad]", "Rzz",
        "Rotations around Z-axis in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations |XYZ| [rad]", "|R|",
        "Combined |XXYYZZ| Rotations." + note, GH_ParamAccess.tree);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      GsaResult2 result;
      string elementlist = "All";

      var outTransX = new DataTree<GH_UnitNumber>();
      var outTransY = new DataTree<GH_UnitNumber>();
      var outTransZ = new DataTree<GH_UnitNumber>();
      var outTransXyz = new DataTree<GH_UnitNumber>();
      var outRotX = new DataTree<GH_UnitNumber>();
      var outRotY = new DataTree<GH_UnitNumber>();
      var outRotZ = new DataTree<GH_UnitNumber>();
      var outRotXyz = new DataTree<GH_UnitNumber>();

      var ghTypes = new List<GH_ObjectWrapper>();
      if (!da.GetDataList(0, ghTypes)) {
        return;
      }

      foreach (GH_ObjectWrapper ghTyp in ghTypes) {
        switch (ghTyp?.Value) {
          case GsaResultGoo goo:
            result = new GsaResult2((GsaResult)goo.Value);
            elementlist = Inputs.GetElementListDefinition(this, da, 1, result.Model);
            break;

          case null:
            this.AddRuntimeWarning("Input is null");
            return;

          default:
            this.AddRuntimeError("Error converting input to GSA Result");
            return;
        }

        ReadOnlyCollection<int> elementIds = result.ElementIds(elementlist, 2);
        IEntity2dResultSubset<IEntity2dQuantity<IDisplacement>, IDisplacement, ResultVector6<Entity2dExtremaKey>> resultSet =
          result.Element2dDisplacements.ResultSubset(elementIds);

        List<int> permutations = result.SelectedPermutationIds ?? new List<int>() {
          1,
        };
        if (permutations.Count == 1 && permutations[0] == -1) {
          permutations = Enumerable.Range(1, resultSet.Subset.Values.First().Count).ToList();
        }

        if (_selectedItems[0] == ExtremaHelper.Vector6Displacements[0]) {
          foreach (KeyValuePair<int, Collection<IEntity2dQuantity<IDisplacement>>> kvp in resultSet.Subset) {
            foreach (int p in permutations) {
              var path = new GH_Path(result.CaseId, result.SelectedPermutationIds == null ? 0 : p, kvp.Key);
              outTransX.AddRange(kvp.Value[p - 1].Results().Select(
                r => new GH_UnitNumber(r.X.ToUnit(_lengthUnit))), path);
              outTransY.AddRange(kvp.Value[p - 1].Results().Select(
                r => new GH_UnitNumber(r.Y.ToUnit(_lengthUnit))), path);
              outTransZ.AddRange(kvp.Value[p - 1].Results().Select(
                r => new GH_UnitNumber(r.Z.ToUnit(_lengthUnit))), path);
              outTransXyz.AddRange(kvp.Value[p - 1].Results().Select(
                r => new GH_UnitNumber(r.Xyz.ToUnit(_lengthUnit))), path);
              outRotX.AddRange(kvp.Value[p - 1].Results().Select(
                r => new GH_UnitNumber(r.Xx)), path);
              outRotY.AddRange(kvp.Value[p - 1].Results().Select(
                r => new GH_UnitNumber(r.Yy)), path);
              outRotZ.AddRange(kvp.Value[p - 1].Results().Select(
                r => new GH_UnitNumber(r.Zz)), path);
              outRotXyz.AddRange(kvp.Value[p - 1].Results().Select(
                r => new GH_UnitNumber(r.Xxyyzz)), path);
            }
          }
        } else {
          Entity2dExtremaKey key = ExtremaHelper.DisplacementExtremaKey(resultSet, _selectedItems[0]);
          IDisplacement extrema = resultSet.GetExtrema(key);
          int perm = result.CaseType == CaseType.AnalysisCase ? 0 : 1;
          var path = new GH_Path(result.CaseId, key.Permutation + perm, key.Id);
          outTransX.Add(new GH_UnitNumber(extrema.X.ToUnit(_lengthUnit)), path);
          outTransY.Add(new GH_UnitNumber(extrema.Y.ToUnit(_lengthUnit)), path);
          outTransZ.Add(new GH_UnitNumber(extrema.Z.ToUnit(_lengthUnit)), path);
          outTransXyz.Add(new GH_UnitNumber(extrema.Xyz.ToUnit(_lengthUnit)), path);
          outRotX.Add(new GH_UnitNumber(extrema.Xx), path);
          outRotY.Add(new GH_UnitNumber(extrema.Yy), path);
          outRotZ.Add(new GH_UnitNumber(extrema.Zz), path);
          outRotXyz.Add(new GH_UnitNumber(extrema.Xxyyzz), path);
        }

        PostHog.Result(result.CaseType, 2, GsaResultsValues.ResultType.Displacement);
      }

      da.SetDataTree(0, outTransX);
      da.SetDataTree(1, outTransY);
      da.SetDataTree(2, outTransZ);
      da.SetDataTree(3, outTransXyz);
      da.SetDataTree(4, outRotX);
      da.SetDataTree(5, outRotY);
      da.SetDataTree(6, outRotZ);
      da.SetDataTree(7, outRotXyz);
    }

    protected override void UpdateUIFromSelectedItems() {
      if (_selectedItems.Count == 1) {
        _spacerDescriptions.Insert(0, "Envelope");
        _dropDownItems.Insert(0, ExtremaHelper.Vector6Displacements.ToList());
        _selectedItems.Insert(0, _dropDownItems[0][0]);
      }

      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
