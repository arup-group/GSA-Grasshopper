using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GhSA.Parameters;
using System.Linq;
using System.Collections.ObjectModel;
using GrasshopperAsyncComponent;


namespace GhSA.Components
{
    /// <summary>
    /// Component to open an existing GSA model
    /// </summary>
    public class AsyncOpenModel : GH_AsyncComponent, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("3f158860-c1dd-4f20-92eb-88e7c2b461bf");
        public AsyncOpenModel()
          : base("Open Model 2", "Open Async", "Open an existing GSA model",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat0())
        { BaseWorker = new OpenWorker(); this.Hidden = true; }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.OpenModel;
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
                fileName = fdi.FileName;

                //add panel input with string
                //delete existing inputs if any
                while (Params.Input[0].Sources.Count > 0)
                    Instances.ActiveCanvas.Document.RemoveObject(Params.Input[0].Sources[0], false);

                //instantiate  new panel
                var panel = new Grasshopper.Kernel.Special.GH_Panel();
                panel.CreateAttributes();

                panel.Attributes.Pivot = new PointF((float)Attributes.DocObject.Attributes.Bounds.Left -
                    panel.Attributes.Bounds.Width - 30, (float)Params.Input[0].Attributes.Pivot.Y - panel.Attributes.Bounds.Height/2);

                //populate value list with our own data
                panel.UserText = fileName;

                //Until now, the panel is a hypothetical object.
                // This command makes it 'real' and adds it to the canvas.
                Grasshopper.Instances.ActiveCanvas.Document.AddObject(panel, false);

                //Connect the new slider to this component
                Params.Input[0].AddSource(panel);

                (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
                Params.OnParametersChanged();
                ExpireSolution(true);
            }
        }
        #endregion

        #region Input and output
        // This region handles input and output parameters

        public static string fileName = null;
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Filename and path", "File", "GSA model to open and work with." + 
                    System.Environment.NewLine + "Input either path component, a text string with path and " +
                    System.Environment.NewLine + "filename or an existing GSA model created in Grasshopper.", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
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
            Params.Input[0].Optional = fileName != null; //filename can have input from user input
            Params.Input[0].ClearRuntimeMessages(); // this needs to be called to avoid having a runtime warning message after changed to optional

            //    Params.Output[i].NickName = "P";
            //    Params.Output[i].Name = "Points";
            //    Params.Output[i].Description = "Points imported from GSA";
            //    Params.Output[i].Access = GH_ParamAccess.list;

        }
        #endregion
        #endregion

        #region (de)serialization
        //This region handles serialisation and deserialisation, meaning that 
        // component states will be remembered when reopening GH script
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            //writer.SetInt32("Mode", (int)_mode);
            writer.SetString("File", (string)fileName);
            //writer.SetBoolean("Advanced", (bool)advanced);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            //_mode = (FoldMode)reader.GetInt32("Mode");
            fileName = (string)reader.GetString("File");
            //advanced = (bool)reader.GetBoolean("Advanced");
            return base.Read(reader);
        }
        #endregion

        public class OpenWorker : WorkerInstance
        {
            public OpenWorker() : base(null) { }
            public override WorkerInstance Duplicate() => new OpenWorker();

            #region fields
            Model model = new Model();
            string fileName = AsyncOpenModel.fileName;
            #endregion

            public override void GetData(IGH_DataAccess DA, GH_ComponentParamServer Params)
            {
                #region GetData
                GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();

                if (DA.GetData(0, ref gh_typ))
                {
                    if (gh_typ.Value is GH_String)
                    {
                        string tempfile = "";
                        if (GH_Convert.ToString(gh_typ, out tempfile, GH_Conversion.Both))
                            fileName = tempfile;

                        if (!fileName.EndsWith(".gwb"))
                            fileName = fileName + ".gwb";
                    }
                    else if (gh_typ.Value is GsaAPI.Model)
                    {
                        gh_typ.CastTo(ref model);
                        GsaModel gsaModel = new GsaModel
                        {
                            Model = model,
                        };
                    }
                }
                #endregion
            }

            public override void SetData(IGH_DataAccess DA)
            {
                // 👉 Checking for cancellation!
                if (CancellationToken.IsCancellationRequested) return;

                GsaModel gsaModel = new GsaModel
                {
                    Model = model,
                    FileName = fileName
                };
                DA.SetData(0, new GsaModelGoo(gsaModel));
            }

            public override void DoWork(Action<string, double> ReportProgress, Action Done)
            {
                ReportProgress("Opening model...", -1);
                ReturnValue status = model.Open(fileName);

                if (status == 0)
                {
                    Util.GsaTitles.GetTitlesFromGSA(model);

                    string mes = Path.GetFileName(fileName);
                    mes = mes.Substring(0, mes.Length - 4);
                    ReportProgress(mes, -1);
                }
                else
                {
                    string mes  = "Unable to open Model" + System.Environment.NewLine + status.ToString();
                    ReportProgress(mes, -20);
                    return;
                }
                Done();
            }
        }
    }
}

