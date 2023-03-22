﻿using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  /// Component to create a GSA model from GWA string
  /// </summary>
  public class GwaCommand : GH_OasysComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("ed3e5d61-9942-49d4-afc7-310285c783c6");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GwaModel;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override string HtmlHelp_Source() {
      return "GOTO:https://docs.oasys-software.com/structural/gsa/references/comautomation.html#gwacommand-function";
    }

    public GwaCommand() : base(
      "GWA Command",
      "GWA",
      "Create a model from a GWA string, inject data into a model using GWA command, or retrieve model data or results through a GWA command.",
      CategoryName.Name(),
      SubCategoryName.Cat0()) {
      Hidden = true;
    }

    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA", "(Optional) Existing GSA model to inject GWA command(s) into. Leave this input empty to create a new GSA model from GWA string.", GH_ParamAccess.item);
      pManager[0].Optional = true;
      pManager.AddTextParameter("GWA string", "GWA", "GWA string from GSA. Right-click on any data, and select copy all. Paste into notepad to check the data. \r\nThis input takes a a list of text strings that will automatically be joined. Construct a tree structure if you want to create multiple GSA files. \r\nThe syntax of the command is based on GWA syntax and the units follow the GWA unit syntax; –\r\nRefer to the “GSA Keywords” document for details.\r\nNote that locale is set to use '.' as decimal separator.\r\nRight-click -> Help for further infor on GWA Commands.", GH_ParamAccess.list);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter());
      pManager.AddGenericParameter("Returned result", "R", "The 'variant' return value from executing a GWA command issued to GSA. \r\nThe syntax of the command is based on GWA syntax and the units follow the GWA unit syntax; –\r\nRefer to the “GSA Keywords” document for details.\r\nNote that locale is set to use '.' as decimal separator.\r\nRight-click -> Help for further infor on GWA Commands.", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaModelGoo gooModel = null;
      GsaModel model = null;
      if (da.GetData(0, ref gooModel))
        model = gooModel.Value;
      Interop.Gsa_10_1.ComAuto m = GsaComHelper.GetGsaComModel(model);

      string gwa = "";
      var strings = new List<string>();
      if (da.GetDataList(1, strings))
        gwa = strings.Aggregate(gwa, (current, s) => current + (s + "\n"));

      da.SetData(1, m.GwaCommand(gwa));

      GsaModel gsaGh = GsaComHelper.GetGsaGhModel();
      da.SetData(0, new GsaModelGoo(gsaGh));
      PostHog.GWA(gwa, Params.Input.Count > 0);
    }
  }
}
