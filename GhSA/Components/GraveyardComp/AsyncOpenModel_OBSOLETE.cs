using System;
using System.IO;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using GrasshopperAsyncComponent;


namespace GsaGH.Components
{
    /// <summary>
    /// Component to open an existing GSA model
    /// </summary>
    public class AsyncOpenModel_OBSOLETE : GH_AsyncComponent, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("3f158860-c1dd-4f20-92eb-88e7c2b461bf");
        public AsyncOpenModel_OBSOLETE()
          : base("Open Model", "OpenGSA", "Open an existing GSA model",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat0())
        { BaseWorker = new OpenWorker(); this.Hidden = true; }

        public override GH_Exposure Exposure => GH_Exposure.hidden;

        protected override Bitmap Icon => GsaGH.Properties.Resources.OpenModel;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        
        public override void CreateAttributes()
        {
            m_attributes = new UI.ButtonComponentUI(this, "Open", OpenFile, "Open GSA file");
        }
        public void OpenFile()
        {
            var fdi = new Rhino.UI.OpenFileDialog { Filter = "GSA Files(*.gwb)|*.gwb|All files (*.*)|*.*" }; //"GSA Files(*.gwa; *.gwb)|*.gwa;*.gwb|All files (*.*)|*.*"
            var res = fdi.ShowOpenDialog();
            if (res) // == DialogResult.OK)
            {
                string file = fdi.FileName;

                // instantiate  new panel
                var panel = new Grasshopper.Kernel.Special.GH_Panel();
                panel.CreateAttributes();

                // set the location relative to the open component on the canvas
                panel.Attributes.Pivot = new PointF((float)Attributes.DocObject.Attributes.Bounds.Left -
                    panel.Attributes.Bounds.Width - 30, (float)Params.Input[0].Attributes.Pivot.Y - panel.Attributes.Bounds.Height / 2);

                // check for existing input
                while (Params.Input[0].Sources.Count > 0)
                {
                    var input = Params.Input[0].Sources[0];
                    // check if input is the one we automatically create below
                    if (Params.Input[0].Sources[0].InstanceGuid == panelGUID)
                    {
                        // update the UserText in existing panel
                        //RecordUndoEvent("Changed OpenGSA Component input");
                        panel = input as Grasshopper.Kernel.Special.GH_Panel;
                        panel.UserText = file;
                        panel.ExpireSolution(true); // update the display of the panel
                    }

                    // remove input
                    Params.Input[0].RemoveSource(input);
                }

                //populate panel with our own content
                panel.UserText = file;

                // record the panel's GUID if new, so that we can update it on change
                panelGUID = panel.InstanceGuid;
                
                //Until now, the panel is a hypothetical object.
                // This command makes it 'real' and adds it to the canvas.
                Grasshopper.Instances.ActiveCanvas.Document.AddObject(panel, false);

                //Connect the new slider to this component
                Params.Input[0].AddSource(panel);

                (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
                Params.OnParametersChanged();
                Params.Input[0].ClearRuntimeMessages();
                CancellationSources.Add(new System.Threading.CancellationTokenSource());
                ClearData();
            }
        }
        #endregion

        #region Input and output
        // This region handles input and output parameters
        private static Guid panelGUID = Guid.NewGuid();

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Filename and path", "File", "GSA model to open and work with." + 
                    System.Environment.NewLine + "Input either path component, a text string with path and " +
                    System.Environment.NewLine + "filename or an existing GSA model created in Grasshopper.", GH_ParamAccess.item);
            //this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "This component uses Async to not block the UI thread" +
            //        System.Environment.NewLine + "This means GH doesnt freeze if you try open a large file or from a jobdrive from home" +
            //        System.Environment.NewLine + "However, sometimes this component outputs an empty model." +
            //        System.Environment.NewLine + "Disable and Enable the component should solve this (Ctrl+E)");
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA Model", "GSA", "GSA Model", GH_ParamAccess.item);
        }
        #region IGH_VariableParameterComponent null implementation
        //This sub region handles any changes to the component after it has been placed on the canvas
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
            //Params.Input[0].Optional = file != ""; //file can be stored inside component from user input
            //Params.Input[0].ClearRuntimeMessages(); // this needs to be called to avoid having a runtime warning message after changed to optional
        }
        #endregion
        #endregion

        #region (de)serialization
        //This region handles serialisation and deserialisation, meaning that 
        // component states will be remembered when reopening GH script
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            //writer.SetString("File", (string)file);
            writer.SetGuid("Guid", (Guid)panelGUID);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            //file = (string)reader.GetString("File");
            panelGUID = (Guid)reader.GetGuid("Guid");
            return base.Read(reader);
        }
        #endregion

        public class OpenWorker : WorkerInstance
        {
            public OpenWorker() : base(null) { }
            public override WorkerInstance Duplicate() => new OpenWorker();

            #region fields
            GsaModel GsaModel = new GsaModel();
            #endregion
            #region GetData
            public override void GetData(IGH_DataAccess DA, GH_ComponentParamServer Params)
            {
                GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
                if (DA.GetData(0, ref gh_typ))
                {
                    if (gh_typ.Value is GH_String)
                    {
                        string tempfile = "";
                        if (GH_Convert.ToString(gh_typ, out tempfile, GH_Conversion.Both))
                        {
                            if (!tempfile.EndsWith(".gwb"))
                                tempfile = tempfile + ".gwb";
                            GsaModel.FileName = tempfile;
                        }
                    }
                    else if (gh_typ.Value is Model)
                    {
                        Model model = new Model();
                        gh_typ.CastTo(ref model);
                        GsaModel.Model = model;
                    }
                }
            }
            #endregion
            public override void SetData(IGH_DataAccess DA)
            {
                // 👉 Checking for cancellation!
                if (CancellationToken.IsCancellationRequested) return;

                DA.SetData(0, new GsaModelGoo(GsaModel));
            }

            public override void DoWork(Action<string, double> ReportProgress, Action Done)
            {
                ReportProgress("Opening model...", -2);
                Model model = new Model();
                string fileName = GsaModel.FileName;
                ReturnValue status = model.Open(fileName);
                GsaModel.Model = model;

                if (status != 0)
                {
                    string message  = "Unable to open Model" + System.Environment.NewLine + status.ToString();
                    ReportProgress(message, -20);
                    return;
                }

                ReportProgress("Updating units...", -2);
                Titles.GetTitlesFromGSA(model);

                string mes = Path.GetFileNameWithoutExtension(fileName);
                ReportProgress(mes, -1);
                Done();
            }
        }
    }
}

