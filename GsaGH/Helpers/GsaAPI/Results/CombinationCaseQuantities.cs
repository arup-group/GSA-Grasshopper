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
using Rhino.Geometry;

namespace GsaGH.Helpers.GsaApi
{
  internal partial class ResultHelper
  {
    /// <summary>
    /// Returns displacement result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="lengthUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetElement3DResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<Element3DResult>> globalResults,
        LengthUnit lengthUnit, List<int> permutations)
    {
      ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0)
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count).ToList();
      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index => // loop through permutations
      {
        int permutationID = permutations[index];
        GsaResultsValues r = new GsaResultsValues();
        r.Type = GsaResultsValues.ResultType.Displacement;

        Parallel.ForEach(globalResults.Keys, key =>
        {
          // lists for results
          ReadOnlyCollection<Element3DResult> results = globalResults[key];
          Element3DResult result = results[permutationID - 1];
          ReadOnlyCollection<Double3> values = result.Displacement;
          if (values.Count == 0) { return; }
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
        rs.TryAdd(permutationID, r); 
      });

      return rs;
    }

    /// <summary>
    /// Returns stress result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="stressUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetElement3DResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<Element3DResult>> globalResults,
        PressureUnit stressUnit, List<int> permutations)
    {
      ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0)
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count).ToList();
      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index => // loop through permutations
      {
        int permutationID = permutations[index];
        GsaResultsValues r = new GsaResultsValues();
        r.Type = GsaResultsValues.ResultType.Stress;

        Parallel.ForEach(globalResults.Keys, key =>
        {
          // lists for results
          ReadOnlyCollection<Element3DResult> results = globalResults[key];
          Element3DResult result = results[permutationID - 1];
          ReadOnlyCollection<Tensor3> values = result.Stress;
          if (values.Count == 0) { return; }
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
        rs.TryAdd(permutationID, r);
      });

      return rs;
    }

    /// <summary>
    /// Returns stress result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="stressUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetElement2DResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<Element2DResult>> globalResults,
        PressureUnit stressUnit, List<int> permutations)
    {
      ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0)
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count).ToList();
      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index => // loop through permutations
      {
        int permutationID = permutations[index];
        GsaResultsValues r = new GsaResultsValues();
        r.Type = GsaResultsValues.ResultType.Stress;

        Parallel.ForEach(globalResults.Keys, key =>
        {
          // lists for results
          ReadOnlyCollection<Element2DResult> results = globalResults[key];
          Element2DResult result = results[permutationID - 1];
          ReadOnlyCollection<Tensor3> values = result.Stress;
          if (values.Count == 0) { return; }
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
        rs.TryAdd(permutationID, r);
      });

      return rs;
    }

    /// <summary>
    /// Returns shear result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="forceUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetElement2DResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<Element2DResult>> globalResults,
        ForcePerLengthUnit forceUnit, List<int> permutations)
    {
      ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0)
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count).ToList();
      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index => // loop through permutations
      {
        int permutationID = permutations[index];
        GsaResultsValues r = new GsaResultsValues();
        r.Type = GsaResultsValues.ResultType.Shear;

        Parallel.ForEach(globalResults.Keys, key =>
        {
          // lists for results
          ReadOnlyCollection<Element2DResult> results = globalResults[key];
          Element2DResult result = results[permutationID - 1];
          ReadOnlyCollection<Vector2> values = result.Shear;
          if (values.Count == 0) { return; }
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
        rs.TryAdd(permutationID, r);
      });

      return rs;
    }

    /// <summary>
    /// Returns force & moment result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="forceUnit"></param>
    /// <param name="momentUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetElement2DResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<Element2DResult>> globalResults,
        ForcePerLengthUnit forceUnit, ForceUnit momentUnit, List<int> permutations)
    {
      ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0)
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count).ToList();
      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index => // loop through permutations
      {
        int permutationID = permutations[index];
        GsaResultsValues r = new GsaResultsValues();
        r.Type = GsaResultsValues.ResultType.Force;

        Parallel.ForEach(globalResults.Keys, key =>
        {
          // lists for results
          ReadOnlyCollection<Element2DResult> results = globalResults[key];
          Element2DResult result = results[permutationID - 1];
          ReadOnlyCollection<Tensor2> forceValues = result.Force;
          ReadOnlyCollection<Tensor2> momentValues = result.Moment;
          if (forceValues.Count == 0) { return; }
          ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
          xyzRes.AsParallel().AsOrdered();
          ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
          xxyyzzRes.AsParallel().AsOrdered();

          // loop through the results
          Parallel.For(1, forceValues.Count, i =>
          {
            // add the values to the vector lists
            xyzRes.TryAdd(i, GetQuantityResult(forceValues[i], forceUnit));
            xxyyzzRes.TryAdd(i, GetQuantityResult(momentValues[i], momentUnit));
          });
          xyzRes.TryAdd(forceValues.Count, GetQuantityResult(forceValues[0], forceUnit)); // add centre point last
          xxyyzzRes.TryAdd(forceValues.Count, GetQuantityResult(momentValues[0], momentUnit)); // add centre point last

          // Wood-Armer moments as xyz (M*x) and xxyyzz (M*y)
          Parallel.ForEach(xxyyzzRes.Keys, i =>
          {
            xyzRes[i].XYZ = new Force(
                      xxyyzzRes[i].X.Value              // Mx
                      + Math.Sign(xxyyzzRes[i].X.Value) // + sign(Mx)
                      * Math.Abs(xxyyzzRes[i].Z.Value), // * abs(Mxy)
                      momentUnit);
            xxyyzzRes[i].XYZ = new Force(
                      xxyyzzRes[i].Y.Value              // Mx
                      + Math.Sign(xxyyzzRes[i].Y.Value) // + sign(Mx)
                      * Math.Abs(xxyyzzRes[i].Z.Value), // * abs(Mxy)
                      momentUnit);
          });

          // add the vector list to the out tree
          r.xyzResults.TryAdd(key, xyzRes);
          r.xxyyzzResults.TryAdd(key, xxyyzzRes);
        });
        r.UpdateMinMax();
        rs.TryAdd(permutationID, r);
      });

      return rs;
    }

    /// <summary>
    /// Returns displacement result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="resultLengthUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetElement2DResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<Element2DResult>> globalResults,
        LengthUnit resultLengthUnit, List<int> permutations)
    {
      ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0)
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count).ToList();
      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index => // loop through permutations
      {
        int permutationID = permutations[index];
        GsaResultsValues r = new GsaResultsValues();
        r.Type = GsaResultsValues.ResultType.Displacement;

        Parallel.ForEach(globalResults.Keys, key =>
        {
          // lists for results
          ReadOnlyCollection<Element2DResult> results = globalResults[key];
          Element2DResult result = results[permutationID - 1];
          ReadOnlyCollection<Double6> values = result.Displacement;
          if (values.Count == 0) { return; }
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
        rs.TryAdd(permutationID, r);
      });

      return rs;
    }

    /// <summary>
    /// Returns forces result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="forceUnit"></param>
    /// <param name="momentUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetElement1DResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<Element1DResult>> globalResults,
        ForceUnit forceUnit, MomentUnit momentUnit, List<int> permutations)
    {
      ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0)
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count).ToList();
      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index => // loop through permutations
      {
        int permutationID = permutations[index];
        GsaResultsValues r = new GsaResultsValues();
        r.Type = GsaResultsValues.ResultType.Force;

        Parallel.ForEach(globalResults.Keys, key =>
        {
          // lists for results
          ReadOnlyCollection<Element1DResult> results = globalResults[key];
          Element1DResult result = results[permutationID - 1];
          ReadOnlyCollection<Double6> values = result.Force;
          if (values.Count == 0) { return; }
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
        rs.TryAdd(permutationID, r);
      });

      return rs;
    }

    /// <summary>
    /// Returns strain energy density result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="energyUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetElement1DResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<Element1DResult>> globalResults,
        EnergyUnit energyUnit, List<int> permutations, bool average = false)
    {
      ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0)
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count).ToList();
      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index => // loop through permutations
      {
        int permutationID = permutations[index];
        GsaResultsValues r = new GsaResultsValues();
        r.Type = GsaResultsValues.ResultType.StrainEnergy;

        Parallel.ForEach(globalResults.Keys, key =>
        {
          // lists for results
          ReadOnlyCollection<Element1DResult> results = globalResults[key];
          Element1DResult result = results[permutationID - 1];
          ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
          xyzRes.AsParallel().AsOrdered();
          
          if (average)
          {
            xyzRes.TryAdd(0, GetQuantityResult(result.AverageStrainEnergyDensity, energyUnit));
            r.xyzResults.TryAdd(key, xyzRes);
          }
          else
          {
            ReadOnlyCollection<double> values = result.StrainEnergyDensity;
            if (values.Count == 0) { return; }

            // loop through the results
            Parallel.For(0, values.Count, i =>
            {
              // add the values to the vector lists
              xyzRes.TryAdd(i, GetQuantityResult(values[i], energyUnit));
            });
            // add the vector list to the out tree
            r.xyzResults.TryAdd(key, xyzRes);
          }
        });
        r.UpdateMinMax();
        rs.TryAdd(permutationID, r);
      });

      return rs;
    }

    /// <summary>
    /// Returns displacement result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="resultLengthUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetElement1DResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<Element1DResult>> globalResults,
        LengthUnit resultLengthUnit, List<int> permutations)
    {
      ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0)
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count).ToList();
      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index => // loop through permutations
      {
        int permutationID = permutations[index];
        GsaResultsValues r = new GsaResultsValues();
        r.Type = GsaResultsValues.ResultType.Displacement;

        Parallel.ForEach(globalResults.Keys, key =>
        {
          // lists for results
          ReadOnlyCollection<Element1DResult> results = globalResults[key];
          Element1DResult result = results[permutationID - 1];
          ReadOnlyCollection<Double6> values = result.Displacement;
          if (values.Count == 0) { return; }
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
        rs.TryAdd(permutationID, r);
      });

      return rs;
    }
    /// <summary>
    /// Returns displacement result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="resultLengthUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetNodeResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> globalResults,
        LengthUnit resultLengthUnit, List<int> permutations)
    {
      ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0)
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count).ToList();
      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index =>
      {
        int permutationID = permutations[index];
        GsaResultsValues r = new GsaResultsValues();
        r.Type = GsaResultsValues.ResultType.Displacement;

        Parallel.ForEach(globalResults.Keys, nodeID =>
        {
          ReadOnlyCollection<NodeResult> results = globalResults[nodeID];
          NodeResult result = results[permutationID - 1];
          Double6 values = result.Displacement;
          
          ConcurrentDictionary<int, GsaResultQuantity> xyz = new ConcurrentDictionary<int, GsaResultQuantity>();
          xyz.TryAdd(0, GetQuantityResult(values, resultLengthUnit));
          r.xyzResults.TryAdd(nodeID, xyz);
          ConcurrentDictionary<int, GsaResultQuantity> xxyyzz = new ConcurrentDictionary<int, GsaResultQuantity>();
          xxyyzz.TryAdd(0, GetQuantityResult(values, AngleUnit.Radian));
          r.xxyyzzResults.TryAdd(nodeID, xxyyzz);
        });

        r.UpdateMinMax();
        rs.TryAdd(permutationID, r);
      });

      return rs;
    }
    /// <summary>
    /// Returns reaction forces result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="forceUnit"></param>
    /// <param name="momentUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <param name="supportnodeIDs">bag of support node IDs, if input contains ids then this method will test all nodes and include results for these IDs even if they are all zero</param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetNodeReactionForceResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> globalResults,
        ForceUnit forceUnit, MomentUnit momentUnit, List<int> permutations, ConcurrentBag<int> supportnodeIDs = null)
    {
      ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0)
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count).ToList();
      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index =>
      {
        int permutationID = permutations[index];
        GsaResultsValues r = new GsaResultsValues();
        r.Type = GsaResultsValues.ResultType.Force;

        Parallel.ForEach(globalResults.Keys, nodeID =>
        {
          ReadOnlyCollection<NodeResult> results = globalResults[nodeID];
          NodeResult result = results[permutationID - 1];
          Double6 values = result.Reaction;
          
          if (supportnodeIDs != null && !supportnodeIDs.Contains(nodeID))
          {
            if (values.X == 0 & values.Y == 0 & values.Z == 0
            & values.XX == 0 & values.YY == 0 & values.ZZ == 0)
            { return ; }
          }

          ConcurrentDictionary<int, GsaResultQuantity> xyz = new ConcurrentDictionary<int, GsaResultQuantity>();
          xyz.TryAdd(0, GetQuantityResult(values, forceUnit));
          r.xyzResults.TryAdd(nodeID, xyz);
          ConcurrentDictionary<int, GsaResultQuantity> xxyyzz = new ConcurrentDictionary<int, GsaResultQuantity>();
          xxyyzz.TryAdd(0, GetQuantityResult(values, momentUnit));
          r.xxyyzzResults.TryAdd(nodeID, xxyyzz);
        });

        r.UpdateMinMax();
        rs.TryAdd(permutationID, r);
      });

      return rs;
    }
    /// <summary>
    /// Returns spring reaction forces result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="forceUnit"></param>
    /// <param name="momentUnit"></param>
    /// <param name="permutations">list of permutations, input an empty list to get all permutations</param>
    /// <param name="supportnodeIDs">bag of support node IDs, if input contains ids then this method will test all nodes and include results for these IDs even if they are all zero</param>
    /// <returns></returns>
    internal static ConcurrentDictionary<int, GsaResultsValues> GetNodeSpringForceResultValues(ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> globalResults,
        ForceUnit forceUnit, MomentUnit momentUnit, List<int> permutations, ConcurrentBag<int> supportnodeIDs = null)
    {
      ConcurrentDictionary<int, GsaResultsValues> rs = new ConcurrentDictionary<int, GsaResultsValues>();

      if (permutations.Count == 0)
        permutations = Enumerable.Range(1, globalResults[globalResults.Keys.First()].Count).ToList();
      int permutationCount = permutations.Count;

      Parallel.For(0, permutationCount, index =>
      {
        int permutationID = permutations[index];
        GsaResultsValues r = new GsaResultsValues();
        r.Type = GsaResultsValues.ResultType.Force;

        Parallel.ForEach(globalResults.Keys, nodeID =>
        {
          ReadOnlyCollection<NodeResult> results = globalResults[nodeID];
          NodeResult result = results[permutationID - 1];
          Double6 values = result.SpringForce;

          if (supportnodeIDs != null && !supportnodeIDs.Contains(nodeID))
          {
            if (values.X == 0 & values.Y == 0 & values.Z == 0
            & values.XX == 0 & values.YY == 0 & values.ZZ == 0)
            { return; }
          }

          ConcurrentDictionary<int, GsaResultQuantity> xyz = new ConcurrentDictionary<int, GsaResultQuantity>();
          xyz.TryAdd(0, GetQuantityResult(values, forceUnit));
          r.xyzResults.TryAdd(nodeID, xyz);
          ConcurrentDictionary<int, GsaResultQuantity> xxyyzz = new ConcurrentDictionary<int, GsaResultQuantity>();
          xxyyzz.TryAdd(0, GetQuantityResult(values, momentUnit));
          r.xxyyzzResults.TryAdd(nodeID, xxyyzz);
        });

        r.UpdateMinMax();
        rs.TryAdd(permutationID, r);
      });

      return rs;
    }
  }
}
