using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to edit a Material and ouput the information
  /// </summary>
  public class EditSectionModifier : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("7c78c61b-f01c-4a0e-9399-712fc853e23b");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
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
      pManager.AddGenericParameter("Section Modifier", "Mo", "Set GSA Section Modifier", GH_ParamAccess.item);

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

      for (int i = 1; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Section Modifier", "Mo", "Set GSA Section Modifier", GH_ParamAccess.item);

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

      pManager.AddIntegerParameter("Stress Option Type", "Str", "Get the Stress Option Type:"
        + System.Environment.NewLine + "0: No calculation"
        + System.Environment.NewLine + "1: Use modified section properties"
        + System.Environment.NewLine + "2: Use unmodified section properties",
        GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaSectionModifier modifier = new GsaSectionModifier();
      if (DA.GetData(0, ref modifier))
      {
        if (this.Params.Input[1].SourceCount > 0)
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

        if (this.Params.Input[2].SourceCount > 0)
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

        if (this.Params.Input[3].SourceCount > 0)
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

        if (this.Params.Input[4].SourceCount > 0)
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

        if (this.Params.Input[5].SourceCount > 0)
          modifier.K11Modifier = CustomInput.RatioInDecimalFractionToPercentage(this, DA, 5);

        if (this.Params.Input[6].SourceCount > 0)
          modifier.K22Modifier = CustomInput.RatioInDecimalFractionToPercentage(this, DA, 6);

        if (this.Params.Input[7].SourceCount > 0)
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
            // try cast to double
            else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
            {
              // create new quantity from default units
              AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Input " + this.Params.Input[8].NickName + " was automatically converted to " + DefaultUnits.LinearDensityUnit.ToString() + ". Be aware that sharing this file or changing your unit settings will change this value!");
              modifier.AdditionalMass = new LinearDensity(val, DefaultUnits.LinearDensityUnit);
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

        int stress = 0;
        if (DA.GetData(11, ref stress))
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

        DA.SetData(0, new GsaSectionModifierGoo(modifier));
        DA.SetData(1, new GH_UnitNumber(modifier.AreaModifier));
        DA.SetData(2, new GH_UnitNumber(modifier.I11Modifier));
        DA.SetData(3, new GH_UnitNumber(modifier.I22Modifier));
        DA.SetData(4, new GH_UnitNumber(modifier.JModifier));
        DA.SetData(5, new GH_UnitNumber(modifier.K11Modifier));
        DA.SetData(6, new GH_UnitNumber(modifier.K22Modifier));
        DA.SetData(7, new GH_UnitNumber(modifier.VolumeModifier));
        DA.SetData(8, new GH_UnitNumber(modifier.AdditionalMass));
        DA.SetData(9, modifier.IsBendingAxesPrincipal);
        DA.SetData(10, modifier.IsReferencePointCentroid);

        if (modifier.StressOption == GsaSectionModifier.StressOptionType.NoCalculation)
          stress = 0;
        else if (modifier.StressOption == GsaSectionModifier.StressOptionType.UseModified)
          stress = 1;
        else if (modifier.StressOption == GsaSectionModifier.StressOptionType.UseUnmodified)
          stress = 2;
        DA.SetData(11, stress);
      }
    }
  }
}

