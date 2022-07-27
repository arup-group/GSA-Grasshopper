//using System;
//using System.Collections.Generic;
//using Grasshopper.Kernel;
//using Grasshopper.Kernel.Types;
//using GsaAPI;
//using Rhino.Geometry;
//using GhSA.Parameters;
//using Grasshopper.Kernel.Parameters;

//namespace GhSA.Components
//{
//    public class CreateNodeLoad_OBSOLETE : GH_OasysComponent, IGH_VariableParameterComponent
//    {
//        #region Name and Ribbon Layout
//        public CreateNodeLoad_OBSOLETE()
//            : base("Create Node Load", "NodeLoad", "Create GSA Node Load",
//                Ribbon.CategoryName.Name(),
//                Ribbon.SubCategoryName.Cat3())
//        { this.Hidden = true; } // sets the initial state of the component to hidden
//        public override Guid ComponentGuid => new Guid("0e30f030-8fc0-4ffa-afd9-02b18c094006");
//        public override GH_Exposure Exposure => GH_Exposure.hidden;

//        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.NodeLoad;
//        #endregion

//        #region Custom UI
//        //This region overrides the typical component layout
//        public override void CreateAttributes()
//        {
//            if (first)
//            {
//                selecteditem = _mode.ToString();
//                first = false;
//            }

//            m_attributes = new UI.DropDownComponentUI(this, SetSelected, dropdownitems, selecteditem, "Load Type");
//        }


//        public void SetSelected(string selected)
//        {
//            selecteditem = selected;
//            switch (selected)
//            {
//                case "Node":
//                    _mode = FoldMode.Node;
//                    break;
//                case "Applied Displ":
//                    _mode = FoldMode.Applied_Displ;
//                    break;
//                case "Settlement":
//                    _mode = FoldMode.Settlements;
//                    break;
//            }
//        }
//        #endregion

//        #region Input and output
//        readonly List<string> dropdownitems = new List<string>(new string[]
//        {
//            "Node",
//            "Applied Displ",
//            "Settlement"
//        });

//        string selecteditem;

//        #endregion

//        protected override void RegisterInputParams(GH_InputParamManager pManager)
//        {
//            pManager.AddIntegerParameter("Load case", "LC", "Load case number (default 1)", GH_ParamAccess.item, 1);
//            pManager.AddTextParameter("Node list", "No", "List of Nodes to apply load to." + System.Environment.NewLine +
//                 "Node list should take the form:" + System.Environment.NewLine +
//                 " 1 11 to 72 step 2 not (XY3 31 to 45)" + System.Environment.NewLine +
//                 "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item);
//            pManager.AddTextParameter("Name", "Na", "Load Name", GH_ParamAccess.item);
//            pManager.AddTextParameter("Direction", "Di", "Load direction (default z)." +
//                    System.Environment.NewLine + "Accepted inputs are:" +
//                    System.Environment.NewLine + "x" +
//                    System.Environment.NewLine + "y" +
//                    System.Environment.NewLine + "z" +
//                    System.Environment.NewLine + "xx" +
//                    System.Environment.NewLine + "yy" +
//                    System.Environment.NewLine + "zz", GH_ParamAccess.item, "z");
//            pManager.AddNumberParameter("Value (" + Units.Force + ")", "V", "Load Value (" + Units.Force + ")", GH_ParamAccess.item);
//            pManager[0].Optional = true;
//            pManager[2].Optional = true;
//            pManager[3].Optional = true;

//        }
//        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
//        {
//            pManager.AddGenericParameter("Node Load", "Ld", "GSA Node Load", GH_ParamAccess.item);
//        }

//        protected override void SolveInstance(IGH_DataAccess DA)
//        {

//            GsaNodeLoad nodeLoad = new GsaNodeLoad();

//            // Node load type
//            switch (_mode)
//            {
//                case FoldMode.Node:
//                    nodeLoad.NodeLoadType = GsaNodeLoad.NodeLoadTypes.NODE_LOAD;
//                    break;
//                case FoldMode.Applied_Displ:
//                    nodeLoad.NodeLoadType = GsaNodeLoad.NodeLoadTypes.APPLIED_DISP;
//                    break;
//                case FoldMode.Settlements:
//                    nodeLoad.NodeLoadType = GsaNodeLoad.NodeLoadTypes.SETTLEMENT;
//                    break;
//            }

//            // 0 Load case
//            int lc = 1;
//            GH_Integer gh_lc = new GH_Integer();
//            if (DA.GetData(0, ref gh_lc))
//                GH_Convert.ToInt32(gh_lc, out lc, GH_Conversion.Both);
//            nodeLoad.NodeLoad.Case = lc;

//            // 1 element/beam list
//            // check that user has not inputted Gsa geometry elements here
//            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
//            if (DA.GetData(1, ref gh_typ))
//            {
//                string type = gh_typ.Value.ToString().ToUpper();
//                if (type.StartsWith("GSA "))
//                {
//                    Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
//                        "You cannot input a Node/Element/Member in NodeList input!" + System.Environment.NewLine +
//                        "Element list should take the form:" + System.Environment.NewLine +
//                        "'1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)'" + System.Environment.NewLine +
//                        "Refer to GSA help file for definition of lists and full vocabulary.");
//                    return;
//                }
//            }
//            string nodeList = "all";
//            GH_String gh_nl = new GH_String();
//            if (DA.GetData(1, ref gh_nl))
//                GH_Convert.ToString(gh_nl, out nodeList, GH_Conversion.Both);
//            nodeLoad.NodeLoad.Nodes = nodeList;

//            // 3 Name
//            string name = "";
//            GH_String gh_name = new GH_String();
//            if (DA.GetData(3, ref gh_name))
//            {
//                if (GH_Convert.ToString(gh_name, out name, GH_Conversion.Both))
//                    nodeLoad.NodeLoad.Name = name;
//            }

//            // 3 direction
//            string dir = "Z";
//            Direction direc = Direction.Z;

//            GH_String gh_dir = new GH_String();
//            if (DA.GetData(3, ref gh_dir))
//                GH_Convert.ToString(gh_dir, out dir, GH_Conversion.Both);
//            dir = dir.ToUpper().Trim();
//            if (dir == "X")
//                direc = Direction.X;
//            if (dir == "Y")
//                direc = Direction.Y;
//            if (dir == "XX")
//                direc = Direction.XX;
//            if (dir == "YY")
//                direc = Direction.YY;
//            if (dir == "ZZ")
//                direc = Direction.ZZ;

//            nodeLoad.NodeLoad.Direction = direc;

//            double load = 0;
//            if (DA.GetData(4, ref load))
//            {
//                if (direc == Direction.Z)
//                    load *= -1000; //convert to kN
//                else
//                    load *= 1000;
//            }

//            nodeLoad.NodeLoad.Value = load;

//            GsaLoad gsaLoad = new GsaLoad(nodeLoad);
//            DA.SetData(0, new GsaLoadGoo(gsaLoad));
//        }

//        #region menu override
//        private enum FoldMode
//        {
//            Node,
//            Applied_Displ,
//            Settlements
//        }
//        private bool first = true;
//        private FoldMode _mode = FoldMode.Node;
//        #endregion

//        #region (de)serialization
//        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
//        {
//            writer.SetInt32("Mode", (int)_mode);
//            writer.SetString("select", selecteditem);
//            return base.Write(writer);
//        }
//        public override bool Read(GH_IO.Serialization.GH_IReader reader)
//        {
//            _mode = (FoldMode)reader.GetInt32("Mode");
//            selecteditem = reader.GetString("select");
//            this.CreateAttributes();
//            return base.Read(reader);
//        }

//        bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
//        {
//            return false;
//        }
//        bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
//        {
//            return false;
//        }
//        IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
//        {
//            return null;
//        }
//        bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
//        {
//            return false;
//        }
//        #endregion
//        #region IGH_VariableParameterComponent null implementation
//        void IGH_VariableParameterComponent.VariableParameterMaintenance()
//        {

//        }
//        #endregion
//    }
//}