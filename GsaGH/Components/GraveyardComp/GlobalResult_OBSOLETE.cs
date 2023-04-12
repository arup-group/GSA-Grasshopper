using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class GlobalResult_OBSOLETE : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("267d8dc3-aa6e-4ed2-b82d-57fc290173cc");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    public GlobalResult_OBSOLETE() : base("Global Results",
      "GlobalResult",
      "Get Global Results from GSA model",
      CategoryName.Name(),
      SubCategoryName.Cat5())
      => Hidden = true;

    protected override Bitmap Icon => Resources.ResultGlobal;
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("GSA Model",
        "GSA",
        "GSA model containing some results",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Load Case",
        "LC",
        "Load Case (default 1)",
        GH_ParamAccess.item,
        1);
      pManager[1]
        .Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddVectorParameter("Total Force Loads [kN]",
        "ΣF",
        "Sum of all Force Loads in GSA Model",
        GH_ParamAccess.item);
      pManager.AddVectorParameter("Total Moment Loads [kNm]",
        "ΣM",
        "Sum of all Moment Loads in GSA Model",
        GH_ParamAccess.item);
      pManager.AddVectorParameter("Total Force Reactions [kN]",
        "ΣRf",
        "Sum of all Rection Forces in GSA Model",
        GH_ParamAccess.item);
      pManager.AddVectorParameter("Total Moment Reactions [kNm]",
        "ΣRm",
        "Sum of all Reaction Moments in GSA Model",
        GH_ParamAccess.item);
      pManager.AddVectorParameter("Effective Mass [kg]",
        "Σkg",
        "Effective Mass in GSA Model",
        GH_ParamAccess.item);
      pManager.AddVectorParameter("Effective Inertia [m4]",
        "ΣI",
        "Effective Inertia in GSA Model",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Mode",
        "Mo",
        "Mode number if LC is a dynamic task",
        GH_ParamAccess.item);
      pManager.AddVectorParameter("Modal",
        "Md",
        "Modal results in vector form:"
        + Environment.NewLine
        + "x: Modal Mass"
        + Environment.NewLine
        + "y: Modal Stiffness"
        + Environment.NewLine
        + "z: Modal Geometric Stiffness",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Frequency [Hz]",
        "f",
        "Frequency of selected LoadCase / mode",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Load Factor",
        "LF",
        "Load Factor for selected LoadCase / mode",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaModel = new GsaModel();
      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp))
        return;

      #region Inputs

      if (ghTyp.Value is GsaModelGoo)
        ghTyp.CastTo(ref gsaModel);
      else {
        this.AddRuntimeError("Error converting input to GSA Model");
        return;
      }

      var ghACase = new GH_Integer();
      da.GetData(1, ref ghACase);
      GH_Convert.ToInt32(ghACase, out int analCase, GH_Conversion.Both);

      #endregion

      #region Get results from GSA

      gsaModel.Model.Results()
        .TryGetValue(analCase, out AnalysisCaseResult analysisCaseResult);
      if (analysisCaseResult == null) {
        this.AddRuntimeError("No results exist for Analysis Case " + analCase + " in file");
        return;
      }

      #endregion

      const double unitfactorForce = 1000;
      const double unitfactorMoment = 1000;

      var force = new Vector3d(analysisCaseResult.Global.TotalLoad.X / unitfactorForce,
        analysisCaseResult.Global.TotalLoad.Y / unitfactorForce,
        analysisCaseResult.Global.TotalLoad.Z / unitfactorForce);

      var moment = new Vector3d(analysisCaseResult.Global.TotalLoad.XX / unitfactorMoment,
        analysisCaseResult.Global.TotalLoad.YY / unitfactorMoment,
        analysisCaseResult.Global.TotalLoad.ZZ / unitfactorMoment);

      var reaction = new Vector3d(analysisCaseResult.Global.TotalReaction.X / unitfactorForce,
        analysisCaseResult.Global.TotalReaction.Y / unitfactorForce,
        analysisCaseResult.Global.TotalReaction.Z / unitfactorForce);

      var reactionmoment = new Vector3d(
        analysisCaseResult.Global.TotalReaction.XX / unitfactorMoment,
        analysisCaseResult.Global.TotalReaction.YY / unitfactorMoment,
        analysisCaseResult.Global.TotalReaction.ZZ / unitfactorMoment);

      var effMass = new Vector3d(analysisCaseResult.Global.EffectiveMass.X,
        analysisCaseResult.Global.EffectiveMass.Y,
        analysisCaseResult.Global.EffectiveMass.Z);

      Vector3d effStiff;
      if (analysisCaseResult.Global.EffectiveInertia != null)
        effStiff = new Vector3d(analysisCaseResult.Global.EffectiveInertia.X,
          analysisCaseResult.Global.EffectiveInertia.Y,
          analysisCaseResult.Global.EffectiveInertia.Z);
      else
        effStiff = new Vector3d();

      var modal = new Vector3d(analysisCaseResult.Global.ModalMass,
        analysisCaseResult.Global.ModalStiffness,
        analysisCaseResult.Global.ModalGeometricStiffness);

      da.SetData(0, force);
      da.SetData(1, moment);
      da.SetData(2, reaction);
      da.SetData(3, reactionmoment);
      da.SetData(4, effMass);
      da.SetData(5, effStiff);
      da.SetData(6, analysisCaseResult.Global.Mode);
      da.SetData(7, modal);
      da.SetData(8, analysisCaseResult.Global.Frequency);
      da.SetData(9, analysisCaseResult.Global.LoadFactor);
    }
  }
}
