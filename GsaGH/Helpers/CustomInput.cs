using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH.Parameters;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Helpers
{
  class CustomInput
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
  }
}
