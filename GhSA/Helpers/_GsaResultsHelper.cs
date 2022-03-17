using GsaAPI;
using Oasys.Units;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using UnitsNet;
using UnitsNet.Units;

namespace GsaGH.Util.Gsa
{
    class ResultHelper
    {
        public static List<double> SmartRounder(double max, double min)
        {
            // find the biggest abs value of max and min
            double val = Math.Max(Math.Abs(max), Math.Abs(min));

            // round that with 4 significant digits
            double scale = RoundToSignificantDigits(val, 4);

            // list to hold output values
            List<double> roundedvals = new List<double>();

            // do max
            if (max == 0)
                roundedvals.Add(0);
            else
            {
                double tempmax = scale * Math.Round(max / (scale), 4);
                tempmax = Math.Ceiling(tempmax * 1000) / 1000;
                roundedvals.Add(tempmax);
            }

            // do min
            if (min == 0)
                roundedvals.Add(0);
            else
            {
                double tempmin = scale * Math.Round(min / (scale), 4);
                tempmin = Math.Floor(tempmin * 1000) / 1000;
                roundedvals.Add(tempmin);
            }

            return roundedvals;
        }
        public static double RoundToSignificantDigits(double d, int digits)
        {
            
            if (d == 0.0)
            {
                return 0.0;
            }
            else
            {
                double leftSideNumbers = Math.Floor(Math.Log10(Math.Abs(d))) + 1;
                double scale = Math.Pow(10, leftSideNumbers);
                double result = scale * Math.Round(d / scale, digits, MidpointRounding.AwayFromZero);

                // Clean possible precision error.
                if ((int)leftSideNumbers >= digits)
                {
                    return Math.Round(result, 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    if (Math.Abs(digits - (int)leftSideNumbers) > 15)
                        return 0.0;
                    return Math.Round(result, digits - (int)leftSideNumbers, MidpointRounding.AwayFromZero);
                }
            }
        }
        public static Vector3d GetResult(Double6 result, ForceUnit unit)
        {
            double x = new Force(result.X, ForceUnit.Newton).As(unit);
            double y = new Force(result.Y, ForceUnit.Newton).As(unit);
            double z = new Force(result.Z, ForceUnit.Newton).As(unit);
            return new Vector3d(x, y, z);
        }
        public static Vector3d GetResult(Double6 result, MomentUnit unit)
        {
            double xx = new Moment(result.XX, MomentUnit.NewtonMeter).As(unit);
            double yy = new Moment(result.YY, MomentUnit.NewtonMeter).As(unit);
            double zz = new Moment(result.ZZ, MomentUnit.NewtonMeter).As(unit);
            return new Vector3d(xx, yy, zz);
        }
        public static Vector3d GetResult(Double6 result, LengthUnit unit)
        {
            double x = new Length(result.X, LengthUnit.Meter).As(unit);
            double y = new Length(result.Y, LengthUnit.Meter).As(unit);
            double z = new Length(result.Z, LengthUnit.Meter).As(unit);
            return new Vector3d(x, y, z);
        }
        public static Vector3d GetResult(Double3 result, LengthUnit unit)
        {
            double x = new Length(result.X, LengthUnit.Meter).As(unit);
            double y = new Length(result.Y, LengthUnit.Meter).As(unit);
            double z = new Length(result.Z, LengthUnit.Meter).As(unit);
            return new Vector3d(x, y, z);
        }
        public static Vector3d GetResult(Double6 result, AngleUnit unit)
        {
            double xx = 0; 
            if (!double.IsNaN(result.XX))
                new Angle(result.XX, AngleUnit.Radian).As(unit);
            double yy = 0;
            if (!double.IsNaN(result.YY)) 
                new Angle(result.YY, AngleUnit.Radian).As(unit);
            double zz = 0;
            if (!double.IsNaN(result.ZZ)) 
                new Angle(result.ZZ, AngleUnit.Radian).As(unit);
            return new Vector3d(xx, yy, zz);
        }
        public static Vector3d GetResult(Double6 result, PressureUnit unit, bool shear = false)
        {
            if (shear)
            {
                double xx = new Pressure(result.XX, PressureUnit.Pascal).As(unit);
                double yy = new Pressure(result.YY, PressureUnit.Pascal).As(unit);
                double zz = new Pressure(result.ZZ, PressureUnit.Pascal).As(unit);
                return new Vector3d(xx, yy, zz);
            }
            else
            {
                double x = new Pressure(result.X, PressureUnit.Pascal).As(unit);
                double y = new Pressure(result.Y, PressureUnit.Pascal).As(unit);
                double z = new Pressure(result.Z, PressureUnit.Pascal).As(unit);
                return new Vector3d(x, y, z);
            }
        }
        public static Vector3d GetResult(Tensor2 result, ForceUnit unit)
        {
            double x = new Force(result.XX, ForceUnit.Newton).As(unit);
            double y = new Force(result.YY, ForceUnit.Newton).As(unit);
            double z = new Force(result.XY, ForceUnit.Newton).As(unit);
            return new Vector3d(x, y, z);
        }
        public static Vector3d GetResult(Tensor2 result, MomentUnit unit)
        {
            double x = new Moment(result.XX, MomentUnit.NewtonMeter).As(unit);
            double y = new Moment(result.YY, MomentUnit.NewtonMeter).As(unit);
            double z = new Moment(result.XY, MomentUnit.NewtonMeter).As(unit);
            return new Vector3d(x, y, z);
        }
        public static Vector3d GetResult(Vector2 result, ForceUnit unit)
        {
            double x = new Force(result.X, ForceUnit.Newton).As(unit);
            double y = new Force(result.Y, ForceUnit.Newton).As(unit);
            double z = 0;
            return new Vector3d(x, y, z);
        }
        public static Vector3d GetResult(Tensor3 result, PressureUnit unit, bool shear = false)
        {
            if (shear)
            {
                double xy = new Pressure(result.XY, PressureUnit.Pascal).As(unit);
                double yz = new Pressure(result.YZ, PressureUnit.Pascal).As(unit);
                double zx = new Pressure(result.ZX, PressureUnit.Pascal).As(unit);
                return new Vector3d(xy, yz, zx);
            }
            else
            {
                double xx = new Pressure(result.XX, PressureUnit.Pascal).As(unit);
                double yy = new Pressure(result.YY, PressureUnit.Pascal).As(unit);
                double zz = new Pressure(result.ZZ, PressureUnit.Pascal).As(unit);
                return new Vector3d(xx, yy, zz);
            }
        }
        public static Vector3d GetResult(Vector3 result, MassUnit unit)
        {
            double x = new Mass(result.X, MassUnit.Kilogram).As(unit);
            double y = new Mass(result.Y, MassUnit.Kilogram).As(unit);
            double z = new Mass(result.Z, MassUnit.Kilogram).As(unit);
            return new Vector3d(x, y, z);
        }
        public static Vector3d GetResult(Vector3 result, AreaMomentOfInertiaUnit unit)
        {
            double x = new AreaMomentOfInertia(result.X, AreaMomentOfInertiaUnit.MeterToTheFourth).As(unit);
            double y = new AreaMomentOfInertia(result.Y, AreaMomentOfInertiaUnit.MeterToTheFourth).As(unit);
            double z = new AreaMomentOfInertia(result.Z, AreaMomentOfInertiaUnit.MeterToTheFourth).As(unit);
            return new Vector3d(x, y, z);
        }
    }
}
