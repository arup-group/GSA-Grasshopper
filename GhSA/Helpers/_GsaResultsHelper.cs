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
            double pyth = Math.Sqrt(Math.Pow(result.XX, 2) + Math.Pow(result.YY, 2) + Math.Pow(result.ZZ, 2));
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

        public static GsaResultsValues Get3DResultValues(ReadOnlyDictionary<int, Element3DResult> globalResults, 
            GsaResultsValues.ResultType type, LengthUnit resultLengthUnit, PressureUnit stressUnit)
        {
            GsaResultsValues r = new GsaResultsValues();

            Parallel.ForEach(globalResults.Keys, key =>
            {
                // lists for results
                Element3DResult elementResults = globalResults[key];
                ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                xyzRes.AsParallel().AsOrdered();
                ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                xxyyzzRes.AsParallel().AsOrdered();

                switch (type)
                {
                    case GsaResultsValues.ResultType.Displacement:
                        List<Double3> trans_vals = elementResults.Displacement.ToList();
                        Parallel.For(0, trans_vals.Count, i => //foreach (Double3 val in trans_vals)
                        {
                            xyzRes[i] = GetQuantityResult(trans_vals[i], resultLengthUnit);

                        });
                        break;

                    case GsaResultsValues.ResultType.Stress:
                        List<Tensor3> stress_vals = elementResults.Stress.ToList();
                        Parallel.For(0, stress_vals.Count * 2, i => // (Tensor3 val in stress_vals)
                        {
                            // split computation into two parts by doubling the i-counter
                            if (i < stress_vals.Count)
                            {
                                xyzRes[i] = GetQuantityResult(stress_vals[i], stressUnit);
                            }
                            else
                            {
                                xxyyzzRes[i - stress_vals.Count] = GetQuantityResult(stress_vals[i - stress_vals.Count], stressUnit);
                            }
                        });
                        break;
                }

                // add vector lists to main lists
                r.xyzResults[key] = xyzRes;
                r.xxyyzzResults[key] = xxyyzzRes;
            });

            // update max and min values
            r.dmax_x = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X.Value).Max()).Max();
            r.dmax_y = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y.Value).Max()).Max();
            r.dmax_z = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z.Value).Max()).Max();
            r.dmax_z = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.XYZ.Value).Max()).Max();
            r.dmin_x = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X.Value).Min()).Min();
            r.dmin_y = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y.Value).Min()).Min();
            r.dmin_z = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z.Value).Min()).Min();
            r.dmin_xyz = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.XYZ.Value).Min()).Min();

            if (type == GsaResultsValues.ResultType.Stress)
            {
                r.dmax_xx = r.xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X.Value).Max()).Max();
                r.dmax_yy = r.xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y.Value).Max()).Max();
                r.dmax_zz = r.xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z.Value).Max()).Max();

                r.dmin_xx = r.xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X.Value).Min()).Min();
                r.dmin_yy = r.xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y.Value).Min()).Min();
                r.dmin_zz = r.xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z.Value).Min()).Min();
            }

            return r;
        }

        public static GsaResultsValues Get2DResultValues(ReadOnlyDictionary<int, Element2DResult> globalResults, GsaResultsValues.ResultType type, 
            LengthUnit resultLengthUnit, ForceUnit forceUnit, MomentUnit momentUnit, PressureUnit stressUnit)
        {
            GsaResultsValues r = new GsaResultsValues();

            Parallel.ForEach(globalResults.Keys, key =>
            {
                // lists for results
                Element2DResult elementResults = globalResults[key];
                ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                xyzRes.AsParallel().AsOrdered();
                ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
                xxyyzzRes.AsParallel().AsOrdered();

                switch (type)
                {
                    case (GsaResultsValues.ResultType.Displacement):

                        List<Double6> disp = elementResults.Displacement.ToList();
                        Parallel.For(0, disp.Count * 2, i => // (Double6 val in values)
                        {
                            // split computation into two parts by doubling the i-counter
                            if (i < disp.Count)
                            {
                                xyzRes[i] = GetQuantityResult(disp[i], resultLengthUnit);
                            }
                            else
                            {
                                xyzRes[i - disp.Count] = GetQuantityResult(disp[i - disp.Count], AngleUnit.Radian);
                            }
                        });
                        break;

                    case (GsaResultsValues.ResultType.Force):
                        List<Tensor2> forces = elementResults.Force.ToList();
                        List<Tensor2> moments = elementResults.Moment.ToList();
                        Parallel.For(0, forces.Count + moments.Count, i => // (Tensor2 force in forces)
                        {
                            // combine forces and momemts (list lengths must be the same) to run
                            // calculations in parallel by doubling the i-counter
                            if (i < forces.Count)
                            {
                                xyzRes[i] = GetResult(forces[i], forceUnit);
                            }
                            else
                            {
                                xxyyzzRes[i - forces.Count] = GetResult(moments[i - forces.Count], momentUnit);
                            }
                        });
                        break;

                    case (GsaResultsValues.ResultType.Shear):
                        List<Vector2> shears = elementResults.Shear.ToList();
                        Parallel.For(0, shears.Count, i => // (Vector2 shear in shears)
                        {
                            xyzRes[i] = GetResult(shears[i], forceUnit);
                        });
                        break;

                    case (GsaResultsValues.ResultType.Stress):
                        List<Tensor3> stresses = elementResults.Stress.ToList();
                        Parallel.For(0, stresses.Count * 2, i => // (Tensor3 stress in stresses)
                        {
                            // split computation into two parts by doubling the i-counter
                            if (i < stresses.Count)
                            {
                                xyzRes[i] = GetResult(stresses[i], stressUnit, false);
                            }
                            else
                            {
                                xxyyzzRes[i - stresses.Count] = GetResult(stresses[i - stresses.Count], stressUnit, true);
                            }
                        });
                        break;
                }

                // add vector lists to main lists
                r.xyzResults[key] = xyzRes;
                r.xxyyzzResults[key] = xxyyzzRes;
            });

            // update max and min values
            r.dmax_x = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Max()).Max();
            r.dmax_y = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Max()).Max();
            r.dmax_z = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z).Max()).Max();
            r.dmax_xyz = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res =>
                Math.Sqrt(Math.Pow(res.X, 2) + Math.Pow(res.Y, 2) + Math.Pow(res.Z, 2))
                ).Max()).Max();
            r.dmin_x = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Min()).Min();
            r.dmin_y = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Min()).Min();
            r.dmin_z = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z).Min()).Min();
            r.dmin_xyz = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res =>
                Math.Sqrt(Math.Pow(res.X, 2) + Math.Pow(res.Y, 2) + Math.Pow(res.Z, 2))).Min()).Min();

            if (type != GsaResultsValues.ResultType.Shear)
            {
                r.dmax_xx = r.xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Max()).Max();
                r.dmax_yy = r.xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Max()).Max();
                r.dmax_zz = r.xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z).Max()).Max();
                r.dmax_xxyyzz = r.xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res =>
                    Math.Sqrt(Math.Pow(res.X, 2) + Math.Pow(res.Y, 2) + Math.Pow(res.Z, 2))).Max()).Max();

                r.dmin_xx = r.xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Min()).Min();
                r.dmin_yy = r.xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Min()).Min();
                r.dmin_zz = r.xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z).Min()).Min();
                r.dmin_xxyyzz = r.xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res =>
                    Math.Sqrt(Math.Pow(res.X, 2) + Math.Pow(res.Y, 2) + Math.Pow(res.Z, 2))).Min()).Min();
            }
            return r;
        }

        public static GsaResultsValues Get1DResultValues(ReadOnlyDictionary<int, Element1DResult> globalResults, 
            GsaResultsValues.ResultType type, LengthUnit resultLengthUnit, ForceUnit forceUnit, MomentUnit momentUnit)
        {
            GsaResultsValues r = new GsaResultsValues();

                Parallel.ForEach(globalResults.Keys, key =>
                {
                    // lists for results
                    Element1DResult elementResults = globalResults[key];
                    List<Double6> values = new List<Double6>();
                    ConcurrentDictionary<int, Vector3d> xyzRes = new ConcurrentDictionary<int, Vector3d>();
                    xyzRes.AsParallel().AsOrdered();
                    ConcurrentDictionary<int, Vector3d> xxyyzzRes = new ConcurrentDictionary<int, Vector3d>();
                    xxyyzzRes.AsParallel().AsOrdered();

                    // set the result type dependent on user selection in dropdown
                    switch (type)
                    {
                        case (GsaResultsValues.ResultType.Displacement):
                            values = elementResults.Displacement.ToList();
                            break;
                        case (GsaResultsValues.ResultType.Force):
                            values = elementResults.Force.ToList();
                            break;
                    }

                    // loop through the results
                    Parallel.For(0, values.Count, i =>
                    {
                        Double6 result = values[i];

                        // add the values to the vector lists
                        switch (type)
                        {
                            case (GsaResultsValues.ResultType.Displacement):
                                xyzRes[i] = GetResult(result, resultLengthUnit);
                                xxyyzzRes[i] = GetResult(result, AngleUnit.Radian);
                                break;
                            case (GsaResultsValues.ResultType.Force):
                                xyzRes[i] = GetResult(result, forceUnit);
                                xxyyzzRes[i] = GetResult(result, momentUnit);
                                break;
                        }
                    });
                    // add the vector list to the out tree
                    r.xyzResults[key] = xyzRes;
                    r.xxyyzzResults[key] = xxyyzzRes;
                });

            // update max and min values
            r.dmax_x = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Max()).Max();
            r.dmax_y = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Max()).Max();
            r.dmax_z = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z).Max()).Max();
            r.dmax_xyz = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res =>
                    Math.Sqrt(Math.Pow(res.X, 2) + Math.Pow(res.Y, 2) + Math.Pow(res.Z, 2))).Max()).Max();
            r.dmin_x = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Min()).Min();
            r.dmin_y = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Min()).Min();
            r.dmin_z = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z).Min()).Min();
            r.dmin_xyz = r.xyzResults.AsParallel().Select(list => list.Value.Values.Select(res =>
                    Math.Sqrt(Math.Pow(res.X, 2) + Math.Pow(res.Y, 2) + Math.Pow(res.Z, 2))).Min()).Min();
            r.dmax_xx = r.xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Max()).Max();
            r.dmax_yy = r.xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Max()).Max();
            r.dmax_zz = r.xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z).Max()).Max();
            r.dmax_xxyyzz = r.xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res =>
                    Math.Sqrt(Math.Pow(res.X, 2) + Math.Pow(res.Y, 2) + Math.Pow(res.Z, 2))).Max()).Max();
            r.dmin_xx = r.xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Min()).Min();
            r.dmin_yy = r.xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Min()).Min();
            r.dmin_zz = r.xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z).Min()).Min();
            r.dmin_xxyyzz = r.xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res =>
                    Math.Sqrt(Math.Pow(res.X, 2) + Math.Pow(res.Y, 2) + Math.Pow(res.Z, 2))).Min()).Min();
                
            return r;
        }


    }
}
