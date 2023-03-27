using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Helpers.GsaAPI {
  internal partial class ResultHelper {
    /// <summary>
    /// Returns displacement result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="resultLengthUnit"></param>
    /// <returns></returns>
    internal static GsaResultsValues GetElement3DResultValues(ReadOnlyDictionary<int, Element3DResult> globalResults,
        LengthUnit resultLengthUnit) {
      var r = new GsaResultsValues {
        Type = GsaResultsValues.ResultType.Displacement,
      };

      Parallel.ForEach(globalResults.Keys, key => {
        Element3DResult elementResults = globalResults[key];
        var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xyzRes.AsParallel().AsOrdered();

        ReadOnlyCollection<Double3> transVals = elementResults.Displacement;
        Parallel.For(1, transVals.Count, i => xyzRes[i] = GetQuantityResult(transVals[i], resultLengthUnit));
        xyzRes[transVals.Count] = GetQuantityResult(transVals[0], resultLengthUnit); // add centre point at the end
        r.XyzResults.TryAdd(key, xyzRes);
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
        PressureUnit stressUnit) {
      var r = new GsaResultsValues {
        Type = GsaResultsValues.ResultType.Stress,
      };

      Parallel.ForEach(globalResults.Keys, key => {
        Element3DResult elementResults = globalResults[key];
        var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xyzRes.AsParallel().AsOrdered();
        var xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xxyyzzRes.AsParallel().AsOrdered();

        ReadOnlyCollection<Tensor3> stressVals = elementResults.Stress;
        Parallel.For(1, stressVals.Count * 2, i => {
          if (i == stressVals.Count)
            return;
          if (i < stressVals.Count)
            xyzRes[i] = GetQuantityResult(stressVals[i], stressUnit);
          else
            xxyyzzRes[i - stressVals.Count] = GetQuantityResult(stressVals[i - stressVals.Count], stressUnit, true);
        });
        xyzRes[stressVals.Count] = GetQuantityResult(stressVals[0], stressUnit); // add centre point at the end
        xxyyzzRes[stressVals.Count] = GetQuantityResult(stressVals[0], stressUnit, true);

        r.XyzResults.TryAdd(key, xyzRes);
        r.XxyyzzResults.TryAdd(key, xxyyzzRes);
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
        PressureUnit stressUnit) {
      var r = new GsaResultsValues {
        Type = GsaResultsValues.ResultType.Stress,
      };

      Parallel.ForEach(globalResults.Keys, key => {
        Element2DResult elementResults = globalResults[key];
        var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xyzRes.AsParallel().AsOrdered();
        var xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xxyyzzRes.AsParallel().AsOrdered();

        ReadOnlyCollection<Tensor3> stresses = elementResults.Stress;
        Parallel.For(1, stresses.Count * 2, i => {
          if (i == stresses.Count)
            return;
          if (i < stresses.Count)
            xyzRes[i] = GetQuantityResult(stresses[i], stressUnit);
          else
            xxyyzzRes[i - stresses.Count] = GetQuantityResult(stresses[i - stresses.Count], stressUnit, true);
        });
        xyzRes[stresses.Count] = GetQuantityResult(stresses[0], stressUnit); // add centre point at the end
        xxyyzzRes[stresses.Count] = GetQuantityResult(stresses[0], stressUnit, true);

        r.XyzResults.TryAdd(key, xyzRes);
        r.XxyyzzResults.TryAdd(key, xxyyzzRes);
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
        ForcePerLengthUnit forceUnit) {
      var r = new GsaResultsValues {
        Type = GsaResultsValues.ResultType.Shear,
      };

      Parallel.ForEach(globalResults.Keys, key => {
        Element2DResult elementResults = globalResults[key];
        var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xyzRes.AsParallel().AsOrdered();
        var xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xxyyzzRes.AsParallel().AsOrdered();

        ReadOnlyCollection<Vector2> shears = elementResults.Shear;
        Parallel.For(1, shears.Count, i => xyzRes[i] = GetQuantityResult(shears[i], forceUnit));
        xyzRes[shears.Count] = GetQuantityResult(shears[0], forceUnit); // add centre point at the end

        r.XyzResults.TryAdd(key, xyzRes);
        r.XxyyzzResults.TryAdd(key, xxyyzzRes);
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
        ForcePerLengthUnit forceUnit, ForceUnit momentUnit) {
      var r = new GsaResultsValues {
        Type = GsaResultsValues.ResultType.Force,
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
          if (i == forces.Count)
            return;
          if (i < forces.Count)
            xyzRes[i] = GetQuantityResult(forces[i], forceUnit);
          else
            xxyyzzRes[i - forces.Count] = GetQuantityResult(moments[i - forces.Count], momentUnit);
        });
        xyzRes[forces.Count] = GetQuantityResult(forces[0], forceUnit); // add centre point at the end
        xxyyzzRes[moments.Count] = GetQuantityResult(moments[0], momentUnit);

        Parallel.ForEach(xxyyzzRes.Keys, i => {
          xyzRes[i].Xyz = new Force(
                    xxyyzzRes[i].X.Value
                    + Math.Sign(xxyyzzRes[i].X.Value)
                    * Math.Abs(xxyyzzRes[i].Z.Value),
                    momentUnit);
          xxyyzzRes[i].Xyz = new Force(
                    xxyyzzRes[i].Y.Value
                    + Math.Sign(xxyyzzRes[i].Y.Value)
                    * Math.Abs(xxyyzzRes[i].Z.Value),
                    momentUnit);
        });

        r.XyzResults.TryAdd(key, xyzRes);
        r.XxyyzzResults.TryAdd(key, xxyyzzRes);
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
        LengthUnit resultLengthUnit) {
      var r = new GsaResultsValues {
        Type = GsaResultsValues.ResultType.Displacement,
      };

      Parallel.ForEach(globalResults.Keys, key => {
        Element2DResult elementResults = globalResults[key];
        var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xyzRes.AsParallel().AsOrdered();
        var xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xxyyzzRes.AsParallel().AsOrdered();

        ReadOnlyCollection<Double6> disp = elementResults.Displacement;
        Parallel.For(1, disp.Count * 2, i => {
          if (i == disp.Count)
            return;
          if (i < disp.Count)
            xyzRes[i] = GetQuantityResult(disp[i], resultLengthUnit);
          else
            xxyyzzRes[i - disp.Count] = GetQuantityResult(disp[i - disp.Count], AngleUnit.Radian);
        });
        xyzRes[disp.Count] = GetQuantityResult(disp[0], resultLengthUnit); // add centre point at the end
        xxyyzzRes[disp.Count - disp.Count] = GetQuantityResult(disp[0], AngleUnit.Radian);

        r.XyzResults.TryAdd(key, xyzRes);
        r.XxyyzzResults.TryAdd(key, xxyyzzRes);
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
        ForceUnit forceUnit, MomentUnit momentUnit) {
      var r = new GsaResultsValues {
        Type = GsaResultsValues.ResultType.Force,
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
          xyzRes[i] = GetQuantityResult(result, forceUnit, true);
          xxyyzzRes[i] = GetQuantityResult(result, momentUnit, true);
        });
        r.XyzResults.TryAdd(key, xyzRes);
        r.XxyyzzResults.TryAdd(key, xxyyzzRes);
      });

      r.UpdateMinMax();
      return r;
    }

    /// <summary>
    /// Returns strain energy density result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="energyUnit"></param>
    /// <param name="average"></param>
    /// <returns></returns>
    internal static GsaResultsValues GetElement1DResultValues(ReadOnlyDictionary<int, Element1DResult> globalResults,
        EnergyUnit energyUnit, bool average = false) {
      var r = new GsaResultsValues {
        Type = GsaResultsValues.ResultType.StrainEnergy,
      };

      Parallel.ForEach(globalResults.Keys, key => {
        Element1DResult elementResults = globalResults[key];
        var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xyzRes.AsParallel().AsOrdered();

        if (average) {
          xyzRes[0] = GetQuantityResult(elementResults.AverageStrainEnergyDensity, energyUnit);
          r.XyzResults.TryAdd(key, xyzRes);
        }
        else {
          ReadOnlyCollection<double> values = elementResults.StrainEnergyDensity;
          Parallel.For(0, values.Count, i => xyzRes[i] = GetQuantityResult(values[i], energyUnit));
          r.XyzResults.TryAdd(key, xyzRes);
        }
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
        LengthUnit resultLengthUnit) {
      var r = new GsaResultsValues { Type = GsaResultsValues.ResultType.Displacement };

      Parallel.ForEach(globalResults.Keys, key => {
        Element1DResult elementResults = globalResults[key];
        var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xyzRes.AsParallel().AsOrdered();
        var xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xxyyzzRes.AsParallel().AsOrdered();

        ReadOnlyCollection<Double6> values = elementResults.Displacement;
        Parallel.For(0, values.Count, i => {
          Double6 result = values[i];
          xyzRes[i] = GetQuantityResult(result, resultLengthUnit);
          xxyyzzRes[i] = GetQuantityResult(result, AngleUnit.Radian);
        });
        r.XyzResults.TryAdd(key, xyzRes);
        r.XxyyzzResults.TryAdd(key, xxyyzzRes);
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
        LengthUnit resultLengthUnit) {
      var r = new GsaResultsValues { Type = GsaResultsValues.ResultType.Force };

      Parallel.ForEach(globalResults.Keys, nodeId => {
        NodeResult result = globalResults[nodeId];
        Double6 values = result.Displacement;

        var xyz = new ConcurrentDictionary<int, GsaResultQuantity>();
        xyz.TryAdd(0, GetQuantityResult(values, resultLengthUnit));
        r.XyzResults.TryAdd(nodeId, xyz);
        var xxyyzz = new ConcurrentDictionary<int, GsaResultQuantity>();
        xxyyzz.TryAdd(0, GetQuantityResult(values, AngleUnit.Radian));
        r.XxyyzzResults.TryAdd(nodeId, xxyyzz);
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
    internal static GsaResultsValues GetNodeReactionForceResultValues(ReadOnlyDictionary<int, NodeResult> globalResults,
        ForceUnit forceUnit, MomentUnit momentUnit, ConcurrentBag<int> supportnodeIDs = null) {
      var r = new GsaResultsValues { Type = GsaResultsValues.ResultType.Force };

      Parallel.ForEach(globalResults.Keys, nodeId => {
        NodeResult result = globalResults[nodeId];
        Double6 values = result.Reaction;

        if (supportnodeIDs != null && !supportnodeIDs.Contains(nodeId)) {
          if (values.X == 0 & values.Y == 0 & values.Z == 0
          & values.XX == 0 & values.YY == 0 & values.ZZ == 0) {
            return;
          }
        }

        var xyz = new ConcurrentDictionary<int, GsaResultQuantity>();
        xyz.TryAdd(0, GetQuantityResult(values, forceUnit));
        r.XyzResults.TryAdd(nodeId, xyz);
        var xxyyzz = new ConcurrentDictionary<int, GsaResultQuantity>();
        xxyyzz.TryAdd(0, GetQuantityResult(values, momentUnit));
        r.XxyyzzResults.TryAdd(nodeId, xxyyzz);
      });

      r.UpdateMinMax();
      return r;
    }
    /// <summary>
    /// Returns spring reaction forces result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="forceUnit"></param>
    /// <param name="momentUnit"></param>
    /// <returns></returns>
    internal static GsaResultsValues GetNodeSpringForceResultValues(ReadOnlyDictionary<int, NodeResult> globalResults,
        ForceUnit forceUnit, MomentUnit momentUnit, ConcurrentBag<int> supportnodeIDs = null) {
      var r = new GsaResultsValues { Type = GsaResultsValues.ResultType.Force };

      Parallel.ForEach(globalResults.Keys, nodeId => {
        NodeResult result = globalResults[nodeId];
        Double6 values = result.SpringForce;

        if (supportnodeIDs != null && !supportnodeIDs.Contains(nodeId)) {
          if (values.X == 0 & values.Y == 0 & values.Z == 0
          & values.XX == 0 & values.YY == 0 & values.ZZ == 0) {
            return;
          }
        }

        var xyz = new ConcurrentDictionary<int, GsaResultQuantity>();
        xyz.TryAdd(0, GetQuantityResult(values, forceUnit));
        r.XyzResults.TryAdd(nodeId, xyz);
        var xxyyzz = new ConcurrentDictionary<int, GsaResultQuantity>();
        xxyyzz.TryAdd(0, GetQuantityResult(values, momentUnit));
        r.XxyyzzResults.TryAdd(nodeId, xxyyzz);
      });

      r.UpdateMinMax();
      return r;
    }
  }
}
