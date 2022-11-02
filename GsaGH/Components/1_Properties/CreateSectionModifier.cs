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
  public class CreateSectionModifier : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("e65d2554-75a9-4fac-9f12-1400e84aeee9");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateSectionModifier;

    public CreateSectionModifier() 
      : base("Create Section Modifier", 
      "SectionModifier", 
      "Create GSA Section Modifier",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = LinearDensity.GetAbbreviation(this.DensityUnit);

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
      pManager.AddParameter(new GsaSectionModifierParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaSectionModifier modifier = new GsaSectionModifier();
      if (this._toMode)
      {
        AreaUnit areaUnit = UnitsHelper.GetAreaUnit(this.LengthUnit);
        AreaMomentOfInertiaUnit inertiaUnit = UnitsHelper.GetAreaMomentOfInertiaUnit(this.LengthUnit);
        VolumePerLengthUnit volUnit = UnitsHelper.GetVolumePerLengthUnit(this.LengthUnit);

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

      modifier.AdditionalMass = (LinearDensity)Input.UnitNumber(this, DA, 7, DensityUnit, true);
      
      bool ax = false;
      if (DA.GetData(8, ref ax))
        modifier.IsBendingAxesPrincipal = ax;

      bool pt = false;
      if (DA.GetData(9, ref pt))
        modifier.IsReferencePointCentroid = pt;

      modifier.StressOption = this.StressOption;

      DA.SetData(0, new GsaSectionModifierGoo(modifier));
    }

    #region Custom UI
    List<string> _optionTypes = new List<string>(new string[]
    {
      "Modify by",
      "Modify to"
    });
    List<string> _stressOptions = new List<string>(new string[]
    {
      "Don't calculate",
      "Use unmodified",
      "Use modified",
    });
    private bool _toMode = false;
    private GsaSectionModifier.StressOptionType StressOption = GsaSectionModifier.StressOptionType.NoCalculation;
    private LinearDensityUnit DensityUnit = DefaultUnits.LinearDensityUnit;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;
    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Modify type", "Density unit", "Stress calc."
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Types
      this.DropDownItems.Add(_optionTypes);
      this.SelectedItems.Add(_optionTypes[0]);

      // Density
      this.DropDownItems.Add(FilteredUnits.FilteredLinearDensityUnits);
      this.SelectedItems.Add(this.DensityUnit.ToString());

      // Stress option
      this.DropDownItems.Add(this._stressOptions);
      this.SelectedItems.Add(this._stressOptions[0]);

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0)
      {
        if (j == 0)
        {
          if (this._toMode == true)
          {
            this.DropDownItems.RemoveAt(1);
            this.SelectedItems.RemoveAt(1);
            this.SpacerDescriptions.RemoveAt(1);
          }
          this._toMode = false;
        }
        else
        {
          if (_toMode == false)
          {
            this.DropDownItems.Insert(1, FilteredUnits.FilteredLengthUnits);
            this.SelectedItems.Insert(1, this.LengthUnit.ToString());
            this.SpacerDescriptions.Insert(1, "Length unit");
          }
          this._toMode = true;
        }
      }

      if (i == 1)
      {
        if (this._toMode == false)
          this.DensityUnit = (LinearDensityUnit)Enum.Parse(typeof(LinearDensityUnit), this.SelectedItems[i]);
        else
          this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);
      }

      if (i == 2)
      {
        if (this._toMode == false)
        {
          if (j == 0)
            this.StressOption = GsaSectionModifier.StressOptionType.NoCalculation;
          if (j == 1)
            this.StressOption = GsaSectionModifier.StressOptionType.UseUnmodified;
          if (j == 2)
            this.StressOption = GsaSectionModifier.StressOptionType.UseModified;
        }
        else
          this.DensityUnit = (LinearDensityUnit)Enum.Parse(typeof(LinearDensityUnit), this.SelectedItems[i]);
      }

      if (i == 3)
      {
        if (j == 0)
          this.StressOption = GsaSectionModifier.StressOptionType.NoCalculation;
        if (j == 1)
          this.StressOption = GsaSectionModifier.StressOptionType.UseUnmodified;
        if (j == 2)
          this.StressOption = GsaSectionModifier.StressOptionType.UseModified;
      }
      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      if (this._toMode)
      {
        this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[1]);
        this.DensityUnit = (LinearDensityUnit)Enum.Parse(typeof(LinearDensityUnit), this.SelectedItems[2]);
        if (this.SelectedItems[3] == this._stressOptions[0])
          this.StressOption = GsaSectionModifier.StressOptionType.NoCalculation;
        if (this.SelectedItems[3] == this._stressOptions[1])
          this.StressOption = GsaSectionModifier.StressOptionType.UseUnmodified;
        if (this.SelectedItems[3] == this._stressOptions[2])
          this.StressOption = GsaSectionModifier.StressOptionType.UseModified;
      }
      else
      {
        this.DensityUnit = (LinearDensityUnit)Enum.Parse(typeof(LinearDensityUnit), this.SelectedItems[1]);
        if (this.SelectedItems[2] == _stressOptions[0])
          this.StressOption = GsaSectionModifier.StressOptionType.NoCalculation;
        if (this.SelectedItems[2] == _stressOptions[1])
          this.StressOption = GsaSectionModifier.StressOptionType.UseUnmodified;
        if (this.SelectedItems[2] == _stressOptions[2])
          this.StressOption = GsaSectionModifier.StressOptionType.UseModified;
      }

      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      Params.Input[7].Name = "Additional Mass [" + LinearDensity.GetAbbreviation(this.DensityUnit) + "]";
      if (_toMode)
      {
        string unit = Length.GetAbbreviation(this.LengthUnit);
        string volUnit = VolumePerLength.GetAbbreviation(UnitsHelper.GetVolumePerLengthUnit(LengthUnit));

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

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetBoolean("toMode", _toMode);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      _toMode = reader.GetBoolean("toMode");
      return base.Read(reader);
    }
    #endregion
  }
}
