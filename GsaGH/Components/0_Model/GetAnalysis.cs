// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  /// Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  public class GetAnalysis : GH_OasysComponent {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("566a94d2-a022-4f12-a645-0366deb1476c");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetAnalysisTask;

    public GetAnalysis() : base("Get Model Analysis Tasks",
      "GetAnalysisTasks",
      "Get Analysis Tasks and their Cases from GSA model",
      CategoryName.Name(),
      SubCategoryName.Cat0()) {
        Hidden = true;
    } // sets the initial state of the component to hidden
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA", "GSA model containing some Analysis Cases and Tasks", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaAnalysisTaskParameter(), "Analysis Tasks", "ΣT", "List of Analysis Tasks in model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaAnalysisCaseParameter(), "Analysis Cases", "ΣA", "List of Analysis Cases in model", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaModel = new GsaModel();
      if (!da.GetData(0, ref gsaModel)) return;

      Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> tuple = Helpers.Import.Analyses.GetAnalysisTasksAndCombinations(gsaModel);

      da.SetDataList(0, tuple.Item1);
      da.SetDataList(1, tuple.Item2);
    }
  }
}

