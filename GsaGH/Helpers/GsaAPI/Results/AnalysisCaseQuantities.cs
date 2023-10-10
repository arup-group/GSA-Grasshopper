using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Parameters._5_Results.Quantities;
using OasysUnits;
using OasysUnits.Units;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using static GsaGH.Parameters.GsaResultsValues;
using AngleUnit = OasysUnits.Units.AngleUnit;
using EnergyUnit = OasysUnits.Units.EnergyUnit;
using ForceUnit = OasysUnits.Units.ForceUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Helpers.GsaApi {
  internal static partial class ResultHelper {

    /// <summary>
    ///   Returns forces result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="forceUnit"></param>
    /// <param name="momentUnit"></param>
    /// <returns></returns>
    internal static GsaResultsValues GetElement1DResultValues(
      ReadOnlyDictionary<int, Element1DResult> globalResults, ForceUnit forceUnit,
      MomentUnit momentUnit) {
      var r = new GsaResultsValues {
        Type = ResultType.Force,
      };

      Parallel.ForEach(globalResults.Keys, key => {
        Element1DResult elementResults = globalResults[key];
        var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xyzRes.AsParallel().AsOrdered();
        var xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xxyyzzRes.AsParallel().AsOrdered();

        ReadOnlyCollection<Double6> values = elementResults.Force;
        Parallel.For(0, values.Count, i => {
          Double6 result = values[i];
          if (!double.IsNaN(values[i].X) && !double.IsNaN(values[i].Y) && !double.IsNaN(values[i].Z)) {
            xyzRes[i] = GetQuantityResult(result, forceUnit, true);
          }

          if (!double.IsNaN(values[i].XX) && !double.IsNaN(values[i].YY) && !double.IsNaN(values[i].ZZ)) {
            xxyyzzRes[i] = GetQuantityResult(result, momentUnit, true);
          }
        });
        r.XyzResults.TryAdd(key, xyzRes);
        r.XxyyzzResults.TryAdd(key, xxyyzzRes);
      });

      r.UpdateMinMax();
      return r;
    }

    /// <summary>
    ///   Returns strain energy density result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="energyUnit"></param>
    /// <param name="average"></param>
    /// <returns></returns>
    internal static GsaResultsValues GetElement1DResultValues(
      ReadOnlyDictionary<int, Element1DResult> globalResults, EnergyUnit energyUnit,
      bool average = false) {
      var r = new GsaResultsValues {
        Type = ResultType.StrainEnergy,
      };

      Parallel.ForEach(globalResults.Keys, key => {
        Element1DResult elementResults = globalResults[key];
        var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xyzRes.AsParallel().AsOrdered();

        if (average) {
          xyzRes[0] = GetQuantityResult(elementResults.AverageStrainEnergyDensity, energyUnit);
          r.XyzResults.TryAdd(key, xyzRes);
        } else {
          ReadOnlyCollection<double> values = elementResults.StrainEnergyDensity;
          Parallel.For(0, values.Count, i => xyzRes[i] = GetQuantityResult(values[i], energyUnit));
          r.XyzResults.TryAdd(key, xyzRes);
        }
      });

      r.UpdateMinMax();
      return r;
    }

    /// <summary>
    ///   Returns displacement result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="resultLengthUnit"></param>
    /// <returns></returns>
    internal static GsaResultsValues GetElement1DResultValues(
      ReadOnlyDictionary<int, Element1DResult> globalResults, LengthUnit resultLengthUnit) {
      var r = new GsaResultsValues {
        Type = ResultType.Displacement,
      };

      Parallel.ForEach(globalResults.Keys, key => {
        Element1DResult elementResults = globalResults[key];
        var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xyzRes.AsParallel().AsOrdered();
        var xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xxyyzzRes.AsParallel().AsOrdered();

        ReadOnlyCollection<Double6> values = elementResults.Displacement;
        Parallel.For(0, values.Count, i => {
          Double6 result = values[i];
          if (!double.IsNaN(values[i].X) && !double.IsNaN(values[i].Y) && !double.IsNaN(values[i].Z)) {
            xyzRes[i] = GetQuantityResult(result, resultLengthUnit);
          }

          if (!double.IsNaN(values[i].XX) && !double.IsNaN(values[i].YY) && !double.IsNaN(values[i].ZZ)) {
            xxyyzzRes[i] = GetQuantityResult(result, AngleUnit.Radian);
          }
        });
        r.XyzResults.TryAdd(key, xyzRes);
        r.XxyyzzResults.TryAdd(key, xxyyzzRes);
      });

      r.UpdateMinMax();
      return r;
    }

    /// <summary>
    ///   Returns stress result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="stressUnit"></param>
    /// <returns></returns>
    internal static GsaResultsValues GetElement2DResultValues(
      ReadOnlyDictionary<int, Element2DResult> globalResults, PressureUnit stressUnit) {
      var r = new GsaResultsValues {
        Type = ResultType.Stress,
      };

      Parallel.ForEach(globalResults.Keys, key => {
        Element2DResult elementResults = globalResults[key];
        var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xyzRes.AsParallel().AsOrdered();
        var xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xxyyzzRes.AsParallel().AsOrdered();

        ReadOnlyCollection<Tensor3> stresses = elementResults.Stress;
        Parallel.For(1, stresses.Count * 2, i => {
          if (i == stresses.Count) {
            return;
          }

          if (i < stresses.Count && !double.IsNaN(stresses[i].XX) &&
              !double.IsNaN(stresses[i].YY) && !double.IsNaN(stresses[i].ZZ)) {
            xyzRes[i] = GetQuantityResult(stresses[i], stressUnit);
          } else if (!double.IsNaN(stresses[i - stresses.Count].XY) 
                     && !double.IsNaN(stresses[i - stresses.Count].YZ)
                     && !double.IsNaN(stresses[i - stresses.Count].ZX)) {
            xxyyzzRes[i - stresses.Count]
              = GetQuantityResult(stresses[i - stresses.Count], stressUnit, true);
          }
        });

        if (!double.IsNaN(stresses[0].XX) && !double.IsNaN(stresses[0].YY)
            && !double.IsNaN(stresses[0].ZZ)) {
          xyzRes[stresses.Count] = GetQuantityResult(stresses[0], stressUnit); // add centre point at the end
        }

        if (!double.IsNaN(stresses[0].XY) && !double.IsNaN(stresses[0].YZ)
            && !double.IsNaN(stresses[0].ZX)) {
          xxyyzzRes[stresses.Count] = GetQuantityResult(stresses[0], stressUnit, true);
        }

        r.XyzResults.TryAdd(key, xyzRes);
        r.XxyyzzResults.TryAdd(key, xxyyzzRes);
      });

      r.UpdateMinMax();
      return r;
    }

    /// <summary>
    ///   Returns shear result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="forceUnit"></param>
    /// <returns></returns>
    internal static GsaResultsValues GetElement2DResultValues(
      ReadOnlyDictionary<int, Element2DResult> globalResults, ForcePerLengthUnit forceUnit) {
      var r = new GsaResultsValues {
        Type = ResultType.Shear,
      };

      Parallel.ForEach(globalResults.Keys, key => {
        Element2DResult elementResults = globalResults[key];
        var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xyzRes.AsParallel().AsOrdered();
        var xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xxyyzzRes.AsParallel().AsOrdered();

        ReadOnlyCollection<Vector2> shears = elementResults.Shear;
        Parallel.For(1, shears.Count, i => {
          if (!double.IsNaN(shears[i].X) && !double.IsNaN(shears[i].Y)) {
            xyzRes[i] = GetQuantityResult(shears[i], forceUnit);
          }
        });

        if (!double.IsNaN(shears[0].X) && !double.IsNaN(shears[0].Y)) {
          // add centre point at the end
          xyzRes[shears.Count] = GetQuantityResult(shears[0], forceUnit);
        }

        r.XyzResults.TryAdd(key, xyzRes);
        r.XxyyzzResults.TryAdd(key, xxyyzzRes);
      });

      r.UpdateMinMax();
      return r;
    }

    /// <summary>
    ///   Returns force and moment result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="forceUnit"></param>
    /// <param name="momentUnit"></param>
    /// <returns></returns>
    internal static GsaResultsValues GetElement2DResultValues(
      ReadOnlyDictionary<int, Element2DResult> globalResults, ForcePerLengthUnit forceUnit,
      ForceUnit momentUnit) {
      var r = new GsaResultsValues {
        Type = ResultType.Force,
      };

      Parallel.ForEach(globalResults.Keys, key => {
        Element2DResult elementResults = globalResults[key];
        var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xyzRes.AsParallel().AsOrdered();
        var xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xxyyzzRes.AsParallel().AsOrdered();

        ReadOnlyCollection<Tensor2> forces = elementResults.Force;
        ReadOnlyCollection<Tensor2> moments = elementResults.Moment;
        Parallel.For(1, forces.Count + moments.Count, i => {
          if (i == forces.Count) {
            return;
          }

          if (i < forces.Count && !double.IsNaN(forces[i].XX) && !double.IsNaN(forces[i].YY)
              && !double.IsNaN(forces[i].XY)) {
            xyzRes[i] = GetQuantityResult(forces[i], forceUnit);
          } else if (!double.IsNaN(moments[i - forces.Count].XX)
                     && !double.IsNaN(moments[i - forces.Count].YY)
                     && !double.IsNaN(moments[i - forces.Count].XY)) {
            xxyyzzRes[i - forces.Count] = GetQuantityResult(moments[i - forces.Count], momentUnit);
          }
        });

        if (!double.IsNaN(forces[0].XX) && !double.IsNaN(forces[0].YY)
            && !double.IsNaN(forces[0].XY)) {
          // add centre point at the end
          xyzRes[forces.Count] = GetQuantityResult(forces[0], forceUnit);
        }

        if (!double.IsNaN(moments[0].XX) && !double.IsNaN(moments[0].YY)
            && !double.IsNaN(moments[0].XY)) {
          xxyyzzRes[moments.Count] = GetQuantityResult(moments[0], momentUnit);
        }

        Parallel.ForEach(xxyyzzRes.Keys, i => {
          xyzRes[i].Xyz = new Force(
            xxyyzzRes[i].X.Value
            + (Math.Sign(xxyyzzRes[i].X.Value) * Math.Abs(xxyyzzRes[i].Z.Value)), momentUnit);
          xxyyzzRes[i].Xyz = new Force(
            xxyyzzRes[i].Y.Value
            + (Math.Sign(xxyyzzRes[i].Y.Value) * Math.Abs(xxyyzzRes[i].Z.Value)), momentUnit);
        });

        r.XyzResults.TryAdd(key, xyzRes);
        r.XxyyzzResults.TryAdd(key, xxyyzzRes);
      });

      r.UpdateMinMax();
      return r;
    }

    /// <summary>
    ///   Returns displacement result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="resultLengthUnit"></param>
    /// <returns></returns>
    internal static GsaResultsValues GetElement2DResultValues(
      ReadOnlyDictionary<int, Element2DResult> globalResults, LengthUnit resultLengthUnit) {
      var r = new GsaResultsValues {
        Type = ResultType.Displacement,
      };

      Parallel.ForEach(globalResults.Keys, key => {
        Element2DResult elementResults = globalResults[key];
        var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xyzRes.AsParallel().AsOrdered();
        var xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xxyyzzRes.AsParallel().AsOrdered();

        ReadOnlyCollection<Double6> disp = elementResults.Displacement;
        Parallel.For(1, disp.Count * 2, i => {
          if (i == disp.Count) {
            return;
          }

          if (i < disp.Count && !double.IsNaN(disp[i].X) && !double.IsNaN(disp[i].Y)
              && !double.IsNaN(disp[i].Z)) {
            xyzRes[i] = GetQuantityResult(disp[i], resultLengthUnit);
          } else if (!double.IsNaN(disp[i - disp.Count].XX)
                     && !double.IsNaN(disp[i - disp.Count].YY)
                     && !double.IsNaN(disp[i - disp.Count].ZZ)) {
            xxyyzzRes[i - disp.Count] = GetQuantityResult(disp[i - disp.Count], AngleUnit.Radian);
          }
        });

        if (!double.IsNaN(disp[0].X) && !double.IsNaN(disp[0].Y) && !double.IsNaN(disp[0].Z)) {
          // add centre point at the end
          xyzRes[disp.Count] = GetQuantityResult(disp[0], resultLengthUnit);
        }

        if (!double.IsNaN(disp[0].XX) && !double.IsNaN(disp[0].YY) && !double.IsNaN(disp[0].ZZ)) {
          xxyyzzRes[disp.Count] = GetQuantityResult(disp[0], AngleUnit.Radian);
        }

        r.XyzResults.TryAdd(key, xyzRes);
        r.XxyyzzResults.TryAdd(key, xxyyzzRes);
      });

      r.UpdateMinMax();
      return r;
    }

    /// <summary>
    ///   Returns displacement result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="resultLengthUnit"></param>
    /// <returns></returns>
    internal static GsaResultsValues GetElement3DResultValues(
      ReadOnlyDictionary<int, Element3DResult> globalResults, LengthUnit resultLengthUnit) {
      var r = new GsaResultsValues {
        Type = ResultType.Displacement,
      };

      Parallel.ForEach(globalResults.Keys, key => {
        Element3DResult elementResults = globalResults[key];
        var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xyzRes.AsParallel().AsOrdered();

        ReadOnlyCollection<Double3> transVals = elementResults.Displacement;
        Parallel.For(1, transVals.Count, i => {
          if (!double.IsNaN(transVals[i].X) && !double.IsNaN(transVals[i].Y) && !double.IsNaN(transVals[i].Z)) {
            xyzRes[i] = GetQuantityResult(transVals[i], resultLengthUnit);
          }
        });

        if (!double.IsNaN(transVals[0].X) && !double.IsNaN(transVals[0].Y) && !double.IsNaN(transVals[0].Z)) {
          // add centre point at the end
          xyzRes[transVals.Count] = GetQuantityResult(transVals[0], resultLengthUnit);
        }

        r.XyzResults.TryAdd(key, xyzRes);
      });

      r.UpdateMinMax();
      return r;
    }

    /// <summary>
    ///   Returns stress result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="stressUnit"></param>
    /// <returns></returns>
    internal static GsaResultsValues GetElement3DResultValues(
      ReadOnlyDictionary<int, Element3DResult> globalResults, PressureUnit stressUnit) {
      var r = new GsaResultsValues {
        Type = ResultType.Stress,
      };

      Parallel.ForEach(globalResults.Keys, key => {
        Element3DResult elementResults = globalResults[key];
        var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xyzRes.AsParallel().AsOrdered();
        var xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xxyyzzRes.AsParallel().AsOrdered();

        ReadOnlyCollection<Tensor3> stressVals = elementResults.Stress;
        Parallel.For(1, stressVals.Count * 2, i => {
          if (i == stressVals.Count) {
            return;
          }

          if (i < stressVals.Count && !double.IsNaN(stressVals[i].XX)
              && !double.IsNaN(stressVals[i].YY) && !double.IsNaN(stressVals[i].ZZ)) {
            xyzRes[i] = GetQuantityResult(stressVals[i], stressUnit);
          } else if (!double.IsNaN(stressVals[i - stressVals.Count].XY)
                     && !double.IsNaN(stressVals[i - stressVals.Count].YZ)
                     && !double.IsNaN(stressVals[i - stressVals.Count].ZX)) {
            xxyyzzRes[i - stressVals.Count]
              = GetQuantityResult(stressVals[i - stressVals.Count], stressUnit, true);
          }
        });

        if (!double.IsNaN(stressVals[0].XX)
              && !double.IsNaN(stressVals[0].YY) && !double.IsNaN(stressVals[0].ZZ)) {
          // add centre point at the end
          xyzRes[stressVals.Count] = GetQuantityResult(stressVals[0], stressUnit);
        }

        if (!double.IsNaN(stressVals[0].XY)
              && !double.IsNaN(stressVals[0].YZ) && !double.IsNaN(stressVals[0].ZX)) {
          xxyyzzRes[stressVals.Count] = GetQuantityResult(stressVals[0], stressUnit, true);
        }

        r.XyzResults.TryAdd(key, xyzRes);
        r.XxyyzzResults.TryAdd(key, xxyyzzRes);
      });

      r.UpdateMinMax();
      return r;
    }

    /// <summary>
    ///   Returns reaction forces result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="forceUnit"></param>
    /// <param name="momentUnit"></param>
    /// <param name="supportnodeIDs"></param>
    /// <returns></returns>
    internal static GsaResultsValues GetNodeReactionForceResultValues(
      ReadOnlyDictionary<int, NodeResult> globalResults, ForceUnit forceUnit, MomentUnit momentUnit,
      ConcurrentBag<int> supportnodeIDs = null) {
      var r = new GsaResultsValues {
        Type = ResultType.Force,
      };

      Parallel.ForEach(globalResults.Keys, nodeId => {
        NodeResult result = globalResults[nodeId];
        Double6 values = result.Reaction;

        if (supportnodeIDs != null && !supportnodeIDs.Contains(nodeId)) {
          if (values.X == 0 & values.Y == 0 & values.Z == 0 & values.XX == 0 & values.YY == 0
            & values.ZZ == 0) {
            return;
          }
        }

        if (!double.IsNaN(values.X) && !double.IsNaN(values.Y) && !double.IsNaN(values.Z)) {
          var xyz = new ConcurrentDictionary<int, GsaResultQuantity>();
          xyz.TryAdd(0, GetQuantityResult(values, forceUnit));
          r.XyzResults.TryAdd(nodeId, xyz);
        }

        if (!double.IsNaN(values.XX) && !double.IsNaN(values.YY) && !double.IsNaN(values.ZZ)) {
          var xxyyzz = new ConcurrentDictionary<int, GsaResultQuantity>();
          xxyyzz.TryAdd(0, GetQuantityResult(values, momentUnit));
          r.XxyyzzResults.TryAdd(nodeId, xxyyzz);
        }
      });

      r.UpdateMinMax();
      return r;
    }

    /// <summary>
    ///   Returns displacement result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="resultLengthUnit"></param>
    /// <returns></returns>
    internal static GsaDisplacementValues GetNodeResultValues(
      ReadOnlyDictionary<int, NodeResult> globalResults, LengthUnit resultLengthUnit) {
      var r = new GsaDisplacementValues {
        Type = ResultType.Displacement,
      };

      Parallel.ForEach(globalResults.Keys, nodeId => {
        NodeResult result = globalResults[nodeId];
        Double6 values = result.Displacement;
        var quantities = new GsaDisplacementQuantity(values, resultLengthUnit, AngleUnit.Radian);
        if (!double.IsNaN(values.X) && !double.IsNaN(values.Y) && !double.IsNaN(values.Z)) {
          var xyz = new ConcurrentDictionary<int, IDisplacementQuantity>();
          xyz.TryAdd(0, quantities);
          r.Results.TryAdd(nodeId, xyz);
        }

        if (!double.IsNaN(values.XX) && !double.IsNaN(values.YY) && !double.IsNaN(values.ZZ)) {
          var xxyyzz = new ConcurrentDictionary<int, IDisplacementQuantity>();
          xxyyzz.TryAdd(0, quantities);
          r.Results.TryAdd(nodeId, xxyyzz);
        }
      });

      r.UpdateMinMax();

      return r;
    }

    /// <summary>
    ///   Returns spring reaction forces result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="forceUnit"></param>
    /// <param name="momentUnit"></param>
    /// <param name="supportnodeIDs"></param>
    /// <returns></returns>
    internal static GsaResultsValues GetNodeSpringForceResultValues(
      ReadOnlyDictionary<int, NodeResult> globalResults, ForceUnit forceUnit, MomentUnit momentUnit,
      ConcurrentBag<int> supportnodeIDs = null) {
      var r = new GsaResultsValues {
        Type = ResultType.Force,
      };

      Parallel.ForEach(globalResults.Keys, nodeId => {
        NodeResult result = globalResults[nodeId];
        Double6 values = result.SpringForce;

        if (supportnodeIDs != null && !supportnodeIDs.Contains(nodeId)) {
          if (values.X == 0 & values.Y == 0 & values.Z == 0 & values.XX == 0 & values.YY == 0
            & values.ZZ == 0) {
            return;
          }
        }

        if (!double.IsNaN(values.X) && !double.IsNaN(values.Y) && !double.IsNaN(values.Z)) {
          var xyz = new ConcurrentDictionary<int, GsaResultQuantity>();
          xyz.TryAdd(0, GetQuantityResult(values, forceUnit));
          r.XyzResults.TryAdd(nodeId, xyz);
        }

        if (!double.IsNaN(values.XX) && !double.IsNaN(values.YY) && !double.IsNaN(values.ZZ)) {
          var xxyyzz = new ConcurrentDictionary<int, GsaResultQuantity>();
          xxyyzz.TryAdd(0, GetQuantityResult(values, momentUnit));
          r.XxyyzzResults.TryAdd(nodeId, xxyyzz);
        }
      });

      r.UpdateMinMax();
      return r;
    }
  }
}
