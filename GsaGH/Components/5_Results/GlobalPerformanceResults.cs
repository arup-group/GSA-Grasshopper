using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Helpers.GsaAPI;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using GsaGH.Helpers.GH;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to get GSA global performance results
    /// </summary>
    public class GlobalPerformanceResults : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("9a0b6077-1cb6-405c-85d3-c24a533d6d43");
    public override GH_Exposure Exposure => GH_Exposure.septenary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.GlobalPerformance;

    public GlobalPerformanceResults() : base("Global Performance Results",
      "GlobalPerformance",
      "Get Global Performance (Dynamic, Model Stability, and Buckling) Results from a GSA model",
      CategoryName.Name(),
      SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      string massUnitAbbreviation = Mass.GetAbbreviation(MassUnit);
      string inertiaUnitAbbreviation = AreaMomentOfInertia.GetAbbreviation(InertiaUnit);
      string forceperlengthUnitAbbreviation = ForcePerLength.GetAbbreviation(ForcePerLengthUnit);

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

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // Result to work on
      GsaResult result = new GsaResult();

      // Get Model
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        #region Inputs
        if (gh_typ == null || gh_typ.Value == null)
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input is null");
          return;
        }
        if (gh_typ.Value is GsaResultGoo)
        {
          result = ((GsaResultGoo)gh_typ.Value).Value;
          if (result.Type == GsaResult.CaseType.Combination)
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Global Result only available for Analysis Cases");
            return;
          }
        }
        else
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Result");
          return;
        }
        #endregion

        #region Get results from GSA
        // ### Get results ###
        //Get analysis case from model
        AnalysisCaseResult analysisCaseResult = result.AnalysisCaseResult;
        #endregion
        int i = 0;

        GsaResultQuantity mass = ResultHelper.GetQuantityResult(analysisCaseResult.Global.EffectiveMass, MassUnit);
        DA.SetData(i++, new GH_UnitNumber(mass.X));
        DA.SetData(i++, new GH_UnitNumber(mass.Y));
        DA.SetData(i++, new GH_UnitNumber(mass.Z));
        DA.SetData(i++, new GH_UnitNumber(mass.XYZ));

        if (analysisCaseResult.Global.EffectiveInertia != null)
        {
          GsaResultQuantity stiff = ResultHelper.GetQuantityResult(analysisCaseResult.Global.EffectiveInertia, InertiaUnit);
          DA.SetData(i++, new GH_UnitNumber(stiff.X));
          DA.SetData(i++, new GH_UnitNumber(stiff.Y));
          DA.SetData(i++, new GH_UnitNumber(stiff.Z));
          DA.SetData(i++, new GH_UnitNumber(stiff.XYZ));
        }
        else
        {
          DA.SetData(i++, null);
          DA.SetData(i++, null);
          DA.SetData(i++, null);
          DA.SetData(i++, null);
        }

        if (analysisCaseResult.Global.Mode != 0)
          DA.SetData(i++, analysisCaseResult.Global.Mode);
        else
          DA.SetData(i++, null);

        if (analysisCaseResult.Global.ModalMass != 0)
        {
          IQuantity mmass = new Mass(analysisCaseResult.Global.ModalMass, MassUnit.Kilogram);
          DA.SetData(i++, new GH_UnitNumber(mmass.ToUnit(MassUnit)));
        }
        else
          DA.SetData(i++, null);

        if (!(analysisCaseResult.Global.Frequency == 0 && analysisCaseResult.Global.LoadFactor == 0))
        {
          IQuantity mstiff = new ForcePerLength(analysisCaseResult.Global.ModalStiffness, ForcePerLengthUnit.NewtonPerMeter);
          DA.SetData(i++, new GH_UnitNumber(mstiff.ToUnit(ForcePerLengthUnit)));
        }
        else
          DA.SetData(i++, null);

        if (analysisCaseResult.Global.ModalGeometricStiffness != 0)
        {
          IQuantity geostiff = new ForcePerLength(analysisCaseResult.Global.ModalGeometricStiffness, ForcePerLengthUnit.NewtonPerMeter);
          DA.SetData(i++, new GH_UnitNumber(geostiff.ToUnit(ForcePerLengthUnit)));
        }
        else
          DA.SetData(i++, null);

        if (analysisCaseResult.Global.Frequency != 0)
          DA.SetData(i++, new GH_UnitNumber(new Frequency(analysisCaseResult.Global.Frequency, FrequencyUnit.Hertz)));
        else
          DA.SetData(i++, null);

        if (analysisCaseResult.Global.LoadFactor != 0)
          DA.SetData(i++, analysisCaseResult.Global.LoadFactor);
        else
          DA.SetData(i++, null);

        if (analysisCaseResult.Global.Frequency == 0 && analysisCaseResult.Global.LoadFactor == 0 && analysisCaseResult.Global.ModalStiffness != 0)
          DA.SetData(i++, analysisCaseResult.Global.ModalStiffness);
        else
          DA.SetData(i++, null);

        Helpers.PostHogResultsHelper.PostHog(result.Type, -1, "Global", "Performance");
      }
    }

    #region Custom UI
    MassUnit MassUnit = DefaultUnits.MassUnit;
    AreaMomentOfInertiaUnit InertiaUnit = AreaMomentOfInertiaUnit.MeterToTheFourth;
    ForcePerLengthUnit ForcePerLengthUnit = ForcePerLengthUnit.KilonewtonPerMeter;

    public override void InitialiseDropdowns()
    {
      if (DefaultUnits.LengthUnitGeometry == LengthUnit.Foot | DefaultUnits.LengthUnitGeometry == LengthUnit.Inch)
      {
        this.InertiaUnit = AreaMomentOfInertiaUnit.FootToTheFourth;
        this.ForcePerLengthUnit = ForcePerLengthUnit.KilopoundForcePerFoot;
      }

      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Mass Unit", "Inertia Unit", "Stiffness Unit"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations((EngineeringUnits.Mass)));
      this.SelectedItems.Add(Mass.GetAbbreviation(this.MassUnit));

      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations((EngineeringUnits.AreaMomentOfInertia)));
      this.SelectedItems.Add(AreaMomentOfInertia.GetAbbreviation((this.InertiaUnit)));

      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations((EngineeringUnits.ForcePerLength)));
      this.SelectedItems.Add(ForcePerLength.GetAbbreviation((this.ForcePerLengthUnit)));

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      switch (i)
      {
        case 0:
          this.MassUnit = (MassUnit)UnitsHelper.Parse(typeof(MassUnit), SelectedItems[i]);
          break;
        case 1:
          this.InertiaUnit = (AreaMomentOfInertiaUnit)UnitsHelper.Parse(typeof(AreaMomentOfInertiaUnit), SelectedItems[i]);
          break;
        case 2:
          this.ForcePerLengthUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), SelectedItems[i]);
          break;
      }
      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems()
    {
      this.MassUnit = (MassUnit)UnitsHelper.Parse(typeof(MassUnit), SelectedItems[0]);
      this.InertiaUnit = (AreaMomentOfInertiaUnit)UnitsHelper.Parse(typeof(AreaMomentOfInertiaUnit), SelectedItems[1]);
      this.ForcePerLengthUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), SelectedItems[2]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string massUnitAbbreviation = Mass.GetAbbreviation(MassUnit);
      string inertiaUnitAbbreviation = AreaMomentOfInertia.GetAbbreviation(InertiaUnit);
      string forceperlengthUnitAbbreviation = ForcePerLength.GetAbbreviation(ForcePerLengthUnit);

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
      Params.Output[i++].Name = "Modal Geometric Stiffness [" + forceperlengthUnitAbbreviation + "]";
    }
    #endregion
  }
}

