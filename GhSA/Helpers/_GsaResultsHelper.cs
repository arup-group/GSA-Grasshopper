using GsaAPI;
using Oasys.Units;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;

namespace GhSA.Util.Gsa
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
            double y = new Length(result.YY, LengthUnit.Meter).As(unit);
            double z = new Length(result.ZZ, LengthUnit.Meter).As(unit);
            return new Vector3d(x, y, z);
        }
        public static Vector3d GetResult(Double6 result, AngleUnit unit)
        {
            double xx = new Angle(result.XX, AngleUnit.Radian).As(unit);
            double yy = new Angle(result.YY, AngleUnit.Radian).As(unit);
            double zz = new Angle(result.ZZ, AngleUnit.Radian).As(unit);
            return new Vector3d(xx, yy, zz);
        }
    }
}
