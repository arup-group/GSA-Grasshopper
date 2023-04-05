using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaAPI;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to select results from a GSA Model
  /// </summary>
  public class ResultsInfo : GH_OasysComponent {
    private Guid _modelGuid;

    protected override void SolveInstance(IGH_DataAccess da) {
      var model = new GsaModel();
      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp))
        return;

      if (ghTyp.Value is GsaModelGoo) {
        var inModel = new GsaModel();
        ghTyp.CastTo(ref inModel);
        if (_modelGuid == new Guid()) {
          if (inModel.Guid != _modelGuid) {
            model = inModel;
            ClearData();
          }
        }
        else {
          model = inModel;
          _modelGuid = model.Guid;
        }
      }
      else {
        this.AddRuntimeError("Error converting input "
          + Params.Input[0]
            .NickName
          + " to GSA Model");
        return;
      }

      Tuple<List<string>, List<int>, DataTree<int?>> modelResults
        = ResultHelper.GetAvalailableResults(model);

      da.SetDataList(0, modelResults.Item1);
      da.SetDataList(1, modelResults.Item2);
      da.SetDataTree(2, modelResults.Item3);
    }

    #region Name and Ribbon Layout

    public override Guid ComponentGuid => new Guid("6874415d-a86c-4a0d-8c84-36b39f2e5255");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.ResultsInfo;

    public ResultsInfo() : base("Get Result Cases",
      "GetCases",
      "Get Analysis or Combination Case IDs from a GSA model with Results",
      CategoryName.Name(),
      SubCategoryName.Cat5())
      => Hidden = true;

    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
      => pManager.AddParameter(new GsaModelParameter(),
        "GSA Model",
        "GSA",
        "GSA model containing some results",
        GH_ParamAccess.item);

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddTextParameter("Result Type", "T", "Result type", GH_ParamAccess.list);
      pManager.AddIntegerParameter("Case",
        "ID",
        "Case ID(s) - to be read in conjunction with Type output ",
        GH_ParamAccess.list);
      pManager.AddIntegerParameter("Permutation",
        "P",
        "Permutations (only applicable for combination cases). Data as a Tree where each path {i} corrosponds to the Combination Case ID.",
        GH_ParamAccess.tree);
    }

    #endregion
  }
}
