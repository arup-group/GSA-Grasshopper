using System;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get information about GSA Analysis Cases
  /// </summary>
  public class AnalysisCaseInfo : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("6f5f7379-4469-4ce8-9a1a-85adc3c2126a");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.AnalysisCaseInfo;

    public AnalysisCaseInfo() : base("Analysis Case Info", "CaseInfo",
      "Get information about a GSA Analysis Case",
      CategoryName.Name(), SubCategoryName.Cat4()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaAnalysisCaseParameter());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddTextParameter("Name", "Na", "Analysis Case Name", GH_ParamAccess.item);
      pManager.AddTextParameter("Definition", "De", "The definition of the analysis case",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("CaseID", "ID",
        "The Case number if the Analysis Case ever belonged to a model", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp)) {
        return;
      }

      if (ghTyp.Value is GsaAnalysisCaseGoo goo) {
        da.SetData(0, goo.Value.ApiCase.Name);
        da.SetData(1, goo.Value.ApiCase.Description);
        da.SetData(2, goo.Value.Id);
      } else {
        string type = ghTyp.Value.GetType().ToString();
        type = type.Replace("GsaGH.Parameters.", string.Empty);
        type = type.Replace("Goo", string.Empty);
        Params.Owner.AddRuntimeError("Unable to convert Analysis Case input parameter of type "
          + type + " to GsaAnalysisCase");
      }
    }
  }
}
