using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using UnitsNet;

namespace GsaGH.Parameters
{
    internal class GsaResultsMinMax
    {
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

        internal GsaResultsMinMax()
        { }
    }
    public class GsaResult
    {
        #region analysiscase members
        internal AnalysisCaseResult AnalysisCaseResult { get; set; }
        internal Dictionary<string, ReadOnlyDictionary<int, Element3DResult>> ACaseElement3DResults { get; set; }
        internal Dictionary<string, ReadOnlyDictionary<int, Element2DResult>> ACaseElement2DResults { get; set; }
        internal Dictionary<string, ReadOnlyDictionary<int, Element1DResult>> ACaseElement1DResults { get; set; }
        internal Dictionary<string, ReadOnlyDictionary<int, NodeResult>> ACaseNodeResults { get; set; }
        internal Dictionary<string, GsaResultsMinMax> ACaseResultsMinMax { get; set; }
        #endregion
        #region combination members
        internal CombinationCaseResult CombinationCaseResult { get; set; }
        internal int CombPermutationID { get; set; }
        internal int NumPermutations { get; set; }
        internal Dictionary<string, ReadOnlyDictionary<int, ReadOnlyCollection<Element3DResult>>> ComboElement3DResults { get; set; }
        internal Dictionary<string, ReadOnlyDictionary<int, ReadOnlyCollection<Element2DResult>>> ComboElement2DResults { get; set; }
        internal Dictionary<string, ReadOnlyDictionary<int, ReadOnlyCollection<Element1DResult>>> ComboElement1DResults { get; set; }
        internal Dictionary<string, ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>>> ComboNodeResults { get; set; }
        internal Dictionary<string, List<GsaResultsMinMax>> ComboResultsMinMax { get; set; }
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
