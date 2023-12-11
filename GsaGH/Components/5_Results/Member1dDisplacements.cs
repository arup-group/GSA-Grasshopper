﻿using System;
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
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get GSA beam displacement values
  /// </summary>
  public class Member1dDisplacements : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("d15f1830-d383-4495-af8e-c78478482bcd");
    // to-do: bug in GSA 10.2.3 member results, hide this component for now
    public override GH_Exposure Exposure => GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Member1dDisplacements;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitResult;

    public Member1dDisplacements() : base("Member 1D Displacements", "Mem1dDisp",
      "1D Member Translation and Rotation result values", CategoryName.Name(),
      SubCategoryName.Cat5()) {
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
      pManager.AddParameter(new GsaMemberListParameter());
      pManager[1].Optional = true;
      pManager.AddIntegerParameter("Intermediate Points", "nP",
        "Number of intermediate equidistant points (default 3)", GH_ParamAccess.item, 3);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      string note = ResultNotes.Note1dResults;

      pManager.AddGenericParameter("Translations X [" + unitAbbreviation + "]", "Ux",
        "Translations in Local Member X-direction." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations Y [" + unitAbbreviation + "]", "Uy",
        "Translations in Local Member Y-direction." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations Z [" + unitAbbreviation + "]", "Uz",
        "Translations in Local Member Z-direction." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations |XYZ| [" + unitAbbreviation + "]", "|U|",
        "Combined |XYZ| Translations." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations XX [rad]", "Rxx",
        "Rotations around Local Member X-axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations YY [rad]", "Ryy",
        "Rotations around Local Member Y-axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations ZZ [rad]", "Rzz",
        "Rotations around Local Member Z-axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations |XYZ| [rad]", "|R|",
        "Combined |XXYYZZ| Rotations." + note, GH_ParamAccess.tree);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      GsaResult result;
      string memberList = "All";
      var ghDivisions = new GH_Integer();
      da.GetData(2, ref ghDivisions);
      GH_Convert.ToInt32(ghDivisions, out int positionsCount, GH_Conversion.Both);
      positionsCount = Math.Abs(positionsCount) + 2; // taken absolute value and add 2 end points.

      var ghTypes = new List<GH_ObjectWrapper>();
      da.GetDataList(0, ghTypes);

      var outTransX = new DataTree<GH_UnitNumber>();
      var outTransY = new DataTree<GH_UnitNumber>();
      var outTransZ = new DataTree<GH_UnitNumber>();
      var outTransXyz = new DataTree<GH_UnitNumber>();
      var outRotX = new DataTree<GH_UnitNumber>();
      var outRotY = new DataTree<GH_UnitNumber>();
      var outRotZ = new DataTree<GH_UnitNumber>();
      var outRotXyz = new DataTree<GH_UnitNumber>();

      foreach (GH_ObjectWrapper ghTyp in ghTypes) {
        switch (ghTyp?.Value) {
          case GsaResultGoo goo:
            result = (GsaResult)goo.Value;
            memberList = Inputs.GetMemberListDefinition(this, da, 1, result.Model);
            break;

          default:
            this.AddRuntimeError("Error converting input to GSA Result");
            return;
        }

        ReadOnlyCollection<int> memberIds = result.MemberIds(memberList);
        IEntity1dResultSubset<IEntity1dDisplacement, IDisplacement, ResultVector6<Entity1dExtremaKey>> resultSet
          = result.Member1dDisplacements.ResultSubset(memberIds, positionsCount);

        List<int> permutations = result.SelectedPermutationIds ?? new List<int>() {
          1,
        };
        if (permutations.Count == 1 && permutations[0] == -1) {
          permutations = Enumerable.Range(1, resultSet.Subset.Values.First().Count).ToList();
        }

        if (_selectedItems[0] == ExtremaHelper.Vector6Displacements[0]) {
          foreach (KeyValuePair<int, IList<IEntity1dDisplacement>> kvp in resultSet.Subset) {
            foreach (int p in permutations) {
              var path = new GH_Path(result.CaseId, result.SelectedPermutationIds == null ? 0 : p,
                kvp.Key);
              outTransX.AddRange(
                kvp.Value[p - 1].Results.Values
                 .Select(r => new GH_UnitNumber(r.X.ToUnit(_lengthUnit))), path);
              outTransY.AddRange(
                kvp.Value[p - 1].Results.Values
                 .Select(r => new GH_UnitNumber(r.Y.ToUnit(_lengthUnit))), path);
              outTransZ.AddRange(
                kvp.Value[p - 1].Results.Values
                 .Select(r => new GH_UnitNumber(r.Z.ToUnit(_lengthUnit))), path);
              outTransXyz.AddRange(
                kvp.Value[p - 1].Results.Values
                 .Select(r => new GH_UnitNumber(r.Xyz.ToUnit(_lengthUnit))), path);
              outRotX.AddRange(kvp.Value[p - 1].Results.Values.Select(r => new GH_UnitNumber(r.Xx)),
                path);
              outRotY.AddRange(kvp.Value[p - 1].Results.Values.Select(r => new GH_UnitNumber(r.Yy)),
                path);
              outRotZ.AddRange(kvp.Value[p - 1].Results.Values.Select(r => new GH_UnitNumber(r.Zz)),
                path);
              outRotXyz.AddRange(
                kvp.Value[p - 1].Results.Values.Select(r => new GH_UnitNumber(r.Xxyyzz)), path);
            }
          }
        } else {
          Entity1dExtremaKey key = ExtremaHelper.DisplacementExtremaKey(resultSet, _selectedItems[0]);
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

        PostHog.Result(result.CaseType, 1, "Displacement", "Member");
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
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }
  }
}