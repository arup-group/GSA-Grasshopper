using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using UnitsNet.GH;
using UnitsNet;
using UnitsNet.Units;
using Oasys.Units;

namespace GsaGH.Components
{
    class GetInput
    {
        internal static Length Length(GH_Component owner, IGH_DataAccess DA, int inputid, LengthUnit lengthUnit, bool isOptional = false)
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
                        return UnitsNet.Length.Zero;
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
                    return UnitsNet.Length.Zero;
                }
            }
            else if (!isOptional)
            {
                owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
                return UnitsNet.Length.Zero;
            }
            else
            {
                if (unitNumber == null)
                    return UnitsNet.Length.Zero;
            }

            return (Length)unitNumber.Value;
        }
        internal static Density Density(GH_Component owner, IGH_DataAccess DA, int inputid, DensityUnit densityUnit, bool isOptional = false)
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
                        return UnitsNet.Density.Zero;
                    }
                }
                // try cast to double
                else if (GH_Convert.ToDouble(gh_typ.Value, out double val, GH_Conversion.Both))
                {
                    // create new quantity from default units
                    unitNumber = new GH_UnitNumber(new Density(val, densityUnit));
                }
                else
                {
                    owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " to UnitNumber");
                    return UnitsNet.Density.Zero;
                }
            }
            else if (!isOptional)
                owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
            else
            {
                if (unitNumber == null)
                    return UnitsNet.Density.Zero;
            }

            return (Density)unitNumber.Value;
        }
        internal static CoefficientOfThermalExpansion CoefficientOfThermalExpansion(GH_Component owner, IGH_DataAccess DA, int inputid, CoefficientOfThermalExpansionUnit coefficientOfThermalExpansionUnit, bool isOptional = false)
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
                        return UnitsNet.CoefficientOfThermalExpansion.Zero;
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
                    return UnitsNet.CoefficientOfThermalExpansion.Zero;
                }
            }
            else if (!isOptional)
                owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
            else
            {
                if (unitNumber == null)
                    return UnitsNet.CoefficientOfThermalExpansion.Zero;
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
        internal static Strain Strain(GH_Component owner, IGH_DataAccess DA, int inputid, StrainUnit strainUnit, bool isOptional = false)
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
                        return Oasys.Units.Strain.Zero;
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
                    return Oasys.Units.Strain.Zero;
                }
                return strainFib;
            }
            else if (!isOptional)
            {
                owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
            }
            return Oasys.Units.Strain.Zero;
        }
        internal static Curvature Curvature(GH_Component owner, IGH_DataAccess DA, int inputid, CurvatureUnit curvatureUnit, bool isOptional = false)
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
                        return Oasys.Units.Curvature.Zero;
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
                    return Oasys.Units.Curvature.Zero;
                }
                return crvature;
            }
            else if (!isOptional)
            {
                owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
            }
            return Oasys.Units.Curvature.Zero;
        }
        internal static Force Force(GH_Component owner, IGH_DataAccess DA, int inputid, ForceUnit forceUnit, bool isOptional = false)
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
                        return UnitsNet.Force.Zero;
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
                    return UnitsNet.Force.Zero;
                }
                return force;
            }
            else if (!isOptional)
            {
                owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
            }
            return UnitsNet.Force.Zero;
        }
        internal static ForcePerLength ForcePerLength(GH_Component owner, IGH_DataAccess DA, int inputid, ForcePerLengthUnit forceUnit, bool isOptional = false)
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
                        return UnitsNet.ForcePerLength.Zero;
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
                    return UnitsNet.ForcePerLength.Zero;
                }
                return force;
            }
            else if (!isOptional)
            {
                owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
            }
            return UnitsNet.ForcePerLength.Zero;
        }
        internal static Moment Moment(GH_Component owner, IGH_DataAccess DA, int inputid, MomentUnit momentUnit, bool isOptional = false)
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
                        return Oasys.Units.Moment.Zero;
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
                    return Oasys.Units.Moment.Zero;
                }
                return moment;
            }
            else if (!isOptional)
            {
                owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
            }
            return Oasys.Units.Moment.Zero;
        }

        internal static Angle Angle(GH_Component owner, IGH_DataAccess DA, int inputid, AngleUnit angleUnit, bool isOptional = false)
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
                        return UnitsNet.Angle.Zero;
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
                    return UnitsNet.Angle.Zero;
                }
                return (Angle)a1.Value;
            }
            else if (!isOptional)
            {
                owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
            }
            return UnitsNet.Angle.Zero;
        }
    }
}
