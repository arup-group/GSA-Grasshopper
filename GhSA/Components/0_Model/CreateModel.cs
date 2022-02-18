using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using System.Linq;
using UnitsNet;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to assemble and analyse a GSA model
    /// </summary>
    public class CreateModel : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("326aa021-10b3-45a0-8286-eefb3dc3e2e1");
        public CreateModel()
          : base("Create Model", "Model", "Create a GSA Model",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat0())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateModel;
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
                dropdownitems.Add(Units.FilteredLengthUnits);
                selecteditems.Add(lengthUnit.ToString());

                IQuantity quantity = new Length(0, lengthUnit);
                unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

                first = false;
            }
            m_attributes = new UI.MultiDropDownCheckBoxesComponentUI(this, SetSelected, dropdownitems, selecteditems, SetAnalysis, initialCheckState, checkboxText, spacerDescriptions);
        }
        public void SetSelected(int i, int j)
        {
            // change selected item
            selecteditems[i] = dropdownitems[i][j];

            lengthUnit = (UnitsNet.Units.LengthUnit)Enum.Parse(typeof(UnitsNet.Units.LengthUnit), selecteditems[i]);

            // update name of inputs (to display unit on sliders)
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        public void SetAnalysis(List<bool> value)
        {
            ReMesh = value[0];
        }
        List<string> checkboxText = new List<string>() { "ElemsFromMems" };
        List<bool> initialCheckState = new List<bool>() { true };
        bool ReMesh;
        private void UpdateUIFromSelectedItems()
        {
            lengthUnit = (UnitsNet.Units.LengthUnit)Enum.Parse(typeof(UnitsNet.Units.LengthUnit), selecteditems[0]);

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
            "Unit",
            "Settings"
        });
        private bool first = true;
        private UnitsNet.Units.LengthUnit lengthUnit = Units.LengthUnitGeometry;
        string unitAbbreviation;
        
        #endregion

        #region input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            IQuantity length = new Length(0, lengthUnit);
            unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

            pManager.AddGenericParameter("GSA Model(s)", "GSA", "Existing GSA Model(s) to append to" + System.Environment.NewLine +
                "If you input more than one model they will be merged" + System.Environment.NewLine + "with first model in list taking priority for IDs", GH_ParamAccess.list);
            pManager.AddGenericParameter("GSA Properties", "Prob", "Sections and Prop2Ds to add/set in model" + System.Environment.NewLine +
                "Properties already added to Elements or Members" + System.Environment.NewLine + "will automatically be added with Geometry input", GH_ParamAccess.list);
            pManager.AddGenericParameter("GSA Geometry in [" + unitAbbreviation + "]", "Geo", "Nodes, Element1Ds, Element2Ds, Member1Ds, Member2Ds and Member3Ds to add/set in model", GH_ParamAccess.list);
            pManager.AddGenericParameter("GSA Load", "Load", "Loads to add to the model" + System.Environment.NewLine + "You can also use this input to add Edited GridPlaneSurfaces", GH_ParamAccess.list);
            for (int i = 0; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Model", "GSA", "GSA Model", GH_ParamAccess.item);
        }
        #endregion

        #region fields
        List<GsaModel> Models { get; set; }
        List<GsaNode> Nodes { get; set; }
        List<GsaElement1d> Elem1ds { get; set; }
        List<GsaElement2d> Elem2ds { get; set; }
        List<GsaElement3d> Elem3ds { get; set; }
        List<GsaMember1d> Mem1ds { get; set; }
        List<GsaMember2d> Mem2ds { get; set; }
        List<GsaMember3d> Mem3ds { get; set; }
        List<GsaLoad> Loads { get; set; }
        List<GsaSection> Sections { get; set; }
        List<GsaProp2d> Prop2Ds { get; set; }
        List<GsaGridPlaneSurface> GridPlaneSurfaces { get; set; }
        GsaModel OutModel { get; set; }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            #region GetData
            Models = null;
            Nodes = null;
            Elem1ds = null;
            Elem2ds = null;
            Elem3ds = null;
            Mem1ds = null;
            Mem2ds = null;
            Mem3ds = null;
            Loads = null;
            Sections = null;
            Prop2Ds = null;
            GridPlaneSurfaces = null;

            // Get Model input
            List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
            if (DA.GetDataList(0, gh_types))
            {
                List<GsaModel> in_models = new List<GsaModel>();
                for (int i = 0; i < gh_types.Count; i++)
                {
                    GH_ObjectWrapper gh_typ = gh_types[i];
                    if (gh_typ == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Model input (index: " + i + ") is null and has been ignored"); continue; }
                    if (gh_typ.Value is GsaModelGoo)
                    {
                        GsaModel in_model = new GsaModel();
                        gh_typ.CastTo(ref in_model);
                        in_models.Add(in_model);
                    }
                    else
                    {
                        string type = gh_typ.Value.GetType().ToString();
                        type = type.Replace("GsaGH.Parameters.", "");
                        type = type.Replace("Goo", "");
                        Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert GSA input parameter of type " +
                            type + " to GsaModel");
                        return;
                    }
                }
                Models = in_models;
            }

            // Get Section Property input
            gh_types = new List<GH_ObjectWrapper>();
            if (DA.GetDataList(1, gh_types))
            {
                List<GsaSection> in_sect = new List<GsaSection>();
                List<GsaProp2d> in_prop = new List<GsaProp2d>();
                for (int i = 0; i < gh_types.Count; i++)
                {
                    GH_ObjectWrapper gh_typ = gh_types[i];
                    if (gh_typ == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Property input (index: " + i + ") is null and has been ignored"); continue; }

                    if (gh_typ.Value is GsaSectionGoo)
                    {
                        GsaSection gsasection = new GsaSection();
                        gh_typ.CastTo(ref gsasection);
                        in_sect.Add(gsasection.Duplicate());
                    }
                    else if (gh_typ.Value is GsaProp2dGoo)
                    {
                        GsaProp2d gsaprop = new GsaProp2d();
                        gh_typ.CastTo(ref gsaprop);
                        in_prop.Add(gsaprop.Duplicate());
                    }
                    else
                    {
                        string type = gh_typ.Value.GetType().ToString();
                        type = type.Replace("GsaGH.Parameters.", "");
                        type = type.Replace("Goo", "");
                        Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert Prop input parameter of type " +
                            type + " to GsaSection or GsaProp2d");
                        return;
                    }
                }
                if (in_sect.Count > 0)
                    Sections = in_sect;
                if (in_prop.Count > 0)
                    Prop2Ds = in_prop;
            }

            // Get Geometry input
            gh_types = new List<GH_ObjectWrapper>();
            List<GsaNode> in_nodes = new List<GsaNode>();
            List<GsaElement1d> in_elem1ds = new List<GsaElement1d>();
            List<GsaElement2d> in_elem2ds = new List<GsaElement2d>();
            List<GsaElement3d> in_elem3ds = new List<GsaElement3d>();
            List<GsaMember1d> in_mem1ds = new List<GsaMember1d>();
            List<GsaMember2d> in_mem2ds = new List<GsaMember2d>();
            List<GsaMember3d> in_mem3ds = new List<GsaMember3d>();
            if (DA.GetDataList(2, gh_types))
            {
                for (int i = 0; i < gh_types.Count; i++)
                {
                    GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
                    gh_typ = gh_types[i];
                    if (gh_typ == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Geometry input (index: " + i + ") is null and has been ignored"); continue; }

                    if (gh_typ.Value is GsaNodeGoo)
                    {
                        GsaNode gsanode = new GsaNode();
                        gh_typ.CastTo(ref gsanode);
                        in_nodes.Add(gsanode.Duplicate());
                    }
                    else if (gh_typ.Value is GsaElement1dGoo)
                    {
                        GsaElement1d gsaelem1 = new GsaElement1d();
                        gh_typ.CastTo(ref gsaelem1);
                        in_elem1ds.Add(gsaelem1.Duplicate());
                    }
                    else if (gh_typ.Value is GsaElement2dGoo)
                    {
                        GsaElement2d gsaelem2 = new GsaElement2d();
                        gh_typ.CastTo(ref gsaelem2);
                        in_elem2ds.Add(gsaelem2.Duplicate());
                    }
                    else if (gh_typ.Value is GsaElement3dGoo)
                    {
                        GsaElement3d gsaelem3 = new GsaElement3d();
                        gh_typ.CastTo(ref gsaelem3);
                        in_elem3ds.Add(gsaelem3.Duplicate());
                    }
                    else if (gh_typ.Value is GsaMember1dGoo)
                    {
                        GsaMember1d gsamem1 = new GsaMember1d();
                        gh_typ.CastTo(ref gsamem1);
                        in_mem1ds.Add(gsamem1.Duplicate());
                    }
                    else if (gh_typ.Value is GsaMember2dGoo)
                    {
                        GsaMember2d gsamem2 = new GsaMember2d();
                        gh_typ.CastTo(ref gsamem2);
                        in_mem2ds.Add(gsamem2.Duplicate());
                    }
                    else if (gh_typ.Value is GsaMember3dGoo)
                    {
                        GsaMember3d gsamem3 = new GsaMember3d();
                        gh_typ.CastTo(ref gsamem3);
                        in_mem3ds.Add(gsamem3.Duplicate());
                    }
                    else
                    {
                        string type = gh_typ.Value.GetType().ToString();
                        type = type.Replace("GsaGH.Parameters.", "");
                        type = type.Replace("Goo", "");
                        Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert Geometry input parameter of type " +
                            type + System.Environment.NewLine + " to Node, Element1D, Element2D, Element3D, Member1D, Member2D or Member3D");
                        return;
                    }
                }
                if (in_nodes.Count > 0)
                    Nodes = in_nodes;
                if (in_elem1ds.Count > 0)
                    Elem1ds = in_elem1ds;
                if (in_elem2ds.Count > 0)
                    Elem2ds = in_elem2ds;
                if (in_elem3ds.Count > 0)
                    Elem3ds = in_elem3ds;
                if (in_mem1ds.Count > 0)
                    Mem1ds = in_mem1ds;
                if (in_mem2ds.Count > 0)
                    Mem2ds = in_mem2ds;
                if (in_mem3ds.Count > 0)
                    Mem3ds = in_mem3ds;
            }


            // Get Loads input
            gh_types = new List<GH_ObjectWrapper>();
            if (DA.GetDataList(3, gh_types))
            {
                List<GsaLoad> in_loads = new List<GsaLoad>();
                List<GsaGridPlaneSurface> in_gps = new List<GsaGridPlaneSurface>();
                for (int i = 0; i < gh_types.Count; i++)
                {
                    GH_ObjectWrapper gh_typ = gh_types[i];
                    if (gh_typ == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Load input (index: " + i + ") is null and has been ignored"); continue; }

                    if (gh_typ.Value is GsaLoadGoo)
                    {
                        GsaLoad gsaload = null;
                        gh_typ.CastTo(ref gsaload);
                        in_loads.Add(gsaload.Duplicate());
                    }
                    else if (gh_typ.Value is GsaGridPlaneSurfaceGoo)
                    {
                        GsaGridPlaneSurface gsaGPS = new GsaGridPlaneSurface();
                        gh_typ.CastTo(ref gsaGPS);
                        in_gps.Add(gsaGPS.Duplicate());
                    }
                    else
                    {
                        string type = gh_typ.Value.GetType().ToString();
                        type = type.Replace("GsaGH.Parameters.", "");
                        type = type.Replace("Goo", "");
                        Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert Load input parameter of type " +
                            type + " to Load or GridPlaneSurface");
                        return;
                    }
                }
                if (in_loads.Count > 0)
                    Loads = in_loads;
                if (in_gps.Count > 0)
                    GridPlaneSurfaces = in_gps;
            }
            // manually add a warning if no input is set, as all inputs are optional
            if (Models == null & Nodes == null & Elem1ds == null & Elem2ds == null &
                Mem1ds == null & Mem2ds == null & Mem3ds == null & Sections == null
                & Prop2Ds == null & Loads == null & GridPlaneSurfaces == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameters failed to collect data");
                return;
            }

            #endregion

            #region DoWork
            GsaModel analysisModel = null;
            if (Models != null)
            {
                if (Models.Count > 0)
                {
                    if (Models.Count > 1)
                    {
                        analysisModel = Util.Gsa.ToGSA.Models.MergeModel(Models);
                    }
                    else
                    {
                        analysisModel = Models[0].Clone();
                    }
                }
            }
            if (analysisModel != null)
                OutModel = analysisModel;
            else
                OutModel = new GsaModel();

            // Assemble model
            Model gsa = Util.Gsa.ToGSA.Assemble.AssembleModel(analysisModel, Nodes, Elem1ds, Elem2ds, Elem3ds, Mem1ds, Mem2ds, Mem3ds, Sections, Prop2Ds, Loads, GridPlaneSurfaces, lengthUnit);
            
            #region meshing
            // Create elements from members
            if (ReMesh)
                gsa.CreateElementsFromMembers();
            #endregion 

            #region analysis


            #endregion
            OutModel.Model = gsa;
            #endregion

            #region SetData
            DA.SetData(0, new GsaModelGoo(OutModel));
            #endregion
        }
        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            Util.GH.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
            writer.SetBoolean("ReMesh", ReMesh);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            try // if users has an old old version of this component then ReMesh won't be there to read
            {
                ReMesh = reader.GetBoolean("ReMesh");
                
                try // if users has an old versopm of this component then dropdown menu wont read
                {
                    Util.GH.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);
                }
                catch (Exception) // we create the dropdown menu with our chosen default
                {
                    dropdownitems = new List<List<string>>();
                    selecteditems = new List<string>();

                    // set length to meters as this was the only option for old components
                    lengthUnit = UnitsNet.Units.LengthUnit.Meter;

                    dropdownitems.Add(Units.FilteredLengthUnits);
                    selecteditems.Add(lengthUnit.ToString());

                    IQuantity quantity = new Length(0, lengthUnit);
                    unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

                    first = false;
                }
            }
            catch (Exception) // we set it to our chosen defaults
            {
                ReMesh = true;

                dropdownitems = new List<List<string>>();
                selecteditems = new List<string>();

                // set length to meters as this was the only option for old components
                lengthUnit = UnitsNet.Units.LengthUnit.Meter;

                dropdownitems.Add(Units.FilteredLengthUnits);
                selecteditems.Add(lengthUnit.ToString());

                IQuantity quantity = new Length(0, lengthUnit);
                unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

                first = false;
            }

            initialCheckState = new List<bool>();
            initialCheckState.Add(ReMesh);
            
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
            IQuantity length = new Length(0, lengthUnit);
            unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

            Params.Input[2].Name = "GSA Geometry in [" + unitAbbreviation + "]";
        }
        #endregion  
    }
}

