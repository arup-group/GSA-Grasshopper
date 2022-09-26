using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH.Parameters;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components
{
  class GetInput
  {
    internal static GH_UnitNumber UnitNumberOrDoubleAsRatioToPercentage(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false)
    {
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          return (GH_UnitNumber)gh_typ.Value;
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          Ratio rat = new Ratio(val, RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Note: Input " + owner.Params.Input[inputid].NickName + " was automatically converted from DecimalFraction (" + val + ") to Percentage (" + rat.ToString("f0") + ")");
          return new GH_UnitNumber(rat);
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber");
          return null;
        }
      }
      else if (!isOptional)
      {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return null;
    }
    internal static Ratio RatioInDecimalFractionToPercentage(GH_Component owner, IGH_DataAccess DA, int inputid)
    {
      GH_UnitNumber unitNumber = null;
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          unitNumber = (GH_UnitNumber)gh_typ.Value;
          // check that unit is of right type
          if (!unitNumber.Value.QuantityInfo.UnitType.Equals(typeof(RatioUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + System.Environment.NewLine + "Unit type is " + unitNumber.Value.QuantityInfo.Name + " but must be Ratio");
            return new Ratio(100, RatioUnit.Percent);
          }
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          Ratio rat = new Ratio(val, RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Note: Input " + owner.Params.Input[inputid].NickName + " was automatically converted from DecimalFraction (" + val + ") to Percentage (" + rat.ToString("f0") + ")");
          return rat;
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber");
          return new Ratio(100, RatioUnit.Percent);
        }
      }
      return new Ratio(100, RatioUnit.Percent);
    }
    internal static Ratio RatioInDecimalFractionToDecimalFraction(GH_Component owner, IGH_DataAccess DA, int inputid)
    {
      GH_UnitNumber unitNumber = null;
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          unitNumber = (GH_UnitNumber)gh_typ.Value;
          // check that unit is of right type
          if (!unitNumber.Value.QuantityInfo.UnitType.Equals(typeof(RatioUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + System.Environment.NewLine + "Unit type is " + unitNumber.Value.QuantityInfo.Name + " but must be Ratio");
            return new Ratio(1, RatioUnit.DecimalFraction);
          }
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Note: Input " + owner.Params.Input[inputid].NickName + " was not automatically converted to percentage");
          return new Ratio(val, RatioUnit.DecimalFraction);
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber");
          return new Ratio(1, RatioUnit.DecimalFraction);
        }
      }
      return new Ratio(1, RatioUnit.DecimalFraction);
    }
    internal static Length GetLength(GH_Component owner, IGH_DataAccess DA, int inputid, LengthUnit lengthUnit, bool isOptional = false)
    {
      GH_UnitNumber unitNumber = null;
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          unitNumber = (GH_UnitNumber)gh_typ.Value;
          // check that unit is of right type
          if (!unitNumber.Value.QuantityInfo.UnitType.Equals(typeof(LengthUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + System.Environment.NewLine + "Unit type is " + unitNumber.Value.QuantityInfo.Name + " but must be Length");
            return Length.Zero;
          }
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          unitNumber = new GH_UnitNumber(new Length(val, lengthUnit));
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber");
          return Length.Zero;
        }
      }
      else if (!isOptional)
      {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
        return Length.Zero;
      }
      else
      {
        if (unitNumber == null)
          return Length.Zero;
      }

      return (Length)unitNumber.Value;
    }
    internal static Density GetDensity(GH_Component owner, IGH_DataAccess DA, int inputid, DensityUnit densityUnit, bool isOptional = false)
    {
      GH_UnitNumber unitNumber = null;
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          unitNumber = (GH_UnitNumber)gh_typ.Value;
          // check that unit is of right type
          if (!unitNumber.Value.QuantityInfo.UnitType.Equals(typeof(DensityUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + System.Environment.NewLine + "Unit type is " + unitNumber.Value.QuantityInfo.Name + " but must be Density");
            return Density.Zero;
          }
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          return new Density(val, densityUnit);
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber");
          return Density.Zero;
        }
      }
      else if (!isOptional)
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      else
      {
        if (unitNumber == null)
          return Density.Zero;
      }

      return (Density)unitNumber.Value;
    }

    internal static LinearDensity GetLinearDensity(GH_Component owner, IGH_DataAccess DA, int inputid, LinearDensityUnit densityUnit, bool isOptional = false)
    {
      GH_UnitNumber unitNumber = null;
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          unitNumber = (GH_UnitNumber)gh_typ.Value;
          // check that unit is of right type
          if (!unitNumber.Value.QuantityInfo.UnitType.Equals(typeof(LinearDensityUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + System.Environment.NewLine + "Unit type is " + unitNumber.Value.QuantityInfo.Name + " but must be LinearDensity");
            return LinearDensity.Zero;
          }
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          unitNumber = new GH_UnitNumber(new LinearDensity(val, densityUnit));
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber");
          return LinearDensity.Zero;
        }
      }
      else if (!isOptional)
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      else
      {
        if (unitNumber == null)
          return LinearDensity.Zero;
      }

      return (LinearDensity)unitNumber.Value;
    }
    internal static VolumePerLength GetVolumePerLength(GH_Component owner, IGH_DataAccess DA, int inputid, VolumePerLengthUnit volumePerLengthUnit, bool isOptional = false)
    {
      GH_UnitNumber unitNumber = null;
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          unitNumber = (GH_UnitNumber)gh_typ.Value;
          // check that unit is of right type
          if (!unitNumber.Value.QuantityInfo.UnitType.Equals(typeof(VolumePerLengthUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + System.Environment.NewLine + "Unit type is " + unitNumber.Value.QuantityInfo.Name + " but must be VolumePerLength");
            return VolumePerLength.Zero;
          }
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          unitNumber = new GH_UnitNumber(new VolumePerLength(val, volumePerLengthUnit));
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber");
          return VolumePerLength.Zero;
        }
      }
      else if (!isOptional)
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      else
      {
        if (unitNumber == null)
          return VolumePerLength.Zero;
      }

      return (VolumePerLength)unitNumber.Value;
    }
    internal static Area GetArea(GH_Component owner, IGH_DataAccess DA, int inputid, AreaUnit areaUnit, bool isOptional = false)
    {
      GH_UnitNumber unitNumber = null;
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          unitNumber = (GH_UnitNumber)gh_typ.Value;
          // check that unit is of right type
          if (!unitNumber.Value.QuantityInfo.UnitType.Equals(typeof(AreaUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + System.Environment.NewLine + "Unit type is " + unitNumber.Value.QuantityInfo.Name + " but must be Area");
            return Area.Zero;
          }
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          unitNumber = new GH_UnitNumber(new Area(val, areaUnit));
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber");
          return Area.Zero;
        }
      }
      else if (!isOptional)
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      else
      {
        if (unitNumber == null)
          return Area.Zero;
      }

      return (Area)unitNumber.Value;
    }
    internal static AreaMomentOfInertia GetAreaMomentOfInertia(GH_Component owner, IGH_DataAccess DA, int inputid, AreaMomentOfInertiaUnit areaUnit, bool isOptional = false)
    {
      GH_UnitNumber unitNumber = null;
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          unitNumber = (GH_UnitNumber)gh_typ.Value;
          // check that unit is of right type
          if (!unitNumber.Value.QuantityInfo.UnitType.Equals(typeof(AreaMomentOfInertiaUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + System.Environment.NewLine + "Unit type is " + unitNumber.Value.QuantityInfo.Name + " but must be AreaMomentOfInertia");
            return AreaMomentOfInertia.Zero;
          }
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          unitNumber = new GH_UnitNumber(new AreaMomentOfInertia(val, areaUnit));
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber");
          return AreaMomentOfInertia.Zero;
        }
      }
      else if (!isOptional)
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      else
      {
        if (unitNumber == null)
          return AreaMomentOfInertia.Zero;
      }

      return (AreaMomentOfInertia)unitNumber.Value;
    }
    internal static CoefficientOfThermalExpansion GetCoefficientOfThermalExpansion(GH_Component owner, IGH_DataAccess DA, int inputid, CoefficientOfThermalExpansionUnit coefficientOfThermalExpansionUnit, bool isOptional = false)
    {
      GH_UnitNumber unitNumber = null;
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          unitNumber = (GH_UnitNumber)gh_typ.Value;
          // check that unit is of right type
          if (!unitNumber.Value.QuantityInfo.UnitType.Equals(typeof(CoefficientOfThermalExpansionUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + System.Environment.NewLine + "Unit type is " + unitNumber.Value.QuantityInfo.Name + " but must be Coefficient of thermal expansion");
            return CoefficientOfThermalExpansion.Zero;
          }
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          unitNumber = new GH_UnitNumber(new CoefficientOfThermalExpansion(val, coefficientOfThermalExpansionUnit));
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber");
          return CoefficientOfThermalExpansion.Zero;
        }
      }
      else if (!isOptional)
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      else
      {
        if (unitNumber == null)
          return CoefficientOfThermalExpansion.Zero;
      }

      return (CoefficientOfThermalExpansion)unitNumber.Value;
    }
    internal static Pressure Stress(GH_Component owner, IGH_DataAccess DA, int inputid, PressureUnit stressUnit, bool isOptional = false)
    {
      Pressure stressFib = new Pressure();

      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        GH_UnitNumber inStress;

        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          inStress = (GH_UnitNumber)gh_typ.Value;
          if (!inStress.Value.QuantityInfo.UnitType.Equals(typeof(PressureUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + System.Environment.NewLine + "Unit type is " + inStress.Value.QuantityInfo.Name + " but must be Stress (Pressure)");
            return Pressure.Zero;
          }
          stressFib = (Pressure)inStress.Value.ToUnit(stressUnit);
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          inStress = new GH_UnitNumber(new Pressure(val, stressUnit));
          stressFib = (Pressure)inStress.Value;
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber of Stress");
          return Pressure.Zero;
        }
        return stressFib;
      }
      else if (!isOptional)
      {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return Pressure.Zero;
    }
    internal static Strain GetStrain(GH_Component owner, IGH_DataAccess DA, int inputid, StrainUnit strainUnit, bool isOptional = false)
    {
      Strain strainFib = new Strain();

      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        GH_UnitNumber inStrain;

        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          inStrain = (GH_UnitNumber)gh_typ.Value;
          if (!inStrain.Value.QuantityInfo.UnitType.Equals(typeof(StrainUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + System.Environment.NewLine + "Unit type is " + inStrain.Value.QuantityInfo.Name + " but must be Strain");
            return Strain.Zero;
          }
          strainFib = (Strain)inStrain.Value.ToUnit(strainUnit);
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          inStrain = new GH_UnitNumber(new Strain(val, strainUnit));
          strainFib = (Strain)inStrain.Value;
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber of Strain");
          return Strain.Zero;
        }
        return strainFib;
      }
      else if (!isOptional)
      {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return Strain.Zero;
    }
    internal static Curvature GetCurvature(GH_Component owner, IGH_DataAccess DA, int inputid, CurvatureUnit curvatureUnit, bool isOptional = false)
    {
      Curvature crvature = new Curvature();

      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        GH_UnitNumber inStrain;

        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          inStrain = (GH_UnitNumber)gh_typ.Value;
          if (!inStrain.Value.QuantityInfo.UnitType.Equals(typeof(CurvatureUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + System.Environment.NewLine + "Unit type is " + inStrain.Value.QuantityInfo.Name + " but must be Curvature");
            return Curvature.Zero;
          }
          crvature = (Curvature)inStrain.Value.ToUnit(curvatureUnit);
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          inStrain = new GH_UnitNumber(new Curvature(val, curvatureUnit));
          crvature = (Curvature)inStrain.Value;
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber of Curvature");
          return Curvature.Zero;
        }
        return crvature;
      }
      else if (!isOptional)
      {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return Curvature.Zero;
    }
    internal static Force GetForce(GH_Component owner, IGH_DataAccess DA, int inputid, ForceUnit forceUnit, bool isOptional = false)
    {
      Force force = new Force();

      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        GH_UnitNumber inForce;

        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          inForce = (GH_UnitNumber)gh_typ.Value;
          if (!inForce.Value.QuantityInfo.UnitType.Equals(typeof(ForceUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + System.Environment.NewLine + "Unit type is " + inForce.Value.QuantityInfo.Name + " but must be Force");
            return Force.Zero;
          }
          force = (Force)inForce.Value.ToUnit(forceUnit);
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          inForce = new GH_UnitNumber(new Force(val, forceUnit));
          force = (Force)inForce.Value;
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber of Force");
          return Force.Zero;
        }
        return force;
      }
      else if (!isOptional)
      {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return Force.Zero;
    }
    internal static ForcePerLength GetForcePerLength(GH_Component owner, IGH_DataAccess DA, int inputid, ForcePerLengthUnit forceUnit, bool isOptional = false)
    {
      ForcePerLength force = new ForcePerLength();

      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        GH_UnitNumber inForce;

        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          inForce = (GH_UnitNumber)gh_typ.Value;
          if (!inForce.Value.QuantityInfo.UnitType.Equals(typeof(ForceUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + System.Environment.NewLine + "Unit type is " + inForce.Value.QuantityInfo.Name + " but must be ForcePerLength");
            return ForcePerLength.Zero;
          }
          force = (ForcePerLength)inForce.Value.ToUnit(forceUnit);
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          inForce = new GH_UnitNumber(new ForcePerLength(val, forceUnit));
          force = (ForcePerLength)inForce.Value;
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber of ForcePerLength");
          return ForcePerLength.Zero;
        }
        return force;
      }
      else if (!isOptional)
      {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return ForcePerLength.Zero;
    }
    internal static Moment GetMoment(GH_Component owner, IGH_DataAccess DA, int inputid, MomentUnit momentUnit, bool isOptional = false)
    {
      Moment moment = new Moment();

      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        GH_UnitNumber inMoment;

        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          inMoment = (GH_UnitNumber)gh_typ.Value;
          if (!inMoment.Value.QuantityInfo.UnitType.Equals(typeof(MomentUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + System.Environment.NewLine + "Unit type is " + inMoment.Value.QuantityInfo.Name + " but must be Moment");
            return Moment.Zero;
          }
          moment = (Moment)inMoment.Value.ToUnit(momentUnit);
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          inMoment = new GH_UnitNumber(new Moment(val, momentUnit));
          moment = (Moment)inMoment.Value;
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber of Moment");
          return Moment.Zero;
        }
        return moment;
      }
      else if (!isOptional)
      {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return Moment.Zero;
    }

    internal static Angle GetAngle(GH_Component owner, IGH_DataAccess DA, int inputid, AngleUnit angleUnit, bool isOptional = false)
    {
      GH_UnitNumber a1 = new GH_UnitNumber(new Angle(0, angleUnit));
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        // try cast directly to quantity type
        if (gh_typ.Value is GH_UnitNumber)
        {
          a1 = (GH_UnitNumber)gh_typ.Value;
          // check that unit is of right type
          if (!a1.Value.QuantityInfo.UnitType.Equals(typeof(AngleUnit)))
          {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in " + owner.Params.Input[inputid].NickName + " input: Wrong unit type"
                + System.Environment.NewLine + "Unit type is " + a1.Value.QuantityInfo.Name + " but must be Angle");
            return Angle.Zero;
          }
        }
        // try cast to double
        else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
        {
          // create new quantity from default units
          a1 = new GH_UnitNumber(new Angle(val, angleUnit));
        }
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to Angle");
          return Angle.Zero;
        }
        return (Angle)a1.Value;
      }
      else if (!isOptional)
      {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return Angle.Zero;
    }
  }
}
