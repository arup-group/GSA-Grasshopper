using System;
using System.Collections.Generic;
using System.Linq;

using GsaAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino;
using GhSA.Util.Gsa;
using Grasshopper.Documentation;
using Rhino.Collections;

namespace GhSA.Parameters
{
    /// <summary>
    /// Model class, this class defines the basic properties and methods for any Gsa Model
    /// </summary>
    public class GsaModel

    {
        public Model Model
        {
            get { return m_model; }
            set { m_model = value; }
        }



        #region fields
        private Model m_model;

        #endregion

        #region constructors
        public GsaModel()
        {
            m_model = new Model();
        }

        public GsaModel(GsaModel model)
        {
            m_model = model.Model;
        }

        //public GsaModel(Model model)
        //{ 
        //    m_model = model;
        //}

        public GsaModel Duplicate()
        {
            Model dup = new Model();

            //duplicate the incoming model ### Shallow copy funcitonality in GsaAPI welcome here....
            if (m_model != null)
            {
                System.Collections.ObjectModel.ReadOnlyDictionary<int, Node> nodes = m_model.Nodes();
                dup.SetNodes(nodes);

                System.Collections.ObjectModel.ReadOnlyDictionary<int, Element> elems = m_model.Elements();
                dup.SetElements(elems);

                System.Collections.ObjectModel.ReadOnlyDictionary<int, Member> mems = m_model.Members();
                dup.SetMembers(mems);

                //ToDo
                //model.SetAnalysisCaseDescription;
                //model.SetAxes;
                //model.SetBeamLoad;
                //model.SetFaceLoad;
                //model.SetGravityLoad;
                //model.SetGridAreaLoad;
                //model.SetGridLineLoad;
                //model.SetGridPlanes;
                //model.SetGridSurfaces;
                //model.SetNodeLoad;
                //model.SetProp2Ds;
                //model.SetSections;
            }
            this.Model = dup;
            return this;
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (m_model == null)
                    return false;
                return true;
            }
        }


        #endregion

        #region methods
        public override string ToString()
        {
            //Could add detailed description of model content here
            return "GSA Model";
        }

        #endregion
    }

    /// <summary>
    /// GsaMember Goo wrapper class, makes sure GsaMember can be used in Grasshopper.
    /// </summary>
    public class GsaModelGoo : GH_Goo<GsaModel>
    {
        #region constructors
        public GsaModelGoo()
        {
            this.Value = new GsaModel();
        }
        public GsaModelGoo(GsaModel model)
        {
            if (model == null)
                model = new GsaModel();
            this.Value = model.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGsaModel();
        }
        public GsaModelGoo DuplicateGsaModel()
        {
            return new GsaModelGoo(Value == null ? new GsaModel() : Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid
        {
            get
            {
                if (Value.Model == null) { return false; }
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
                return "Null GSA Model";
            else
                return Value.ToString();
        }
        public override string TypeName
        {
            get { return ("GSA Model"); }
        }
        public override string TypeDescription
        {
            get { return ("GSA Model without results"); }
        }


        #endregion

        #region casting methods
        public override bool CastTo<Q>(ref Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance of GsaModel into some other type Q.            


            if (typeof(Q).IsAssignableFrom(typeof(GsaModel)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value;
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(Model)))
            {
                if (Value == null)
                   target = default(Q);
                else
                target = (Q)(object)Value.Model;
                return true;
            }


            target = default(Q);
            return false;
        }
        public override bool CastFrom(object source)
        {
            // This function is called when Grasshopper needs to convert other data 
            // into GsaModel.


            if (source == null) { return false; }

            //Cast from GsaModel
            if (typeof(GsaModel).IsAssignableFrom(source.GetType()))
            {
               Value = (GsaModel)source;
                return true;
            }

            if (typeof(Model).IsAssignableFrom(source.GetType()))
            {
                Value.Model = (Model)source;
                return true;
            }


            return false;
        }
        #endregion


    }

    /// <summary>
    /// This class provides a Parameter interface for the Data_GsaMember type.
    /// </summary>
    public class GsaModelParameter : GH_PersistentParam<GsaModelGoo>
    {
        public GsaModelParameter()
          : base(new GH_InstanceDescription("GSA Model", "GSA", "A GSA model without results", GhSA.Components.Ribbon.CategoryName.name(), GhSA.Components.Ribbon.SubCategoryName.cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("43eb8fb6-d469-4c3b-ab3c-e8d6ad378d9a");

        public override GH_Exposure Exposure => GH_Exposure.primary;

        //protected override Bitmap Icon => Resources.CrossSections;

        //We do not allow users to pick boatshells, 
        //therefore the following 4 methods disable all this ui.
        protected override GH_GetterResult Prompt_Plural(ref List<GsaModelGoo> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref GsaModelGoo value)
        {
            return GH_GetterResult.cancel;
        }
        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomSingleValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem();
            item.Text = "Not available";
            item.Visible = false;
            return item;
        }
        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem();
            item.Text = "Not available";
            item.Visible = false;
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
