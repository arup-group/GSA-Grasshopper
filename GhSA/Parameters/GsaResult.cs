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
        internal Dictionary<string, ReadOnlyDictionary<int, Element3DResult>> ACaseElement3DResults { get; set; }
        /// <summary>
        /// Analysis Case 3DElement Result VALUES Dictionary 
        /// Append to this dictionary to chache results
        /// key = elementList
        /// </summary>
        internal Dictionary<string, GsaResultsValues> ACaseElement3DValues { get; set; }

        /// <summary>
        /// Analysis Case 2DElement API Result Dictionary 
        /// Append to this dictionary to chache results
        /// key = elementList
        /// </summary>
        internal Dictionary<string, ReadOnlyDictionary<int, Element2DResult>> ACaseElement2DResults { get; set; }
        /// <summary>
        /// Analysis Case 2DElement Result VALUES Dictionary 
        /// Append to this dictionary to chache results
        /// key = elementList
        /// </summary>
        internal Dictionary<string, GsaResultsValues> ACaseElement2DValues { get; set; }

        /// <summary>
        /// Analysis Case 1DElement API Result Dictionary 
        /// Append to this dictionary to chache results
        /// key = Tuple<elementList, numberOfDivisions>
        /// </summary>
        internal Dictionary<Tuple<string, int>, ReadOnlyDictionary<int, Element1DResult>> ACaseElement1DResults { get; set; }
        /// <summary>
        /// Analysis Case 1DElement Result VALUES Dictionary 
        /// Append to this dictionary to chache results
        /// key = Tuple<elementList, numberOfDivisions>
        /// </summary>
        internal Dictionary<Tuple<string, int>, GsaResultsValues> ACaseElement1DValues { get; set; }

        /// <summary>
        /// Analysis Case Node API Result Dictionary 
        /// Append to this dictionary to chache results
        /// key = elementList
        /// </summary>
        internal Dictionary<string, ReadOnlyDictionary<int, NodeResult>> ACaseNodeResults { get; set; }
        /// <summary>
        /// Analysis Case Node Result VALUES Dictionary 
        /// Append to this dictionary to chache results
        /// key = elementList
        /// </summary>
        internal Dictionary<string, GsaResultsValues> ACaseNodeValues { get; set; }
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
        internal Dictionary<string, ReadOnlyDictionary<int, ReadOnlyCollection<Element3DResult>>> ComboElement3DResults { get; set; }
        /// <summary>
        /// Combination Case 3DElement Result VALUES Dictionary 
        /// Append to this dictionary to chache results
        /// key = elementList
        /// value = List<permutationsResults>
        /// </summary>
        internal Dictionary<string, List<GsaResultsValues>> ComboElement3DValues { get; set; }

        /// <summary>
        /// Combination Case 1DElement API Result Dictionary 
        /// Append to this dictionary to chache results
        /// key = Tuple<elementList, numberOfDivisions>
        /// value = <elementID, collection<permutationResult>
        /// </summary>
        internal Dictionary<string, ReadOnlyDictionary<int, ReadOnlyCollection<Element2DResult>>> ComboElement2DResults { get; set; }
        /// <summary>
        /// Combination Case 2DElement Result VALUES Dictionary 
        /// Append to this dictionary to chache results
        /// key = elementList
        /// value = List<permutationsResults>
        /// </summary>
        internal Dictionary<string, List<GsaResultsValues>> ComboElement2DValues { get; set; }

        /// <summary>
        /// Combination Case 1DElement API Result Dictionary 
        /// Append to this dictionary to chache results
        /// key = Tuple<elementList, numberOfDivisions>
        /// value = <elementID, collection<permutationResult>
        /// </summary>
        internal Dictionary<Tuple<string, int>, ReadOnlyDictionary<int, ReadOnlyCollection<Element1DResult>>> ComboElement1DResults { get; set; }
        /// <summary>
        /// Combination Case 1DElement Result VALUES Dictionary 
        /// Append to this dictionary to chache results
        /// key = elementList
        /// value = List<permutationsResults>
        /// </summary>
        internal Dictionary<Tuple<string, int>, List<GsaResultsValues>> ComboElement1DValues { get; set; }

        /// <summary>
        /// Combination Case Node API Result Dictionary 
        /// Append to this dictionary to chache results
        /// key = elementList
        /// value = <elementID, collection<permutationResult>
        /// </summary>
        internal Dictionary<string, ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>>> ComboNodeResults { get; set; }
        /// <summary>
        /// Combination Case Node Result VALUES Dictionary 
        /// Append to this dictionary to chache results
        /// key = elementList
        /// value = List<permutationsResults>
        /// </summary>
        internal Dictionary<string, List<GsaResultsValues>> ComboNodeValues { get; set; }
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
            this.NumPermutations = ComboNodeResults["all"].Count;
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

        #region methods
        public override string ToString()
        {
            string txt = "";
            if (Type == ResultType.AnalysisCase)
                txt = "A" + CaseID;
            else if (Type == ResultType.Combination)
            {
                txt = "C" + CaseID;
                if (CombPermutationID < 0)
                {
                    if (NumPermutations != 0 || NumPermutations > 0)
                        txt = txt + " " + NumPermutations + " permutations";
                }
                else
                    txt = txt + " p" + CombPermutationID;

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
    //public class GsaAnalysisCaseParameter : GH_PersistentParam<GsaAnalysisCaseGoo>
    //{
    //    public GsaAnalysisCaseParameter()
    //      : base(new GH_InstanceDescription("AnalysisCase", "ΣC", "GSA Analysis Case", GsaGH.Components.Ribbon.CategoryName.Name(), GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    //    {
    //    }

    //    public override Guid ComponentGuid => new Guid("6b99a192-bdbd-41bf-8efa-1bc146d3c224");

    //    public override GH_Exposure Exposure => GH_Exposure.secondary;

    //    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.AnalysisCaseParam;

    //    protected override GH_GetterResult Prompt_Plural(ref List<GsaAnalysisCaseGoo> values)
    //    {
    //        return GH_GetterResult.cancel;
    //    }
    //    protected override GH_GetterResult Prompt_Singular(ref GsaAnalysisCaseGoo value)
    //    {
    //        return GH_GetterResult.cancel;
    //    }
    //    protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomSingleValueItem()
    //    {
    //        System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
    //        {
    //            Text = "Not available",
    //            Visible = false
    //        };
    //        return item;
    //    }
    //    protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem()
    //    {
    //        System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
    //        {
    //            Text = "Not available",
    //            Visible = false
    //        };
    //        return item;
    //    }

    //    #region preview methods

    //    public bool Hidden
    //    {
    //        get { return true; }
    //        //set { m_hidden = value; }
    //    }
    //    public bool IsPreviewCapable
    //    {
    //        get { return false; }
    //    }
    //    #endregion
    //}

}
