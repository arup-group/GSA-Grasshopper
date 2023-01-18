using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Grasshopper.Kernel;
using GsaAPI;
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
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.ResultsInfo;

    public FootfallResults() : base("Get Footfall Result",
      "Footfall",
      "Get the maximum response factor for a footfall analysis case",
      CategoryName.Name(),
      SubCategoryName.Cat6())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA", "GSA model containing some results", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Case", "ID", "Case ID", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddNumberParameter("Response Factor", "RF", "Maximum response factor", GH_ParamAccess.item);
    }
    #endregion

    Guid _modelGUID = new Guid(); // chache model to 
    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Interop.Gsa_10_1.ComAuto GSA = new Interop.Gsa_10_1.ComAuto();
      string temp = Path.GetTempPath() + Guid.NewGuid().ToString() + ".gwb";

      GsaModelGoo model = null;
      if (DA.GetData(0, ref model))
      {
        model.Value.Model.SaveAs(temp);
        GSA.Open(temp);
      }

      int caseId = 0;
      DA.GetData(1, ref caseId);

      GSA.SetLocale(Interop.Gsa_10_1.Locale.LOC_EN_GB);
      ReadOnlyDictionary<int, Node> nodes = model.Value.Model.Nodes();
      var check = GSA.Output_Init(0, "local", "A" + caseId, 12009001, 0);
      double ffMax = 0;
      foreach(int key in nodes.Keys) 
      {
        if (nodes[key].Restraint.Z)
          continue;
        var r = GSA.Output_Extract(key, 0);
        if (r > ffMax)
          ffMax = r;
      }

      DA.SetData(0, ffMax);
      
      GSA.Close();
      GSA = null;

      PostHogGWA("Footfall results component");
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
