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

namespace GsaGH.Helpers.GsaAPI
{
  internal partial class ResultHelper
  {
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

        ReadOnlyCollection<Double3> trans_vals = elementResults.Displacement;
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

        ReadOnlyCollection<Tensor3> stress_vals = elementResults.Stress;
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

        ReadOnlyCollection<Tensor3> stresses = elementResults.Stress;
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

        ReadOnlyCollection<Vector2> shears = elementResults.Shear;
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

        ReadOnlyCollection<Tensor2> forces = elementResults.Force;
        ReadOnlyCollection<Tensor2> moments = elementResults.Moment;
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

        ReadOnlyCollection<Double6> disp = elementResults.Displacement;
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
        ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xyzRes.AsParallel().AsOrdered();
        ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xxyyzzRes.AsParallel().AsOrdered();

        ReadOnlyCollection<Double6> values = elementResults.Force;

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
    /// Returns strain energy density result values
    /// </summary>
    /// <param name="globalResults"></param>
    /// <param name="energyUnit"></param>
    /// <returns></returns>
    internal static GsaResultsValues GetElement1DResultValues(ReadOnlyDictionary<int, Element1DResult> globalResults,
        EnergyUnit energyUnit, bool average = false)
    {
      GsaResultsValues r = new GsaResultsValues();
      r.Type = GsaResultsValues.ResultType.StrainEnergy;

      Parallel.ForEach(globalResults.Keys, key =>
      {
        // lists for results
        Element1DResult elementResults = globalResults[key];
        ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xyzRes.AsParallel().AsOrdered();

        if (average)
        {
          xyzRes[0] = GetQuantityResult(elementResults.AverageStrainEnergyDensity, energyUnit);
          r.xyzResults.TryAdd(key, xyzRes);
        }
        else
        {
          ReadOnlyCollection<double> values = elementResults.StrainEnergyDensity;

          // loop through the results
          Parallel.For(0, values.Count, i =>
          {
            // add the values to the vector lists
            xyzRes[i] = GetQuantityResult(values[i], energyUnit);
          });
          // add the vector list to the out tree
          r.xyzResults.TryAdd(key, xyzRes);
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
        LengthUnit resultLengthUnit)
    {
      GsaResultsValues r = new GsaResultsValues();
      r.Type = GsaResultsValues.ResultType.Displacement;

      Parallel.ForEach(globalResults.Keys, key =>
      {
        // lists for results
        Element1DResult elementResults = globalResults[key];
        ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xyzRes.AsParallel().AsOrdered();
        ConcurrentDictionary<int, GsaResultQuantity> xxyyzzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        xxyyzzRes.AsParallel().AsOrdered();

        // set the result type dependent on user selection in dropdown
        ReadOnlyCollection<Double6> values = elementResults.Displacement;

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
    internal static GsaResultsValues GetNodeReactionForceResultValues(ReadOnlyDictionary<int, NodeResult> globalResults,
        ForceUnit forceUnit, MomentUnit momentUnit, ConcurrentBag<int> supportnodeIDs = null)
    {
      GsaResultsValues r = new GsaResultsValues();
      r.Type = GsaResultsValues.ResultType.Force;

      Parallel.ForEach(globalResults.Keys, nodeID =>
      {
        NodeResult result = globalResults[nodeID];
        Double6 values = result.Reaction;

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
        ForceUnit forceUnit, MomentUnit momentUnit, ConcurrentBag<int> supportnodeIDs = null)
    {
      GsaResultsValues r = new GsaResultsValues();
      r.Type = GsaResultsValues.ResultType.Force;

      Parallel.ForEach(globalResults.Keys, nodeID =>
      {
        NodeResult result = globalResults[nodeID];
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
      return r;
    }
  }
}
