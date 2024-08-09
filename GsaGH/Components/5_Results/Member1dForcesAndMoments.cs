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
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get GSA beam force values
  /// </summary>
  public class Member1dForcesAndMoments : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("36973d2e-ee21-4165-aa7e-2fd07a76aec3");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Member1dForcesAndMoments;
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private MomentUnit _momentUnit = DefaultUnits.MomentUnit;

    public Member1dForcesAndMoments() : base("Member 1D Forces and Moments", "Mem1dForces",
      "1D Member Force and Moment result values", CategoryName.Name(), SubCategoryName.Cat5()) {
      Hidden = true;
    }

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
      Params.Output[i++].Name = "Force X [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Force Y [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Force Z [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Force |YZ| [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment XX [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment YY [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment ZZ [" + momentunitAbbreviation + "]";
      Params.Output[i].Name = "Moment |YYZZ| [" + momentunitAbbreviation + "]";
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
      pManager.AddParameter(new GsaMemberListParameter());
      pManager[1].Optional = true;
      pManager.AddIntegerParameter("Intermediate Points", "nP",
        "Number of intermediate equidistant points (default 3)", GH_ParamAccess.item, 3);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string forceunitAbbreviation = Force.GetAbbreviation(_forceUnit);
      string momentunitAbbreviation = Moment.GetAbbreviation(_momentUnit);

      string forcerule = Environment.NewLine + "+ve axial forces are tensile";
      string momentrule = Environment.NewLine + "Moments follow the right hand grip rule";
      string note = ResultNotes.Note1dResults;

      pManager.AddGenericParameter("Force X [" + forceunitAbbreviation + "]", "Fx",
        "Member Axial Force in Local Member X-direction" + forcerule + note,
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force Y [" + forceunitAbbreviation + "]", "Fy",
        "Member Shear Force in Local Member Y-direction" + forcerule + note,
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force Z [" + forceunitAbbreviation + "]", "Fz",
        "Member Shear Force in Local Member Z-direction" + forcerule + note,
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force |YZ| [" + forceunitAbbreviation + "]", "|Fyz|",
        "Total |YZ| Member Shear Force" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment XX [" + momentunitAbbreviation + "]", "Mxx",
        "Member Torsional Moment around Local Member X-axis" + momentrule + note,
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment YY [" + momentunitAbbreviation + "]", "Myy",
        "Member Bending Moment around Local Member Y-axis" + momentrule + note,
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment ZZ [" + momentunitAbbreviation + "]", "Mzz",
        "Member Bending Moment around Local Member Z-axis" + momentrule + note,
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment |YZ| [" + momentunitAbbreviation + "]", "|Myz|",
        "Total |YYZZ| Member Bending Moment" + note, GH_ParamAccess.tree);
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
        result = Inputs.GetResultInput(this, ghTyp);
        if (result == null) {
          return;
        }

        memberList = Inputs.GetMemberListDefinition(this, da, 1, result.Model);
        ReadOnlyCollection<int> elementIds = result.MemberIds(memberList);
        IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet =
          result.Member1dInternalForces.ResultSubset(elementIds, positionsCount);

        List<int> permutations = result.SelectedPermutationIds ?? new List<int>() {
          1,
        };
        if (permutations.Count == 1 && permutations[0] == -1) {
          permutations = Enumerable.Range(1, resultSet.Subset.Values.First().Count).ToList();
        }

        if (_selectedItems[0] == ExtremaHelper.Vector6InternalForces[0]) {
          foreach (KeyValuePair<int, IList<IEntity1dQuantity<IInternalForce>>> kvp in resultSet.Subset) {
            foreach (int p in permutations) {
              var path = new GH_Path(result.CaseId, result.SelectedPermutationIds == null ? 0 : p, kvp.Key);
              outTransX.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.X.ToUnit(_forceUnit))), path);
              outTransY.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.Y.ToUnit(_forceUnit))), path);
              outTransZ.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.Z.ToUnit(_forceUnit))), path);
              outTransXyz.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.Xyz.ToUnit(_forceUnit))), path);
              outRotX.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.Xx.ToUnit(_momentUnit))), path);
              outRotY.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.Yy.ToUnit(_momentUnit))), path);
              outRotZ.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.Zz.ToUnit(_momentUnit))), path);
              outRotXyz.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.Xxyyzz.ToUnit(_momentUnit))), path);
            }
          }
        } else {
          Entity1dExtremaKey key = ExtremaHelper.InternalForceExtremaKey(resultSet, _selectedItems[0]);
          IInternalForce extrema = resultSet.GetExtrema(key);
          int perm = result.CaseType == CaseType.AnalysisCase ? 0 : 1;
          var path = new GH_Path(result.CaseId, key.Permutation + perm, key.Id);
          outTransX.Add(new GH_UnitNumber(extrema.X.ToUnit(_forceUnit)), path);
          outTransY.Add(new GH_UnitNumber(extrema.Y.ToUnit(_forceUnit)), path);
          outTransZ.Add(new GH_UnitNumber(extrema.Z.ToUnit(_forceUnit)), path);
          outTransXyz.Add(new GH_UnitNumber(extrema.Xyz.ToUnit(_forceUnit)), path);
          outRotX.Add(new GH_UnitNumber(extrema.Xx.ToUnit(_momentUnit)), path);
          outRotY.Add(new GH_UnitNumber(extrema.Yy.ToUnit(_momentUnit)), path);
          outRotZ.Add(new GH_UnitNumber(extrema.Zz.ToUnit(_momentUnit)), path);
          outRotXyz.Add(new GH_UnitNumber(extrema.Xxyyzz.ToUnit(_momentUnit)), path);
        }

        PostHog.Result(result.CaseType, 1, "Force", "Member");
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
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[1]);
      _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), _selectedItems[2]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
