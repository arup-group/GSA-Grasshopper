using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using UnitsNet;
using System.Collections.Concurrent;
using Rhino.Geometry;
using UnitsNet.Units;
using Oasys.Units;
using System.Linq;
using GsaGH.Util.Gsa;
using System.Threading.Tasks;

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
            // update max and min values
            dmax_x = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X.Value).Max()).Max();
            dmax_y = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y.Value).Max()).Max();
            dmax_z = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z.Value).Max()).Max();
            try { dmax_xyz = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.XYZ.Value).Max()).Max(); } catch (Exception) { }
            dmin_x = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X.Value).Min()).Min();
            dmin_y = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y.Value).Min()).Min();
            dmin_z = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z.Value).Min()).Min();
            try { dmin_xyz = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.XYZ.Value).Min()).Min(); } catch (Exception) { }
            dmax_xx = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X.Value).Max()).Max();
            dmax_yy = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y.Value).Max()).Max();
            dmax_zz = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z.Value).Max()).Max();
            try { dmax_xxyyzz = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.XYZ.Value).Max()).Max(); } catch (Exception) { }
            dmin_xx = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X.Value).Min()).Min();
            dmin_yy = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y.Value).Min()).Min();
            dmin_zz = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z.Value).Min()).Min();
            try { dmin_xxyyzz = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.XYZ.Value).Min()).Min(); } catch (Exception) { }
            
        }
        /// <summary>
        /// Translation, forces, etc results
        /// dictionary< key = node/elementID, value = dictionary< key = position on element, value = value>>
        /// </summary>
        internal ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xyzResults { get; set; } = new ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>>();
        internal ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xxyyzzResults { get; set; } = new ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>>();
        internal double dmax_x { get; set; } = 0;
        internal double dmax_y { get; set; } = 0;
        internal double dmax_z { get; set; } = 0;
        internal double dmax_xx { get; set; } = 0;
        internal double dmax_yy { get; set; } = 0;
        internal double dmax_zz { get; set; } = 0;
        internal double dmax_xyz { get; set; } = 0;
        internal double dmax_xxyyzz { get; set; } = 0;
        internal double dmin_x { get; set; } = 0;
        internal double dmin_y { get; set; } = 0;
        internal double dmin_z { get; set; } = 0;
        internal double dmin_xx { get; set; } = 0;
        internal double dmin_yy { get; set; } = 0;
        internal double dmin_zz { get; set; } = 0;
        internal double dmin_xyz { get; set; } = 0;
        internal double dmin_xxyyzz { get; set; } = 0;

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
        /// key = elementList
        /// </summary>
        internal Dictionary<string, ReadOnlyDictionary<int, Element2DResult>> ACaseElement2DResults { get; set; } = new Dictionary<string, ReadOnlyDictionary<int, Element2DResult>>();
        
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
        /// key = elementList
        /// </summary>
        internal Dictionary<string, GsaResultsValues> ACaseElement2DStressValues { get; set; } = new Dictionary<string, GsaResultsValues>();

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
        internal int CombPermutationID { get; set; }
        
        /// <summary>
        /// Calculated number of permutations in combination case
        /// </summary>
        internal int NumPermutations { get; }

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
        internal Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>> Combo3DDisplacementValues { get; set; } = new Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>();
        
        /// <summary>
        /// Combination Case 3DElement Stress Result VALUES Dictionary 
        /// Append to this dictionary to chache results
        /// key = elementList
        /// value = Dictionary<elementID, Dictionary<permutationID, permutationsResults>>
        /// </summary>
        internal Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>> Combo3DStressValues { get; set; } = new Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>();

        /// <summary>
        /// Combination Case 2DElement API Result Dictionary 
        /// Append to this dictionary to chache results
        /// key = Tuple<elementList, numberOfDivisions>
        /// value = Dictionary<elementID, Dictionary<permutationID, permutationsResults>>
        /// </summary>
        internal Dictionary<string, ReadOnlyDictionary<int, ReadOnlyCollection<Element2DResult>>> ComboElement2DResults { get; set; } = new Dictionary<string, ReadOnlyDictionary<int, ReadOnlyCollection<Element2DResult>>>();
        
        /// <summary>
        /// Combination Case 2DElement Displacement Result VALUES Dictionary 
        /// Append to this dictionary to chache results
        /// key = elementList
        /// value = <elementID, collection<permutationResult>
        /// </summary>
        internal Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>> Combo2DDisplacementValues { get; set; } = new Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>();
        
        /// <summary>
        /// Combination Case 2DElement Force/Moment Result VALUES Dictionary 
        /// Append to this dictionary to chache results
        /// key = elementList
        /// value = Dictionary<elementID, Dictionary<permutationID, permutationsResults>>
        /// </summary>
        internal Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>> Combo2DForceValues { get; set; } = new Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>();
        
        /// <summary>
        /// Combination Case 2DElement Stress Result VALUES Dictionary 
        /// Append to this dictionary to chache results
        /// key = elementList
        /// value = Dictionary<elementID, Dictionary<permutationID, permutationsResults>>
        /// </summary>
        internal Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>> Combo2DStressValues { get; set; } = new Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>();

        /// <summary>
        /// Combination Case 2DElement Result VALUES Dictionary 
        /// Append to this dictionary to chache results
        /// key = Tuple<elementList, permutations>
        /// value = Dictionary<elementID, Dictionary<numberOfDivisions, results>>
        /// </summary>
        internal Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>> Combo2DShearValues { get; set; } = new Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>();

        /// <summary>
        /// Combination Case 1DElement API Result Dictionary 
        /// Append to this dictionary to chache results
        /// key = Tuple<elementList, permutations>
        /// value = Dictionary<elementID, Dictionary<numberOfDivisions, results>>
        /// </summary>
        internal Dictionary<Tuple<string, int>, ReadOnlyDictionary<int, ReadOnlyCollection<Element1DResult>>> ComboElement1DResults { get; set; } = new Dictionary<Tuple<string, int>, ReadOnlyDictionary<int, ReadOnlyCollection<Element1DResult>>>();

        /// <summary>
        /// Combination Case 1DElement Forves Result VALUES Dictionary 
        /// Append to this dictionary to chache results
        /// key = Tuple<elementList, permutations>
        /// value = Dictionary<elementID, Dictionary<numberOfDivisions, results>>
        /// </summary>
        internal Dictionary<Tuple<string, int>, ConcurrentDictionary<int, GsaResultsValues>> ComboElement1DForceValues { get; set; } = new Dictionary<Tuple<string, int>, ConcurrentDictionary<int, GsaResultsValues>>();

        /// <summary>
        /// Combination Case 1DElement Forves Result VALUES Dictionary 
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
        public enum ResultType
        {
            AnalysisCase,
            Combination
        }
        internal ResultType Type { get; set; }
        internal Model Model { get; set; }
        public GsaResult()
        { }
        internal GsaResult(Model model, AnalysisCaseResult result, int id)
        {
            this.Model = model;
            this.AnalysisCaseResult = result;
            this.Type = ResultType.AnalysisCase;
            this.CaseID = id;
        }
        internal GsaResult(Model model, CombinationCaseResult result, int id, int permutation = -1)
        {
            this.Model = model;
            this.CombinationCaseResult = result;
            this.Type = ResultType.Combination;
            this.CaseID = id;
            this.CombPermutationID = permutation;
            this.ComboNodeResults = new Dictionary<string, ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>>>();
            this.ComboNodeResults.Add("all", CombinationCaseResult.NodeResults("all"));
            this.NumPermutations = ComboNodeResults["all"].First().Value.Count;
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
        internal List<GsaResultsValues> NodeDisplacementValues(string nodelist, LengthUnit lengthUnit)
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
                return new List<GsaResultsValues> { ACaseNodeDisplacementValues[nodelist] };
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
                        ResultHelper.GetNodeResultValues(ComboNodeResults[nodelist], lengthUnit, CombPermutationID));
                }
                return new List<GsaResultsValues>(ComboNodeDisplacementValues[nodelist].Values);
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
        internal List<GsaResultsValues> NodeReactionForceValues(string nodelist, ForceUnit forceUnit, MomentUnit momentUnit)
        {
            // get list of support nodes
            if (nodelist.ToLower() == "all" | nodelist == "")
            {
                ReadOnlyDictionary<int, Node> nodes = Model.Nodes();
                ConcurrentBag<int> supportnodeIDs = new ConcurrentBag<int>();
                Parallel.ForEach(nodes, node =>
                {
                    NodalRestraint rest = node.Value.Restraint;
                    if (rest.X || rest.Y || rest.Z || rest.XX || rest.YY || rest.ZZ)
                        supportnodeIDs.Add(node.Key);
                });
                nodelist = string.Join(" ", supportnodeIDs.OrderBy(x => x).ToArray());
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
                        ResultHelper.GetNodeResultValues(ACaseNodeResults[nodelist], forceUnit, momentUnit));
                }
                return new List<GsaResultsValues> { ACaseNodeReactionForceValues[nodelist] };
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
                        ResultHelper.GetNodeResultValues(ComboNodeResults[nodelist], forceUnit, momentUnit, CombPermutationID));
                }
                return new List<GsaResultsValues>(ComboNodeDisplacementValues[nodelist].Values);
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
                        this.ComboElement1DResults.Add(key, CombinationCaseResult.Element1DResults(elementlist, positionsCount));
                    }
                    // compute result values and add to dictionary for cache
                    this.ComboElement1DDisplacementValues.Add(key,
                        ResultHelper.GetElement1DResultValues(ComboElement1DResults[key], lengthUnit, CombPermutationID));
                }
            return new List<GsaResultsValues>(ComboElement1DDisplacementValues[key].Values);
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
                if (CombPermutationID <= 0)
                {
                    if (NumPermutations > 1)
                        txt = txt + " " + NumPermutations + " permutations";
                }
                else
                    txt = txt + " P" + CombPermutationID;

            }
            return "GSA Result {" + txt + "}";
        }

        #endregion
    }

    /// <summary>
    /// GsaSection Goo wrapper class, makes sure GsaSection can be used in Grasshopper.
    /// </summary>
    public class GsaResultGoo : GH_Goo<GsaResult>
    {
        #region constructors
        public GsaResultGoo()
        {
            this.Value = new GsaResult();
        }
        public GsaResultGoo(GsaResult anal)
        {
            if (anal == null)
                anal = new GsaResult();
            this.Value = anal; //section.Duplicate();
        }
        public override IGH_Goo Duplicate()
        {
            return DuplicateGsaAnalysisCase();
        }
        public GsaResultGoo DuplicateGsaAnalysisCase()
        {
            return new GsaResultGoo(Value == null ? new GsaResult() : Value);
        }
        #endregion

        #region properties
        public override bool IsValid
        {
            get
            {
                if (Value == null) { return false; }
                return true;
            }
        }
        public override string IsValidWhyNot
        {
            get
            {
                //if (Value == null) { return "No internal GsaMember instance"; }
                if (Value.IsValid) { return string.Empty; }
                return Value.IsValid.ToString(); //Todo: beef this up to be more informative.
            }
        }
        public override string ToString()
        {
            if (Value == null)
                return "Null GSA Result";
            else
                return Value.ToString();
        }
        public override string TypeName
        {
            get { return ("GSA Result"); }
        }
        public override string TypeDescription
        {
            get { return ("GSA Result"); }
        }


        #endregion

        #region casting methods
        public override bool CastTo<Q>(ref Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance into some other type Q.            


            target = default;
            return false;
        }
        public override bool CastFrom(object source)
        {
            // This function is called when Grasshopper needs to convert other data 
            // into GsaSection.


            if (source == null) { return false; }


            return false;
        }
        #endregion

    }

    /// <summary>
    /// This class provides a Parameter interface for the Data_GsaSection type.
    /// </summary>
    public class GsaResultsParameter : GH_PersistentParam<GsaResultGoo>
    {
        public GsaResultsParameter()
          : base(new GH_InstanceDescription("Result", "Res", "GSA Result", GsaGH.Components.Ribbon.CategoryName.Name(), 
              GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("81f6f103-cb53-414c-908b-6adf46c3260d");

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.ResultParam;

        protected override GH_GetterResult Prompt_Plural(ref List<GsaResultGoo> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref GsaResultGoo value)
        {
            return GH_GetterResult.cancel;
        }
        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomSingleValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
            {
                Text = "Not available",
                Visible = false
            };
            return item;
        }
        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
            {
                Text = "Not available",
                Visible = false
            };
            return item;
        }

        #region preview methods

        public bool Hidden
        {
            get { return true; }
            //set { m_hidden = value; }
        }
        public bool IsPreviewCapable
        {
            get { return false; }
        }
        #endregion
    }

}
