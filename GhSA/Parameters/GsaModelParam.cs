using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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
    [Serializable]
    public class GsaModel
    {
        public Model Model
        {
            get { return m_model; }
            set { m_model = value; }
        }

        public string FileName
        {
            get { return m_filename; }
            set { m_filename = value; }
        }
        public Guid GUID
        {
            get { return m_guid; }
        }

        #region fields
        private Model m_model;
        private string m_filename = "";
        private Guid m_guid = Guid.NewGuid();

        #endregion

        #region constructors
        public GsaModel()
        {
            m_model = new Model();
        }

        /// <summary>
        /// NB! -work in progress, do NOT call this method
        /// </summary>
        /// <returns></returns>
        public GsaModel Copy() // work in progress
        {
            // Let's work just on the model (not wrapped)
            Model gsa = new Model();

            gsa.SetNodes(m_model.Nodes());
            gsa.SetElements(m_model.Elements());
            gsa.SetMembers(m_model.Members());
            gsa.SetSections(m_model.Sections());
            gsa.SetProp2Ds(m_model.Prop2Ds());
            gsa.SetAxes(m_model.Axes());
            gsa.SetGridPlanes(m_model.GridPlanes());

            //gravity load
            ReadOnlyCollection<GravityLoad> setgrav = m_model.GravityLoads();
            for (int i = 0; i < setgrav.Count; i++)
                gsa.SetGravityLoad(i + 1, setgrav[i]);

            //node loads
            ReadOnlyCollection<NodeLoad> setnode_disp = m_model.NodeLoads(NodeLoadType.APPLIED_DISP);
            for (int i = 0; i < setnode_disp.Count; i++)
                gsa.SetNodeLoad(NodeLoadType.APPLIED_DISP, i + 1, setnode_disp[i]);

            ReadOnlyCollection<NodeLoad> setnode_node = m_model.NodeLoads(NodeLoadType.NODE_LOAD);
            for (int i = 0; i < setnode_node.Count; i++)
                gsa.SetNodeLoad(NodeLoadType.NODE_LOAD, i + 1, setnode_node[i]);

            ReadOnlyCollection<NodeLoad> setnode_setl = m_model.NodeLoads(NodeLoadType.SETTLEMENT);
            for (int i = 0; i < setnode_setl.Count; i++)
                gsa.SetNodeLoad(NodeLoadType.SETTLEMENT, i + 1, setnode_setl[i]);

            //beam loads
            ReadOnlyCollection<BeamLoad> setbeam = m_model.BeamLoads();
            for (int i = 0; i < setbeam.Count; i++)
                gsa.SetBeamLoad(i + 1, setbeam[i]);

            //face loads
            ReadOnlyCollection<FaceLoad> setface = m_model.FaceLoads();
            for (int i = 0; i < setface.Count; i++)
                gsa.SetFaceLoad(i + 1, setface[i]);

            //grid point loads
            ReadOnlyCollection<GridPointLoad> setpoint = m_model.GridPointLoads();
            for (int i = 0; i < setpoint.Count; i++)
                gsa.SetGridPointLoad(i + 1, setpoint[i]);

            //grid line loads
            ReadOnlyCollection<GridLineLoad> setline = m_model.GridLineLoads();
            for (int i = 0; i < setline.Count; i++)
                gsa.SetGridLineLoad(i + 1, setline[i]);

            //grid area loads
            ReadOnlyCollection<GridAreaLoad> setarea = m_model.GridAreaLoads();
            for (int i = 0; i < setarea.Count; i++)
                gsa.SetGridAreaLoad(i + 1, setarea[i]);
            #endregion
            
            //analysis
            IReadOnlyDictionary<int, AnalysisTask> gsaTasks = m_model.AnalysisTasks();

            GsaModel gsaModel = new GsaModel();
            gsaModel.Model = gsa;
            return gsaModel;
        }
        public GsaModel Clone()
        {
            GsaModel clone = new GsaModel();

            // workaround duplicate model
            string tempfilename = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Oasys") + "GSA-Grasshopper_temp.gwb";
            m_model.SaveAs(tempfilename);
            clone.Model.Open(tempfilename);

            clone.FileName = m_filename;
            clone.m_guid = Guid.NewGuid();
            return clone;
        }

        public GsaModel Duplicate() // I think duplicate is called by Grasshopper every time the Goo-parameter is created. Avoid copying the potential heavy data here
        {
            //duplicate the incoming model ### 
            if (m_model != null)
            {
                GsaModel dup = new GsaModel();
                dup.Model = m_model;
                dup.FileName = m_filename;
                dup.m_guid = Guid.NewGuid();
                return dup;
            }
            return null;
        }

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
            get { return ("GSA Model"); }
        }


        #endregion

        #region casting methods
        public override bool CastTo<Q>(ref Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance of GsaModel into some other type Q.            

            if (typeof(Q).IsAssignableFrom(typeof(GsaModelGoo)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)this;
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(GsaModel)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value;
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(Model)))
            {
                if (Value == null)
                   target = default;
                else
                target = (Q)(object)Value.Model;
                return true;
            }


            target = default;
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
    /// This class provides a Parameter interface for the Data_GsaModel type.
    /// </summary>
    public class GsaModelParameter : GH_PersistentParam<GsaModelGoo>
    {
        public GsaModelParameter()
          : base(new GH_InstanceDescription("GSA Model", "GSA", "A GSA Model", GhSA.Components.Ribbon.CategoryName.Name(), GhSA.Components.Ribbon.SubCategoryName.Cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("43eb8fb6-d469-4c3b-ab3c-e8d6ad378d9a");

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.GsaModel;

        //We do not allow users to pick parameter, 
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
