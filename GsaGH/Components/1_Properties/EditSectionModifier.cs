using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to edit a Material and ouput the information
  /// </summary>
  public class EditSectionModifier : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("7c78c61b-f01c-4a0e-9399-712fc853e23b");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.EditSectionModifier;

    public EditSectionModifier() : base("Edit Section Modifier",
      "ModifierEdit",
      "Modify GSA Section Modifier",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaSectionModifierParameter(), GsaSectionModifierGoo.Name, GsaSectionModifierGoo.NickName, GsaSectionModifierGoo.Description + " to get or set information for. Leave blank to create a new " + GsaSectionModifierGoo.Name, GH_ParamAccess.item);

      pManager.AddGenericParameter("Area Modifier", "A", "Modify the effective Area using either:" + System.Environment.NewLine + "BY using a Percentage UnitNumber (tweaking the existing value BY this percentage)" + System.Environment.NewLine + "TO using an Area UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("I11 Modifier", "I11", "Modify the effective Iyy/Iuu using either:" + System.Environment.NewLine + "BY using a Percentage UnitNumber (tweaking the existing value BY this percentage)" + System.Environment.NewLine + "TO using an AreaMomentOfInertia UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("I22 Modifier", "I22", "Modify the effective Izz/Ivv using either:" + System.Environment.NewLine + "BY using a Percentage UnitNumber (tweaking the existing value BY this percentage)" + System.Environment.NewLine + "TO using an AreaMomentOfInertia UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("J Modifier", "J", "Modify the effective J using either:" + System.Environment.NewLine + "BY using a Percentage UnitNumber (tweaking the existing value BY this percentage)" + System.Environment.NewLine + "TO using an AreaMomentOfInertia UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("K11 Modifier", "K11", "Modify the effective Kyy/Kuu using either:" + System.Environment.NewLine + "BY using a Percentage UnitNumber (tweaking the existing value BY this percentage)" + System.Environment.NewLine + "TO using a DecimalFraction UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("K22 Modifier", "K22", "Modify the effective Kzz/Kvv using either:" + System.Environment.NewLine + "BY using a Percentage UnitNumber (tweaking the existing value BY this percentage)" + System.Environment.NewLine + "TO using a DecimalFraction UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("Volume Modifier", "V", "Modify the effective Volume/Length using either:" + System.Environment.NewLine + "BY using a Percentage UnitNumber (tweaking the existing value BY this percentage)" + System.Environment.NewLine + "TO using a VolumePerLength UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("Additional Mass", "+kg", "Additional mass per unit length using a LinearDensity UnitNumber", GH_ParamAccess.item);

      pManager.AddBooleanParameter("Principal Bending Axis", "Ax", "[Optional] Set to 'true' to use Principal (u,v) Axis for Bending. If false (and by default), Local (y,z) Axis will be used", GH_ParamAccess.item);

      pManager.AddBooleanParameter("Reference Point Centroid", "Ref", "[Optional] Set to 'true' to use the Centroid as Analysis Reference Point. If false (and by default), the specified point will be used", GH_ParamAccess.item);

      pManager.AddIntegerParameter("Stress Option Type", "Str", "Set the Stress Option Type. Accepted inputs are:"
        + System.Environment.NewLine + "0: No calculation"
        + System.Environment.NewLine + "1: Use modified section properties"
        + System.Environment.NewLine + "2: Use unmodified section properties",
        GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaSectionModifierParameter(), GsaSectionModifierGoo.Name, GsaSectionModifierGoo.NickName, GsaSectionModifierGoo.Description + " with applied changes.", GH_ParamAccess.item);

      pManager.AddGenericParameter("Area Modifier", "A", "Modified effective Area in either:" + System.Environment.NewLine + "BY as a Percentage UnitNumber" + System.Environment.NewLine + "TO as an Area UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("I11 Modifier", "I11", "Modify the effective Iyy/Iuu in either:" + System.Environment.NewLine + "BY as a Percentage UnitNumber" + System.Environment.NewLine + "TO as an AreaMomentOfInertia UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("I22 Modifier", "I22", "Modify the effective Izz/Ivv in either:" + System.Environment.NewLine + "BY as a Percentage UnitNumber" + System.Environment.NewLine + "TO as an AreaMomentOfInertia UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("J Modifier", "J", "Modify the effective J in either:" + System.Environment.NewLine + "BY as a Percentage UnitNumber" + System.Environment.NewLine + "TO as an AreaMomentOfInertia UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("K11 Modifier", "K11", "Modify the effective Kyy/Kuu in either:" + System.Environment.NewLine + "BY as a Percentage UnitNumber" + System.Environment.NewLine + "TO as a DecimalFraction UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("K22 Modifier", "K22", "Modify the effective Kzz/Kvv in either:" + System.Environment.NewLine + "BY as a Percentage UnitNumber" + System.Environment.NewLine + "TO as a DecimalFraction UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("Volume Modifier", "V", "Modify the effective Volume/Length in either:" + System.Environment.NewLine + "BY as a Percentage UnitNumber" + System.Environment.NewLine + "TO as a VolumePerLength UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("Additional Mass", "+kg", "Additional mass per unit length", GH_ParamAccess.item);

      pManager.AddBooleanParameter("Principal Bending Axis", "Ax", "If 'true' GSA will use Principal (u,v) Axis for Bending. If false, Local (y,z) Axis will be used", GH_ParamAccess.item);

      pManager.AddBooleanParameter("Reference Point Centroid", "Ref", "If 'true' GSA will use the Centroid as Analysis Reference Point. If false, the specified point will be used", GH_ParamAccess.item);

      pManager.AddGenericParameter("Stress Option Type", "Str", "Get the Stress Option Type:"
        + System.Environment.NewLine + "0: No Calculation"
        + System.Environment.NewLine + "1: Use Modified section properties"
        + System.Environment.NewLine + "2: Use Unmodified section properties",
        GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaSectionModifier modifier = new GsaSectionModifier();
      GsaSectionModifier gsaModifier = new GsaSectionModifier();
      if (DA.GetData(0, ref gsaModifier))
      {
        modifier = gsaModifier.Duplicate();
      }

      if (modifier != null)
      {
        if (this.Params.Input[1].SourceCount > 0)
        {
          GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
          DA.GetData(1, ref gh_typ);
          if (GH_Convert.ToString(gh_typ.Value, out string txt, GH_Conversion.Both))
          {
            if (Area.TryParse(txt, out Area res))
              modifier.AreaModifier = res;
            else
            {
              try
              {
                modifier.AreaModifier = CustomInput.UnitNumberOrDoubleAsRatioToPercentage(this, DA, 1, true).Value;
              }
              catch (Exception e)
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                return;
              }
            }
          }
        }

        if (this.Params.Input[2].SourceCount > 0)
        {
          GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
          DA.GetData(2, ref gh_typ);
          if (GH_Convert.ToString(gh_typ.Value, out string txt, GH_Conversion.Both))
          {
            if (AreaMomentOfInertia.TryParse(txt, out AreaMomentOfInertia res))
              modifier.I11Modifier = res;
            else
            {
              try
              {
                modifier.I11Modifier = CustomInput.UnitNumberOrDoubleAsRatioToPercentage(this, DA, 2, true).Value;
              }
              catch (Exception e)
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                return;
              }
            }
          }
        }

        if (this.Params.Input[3].SourceCount > 0)
        {
          GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
          DA.GetData(3, ref gh_typ);
          if (GH_Convert.ToString(gh_typ.Value, out string txt, GH_Conversion.Both))
          {
            if (AreaMomentOfInertia.TryParse(txt, out AreaMomentOfInertia res))
              modifier.I22Modifier = res;
            else
            {
              try
              {
                modifier.I22Modifier = CustomInput.UnitNumberOrDoubleAsRatioToPercentage(this, DA, 3, true).Value;
              }
              catch (Exception e)
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                return;
              }
            }
          }
        }

        if (this.Params.Input[4].SourceCount > 0)
        {
          GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
          DA.GetData(4, ref gh_typ);
          if (GH_Convert.ToString(gh_typ.Value, out string txt, GH_Conversion.Both))
          {
            if (AreaMomentOfInertia.TryParse(txt, out AreaMomentOfInertia res))
              modifier.JModifier = res;
            else
            {
              try
              {
                modifier.JModifier = CustomInput.UnitNumberOrDoubleAsRatioToPercentage(this, DA, 4, true).Value;
              }
              catch (Exception e)
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                return;
              }
            }
          }
        }

        if (this.Params.Input[5].SourceCount > 0)
          modifier.K11Modifier = CustomInput.RatioInDecimalFractionToPercentage(this, DA, 5);

        if (this.Params.Input[6].SourceCount > 0)
          modifier.K22Modifier = CustomInput.RatioInDecimalFractionToPercentage(this, DA, 6);

        if (this.Params.Input[7].SourceCount > 0)
        {
          GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
          DA.GetData(7, ref gh_typ);
          if (GH_Convert.ToString(gh_typ.Value, out string txt, GH_Conversion.Both))
          {
            if (VolumePerLength.TryParse(txt, out VolumePerLength res))
              modifier.VolumeModifier = res;
            else
            {
              try
              {
                modifier.VolumeModifier = CustomInput.UnitNumberOrDoubleAsRatioToPercentage(this, DA, 7, true).Value;
              }
              catch (Exception e)
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                return;
              }
            }
          }
        }

        if (this.Params.Input[8].SourceCount > 0)
        {
          GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
          if (DA.GetData(8, ref gh_typ))
          {
            // try cast directly to quantity type
            if (gh_typ.Value is GH_UnitNumber)
            {
              GH_UnitNumber unitNumber = (GH_UnitNumber)gh_typ.Value;
              // check that unit is of right type
              if (!unitNumber.Value.QuantityInfo.UnitType.Equals(typeof(LinearDensityUnit)))
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + this.Params.Input[8].NickName + " input: Wrong unit type"
                    + System.Environment.NewLine + "Unit type is " + unitNumber.Value.QuantityInfo.Name + " but must be LinearDensity");
                return;
              }
              else
                modifier.AdditionalMass = (LinearDensity)unitNumber.Value;
            }
            // try cast to string
            else if (GH_Convert.ToString(gh_typ.Value, out string txt, GH_Conversion.Both))
            {
              if (LinearDensity.TryParse(txt, out LinearDensity res))
                modifier.AdditionalMass = res;
              else
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + this.Params.Input[8].NickName + " to LinearDensity");
            }
            // try cast to double
            else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
            {
              modifier.AdditionalMass = new LinearDensity(val, this.LinearDensityUnit);
            }
            else
            {
              AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + this.Params.Input[8].NickName + " to UnitNumber");
              return;
            }
          }
        }

        bool ax = false;
        if (DA.GetData(9, ref ax))
          modifier.IsBendingAxesPrincipal = ax;

        bool pt = false;
        if (DA.GetData(10, ref pt))
          modifier.IsReferencePointCentroid = pt;

        GH_ObjectWrapper obj = new GH_ObjectWrapper();
        if (DA.GetData(11, ref obj))
        {
          if (GH_Convert.ToInt32(obj, out int stress, GH_Conversion.Both))
          {
            if (stress == 0)
              modifier.StressOption = GsaSectionModifier.StressOptionType.NoCalculation;
            else if (stress == 1)
              modifier.StressOption = GsaSectionModifier.StressOptionType.UseModified;
            else if (stress == 2)
              modifier.StressOption = GsaSectionModifier.StressOptionType.UseUnmodified;
            else
            {
              AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + this.Params.Input[11].NickName + " input: Must be either 0, 1 or 2 but is " + stress);
              return;
            }
          }
          else if (GH_Convert.ToString(obj, out string stressString, GH_Conversion.Both))
          {
            if (stressString.ToLower().Contains("no"))
              modifier.StressOption = GsaSectionModifier.StressOptionType.NoCalculation;
            else if (stressString.ToLower().Replace(" ", string.Empty).Contains("unmod"))
              modifier.StressOption = GsaSectionModifier.StressOptionType.UseUnmodified;
            else if (stressString.ToLower().Replace(" ", string.Empty).Contains("mod"))
              modifier.StressOption = GsaSectionModifier.StressOptionType.UseModified;
            else
            {
              AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + this.Params.Input[11].NickName + " input: Must contain the one of the following phrases 'no', 'unmod' or 'mod' (case insensitive), but input is '" + stress + "'");
              return;
            }
          }
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + this.Params.Input[11].NickName + " input: Must be either 0, 1 or 2 or contain the one of the following phrases 'no', 'unmod' or mod' (case insensitive), but input is " + stress + "'");
            return;
          }
        }

        DA.SetData(0, new GsaSectionModifierGoo(modifier));
        if (modifier._sectionModifier.AreaModifier.Option == SectionModifierOptionType.BY)
          DA.SetData(1, new GH_UnitNumber(modifier.AreaModifier));
        else
          DA.SetData(1, new GH_UnitNumber(modifier.AreaModifier.ToUnit(UnitsHelper.GetAreaUnit(this.LengthUnit))));

        if (modifier._sectionModifier.I11Modifier.Option == SectionModifierOptionType.BY)
          DA.SetData(2, new GH_UnitNumber(modifier.I11Modifier));
        else
          DA.SetData(2, new GH_UnitNumber(modifier.I11Modifier.ToUnit(UnitsHelper.GetAreaMomentOfInertiaUnit(this.LengthUnit))));

        if (modifier._sectionModifier.I22Modifier.Option == SectionModifierOptionType.BY)
          DA.SetData(3, new GH_UnitNumber(modifier.I22Modifier));
        else
          DA.SetData(3, new GH_UnitNumber(modifier.I22Modifier.ToUnit(UnitsHelper.GetAreaMomentOfInertiaUnit(this.LengthUnit))));

        if (modifier._sectionModifier.JModifier.Option == SectionModifierOptionType.BY)
          DA.SetData(4, new GH_UnitNumber(modifier.JModifier));
        else
          DA.SetData(4, new GH_UnitNumber(modifier.JModifier.ToUnit(UnitsHelper.GetAreaMomentOfInertiaUnit(this.LengthUnit))));

        DA.SetData(5, new GH_UnitNumber(modifier.K11Modifier));
        DA.SetData(6, new GH_UnitNumber(modifier.K22Modifier));

        if (modifier._sectionModifier.VolumeModifier.Option == SectionModifierOptionType.BY)
          DA.SetData(7, new GH_UnitNumber(modifier.VolumeModifier));
        else
          DA.SetData(7, new GH_UnitNumber(modifier.VolumeModifier.ToUnit(UnitsHelper.GetVolumePerLengthUnit(this.LengthUnit))));

        DA.SetData(8, new GH_UnitNumber(modifier.AdditionalMass.ToUnit(this.LinearDensityUnit)));

        DA.SetData(9, modifier.IsBendingAxesPrincipal);
        DA.SetData(10, modifier.IsReferencePointCentroid);
        DA.SetData(11, (int)modifier.StressOption);
      }
    }

    #region Custom UI
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;
    private LinearDensityUnit LinearDensityUnit = DefaultUnits.LinearDensityUnit;
    protected override void BeforeSolveInstance()
    {
      UpdateMessage();
    }
    public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
    {
      Menu_AppendSeparator(menu);

      ToolStripMenuItem lengthUnitsMenu = new ToolStripMenuItem("Length");
      lengthUnitsMenu.Enabled = true;
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length))
      {
        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { UpdateLength(unit); });
        toolStripMenuItem.Checked = unit == Length.GetAbbreviation(this.LengthUnit);
        toolStripMenuItem.Enabled = true;
        lengthUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      ToolStripMenuItem densityUnitsMenu = new ToolStripMenuItem("Density");
      densityUnitsMenu.Enabled = true;
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.LinearDensity))
      {
        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { UpdateDensity(unit); });
        toolStripMenuItem.Checked = unit == LinearDensity.GetAbbreviation(this.LinearDensityUnit);
        toolStripMenuItem.Enabled = true;
        densityUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      ToolStripMenuItem unitsMenu = new ToolStripMenuItem("Select Units", Properties.Resources.Units);
      unitsMenu.DropDownItems.AddRange(new ToolStripItem[] { lengthUnitsMenu, densityUnitsMenu });
      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;

      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }

    private void UpdateLength(string unit)
    {
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      Update();
    }
    private void UpdateDensity(string unit)
    {
      this.LinearDensityUnit = (LinearDensityUnit)UnitsHelper.Parse(typeof(LinearDensityUnit), unit);
      Update();
    }
    private void Update()
    {
      UpdateMessage();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }
    private void UpdateMessage()
    {
      this.Message =
        Length.GetAbbreviation(this.LengthUnit) + ", " +
        LinearDensity.GetAbbreviation(this.LinearDensityUnit);
    }

    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetString("LengthUnit", this.LengthUnit.ToString());
      writer.SetString("DensityUnit", this.LinearDensityUnit.ToString());
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      if (reader.ItemExists("LengthUnit"))
      {
        this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("LengthUnit"));
        this.LinearDensityUnit = (LinearDensityUnit)UnitsHelper.Parse(typeof(LinearDensityUnit), reader.GetString("DensityUnit"));
      }
      else
      {
        this.LengthUnit = OasysGH.Units.DefaultUnits.LengthUnitSection;
        this.LinearDensityUnit = DefaultUnits.LinearDensityUnit;
        List<IGH_Param> inputs = this.Params.Input.ToList();
        List<IGH_Param> outputs = this.Params.Output.ToList();
        bool flag = base.Read(reader);
        foreach (IGH_Param param in inputs)
          this.Params.RegisterInputParam(param);
        foreach (IGH_Param param in outputs)
          this.Params.RegisterOutputParam(param);
        return flag;
      }
      return base.Read(reader);
    }

    #region IGH_VariableParameterComponent null implementation
    public virtual void VariableParameterMaintenance()
    {
      string unit = Length.GetAbbreviation(this.LengthUnit);
      string volUnit = VolumePerLength.GetAbbreviation(UnitsHelper.GetVolumePerLengthUnit(this.LengthUnit));
      Params.Output[1].Name = "Area Modifier [" + unit + "\u00B2]";
      Params.Output[2].Name = "I11 Modifier [" + unit + "\u2074]";
      Params.Output[3].Name = "I22 Modifier [" + unit + "\u2074]";
      Params.Output[7].Name = "Volume Modifier [" + volUnit + "]";
      string unitAbbreviation = LinearDensity.GetAbbreviation(this.LinearDensityUnit);
      Params.Output[8].Name = "Additional Mass [" + unitAbbreviation + "]";
    }

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index) => false;

    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index) => false;

    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index) => null;

    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) => false;
    #endregion
    #endregion
  }
}
