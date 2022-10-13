using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using GsaGH.Helpers;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create a new Offset
  /// </summary>
  public class CreateSectionModifier : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("e65d2554-75a9-4fac-9f12-1400e84aeee9");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateSectionModifier;

    public CreateSectionModifier()      : base("Create Section Modifier", 
      "SectionModifier", 
      "Create GSA Section Modifier",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        dropdownitems.Add(optionTypes);
        selecteditems.Add(optionTypes[0]);

        dropdownitems.Add(FilteredUnits.FilteredLinearDensityUnits);
        selecteditems.Add(densityUnit.ToString());

        dropdownitems.Add(stressOptions);
        selecteditems.Add(stressOptions[0]);

        first = false;
      }

      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }

    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      if (i == 0)
      {
        if (j == 0)
        {
          if (toMode == true)
          {
            dropdownitems.RemoveAt(1);
            selecteditems.RemoveAt(1);
            spacerDescriptions.RemoveAt(1);
          }
          toMode = false;
        }
        else
        {
          if (toMode == false)
          {
            dropdownitems.Insert(1, FilteredUnits.FilteredLengthUnits);
            selecteditems.Insert(1, lengthUnit.ToString());
            spacerDescriptions.Insert(1, "Length unit");
          }
          toMode = true;
        }
      }

      if (i == 1)
      {
        if (toMode == false)
          densityUnit = (LinearDensityUnit)Enum.Parse(typeof(LinearDensityUnit), selecteditems[i]);
        else
          lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[i]);
      }

      if (i == 2)
      {
        if (toMode == false)
        {
          if (j == 0)
            stressOption = GsaSectionModifier.StressOptionType.NoCalculation;
          if (j == 1)
            stressOption = GsaSectionModifier.StressOptionType.UseUnmodified;
          if (j == 2)
            stressOption = GsaSectionModifier.StressOptionType.UseModified;
        }
        else
          densityUnit = (LinearDensityUnit)Enum.Parse(typeof(LinearDensityUnit), selecteditems[i]);
      }

      if (i == 3)
      {
        if (j == 0)
          stressOption = GsaSectionModifier.StressOptionType.NoCalculation;
        if (j == 1)
          stressOption = GsaSectionModifier.StressOptionType.UseUnmodified;
        if (j == 2)
          stressOption = GsaSectionModifier.StressOptionType.UseModified;
      }

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      if (toMode)
      {

        lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[1]);
        densityUnit = (LinearDensityUnit)Enum.Parse(typeof(LinearDensityUnit), selecteditems[2]);
        if (selecteditems[3] == stressOptions[0])
          stressOption = GsaSectionModifier.StressOptionType.NoCalculation;
        if (selecteditems[3] == stressOptions[1])
          stressOption = GsaSectionModifier.StressOptionType.UseUnmodified;
        if (selecteditems[3] == stressOptions[2])
          stressOption = GsaSectionModifier.StressOptionType.UseModified;
      }
      else
      {
        densityUnit = (LinearDensityUnit)Enum.Parse(typeof(LinearDensityUnit), selecteditems[1]);
        if (selecteditems[2] == stressOptions[0])
          stressOption = GsaSectionModifier.StressOptionType.NoCalculation;
        if (selecteditems[2] == stressOptions[1])
          stressOption = GsaSectionModifier.StressOptionType.UseUnmodified;
        if (selecteditems[2] == stressOptions[2])
          stressOption = GsaSectionModifier.StressOptionType.UseModified;
      }

      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    #endregion

    #region Input and output
    // list of lists with all dropdown lists conctent
    List<List<string>> dropdownitems;
    // list of selected items
    List<string> selecteditems;
    // list of descriptions 
    List<string> spacerDescriptions = new List<string>(new string[]
    {
      "Modify type",
      "Density unit",
      "Stress calc."
    });
    List<string> optionTypes = new List<string>(new string[]
    {
      "Modify by",
      "Modify to"
    });
    List<string> stressOptions = new List<string>(new string[]
    {
      "Don't calculate",
      "Use unmodified",
      "Use modified",
    });
    private bool toMode = false;
    private bool first = true;
    private GsaSectionModifier.StressOptionType stressOption = GsaSectionModifier.StressOptionType.NoCalculation;
    private LinearDensityUnit densityUnit = DefaultUnits.LinearDensityUnit;
    private LengthUnit lengthUnit = DefaultUnits.LengthUnitSection;
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity quantity = new LinearDensity(0, densityUnit);
      string unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Area Modifier", "A", "[Optional] Modify the effective Area BY this decimal fraction value (Default = 1.0 -> 100%)", GH_ParamAccess.item);
      pManager.AddGenericParameter("I11 Modifier", "I11", "[Optional] Modify the effective Iyy/Iuu BY this decimal fraction value (Default = 1.0 -> 100%)", GH_ParamAccess.item);
      pManager.AddGenericParameter("I22 Modifier", "I22", "[Optional] Modify the effective Izz/Ivv BY this decimal fraction value (Default = 1.0 -> 100%)", GH_ParamAccess.item);
      pManager.AddGenericParameter("J Modifier", "J", "[Optional] Modify the effective J BY this decimal fraction value (Default = 1.0 -> 100%)", GH_ParamAccess.item);
      pManager.AddGenericParameter("K11 Modifier", "K11", "[Optional] Modify the effective Kyy/Kuu BY this decimal fraction value (Default = 1.0 -> 100%)", GH_ParamAccess.item);
      pManager.AddGenericParameter("K22 Modifier", "K22", "[Optional] Modify the effective Kzz/Kvv BY this decimal fraction value (Default = 1.0 -> 100%)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Volume Modifier", "V", "[Optional] Modify the effective Volume/Length BY this decimal fraction value (Default = 1.0 -> 100%)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Additional Mass [" + unitAbbreviation + "]", "+kg", "[Optional] Additional mass per unit length (Default = 0 -> no additional mass)", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Principal Bending Axis", "Ax", "[Optional] Set to 'true' to use Principal (u,v) Axis for Bending. If false (and by default), Local (y,z) Axis will be used", GH_ParamAccess.item, false);
      pManager.AddBooleanParameter("Reference Point Centroid", "Ref", "[Optional] Set to 'true' to use the Centroid as Analysis Reference Point. If false (and by default), the specified point will be used", GH_ParamAccess.item, false);

      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Section Modifier", "Mo", "GSA Section Modifier", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaSectionModifier modifier = new GsaSectionModifier();
      if (toMode)
      {
        AreaUnit areaUnit = UnitsHelper.GetAreaUnit(lengthUnit);
        AreaMomentOfInertiaUnit inertiaUnit = UnitsHelper.GetAreaMomentOfInertiaUnit(lengthUnit);
        VolumePerLengthUnit volUnit = UnitsHelper.GetVolumePerLengthUnit(lengthUnit);

        if (this.Params.Input[0].SourceCount > 0)
          modifier.AreaModifier = Input.UnitNumber(this, DA, 0, areaUnit, true);
        if (this.Params.Input[1].SourceCount > 0)
          modifier.I11Modifier = Input.UnitNumber(this, DA, 1, inertiaUnit, true);
        if (this.Params.Input[2].SourceCount > 0)
          modifier.I22Modifier = Input.UnitNumber(this, DA, 2, inertiaUnit, true);
        if (this.Params.Input[3].SourceCount > 0)
          modifier.JModifier = Input.UnitNumber(this, DA, 3, inertiaUnit, true);
        if (this.Params.Input[4].SourceCount > 0)
          modifier.K11Modifier = CustomInput.RatioInDecimalFractionToDecimalFraction(this, DA, 4);
        if (this.Params.Input[5].SourceCount > 0)
          modifier.K22Modifier = CustomInput.RatioInDecimalFractionToDecimalFraction(this, DA, 5);
        if (this.Params.Input[6].SourceCount > 0)
          modifier.VolumeModifier = Input.UnitNumber(this, DA, 6, volUnit, true);
      }
      else
      {
        modifier.AreaModifier = CustomInput.RatioInDecimalFractionToPercentage(this, DA, 0);
        modifier.I11Modifier = CustomInput.RatioInDecimalFractionToPercentage(this, DA, 1);
        modifier.I22Modifier = CustomInput.RatioInDecimalFractionToPercentage(this, DA, 2);
        modifier.JModifier = CustomInput.RatioInDecimalFractionToPercentage(this, DA, 3);
        modifier.K11Modifier = CustomInput.RatioInDecimalFractionToPercentage(this, DA, 4);
        modifier.K22Modifier = CustomInput.RatioInDecimalFractionToPercentage(this, DA, 5);
        modifier.VolumeModifier = CustomInput.RatioInDecimalFractionToPercentage(this, DA, 6);
      }

      modifier.AdditionalMass = (LinearDensity)Input.UnitNumber(this, DA, 7, densityUnit, true);
      
      bool ax = false;
      if (DA.GetData(8, ref ax))
        modifier.IsBendingAxesPrincipal = ax;

      bool pt = false;
      if (DA.GetData(9, ref pt))
        modifier.IsReferencePointCentroid = pt;

      modifier.StressOption = stressOption;

      DA.SetData(0, new GsaSectionModifierGoo(modifier));
    }

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      GsaGH.Util.GH.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
      writer.SetBoolean("toMode", toMode);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      GsaGH.Util.GH.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);
      toMode = reader.GetBoolean("toMode");
      UpdateUIFromSelectedItems();
      first = false;
      return base.Read(reader);
    }
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
    #endregion

    #region IGH_VariableParameterComponent null implementation
    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
      LinearDensity quantity = new LinearDensity(0, densityUnit);
      Params.Input[7].Name = "Additional Mass [" + quantity.ToString("a") + "]";
      if (toMode)
      {
        IQuantity len = new Length(0, lengthUnit);
        string unit = string.Concat(len.ToString().Where(char.IsLetter));

        VolumePerLength vol = new VolumePerLength(0, UnitsHelper.GetVolumePerLengthUnit(lengthUnit));
        string volUnit = vol.ToString("a");

        Params.Input[0].Name = "Area Modifier [" + unit + "\u00B2]";
        Params.Input[0].Description = "[Optional] Modify the effective Area TO this value";
        Params.Input[1].Name = "I11 Modifier [" + unit + "\u2074]";
        Params.Input[1].Description = "[Optional] Modify the effective Iyy/Iuu TO this value";
        Params.Input[2].Name = "I22 Modifier [" + unit + "\u2074]";
        Params.Input[2].Description = "[Optional] Modify the effective Izz/Ivv TO this value";
        Params.Input[3].Name = "J Modifier [" + unit + "\u2074]";
        Params.Input[3].Description = "[Optional] Modify the effective J TO this value";
        Params.Input[4].Name = "K11 Modifier [-]";
        Params.Input[4].Description = "[Optional] Modify the effective Kyy/Kuu TO this value";
        Params.Input[5].Name = "K22 Modifier [-]";
        Params.Input[5].Description = "[Optional] Modify the effective Kzz/Kvv TO this value";
        Params.Input[6].Name = "Volume Modifier [" + volUnit + "]";
        Params.Input[6].Description = "[Optional] Modify the effective Volume/Length TO this value";
      }
      else
      {
        Params.Input[0].Name = "Area Modifier";
        Params.Input[0].Description = "[Optional] Modify the effective Area BY this decimal fraction value (Default = 1.0 -> 100%)";
        Params.Input[1].Name = "I11 Modifier";
        Params.Input[1].Description = "[Optional] Modify the effective Iyy/Iuu BY this decimal fraction value (Default = 1.0 -> 100%)";
        Params.Input[2].Name = "I22 Modifier";
        Params.Input[2].Description = "[Optional] Modify the effective Izz/Ivv BY this decimal fraction value (Default = 1.0 -> 100%)";
        Params.Input[3].Name = "J Modifier";
        Params.Input[3].Description = "[Optional] Modify the effective J BY this decimal fraction value (Default = 1.0 -> 100%)";
        Params.Input[4].Name = "K11 Modifier";
        Params.Input[4].Description = "[Optional] Modify the effective Kyy/Kuu BY this decimal fraction value (Default = 1.0 -> 100%)";
        Params.Input[5].Name = "K22 Modifier";
        Params.Input[5].Description = "[Optional] Modify the effective Kzz/Kvv BY this decimal fraction value (Default = 1.0 -> 100%)";
        Params.Input[6].Name = "Volume Modifier";
        Params.Input[6].Description = "[Optional] Modify the effective Volume/Length BY this decimal fraction value (Default = 1.0 -> 100%)";
      }
    }
    #endregion
  }
}

