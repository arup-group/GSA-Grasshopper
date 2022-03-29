using System;
using System.Collections.Generic;

using GsaAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using UnitsNet;

namespace GsaGH.Parameters
{
    public class GsaCombinationCase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        internal int ID { get; set; } = 0;
        public GsaCombinationCase()
        { }
        internal GsaCombinationCase(int id, string name, string description)
        {
            this.ID = id;
            this.Name = name;  
            this.Description = description;
        }
        public GsaCombinationCase(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }
        public GsaCombinationCase Duplicate()
        {
            return new GsaCombinationCase(ID, Name, Description);
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
            return "GSA Combination Case '" + Name.ToString() + "' {" + Description.ToString() + "}";
        }

        #endregion
    }

    /// <summary>
    /// GsaSection Goo wrapper class, makes sure GsaSection can be used in Grasshopper.
    /// </summary>
    public class GsaCombinationCaseGoo : GH_Goo<GsaCombinationCase>
    {
        #region constructors
        public GsaCombinationCaseGoo()
        {
            this.Value = new GsaCombinationCase();
        }
        public GsaCombinationCaseGoo(GsaCombinationCase anal)
        {
            if (anal == null)
                anal = new GsaCombinationCase();
            this.Value = anal; //section.Duplicate();
        }
        public override IGH_Goo Duplicate()
        {
            return DuplicateGsaAnalysisCase();
        }
        public GsaCombinationCaseGoo DuplicateGsaAnalysisCase()
        {
            return new GsaCombinationCaseGoo(Value == null ? new GsaCombinationCase() : Value.Duplicate());
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
                return "Null GSA Combination Case";
            else
                return Value.ToString();
        }
        public override string TypeName
        {
            get { return ("GSA Combination Case"); }
        }
        public override string TypeDescription
        {
            get { return ("GSA Combination Case"); }
        }


        #endregion

        #region casting methods
        public override bool CastTo<Q>(ref Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance into some other type Q.            

            //if (typeof(Q).IsAssignableFrom(typeof(GH_Integer)))
            //{
            //    if (Value == null)
            //        target = default;
            //    else
            //    {
            //        GH_Integer ghint = new GH_Integer();
            //        if (GH_Convert.ToGHInteger(Value.ID, GH_Conversion.Both, ref ghint))
            //            target = (Q)(object)ghint;
            //        else
            //            target = default;
            //    }
            //    return true;
            //}

            target = default;
            return false;
        }
        public override bool CastFrom(object source)
        {
            // This function is called when Grasshopper needs to convert other data 
            // into GsaSection.


            if (source == null) { return false; }

            //Cast from string
            //if (GH_Convert.ToString(source, out string name, GH_Conversion.Both))
            //{
            //    Value = new GsaAnalysisCase();
            //    Value.Name = name;
            //    return true;
            //}

            ////Cast from string
            //if (GH_Convert.ToInt32(source, out int id, GH_Conversion.Both))
            //{
            //    Value = new GsaAnalysisCase();
            //    Value.ID = id;
            //    return true;
            //}

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
