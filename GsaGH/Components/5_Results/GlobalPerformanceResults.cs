using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Util.GH;
using GsaGH.Util.Gsa;
using OasysGH.Parameters;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  public class GlobalPerformanceResults : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("9a0b6077-1cb6-405c-85d3-c24a533d6d43");
    public GlobalPerformanceResults()
      : base("Global Performance Results", "GlobalPerformance", "Get Global Performance (Dynamic, Model Stability, and Buckling) Results from a GSA model",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override GH_Exposure Exposure => GH_Exposure.septenary | GH_Exposure.obscure;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.GlobalPerformance;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    #endregion
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        dropdownitems.Add(Units.FilteredMassUnits);
        dropdownitems.Add(Units.FilteredAreaMomentOfInertiaUnits);
        dropdownitems.Add(Units.FilteredForcePerLengthUnits);

        selecteditems = new List<string>();
        selecteditems.Add(Units.MassUnit.ToString());
        if (Units.LengthUnitGeometry == LengthUnit.Foot | Units.LengthUnitGeometry == LengthUnit.Inch)
        {
          selecteditems.Add(AreaMomentOfInertiaUnit.FootToTheFourth.ToString());
          selecteditems.Add(ForcePerLengthUnit.KilopoundForcePerFoot.ToString());
        }
        else
        {
          selecteditems.Add(AreaMomentOfInertiaUnit.MeterToTheFourth.ToString());
          selecteditems.Add(ForcePerLengthUnit.KilonewtonPerMeter.ToString());
        }

        first = false;
      }

      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      switch (i)
      {
        case 0:
          massUnit = (MassUnit)Enum.Parse(typeof(MassUnit), selecteditems[i]);
          break;
        case 1:
          inertiaUnit = (AreaMomentOfInertiaUnit)Enum.Parse(typeof(AreaMomentOfInertiaUnit), selecteditems[i]);
          break;
        case 2:
          forcePerLengthUnit = (ForcePerLengthUnit)Enum.Parse(typeof(ForcePerLengthUnit), selecteditems[i]);
          break;
      }
        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    private void UpdateUIFromSelectedItems()
    {
      massUnit = (MassUnit)Enum.Parse(typeof(MassUnit), selecteditems[0]);
      inertiaUnit = (AreaMomentOfInertiaUnit)Enum.Parse(typeof(AreaMomentOfInertiaUnit), selecteditems[1]);
      forcePerLengthUnit = (ForcePerLengthUnit)Enum.Parse(typeof(ForcePerLengthUnit), selecteditems[2]);

      CreateAttributes();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    // list of lists with all dropdown lists conctent
    List<List<string>> dropdownitems;
    // list of selected items
    List<string> selecteditems;
    // list of descriptions 
    List<string> spacerDescriptions = new List<string>(new string[]
    {
            "Mass Unit",
            "Inertia Unit",
            "Stiffness Unit",
    });

    private MassUnit massUnit = Units.MassUnit;
    private AreaMomentOfInertiaUnit inertiaUnit = Units.SectionAreaMomentOfInertiaUnit;
    private ForcePerLengthUnit forcePerLengthUnit = Units.ForcePerLengthUnit;
    bool first = true;
    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Result", "Res", "GSA Result", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      string massUnitAbbreviation = Mass.GetAbbreviation(massUnit);
      string inertiaUnitAbbreviation = AreaMomentOfInertia.GetAbbreviation(inertiaUnit);
      string forceperlengthUnitAbbreviation = ForcePerLength.GetAbbreviation(forcePerLengthUnit);

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
        if (gh_typ.Value is GsaResultGoo)
        {
          result = ((GsaResultGoo)gh_typ.Value).Value;
          if (result.Type == GsaResult.ResultType.Combination)
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

        GsaResultQuantity mass = ResultHelper.GetQuantityResult(analysisCaseResult.Global.EffectiveMass, massUnit);
        DA.SetData(i++, new GH_UnitNumber(mass.X));
        DA.SetData(i++, new GH_UnitNumber(mass.Y));
        DA.SetData(i++, new GH_UnitNumber(mass.Z));
        DA.SetData(i++, new GH_UnitNumber(mass.XYZ));

        if (analysisCaseResult.Global.EffectiveInertia != null)
        {
          GsaResultQuantity stiff = ResultHelper.GetQuantityResult(analysisCaseResult.Global.EffectiveInertia, inertiaUnit);
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
          DA.SetData(i++, new GH_UnitNumber(mmass.ToUnit(massUnit)));
        }
        else
          DA.SetData(i++, null);

        if (!(analysisCaseResult.Global.Frequency == 0 && analysisCaseResult.Global.LoadFactor == 0))
        {
          IQuantity mstiff = new ForcePerLength(analysisCaseResult.Global.ModalStiffness, ForcePerLengthUnit.NewtonPerMeter);
          DA.SetData(i++, new GH_UnitNumber(mstiff.ToUnit(forcePerLengthUnit)));
        }
        else
          DA.SetData(i++, null);

        if (analysisCaseResult.Global.ModalGeometricStiffness != 0)
        {
          IQuantity geostiff = new ForcePerLength(analysisCaseResult.Global.ModalGeometricStiffness, ForcePerLengthUnit.NewtonPerMeter);
          DA.SetData(i++, new GH_UnitNumber(geostiff.ToUnit(forcePerLengthUnit)));
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
      }
    }
    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);
      
      first = false;
      UpdateUIFromSelectedItems();
      return base.Read(reader);
    }
    #endregion
    #region IGH_VariableParameterComponent null implementation

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
    {
      return null;
    }
    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
      string massUnitAbbreviation = Mass.GetAbbreviation(massUnit);
      string inertiaUnitAbbreviation = AreaMomentOfInertia.GetAbbreviation(inertiaUnit);
      string forceperlengthUnitAbbreviation = ForcePerLength.GetAbbreviation(forcePerLengthUnit);

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

