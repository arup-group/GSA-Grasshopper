using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to select results from a GSA Model
  /// </summary>
  public class FootfallResults : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("c5194fe3-8c20-43f0-a8cb-3207ed867221");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Footfall;

    public FootfallResults() : base("Footfall Result",
      "Footfall",
      "Get the maximum response factor for a footfall analysis case",
      CategoryName.Name(),
      SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.item);
      pManager.AddTextParameter("Node filter list", "No", "Filter results by list." + Environment.NewLine +
          "Node list should take the form:" + Environment.NewLine +
          " 1 11 to 72 step 2 not (XY3 31 to 45)" + Environment.NewLine +
          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddNumberParameter("Resonant Response Factor", "RRF", "Maximum resonant response factor", GH_ParamAccess.item);
      pManager.AddNumberParameter("Transient Response Factor", "TRF", "Maximum transient response factor", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // Result to work on
      GsaResult result = new GsaResult();

      // Get Model
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        if (gh_typ == null || gh_typ.Value == null)
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input is null");
          return;
        }
        if (gh_typ.Value is GsaResultGoo)
        {
          result = ((GsaResultGoo)gh_typ.Value).Value;
          if (result.Type == GsaResult.ResultType.Combination)
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Footfall Result only available for Analysis Cases");
            return;
          }
        }
        else
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Result");
          return;
        }

        // Get node filter list
        string nodeList = "All";
        GH_String gh_noList = new GH_String();
        if (DA.GetData(1, ref gh_noList))
        {
          if (GH_Convert.ToString(gh_noList, out string tempnodeList, GH_Conversion.Both))
            nodeList = tempnodeList;
        }

        if (nodeList.ToLower() == "all" || nodeList == "")
          nodeList = "All";

        GsaResultsValues res = result.NodeFootfallValues(nodeList, Helpers.GsaAPI.FootfallResultType.Resonant);
        GsaResultsValues tra = result.NodeFootfallValues(nodeList, Helpers.GsaAPI.FootfallResultType.Transient);

        DA.SetData(0, res.dmax_x.Value);
        DA.SetData(1, tra.dmax_x.Value);

        PostHogGWA("FootfallResultsComponent");
      }
    }

    private void PostHogGWA(string gwa)
    {
      string[] commands = gwa.Split('\n');
      foreach (string command in commands)
      {
        if (command == "") { continue; }
        string key = command.Split('.')[0].Split(',')[0].Split('\t')[0].Split(' ')[0];
        if (key == "") { continue; }
        string eventName = "GwaCommand";
        Dictionary<string, object> properties = new Dictionary<string, object>()
        {
          { key, command },
          { "existingModel", this.Params.Input.Count > 0 },
        };
        _ = PostHog.SendToPostHog(GsaGH.PluginInfo.Instance, eventName, properties);
      }
    }
  }
}
