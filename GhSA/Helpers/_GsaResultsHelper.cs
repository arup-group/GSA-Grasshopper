using GsaAPI;
using GsaGH.Parameters;
using Oasys.Units;
using Rhino.Geometry;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;

namespace GsaGH.Util.Gsa
{
    class ResultHelper
    {
        internal static List<double> SmartRounder(double max, double min)
        {
            // list to hold output values
            List<double> roundedvals = new List<double>();
            
            // check if both are zero then return
            if (max == 0 & min == 0)
            {
                roundedvals.Add(max);
                roundedvals.Add(min);
                roundedvals.Add(0);
                return roundedvals;
            }
            int signMax = Math.Sign(max);
            int signMin = Math.Sign(min);

            int significantNumbers = 2;

            // find the biggest abs value of max and min
            double val = Math.Max(Math.Abs(max), Math.Abs(min));
            max = Math.Abs(max);
            min = Math.Abs(min);

            // a value for how to round the values on the legend
            int numberOfDigitsOut = significantNumbers;
            //factor for scaling small numbers (0.00012312451)
            double factor = 1;
            if (val < 1)
            {
                // count the number of zeroes after the decimal point
                string valString = val.ToString().Split('.')[1];
                int digits = 0;
                while (valString[digits] == '0')
                    digits++;
                // create the factor, we want to remove the zeroes as well as making it big enough for rounding
                factor = Math.Pow(10, digits + 1);
                // scale up max/min values 
                max = max * factor;
                min = min * factor;
                max = (signMax > 0) ? Math.Ceiling(max) : Math.Floor(max);
                min = (signMin > 0) ? Math.Floor(min) : Math.Ceiling(min);
                max = max / factor;
                min = min / factor;
                numberOfDigitsOut = digits + significantNumbers;
            }
            else
            {
                string valString = val.ToString();
                // count the number of digits before the decimal point
                int digits = valString.Split('.')[0].Count();
                // create the factor, we want to remove the zeroes as well as making it big enough for rounding
                int power = 10;
                if (val < 500)
                    power = 5;

                factor = Math.Pow(power, digits - 1);
                // scale up max/min values 
                max = max / factor;
                min = min / factor;
                max = (signMax > 0) ? Math.Ceiling(max) : Math.Floor(max);
                min = (signMin > 0) ? Math.Floor(min) : Math.Ceiling(min);
                max = max * factor;
                min = min * factor;
                numberOfDigitsOut = significantNumbers;
            }
            
            roundedvals.Add(max * signMax);
            roundedvals.Add(min * signMin);
            roundedvals.Add(numberOfDigitsOut);

            return roundedvals;
        }
        internal static double RoundToSignificantDigits(double d, int digits)
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
        #region Vector3d results
        internal static Vector3d GetResult(Double6 result, ForceUnit unit)
        {
            double x = new Force(result.X, ForceUnit.Newton).As(unit);
            double y = new Force(result.Y, ForceUnit.Newton).As(unit);
            double z = new Force(result.Z, ForceUnit.Newton).As(unit);
            return new Vector3d(x, y, z);
        }
        internal static Vector3d GetResult(Double6 result, MomentUnit unit)
        {
            double xx = new Moment(result.XX, MomentUnit.NewtonMeter).As(unit);
            double yy = new Moment(result.YY, MomentUnit.NewtonMeter).As(unit);
            double zz = new Moment(result.ZZ, MomentUnit.NewtonMeter).As(unit);
            return new Vector3d(xx, yy, zz);
        }
        internal static Vector3d GetResult(Double6 result, LengthUnit unit)
        {
            double x = new Length(result.X, LengthUnit.Meter).As(unit);
            double y = new Length(result.Y, LengthUnit.Meter).As(unit);
            double z = new Length(result.Z, LengthUnit.Meter).As(unit);
            return new Vector3d(x, y, z);
        }
        internal static Vector3d GetResult(Double3 result, LengthUnit unit)
        {
            double x = new Length(result.X, LengthUnit.Meter).As(unit);
            double y = new Length(result.Y, LengthUnit.Meter).As(unit);
            double z = new Length(result.Z, LengthUnit.Meter).As(unit);
            return new Vector3d(x, y, z);
        }
        internal static Vector3d GetResult(Double6 result, AngleUnit unit)
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
        internal static Vector3d GetResult(Double6 result, PressureUnit unit, bool shear = false)
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
        internal static Vector3d GetResult(Tensor2 result, ForceUnit unit)
        {
            double x = new Force(result.XX, ForceUnit.Newton).As(unit);
            double y = new Force(result.YY, ForceUnit.Newton).As(unit);
            double z = new Force(result.XY, ForceUnit.Newton).As(unit);
            return new Vector3d(x, y, z);
        }
        internal static Vector3d GetResult(Tensor2 result, MomentUnit unit)
        {
            double x = new Moment(result.XX, MomentUnit.NewtonMeter).As(unit);
            double y = new Moment(result.YY, MomentUnit.NewtonMeter).As(unit);
            double z = new Moment(result.XY, MomentUnit.NewtonMeter).As(unit);
            return new Vector3d(x, y, z);
        }
        internal static Vector3d GetResult(Vector2 result, ForceUnit unit)
        {
            double x = new Force(result.X, ForceUnit.Newton).As(unit);
            double y = new Force(result.Y, ForceUnit.Newton).As(unit);
            double z = 0;
            return new Vector3d(x, y, z);
        }
        internal static Vector3d GetResult(Tensor3 result, PressureUnit unit, bool shear = false)
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
        internal static Vector3d GetResult(Vector3 result, MassUnit unit)
        {
            double x = new Mass(result.X, MassUnit.Kilogram).As(unit);
            double y = new Mass(result.Y, MassUnit.Kilogram).As(unit);
            double z = new Mass(result.Z, MassUnit.Kilogram).As(unit);
            return new Vector3d(x, y, z);
        }
        internal static Vector3d GetResult(Vector3 result, AreaMomentOfInertiaUnit unit)
        {
            double x = new AreaMomentOfInertia(result.X, AreaMomentOfInertiaUnit.MeterToTheFourth).As(unit);
            double y = new AreaMomentOfInertia(result.Y, AreaMomentOfInertiaUnit.MeterToTheFourth).As(unit);
            double z = new AreaMomentOfInertia(result.Z, AreaMomentOfInertiaUnit.MeterToTheFourth).As(unit);
            return new Vector3d(x, y, z);
        }
        #endregion
        #region Quantity results
        internal static GsaResultQuantity GetQuantityResult(Double6 result, ForceUnit unit, bool isBeam = false)
        {
            IQuantity x = new Force(new Force(result.X, ForceUnit.Newton).As(unit), unit);
            IQuantity y = new Force(new Force(result.Y, ForceUnit.Newton).As(unit), unit);
            IQuantity z = new Force(new Force(result.Z, ForceUnit.Newton).As(unit), unit);
            double pyth = 0;
            if (isBeam)
                pyth = Math.Sqrt(Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2)); // absolute shear is only |YZ|
            else
                pyth = Math.Sqrt(Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2));
            IQuantity xyz = new Force(new Force(pyth, ForceUnit.Newton).As(unit), unit);
            return new GsaResultQuantity() { X = x, Y = y, Z = z, XYZ = xyz};
        }
        internal static GsaResultQuantity GetQuantityResult(Double6 result, MomentUnit unit, bool isBeam = false)
        {
            IQuantity xx = new Moment(new Moment(result.XX, MomentUnit.NewtonMeter).As(unit), unit);
            IQuantity yy = new Moment(new Moment(result.YY, MomentUnit.NewtonMeter).As(unit), unit);
            IQuantity zz = new Moment(new Moment(result.ZZ, MomentUnit.NewtonMeter).As(unit), unit);
            double pyth = 0;
            if (isBeam)
                pyth = Math.Sqrt(Math.Pow(result.YY, 2) + Math.Pow(result.ZZ, 2)); // absolute bending is only |YZ|
            else
                pyth = Math.Sqrt(Math.Pow(result.XX, 2) + Math.Pow(result.YY, 2) + Math.Pow(result.ZZ, 2));
            IQuantity xxyyzz = new Moment(new Moment(pyth, MomentUnit.NewtonMeter).As(unit), unit);
            return new GsaResultQuantity() { X = xx, Y = yy, Z = zz, XYZ = xxyyzz };
        }
        internal static GsaResultQuantity GetQuantityResult(Double6 result, LengthUnit unit)
        {
            IQuantity x = new Length(new Length(result.X, LengthUnit.Meter).As(unit), unit);
            IQuantity y = new Length(new Length(result.Y, LengthUnit.Meter).As(unit), unit);
            IQuantity z = new Length(new Length(result.Z, LengthUnit.Meter).As(unit), unit);
            double pyth = Math.Sqrt(Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2));
            IQuantity xyz = new Length(new Length(pyth, LengthUnit.Meter).As(unit), unit);
            return new GsaResultQuantity() { X = x, Y = y, Z = z, XYZ = xyz };
        }
        internal static GsaResultQuantity GetQuantityResult(Double3 result, LengthUnit unit)
        {
            IQuantity x = new Length(new Length(result.X, LengthUnit.Meter).As(unit), unit);
            IQuantity y = new Length(new Length(result.Y, LengthUnit.Meter).As(unit), unit);
            IQuantity z = new Length(new Length(result.Z, LengthUnit.Meter).As(unit), unit);
            double pyth = Math.Sqrt(Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2));
            IQuantity xyz = new Length(new Length(pyth, LengthUnit.Meter).As(unit), unit);
            return new GsaResultQuantity() { X = x, Y = y, Z = z, XYZ = xyz };
        }
        internal static GsaResultQuantity GetQuantityResult(Double6 result, AngleUnit unit)
        {
            IQuantity x;
            if (!double.IsNaN(result.XX))
                x = new Angle(new Angle(result.XX, AngleUnit.Radian).As(unit), unit);
            else
                x = new Angle(0, unit);

            IQuantity y;
            if (!double.IsNaN(result.YY))
                y = new Angle(new Angle(result.YY, AngleUnit.Radian).As(unit), unit);
            else
                y = new Angle(0, unit);

            IQuantity z;
            if (!double.IsNaN(result.ZZ))
                z = new Angle(new Angle(result.ZZ, AngleUnit.Radian).As(unit), unit);
            else
                z = new Angle(0, unit);
            double pyth = Math.Sqrt(Math.Pow(x.Value, 2) + Math.Pow(y.Value, 2) + Math.Pow(z.Value, 2));
            IQuantity xyz = new Angle(new Angle(pyth, AngleUnit.Radian).As(unit), unit);
            return new GsaResultQuantity() { X = x, Y = y, Z = z, XYZ = xyz };
        }
        internal static GsaResultQuantity GetQuantityResult(Double6 result, PressureUnit unit, bool shear = false)
        {
            if (shear)
            {
                IQuantity x = new Pressure(new Pressure(result.XX, PressureUnit.Pascal).As(unit), unit);
                IQuantity y = new Pressure(new Pressure(result.YY, PressureUnit.Pascal).As(unit), unit);
                IQuantity z = new Pressure(new Pressure(result.ZZ, PressureUnit.Pascal).As(unit), unit);
                double pyth = Math.Sqrt(Math.Pow(result.XX, 2) + Math.Pow(result.YY, 2) + Math.Pow(result.ZZ, 2));
                IQuantity xyz = new Pressure(new Pressure(pyth, PressureUnit.Pascal).As(unit), unit);
                return new GsaResultQuantity() { X = x, Y = y, Z = z, XYZ = xyz };
            }
            else
            {
                IQuantity x = new Pressure(new Pressure(result.X, PressureUnit.Pascal).As(unit), unit);
                IQuantity y = new Pressure(new Pressure(result.Y, PressureUnit.Pascal).As(unit), unit);
                IQuantity z = new Pressure(new Pressure(result.Z, PressureUnit.Pascal).As(unit), unit);
                double pyth = Math.Sqrt(Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2));
                IQuantity xyz = new Pressure(new Pressure(pyth, PressureUnit.Pascal).As(unit), unit);
                return new GsaResultQuantity() { X = x, Y = y, Z = z, XYZ = xyz };
            }
        }
        internal static GsaResultQuantity GetQuantityResult(Tensor2 result, ForcePerLengthUnit unit)
        {
            IQuantity x = new ForcePerLength(new ForcePerLength(result.XX, ForcePerLengthUnit.NewtonPerMeter).As(unit), unit);
            IQuantity y = new ForcePerLength(new ForcePerLength(result.YY, ForcePerLengthUnit.NewtonPerMeter).As(unit), unit);
            IQuantity z = new ForcePerLength(new ForcePerLength(result.XY, ForcePerLengthUnit.NewtonPerMeter).As(unit), unit);
            return new GsaResultQuantity() { X = x, Y = y, Z = z, XYZ = null };
        }
        internal static GsaResultQuantity GetQuantityResult(Tensor2 result, ForceUnit unit)
        {
            IQuantity xx = new Force(new Force(result.XX, ForceUnit.Newton).As(unit), unit);
            IQuantity yy = new Force(new Force(result.YY, ForceUnit.Newton).As(unit), unit);
            IQuantity zz = new Force(new Force(result.XY, ForceUnit.Newton).As(unit), unit);
            return new GsaResultQuantity() { X = xx, Y = yy, Z = zz, XYZ = null };
        }
        internal static GsaResultQuantity GetQuantityResult(Vector2 result, ForcePerLengthUnit unit)
        {
            IQuantity x = new ForcePerLength(new ForcePerLength(result.X, ForcePerLengthUnit.NewtonPerMeter).As(unit), unit);
            IQuantity y = new ForcePerLength(new ForcePerLength(result.Y, ForcePerLengthUnit.NewtonPerMeter).As(unit), unit);
            return new GsaResultQuantity() { X = x, Y = y, Z = null, XYZ = null };
        }
        internal static GsaResultQuantity GetQuantityResult(Tensor3 result, PressureUnit unit, bool shear = false)
        {
            if (shear)
            {
                IQuantity x = new Pressure(new Pressure(result.XY, PressureUnit.Pascal).As(unit), unit);
                IQuantity y = new Pressure(new Pressure(result.YZ, PressureUnit.Pascal).As(unit), unit);
                IQuantity z = new Pressure(new Pressure(result.ZX, PressureUnit.Pascal).As(unit), unit);
                return new GsaResultQuantity() { X = x, Y = y, Z = z, XYZ = null };
            }
            else
            {
                IQuantity x = new Pressure(new Pressure(result.XX, PressureUnit.Pascal).As(unit), unit);
                IQuantity y = new Pressure(new Pressure(result.YY, PressureUnit.Pascal).As(unit), unit);
                IQuantity z = new Pressure(new Pressure(result.ZZ, PressureUnit.Pascal).As(unit), unit);
                return new GsaResultQuantity() { X = x, Y = y, Z = z, XYZ = null };
            }
        }
        internal static GsaResultQuantity GetQuantityResult(Vector3 result, MassUnit unit)
        {
            IQuantity x = new Mass(new Mass(result.X, MassUnit.Kilogram).As(unit), unit);
            IQuantity y = new Mass(new Mass(result.Y, MassUnit.Kilogram).As(unit), unit);
            IQuantity z = new Mass(new Mass(result.Z, MassUnit.Kilogram).As(unit), unit);
            double pyth = Math.Sqrt(Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2));
            IQuantity xyz = new Mass(new Mass(pyth, MassUnit.Kilogram).As(unit), unit);
            return new GsaResultQuantity() { X = x, Y = y, Z = z, XYZ = xyz };
        }
        internal static GsaResultQuantity GetQuantityResult(Vector3 result, AreaMomentOfInertiaUnit unit)
        {
            IQuantity x = new AreaMomentOfInertia(new AreaMomentOfInertia(result.X, AreaMomentOfInertiaUnit.MeterToTheFourth).As(unit), unit);
            IQuantity y = new AreaMomentOfInertia(new AreaMomentOfInertia(result.Y, AreaMomentOfInertiaUnit.MeterToTheFourth).As(unit), unit);
            IQuantity z = new AreaMomentOfInertia(new AreaMomentOfInertia(result.Z, AreaMomentOfInertiaUnit.MeterToTheFourth).As(unit), unit);
            double pyth = Math.Sqrt(Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2));
            IQuantity xyz = new AreaMomentOfInertia(new AreaMomentOfInertia(pyth, AreaMomentOfInertiaUnit.MeterToTheFourth).As(unit), unit);
            return new GsaResultQuantity() { X = x, Y = y, Z = z, XYZ = xyz };
        }
        #endregion

        #region Analysis Case Quantity Results
        /// <summary>
        /// Returns displacement result values
        /// </summary>
        /// <param name="globalResults"></param>
        /// <param name="resultLengthUnit"></param>
        /// <returns></returns>
        internal static GsaResultsValues GetElement3DResultValues(ReadOnlyDictionary<int, Element3DResult> globalResults, 
            LengthUnit resultLengthUnit)
        {
            GsaResultsValues r = new GsaResultsValues();
            r.Type = GsaResultsValues.ResultType.Displacement;

            Parallel.ForEach(globalResults.Keys, key =>
            {
                // lists for results
                Element3DResult elementResults = globalResults[key];
                ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                xyzRes.AsParallel().AsOrdered();

                List<Double3> trans_vals = elementResults.Displacement.ToList();
                Parallel.For(1, trans_vals.Count, i => //foreach (Double3 val in trans_vals)
                {
                    xyzRes[i] = GetQuantityResult(trans_vals[i], resultLengthUnit);

                });
                xyzRes[trans_vals.Count] = GetQuantityResult(trans_vals[0], resultLengthUnit); // add centre point at the end

                // add vector lists to main lists
                r.xyzResults.TryAdd(key, xyzRes);
            });

            r.UpdateMinMax();

            return r;
        }
        /// <summary>
        /// Returns stress result values
        /// </summary>
        /// <param name="globalResults"></param>
        /// <param name="stressUnit"></param>
        /// <returns></returns>
        internal static GsaResultsValues GetElement3DResultValues(ReadOnlyDictionary<int, Element3DResult> globalResults,
            PressureUnit stressUnit)
        {
            GsaResultsValues r = new GsaResultsValues();
            r.Type = GsaResultsValues.ResultType.Stress;

            Parallel.ForEach(globalResults.Keys, key =>
            {
                // lists for results
                Element3DResult elementResults = globalResults[key];
                ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                xyzRes.AsParallel().AsOrdered();
                ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                xxyyzzRes.AsParallel().AsOrdered();

                List<Tensor3> stress_vals = elementResults.Stress.ToList();
                Parallel.For(1, stress_vals.Count * 2, i => // (Tensor3 val in stress_vals)
                {
                    if (i == stress_vals.Count)
                        return;
                    // split computation into two parts by doubling the i-counter
                    if (i < stress_vals.Count)
                        xyzRes[i] = GetQuantityResult(stress_vals[i], stressUnit);
                    else
                        xxyyzzRes[i - stress_vals.Count] = GetQuantityResult(stress_vals[i - stress_vals.Count], stressUnit, true);
                });
                xyzRes[stress_vals.Count] = GetQuantityResult(stress_vals[0], stressUnit); // add centre point at the end
                xxyyzzRes[stress_vals.Count] = GetQuantityResult(stress_vals[0], stressUnit, true);

                // add vector lists to main lists
                r.xyzResults.TryAdd(key, xyzRes); 
                r.xxyyzzResults.TryAdd(key, xxyyzzRes);
            });

            r.UpdateMinMax();

            return r;
        }
        /// <summary>
        /// Returns stress result values
        /// </summary>
        /// <param name="globalResults"></param>
        /// <param name="stressUnit"></param>
        /// <returns></returns>
        internal static GsaResultsValues GetElement2DResultValues(ReadOnlyDictionary<int, Element2DResult> globalResults,
            PressureUnit stressUnit)
        {
            GsaResultsValues r = new GsaResultsValues();
            r.Type = GsaResultsValues.ResultType.Stress;

            Parallel.ForEach(globalResults.Keys, key =>
            {
                // lists for results
                Element2DResult elementResults = globalResults[key];
                ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                xyzRes.AsParallel().AsOrdered();
                ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                xxyyzzRes.AsParallel().AsOrdered();

                List<Tensor3> stresses = elementResults.Stress.ToList();
                Parallel.For(1, stresses.Count * 2, i => // (Tensor3 stress in stresses)
                {
                    if (i == stresses.Count)
                        return;
                    // split computation into two parts by doubling the i-counter
                    if (i < stresses.Count)
                        xyzRes[i] = GetQuantityResult(stresses[i], stressUnit, false);
                    else
                        xxyyzzRes[i - stresses.Count] = GetQuantityResult(stresses[i - stresses.Count], stressUnit, true);
                });
                xyzRes[stresses.Count] = GetQuantityResult(stresses[0], stressUnit, false); // add centre point at the end
                xxyyzzRes[stresses.Count] = GetQuantityResult(stresses[0], stressUnit, true);

                // add vector lists to main lists
                r.xyzResults.TryAdd(key, xyzRes);
                r.xxyyzzResults.TryAdd(key, xxyyzzRes);
            });

            r.UpdateMinMax();

            return r;
        }
        /// <summary>
        /// Returns shear result values
        /// </summary>
        /// <param name="globalResults"></param>
        /// <param name="forceUnit"></param>
        /// <returns></returns>
        internal static GsaResultsValues GetElement2DResultValues(ReadOnlyDictionary<int, Element2DResult> globalResults,
            ForcePerLengthUnit forceUnit)
        {
            GsaResultsValues r = new GsaResultsValues();
            r.Type = GsaResultsValues.ResultType.Shear;

            Parallel.ForEach(globalResults.Keys, key =>
            {
                // lists for results
                Element2DResult elementResults = globalResults[key];
                ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                xyzRes.AsParallel().AsOrdered();
                ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                xxyyzzRes.AsParallel().AsOrdered();

                List<Vector2> shears = elementResults.Shear.ToList();
                Parallel.For(1, shears.Count, i => // (Vector2 shear in shears)
                {
                    xyzRes[i] = GetQuantityResult(shears[i], forceUnit);
                });
                xyzRes[shears.Count] = GetQuantityResult(shears[0], forceUnit); // add centre point at the end

                // add vector lists to main lists
                r.xyzResults.TryAdd(key, xyzRes);
                r.xxyyzzResults.TryAdd(key, xxyyzzRes);
            });

            r.UpdateMinMax();

            return r;
        }
        /// <summary>
        /// Returns force & moment result values
        /// </summary>
        /// <param name="globalResults"></param>
        /// <param name="forceUnit"></param>
        /// <param name="momentUnit"></param>
        /// <returns></returns>
        internal static GsaResultsValues GetElement2DResultValues(ReadOnlyDictionary<int, Element2DResult> globalResults, 
            ForcePerLengthUnit forceUnit, ForceUnit momentUnit)
        {
            GsaResultsValues r = new GsaResultsValues();
            r.Type = GsaResultsValues.ResultType.Force;

            Parallel.ForEach(globalResults.Keys, key =>
            {
                // lists for results
                Element2DResult elementResults = globalResults[key];
                ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                xyzRes.AsParallel().AsOrdered();
                ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                xxyyzzRes.AsParallel().AsOrdered();

                List<Tensor2> forces = elementResults.Force.ToList();
                List<Tensor2> moments = elementResults.Moment.ToList();
                Parallel.For(1, forces.Count + moments.Count, i => // (Tensor2 force in forces)
                {
                    if (i == forces.Count)
                        return;

                    // combine forces and momemts (list lengths must be the same) to run
                    // calculations in parallel by doubling the i-counter
                    if (i < forces.Count)
                        xyzRes[i] = GetQuantityResult(forces[i], forceUnit);
                    else
                        xxyyzzRes[i - forces.Count] = GetQuantityResult(moments[i - forces.Count], momentUnit);
                });
                xyzRes[forces.Count] = GetQuantityResult(forces[0], forceUnit); // add centre point at the end
                xxyyzzRes[moments.Count] = GetQuantityResult(moments[0], momentUnit);

                // add vector lists to main lists
                r.xyzResults.TryAdd(key, xyzRes);
                r.xxyyzzResults.TryAdd(key, xxyyzzRes);
            });

            r.UpdateMinMax();

            return r;
        }
        /// <summary>
        /// Returns displacement result values
        /// </summary>
        /// <param name="globalResults"></param>
        /// <param name="resultLengthUnit"></param>
        /// <returns></returns>
        internal static GsaResultsValues GetElement2DResultValues(ReadOnlyDictionary<int, Element2DResult> globalResults,
            LengthUnit resultLengthUnit)
        {
            GsaResultsValues r = new GsaResultsValues();
            r.Type = GsaResultsValues.ResultType.Displacement;

            Parallel.ForEach(globalResults.Keys, key =>
            {
                // lists for results
                Element2DResult elementResults = globalResults[key];
                ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                xyzRes.AsParallel().AsOrdered();
                ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                xxyyzzRes.AsParallel().AsOrdered();

                List<Double6> disp = elementResults.Displacement.ToList();
                Parallel.For(1, disp.Count * 2, i => // (Double6 val in values)
                {
                    if (i == disp.Count)
                        return;
                    // split computation into two parts by doubling the i-counter
                    if (i < disp.Count)
                        xyzRes[i] = GetQuantityResult(disp[i], resultLengthUnit);
                    else
                        xxyyzzRes[i - disp.Count] = GetQuantityResult(disp[i - disp.Count], AngleUnit.Radian);
                });
                xyzRes[disp.Count] = GetQuantityResult(disp[0], resultLengthUnit); // add centre point at the end
                xxyyzzRes[disp.Count - disp.Count] = GetQuantityResult(disp[0], AngleUnit.Radian);

                // add vector lists to main lists
                r.xyzResults.TryAdd(key, xyzRes);
                r.xxyyzzResults.TryAdd(key, xxyyzzRes);
            });

            r.UpdateMinMax();

            return r;
        }
        /// <summary>
        /// Returns forces result values
        /// </summary>
        /// <param name="globalResults"></param>
        /// <param name="forceUnit"></param>
        /// <param name="momentUnit"></param>
        /// <returns></returns>
        internal static GsaResultsValues GetElement1DResultValues(ReadOnlyDictionary<int, Element1DResult> globalResults,
            ForceUnit forceUnit, MomentUnit momentUnit)
        {
            GsaResultsValues r = new GsaResultsValues();
            r.Type = GsaResultsValues.ResultType.Force;

            Parallel.ForEach(globalResults.Keys, key =>
            {
                // lists for results
                Element1DResult elementResults = globalResults[key];
                List<Double6> values = new List<Double6>();
                ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                xyzRes.AsParallel().AsOrdered();
                ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                xxyyzzRes.AsParallel().AsOrdered();

                values = elementResults.Force.ToList();

                // loop through the results
                Parallel.For(0, values.Count, i =>
                {
                    Double6 result = values[i];

                    // add the values to the vector lists
                    xyzRes[i] = GetQuantityResult(result, forceUnit, true);
                    xxyyzzRes[i] = GetQuantityResult(result, momentUnit, true);
                });
                // add the vector list to the out tree
                r.xyzResults.TryAdd(key, xyzRes);
                r.xxyyzzResults.TryAdd(key, xxyyzzRes);
            });

            r.UpdateMinMax();

            return r;
        }
        /// <summary>
        /// Returns displacement result values
        /// </summary>
        /// <param name="globalResults"></param>
        /// <param name="resultLengthUnit"></param>
        /// <returns></returns>
        internal static GsaResultsValues GetElement1DResultValues(ReadOnlyDictionary<int, Element1DResult> globalResults, 
            LengthUnit resultLengthUnit)
        {
            GsaResultsValues r = new GsaResultsValues();
            r.Type = GsaResultsValues.ResultType.Displacement;

            Parallel.ForEach(globalResults.Keys, key =>
            {
                // lists for results
                Element1DResult elementResults = globalResults[key];
                List<Double6> values = new List<Double6>();
                ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                xyzRes.AsParallel().AsOrdered();
                ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                xxyyzzRes.AsParallel().AsOrdered();

                // set the result type dependent on user selection in dropdown
                values = elementResults.Displacement.ToList();

                // loop through the results
                Parallel.For(0, values.Count, i =>
                {
                    Double6 result = values[i];

                    // add the values to the vector lists
                    xyzRes[i] = GetQuantityResult(result, resultLengthUnit);
                    xxyyzzRes[i] = GetQuantityResult(result, AngleUnit.Radian);
                });
                // add the vector list to the out tree
                r.xyzResults.TryAdd(key, xyzRes);
                r.xxyyzzResults.TryAdd(key, xxyyzzRes);
            });

            r.UpdateMinMax();

            return r;
        }
        /// <summary>
        /// Returns displacement result values
        /// </summary>
        /// <param name="globalResults"></param>
        /// <param name="resultLengthUnit"></param>
        /// <returns></returns>
        internal static GsaResultsValues GetNodeResultValues(ReadOnlyDictionary<int, NodeResult> globalResults,
            LengthUnit resultLengthUnit)
        {
            GsaResultsValues r = new GsaResultsValues();
            r.Type = GsaResultsValues.ResultType.Displacement;

            Parallel.ForEach(globalResults.Keys, nodeID =>
            {
                NodeResult result = globalResults[nodeID];
                Double6 values = result.Displacement;

                ConcurrentDictionary<int, GsaResultQuantity> xyz = new ConcurrentDictionary<int, GsaResultQuantity>();
                xyz.TryAdd(0, GetQuantityResult(values, resultLengthUnit));
                r.xyzResults.TryAdd(nodeID, xyz);
                ConcurrentDictionary<int, GsaResultQuantity> xxyyzz = new ConcurrentDictionary<int, GsaResultQuantity>();
                xxyyzz.TryAdd(0, GetQuantityResult(values, AngleUnit.Radian));
                r.xxyyzzResults.TryAdd(nodeID, xxyyzz);
            });

            r.UpdateMinMax();

            return r;
        }
        /// <summary>
        /// Returns reaction forces result values
        /// </summary>
        /// <param name="globalResults"></param>
        /// <param name="forceUnit"></param>
        /// <param name="momentUnit"></param>
        /// <returns></returns>
        internal static GsaResultsValues GetNodeResultValues(ReadOnlyDictionary<int, NodeResult> globalResults,
            ForceUnit forceUnit, MomentUnit momentUnit)
        {
            GsaResultsValues r = new GsaResultsValues();
            r.Type = GsaResultsValues.ResultType.Force;

            Parallel.ForEach(globalResults.Keys, nodeID =>
            {
                NodeResult result = globalResults[nodeID];
                Double6 values = result.Reaction;

                ConcurrentDictionary<int, GsaResultQuantity> xyz = new ConcurrentDictionary<int, GsaResultQuantity>();
                xyz.TryAdd(0, GetQuantityResult(values, forceUnit));
                r.xyzResults.TryAdd(nodeID, xyz);
                ConcurrentDictionary<int, GsaResultQuantity> xxyyzz = new ConcurrentDictionary<int, GsaResultQuantity>();
                xxyyzz.TryAdd(0, GetQuantityResult(values, momentUnit));
                r.xxyyzzResults.TryAdd(nodeID, xxyyzz);
            });

            r.UpdateMinMax();

            return r;
        }
        #endregion

        #region Combination case quantity results
        /// <summary>
        /// Returns displacement result values
        /// </summary>
        /// <param name="globalResults"></param>
        /// <param name="resultLengthUnit"></param>
        /// <returns></returns>
        internal static ConcurrentDictionary<int, GsaResultsValues> GetElement3DResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<Element3DResult>> globalResults,
            LengthUnit lengthUnit, int permutation)
        {
            ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int, GsaResultsValues>();

            if (permutation > 0)
            {
                GsaResultsValues r = new GsaResultsValues();
                r.Type = GsaResultsValues.ResultType.Displacement;

                Parallel.ForEach(globalResults.Keys, key =>
                {
                    // lists for results
                    ReadOnlyCollection<Element3DResult> results = globalResults[key];
                    Element3DResult result = results[permutation - 1];
                    ReadOnlyCollection<Double3> values = result.Displacement;
                    ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                    xyzRes.AsParallel().AsOrdered();

                    // loop through the results
                    Parallel.For(0, values.Count, i =>
                    {
                        // add the values to the vector lists
                        xyzRes.TryAdd(i, GetQuantityResult(values[i], lengthUnit));
                    });
                    // add the vector list to the out tree
                    r.xyzResults.TryAdd(key, xyzRes);
                });
                r.UpdateMinMax();
                rs.TryAdd(permutation, r);
            }
            else
            {
                int permutationCount = globalResults[globalResults.Keys.First()].Count;

                Parallel.For(0, permutationCount, index => // loop through permutations
                {
                    GsaResultsValues r = new GsaResultsValues();
                    r.Type = GsaResultsValues.ResultType.Displacement;

                    Parallel.ForEach(globalResults.Keys, key =>
                    {
                        // lists for results
                        ReadOnlyCollection<Element3DResult> results = globalResults[key];
                        Element3DResult result = results[index];
                        ReadOnlyCollection<Double3> values = result.Displacement;
                        ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                        xyzRes.AsParallel().AsOrdered();

                        // loop through the results
                        Parallel.For(0, values.Count, i =>
                        {
                            // add the values to the vector lists
                            xyzRes.TryAdd(i, GetQuantityResult(values[i], lengthUnit));
                        });
                        // add the vector list to the out tree
                        r.xyzResults.TryAdd(key, xyzRes);
                    });
                    r.UpdateMinMax();
                    rs.TryAdd(index + 1, r); // permutation ID = index + 1
                });
            }

            return rs;
        }

        /// <summary>
        /// Returns stress result values
        /// </summary>
        /// <param name="globalResults"></param>
        /// <param name="stressUnit"></param>
        /// <returns></returns>
        internal static ConcurrentDictionary<int, GsaResultsValues> GetElement3DResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<Element3DResult>> globalResults,
            PressureUnit stressUnit, int permutation)
        {
            ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int, GsaResultsValues>();

            if (permutation > 0)
            {
                GsaResultsValues r = new GsaResultsValues();
                r.Type = GsaResultsValues.ResultType.Stress;

                Parallel.ForEach(globalResults.Keys, key =>
                {
                    // lists for results
                    ReadOnlyCollection<Element3DResult> results = globalResults[key];
                    Element3DResult result = results[permutation - 1];
                    ReadOnlyCollection<Tensor3> values = result.Stress;
                    ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                    xyzRes.AsParallel().AsOrdered();
                    ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                    xxyyzzRes.AsParallel().AsOrdered();

                    // loop through the results
                    Parallel.For(0, values.Count, i =>
                    {
                        // add the values to the vector lists
                        xyzRes.TryAdd(i, GetQuantityResult(values[i], stressUnit));
                        xxyyzzRes.TryAdd(i, GetQuantityResult(values[i], stressUnit, true));
                    });
                    // add the vector list to the out tree
                    r.xyzResults.TryAdd(key, xyzRes);
                    r.xxyyzzResults.TryAdd(key, xxyyzzRes);
                });
                r.UpdateMinMax();
                rs.TryAdd(permutation, r);
            }
            else
            {
                int permutationCount = globalResults[globalResults.Keys.First()].Count;

                Parallel.For(0, permutationCount, index => // loop through permutations
                {
                    GsaResultsValues r = new GsaResultsValues();
                    r.Type = GsaResultsValues.ResultType.Stress;

                    Parallel.ForEach(globalResults.Keys, key =>
                    {
                        // lists for results
                        ReadOnlyCollection<Element3DResult> results = globalResults[key];
                        Element3DResult result = results[index];
                        ReadOnlyCollection<Tensor3> values = result.Stress;
                        ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                        xyzRes.AsParallel().AsOrdered();
                        ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                        xxyyzzRes.AsParallel().AsOrdered();

                        // loop through the results
                        Parallel.For(0, values.Count, i =>
                        {
                            // add the values to the vector lists
                            xyzRes.TryAdd(i, GetQuantityResult(values[i], stressUnit));
                            xxyyzzRes.TryAdd(i, GetQuantityResult(values[i], stressUnit, true));
                        });
                        // add the vector list to the out tree
                        r.xyzResults.TryAdd(key, xyzRes);
                        r.xxyyzzResults.TryAdd(key, xxyyzzRes);
                    });
                    r.UpdateMinMax();
                    rs.TryAdd(index + 1, r); // permutation ID = index + 1
                });
            }

            return rs;
        }

        /// <summary>
        /// Returns stress result values
        /// </summary>
        /// <param name="globalResults"></param>
        /// <param name="stressUnit"></param>
        /// <returns></returns>
        internal static ConcurrentDictionary<int, GsaResultsValues> GetElement2DResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<Element2DResult>> globalResults,
            PressureUnit stressUnit, int permutation)
        {
            ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int, GsaResultsValues>();

            if (permutation > 0)
            {
                GsaResultsValues r = new GsaResultsValues();
                r.Type = GsaResultsValues.ResultType.Stress;

                Parallel.ForEach(globalResults.Keys, key =>
                {
                    // lists for results
                    ReadOnlyCollection<Element2DResult> results = globalResults[key];
                    Element2DResult result = results[permutation - 1];
                    ReadOnlyCollection<Tensor3> values = result.Stress;
                    ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                    xyzRes.AsParallel().AsOrdered();
                    ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                    xxyyzzRes.AsParallel().AsOrdered();

                    // loop through the results
                    Parallel.For(1, values.Count, i =>
                    {
                        // add the values to the vector lists
                        xyzRes.TryAdd(i, GetQuantityResult(values[i], stressUnit));
                        xxyyzzRes.TryAdd(i, GetQuantityResult(values[i], stressUnit, true));
                    });
                    xyzRes.TryAdd(values.Count, GetQuantityResult(values[0], stressUnit)); // add centre point last
                    xxyyzzRes.TryAdd(values.Count, GetQuantityResult(values[0], stressUnit, true)); // add centre point last
                    // add the vector list to the out tree
                    r.xyzResults.TryAdd(key, xyzRes);
                    r.xxyyzzResults.TryAdd(key, xxyyzzRes);
                });
                r.UpdateMinMax();
                rs.TryAdd(permutation, r);
            }
            else
            {
                int permutationCount = globalResults[globalResults.Keys.First()].Count;

                Parallel.For(0, permutationCount, index => // loop through permutations
                {
                    GsaResultsValues r = new GsaResultsValues();
                    r.Type = GsaResultsValues.ResultType.Stress;

                    Parallel.ForEach(globalResults.Keys, key =>
                    {
                        // lists for results
                        ReadOnlyCollection<Element2DResult> results = globalResults[key];
                        Element2DResult result = results[index];
                        ReadOnlyCollection<Tensor3> values = result.Stress;
                        ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                        xyzRes.AsParallel().AsOrdered();
                        ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                        xxyyzzRes.AsParallel().AsOrdered();

                        // loop through the results
                        Parallel.For(0, values.Count, i =>
                        {
                            // add the values to the vector lists
                            xyzRes.TryAdd(i, GetQuantityResult(values[i], stressUnit));
                            xxyyzzRes.TryAdd(i, GetQuantityResult(values[i], stressUnit, true));
                        });
                        xyzRes.TryAdd(values.Count, GetQuantityResult(values[0], stressUnit)); // add centre point last
                        xxyyzzRes.TryAdd(values.Count, GetQuantityResult(values[0], stressUnit, true)); // add centre point last
                        // add the vector list to the out tree
                        r.xyzResults.TryAdd(key, xyzRes);
                        r.xxyyzzResults.TryAdd(key, xxyyzzRes);
                    });
                    r.UpdateMinMax();
                    rs.TryAdd(index + 1, r); // permutation ID = index + 1
                });
            }

            return rs;
        }

        /// <summary>
        /// Returns shear result values
        /// </summary>
        /// <param name="globalResults"></param>
        /// <param name="type"></param>
        /// <param name="forceUnit"></param>
        /// <returns></returns>
        internal static ConcurrentDictionary<int, GsaResultsValues> GetElement2DResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<Element2DResult>> globalResults,
            ForcePerLengthUnit forceUnit, int permutation)
        {
            ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int, GsaResultsValues>();

            if (permutation > 0)
            {
                GsaResultsValues r = new GsaResultsValues();
                r.Type = GsaResultsValues.ResultType.Shear;

                Parallel.ForEach(globalResults.Keys, key =>
                {
                    // lists for results
                    ReadOnlyCollection<Element2DResult> results = globalResults[key];
                    Element2DResult result = results[permutation - 1];
                    ReadOnlyCollection<Vector2> values = result.Shear;
                    ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                    xyzRes.AsParallel().AsOrdered();

                    // loop through the results
                    Parallel.For(1, values.Count, i =>
                    {
                        // add the values to the vector lists
                        xyzRes.TryAdd(i, GetQuantityResult(values[i], forceUnit));
                    });
                    xyzRes.TryAdd(values.Count, GetQuantityResult(values[0], forceUnit)); // add centre point last
                    // add the vector list to the out tree
                    r.xyzResults.TryAdd(key, xyzRes);
                });
                r.UpdateMinMax();
                rs.TryAdd(permutation, r);
            }
            else
            {
                int permutationCount = globalResults[globalResults.Keys.First()].Count;

                Parallel.For(0, permutationCount, index => // loop through permutations
                {
                    GsaResultsValues r = new GsaResultsValues();
                    r.Type = GsaResultsValues.ResultType.Shear;

                    Parallel.ForEach(globalResults.Keys, key =>
                    {
                        // lists for results
                        ReadOnlyCollection<Element2DResult> results = globalResults[key];
                        Element2DResult result = results[index];
                        ReadOnlyCollection<Vector2> values = result.Shear;
                        ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                        xyzRes.AsParallel().AsOrdered();

                        // loop through the results
                        Parallel.For(1, values.Count, i =>
                        {
                            // add the values to the vector lists
                            xyzRes.TryAdd(i, GetQuantityResult(values[i], forceUnit));
                        });
                        xyzRes.TryAdd(values.Count, GetQuantityResult(values[0], forceUnit)); // add centre point last

                        // add the vector list to the out tree
                        r.xyzResults.TryAdd(key, xyzRes);
                    });
                    r.UpdateMinMax();
                    rs.TryAdd(index + 1, r); // permutation ID = index + 1
                });
            }

            return rs;
        }

        /// <summary>
        /// Returns force & moment result values
        /// </summary>
        /// <param name="globalResults"></param>
        /// <param name="forceUnit"></param>
        /// <param name="momentUnit"></param>
        /// <returns></returns>
        internal static ConcurrentDictionary<int, GsaResultsValues> GetElement2DResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<Element2DResult>> globalResults,
            ForcePerLengthUnit forceUnit, ForceUnit momentUnit, int permutation)
        {
            ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int, GsaResultsValues>();

            if (permutation > 0)
            {
                GsaResultsValues r = new GsaResultsValues();
                r.Type = GsaResultsValues.ResultType.Force;

                Parallel.ForEach(globalResults.Keys, key =>
                {
                    // lists for results
                    ReadOnlyCollection<Element2DResult> results = globalResults[key];
                    Element2DResult result = results[permutation - 1];
                    ReadOnlyCollection<Tensor2> values = result.Force;
                    ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                    xyzRes.AsParallel().AsOrdered();
                    ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                    xxyyzzRes.AsParallel().AsOrdered();

                    // loop through the results
                    Parallel.For(1, values.Count, i =>
                    {
                        // add the values to the vector lists
                        xyzRes.TryAdd(i, GetQuantityResult(values[i], forceUnit));
                        xxyyzzRes.TryAdd(i, GetQuantityResult(values[i], momentUnit));
                    });
                    xyzRes.TryAdd(values.Count, GetQuantityResult(values[0], forceUnit)); // add centre point last
                    xxyyzzRes.TryAdd(values.Count, GetQuantityResult(values[0], momentUnit)); // add centre point last
                    // add the vector list to the out tree
                    r.xyzResults.TryAdd(key, xyzRes);
                    r.xxyyzzResults.TryAdd(key, xxyyzzRes);
                });
                r.UpdateMinMax();
                rs.TryAdd(permutation, r);
            }
            else
            {
                int permutationCount = globalResults[globalResults.Keys.First()].Count;

                Parallel.For(0, permutationCount, index => // loop through permutations
                {
                    GsaResultsValues r = new GsaResultsValues();
                    r.Type = GsaResultsValues.ResultType.Force;

                    Parallel.ForEach(globalResults.Keys, key =>
                    {
                        // lists for results
                        ReadOnlyCollection<Element2DResult> results = globalResults[key];
                        Element2DResult result = results[index];
                        ReadOnlyCollection<Tensor2> values = result.Force;
                        ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                        xyzRes.AsParallel().AsOrdered();
                        ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                        xxyyzzRes.AsParallel().AsOrdered();

                        // loop through the results
                        Parallel.For(1, values.Count, i =>
                        {
                            // add the values to the vector lists
                            xyzRes.TryAdd(i, GetQuantityResult(values[i], forceUnit));
                            xxyyzzRes.TryAdd(i, GetQuantityResult(values[i], momentUnit));
                        });
                        xyzRes.TryAdd(values.Count, GetQuantityResult(values[0], forceUnit)); // add centre point last
                        xxyyzzRes.TryAdd(values.Count, GetQuantityResult(values[0], momentUnit)); // add centre point last
                        // add the vector list to the out tree
                        r.xyzResults.TryAdd(key, xyzRes);
                        r.xxyyzzResults.TryAdd(key, xxyyzzRes);
                    });
                    r.UpdateMinMax();
                    rs.TryAdd(index + 1, r); // permutation ID = index + 1
                });
            }

            return rs;
        }

        /// <summary>
        /// Returns displacement result values
        /// </summary>
        /// <param name="globalResults"></param>
        /// <param name="resultLengthUnit"></param>
        /// <returns></returns>
        internal static ConcurrentDictionary<int, GsaResultsValues> GetElement2DResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<Element2DResult>> globalResults,
            LengthUnit resultLengthUnit, int permutation)
        {
            ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int, GsaResultsValues>();

            if (permutation > 0)
            {
                GsaResultsValues r = new GsaResultsValues();
                r.Type = GsaResultsValues.ResultType.Displacement;

                Parallel.ForEach(globalResults.Keys, key =>
                {
                    // lists for results
                    ReadOnlyCollection<Element2DResult> results = globalResults[key];
                    Element2DResult result = results[permutation - 1];
                    List<Double6> values = result.Displacement.ToList();
                    ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                    xyzRes.AsParallel().AsOrdered();
                    ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                    xxyyzzRes.AsParallel().AsOrdered();

                    // loop through the results
                    Parallel.For(1, values.Count, i =>
                    {
                        // add the values to the vector lists
                        xyzRes.TryAdd(i, GetQuantityResult(values[i], resultLengthUnit));
                        xxyyzzRes.TryAdd(i, GetQuantityResult(values[i], AngleUnit.Radian));
                    });
                    xyzRes.TryAdd(values.Count, GetQuantityResult(values[0], resultLengthUnit)); // add centre point last
                    xxyyzzRes.TryAdd(values.Count, GetQuantityResult(values[0], AngleUnit.Radian)); // add centre point last
                    // add the vector list to the out tree
                    r.xyzResults.TryAdd(key, xyzRes);
                    r.xxyyzzResults.TryAdd(key, xxyyzzRes);
                });
                r.UpdateMinMax();
                rs.TryAdd(permutation, r);
            }
            else
            {
                int permutationCount = globalResults[globalResults.Keys.First()].Count;

                Parallel.For(0, permutationCount, index => // loop through permutations
                {
                    GsaResultsValues r = new GsaResultsValues();
                    r.Type = GsaResultsValues.ResultType.Displacement;

                    Parallel.ForEach(globalResults.Keys, key =>
                    {
                        // lists for results
                        ReadOnlyCollection<Element2DResult> results = globalResults[key];
                        Element2DResult result = results[index];
                        List<Double6> values = result.Displacement.ToList();
                        ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                        xyzRes.AsParallel().AsOrdered();
                        ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                        xxyyzzRes.AsParallel().AsOrdered();

                        // loop through the results
                        Parallel.For(1, values.Count, i =>
                        {
                            // add the values to the vector lists
                            xyzRes.TryAdd(i, GetQuantityResult(values[i], resultLengthUnit));
                            xxyyzzRes.TryAdd(i, GetQuantityResult(values[i], AngleUnit.Radian));
                        });
                        xyzRes.TryAdd(values.Count, GetQuantityResult(values[0], resultLengthUnit)); // add centre point last
                        xxyyzzRes.TryAdd(values.Count, GetQuantityResult(values[0], AngleUnit.Radian)); // add centre point last
                        // add the vector list to the out tree
                        r.xyzResults.TryAdd(key, xyzRes);
                        r.xxyyzzResults.TryAdd(key, xxyyzzRes);
                    });
                    r.UpdateMinMax();
                    rs.TryAdd(index + 1, r); // permutation ID = index + 1
                });
            }

            return rs;
        }

        /// <summary>
        /// Returns forces result values
        /// </summary>
        /// <param name="globalResults"></param>
        /// <param name="forceUnit"></param>
        /// <param name="momentUnit"></param>
        /// /// <param name="permutation"></param>
        /// <returns></returns>
        internal static ConcurrentDictionary<int, GsaResultsValues> GetElement1DResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<Element1DResult>> globalResults,
            ForceUnit forceUnit, MomentUnit momentUnit, int permutation)
        {
            ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int, GsaResultsValues>();

            if (permutation > 0)
            {
                GsaResultsValues r = new GsaResultsValues();
                r.Type = GsaResultsValues.ResultType.Force;

                Parallel.ForEach(globalResults.Keys, key =>
                {
                    // lists for results
                    ReadOnlyCollection<Element1DResult> results = globalResults[key];
                    Element1DResult result = results[permutation - 1];
                    List<Double6> values = result.Force.ToList();
                    ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                    xyzRes.AsParallel().AsOrdered();
                    ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                    xxyyzzRes.AsParallel().AsOrdered();

                    // loop through the results
                    Parallel.For(0, values.Count, i =>
                    {
                        // add the values to the vector lists
                        xyzRes.TryAdd(i, GetQuantityResult(values[i], forceUnit, true));
                        xxyyzzRes.TryAdd(i, GetQuantityResult(values[i], momentUnit, true));
                    });
                    // add the vector list to the out tree
                    r.xyzResults.TryAdd(key, xyzRes);
                    r.xxyyzzResults.TryAdd(key, xxyyzzRes);
                });
                r.UpdateMinMax();
                rs.TryAdd(permutation, r);
            }
            else
            {
                int permutationCount = globalResults[globalResults.Keys.First()].Count;

                Parallel.For(0, permutationCount, index => // loop through permutations
                {
                    GsaResultsValues r = new GsaResultsValues();
                    r.Type = GsaResultsValues.ResultType.Force;

                    Parallel.ForEach(globalResults.Keys, key =>
                    {
                        // lists for results
                        ReadOnlyCollection<Element1DResult> results = globalResults[key];
                        Element1DResult result = results[index];
                        List<Double6> values = result.Force.ToList();
                        ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                        xyzRes.AsParallel().AsOrdered();
                        ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                        xxyyzzRes.AsParallel().AsOrdered();

                        // loop through the results
                        Parallel.For(0, values.Count, i =>
                        {
                            // add the values to the vector lists
                            xyzRes.TryAdd(i, GetQuantityResult(values[i], forceUnit, true));
                            xxyyzzRes.TryAdd(i, GetQuantityResult(values[i], momentUnit, true));
                        });
                        // add the vector list to the out tree
                        r.xyzResults.TryAdd(key, xyzRes);
                        r.xxyyzzResults.TryAdd(key, xxyyzzRes);
                    });
                    r.UpdateMinMax();
                    rs.TryAdd(index + 1, r); // permutation ID = index + 1
                });
            }

            return rs;
        }

        /// <summary>
        /// Returns displacement result values
        /// </summary>
        /// <param name="globalResults"></param>
        /// <param name="resultLengthUnit"></param>
        /// <returns></returns>
        internal static ConcurrentDictionary<int, GsaResultsValues> GetElement1DResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<Element1DResult>> globalResults,
            LengthUnit resultLengthUnit, int permutation)
        {
            ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int, GsaResultsValues>();

            if (permutation > 0)
            {
                GsaResultsValues r = new GsaResultsValues();
                r.Type = GsaResultsValues.ResultType.Displacement;

                Parallel.ForEach(globalResults.Keys, key =>
                {
                    // lists for results
                    ReadOnlyCollection<Element1DResult> results = globalResults[key];
                    Element1DResult result = results[permutation - 1];
                    List<Double6> values = result.Displacement.ToList();
                    ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                    xyzRes.AsParallel().AsOrdered();
                    ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                    xxyyzzRes.AsParallel().AsOrdered();

                    // loop through the results
                    Parallel.For(0, values.Count, i =>
                    {
                        // add the values to the vector lists
                        xyzRes.TryAdd(i, GetQuantityResult(values[i], resultLengthUnit));
                        xxyyzzRes.TryAdd(i, GetQuantityResult(values[i], AngleUnit.Radian));
                    });
                    // add the vector list to the out tree
                    r.xyzResults.TryAdd(key, xyzRes);
                    r.xxyyzzResults.TryAdd(key, xxyyzzRes);
                });
                r.UpdateMinMax();
                rs.TryAdd(permutation, r);
            }
            else
            {
                int permutationCount = globalResults[globalResults.Keys.First()].Count;
                
                Parallel.For(0, permutationCount, index => // loop through permutations
                {
                    GsaResultsValues r = new GsaResultsValues();
                    r.Type = GsaResultsValues.ResultType.Displacement;

                    Parallel.ForEach(globalResults.Keys, key =>
                    {
                        // lists for results
                        ReadOnlyCollection<Element1DResult> results = globalResults[key];
                        Element1DResult result = results[index];
                        List<Double6> values = result.Displacement.ToList();
                        ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                        xyzRes.AsParallel().AsOrdered();
                        ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                        xxyyzzRes.AsParallel().AsOrdered();

                        // loop through the results
                        Parallel.For(0, values.Count, i =>
                        {
                            // add the values to the vector lists
                            xyzRes.TryAdd(i, GetQuantityResult(values[i], resultLengthUnit));
                            xxyyzzRes.TryAdd(i, GetQuantityResult(values[i], AngleUnit.Radian));
                        });
                        // add the vector list to the out tree
                        r.xyzResults.TryAdd(key, xyzRes);
                        r.xxyyzzResults.TryAdd(key, xxyyzzRes);
                    });
                    r.UpdateMinMax();
                    rs.TryAdd(index + 1, r); // permutation ID = index + 1
                });
            }

            return rs;
        }
        /// <summary>
        /// Returns displacement result values
        /// </summary>
        /// <param name="globalResults"></param>
        /// <param name="resultLengthUnit"></param>
        /// <returns></returns>
        internal static ConcurrentDictionary<int, GsaResultsValues> GetNodeResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> globalResults,
            LengthUnit resultLengthUnit, int permutation)
        {
            ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int,GsaResultsValues>();

            if (permutation > 0)
            {
                GsaResultsValues r = new GsaResultsValues();
                r.Type = GsaResultsValues.ResultType.Displacement;

                Parallel.ForEach(globalResults.Keys, nodeID =>
                {
                    ReadOnlyCollection<NodeResult> results = globalResults[nodeID];
                    NodeResult result = results[permutation - 1];
                    Double6 values = result.Displacement;
                    
                    ConcurrentDictionary<int, GsaResultQuantity> xyz = new ConcurrentDictionary<int, GsaResultQuantity>();
                    xyz.TryAdd(0, GetQuantityResult(values, resultLengthUnit));
                    r.xyzResults.TryAdd(nodeID, xyz);
                    ConcurrentDictionary<int, GsaResultQuantity> xxyyzz = new ConcurrentDictionary<int, GsaResultQuantity>();
                    xxyyzz.TryAdd(0, GetQuantityResult(values, AngleUnit.Radian));
                    r.xxyyzzResults.TryAdd(nodeID, xxyyzz);

                });

                r.UpdateMinMax();
                rs.TryAdd(permutation, r);
            }
            else
            {
                int permutationCount = globalResults[globalResults.Keys.First()].Count;
                Parallel.For(0, permutationCount, index =>
                {
                    GsaResultsValues r = new GsaResultsValues();
                    r.Type = GsaResultsValues.ResultType.Displacement;

                    Parallel.ForEach(globalResults.Keys, nodeID =>
                    {
                        ReadOnlyCollection<NodeResult> results = globalResults[nodeID];
                        NodeResult result = results[index];
                        Double6 values = result.Displacement;

                        ConcurrentDictionary<int, GsaResultQuantity> xyz = new ConcurrentDictionary<int, GsaResultQuantity>();
                        xyz.TryAdd(0, GetQuantityResult(values, resultLengthUnit));
                        r.xyzResults.TryAdd(nodeID, xyz);
                        ConcurrentDictionary<int, GsaResultQuantity> xxyyzz = new ConcurrentDictionary<int, GsaResultQuantity>();
                        xxyyzz.TryAdd(0, GetQuantityResult(values, AngleUnit.Radian));
                        r.xxyyzzResults.TryAdd(nodeID, xxyyzz);
                    });

                    r.UpdateMinMax();
                    rs.TryAdd(index + 1, r); // permutation ID = index + 1
                });
            }

            return rs;
        }
        /// <summary>
        /// Returns reaction forces result values
        /// </summary>
        /// <param name="globalResults"></param>
        /// <param name="forceUnit"></param>
        /// <param name="momentUnit"></param>
        /// <returns></returns>
        internal static ConcurrentDictionary<int, GsaResultsValues> GetNodeResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> globalResults,
            ForceUnit forceUnit, MomentUnit momentUnit, int permutation)
        {
            ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int, GsaResultsValues>();

            if (permutation > 0)
            {
                GsaResultsValues r = new GsaResultsValues();
                r.Type = GsaResultsValues.ResultType.Force;

                Parallel.ForEach(globalResults.Keys, nodeID =>
                {
                    ReadOnlyCollection<NodeResult> results = globalResults[nodeID];
                    NodeResult result = results[permutation - 1];
                    Double6 values = result.Reaction;

                    ConcurrentDictionary<int, GsaResultQuantity> xyz = new ConcurrentDictionary<int, GsaResultQuantity>();
                    xyz.TryAdd(0, GetQuantityResult(values, forceUnit));
                    r.xyzResults.TryAdd(nodeID, xyz);
                    ConcurrentDictionary<int, GsaResultQuantity> xxyyzz = new ConcurrentDictionary<int, GsaResultQuantity>();
                    xxyyzz.TryAdd(0, GetQuantityResult(values, momentUnit));
                    r.xxyyzzResults.TryAdd(nodeID, xxyyzz);

                });

                r.UpdateMinMax();
                rs.TryAdd(permutation, r);
            }
            else
            {
                int permutationCount = globalResults[globalResults.Keys.First()].Count;
                Parallel.For(0, permutationCount, index =>
                {
                    GsaResultsValues r = new GsaResultsValues();
                    r.Type = GsaResultsValues.ResultType.Force;

                    Parallel.ForEach(globalResults.Keys, nodeID =>
                    {
                        ReadOnlyCollection<NodeResult> results = globalResults[nodeID];
                        NodeResult result = results[index];
                        Double6 values = result.Reaction;

                        ConcurrentDictionary<int, GsaResultQuantity> xyz = new ConcurrentDictionary<int, GsaResultQuantity>();
                        xyz.TryAdd(0, GetQuantityResult(values, forceUnit));
                        r.xyzResults.TryAdd(nodeID, xyz);
                        ConcurrentDictionary<int, GsaResultQuantity> xxyyzz = new ConcurrentDictionary<int, GsaResultQuantity>();
                        xxyyzz.TryAdd(0, GetQuantityResult(values, momentUnit));
                        r.xxyyzzResults.TryAdd(nodeID, xxyyzz);
                    });

                    r.UpdateMinMax();
                    rs.TryAdd(index + 1, r); // permutation ID = index + 1
                });
            }

            return rs;
        }
        #endregion
    }
}
