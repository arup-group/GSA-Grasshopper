using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Parameters;


namespace GsaGH.Components
{
    /// <summary>
    /// Component to retrieve non-geometric objects from a GSA model
    /// </summary>
    public class CreateAnalysisCase : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("75bf9454-92c4-4a3c-8abf-75f1d449bb85");
        public CreateAnalysisCase()
          : base("Create Analysis Case", "CreateCase", "Create a new GSA Analysis Case (Load Case or Combination)",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat4())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateAnalysisCase;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "Na", "Case Name", GH_ParamAccess.item);
            pManager.AddTextParameter("Description", "De",
                "The description should take the form: 1.5A1 + 0.4A3." + System.Environment.NewLine +
                "Use 'or' for enveloping cases eg (1 or -1.4)A1," + System.Environment.NewLine +
                "'to' for enveloping a range of cases eg (C1 to C3)", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Analysis Case", "ΣC", "GSA Analysis Case", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = "";
            DA.GetData(0, ref name);

            string desc = "";
            DA.GetData(1, ref desc);

            DA.SetData(0, new GsaAnalysisCaseGoo(new GsaAnalysisCase(name, desc)));
        }
    }
}

