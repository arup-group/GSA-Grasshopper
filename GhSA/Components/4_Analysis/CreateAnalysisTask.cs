using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;


namespace GsaGH.Components
{
    /// <summary>
    /// Component to retrieve non-geometric objects from a GSA model
    /// </summary>
    public class CreateAnalysisTask : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("6ef86d0b-892c-4b6f-950e-b4477e9f0910");
        public CreateAnalysisTask()
          : base("Create Analysis Task", "CreateTask", "Create a GSA Analysis Task",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat4())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateAnalysisTask;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (first)
            {
                dropdownitems = new List<List<string>>();
                selecteditems = new List<string>();

                // length
                //dropdownitems.Add(Enum.GetNames(typeof(UnitsNet.Units.LengthUnit)).ToList());
                dropdownitems.Add(Enum.GetValues(typeof(GsaAnalysisTask.AnalysisType)).Cast<GsaAnalysisTask.AnalysisType>().Select(x => x.ToString()).ToList());
                selecteditems.Add(analtype.ToString());

                first = false;
            }
            m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
        }
        public void SetSelected(int i, int j)
        {
            // change selected item
            selecteditems[i] = dropdownitems[i][j];

            analtype = (GsaAnalysisTask.AnalysisType)Enum.Parse(typeof(GsaAnalysisTask.AnalysisType), selecteditems[i]);

            // update name of inputs (to display unit on sliders)
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        private void UpdateUIFromSelectedItems()
        {
            analtype = (GsaAnalysisTask.AnalysisType)Enum.Parse(typeof(GsaAnalysisTask.AnalysisType), selecteditems[0]);

            CreateAttributes();
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        // list of lists with all dropdown lists conctent
        List<List<string>> dropdownitems;
        // list of selected items
        List<string> selecteditems;
        // list of descriptions 
        List<string> spacerDescriptions = new List<string>(new string[]
        {
            "Solver type"
        });
        private bool first = true;
        private GsaAnalysisTask.AnalysisType analtype = GsaAnalysisTask.AnalysisType.Static;
        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "Na", "Task Name", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddGenericParameter("Analysis Cases", "ΣCs", "List of GSA Analysis Cases", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Analysis Task", "ΣT", "GSA Analysis Task", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = analtype.ToString();
            DA.GetData(0, ref name);

            List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
            if (DA.GetDataList(1, gh_types))
            {
                List<GsaAnalysisCase> cases = new List<GsaAnalysisCase>();
                for (int i = 0; i < gh_types.Count; i++)
                {
                    GH_ObjectWrapper gh_typ = gh_types[i];
                    if (gh_typ == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Analysis Case input (index: " + i + ") is null and has been ignored"); continue; }
                    if (gh_typ.Value is GsaAnalysisCaseGoo)
                    {
                        cases.Add(((GsaAnalysisCaseGoo)gh_typ.Value).Value.Duplicate());
                    }
                    else
                    {
                        string type = gh_typ.Value.GetType().ToString();
                        type = type.Replace("GsaGH.Parameters.", "");
                        type = type.Replace("Goo", "");
                        Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert Analysis Case input parameter of type " +
                            type + " to GsaAnalysisCase");
                        return;
                    }
                }

                GsaAnalysisTask task = new GsaAnalysisTask();
                task.Name = name;
                task.Cases = cases;
                task.Type = analtype;
                DA.SetData(0, new GsaAnalysisTaskGoo(task));
            }
        }
        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            Util.GH.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            Util.GH.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);

            UpdateUIFromSelectedItems();

            first = false;

            return base.Read(reader);
        }
        #endregion

        #region IGH_VariableParameterComponent null implementation
        bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
        {
            return false;
        }
        bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
        {
            return false;
        }
        IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
        {
            return null;
        }
        bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
        {
            return false;
        }
        void IGH_VariableParameterComponent.VariableParameterMaintenance()
        {

        }
        #endregion  
    }
}

