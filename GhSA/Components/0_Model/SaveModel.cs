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

namespace GhSA.Components
{
    /// <summary>
    /// Component to open an existing GSA model
    /// </summary>
    public class SaveModel : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("e9989dce-717e-47ea-992c-e22d718e9ebb");
        public SaveModel()
          : base("Save GSA Model", "Save", "Saves your GSA model from this parametric nightmare",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat0())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.SaveModel;
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
                string mes = gsaSaveModel.SaveAs(fileName).ToString();
                if (mes == GsaAPI.ReturnValue.GS_OK.ToString())
                {
                    canOpen = true;
                    //this.OnAttributesChanged();
                    //CreateAttributes();
                    mes = "Saved file";
                    //ExpireSolution(true);
                }
                else
                {
                    mes = Char.ToUpper(mes[3]) + mes.Substring(4).ToLower().Replace("_", " ");
                }
                this.Message = mes;
            }
        }

        public void SaveAsFile()
        {
            var fdi = new Rhino.UI.SaveFileDialog { Filter = "GSA File (*.gwb)|*.gwb|All files (*.*)|*.*" };
            var res = fdi.ShowSaveDialog();
            if (res) // == DialogResult.OK)
            {
                fileName = fdi.FileName;
                usersetFileName = true;
                string mes = gsaSaveModel.SaveAs(fileName).ToString();
                if (mes == GsaAPI.ReturnValue.GS_OK.ToString())
                {
                    canOpen = true;
                    //CreateAttributes();
                    mes = "Saved file";

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
                else
                {
                    mes = Char.ToUpper(mes[3]) + mes.Substring(4).ToLower().Replace("_", " ");
                }
                this.Message = mes;
            }
        }

        public void OpenGSAexe()
        {
            if (fileName != null)
            {
                if (fileName != "")
                {
                    if (canOpen)
                        System.Diagnostics.Process.Start(fileName);
                }
            }
        }
        #endregion

        #region Input and output
        // This region handles input and output parameters

        string fileName = null;
        bool usersetFileName = false;
        Model gsaSaveModel;
        bool canOpen = false;
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA Model", "GSA", "GSA model to save", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Save?", "Save", "Input 'True' to save or use button", GH_ParamAccess.item, false);
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
            writer.SetString("File", (string)fileName);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            fileName = (string)reader.GetString("File");
            UpdateUIFromSelectedItems();
            return base.Read(reader);
        }
        private void UpdateUIFromSelectedItems()
        {
            CreateAttributes();
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Model model = new Model();
            GsaModel gsaModel = new GsaModel();
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(0, ref gh_typ))
            {
                if (gh_typ == null) { return; }
                if (gh_typ.Value is GsaModelGoo)
                {
                    gh_typ.CastTo(ref gsaModel);
                    gsaSaveModel = gsaModel.Model;
                    Message = "";
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Model");
                    return;
                }

                if (!usersetFileName)
                {
                    if (gsaModel.FileName != "")
                        fileName = gsaModel.FileName;
                }

                string tempfile = "";
                if (DA.GetData(2, ref tempfile))
                    fileName = tempfile;

                bool save = false;
                if (DA.GetData(1, ref save))
                {
                    if (save)
                        Message = gsaSaveModel.SaveAs(fileName).ToString();
                }

                DA.SetData(0, new GsaModelGoo(gsaModel));
            }
        }
    }
}

