using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
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
  ///   Component to get SteelDesignEffectiveLength
  /// </summary>
  public class SteelUtilisations : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("5f6ff7ac-65c9-417b-a373-bb45a3163e99");
    public override GH_Exposure Exposure => GH_Exposure.septenary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.DesignTaskInfo;

    public SteelUtilisations() : base("Steel Utilisations", "SteelUtil", "Steel Utilisation values",
      CategoryName.Name(), SubCategoryName.Cat5()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Max/Min",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(ExtremaHelper.SteelUtilisations.ToList());
      _selectedItems.Add(_dropDownItems[0][0]);

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaMemberListParameter());
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("Overall", "O", "Overall Utilisation ratio", GH_ParamAccess.list);
      pManager.AddGenericParameter("LocalCombined", "LC", "Local Combined Utilisation ratio", GH_ParamAccess.list);
      pManager.AddGenericParameter("BucklingCombined", "BC", "Buckling Combined Utilisation ratio", GH_ParamAccess.list);
      pManager.AddGenericParameter("LocalAxial", "Ax", "Axial Utilisation ratio", GH_ParamAccess.list);
      pManager.AddGenericParameter("LocalShearU", "Su", "Local Major Shear Utilisation ratio", GH_ParamAccess.list);
      pManager.AddGenericParameter("LocalShearV", "Sv", "Local Minor Shear Utilisation ratio", GH_ParamAccess.list);
      pManager.AddGenericParameter("LocalTorsion", "To", "Local Torsion Utilisation ratio", GH_ParamAccess.list);
      pManager.AddGenericParameter("LocalMajorMoment", "MaM", "Local Major Moment Utilisation ratio", GH_ParamAccess.list);
      pManager.AddGenericParameter("LocalMinorMoment", "MiM", "Local Minor Moment Utilisation ratio", GH_ParamAccess.list);
      pManager.AddGenericParameter("MajorBuckling", "MaB", "Major Buckling Utilisation ratio", GH_ParamAccess.list);
      pManager.AddGenericParameter("MinorBuckling", "MiB", "Minor Buckling Utilisation ratio", GH_ParamAccess.list);
      pManager.AddGenericParameter("LateralTorsionalBuckling", "LTB", "Lateral Torsional Buckling Utilisation ratio", GH_ParamAccess.list);
      pManager.AddGenericParameter("TorsionalBuckling", "TB", "Torsional Buckling Utilisation ratio", GH_ParamAccess.list);
      pManager.AddGenericParameter("FlexuralBuckling", "FB", "Flexural Buckling Utilisation ratio", GH_ParamAccess.list);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      GsaResult result = null;
      string memberList = "All";

      var ghTypes = new List<GH_ObjectWrapper>();
      da.GetDataList(0, ghTypes);

      foreach (GH_ObjectWrapper ghTyp in ghTypes) {
        result = Inputs.GetResultInput(this, ghTyp);
        if (result == null) {
          return;
        }
      }

      var o = new DataTree<GH_UnitNumber>();
      var lc = new DataTree<GH_UnitNumber>();
      var bc = new DataTree<GH_UnitNumber>();
      var ax = new DataTree<GH_UnitNumber>();
      var su = new DataTree<GH_UnitNumber>();
      var sv = new DataTree<GH_UnitNumber>();
      var to = new DataTree<GH_UnitNumber>();
      var mam = new DataTree<GH_UnitNumber>();
      var mim = new DataTree<GH_UnitNumber>();
      var mab = new DataTree<GH_UnitNumber>();
      var mib = new DataTree<GH_UnitNumber>();
      var ltb = new DataTree<GH_UnitNumber>();
      var tb = new DataTree<GH_UnitNumber>();
      var fb = new DataTree<GH_UnitNumber>();

      memberList = Inputs.GetMemberListDefinition(this, da, 1, result.Model);
      ReadOnlyCollection<int> memberIds = result.MemberIds(memberList);
      IEntity0dResultSubset<ISteelUtilisation, SteelUtilisationExtremaKeys> resultSet
        = result.SteelUtilisations.ResultSubset(memberIds);

      List<int> permutations = result.SelectedPermutationIds ?? new List<int>() {
        1,
      };
      if (permutations.Count == 1 && permutations[0] == -1) {
        permutations = Enumerable.Range(1, resultSet.Subset.Values.First().Count).ToList();
      }

      foreach (KeyValuePair<int, IList<ISteelUtilisation>> kvp in resultSet.Subset) {
        int p = permutations[0];
        var path = new GH_Path(result.CaseId,
          result.SelectedPermutationIds == null ? 0 : permutations[0], kvp.Key);

        o.Add(new GH_UnitNumber(kvp.Value[p - 1].Overall), path);
        lc.Add(new GH_UnitNumber(kvp.Value[p - 1].LocalCombined), path);
        bc.Add(new GH_UnitNumber(kvp.Value[p - 1].BucklingCombined), path);
        ax.Add(new GH_UnitNumber(kvp.Value[p - 1].LocalAxial), path);
        su.Add(new GH_UnitNumber(kvp.Value[p - 1].LocalShearU), path);
        sv.Add(new GH_UnitNumber(kvp.Value[p - 1].LocalShearV), path);
        to.Add(new GH_UnitNumber(kvp.Value[p - 1].LocalTorsion), path);
        mam.Add(new GH_UnitNumber(kvp.Value[p - 1].LocalMajorMoment), path);
        mim.Add(new GH_UnitNumber(kvp.Value[p - 1].LocalMinorMoment), path);
        mab.Add(new GH_UnitNumber(kvp.Value[p - 1].MajorBuckling), path);
        mib.Add(new GH_UnitNumber(kvp.Value[p - 1].MinorBuckling), path);
        ltb.Add(new GH_UnitNumber(kvp.Value[p - 1].LateralTorsionalBuckling), path);
        tb.Add(new GH_UnitNumber(kvp.Value[p - 1].TorsionalBuckling), path);
        fb.Add(new GH_UnitNumber(kvp.Value[p - 1].FlexuralBuckling), path);

      }

      PostHog.Result(result.CaseType, 1, "SteelUtilisations");

      da.SetDataTree(0, o);
      da.SetDataTree(1, lc);
      da.SetDataTree(2, bc);
      da.SetDataTree(3, ax);
      da.SetDataTree(4, su);
      da.SetDataTree(5, sv);
      da.SetDataTree(6, to);
      da.SetDataTree(7, mam);
      da.SetDataTree(8, mim);
      da.SetDataTree(9, mab);
      da.SetDataTree(10, mib);
      da.SetDataTree(11, ltb);
      da.SetDataTree(12, tb);
      da.SetDataTree(13, fb);
    }
  }
}
