using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;
using GsaGH.Util.Gsa;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters
{
  internal class GsaResultQuantity
  {
    internal IQuantity X { get; set; }
    internal IQuantity Y { get; set; }
    internal IQuantity Z { get; set; }
    internal IQuantity XYZ { get; set; }
    internal GsaResultQuantity()
    { }
  }
  internal class GsaResultsValues
  {
    internal enum ResultType
    {
      Displacement,
      Force,
      Stress,
      Shear,
      StrainEnergy
    }
    internal ResultType Type { get; set; }
    internal void UpdateMinMax()
    {
      if (xyzResults.Count > 0)
      {
        // update max and min values
        dmax_x = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Max()).Max();
        dmax_y = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Max()).Max();
        try { dmax_z = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z).Max()).Max(); } catch (Exception) { } // shear does not set this value
        try { dmax_xyz = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.XYZ).Max()).Max(); } catch (Exception) { } // resultant may not always be computed
        dmin_x = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Min()).Min();
        dmin_y = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Min()).Min();
        try { dmin_z = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z).Min()).Min(); } catch (Exception) { } // shear does not set this value
        try { dmin_xyz = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.XYZ).Min()).Min(); } catch (Exception) { } // resultant may not always be computed
      }
      if (xxyyzzResults.Count > 0)
      {
        try // some cases doesnt compute xxyyzz results at all
        {
          dmax_xx = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Max()).Max();
          dmax_yy = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Max()).Max();
          dmax_zz = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z).Max()).Max();
        }
        catch (Exception) { }
        try { dmax_xxyyzz = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.XYZ).Max()).Max(); } catch (Exception) { } // resultant may not always be computed
        try // some cases doesnt compute xxyyzz results at all
        {
          dmin_xx = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Min()).Min();
          dmin_yy = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Min()).Min();
          dmin_zz = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z).Min()).Min();
        }
        catch (Exception) { }

        try { dmin_xxyyzz = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.XYZ).Min()).Min(); } catch (Exception) { } // resultant may not always be computed
      }
    }
    /// <summary>
    /// Translation, forces, etc results
    /// dictionary< key = node/elementID, value = dictionary< key = position on element, value = value>>
    /// </summary>
    internal ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xyzResults { get; set; } = new ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>>();
    internal ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xxyyzzResults { get; set; } = new ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>>();
    internal IQuantity dmax_x { get; set; }
    internal IQuantity dmax_y { get; set; }
    internal IQuantity dmax_z { get; set; }
    internal IQuantity dmax_xx { get; set; }
    internal IQuantity dmax_yy { get; set; }
    internal IQuantity dmax_zz { get; set; }
    internal IQuantity dmax_xyz { get; set; }
    internal IQuantity dmax_xxyyzz { get; set; }
    internal IQuantity dmin_x { get; set; }
    internal IQuantity dmin_y { get; set; }
    internal IQuantity dmin_z { get; set; }
    internal IQuantity dmin_xx { get; set; }
    internal IQuantity dmin_yy { get; set; }
    internal IQuantity dmin_zz { get; set; }
    internal IQuantity dmin_xyz { get; set; }
    internal IQuantity dmin_xxyyzz { get; set; }

    internal GsaResultsValues()
    { }

  }
  public class GsaResult
  {
    #region analysiscase members
    /// <summary>
    /// Analysis Case API Result
    /// </summary>
    internal AnalysisCaseResult AnalysisCaseResult { get; set; }

    /// <summary>
    /// Analysis Case 3DElement API Result Dictionary 
    /// Append to this dictionary to chache results
    /// key = elementList
    /// </summary>
    internal Dictionary<string, ReadOnlyDictionary<int, Element3DResult>> ACaseElement3DResults { get; set; } = new Dictionary<string, ReadOnlyDictionary<int, Element3DResult>>();

    /// <summary>
    /// Analysis Case 3DElement Result VALUES Dictionary 
    /// Append to this dictionary to chache results
    /// key = elementList
    /// </summary>
    internal Dictionary<string, GsaResultsValues> ACaseElement3DDisplacementValues { get; set; } = new Dictionary<string, GsaResultsValues>();

    /// <summary>
    /// Analysis Case 3DElement Result VALUES Dictionary 
    /// Append to this dictionary to chache results
    /// key = elementList
    /// </summary>
    internal Dictionary<string, GsaResultsValues> ACaseElement3DStressValues { get; set; } = new Dictionary<string, GsaResultsValues>();

    /// <summary>
    /// Analysis Case 2DElement API Result Dictionary 
    /// Append to this dictionary to chache results
    /// key = tuple<elementList, layer>
    /// </summary>
    internal Dictionary<Tuple<string, double>, ReadOnlyDictionary<int, Element2DResult>> ACaseElement2DResults { get; set; } = new Dictionary<Tuple<string, double>, ReadOnlyDictionary<int, Element2DResult>>();

    /// <summary>
    /// Analysis Case 2DElement Displacement Result VALUES Dictionary 
    /// Append to this dictionary to chache results
    /// key = elementList
    /// </summary>
    internal Dictionary<string, GsaResultsValues> ACaseElement2DDisplacementValues { get; set; } = new Dictionary<string, GsaResultsValues>();

    /// <summary>
    /// Analysis Case 2DElement Force Result VALUES Dictionary 
    /// Append to this dictionary to chache results
    /// key = elementList
    /// </summary>
    internal Dictionary<string, GsaResultsValues> ACaseElement2DForceValues { get; set; } = new Dictionary<string, GsaResultsValues>();

    /// <summary>
    /// Analysis Case 2DElement Shear Result VALUES Dictionary 
    /// Append to this dictionary to chache results
    /// key = elementList
    /// </summary>
    internal Dictionary<string, GsaResultsValues> ACaseElement2DShearValues { get; set; } = new Dictionary<string, GsaResultsValues>();

    /// <summary>
    /// Analysis Case 2DElement Stress Result VALUES Dictionary 
    /// Append to this dictionary to chache results
    /// key = tuple<elementList, layer>
    /// </summary>
    internal Dictionary<Tuple<string, double>, GsaResultsValues> ACaseElement2DStressValues { get; set; } = new Dictionary<Tuple<string, double>, GsaResultsValues>();

    /// <summary>
    /// Analysis Case 1DElement API Result Dictionary 
    /// Append to this dictionary to chache results
    /// key = Tuple<elementList, numberOfDivisions>
    /// </summary>
    internal Dictionary<Tuple<string, int>, ReadOnlyDictionary<int, Element1DResult>> ACaseElement1DResults { get; set; } = new Dictionary<Tuple<string, int>, ReadOnlyDictionary<int, Element1DResult>>();

    /// <summary>
    /// Analysis Case 1DElement Displacement Result VALUES Dictionary 
    /// Append to this dictionary to chache results
    /// key = Tuple<elementList, numberOfDivisions>
    /// </summary>
    internal Dictionary<Tuple<string, int>, GsaResultsValues> ACaseElement1DDisplacementValues { get; set; } = new Dictionary<Tuple<string, int>, GsaResultsValues>();

    /// <summary>
    /// Analysis Case 1DElement Force Result VALUES Dictionary 
    /// Append to this dictionary to chache results
    /// key = Tuple<elementList, numberOfDivisions>
    /// </summary>
    internal Dictionary<Tuple<string, int>, GsaResultsValues> ACaseElement1DForceValues { get; set; } = new Dictionary<Tuple<string, int>, GsaResultsValues>();

    /// <summary>
    /// Analysis Case Node API Result Dictionary 
    /// Append to this dictionary to chache results
    /// key = elementList
    /// </summary>
    internal Dictionary<string, ReadOnlyDictionary<int, NodeResult>> ACaseNodeResults { get; set; } = new Dictionary<string, ReadOnlyDictionary<int, NodeResult>>();

    /// <summary>
    /// Analysis Case Node Displacement Result VALUES Dictionary 
    /// Append to this dictionary to chache results
    /// key = elementList
    /// </summary>
    internal Dictionary<string, GsaResultsValues> ACaseNodeDisplacementValues { get; set; } = new Dictionary<string, GsaResultsValues>();

    /// <summary>
    /// Analysis Case Node Reaction Force Result VALUES Dictionary 
    /// Append to this dictionary to chache results
    /// key = elementList
    /// </summary>
    internal Dictionary<string, GsaResultsValues> ACaseNodeReactionForceValues { get; set; } = new Dictionary<string, GsaResultsValues>();

    #endregion

    #region combination members
    /// <summary>
    /// Combination Case API Result
    /// </summary>
    internal CombinationCaseResult CombinationCaseResult { get; set; }

    /// <summary>
    /// User set permutation ID. If -1 => return all.
    /// </summary>
    internal List<int> SelectedPermutationIDs { get; set; }

    /// <summary>
    /// Combination Case 3DElement API Result Dictionary 
    /// Append to this dictionary to chache results
    /// key = elementList
    /// value = <elementID, collection<permutationResult>
    /// </summary>
    internal Dictionary<string, ReadOnlyDictionary<int, ReadOnlyCollection<Element3DResult>>> ComboElement3DResults { get; set; } = new Dictionary<string, ReadOnlyDictionary<int, ReadOnlyCollection<Element3DResult>>>();

    /// <summary>
    /// Combination Case 3DElement Displacement Result VALUES Dictionary 
    /// Append to this dictionary to chache results
    /// key = elementList
    /// value = Dictionary<elementID, Dictionary<permutationID, permutationsResults>>
    /// </summary>
    internal Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>> ComboElement3DDisplacementValues { get; set; } = new Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>();

    /// <summary>
    /// Combination Case 3DElement Stress Result VALUES Dictionary 
    /// Append to this dictionary to chache results
    /// key = elementList
    /// value = Dictionary<elementID, Dictionary<permutationID, permutationsResults>>
    /// </summary>
    internal Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>> ComboElement3DStressValues { get; set; } = new Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>();

    /// <summary>
    /// Combination Case 2DElement API Result Dictionary 
    /// Append to this dictionary to chache results
    /// key = Tuple<elementList, layer>
    /// value = Dictionary<elementID, Dictionary<permutationID, permutationsResults>>
    /// </summary>
    internal Dictionary<Tuple<string, double>, ReadOnlyDictionary<int, ReadOnlyCollection<Element2DResult>>> ComboElement2DResults { get; set; } = new Dictionary<Tuple<string, double>, ReadOnlyDictionary<int, ReadOnlyCollection<Element2DResult>>>();

    /// <summary>
    /// Combination Case 2DElement Displacement Result VALUES Dictionary 
    /// Append to this dictionary to chache results
    /// key = elementList
    /// value = <elementID, collection<permutationResult>
    /// </summary>
    internal Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>> ComboElement2DDisplacementValues { get; set; } = new Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>();

    /// <summary>
    /// Combination Case 2DElement Force/Moment Result VALUES Dictionary 
    /// Append to this dictionary to chache results
    /// key = elementList
    /// value = Dictionary<elementID, Dictionary<permutationID, permutationsResults>>
    /// </summary>
    internal Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>> ComboElement2DForceValues { get; set; } = new Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>();

    /// <summary>
    /// Combination Case 2DElement Stress Result VALUES Dictionary 
    /// Append to this dictionary to chache results
    /// key = tuple<elementList, layer>
    /// value = Dictionary<elementID, Dictionary<permutationID, permutationsResults>>
    /// </summary>
    internal Dictionary<Tuple<string, double>, ConcurrentDictionary<int, GsaResultsValues>> ComboElement2DStressValues { get; set; } = new Dictionary<Tuple<string, double>, ConcurrentDictionary<int, GsaResultsValues>>();

    /// <summary>
    /// Combination Case 2DElement Result VALUES Dictionary 
    /// Append to this dictionary to chache results
    /// key = elementList
    /// value = Dictionary<elementID, Dictionary<numberOfDivisions, results>>
    /// </summary>
    internal Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>> ComboElement2DShearValues { get; set; } = new Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>();

    /// <summary>
    /// Combination Case 1DElement API Result Dictionary 
    /// Append to this dictionary to chache results
    /// key = Tuple<elementList, permutations>
    /// value = Dictionary<elementID, Dictionary<numberOfDivisions, results>>
    /// </summary>
    internal Dictionary<Tuple<string, int>, ReadOnlyDictionary<int, ReadOnlyCollection<Element1DResult>>> ComboElement1DResults { get; set; } = new Dictionary<Tuple<string, int>, ReadOnlyDictionary<int, ReadOnlyCollection<Element1DResult>>>();

    /// <summary>
    /// Combination Case 1DElement Forces Result VALUES Dictionary 
    /// Append to this dictionary to chache results
    /// key = Tuple<elementList, permutations>
    /// value = Dictionary<elementID, Dictionary<numberOfDivisions, results>>
    /// </summary>
    internal Dictionary<Tuple<string, int>, ConcurrentDictionary<int, GsaResultsValues>> ComboElement1DForceValues { get; set; } = new Dictionary<Tuple<string, int>, ConcurrentDictionary<int, GsaResultsValues>>();

    /// <summary>
    /// Combination Case 1DElement Displacement Result VALUES Dictionary 
    /// Append to this dictionary to chache results
    /// key = Tuple<elementList, permutations>
    /// value = Dictionary<elementID, Dictionary<numberOfDivisions, results>>
    /// </summary>
    internal Dictionary<Tuple<string, int>, ConcurrentDictionary<int, GsaResultsValues>> ComboElement1DDisplacementValues { get; set; } = new Dictionary<Tuple<string, int>, ConcurrentDictionary<int, GsaResultsValues>>();

    /// <summary>
    /// Combination Case Node API Result Dictionary 
    /// Append to this dictionary to chache results
    /// key = elementList
    /// value = <elementID, collection<permutationResult>
    /// </summary>
    internal Dictionary<string, ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>>> ComboNodeResults { get; set; } = new Dictionary<string, ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>>>();

    /// <summary>
    /// Combination Case Node Displacement Result VALUES Dictionary 
    /// Append to this dictionary to chache results
    /// key = elementList
    /// value = Dictionary<permutationID, permutationsResults>>
    /// </summary>
    internal Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>> ComboNodeDisplacementValues { get; set; } = new Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>();

    /// <summary>
    /// Combination Case Node Reaction Force Result VALUES Dictionary 
    /// Append to this dictionary to chache results
    /// key = elementList
    /// value = Dictionary<permutationID, permutationsResults>>
    /// </summary>
    internal Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>> ComboNodeReactionForceValues { get; set; } = new Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>();

    #endregion

    internal int CaseID { get; set; }
    internal string CaseName { get; set; }
    public enum ResultType
    {
      AnalysisCase,
      Combination
    }
    internal ResultType Type { get; set; }
    internal Model Model { get; set; }
    public GsaResult()
    { }
    internal GsaResult(Model model, AnalysisCaseResult result, int caseID)
    {
      this.Model = model;
      this.AnalysisCaseResult = result;
      this.Type = ResultType.AnalysisCase;
      this.CaseID = caseID;
      this.CaseName = model.AnalysisCaseName(this.CaseID);
    }
    internal GsaResult(Model model, CombinationCaseResult result, int caseID, List<int> permutations)
    {
      this.Model = model;
      this.CombinationCaseResult = result;
      this.Type = ResultType.Combination;
      this.CaseID = caseID;
      this.SelectedPermutationIDs = permutations.OrderBy(x => x).ToList();
    }

    #region properties
    public bool IsValid
    {
      get
      {
        return true;
      }
    }
    #endregion

    #region output methods
    /// <summary>
    /// Get node displacement values 
    /// For analysis case the length of the list will be 1
    /// This method will use cache data if it exists
    /// </summary>
    /// <param name="nodelist"></param>
    /// <param name="lengthUnit"></param>
    /// <returns></returns>
    internal Tuple<List<GsaResultsValues>, List<int>> NodeDisplacementValues(string nodelist, LengthUnit lengthUnit)
    {
      if (this.Type == ResultType.AnalysisCase)
      {
        if (!this.ACaseNodeDisplacementValues.ContainsKey(nodelist)) // see if values exist
        {
          if (!this.ACaseNodeResults.ContainsKey(nodelist)) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ACaseNodeResults.Add(nodelist, AnalysisCaseResult.NodeResults(nodelist));
          }
          // compute result values and add to dictionary for cache
          this.ACaseNodeDisplacementValues.Add(nodelist,
              ResultHelper.GetNodeResultValues(ACaseNodeResults[nodelist], lengthUnit));
        }
        return new Tuple<List<GsaResultsValues>, List<int>>(new List<GsaResultsValues> { ACaseNodeDisplacementValues[nodelist] }, Model.Nodes(nodelist).Keys.ToList());
      }
      else
      {
        if (!this.ComboNodeDisplacementValues.ContainsKey(nodelist)) // see if values exist
        {
          if (!this.ComboNodeResults.ContainsKey(nodelist)) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ComboNodeResults.Add(nodelist, CombinationCaseResult.NodeResults(nodelist));
          }
          // compute result values and add to dictionary for cache
          this.ComboNodeDisplacementValues.Add(nodelist,
              ResultHelper.GetNodeResultValues(ComboNodeResults[nodelist], lengthUnit, SelectedPermutationIDs));
        }
        return new Tuple<List<GsaResultsValues>, List<int>>(
          new List<GsaResultsValues>(ComboNodeDisplacementValues[nodelist].Values), Model.Nodes(nodelist).Keys.ToList());
      }
    }

    /// <summary>
    /// Get node displacement values 
    /// For analysis case the length of the list will be 1
    /// This method will use cache data if it exists
    /// </summary>
    /// <param name="nodelist"></param>
    /// <param name="lengthUnit"></param>
    /// <returns></returns>
    internal Tuple<List<GsaResultsValues>, List<int>> NodeReactionForceValues(string nodelist, ForceUnit forceUnit, MomentUnit momentUnit)
    {
      // get list of support nodes
      ConcurrentBag<int> supportnodeIDs = null;
      if (nodelist.ToLower() == "all" | nodelist == "")
      {
        supportnodeIDs = new ConcurrentBag<int>();
        ReadOnlyDictionary<int, Node> nodes = Model.Nodes();
        Parallel.ForEach(nodes, node =>
        {
          NodalRestraint rest = node.Value.Restraint;
          if (rest.X || rest.Y || rest.Z || rest.XX || rest.YY || rest.ZZ)
            supportnodeIDs.Add(node.Key);
        });
        nodelist = "All";
      }

      if (this.Type == ResultType.AnalysisCase)
      {
        if (!this.ACaseNodeReactionForceValues.ContainsKey(nodelist)) // see if values exist
        {
          if (!this.ACaseNodeResults.ContainsKey(nodelist)) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ACaseNodeResults.Add(nodelist, AnalysisCaseResult.NodeResults(nodelist));
          }
          // compute result values and add to dictionary for cache
          this.ACaseNodeReactionForceValues.Add(nodelist,
              ResultHelper.GetNodeReactionForceResultValues(ACaseNodeResults[nodelist], forceUnit, momentUnit, supportnodeIDs));
        }
        return new Tuple<List<GsaResultsValues>, List<int>>(
            new List<GsaResultsValues> { ACaseNodeReactionForceValues[nodelist] }, ACaseNodeReactionForceValues[nodelist].xyzResults.Keys.OrderBy(x => x).ToList());
      }
      else
      {
        if (!this.ComboNodeReactionForceValues.ContainsKey(nodelist)) // see if values exist
        {
          if (!this.ComboNodeResults.ContainsKey(nodelist)) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ComboNodeResults.Add(nodelist, CombinationCaseResult.NodeResults(nodelist));
          }
          // compute result values and add to dictionary for cache
          this.ComboNodeReactionForceValues.Add(nodelist,
              ResultHelper.GetNodeReactionForceResultValues(ComboNodeResults[nodelist], forceUnit, momentUnit, SelectedPermutationIDs, supportnodeIDs));
        }
        return new Tuple<List<GsaResultsValues>, List<int>>(
            new List<GsaResultsValues>(ComboNodeReactionForceValues[nodelist].Values), ComboNodeReactionForceValues[nodelist].Values.First().xyzResults.Keys.OrderBy(x => x).ToList());
      }
    }

    /// <summary>
    /// Get node displacement values 
    /// For analysis case the length of the list will be 1
    /// This method will use cache data if it exists
    /// </summary>
    /// <param name="nodelist"></param>
    /// <param name="lengthUnit"></param>
    /// <returns></returns>
    internal Tuple<List<GsaResultsValues>, List<int>> SpringReactionForceValues(string nodelist, ForceUnit forceUnit, MomentUnit momentUnit)
    {
      // get list of support nodes
      ConcurrentBag<int> supportnodeIDs = null;
      if (nodelist.ToLower() == "all" | nodelist == "")
      {
        supportnodeIDs = new ConcurrentBag<int>();
        ReadOnlyDictionary<int, Node> nodes = Model.Nodes();
        Parallel.ForEach(nodes, node =>
        {
          NodalRestraint rest = node.Value.Restraint;
          if (rest.X || rest.Y || rest.Z || rest.XX || rest.YY || rest.ZZ)
            supportnodeIDs.Add(node.Key);
        });
        nodelist = "All";
      }

      if (this.Type == ResultType.AnalysisCase)
      {
        if (!this.ACaseNodeReactionForceValues.ContainsKey(nodelist)) // see if values exist
        {
          if (!this.ACaseNodeResults.ContainsKey(nodelist)) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ACaseNodeResults.Add(nodelist, AnalysisCaseResult.NodeResults(nodelist));
          }
          // compute result values and add to dictionary for cache
          this.ACaseNodeReactionForceValues.Add(nodelist,
              ResultHelper.GetNodeSpringForceResultValues(ACaseNodeResults[nodelist], forceUnit, momentUnit, supportnodeIDs));
        }
        return new Tuple<List<GsaResultsValues>, List<int>>(
            new List<GsaResultsValues> { ACaseNodeReactionForceValues[nodelist] }, ACaseNodeReactionForceValues[nodelist].xyzResults.Keys.OrderBy(x => x).ToList());
      }
      else
      {
        if (!this.ComboNodeReactionForceValues.ContainsKey(nodelist)) // see if values exist
        {
          if (!this.ComboNodeResults.ContainsKey(nodelist)) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ComboNodeResults.Add(nodelist, CombinationCaseResult.NodeResults(nodelist));
          }
          // compute result values and add to dictionary for cache
          this.ComboNodeReactionForceValues.Add(nodelist,
              ResultHelper.GetNodeSpringForceResultValues(ComboNodeResults[nodelist], forceUnit, momentUnit, SelectedPermutationIDs, supportnodeIDs));
        }
        return new Tuple<List<GsaResultsValues>, List<int>>(
            new List<GsaResultsValues>(ComboNodeReactionForceValues[nodelist].Values), ComboNodeReactionForceValues[nodelist].Values.First().xyzResults.Keys.OrderBy(x => x).ToList());
      }
    }

    /// <summary>
    /// Get beam displacement values 
    /// For analysis case the length of the list will be 1
    /// This method will use cache data if it exists
    /// </summary>
    /// <param name="elementlist"></param>
    /// <param name="lengthUnit"></param>
    /// <returns></returns>
    internal List<GsaResultsValues> Element1DDisplacementValues(string elementlist, int positionsCount, LengthUnit lengthUnit)
    {
      Tuple<string, int> key = new Tuple<string, int>(elementlist, positionsCount);
      if (this.Type == ResultType.AnalysisCase)
      {
        if (!this.ACaseElement1DDisplacementValues.ContainsKey(key)) // see if values exist
        {
          if (!this.ACaseElement1DResults.ContainsKey(key)) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ACaseElement1DResults.Add(key, AnalysisCaseResult.Element1DResults(elementlist, positionsCount));
          }
          // compute result values and add to dictionary for cache
          this.ACaseElement1DDisplacementValues.Add(key,
              ResultHelper.GetElement1DResultValues(ACaseElement1DResults[key], lengthUnit));
        }
        return new List<GsaResultsValues>() { ACaseElement1DDisplacementValues[key] };
      }
      else
      {
        if (!this.ComboElement1DDisplacementValues.ContainsKey(key)) // see if values exist
        {
          if (!this.ComboElement1DResults.ContainsKey(key)) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ComboElement1DResults.Add(key, CombinationCaseResult.Element1DResults(elementlist, positionsCount, false));
          }
          // compute result values and add to dictionary for cache
          this.ComboElement1DDisplacementValues.Add(key,
              ResultHelper.GetElement1DResultValues(ComboElement1DResults[key], lengthUnit, SelectedPermutationIDs));
        }
        return new List<GsaResultsValues>(ComboElement1DDisplacementValues[key].Values);
      }
    }

    /// <summary>
    /// Get beam force values 
    /// For analysis case the length of the list will be 1
    /// This method will use cache data if it exists
    /// </summary>
    /// <param name="elementlist"></param>
    /// <param name="forceUnit"></param>
    /// /// <param name="momentUnit"></param>
    /// <returns></returns>
    internal List<GsaResultsValues> Element1DForceValues(string elementlist, int positionsCount, ForceUnit forceUnit, MomentUnit momentUnit)
    {
      Tuple<string, int> key = new Tuple<string, int>(elementlist, positionsCount);
      if (this.Type == ResultType.AnalysisCase)
      {
        if (!this.ACaseElement1DForceValues.ContainsKey(key)) // see if values exist
        {
          if (!this.ACaseElement1DResults.ContainsKey(key)) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ACaseElement1DResults.Add(key, AnalysisCaseResult.Element1DResults(elementlist, positionsCount));
          }
          // compute result values and add to dictionary for cache
          this.ACaseElement1DForceValues.Add(key,
              ResultHelper.GetElement1DResultValues(ACaseElement1DResults[key], forceUnit, momentUnit));
        }
        return new List<GsaResultsValues>() { ACaseElement1DForceValues[key] };
      }
      else
      {
        if (!this.ComboElement1DForceValues.ContainsKey(key)) // see if values exist
        {
          if (!this.ComboElement1DResults.ContainsKey(key)) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ComboElement1DResults.Add(key, CombinationCaseResult.Element1DResults(elementlist, positionsCount, false));
          }
          // compute result values and add to dictionary for cache
          this.ComboElement1DForceValues.Add(key,
              ResultHelper.GetElement1DResultValues(ComboElement1DResults[key], forceUnit, momentUnit, SelectedPermutationIDs));
        }
        return new List<GsaResultsValues>(ComboElement1DForceValues[key].Values);
      }
    }

    /// <summary>
    /// Get beam strain energy density values 
    /// For analysis case the length of the list will be 1
    /// This method will use cache data if it exists
    /// </summary>
    /// <param name="elementlist"></param>
    /// <param name="energyUnit"></param>
    /// <returns></returns>
    internal List<GsaResultsValues> Element1DStrainEnergyDensityValues(string elementlist, int positionsCount, EnergyUnit energyUnit)
    {
      Tuple<string, int> key = new Tuple<string, int>(elementlist, positionsCount);
      if (this.Type == ResultType.AnalysisCase)
      {
        if (!this.ACaseElement1DForceValues.ContainsKey(key)) // see if values exist
        {
          if (!this.ACaseElement1DResults.ContainsKey(key)) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ACaseElement1DResults.Add(key, AnalysisCaseResult.Element1DResults(elementlist, positionsCount));
          }
          // compute result values and add to dictionary for cache
          this.ACaseElement1DForceValues.Add(key,
              ResultHelper.GetElement1DResultValues(ACaseElement1DResults[key], energyUnit));
        }
        return new List<GsaResultsValues>() { ACaseElement1DForceValues[key] };
      }
      else
      {
        if (!this.ComboElement1DForceValues.ContainsKey(key)) // see if values exist
        {
          if (!this.ComboElement1DResults.ContainsKey(key)) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ComboElement1DResults.Add(key, CombinationCaseResult.Element1DResults(elementlist, positionsCount, true));
          }
          // compute result values and add to dictionary for cache
          this.ComboElement1DForceValues.Add(key,
              ResultHelper.GetElement1DResultValues(ComboElement1DResults[key], energyUnit, SelectedPermutationIDs));
        }
        return new List<GsaResultsValues>(ComboElement1DForceValues[key].Values);
      }
    }

    /// <summary>
    /// Get beam average strain energy density values 
    /// For analysis case the length of the list will be 1
    /// This method will use cache data if it exists
    /// </summary>
    /// <param name="elementlist"></param>
    /// <param name="energyUnit"></param>
    /// <returns></returns>
    internal List<GsaResultsValues> Element1DStrainEnergyDensityValues(string elementlist, EnergyUnit energyUnit)
    {
      Tuple<string, int> key = new Tuple<string, int>(elementlist, 1);
      if (this.Type == ResultType.AnalysisCase)
      {
        if (!this.ACaseElement1DForceValues.ContainsKey(key)) // see if values exist
        {
          if (!this.ACaseElement1DResults.ContainsKey(key)) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ACaseElement1DResults.Add(key, AnalysisCaseResult.Element1DResults(elementlist, 1));
          }
          // compute result values and add to dictionary for cache
          this.ACaseElement1DForceValues.Add(key,
              ResultHelper.GetElement1DResultValues(ACaseElement1DResults[key], energyUnit, true));
        }
        return new List<GsaResultsValues>() { ACaseElement1DForceValues[key] };
      }
      else
      {
        if (!this.ComboElement1DForceValues.ContainsKey(key)) // see if values exist
        {
          if (!this.ComboElement1DResults.ContainsKey(key)) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ComboElement1DResults.Add(key, CombinationCaseResult.Element1DResults(elementlist, 1, true));
          }
          // compute result values and add to dictionary for cache
          this.ComboElement1DForceValues.Add(key,
              ResultHelper.GetElement1DResultValues(ComboElement1DResults[key], energyUnit, SelectedPermutationIDs, true));
        }
        return new List<GsaResultsValues>(ComboElement1DForceValues[key].Values);
      }
    }

    /// <summary>
    /// Get 2D displacement values 
    /// For analysis case the length of the list will be 1
    /// This method will use cache data if it exists
    /// </summary>
    /// <param name="elementlist"></param>
    /// <param name="lengthUnit"></param>
    /// <returns></returns>
    internal List<GsaResultsValues> Element2DDisplacementValues(string elementlist, LengthUnit lengthUnit)
    {
      if (this.Type == ResultType.AnalysisCase)
      {
        if (!this.ACaseElement2DDisplacementValues.ContainsKey(elementlist)) // see if values exist
        {
          if (!this.ACaseElement2DResults.ContainsKey(new Tuple<string, double>(elementlist, 0))) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ACaseElement2DResults.Add(new Tuple<string, double>(elementlist, 0), AnalysisCaseResult.Element2DResults(elementlist, 0));
          }
          // compute result values and add to dictionary for cache
          this.ACaseElement2DDisplacementValues.Add(elementlist,
              ResultHelper.GetElement2DResultValues(ACaseElement2DResults[new Tuple<string, double>(elementlist, 0)], lengthUnit));
        }
        return new List<GsaResultsValues>() { ACaseElement2DDisplacementValues[elementlist] };
      }
      else
      {
        if (!this.ComboElement2DDisplacementValues.ContainsKey(elementlist)) // see if values exist
        {
          if (!this.ComboElement2DResults.ContainsKey(new Tuple<string, double>(elementlist, 0))) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ComboElement2DResults.Add(new Tuple<string, double>(elementlist, 0), CombinationCaseResult.Element2DResults(elementlist, 0));
          }
          // compute result values and add to dictionary for cache
          this.ComboElement2DDisplacementValues.Add(elementlist,
              ResultHelper.GetElement2DResultValues(ComboElement2DResults[new Tuple<string, double>(elementlist, 0)], lengthUnit, SelectedPermutationIDs));
        }
        return new List<GsaResultsValues>(ComboElement2DDisplacementValues[elementlist].Values);
      }
    }

    /// <summary>
    /// Get 2D force values 
    /// For analysis case the length of the list will be 1
    /// This method will use cache data if it exists
    /// </summary>
    /// <param name="elementlist"></param>
    /// <param name="forceUnit"></param>
    /// <param name="momentUnit"></param>
    /// <returns></returns>
    internal List<GsaResultsValues> Element2DForceValues(string elementlist, ForcePerLengthUnit forceUnit, ForceUnit momentUnit)
    {
      if (this.Type == ResultType.AnalysisCase)
      {
        if (!this.ACaseElement2DForceValues.ContainsKey(elementlist)) // see if values exist
        {
          if (!this.ACaseElement2DResults.ContainsKey(new Tuple<string, double>(elementlist, 0))) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ACaseElement2DResults.Add(new Tuple<string, double>(elementlist, 0), AnalysisCaseResult.Element2DResults(elementlist, 0));
          }
          // compute result values and add to dictionary for cache
          this.ACaseElement2DForceValues.Add(elementlist,
              ResultHelper.GetElement2DResultValues(ACaseElement2DResults[new Tuple<string, double>(elementlist, 0)], forceUnit, momentUnit));
        }
        return new List<GsaResultsValues>() { ACaseElement2DForceValues[elementlist] };
      }
      else
      {
        if (!this.ComboElement2DForceValues.ContainsKey(elementlist)) // see if values exist
        {
          if (!this.ComboElement2DResults.ContainsKey(new Tuple<string, double>(elementlist, 0))) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ComboElement2DResults.Add(new Tuple<string, double>(elementlist, 0), CombinationCaseResult.Element2DResults(elementlist, 0));
          }
          // compute result values and add to dictionary for cache
          this.ComboElement2DForceValues.Add(elementlist,
              ResultHelper.GetElement2DResultValues(ComboElement2DResults[new Tuple<string, double>(elementlist, 0)], forceUnit, momentUnit, SelectedPermutationIDs));
        }
        return new List<GsaResultsValues>(ComboElement2DForceValues[elementlist].Values);
      }
    }

    /// <summary>
    /// Get 2D shear force values 
    /// For analysis case the length of the list will be 1
    /// This method will use cache data if it exists
    /// </summary>
    /// <param name="elementlist"></param>
    /// /// <param name="forceUnit"></param>
    /// <returns></returns>
    internal List<GsaResultsValues> Element2DShearValues(string elementlist, ForcePerLengthUnit forceUnit)
    {
      if (this.Type == ResultType.AnalysisCase)
      {
        if (!this.ACaseElement2DShearValues.ContainsKey(elementlist)) // see if values exist
        {
          if (!this.ACaseElement2DResults.ContainsKey(new Tuple<string, double>(elementlist, 0))) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ACaseElement2DResults.Add(new Tuple<string, double>(elementlist, 0), AnalysisCaseResult.Element2DResults(elementlist, 0));
          }
          // compute result values and add to dictionary for cache
          this.ACaseElement2DShearValues.Add(elementlist,
              ResultHelper.GetElement2DResultValues(ACaseElement2DResults[new Tuple<string, double>(elementlist, 0)], forceUnit));
        }
        return new List<GsaResultsValues>() { ACaseElement2DShearValues[elementlist] };
      }
      else
      {
        if (!this.ComboElement2DShearValues.ContainsKey(elementlist)) // see if values exist
        {
          if (!this.ComboElement2DResults.ContainsKey(new Tuple<string, double>(elementlist, 0))) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ComboElement2DResults.Add(new Tuple<string, double>(elementlist, 0), CombinationCaseResult.Element2DResults(elementlist, 0));
          }
          // compute result values and add to dictionary for cache
          this.ComboElement2DShearValues.Add(elementlist,
              ResultHelper.GetElement2DResultValues(ComboElement2DResults[new Tuple<string, double>(elementlist, 0)], forceUnit, SelectedPermutationIDs));
        }
        return new List<GsaResultsValues>(ComboElement2DShearValues[elementlist].Values);
      }
    }

    /// <summary>
    /// Get 2D stress values 
    /// For analysis case the length of the list will be 1
    /// This method will use cache data if it exists
    /// </summary>
    /// <param name="elementlist"></param>
    /// <param name="layer"></param>
    /// <param name="stressUnit"></param>
    /// <returns></returns>
    internal List<GsaResultsValues> Element2DStressValues(string elementlist, double layer, PressureUnit stressUnit)
    {
      Tuple<string, double> key = new Tuple<string, double>(elementlist, layer);
      if (this.Type == ResultType.AnalysisCase)
      {
        if (!this.ACaseElement2DStressValues.ContainsKey(key)) // see if values exist
        {
          if (!this.ACaseElement2DResults.ContainsKey(key)) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ACaseElement2DResults.Add(key, AnalysisCaseResult.Element2DResults(elementlist, layer));
          }
          // compute result values and add to dictionary for cache
          this.ACaseElement2DStressValues.Add(key,
              ResultHelper.GetElement2DResultValues(ACaseElement2DResults[key], stressUnit));
        }
        return new List<GsaResultsValues>() { ACaseElement2DStressValues[key] };
      }
      else
      {
        if (!this.ComboElement2DStressValues.ContainsKey(key)) // see if values exist
        {
          if (!this.ComboElement2DResults.ContainsKey(key)) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ComboElement2DResults.Add(key, CombinationCaseResult.Element2DResults(elementlist, layer));
          }
          // compute result values and add to dictionary for cache
          this.ComboElement2DStressValues.Add(key,
              ResultHelper.GetElement2DResultValues(ComboElement2DResults[key], stressUnit, SelectedPermutationIDs));
        }
        return new List<GsaResultsValues>(ComboElement2DStressValues[key].Values);
      }
    }

    /// <summary>
    /// Get 3D displacement values 
    /// For analysis case the length of the list will be 1
    /// This method will use cache data if it exists
    /// </summary>
    /// <param name="elementlist"></param>
    /// <param name="lengthUnit"></param>
    /// <returns></returns>
    internal List<GsaResultsValues> Element3DDisplacementValues(string elementlist, LengthUnit lengthUnit)
    {
      if (this.Type == ResultType.AnalysisCase)
      {
        if (!this.ACaseElement3DDisplacementValues.ContainsKey(elementlist)) // see if values exist
        {
          if (!this.ACaseElement3DResults.ContainsKey(elementlist)) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ACaseElement3DResults.Add(elementlist, AnalysisCaseResult.Element3DResults(elementlist));
          }
          // compute result values and add to dictionary for cache
          this.ACaseElement3DDisplacementValues.Add(elementlist,
              ResultHelper.GetElement3DResultValues(ACaseElement3DResults[elementlist], lengthUnit));
        }
        return new List<GsaResultsValues>() { ACaseElement3DDisplacementValues[elementlist] };
      }
      else
      {
        if (!this.ComboElement3DDisplacementValues.ContainsKey(elementlist)) // see if values exist
        {
          if (!this.ComboElement3DResults.ContainsKey(elementlist)) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ComboElement3DResults.Add(elementlist, CombinationCaseResult.Element3DResults(elementlist));
          }
          // compute result values and add to dictionary for cache
          this.ComboElement3DDisplacementValues.Add(elementlist,
              ResultHelper.GetElement3DResultValues(ComboElement3DResults[elementlist], lengthUnit, SelectedPermutationIDs));
        }
        return new List<GsaResultsValues>(ComboElement3DDisplacementValues[elementlist].Values);
      }
    }

    /// <summary>
    /// Get 2D stress values 
    /// For analysis case the length of the list will be 1
    /// This method will use cache data if it exists
    /// </summary>
    /// <param name="elementlist"></param>
    /// <param name="stressUnit"></param>
    /// <returns></returns>
    internal List<GsaResultsValues> Element3DStressValues(string elementlist, PressureUnit stressUnit)
    {
      if (this.Type == ResultType.AnalysisCase)
      {
        if (!this.ACaseElement3DStressValues.ContainsKey(elementlist)) // see if values exist
        {
          if (!this.ACaseElement3DResults.ContainsKey(elementlist)) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ACaseElement3DResults.Add(elementlist, AnalysisCaseResult.Element3DResults(elementlist));
          }
          // compute result values and add to dictionary for cache
          this.ACaseElement3DStressValues.Add(elementlist,
              ResultHelper.GetElement3DResultValues(ACaseElement3DResults[elementlist], stressUnit));
        }
        return new List<GsaResultsValues>() { ACaseElement3DStressValues[elementlist] };
      }
      else
      {
        if (!this.ComboElement3DStressValues.ContainsKey(elementlist)) // see if values exist
        {
          if (!this.ComboElement3DResults.ContainsKey(elementlist)) // see if result exist
          {
            // if the results hasn't already been taken out and add them to our dictionary
            this.ComboElement3DResults.Add(elementlist, CombinationCaseResult.Element3DResults(elementlist));
          }
          // compute result values and add to dictionary for cache
          this.ComboElement3DStressValues.Add(elementlist,
              ResultHelper.GetElement3DResultValues(ComboElement3DResults[elementlist], stressUnit, SelectedPermutationIDs));
        }
        return new List<GsaResultsValues>(ComboElement3DStressValues[elementlist].Values);
      }
    }
    #endregion
    #region other methods
    public override string ToString()
    {
      string txt = "";
      if (Type == ResultType.AnalysisCase)
        txt = "A" + CaseID;
      else if (Type == ResultType.Combination)
      {
        txt = "C" + CaseID;
        if (SelectedPermutationIDs.Count > 0)
        {
          if (SelectedPermutationIDs.Count > 1)
            txt = txt + " P:" + SelectedPermutationIDs.Count;
          else
            txt = txt + " p" + SelectedPermutationIDs[0];
        }
      }
      return txt;
    }

    #endregion
  }
}
