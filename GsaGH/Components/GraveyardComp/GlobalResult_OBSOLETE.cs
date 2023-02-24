﻿using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using Rhino.Geometry;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to retrieve non-geometric objects from a GSA model
    /// </summary>
    public class GlobalResult_OBSOLETE : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("267d8dc3-aa6e-4ed2-b82d-57fc290173cc");
    public GlobalResult_OBSOLETE()
      : base("Global Results", "GlobalResult", "Get Global Results from GSA model",
            CategoryName.Name(),
            SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.ResultGlobal;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("GSA Model", "GSA", "GSA model containing some results", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Load Case", "LC", "Load Case (default 1)", GH_ParamAccess.item, 1);
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {

      pManager.AddVectorParameter("Total Force Loads [kN]", "ΣF", "Sum of all Force Loads in GSA Model", GH_ParamAccess.item);
      pManager.AddVectorParameter("Total Moment Loads [kNm]", "ΣM", "Sum of all Moment Loads in GSA Model", GH_ParamAccess.item);
      pManager.AddVectorParameter("Total Force Reactions [kN]", "ΣRf", "Sum of all Rection Forces in GSA Model", GH_ParamAccess.item);
      pManager.AddVectorParameter("Total Moment Reactions [kNm]", "ΣRm", "Sum of all Reaction Moments in GSA Model", GH_ParamAccess.item);
      pManager.AddVectorParameter("Effective Mass [kg]", "Σkg", "Effective Mass in GSA Model", GH_ParamAccess.item);
      pManager.AddVectorParameter("Effective Inertia [m4]", "ΣI", "Effective Inertia in GSA Model", GH_ParamAccess.item);
      pManager.AddNumberParameter("Mode", "Mo", "Mode number if LC is a dynamic task", GH_ParamAccess.item);
      pManager.AddVectorParameter("Modal", "Md", "Modal results in vector form:" + Environment.NewLine +
          "x: Modal Mass" + Environment.NewLine +
          "y: Modal Stiffness" + Environment.NewLine +
          "z: Modal Geometric Stiffness", GH_ParamAccess.item);
      pManager.AddNumberParameter("Frequency [Hz]", "f", "Frequency of selected LoadCase / mode", GH_ParamAccess.item);
      pManager.AddNumberParameter("Load Factor", "LF", "Load Factor for selected LoadCase / mode", GH_ParamAccess.item);

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
          this.AddRuntimeError("Error converting input to GSA Model");
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
          this.AddRuntimeError("No results exist for Analysis Case " + analCase + " in file");
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

        Vector3d effStiff;
        if (analysisCaseResult.Global.EffectiveInertia != null)
        {
          effStiff = new Vector3d(
              analysisCaseResult.Global.EffectiveInertia.X,
              analysisCaseResult.Global.EffectiveInertia.Y,
              analysisCaseResult.Global.EffectiveInertia.Z);
        }
        else
          effStiff = new Vector3d();

        Vector3d modal = new Vector3d(
            analysisCaseResult.Global.ModalMass,
            analysisCaseResult.Global.ModalStiffness,
            analysisCaseResult.Global.ModalGeometricStiffness);

        DA.SetData(0, force);
        DA.SetData(1, moment);
        DA.SetData(2, reaction);
        DA.SetData(3, reactionmoment);
        DA.SetData(4, effMass);
        DA.SetData(5, effStiff);
        DA.SetData(6, analysisCaseResult.Global.Mode);
        DA.SetData(7, modal);
        DA.SetData(8, analysisCaseResult.Global.Frequency);
        DA.SetData(9, analysisCaseResult.Global.LoadFactor);
      }
    }
  }
}

