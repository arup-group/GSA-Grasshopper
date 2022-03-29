using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using UnitsNet;

namespace GsaGH.Parameters
{
    /// <summary>
    /// Section class, this class defines the basic properties and methods for any Gsa Section
    /// </summary>
    public class GsaAnalysisTask
    {
        public enum AnalysisType
        {
            Static = 1,
            Static_P_delta = 4,
            Nonlinear_static = 8,
            Modal_dynamic = 2,
            Modal_P_delta = 5,
            Ritz = 32,
            Ritz_P_Delta = 33,
            Response_spectrum = 6,
            Pseudo_Response_spectrum = 42,
            Linear_time_history = 15,
            Harmonic = 14,
            Footfall = 34,
            Periodic = 35,
            Buckling = 3,
            Form_finding = 9,
            Envelope = 37,
            Model_stability = 39,
            Model_stability_P_delta = 40
        }
        public string Name { get; set; }
        public AnalysisType Type { get; set; }
        public int ID { get { return m_idd; } }
        internal void SetID(int id)
        {
            m_idd = id;
        }
        #region fields
        private int m_idd = 0;
        public List<GsaAnalysisCase> Cases { get; set; } = null;
        #endregion

        #region constructors
        public GsaAnalysisTask()
        {
            m_idd = 0;
            Cases = new List<GsaAnalysisCase>();
            Type = AnalysisType.Static;
        }
        internal GsaAnalysisTask(int ID, AnalysisTask task, Model model)
        {
            m_idd = ID;
            Cases = new List<GsaAnalysisCase>();
            foreach (int caseID in task.Cases)
            {
                string caseName = model.AnalysisCaseName(caseID);
                string caseDescription = model.AnalysisCaseDescription(caseID);
                Cases.Add(new GsaAnalysisCase(caseID, caseName, caseDescription));
            }
            Type = (AnalysisType)task.Type;
            Name = task.Name;
        }
        internal void CreateDeafultCases(Model model)
        {
            Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> tuple = Util.Gsa.FromGSA.GetAnalysisTasksAndCombinations(model);
            this.Cases = tuple.Item2.Select(x => x.Value).ToList();
        }
        internal void CreateDeafultCases(GsaModel gsaModel)
        {
            Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> tuple = Util.Gsa.FromGSA.GetAnalysisTasksAndCombinations(gsaModel);
            this.Cases = tuple.Item2.Select(x => x.Value).ToList();
        }
        
        public GsaAnalysisTask Duplicate()
        {
            if (this == null) { return null; }
            GsaAnalysisTask dup = new GsaAnalysisTask();
            dup.m_idd = m_idd;
            dup.Cases = Cases;
            dup.Type = Type;
            dup.Name = Name;
            return dup;
        }
        #endregion

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
            return "GSA Analysis Task" + ((ID > 0) ? " " + ID.ToString() : "") + " '" + Name + "' {" + Type.ToString() + "}";
        }
        #endregion
    }

    /// <summary>
    /// GsaSection Goo wrapper class, makes sure GsaSection can be used in Grasshopper.
    /// </summary>
    public class GsaAnalysisTaskGoo : GH_Goo<GsaAnalysisTask>
    {
        #region constructors
        public GsaAnalysisTaskGoo()
        {
            this.Value = new GsaAnalysisTask();
        }
        public GsaAnalysisTaskGoo(GsaAnalysisTask anal)
        {
            if (anal == null)
                anal = new GsaAnalysisTask();
            this.Value = anal; //section.Duplicate();
        }
        public override IGH_Goo Duplicate()
        {
            return DuplicateGsaAnalysisTask();
        }
        public GsaAnalysisTaskGoo DuplicateGsaAnalysisTask()
        {
            return new GsaAnalysisTaskGoo(Value == null ? new GsaAnalysisTask() : Value.Duplicate());
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
                return "Null GSA Analysis Task";
            else
                return Value.ToString();
        }
        public override string TypeName
        {
            get { return ("GSA Analysis Task"); }
        }
        public override string TypeDescription
        {
            get { return ("GSA Analysis Task"); }
        }


        #endregion

        #region casting methods
        public override bool CastTo<Q>(ref Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance into some other type Q.            

            if (typeof(Q).IsAssignableFrom(typeof(GH_Integer)))
            {
                if (Value == null)
                    target = default;
                else
                {
                    GH_Integer ghint = new GH_Integer();
                    if (GH_Convert.ToGHInteger(Value.ID, GH_Conversion.Both, ref ghint))
                        target = (Q)(object)ghint;
                    else
                        target = default;
                }
                return true;
            }

            target = default;
            return false;
        }
        public override bool CastFrom(object source)
        {
            // This function is called when Grasshopper needs to convert other data 
            // into GsaSection.


            if (source == null) { return false; }

            //Cast from string
            if (GH_Convert.ToString(source, out string name, GH_Conversion.Both))
            {
                Value = new GsaAnalysisTask();
                Value.Name = name;
                try
                {
                    Value.Type = (GsaAnalysisTask.AnalysisType)Enum.Parse(typeof(GsaAnalysisTask.AnalysisType), name);
                }
                catch (Exception) { }
                return true;
            }

            return false;
        }
        #endregion

    }

    /// <summary>
    /// This class provides a Parameter interface for the Data_GsaSection type.
    /// </summary>
    public class GsaAnalysisTaskParameter : GH_PersistentParam<GsaAnalysisTaskGoo>
    {
        public GsaAnalysisTaskParameter()
          : base(new GH_InstanceDescription("AnalysisTask", "ΣT", "GSA Analysis Task", GsaGH.Components.Ribbon.CategoryName.Name(), GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("51048d67-3652-45d0-9eec-0f9ef339c1a5");

        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.AnalysisTaskParam;

        protected override GH_GetterResult Prompt_Plural(ref List<GsaAnalysisTaskGoo> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref GsaAnalysisTaskGoo value)
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
