using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GhSA.Parameters;
using GrasshopperAsyncComponent;

namespace GhSA.Components
{
    /// <summary>
    /// Component to open an existing GSA model
    /// </summary>
    public class AsyncSaveModel : GH_AsyncComponent, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("333fa19b-5521-412f-a864-13cfe451283b");
        public AsyncSaveModel()
          : base("Save Model 2", "Save Async", "Saves your GSA model from this parametric nightmare",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat0())
        { BaseWorker = new SaveWorker(); this.Hidden = true; }// sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => GSA.Properties.Resources.SaveModel;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            m_attributes = new UI.Button3ComponentUI(this, "Save", "Save As", "Open in GSA", SaveFile, SaveAsFile, OpenGSAexe, true, "Save GSA file");
        }

        public void SaveFile()
        {
            if (fileName == null | fileName == "")
                SaveAsFile();
            else
            {
                save = true;
                Params.OnParametersChanged();
                ExpireSolution(true);
            }
        }

        public void SaveAsFile()
        {
            var fdi = new Rhino.UI.SaveFileDialog { Filter = "GSA Files(*.gwb)|*.gwb|All files (*.*)|*.*" };
            var res = fdi.ShowSaveDialog();
            if (res) // == DialogResult.OK)
            {
                fileName = fdi.FileName;
                usersetFileName = true;
                save = true;

                //add panel input with string
                //delete existing inputs if any
                while (Params.Input[2].Sources.Count > 0)
                    Grasshopper.Instances.ActiveCanvas.Document.RemoveObject(Params.Input[2].Sources[0], false);

                //instantiate  new panel
                var panel = new Grasshopper.Kernel.Special.GH_Panel();
                panel.CreateAttributes();

                panel.Attributes.Pivot = new PointF((float)Attributes.DocObject.Attributes.Bounds.Left -
                    panel.Attributes.Bounds.Width - 40, (float)Attributes.DocObject.Attributes.Bounds.Bottom - panel.Attributes.Bounds.Height);

                //populate value list with our own data
                panel.UserText = fileName;

                //Until now, the panel is a hypothetical object.
                // This command makes it 'real' and adds it to the canvas.
                Grasshopper.Instances.ActiveCanvas.Document.AddObject(panel, false);

                //Connect the new slider to this component
                Params.Input[2].AddSource(panel);
                Params.OnParametersChanged();
                ExpireSolution(true);
            }
        }

        public void OpenGSAexe()
        {
            Message = "Opening GSA...";
            if (fileName != null)
            {
                if (fileName != "")
                {
                    if (canOpen)
                    {
                        System.Diagnostics.Process.Start(fileName);
                    }
                }
            }
            else
                Message = "Save first";
        }
        #endregion

        #region Input and output
        // This region handles input and output parameters

        public static string fileName = null;
        public static bool usersetFileName = false;
        public static bool save = false;
        public static bool canOpen = false;
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA Model", "GSA", "GSA model to save", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Save?", "Save", "Input 'True' to save or use button", GH_ParamAccess.item);
            pManager.AddTextParameter("File and Path", "File", "Filename and path", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
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

        public class SaveWorker : WorkerInstance
        {
            public SaveWorker() : base(null) { }
            public override WorkerInstance Duplicate() => new SaveWorker();

            #region fields
            GsaModel model = null;
            #endregion

            public override void GetData(IGH_DataAccess DA, GH_ComponentParamServer Params)
            {
                #region GetData
                GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
                if (DA.GetData(0, ref gh_typ))
                {
                    if (gh_typ.Value is GsaModelGoo)
                    {
                        gh_typ.CastTo(ref model);
                    }
                    if (!usersetFileName)
                    {
                        fileName = model.FileName;
                    }
                }

                string tempfile = "";
                if (DA.GetData(2, ref tempfile))
                {
                    fileName = tempfile;
                }

                DA.GetData(1, ref save);
                #endregion
            }

            public override void SetData(IGH_DataAccess DA)
            {
                DA.SetData(0, new GsaModelGoo(model));
            }

            public override void DoWork(Action<string, double> ReportProgress, Action Done)
            {
                if (save)
                {
                    string mes = "";
                    if (fileName != null)
                    {
                        ReportProgress("Saving model...", -1);
                        ReturnValue res = model.Model.SaveAs(fileName);
                        if (res != ReturnValue.GS_OK)
                            ReportProgress(res.ToString(), -20);
                        else
                            canOpen = true;
                    }
                    else
                    {
                        ReportProgress("Saving model...", -1);
                        ReturnValue res = model.Model.Save();
                        if (res != ReturnValue.GS_OK)
                            ReportProgress(res.ToString(), -20);
                        else
                            canOpen = true;
                    }
                    Done();
                }
                else
                    return;
            }
        }
    }
}

