using System;
using System.Collections.Generic;
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
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get GSA global performance results
  /// </summary>
  public class GlobalPerformanceResults : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("2346fd74-b348-47b6-bcaa-31526afe1f7b");
    public override GH_Exposure Exposure => GH_Exposure.septenary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GlobalPerformanceResults;
    private ForcePerLengthUnit _forcePerLengthUnit = ForcePerLengthUnit.KilonewtonPerMeter;
    private AreaMomentOfInertiaUnit _inertiaUnit = AreaMomentOfInertiaUnit.MeterToTheFourth;
    private MassUnit _massUnit = DefaultUnits.MassUnit;

    public GlobalPerformanceResults() : base("Global Performance Results", "GlobalPerformance",
      "Get Global Performance (Dynamic, Model Stability, and Buckling) Results from a GSA model",
      CategoryName.Name(), SubCategoryName.Cat5()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      switch (i) {
        case 0:
          _massUnit = (MassUnit)UnitsHelper.Parse(typeof(MassUnit), _selectedItems[i]);
          break;

        case 1:
          _inertiaUnit
            = (AreaMomentOfInertiaUnit)UnitsHelper.Parse(typeof(AreaMomentOfInertiaUnit),
              _selectedItems[i]);
          break;

        case 2:
          _forcePerLengthUnit
            = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), _selectedItems[i]);
          break;
      }

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string massUnitAbbreviation = Mass.GetAbbreviation(_massUnit);
      string inertiaUnitAbbreviation = AreaMomentOfInertia.GetAbbreviation(_inertiaUnit);
      string forceperlengthUnitAbbreviation = ForcePerLength.GetAbbreviation(_forcePerLengthUnit);

      int i = 0;
      Params.Output[i++].Name = "Effective Mass X [" + massUnitAbbreviation + "]";
      Params.Output[i++].Name = "Effective Mass Y [" + massUnitAbbreviation + "]";
      Params.Output[i++].Name = "Effective Mass Z [" + massUnitAbbreviation + "]";
      Params.Output[i++].Name = "Effective Inertia X [" + inertiaUnitAbbreviation + "]";
      Params.Output[i++].Name = "Effective Inertia Y [" + inertiaUnitAbbreviation + "]";
      Params.Output[i++].Name = "Effective Inertia Z [" + inertiaUnitAbbreviation + "]";
      i++;
      Params.Output[i++].Name = "Modal Mass [" + massUnitAbbreviation + "]";
      Params.Output[i++].Name = "Modal Stiffness [" + forceperlengthUnitAbbreviation + "]";
      Params.Output[i].Name = "Modal Geometric Stiffness [" + forceperlengthUnitAbbreviation + "]";
    }

    protected override void InitialiseDropdowns() {
      if ((DefaultUnits.LengthUnitGeometry == LengthUnit.Foot)
        | (DefaultUnits.LengthUnitGeometry == LengthUnit.Inch)) {
        _inertiaUnit = AreaMomentOfInertiaUnit.FootToTheFourth;
        _forcePerLengthUnit = ForcePerLengthUnit.KilopoundForcePerFoot;
      }

      _spacerDescriptions = new List<string>(new[] {
        "Mass Unit",
        "Inertia Unit",
        "Stiffness Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Mass));
      _selectedItems.Add(Mass.GetAbbreviation(_massUnit));

      _dropDownItems.Add(
        UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.AreaMomentOfInertia));
      _selectedItems.Add(AreaMomentOfInertia.GetAbbreviation(_inertiaUnit));

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerLength));
      _selectedItems.Add(ForcePerLength.GetAbbreviation(_forcePerLengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string massUnitAbbreviation = Mass.GetAbbreviation(_massUnit);
      string inertiaUnitAbbreviation = AreaMomentOfInertia.GetAbbreviation(_inertiaUnit);
      string forceperlengthUnitAbbreviation = ForcePerLength.GetAbbreviation(_forcePerLengthUnit);

      pManager.AddGenericParameter("Effective Mass X [" + massUnitAbbreviation + "]", "Σmx",
        "Effective Mass in GSA Model in X-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Effective Mass Y [" + massUnitAbbreviation + "]", "Σmy",
        "Effective Mass in GSA Model in Y-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Effective Mass Z [" + massUnitAbbreviation + "]", "Σmz",
        "Effective Mass in GSA Model in Z-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Effective Inertia X [" + inertiaUnitAbbreviation + "]", "ΣIx",
        "Effective Inertia in GSA Model in X-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Effective Inertia Y [" + inertiaUnitAbbreviation + "]", "ΣIy",
        "Effective Inertia in GSA Model in Y-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Effective Inertia Z [" + inertiaUnitAbbreviation + "]", "ΣIz",
        "Effective Inertia in GSA Model in Z-direction", GH_ParamAccess.item);
      pManager.AddNumberParameter("Mode", "Mo", "Mode number if LC is a dynamic task",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Modal Mass [" + massUnitAbbreviation + "]", "MM",
        "Modal Mass of selected LoadCase / mode", GH_ParamAccess.item);
      pManager.AddGenericParameter("Modal Stiffness [" + forceperlengthUnitAbbreviation + "]", "MS",
        "Modal Stiffness of selected LoadCase / mode", GH_ParamAccess.item);
      pManager.AddGenericParameter(
        "Modal Geometric Stiffness [" + forceperlengthUnitAbbreviation + "]", "MGS",
        "Modal Geometric Stiffness of selected LoadCase / mode", GH_ParamAccess.item);
      pManager.AddGenericParameter("Frequency [Hz]", "f", "Frequency of selected LoadCase / mode",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Load Factor", "LF", "Load Factor for selected LoadCase / mode",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Eigenvalue", "Eig", "Eigenvalue for selected LoadCase / mode",
        GH_ParamAccess.item);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      GsaResult result;
      GH_ObjectWrapper ghTyp = null;
      da.GetData(0, ref ghTyp);

      result = Inputs.GetResultInput(this, ghTyp);
      if (result == null) {
        return;
      }

      if (result.CaseType == CaseType.CombinationCase) {
        this.AddRuntimeError("Global Result only available for Analysis Cases");
        return;
      }

      IGlobalResultsCache globalResultsCache = result.GlobalResults;
      int i = 0;
      IEffectiveMass mass = globalResultsCache.EffectiveMass;
      da.SetData(i++, new GH_UnitNumber(mass.X.ToUnit(_massUnit)));
      da.SetData(i++, new GH_UnitNumber(mass.Y.ToUnit(_massUnit)));
      da.SetData(i++, new GH_UnitNumber(mass.Z.ToUnit(_massUnit)));
      if (globalResultsCache.EffectiveInertia != null) {
        IEffectiveInertia stiff = globalResultsCache.EffectiveInertia;
        da.SetData(i++, new GH_UnitNumber(stiff.X.ToUnit(_inertiaUnit)));
        da.SetData(i++, new GH_UnitNumber(stiff.Y.ToUnit(_inertiaUnit)));
        da.SetData(i++, new GH_UnitNumber(stiff.Z.ToUnit(_inertiaUnit)));
      } else {
        da.SetData(i++, null);
        da.SetData(i++, null);
        da.SetData(i++, null);
      }

      da.SetData(i++, globalResultsCache.Mode);

      da.SetData(i++,
        globalResultsCache.ModalMass.Value != 0 ?
          new GH_UnitNumber(globalResultsCache.ModalMass.ToUnit(_massUnit)) : null);

      da.SetData(i++,
        !(globalResultsCache.Frequency.Value == 0 && globalResultsCache.LoadFactor.Value == 0) ?
          new GH_UnitNumber(globalResultsCache.ModalStiffness.ToUnit(_forcePerLengthUnit)) : null);

      da.SetData(i++,
        globalResultsCache.ModalGeometricStiffness.Value != 0 ?
          new GH_UnitNumber(
            globalResultsCache.ModalGeometricStiffness.ToUnit(_forcePerLengthUnit)) : null);

      da.SetData(i++,
        globalResultsCache.Frequency.Value != 0 ? new GH_UnitNumber(globalResultsCache.Frequency) :
          null);

      da.SetData(i++,
        globalResultsCache.LoadFactor.Value != 0 ?
          new GH_UnitNumber(globalResultsCache.LoadFactor) : null);

      if (globalResultsCache.Frequency.Value == 0 && globalResultsCache.LoadFactor.Value == 0
        && globalResultsCache.ModalStiffness.Value != 0) {
        da.SetData(i,
          new GH_UnitNumber(globalResultsCache.ModalStiffness.ToUnit(_forcePerLengthUnit)));
      } else {
        da.SetData(i, null);
      }

      PostHog.Result(result.CaseType, -1, "Global", "Performance");
    }

    protected override void UpdateUIFromSelectedItems() {
      _massUnit = (MassUnit)UnitsHelper.Parse(typeof(MassUnit), _selectedItems[0]);
      _inertiaUnit
        = (AreaMomentOfInertiaUnit)UnitsHelper.Parse(typeof(AreaMomentOfInertiaUnit),
          _selectedItems[1]);
      _forcePerLengthUnit
        = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), _selectedItems[2]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
