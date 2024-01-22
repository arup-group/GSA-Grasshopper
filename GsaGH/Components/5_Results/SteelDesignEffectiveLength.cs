using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get SteelDesignEffectiveLength
  /// </summary>
  public class SteelDesignEffectiveLength : GH_OasysDropDownComponent {
    private enum SteelDesignTypes  {
      Major,
      Minor,
      LT,
    }
    public override Guid ComponentGuid => new Guid("7d09d701-edf1-4346-a50f-9e11e7855893");
    public override GH_Exposure Exposure => GH_Exposure.septenary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.ContourNodeResults;
    private SteelDesignTypes _type = SteelDesignTypes.Major;

    private readonly IReadOnlyDictionary<SteelDesignTypes, string> _steelDesignTypes
      = new Dictionary<SteelDesignTypes, string> {
        { SteelDesignTypes.Major, "Major" },
        { SteelDesignTypes.Minor, "Minor" },
        { SteelDesignTypes.LT, "LT" },
      };

    public SteelDesignEffectiveLength() : base("Steel Design Effective Length", "SDEL",
      "Steel Design Effective Length", CategoryName.Name(),
      SubCategoryName.Cat5()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      SteelDesignTypes type = GetModeBy(_selectedItems[0]);
      UpdateParameters(type);

      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Type",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(_steelDesignTypes.Values.ToList());
      _selectedItems.Add(_steelDesignTypes.Values.ElementAt(0));

      _isInitialised = true;
    }

    public override void VariableParameterMaintenance() {
      switch (_type) {
        case SteelDesignTypes.Major:
          SetOutputProperties(1, "Major Span","Spu", "Span number(s) for major axis buckling mode");
          SetOutputProperties(2, "Major Span Elements","Elu"," Span Elements for major axis buckling mode");
          SetOutputProperties(3, "Major Start Position","SPu"," The start position of each span along the length of the member");
          SetOutputProperties(4, "Major End Position","EPu","The start position of each span along the length of the member");
          SetOutputProperties(5, "Major Span Length","Slu","The length of each span");
          SetOutputProperties(6, "Major Effective Length","Leu","The start position of each span along the length of the member");
          SetOutputProperties(7, "Major Effective Span Ratio","Lru","The ratio between effective and total length of the member");
          SetOutputProperties(8, "Major Effective Span Ratio,","Lsu","The ratio between effective and span length");
          SetOutputProperties(9, "Major Slenderness Ratio","Sru","The ratio between effective and span length");
          break;
        case SteelDesignTypes.Minor:
          SetOutputProperties(1, "Minor Span","Spv", "Span number(s) for minor axis buckling mode");
          SetOutputProperties(2, "Minor Span Elements","Elv", " Span Elements for minor axis buckling mode");
          SetOutputProperties(3, "Minor Start Position","SPv"," The start position of each span along the length of the member");
          SetOutputProperties(4, "Minor End Position","EPv","The start position of each span along the length of the member");
          SetOutputProperties(5, "Minor Span Length","Slv","The length of each span");
          SetOutputProperties(6, "Minor Effective Length","Lev","The start position of each span along the length of the member");
          SetOutputProperties(7, "Minor Effective Span Ratio","Lrv","The ratio between effective and total length of the member");
          SetOutputProperties(8, "Minor Effective Span Ratio,","Lsv","The ratio between effective and span length");
          SetOutputProperties(9, "Minor Slenderness Ratio","Srv","The ratio between effective and span length");
          break;
        case SteelDesignTypes.LT:
          SetOutputProperties(1, "LT Span", "Spt", "Span number(s) for minor axis buckling mode");
          SetOutputProperties(2, "LT Span Elements", "Elt", " Span Elements for minor axis buckling mode");
          SetOutputProperties(3, "LT Start Position", "SPt", " The start position of each span along the length of the member");
          SetOutputProperties(4, "LT End Position", "EPt", "The start position of each span along the length of the member");
          SetOutputProperties(5, "LT Span Length", "Slt", "The length of each span");
          SetOutputProperties(6, "LT Effective Length", "Let", "The start position of each span along the length of the member");
          SetOutputProperties(7, "LT Effective Span Ratio", "Lrt", "The ratio between effective and total length of the member");
          SetOutputProperties(8, "LT Effective Span Ratio,", "Lst", "The ratio between effective and span length");
          SetOutputProperties(9, "LT Slenderness Ratio", "Srt", "The ratio between effective and span length");
          break;
        default:this.AddRuntimeError("Incorrect type of steel design effective length");
          break;
      }
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.list);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("Member Length", "L", "The length of the member", GH_ParamAccess.list);
      RegisterOutputParams();
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      //GsaResult result;
      //string elementlist = "All";
      //var ghDivisions = new GH_Integer();
      //da.GetData(2, ref ghDivisions);
      //GH_Convert.ToInt32(ghDivisions, out int positionsCount, GH_Conversion.Both);
      //positionsCount = Math.Abs(positionsCount) + 2; // taken absolute value and add 2 end points.

      //var ghTypes = new List<GH_ObjectWrapper>();
      //da.GetDataList(0, ghTypes);

      //var outShearY = new DataTree<GH_UnitNumber>();
      //var outShearZ = new DataTree<GH_UnitNumber>();
      //var outTorsional = new DataTree<GH_UnitNumber>();
      //var outVonMises = new DataTree<GH_UnitNumber>();

      //foreach (GH_ObjectWrapper ghTyp in ghTypes) {
      //  result = Inputs.GetResultInput(this, ghTyp);
      //  if (result == null) {
      //    return;
      //  }

      //  elementlist = Inputs.GetElementListDefinition(this, da, 1, result.Model);
      //  ReadOnlyCollection<int> elementIds = result.ElementIds(elementlist, 1);
      //  IEntity1dResultSubset<IStress1dDerived, ResultDerivedStress1d<Entity1dExtremaKey>> resultSet =
      //    result.Element1dDerivedStresses.ResultSubset(elementIds, positionsCount);

      //  List<int> permutations = result.SelectedPermutationIds ?? new List<int>() {
      //    1,
      //  };
      //  if (permutations.Count == 1 && permutations[0] == -1) {
      //    permutations = Enumerable.Range(1, resultSet.Subset.Values.First().Count).ToList();
      //  }

      //  if (_selectedItems[0] == ExtremaHelper.Stress1d[0]) {
      //    foreach (KeyValuePair<int, IList<IEntity1dQuantity<IStress1dDerived>>> kvp in resultSet.Subset) {
      //      foreach (int p in permutations) {
      //        var path = new GH_Path(result.CaseId, result.SelectedPermutationIds == null ? 0 : p, kvp.Key);
      //        outShearY.AddRange(kvp.Value[p - 1].Results.Values.Select(
      //          r => new GH_UnitNumber(r.ElasticShearY.ToUnit(_stressUnit))), path);
      //        outShearZ.AddRange(kvp.Value[p - 1].Results.Values.Select(
      //          r => new GH_UnitNumber(r.ElasticShearZ.ToUnit(_stressUnit))), path);
      //        outTorsional.AddRange(kvp.Value[p - 1].Results.Values.Select(
      //          r => new GH_UnitNumber(r.Torsional.ToUnit(_stressUnit))), path);
      //        outVonMises.AddRange(kvp.Value[p - 1].Results.Values.Select(
      //          r => new GH_UnitNumber(r.VonMises.ToUnit(_stressUnit))), path);
      //      }
      //    }
      //  } else {
      //    Entity1dExtremaKey key = ExtremaHelper.Stress1dDerivedExtremaKey(resultSet, _selectedItems[0]);
      //    IStress1dDerived extrema = resultSet.GetExtrema(key);
      //    int perm = result.CaseType == CaseType.AnalysisCase ? 0 : 1;
      //    var path = new GH_Path(result.CaseId, key.Permutation + perm, key.Id);
      //    outShearY.Add(new GH_UnitNumber(extrema.ElasticShearY.ToUnit(_stressUnit)), path);
      //    outShearZ.Add(new GH_UnitNumber(extrema.ElasticShearZ.ToUnit(_stressUnit)), path);
      //    outTorsional.Add(new GH_UnitNumber(extrema.Torsional.ToUnit(_stressUnit)), path);
      //    outVonMises.Add(new GH_UnitNumber(extrema.VonMises.ToUnit(_stressUnit)), path);
      //  }

      //  PostHog.Result(result.CaseType, 1, "Displacement");
      //}

      //da.SetDataTree(0, outShearY);
      //da.SetDataTree(1, outShearZ);
      //da.SetDataTree(2, outTorsional);
      //da.SetDataTree(3, outVonMises);
    }

    protected override void UpdateUIFromSelectedItems() {
      SteelDesignTypes mode = GetModeBy(_selectedItems[0]);
      UpdateParameters(mode);
      base.UpdateUIFromSelectedItems();
    }


    private void UpdateParameters(SteelDesignTypes type) {
      if (type == _type) {
        return;
      }

      _steelDesignTypes.TryGetValue(type, out string eventName);
      RecordUndoEvent($"{eventName} Steel Design Type");

      while (Params.Output.Count > 1) {
        Params.UnregisterOutputParameter(Params.Output[1], true);
      }

      RegisterOutputParams();
      _type = type;
    }

    private void RegisterOutputParams() {
      Params.RegisterOutputParam(new Param_Integer());
      Params.RegisterOutputParam(new Param_String());
      Params.RegisterOutputParam(new Param_GenericObject());
      Params.RegisterOutputParam(new Param_GenericObject());
      Params.RegisterOutputParam(new Param_GenericObject());
      Params.RegisterOutputParam(new Param_GenericObject());
      Params.RegisterOutputParam(new Param_GenericObject());
      Params.RegisterOutputParam(new Param_GenericObject());
      Params.RegisterOutputParam(new Param_GenericObject());
    }

    private void SetOutputProperties(int index, string name, string nickname, string description) {
      Params.Output[index].Name = name;
      Params.Output[index].NickName = nickname;
      Params.Output[index].Description = description;
      Params.Output[index].Access = GH_ParamAccess.list;
    }

    private SteelDesignTypes GetModeBy(string name) {
      foreach (KeyValuePair<SteelDesignTypes, string> item in _steelDesignTypes) {
        if (item.Value.Equals(name)) {
          return item.Key;
        }
      }
      throw new ArgumentException("Unable to convert " + name + " to Steel Design Effective Length Type");
    }
  }
}
