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
        #region Vector3d results
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
        #endregion
        #region Quantity results
        public static GsaResultQuantity GetQuantityResult(Double6 result, ForceUnit unit)
        {
            IQuantity x = new Force(new Force(result.X, ForceUnit.Newton).As(unit), unit);
            IQuantity y = new Force(new Force(result.Y, ForceUnit.Newton).As(unit), unit);
            IQuantity z = new Force(new Force(result.Z, ForceUnit.Newton).As(unit), unit);
            double pyth = Math.Sqrt(Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2));
            IQuantity xyz = new Force(new Force(pyth, ForceUnit.Newton).As(unit), unit);
            return new GsaResultQuantity() { X = x, Y = y, Z = z, XYZ = xyz};
        }
        public static GsaResultQuantity GetQuantityResult(Double6 result, MomentUnit unit)
        {
            IQuantity xx = new Moment(new Moment(result.XX, MomentUnit.NewtonMeter).As(unit), unit);
            IQuantity yy = new Moment(new Moment(result.YY, MomentUnit.NewtonMeter).As(unit), unit);
            IQuantity zz = new Moment(new Moment(result.ZZ, MomentUnit.NewtonMeter).As(unit), unit);
            double pyth = Math.Sqrt(Math.Pow(result.XX, 2) + Math.Pow(result.YY, 2) + Math.Pow(result.ZZ, 2));
            IQuantity xxyyzz = new Moment(new Moment(pyth, MomentUnit.NewtonMeter).As(unit), unit);
            return new GsaResultQuantity() { X = xx, Y = yy, Z = zz, XYZ = xxyyzz };
        }
        public static GsaResultQuantity GetQuantityResult(Double6 result, LengthUnit unit)
        {
            IQuantity x = new Length(new Length(result.X, LengthUnit.Meter).As(unit), unit);
            IQuantity y = new Length(new Length(result.Y, LengthUnit.Meter).As(unit), unit);
            IQuantity z = new Length(new Length(result.Z, LengthUnit.Meter).As(unit), unit);
            double pyth = Math.Sqrt(Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2));
            IQuantity xyz = new Length(new Length(pyth, LengthUnit.Meter).As(unit), unit);
            return new GsaResultQuantity() { X = x, Y = y, Z = z, XYZ = xyz };
        }
        public static GsaResultQuantity GetQuantityResult(Double3 result, LengthUnit unit)
        {
            IQuantity x = new Length(new Length(result.X, LengthUnit.Meter).As(unit), unit);
            IQuantity y = new Length(new Length(result.Y, LengthUnit.Meter).As(unit), unit);
            IQuantity z = new Length(new Length(result.Z, LengthUnit.Meter).As(unit), unit);
            double pyth = Math.Sqrt(Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2));
            IQuantity xyz = new Length(new Length(pyth, LengthUnit.Meter).As(unit), unit);
            return new GsaResultQuantity() { X = x, Y = y, Z = z, XYZ = xyz };
        }
        public static GsaResultQuantity GetQuantityResult(Double6 result, AngleUnit unit)
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
        public static GsaResultQuantity GetQuantityResult(Double6 result, PressureUnit unit, bool shear = false)
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
        public static GsaResultQuantity GetQuantityResult(Tensor2 result, ForceUnit unit)
        {
            IQuantity x = new Force(new Force(result.XX, ForceUnit.Newton).As(unit), unit);
            IQuantity y = new Force(new Force(result.YY, ForceUnit.Newton).As(unit), unit);
            IQuantity z = new Force(new Force(result.XY, ForceUnit.Newton).As(unit), unit);
            return new GsaResultQuantity() { X = x, Y = y, Z = z, XYZ = null };
        }
        public static GsaResultQuantity GetQuantityResult(Tensor2 result, MomentUnit unit)
        {
            IQuantity xx = new Moment(new Moment(result.XX, MomentUnit.NewtonMeter).As(unit), unit);
            IQuantity yy = new Moment(new Moment(result.YY, MomentUnit.NewtonMeter).As(unit), unit);
            IQuantity zz = new Moment(new Moment(result.XY, MomentUnit.NewtonMeter).As(unit), unit);
            return new GsaResultQuantity() { X = xx, Y = yy, Z = zz, XYZ = null };
        }
        public static GsaResultQuantity GetQuantityResult(Vector2 result, ForceUnit unit)
        {
            IQuantity x = new Force(new Force(result.X, ForceUnit.Newton).As(unit), unit);
            IQuantity y = new Force(new Force(result.Y, ForceUnit.Newton).As(unit), unit);
            return new GsaResultQuantity() { X = x, Y = y, Z = null, XYZ = null };
        }
        public static GsaResultQuantity GetQuantityResult(Tensor3 result, PressureUnit unit, bool shear = false)
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
        public static GsaResultQuantity GetQuantityResult(Vector3 result, MassUnit unit)
        {
            IQuantity x = new Mass(new Mass(result.X, MassUnit.Kilogram).As(unit), unit);
            IQuantity y = new Mass(new Mass(result.Y, MassUnit.Kilogram).As(unit), unit);
            IQuantity z = new Mass(new Mass(result.Z, MassUnit.Kilogram).As(unit), unit);
            double pyth = Math.Sqrt(Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2));
            IQuantity xyz = new Mass(new Mass(pyth, MassUnit.Kilogram).As(unit), unit);
            return new GsaResultQuantity() { X = x, Y = y, Z = z, XYZ = xyz };
        }
        public static GsaResultQuantity GetQuantityResult(Vector3 result, AreaMomentOfInertiaUnit unit)
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
        public static GsaResultsValues GetElement3DResultValues(ReadOnlyDictionary<int, Element3DResult> globalResults, 
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
                ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                xxyyzzRes.AsParallel().AsOrdered();

                List<Double3> trans_vals = elementResults.Displacement.ToList();
                Parallel.For(0, trans_vals.Count, i => //foreach (Double3 val in trans_vals)
                {
                    xyzRes[i] = GetQuantityResult(trans_vals[i], resultLengthUnit);

                });

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
        public static GsaResultsValues GetElement3DResultValues(ReadOnlyDictionary<int, Element3DResult> globalResults,
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
                Parallel.For(0, stress_vals.Count * 2, i => // (Tensor3 val in stress_vals)
                {
                    // split computation into two parts by doubling the i-counter
                    if (i < stress_vals.Count)
                        xyzRes[i] = GetQuantityResult(stress_vals[i], stressUnit);
                    else
                        xxyyzzRes[i - stress_vals.Count] = GetQuantityResult(stress_vals[i - stress_vals.Count], stressUnit);
                });

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
        public static GsaResultsValues GetElement2DResultValues(ReadOnlyDictionary<int, Element2DResult> globalResults,
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
                Parallel.For(0, stresses.Count * 2, i => // (Tensor3 stress in stresses)
                {
                    // split computation into two parts by doubling the i-counter
                    if (i < stresses.Count)
                        xyzRes[i] = GetQuantityResult(stresses[i], stressUnit, false);
                    else
                        xxyyzzRes[i - stresses.Count] = GetQuantityResult(stresses[i - stresses.Count], stressUnit, true);
                });

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
        /// <param name="type"></param>
        /// <param name="forceUnit"></param>
        /// <returns></returns>
        public static GsaResultsValues GetElement2DResultValues(ReadOnlyDictionary<int, Element2DResult> globalResults,
            ForceUnit forceUnit)
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
                Parallel.For(0, shears.Count, i => // (Vector2 shear in shears)
                {
                    xyzRes[i] = GetQuantityResult(shears[i], forceUnit);
                });

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
        public static GsaResultsValues GetElement2DResultValues(ReadOnlyDictionary<int, Element2DResult> globalResults, 
            ForceUnit forceUnit, MomentUnit momentUnit)
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
                Parallel.For(0, forces.Count + moments.Count, i => // (Tensor2 force in forces)
                {
                    // combine forces and momemts (list lengths must be the same) to run
                    // calculations in parallel by doubling the i-counter
                    if (i < forces.Count)
                        xyzRes[i] = GetQuantityResult(forces[i], forceUnit);
                    else
                        xxyyzzRes[i - forces.Count] = GetQuantityResult(moments[i - forces.Count], momentUnit);
                });

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
        public static GsaResultsValues GetElement2DResultValues(ReadOnlyDictionary<int, Element2DResult> globalResults,
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
                Parallel.For(0, disp.Count * 2, i => // (Double6 val in values)
                {
                    // split computation into two parts by doubling the i-counter
                    if (i < disp.Count)
                        xyzRes[i] = GetQuantityResult(disp[i], resultLengthUnit);
                    else
                        xyzRes[i - disp.Count] = GetQuantityResult(disp[i - disp.Count], AngleUnit.Radian);
                });

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
        public static GsaResultsValues GetElement1DResultValues(ReadOnlyDictionary<int, Element1DResult> globalResults,
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
                    xyzRes[i] = GetQuantityResult(result, forceUnit);
                    xxyyzzRes[i] = GetQuantityResult(result, momentUnit);
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
        public static GsaResultsValues GetElement1DResultValues(ReadOnlyDictionary<int, Element1DResult> globalResults, 
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
        public static GsaResultsValues GetNodeResultValues(ReadOnlyDictionary<int, NodeResult> globalResults,
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
        public static GsaResultsValues GetNodeResultValues(ReadOnlyDictionary<int, NodeResult> globalResults,
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
        //public static ConcurrentDictionary<int, GsaResultsValues> Get3DResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<Element3DResult>> globalResults,
        //    LengthUnit resultLengthUnit, int permutation)
        //{
        //    ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int, GsaResultsValues>();

        //    if (permutation > 0)
        //    {
        //        GsaResultsValues r = new GsaResultsValues();

        //        Parallel.ForEach(globalResults.Keys, key =>
        //        {
        //            // lists for results
        //            ReadOnlyCollection<Element3DResult> results = globalResults[key];
        //            Element3DResult elementResults = results[permutation];
        //            ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        //            xyzRes.AsParallel().AsOrdered();
        //            ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        //            xxyyzzRes.AsParallel().AsOrdered();

        //            List<Double3> trans_vals = elementResults.Displacement.ToList();
        //            Parallel.For(0, trans_vals.Count, i => //foreach (Double3 val in trans_vals)
        //            {
        //                xyzRes[i] = GetQuantityResult(trans_vals[i], resultLengthUnit);

        //            });

        //            // add vector lists to main lists
        //            r.xyzResults[key] = xyzRes;
        //            r.xxyyzzResults[key] = xxyyzzRes;
        //        });

        //        r.UpdateMinMax();
        //        rs.TryAdd(permutation, r);
        //    }
        //    else
        //    {
        //        int permutationCount = globalResults[globalResults.Keys.First()].Count;
        //        for (int i = 0; i < permutationCount; i++)
        //        {
        //            GsaResultsValues r = new GsaResultsValues();

        //            Parallel.ForEach(globalResults.Keys, key =>
        //            {
        //                // lists for results
        //                ReadOnlyCollection<Element3DResult> results = globalResults[key];
        //                Element3DResult elementResults = results[i];
        //                ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        //                xyzRes.AsParallel().AsOrdered();
        //                ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        //                xxyyzzRes.AsParallel().AsOrdered();

        //                List<Double3> trans_vals = elementResults.Displacement.ToList();
        //                Parallel.For(0, trans_vals.Count, j => //foreach (Double3 val in trans_vals)
        //                {
        //                    xyzRes[j] = GetQuantityResult(trans_vals[j], resultLengthUnit);

        //                });

        //                // add vector lists to main lists
        //                r.xyzResults[key] = xyzRes;
        //                r.xxyyzzResults[key] = xxyyzzRes;
        //            });

        //            r.UpdateMinMax();
        //            rs.TryAdd(permutation, r);
        //        }
        //    }


        //    return rs;
        //}
        /// <summary>
        /// Returns stress result values
        /// </summary>
        /// <param name="globalResults"></param>
        /// <param name="stressUnit"></param>
        /// <returns></returns>
        //public static GsaResultsValues Get3DResultValues(ReadOnlyDictionary<int, Element3DResult> globalResults,
        //    PressureUnit stressUnit)
        //{
        //    GsaResultsValues r = new GsaResultsValues();
        //    r.Type = GsaResultsValues.ResultType.Stress;

        //    Parallel.ForEach(globalResults.Keys, key =>
        //    {
        //        // lists for results
        //        Element3DResult elementResults = globalResults[key];
        //        ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        //        xyzRes.AsParallel().AsOrdered();
        //        ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        //        xxyyzzRes.AsParallel().AsOrdered();

        //        List<Tensor3> stress_vals = elementResults.Stress.ToList();
        //        Parallel.For(0, stress_vals.Count * 2, i => // (Tensor3 val in stress_vals)
        //        {
        //            // split computation into two parts by doubling the i-counter
        //            if (i < stress_vals.Count)
        //            {
        //                xyzRes[i] = GetQuantityResult(stress_vals[i], stressUnit);
        //            }
        //            else
        //            {
        //                xxyyzzRes[i - stress_vals.Count] = GetQuantityResult(stress_vals[i - stress_vals.Count], stressUnit);
        //            }
        //        });

        //        // add vector lists to main lists
        //        r.xyzResults[key] = xyzRes;
        //        r.xxyyzzResults[key] = xxyyzzRes;
        //    });

        //    r.UpdateMinMax();

        //    return r;
        //}
        ///// <summary>
        ///// Returns stress result values
        ///// </summary>
        ///// <param name="globalResults"></param>
        ///// <param name="stressUnit"></param>
        ///// <returns></returns>
        //public static GsaResultsValues Get2DResultValues(ReadOnlyDictionary<int, Element2DResult> globalResults,
        //    PressureUnit stressUnit)
        //{
        //    GsaResultsValues r = new GsaResultsValues();
        //    r.Type = GsaResultsValues.ResultType.Stress;

        //    Parallel.ForEach(globalResults.Keys, key =>
        //    {
        //        // lists for results
        //        Element2DResult elementResults = globalResults[key];
        //        ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        //        xyzRes.AsParallel().AsOrdered();
        //        ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        //        xxyyzzRes.AsParallel().AsOrdered();

        //        List<Tensor3> stresses = elementResults.Stress.ToList();
        //        Parallel.For(0, stresses.Count * 2, i => // (Tensor3 stress in stresses)
        //        {
        //            // split computation into two parts by doubling the i-counter
        //            if (i < stresses.Count)
        //            {
        //                xyzRes[i] = GetQuantityResult(stresses[i], stressUnit, false);
        //            }
        //            else
        //            {
        //                xxyyzzRes[i - stresses.Count] = GetQuantityResult(stresses[i - stresses.Count], stressUnit, true);
        //            }
        //        });

        //        // add vector lists to main lists
        //        r.xyzResults[key] = xyzRes;
        //        r.xxyyzzResults[key] = xxyyzzRes;
        //    });

        //    r.UpdateMinMax();

        //    return r;
        //}
        ///// <summary>
        ///// Returns shear result values
        ///// </summary>
        ///// <param name="globalResults"></param>
        ///// <param name="type"></param>
        ///// <param name="forceUnit"></param>
        ///// <returns></returns>
        //public static GsaResultsValues Get2DResultValues(ReadOnlyDictionary<int, Element2DResult> globalResults, GsaResultsValues.ResultType type,
        //    ForceUnit forceUnit)
        //{
        //    GsaResultsValues r = new GsaResultsValues();
        //    r.Type = GsaResultsValues.ResultType.Shear;

        //    Parallel.ForEach(globalResults.Keys, key =>
        //    {
        //        // lists for results
        //        Element2DResult elementResults = globalResults[key];
        //        ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        //        xyzRes.AsParallel().AsOrdered();
        //        ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        //        xxyyzzRes.AsParallel().AsOrdered();

        //        List<Vector2> shears = elementResults.Shear.ToList();
        //        Parallel.For(0, shears.Count, i => // (Vector2 shear in shears)
        //        {
        //            xyzRes[i] = GetQuantityResult(shears[i], forceUnit);
        //        });

        //        // add vector lists to main lists
        //        r.xyzResults[key] = xyzRes;
        //        r.xxyyzzResults[key] = xxyyzzRes;
        //    });

        //    r.UpdateMinMax();

        //    return r;
        //}
        ///// <summary>
        ///// Returns force & moment result values
        ///// </summary>
        ///// <param name="globalResults"></param>
        ///// <param name="forceUnit"></param>
        ///// <param name="momentUnit"></param>
        ///// <returns></returns>
        //public static GsaResultsValues Get2DResultValues(ReadOnlyDictionary<int, Element2DResult> globalResults,
        //    ForceUnit forceUnit, MomentUnit momentUnit)
        //{
        //    GsaResultsValues r = new GsaResultsValues();
        //    r.Type = GsaResultsValues.ResultType.Force;

        //    Parallel.ForEach(globalResults.Keys, key =>
        //    {
        //        // lists for results
        //        Element2DResult elementResults = globalResults[key];
        //        ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        //        xyzRes.AsParallel().AsOrdered();
        //        ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        //        xxyyzzRes.AsParallel().AsOrdered();

        //        List<Tensor2> forces = elementResults.Force.ToList();
        //        List<Tensor2> moments = elementResults.Moment.ToList();
        //        Parallel.For(0, forces.Count + moments.Count, i => // (Tensor2 force in forces)
        //        {
        //            // combine forces and momemts (list lengths must be the same) to run
        //            // calculations in parallel by doubling the i-counter
        //            if (i < forces.Count)
        //            {
        //                xyzRes[i] = GetQuantityResult(forces[i], forceUnit);
        //            }
        //            else
        //            {
        //                xxyyzzRes[i - forces.Count] = GetQuantityResult(moments[i - forces.Count], momentUnit);
        //            }
        //        });

        //        // add vector lists to main lists
        //        r.xyzResults[key] = xyzRes;
        //        r.xxyyzzResults[key] = xxyyzzRes;
        //    });

        //    r.UpdateMinMax();

        //    return r;
        //}
        ///// <summary>
        ///// Returns displacement result values
        ///// </summary>
        ///// <param name="globalResults"></param>
        ///// <param name="resultLengthUnit"></param>
        ///// <returns></returns>
        //public static GsaResultsValues Get2DResultValues(ReadOnlyDictionary<int, Element2DResult> globalResults,
        //    LengthUnit resultLengthUnit)
        //{
        //    GsaResultsValues r = new GsaResultsValues();
        //    r.Type = GsaResultsValues.ResultType.Displacement;

        //    Parallel.ForEach(globalResults.Keys, key =>
        //    {
        //        // lists for results
        //        Element2DResult elementResults = globalResults[key];
        //        ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        //        xyzRes.AsParallel().AsOrdered();
        //        ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        //        xxyyzzRes.AsParallel().AsOrdered();

        //        List<Double6> disp = elementResults.Displacement.ToList();
        //        Parallel.For(0, disp.Count * 2, i => // (Double6 val in values)
        //        {
        //            // split computation into two parts by doubling the i-counter
        //            if (i < disp.Count)
        //            {
        //                xyzRes[i] = GetQuantityResult(disp[i], resultLengthUnit);
        //            }
        //            else
        //            {
        //                xyzRes[i - disp.Count] = GetQuantityResult(disp[i - disp.Count], AngleUnit.Radian);
        //            }
        //        });

        //        // add vector lists to main lists
        //        r.xyzResults[key] = xyzRes;
        //        r.xxyyzzResults[key] = xxyyzzRes;
        //    });

        //    r.UpdateMinMax();

        //    return r;
        //}
        ///// <summary>
        ///// Returns forces result values
        ///// </summary>
        ///// <param name="globalResults"></param>
        ///// <param name="forceUnit"></param>
        ///// <param name="momentUnit"></param>
        ///// <returns></returns>
        //public static GsaResultsValues Get1DResultValues(ReadOnlyDictionary<int, Element1DResult> globalResults,
        //    ForceUnit forceUnit, MomentUnit momentUnit)
        //{
        //    GsaResultsValues r = new GsaResultsValues();
        //    r.Type = GsaResultsValues.ResultType.Force;

        //    Parallel.ForEach(globalResults.Keys, key =>
        //    {
        //        // lists for results
        //        Element1DResult elementResults = globalResults[key];
        //        List<Double6> values = new List<Double6>();
        //        ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        //        xyzRes.AsParallel().AsOrdered();
        //        ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        //        xxyyzzRes.AsParallel().AsOrdered();

        //        values = elementResults.Force.ToList();

        //        // loop through the results
        //        Parallel.For(0, values.Count, i =>
        //        {
        //            Double6 result = values[i];

        //            // add the values to the vector lists
        //            xyzRes[i] = GetQuantityResult(result, forceUnit);
        //            xxyyzzRes[i] = GetQuantityResult(result, momentUnit);
        //        });
        //        // add the vector list to the out tree
        //        r.xyzResults[key] = xyzRes;
        //        r.xxyyzzResults[key] = xxyyzzRes;
        //    });

        //    r.UpdateMinMax();

        //    return r;
        //}
        
        /// <summary>
        /// Returns displacement result values
        /// </summary>
        /// <param name="globalResults"></param>
        /// <param name="resultLengthUnit"></param>
        /// <returns></returns>
        public static ConcurrentDictionary<int, GsaResultsValues> GetElement1DResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<Element1DResult>> globalResults,
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
        public static ConcurrentDictionary<int, GsaResultsValues> GetNodeResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> globalResults,
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
        public static ConcurrentDictionary<int, GsaResultsValues> GetNodeResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> globalResults,
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
