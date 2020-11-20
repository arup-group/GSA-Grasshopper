using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GhSA.Parameters;


namespace GhSA.Components
{
    /// <summary>
    /// Component to retrieve non-geometric objects from a GSA model
    /// </summary>
    public class GlobalResult : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("267d8dc3-aa6e-4ed2-b82d-57fc290173cc");
        public GlobalResult()
          : base("Global Results", "GlobalResult", "Get Global Results from GSA model",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat5())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => GSA.Properties.Resources.GlobalResults;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA Model", "GSA", "GSA model containing some results", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Load Case", "LC", "Load Case (default 1)", GH_ParamAccess.item, 1);
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddVectorParameter("Total Force Loads", "ΣF", "Sum of all Force Loads in GSA Model (" + Util.GsaUnit.Force + ")", GH_ParamAccess.item);
            pManager.AddVectorParameter("Total Moment Loads", "ΣM", "Sum of all Moment Loads in GSA Model (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")", GH_ParamAccess.item);
            pManager.AddVectorParameter("Total Force Reactions", "ΣRF", "Sum of all Rection Forces in GSA Model (" + Util.GsaUnit.Force + ")", GH_ParamAccess.item);
            pManager.AddVectorParameter("Total Moment Reactions", "ΣRM", "Sum of all Reaction Moments in GSA Model (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")", GH_ParamAccess.item);
            pManager.AddVectorParameter("Effective Mass", "Σkg", "Effective Mass in GSA Model (" + Util.GsaUnit.Mass + ")", GH_ParamAccess.item);
            //pManager.AddVectorParameter("Effective Inertia", "ΣI", "Effective Inertia in GSA Model", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Model to work on
            GsaModel gsaModel = new GsaModel();

            // Get Model
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(0, ref gh_typ))
            {
                #region Inputs
                if (gh_typ.Value is GsaModelGoo)
                    gh_typ.CastTo(ref gsaModel);
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Model");
                    return;
                }

                // Get analysis case 
                GH_Integer gh_aCase = new GH_Integer();
                DA.GetData(1, ref gh_aCase);
                int analCase = 1;
                GH_Convert.ToInt32(gh_aCase, out analCase, GH_Conversion.Both);
                #endregion

                #region Get results from GSA
                // ### Get results ###
                //Get analysis case from model
                AnalysisCaseResult analysisCaseResult = null;
                gsaModel.Model.Results().TryGetValue(analCase, out analysisCaseResult);
                if (analysisCaseResult == null)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No results exist for Analysis Case " + analCase + " in file");
                    return;
                }
                #endregion

                double unitfactorForce = 1000;
                double unitfactorMoment = 1000;

                Vector3d force = new Vector3d(
                    analysisCaseResult.Global.TotalLoad.X / unitfactorForce,
                    analysisCaseResult.Global.TotalLoad.Y / unitfactorForce,
                    analysisCaseResult.Global.TotalLoad.Z / unitfactorForce);
                
                Vector3d moment = new Vector3d(
                    analysisCaseResult.Global.TotalLoad.XX / unitfactorMoment,
                    analysisCaseResult.Global.TotalLoad.YY / unitfactorMoment,
                    analysisCaseResult.Global.TotalLoad.ZZ / unitfactorMoment);
                
                Vector3d reaction = new Vector3d(
                    analysisCaseResult.Global.TotalReaction.X / unitfactorForce,
                    analysisCaseResult.Global.TotalReaction.Y / unitfactorForce,
                    analysisCaseResult.Global.TotalReaction.Z / unitfactorForce);
                
                Vector3d reactionmoment = new Vector3d(
                    analysisCaseResult.Global.TotalReaction.XX / unitfactorMoment,
                    analysisCaseResult.Global.TotalReaction.YY / unitfactorMoment,
                    analysisCaseResult.Global.TotalReaction.ZZ / unitfactorMoment);

                Vector3d effMass = new Vector3d(
                    analysisCaseResult.Global.EffectiveMass.X,
                    analysisCaseResult.Global.EffectiveMass.Y,
                    analysisCaseResult.Global.EffectiveMass.Z);

                //Vector3d effStiff = new Vector3d(
                //    analysisCaseResult.Global.EffectiveInertia.X,
                 //   analysisCaseResult.Global.EffectiveInertia.Y,
                 //   analysisCaseResult.Global.EffectiveInertia.Z);

                DA.SetData(0, force);
                DA.SetData(1, moment);
                DA.SetData(2, reaction);
                DA.SetData(3, reactionmoment);
                DA.SetData(4, effMass);
                //DA.SetData(5, effStiff);
            }
        }
    }
}

