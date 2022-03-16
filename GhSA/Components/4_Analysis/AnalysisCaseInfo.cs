using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;


namespace GsaGH.Components
{
    /// <summary>
    /// Component to retrieve non-geometric objects from a GSA model
    /// </summary>
    public class AnalysisCaseInfo : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("6f5f7379-4469-4ce8-9a1a-85adc3c2126a");
        public AnalysisCaseInfo()
          : base("Analysis Case Info", "CaseInfo", "Get information about the properties of a GSA Analysis Case (Load Case or Combination)",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat4())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.AnalysisCaseInfo;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Analysis Case", "ΣC", "GSA Analysis Case", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "Na", "Analysis Case Name", GH_ParamAccess.item);
            pManager.AddTextParameter("Description", "De",
                "The description should take the form: 1.5A1 + 0.4A3." + System.Environment.NewLine +
                "Use 'or' for enveloping cases eg (1 or -1.4)A1," + System.Environment.NewLine +
                "'to' for enveloping a range of cases eg (C1 to C3)", GH_ParamAccess.item);
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
                    Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert Analysis Case input parameter of type " +
                        type + " to GsaAnalysisCase");
                    return;
                }
            }
        }
    }
}

