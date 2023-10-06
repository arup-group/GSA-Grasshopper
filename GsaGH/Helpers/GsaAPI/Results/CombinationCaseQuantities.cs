using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
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
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetElement1DResultValues(
      ReadOnlyDictionary<int, ReadOnlyCollection<Element1DResult>> globalResults,
      ForceUnit forceUnit, MomentUnit momentUnit, List<int> permutations) {
      var rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0) {
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count)
         .ToList();
      }

      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index => {
        int permutationId = permutations[index];
        var r = new GsaResultsValues {
          Type = GsaResultsValues.ResultType.Force,
        };

        Parallel.ForEach(globalResults.Keys, key => {
          ReadOnlyCollection<Element1DResult> results = globalResults[key];
          Element1DResult result = results[permutationId - 1];
          ReadOnlyCollection<Double6> values = result.Force;
          if (values.Count == 0) {
            return;
          }

          var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
          xyzRes.AsParallel().AsOrdered();
          var xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
          xxyyzzRes.AsParallel().AsOrdered();

          Parallel.For(0, values.Count, i => {
            if (!double.IsNaN(values[i].X) && !double.IsNaN(values[i].Y) 
            && !double.IsNaN(values[i].Z)) {
              xyzRes.TryAdd(i, GetQuantityResult(values[i], forceUnit, true));
            }

            if (!double.IsNaN(values[i].XX) && !double.IsNaN(values[i].YY) 
            && !double.IsNaN(values[i].ZZ)) {
              xxyyzzRes.TryAdd(i, GetQuantityResult(values[i], momentUnit, true));
            }
          });
          r.XyzResults.TryAdd(key, xyzRes);
          r.XxyyzzResults.TryAdd(key, xxyyzzRes);
        });
        r.UpdateMinMax();
        rs.TryAdd(permutationId, r);
      });

      return rs;
    }

    /// <summary>
    ///   Returns strain energy density result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="energyUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <param name="average"></param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetElement1DResultValues(
      ReadOnlyDictionary<int, ReadOnlyCollection<Element1DResult>> globalResults,
      EnergyUnit energyUnit, List<int> permutations, bool average = false) {
      var rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0) {
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count)
         .ToList();
      }

      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index => {
        int permutationId = permutations[index];
        var r = new GsaResultsValues {
          Type = GsaResultsValues.ResultType.StrainEnergy,
        };

        Parallel.ForEach(globalResults.Keys, key => {
          ReadOnlyCollection<Element1DResult> results = globalResults[key];
          Element1DResult result = results[permutationId - 1];
          var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
          xyzRes.AsParallel().AsOrdered();

          if (average) {
            xyzRes.TryAdd(0, GetQuantityResult(result.AverageStrainEnergyDensity, energyUnit));
            r.XyzResults.TryAdd(key, xyzRes);
          } else {
            ReadOnlyCollection<double> values = result.StrainEnergyDensity;
            if (values.Count == 0) {
              return;
            }

            Parallel.For(0, values.Count,
              i => xyzRes.TryAdd(i, GetQuantityResult(values[i], energyUnit)));
            r.XyzResults.TryAdd(key, xyzRes);
          }
        });
        r.UpdateMinMax();
        rs.TryAdd(permutationId, r);
      });

      return rs;
    }

    /// <summary>
    ///   Returns displacement result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="resultLengthUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetElement1DResultValues(
      ReadOnlyDictionary<int, ReadOnlyCollection<Element1DResult>> globalResults,
      LengthUnit resultLengthUnit, List<int> permutations) {
      var rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0) {
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count)
         .ToList();
      }

      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index => {
        int permutationId = permutations[index];
        var r = new GsaResultsValues {
          Type = GsaResultsValues.ResultType.Displacement,
        };

        Parallel.ForEach(globalResults.Keys, key => {
          ReadOnlyCollection<Element1DResult> results = globalResults[key];
          Element1DResult result = results[permutationId - 1];
          ReadOnlyCollection<Double6> values = result.Displacement;
          if (values.Count == 0) {
            return;
          }

          var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
          xyzRes.AsParallel().AsOrdered();
          var xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
          xxyyzzRes.AsParallel().AsOrdered();

          Parallel.For(0, values.Count, i => {
            if (!double.IsNaN(values[i].X) && !double.IsNaN(values[i].Y) 
            && !double.IsNaN(values[i].Z)) {
              xyzRes.TryAdd(i, GetQuantityResult(values[i], resultLengthUnit));
            }

            if (!double.IsNaN(values[i].XX) && !double.IsNaN(values[i].YY) 
            && !double.IsNaN(values[i].ZZ)) {
              xxyyzzRes.TryAdd(i, GetQuantityResult(values[i], AngleUnit.Radian));
            }
          });
          r.XyzResults.TryAdd(key, xyzRes);
          r.XxyyzzResults.TryAdd(key, xxyyzzRes);
        });
        r.UpdateMinMax();
        rs.TryAdd(permutationId, r);
      });

      return rs;
    }

    /// <summary>
    ///   Returns stress result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="stressUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetElement2DResultValues(
      ReadOnlyDictionary<int, ReadOnlyCollection<Element2DResult>> globalResults,
      PressureUnit stressUnit, List<int> permutations) {
      var rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0) {
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count)
         .ToList();
      }

      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index => {
        int permutationId = permutations[index];
        var r = new GsaResultsValues {
          Type = GsaResultsValues.ResultType.Stress,
        };

        Parallel.ForEach(globalResults.Keys, key => {
          ReadOnlyCollection<Element2DResult> results = globalResults[key];
          Element2DResult result = results[permutationId - 1];
          ReadOnlyCollection<Tensor3> values = result.Stress;
          if (values.Count == 0) {
            return;
          }

          var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
          xyzRes.AsParallel().AsOrdered();
          var xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
          xxyyzzRes.AsParallel().AsOrdered();

          Parallel.For(1, values.Count, i => {
            if (!double.IsNaN(values[i].XX) && !double.IsNaN(values[i].YY) 
            && !double.IsNaN(values[i].ZZ)) {
              xyzRes.TryAdd(i, GetQuantityResult(values[i], stressUnit));
            }

            if (!double.IsNaN(values[i].XY) && !double.IsNaN(values[i].YZ) 
            && !double.IsNaN(values[i].ZX)) {
              xxyyzzRes.TryAdd(i, GetQuantityResult(values[i], stressUnit, true));
            }
          });
          if (!double.IsNaN(values[0].XX) && !double.IsNaN(values[0].YY) 
          && !double.IsNaN(values[0].ZZ)) {
            xyzRes.TryAdd(values.Count, GetQuantityResult(values[0], stressUnit)); // add centre point last
          }

          if (!double.IsNaN(values[0].XY) && !double.IsNaN(values[0].YZ) 
          && !double.IsNaN(values[0].ZX)) {
            xxyyzzRes.TryAdd(values.Count, GetQuantityResult(values[0], stressUnit, true));
          }

          r.XyzResults.TryAdd(key, xyzRes);
          r.XxyyzzResults.TryAdd(key, xxyyzzRes);
        });
        r.UpdateMinMax();
        rs.TryAdd(permutationId, r);
      });

      return rs;
    }

    /// <summary>
    ///   Returns shear result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="forceUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetElement2DResultValues(
      ReadOnlyDictionary<int, ReadOnlyCollection<Element2DResult>> globalResults,
      ForcePerLengthUnit forceUnit, List<int> permutations) {
      var rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0) {
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count)
         .ToList();
      }

      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index => {
        int permutationId = permutations[index];
        var r = new GsaResultsValues {
          Type = GsaResultsValues.ResultType.Shear,
        };

        Parallel.ForEach(globalResults.Keys, key => {
          ReadOnlyCollection<Element2DResult> results = globalResults[key];
          Element2DResult result = results[permutationId - 1];
          ReadOnlyCollection<Vector2> values = result.Shear;
          if (values.Count == 0) {
            return;
          }

          var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
          xyzRes.AsParallel().AsOrdered();

          Parallel.For(1, values.Count, i => {
            if (!double.IsNaN(values[i].X) && !double.IsNaN(values[i].Y)) {
              xyzRes.TryAdd(i, GetQuantityResult(values[i], forceUnit));
            }
          });

          if (!double.IsNaN(values[0].X) && !double.IsNaN(values[0].Y)) {
            // add centre point last
            xyzRes.TryAdd(values.Count, GetQuantityResult(values[0], forceUnit)); 
          }

          r.XyzResults.TryAdd(key, xyzRes);
        });
        r.UpdateMinMax();
        rs.TryAdd(permutationId, r);
      });

      return rs;
    }

    /// <summary>
    ///   Returns force and moment result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="forceUnit"></param>
    /// <param name="momentUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetElement2DResultValues(
      ReadOnlyDictionary<int, ReadOnlyCollection<Element2DResult>> globalResults,
      ForcePerLengthUnit forceUnit, ForceUnit momentUnit, List<int> permutations) {
      var rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0) {
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count)
         .ToList();
      }

      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index => {
        int permutationId = permutations[index];
        var r = new GsaResultsValues {
          Type = GsaResultsValues.ResultType.Force,
        };

        Parallel.ForEach(globalResults.Keys, key => {
          ReadOnlyCollection<Element2DResult> results = globalResults[key];
          Element2DResult result = results[permutationId - 1];
          ReadOnlyCollection<Tensor2> forceValues = result.Force;
          ReadOnlyCollection<Tensor2> momentValues = result.Moment;
          if (forceValues.Count == 0) {
            return;
          }

          var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
          xyzRes.AsParallel().AsOrdered();
          var xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
          xxyyzzRes.AsParallel().AsOrdered();

          Parallel.For(1, forceValues.Count, i => {
            if (!double.IsNaN(forceValues[i].XX) && !double.IsNaN(forceValues[i].YY) 
            && !double.IsNaN(forceValues[i].XY)) {
              xyzRes.TryAdd(i, GetQuantityResult(forceValues[i], forceUnit));
            }

            if (!double.IsNaN(momentValues[i].XX) && !double.IsNaN(momentValues[i].YY) 
            && !double.IsNaN(momentValues[i].XY)) {
              xxyyzzRes.TryAdd(i, GetQuantityResult(momentValues[i], momentUnit));
            }
          });

          if (!double.IsNaN(forceValues[0].XX) && !double.IsNaN(forceValues[0].YY) 
          && !double.IsNaN(forceValues[0].XY)) {
            xyzRes.TryAdd(forceValues.Count,
              GetQuantityResult(forceValues[0], forceUnit)); // add centre point last
          }

          if (!double.IsNaN(momentValues[0].XX) && !double.IsNaN(momentValues[0].YY) 
          && !double.IsNaN(momentValues[0].XY)) {
            xxyyzzRes.TryAdd(forceValues.Count, GetQuantityResult(momentValues[0], momentUnit));
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
        rs.TryAdd(permutationId, r);
      });

      return rs;
    }

    /// <summary>
    ///   Returns displacement result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="resultLengthUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetElement2DResultValues(
      ReadOnlyDictionary<int, ReadOnlyCollection<Element2DResult>> globalResults,
      LengthUnit resultLengthUnit, List<int> permutations) {
      var rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0) {
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count)
         .ToList();
      }

      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index => {
        int permutationId = permutations[index];
        var r = new GsaResultsValues {
          Type = GsaResultsValues.ResultType.Displacement,
        };

        Parallel.ForEach(globalResults.Keys, key => {
          ReadOnlyCollection<Element2DResult> results = globalResults[key];
          Element2DResult result = results[permutationId - 1];
          ReadOnlyCollection<Double6> values = result.Displacement;
          if (values.Count == 0) {
            return;
          }

          var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
          xyzRes.AsParallel().AsOrdered();
          var xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
          xxyyzzRes.AsParallel().AsOrdered();

          Parallel.For(1, values.Count, i => {
            if (!double.IsNaN(values[i].X) && !double.IsNaN(values[i].Y) && !double.IsNaN(values[i].Z)) {
              xyzRes.TryAdd(i, GetQuantityResult(values[i], resultLengthUnit));
            }

            if (!double.IsNaN(values[i].XX) && !double.IsNaN(values[i].YY) 
            && !double.IsNaN(values[i].ZZ)) {
              xxyyzzRes.TryAdd(i, GetQuantityResult(values[i], AngleUnit.Radian));
            }
          });
          xyzRes.TryAdd(values.Count,
            GetQuantityResult(values[0], resultLengthUnit)); // add centre point last
          xxyyzzRes.TryAdd(values.Count, GetQuantityResult(values[0], AngleUnit.Radian));
          r.XyzResults.TryAdd(key, xyzRes);
          r.XxyyzzResults.TryAdd(key, xxyyzzRes);
        });
        r.UpdateMinMax();
        rs.TryAdd(permutationId, r);
      });

      return rs;
    }

    /// <summary>
    ///   Returns displacement result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="lengthUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetElement3DResultValues(
      ReadOnlyDictionary<int, ReadOnlyCollection<Element3DResult>> globalResults,
      LengthUnit lengthUnit, List<int> permutations) {
      var rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0) {
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count)
         .ToList();
      }

      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index => {
        int permutationId = permutations[index];
        var r = new GsaResultsValues {
          Type = GsaResultsValues.ResultType.Displacement,
        };

        Parallel.ForEach(globalResults.Keys, key => {
          ReadOnlyCollection<Element3DResult> results = globalResults[key];
          Element3DResult result = results[permutationId - 1];
          ReadOnlyCollection<Double3> values = result.Displacement;
          if (values.Count == 0) {
            return;
          }

          var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
          xyzRes.AsParallel().AsOrdered();

          Parallel.For(1, values.Count, i => {
            if (!double.IsNaN(values[i].X) && !double.IsNaN(values[i].Y) 
            && !double.IsNaN(values[i].Z)) {
              xyzRes.TryAdd(i, GetQuantityResult(values[i], lengthUnit));
            }
          });
          if (!double.IsNaN(values[0].X) && !double.IsNaN(values[0].Y) 
          && !double.IsNaN(values[0].Z)) {
            xyzRes[values.Count] = GetQuantityResult(values[0], lengthUnit); // add centre point at the end
          }

          r.XyzResults.TryAdd(key, xyzRes);
        });
        r.UpdateMinMax();
        rs.TryAdd(permutationId, r);
      });

      return rs;
    }

    /// <summary>
    ///   Returns stress result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="stressUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetElement3DResultValues(
      ReadOnlyDictionary<int, ReadOnlyCollection<Element3DResult>> globalResults,
      PressureUnit stressUnit, List<int> permutations) {
      var rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0) {
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count)
         .ToList();
      }

      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index => {
        int permutationId = permutations[index];
        var r = new GsaResultsValues {
          Type = GsaResultsValues.ResultType.Stress,
        };

        Parallel.ForEach(globalResults.Keys, key => {
          ReadOnlyCollection<Element3DResult> results = globalResults[key];
          Element3DResult result = results[permutationId - 1];
          ReadOnlyCollection<Tensor3> values = result.Stress;
          if (values.Count == 0) {
            return;
          }

          var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
          xyzRes.AsParallel().AsOrdered();
          var xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
          xxyyzzRes.AsParallel().AsOrdered();

          Parallel.For(1, values.Count, i => {
            if (!double.IsNaN(values[i].XX) && !double.IsNaN(values[i].YY) 
            && !double.IsNaN(values[i].ZZ)) {
              xyzRes.TryAdd(i, GetQuantityResult(values[i], stressUnit));
            }

            if (!double.IsNaN(values[i].XY) && !double.IsNaN(values[i].YZ) 
            && !double.IsNaN(values[i].ZX)) {
              xxyyzzRes.TryAdd(i, GetQuantityResult(values[i], stressUnit, true));
            }
          });
          if (!double.IsNaN(values[0].XX) && !double.IsNaN(values[0].YY) 
          && !double.IsNaN(values[0].ZZ)) {
            // add centre point at the end
            xyzRes[values.Count] = GetQuantityResult(values[0], stressUnit); 
          }

          if (!double.IsNaN(values[0].XY) && !double.IsNaN(values[0].YZ) 
          && !double.IsNaN(values[0].ZX)) {
            xxyyzzRes[values.Count] = GetQuantityResult(values[0], stressUnit, true);
          }

          r.XyzResults.TryAdd(key, xyzRes);
          r.XxyyzzResults.TryAdd(key, xxyyzzRes);
        });
        r.UpdateMinMax();
        rs.TryAdd(permutationId, r);
      });

      return rs;
    }

    /// <summary>
    ///   Returns reaction forces result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="forceUnit"></param>
    /// <param name="momentUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <param name="supportnodeIDs">
    ///   bag of support node IDs, if input contains ids then this method will test all nodes and
    ///   include results for these IDs even if they are all zero
    /// </param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetNodeReactionForceResultValues(
      ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> globalResults, ForceUnit forceUnit,
      MomentUnit momentUnit, List<int> permutations, ConcurrentBag<int> supportnodeIDs = null) {
      var rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0) {
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count)
         .ToList();
      }

      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index => {
        int permutationId = permutations[index];
        var r = new GsaResultsValues {
          Type = GsaResultsValues.ResultType.Force,
        };

        Parallel.ForEach(globalResults.Keys, nodeId => {
          ReadOnlyCollection<NodeResult> results = globalResults[nodeId];
          NodeResult result = results[permutationId - 1];
          Double6 values = result.Reaction;

          if (supportnodeIDs != null && !supportnodeIDs.Contains(nodeId)) {
            if (values.X == 0 & values.Y == 0 & values.Z == 0
              & values.XX == 0 & values.YY == 0 & values.ZZ == 0) {
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
        rs.TryAdd(permutationId, r);
      });

      return rs;
    }

    /// <summary>
    ///   Returns displacement result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="resultLengthUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetNodeResultValues(
      ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> globalResults,
      LengthUnit resultLengthUnit, List<int> permutations) {
      var rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0) {
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count)
         .ToList();
      }

      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index => {
        int permutationId = permutations[index];
        var r = new GsaResultsValues {
          Type = GsaResultsValues.ResultType.Displacement,
        };

        Parallel.ForEach(globalResults.Keys, nodeId => {
          ReadOnlyCollection<NodeResult> results = globalResults[nodeId];
          NodeResult result = results[permutationId - 1];
          Double6 values = result.Displacement;

          if (!double.IsNaN(values.X) && !double.IsNaN(values.Y) && !double.IsNaN(values.Z)) {
            var xyz = new ConcurrentDictionary<int, GsaResultQuantity>();
            xyz.TryAdd(0, GetQuantityResult(values, resultLengthUnit));
            r.XyzResults.TryAdd(nodeId, xyz);
          }

          if (!double.IsNaN(values.XX) && !double.IsNaN(values.YY) && !double.IsNaN(values.ZZ)) {
            var xxyyzz = new ConcurrentDictionary<int, GsaResultQuantity>();
            xxyyzz.TryAdd(0, GetQuantityResult(values, AngleUnit.Radian));
            r.XxyyzzResults.TryAdd(nodeId, xxyyzz);
          }
        });

        r.UpdateMinMax();
        rs.TryAdd(permutationId, r);
      });

      return rs;
    }

    /// <summary>
    ///   Returns spring reaction forces result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="forceUnit"></param>
    /// <param name="momentUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <param name="supportnodeIDs">
    ///   bag of support node IDs, if input contains ids then this method will test all nodes and
    ///   include results for these IDs even if they are all zero
    /// </param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetNodeSpringForceResultValues(
      ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> globalResults, ForceUnit forceUnit,
      MomentUnit momentUnit, List<int> permutations, ConcurrentBag<int> supportnodeIDs = null) {
      var rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0) {
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count)
         .ToList();
      }

      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index => {
        int permutationId = permutations[index];
        var r = new GsaResultsValues {
          Type = GsaResultsValues.ResultType.Force,
        };

        Parallel.ForEach(globalResults.Keys, nodeId => {
          ReadOnlyCollection<NodeResult> results = globalResults[nodeId];
          NodeResult result = results[permutationId - 1];
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
        rs.TryAdd(permutationId, r);
      });

      return rs;
    }
  }
}
