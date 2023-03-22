using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaAPI;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  /// Component to get GSA global performance results
  /// </summary>
  public class GlobalPerformanceResults : GH_OasysDropDownComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("9a0b6077-1cb6-405c-85d3-c24a533d6d43");
    public override GH_Exposure Exposure => GH_Exposure.septenary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GlobalPerformance;

    public GlobalPerformanceResults() : base("Global Performance Results",
      "GlobalPerformance",
      "Get Global Performance (Dynamic, Model Stability, and Buckling) Results from a GSA model",
      CategoryName.Name(),
      SubCategoryName.Cat5()) {
      Hidden = true;
    }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string massUnitAbbreviation = Mass.GetAbbreviation(_massUnit);
      string inertiaUnitAbbreviation = AreaMomentOfInertia.GetAbbreviation(_inertiaUnit);
      string forceperlengthUnitAbbreviation = ForcePerLength.GetAbbreviation(_forcePerLengthUnit);

      pManager.AddGenericParameter("Effective Mass X [" + massUnitAbbreviation + "]", "Σmx", "Effective Mass in GSA Model in X-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Effective Mass Y [" + massUnitAbbreviation + "]", "Σmy", "Effective Mass in GSA Model in Y-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Effective Mass Z [" + massUnitAbbreviation + "]", "Σmz", "Effective Mass in GSA Model in Z-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Effective Mass |XYZ| [" + massUnitAbbreviation + "]", "Σ|m|", "Effective Mass in GSA Model", GH_ParamAccess.item);
      pManager.AddGenericParameter("Effective Inertia X [" + inertiaUnitAbbreviation + "]", "ΣIx", "Effective Inertia in GSA Model in X-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Effective Inertia Y [" + inertiaUnitAbbreviation + "]", "ΣIy", "Effective Inertia in GSA Model in Y-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Effective Inertia Z [" + inertiaUnitAbbreviation + "]", "ΣIz", "Effective Inertia in GSA Model in Z-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Effective Inertia |XYZ| [" + inertiaUnitAbbreviation + "]", "Σ|I|", "Effective Inertia in GSA Model", GH_ParamAccess.item);
      pManager.AddNumberParameter("Mode", "Mo", "Mode number if LC is a dynamic task", GH_ParamAccess.item);
      pManager.AddGenericParameter("Modal Mass [" + massUnitAbbreviation + "]", "MM", "Modal Mass of selected LoadCase / mode", GH_ParamAccess.item);
      pManager.AddGenericParameter("Modal Stiffness [" + forceperlengthUnitAbbreviation + "]", "MS", "Modal Stiffness of selected LoadCase / mode", GH_ParamAccess.item);
      pManager.AddGenericParameter("Modal Geometric Stiffness [" + forceperlengthUnitAbbreviation + "]", "MGS", "Modal Geometric Stiffness of selected LoadCase / mode", GH_ParamAccess.item);
      pManager.AddGenericParameter("Frequency [Hz]", "f", "Frequency of selected LoadCase / mode", GH_ParamAccess.item);
      pManager.AddNumberParameter("Load Factor", "LF", "Load Factor for selected LoadCase / mode", GH_ParamAccess.item);
      pManager.AddNumberParameter("Eigenvalue", "Eig", "Eigenvalue for selected LoadCase / mode", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var result = new GsaResult();
      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp)) {
        return;
      }

      #region Inputs
      switch (ghTyp?.Value) {
        case null:
          this.AddRuntimeWarning("Input is null");
          return;
        case GsaResultGoo goo: {
            result = goo.Value;
            if (result.Type == GsaResult.CaseType.Combination) {
              this.AddRuntimeError("Global Result only available for Analysis Cases");
              return;
            }

            break;
          }
        default:
          this.AddRuntimeError("Error converting input to GSA Result");
          return;
      }

      #endregion

      #region Get results from GSA
      AnalysisCaseResult analysisCaseResult = result.AnalysisCaseResult;
      #endregion
      int i = 0;

      GsaResultQuantity mass = ResultHelper.GetQuantityResult(analysisCaseResult.Global.EffectiveMass, _massUnit);
      da.SetData(i++, new GH_UnitNumber(mass.X));
      da.SetData(i++, new GH_UnitNumber(mass.Y));
      da.SetData(i++, new GH_UnitNumber(mass.Z));
      da.SetData(i++, new GH_UnitNumber(mass.XYZ));

      if (analysisCaseResult.Global.EffectiveInertia != null) {
        GsaResultQuantity stiff = ResultHelper.GetQuantityResult(analysisCaseResult.Global.EffectiveInertia, _inertiaUnit);
        da.SetData(i++, new GH_UnitNumber(stiff.X));
        da.SetData(i++, new GH_UnitNumber(stiff.Y));
        da.SetData(i++, new GH_UnitNumber(stiff.Z));
        da.SetData(i++, new GH_UnitNumber(stiff.XYZ));
      }
      else {
        da.SetData(i++, null);
        da.SetData(i++, null);
        da.SetData(i++, null);
        da.SetData(i++, null);
      }

      if (analysisCaseResult.Global.Mode != 0)
        da.SetData(i++, analysisCaseResult.Global.Mode);
      else
        da.SetData(i++, null);

      if (analysisCaseResult.Global.ModalMass != 0) {
        IQuantity mmass = new Mass(analysisCaseResult.Global.ModalMass, MassUnit.Kilogram);
        da.SetData(i++, new GH_UnitNumber(mmass.ToUnit(_massUnit)));
      }
      else
        da.SetData(i++, null);

      if (!(analysisCaseResult.Global.Frequency == 0 && analysisCaseResult.Global.LoadFactor == 0)) {
        IQuantity mstiff = new ForcePerLength(analysisCaseResult.Global.ModalStiffness, ForcePerLengthUnit.NewtonPerMeter);
        da.SetData(i++, new GH_UnitNumber(mstiff.ToUnit(_forcePerLengthUnit)));
      }
      else
        da.SetData(i++, null);

      if (analysisCaseResult.Global.ModalGeometricStiffness != 0) {
        IQuantity geostiff = new ForcePerLength(analysisCaseResult.Global.ModalGeometricStiffness, ForcePerLengthUnit.NewtonPerMeter);
        da.SetData(i++, new GH_UnitNumber(geostiff.ToUnit(_forcePerLengthUnit)));
      }
      else
        da.SetData(i++, null);

      da.SetData(i++,
        analysisCaseResult.Global.Frequency != 0
          ? new GH_UnitNumber(new Frequency(analysisCaseResult.Global.Frequency, FrequencyUnit.Hertz))
          : null);

      if (analysisCaseResult.Global.LoadFactor != 0)
        da.SetData(i++, analysisCaseResult.Global.LoadFactor);
      else
        da.SetData(i++, null);

      if (analysisCaseResult.Global.Frequency == 0 && analysisCaseResult.Global.LoadFactor == 0 && analysisCaseResult.Global.ModalStiffness != 0)
        da.SetData(i, analysisCaseResult.Global.ModalStiffness);
      else
        da.SetData(i, null);

      Helpers.PostHog.Result(result.Type, -1, "Global", "Performance");
    }

    #region Custom UI
    private MassUnit _massUnit = DefaultUnits.MassUnit;
    private AreaMomentOfInertiaUnit _inertiaUnit = AreaMomentOfInertiaUnit.MeterToTheFourth;
    private ForcePerLengthUnit _forcePerLengthUnit = ForcePerLengthUnit.KilonewtonPerMeter;

    public override void InitialiseDropdowns() {
      if (DefaultUnits.LengthUnitGeometry == LengthUnit.Foot | DefaultUnits.LengthUnitGeometry == LengthUnit.Inch) {
        _inertiaUnit = AreaMomentOfInertiaUnit.FootToTheFourth;
        _forcePerLengthUnit = ForcePerLengthUnit.KilopoundForcePerFoot;
      }

      SpacerDescriptions = new List<string>(new[]
        {
          "Mass Unit",
          "Inertia Unit",
          "Stiffness Unit",
        });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations((EngineeringUnits.Mass)));
      SelectedItems.Add(Mass.GetAbbreviation(_massUnit));

      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations((EngineeringUnits.AreaMomentOfInertia)));
      SelectedItems.Add(AreaMomentOfInertia.GetAbbreviation((_inertiaUnit)));

      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations((EngineeringUnits.ForcePerLength)));
      SelectedItems.Add(ForcePerLength.GetAbbreviation((_forcePerLengthUnit)));

      IsInitialised = true;
    }

    public override void SetSelected(int i, int j) {
      SelectedItems[i] = DropDownItems[i][j];
      switch (i) {
        case 0:
          _massUnit = (MassUnit)UnitsHelper.Parse(typeof(MassUnit), SelectedItems[i]);
          break;
        case 1:
          _inertiaUnit = (AreaMomentOfInertiaUnit)UnitsHelper.Parse(typeof(AreaMomentOfInertiaUnit), SelectedItems[i]);
          break;
        case 2:
          _forcePerLengthUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), SelectedItems[i]);
          break;
      }
      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems() {
      _massUnit = (MassUnit)UnitsHelper.Parse(typeof(MassUnit), SelectedItems[0]);
      _inertiaUnit = (AreaMomentOfInertiaUnit)UnitsHelper.Parse(typeof(AreaMomentOfInertiaUnit), SelectedItems[1]);
      _forcePerLengthUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), SelectedItems[2]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance() {
      string massUnitAbbreviation = Mass.GetAbbreviation(_massUnit);
      string inertiaUnitAbbreviation = AreaMomentOfInertia.GetAbbreviation(_inertiaUnit);
      string forceperlengthUnitAbbreviation = ForcePerLength.GetAbbreviation(_forcePerLengthUnit);

      int i = 0;
      Params.Output[i++].Name = "Effective Mass X [" + massUnitAbbreviation + "]";
      Params.Output[i++].Name = "Effective Mass Y [" + massUnitAbbreviation + "]";
      Params.Output[i++].Name = "Effective Mass Z [" + massUnitAbbreviation + "]";
      Params.Output[i++].Name = "Effective Mass |XYZ| [" + massUnitAbbreviation + "]";
      Params.Output[i++].Name = "Effective Inertia X [" + inertiaUnitAbbreviation + "]";
      Params.Output[i++].Name = "Effective Inertia Y [" + inertiaUnitAbbreviation + "]";
      Params.Output[i++].Name = "Effective Inertia Z [" + inertiaUnitAbbreviation + "]";
      Params.Output[i++].Name = "Effective Inertia |XYZ| [" + inertiaUnitAbbreviation + "]";
      i++;
      Params.Output[i++].Name = "Modal Mass [" + massUnitAbbreviation + "]";
      Params.Output[i++].Name = "Modal Stiffness [" + forceperlengthUnitAbbreviation + "]";
      Params.Output[i].Name = "Modal Geometric Stiffness [" + forceperlengthUnitAbbreviation + "]";
    }
    #endregion
  }
}

