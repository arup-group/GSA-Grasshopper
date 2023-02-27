using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to get information about GSA Analysis Cases
    /// </summary>
    public class AnalysisCaseInfo : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("6f5f7379-4469-4ce8-9a1a-85adc3c2126a");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.AnalysisCaseInfo;

    public AnalysisCaseInfo() : base("Analysis Case Info",
      "CaseInfo",
      "Get information about the properties of a GSA Analysis Case (Load Case or Combination)",
      CategoryName.Name(),
      SubCategoryName.Cat4())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaAnalysisCaseParameter());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddTextParameter("Name", "Na", "Analysis Case Name", GH_ParamAccess.item);
      pManager.AddTextParameter("Description", "De", "The description of the analysis case", GH_ParamAccess.item);
      pManager.AddIntegerParameter("CaseID", "ID", "The Case number if the Analysis Case ever belonged to a model", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        GsaAnalysisCase gsaCase = null;
        if (gh_typ.Value is GsaAnalysisCaseGoo)
        {
          gsaCase = ((GsaAnalysisCaseGoo)gh_typ.Value).Value.Duplicate();
          DA.SetData(0, gsaCase.Name);
          DA.SetData(1, gsaCase.Description);
          DA.SetData(2, gsaCase.ID);
        }
        else
        {
          string type = gh_typ.Value.GetType().ToString();
          type = type.Replace("GsaGH.Parameters.", "");
          type = type.Replace("Goo", "");
          Params.Owner.AddRuntimeError("Unable to convert Analysis Case input parameter of type " +
              type + " to GsaAnalysisCase");
          return;
        }
      }
    }
  }
}

