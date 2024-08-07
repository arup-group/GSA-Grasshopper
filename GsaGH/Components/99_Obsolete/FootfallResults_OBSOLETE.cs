using System;
using System.Collections.ObjectModel;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to select results from a GSA Model
  /// </summary>
  public class FootfallResults_OBSOLETE : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("c5194fe3-8c20-43f0-a8cb-3207ed867221");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.FootfallResults;

    public FootfallResults_OBSOLETE() : base("Footfall Results", "Footfall",
      "Get the maximum response factor for a footfall analysis case", CategoryName.Name(),
      SubCategoryName.Cat5()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaNodeListParameter());
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddNumberParameter("Resonant Response Factor", "RRF",
        "Maximum resonant response factor", GH_ParamAccess.item);
      pManager.AddNumberParameter("Transient Response Factor", "TRF",
        "Maximum transient response factor", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaResult result;
      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp)) {
        return;
      }

      this.AddRuntimeError("This component is obsolete and will be removed in the next version of the plugin. Please update to use the newest version of Footfall results which is faster and better all together.");

      switch (ghTyp?.Value) {
        case null:
          this.AddRuntimeWarning("Input is null");
          return;

        case GsaResultGoo goo: {
            result = (GsaResult)goo.Value;
            if (result.CaseType == CaseType.CombinationCase) {
              this.AddRuntimeError("Footfall Result only available for Analysis Cases");
              return;
            }

            break;
          }
        default:
          this.AddRuntimeError("Error converting input to GSA Result");
          return;
      }

      string nodeList = Inputs.GetNodeListDefinition(this, da, 1, result.Model);

      ReadOnlyCollection<int> nodeIds = result.NodeIds(nodeList);
      IEntity0dResultSubset<IFootfall, ResultFootfall<Entity0dExtremaKey>> resonant
        = result.NodeResonantFootfalls.ResultSubset(nodeIds);
      IEntity0dResultSubset<IFootfall, ResultFootfall<Entity0dExtremaKey>> transient
        = result.NodeTransientFootfalls.ResultSubset(nodeIds);

      da.SetData(0, resonant.GetExtrema(resonant.Max.MaximumResponseFactor).MaximumResponseFactor);
      da.SetData(1, transient.GetExtrema(transient.Max.MaximumResponseFactor).MaximumResponseFactor);

      PostHog.Result(result.CaseType, 0, "Footfall", "Max");
    }
  }
}
