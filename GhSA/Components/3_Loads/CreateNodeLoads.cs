using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using Rhino.Geometry;
using GhSA.Parameters;
using Grasshopper.Kernel.Parameters;
using UnitsNet.Units;
using UnitsNet;
using System.Linq;

namespace GhSA.Components
{
    public class CreateNodeLoad : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        public CreateNodeLoad()
            : base("Create Node Load", "NodeLoad", "Create GSA Node Load",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat3())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override Guid ComponentGuid => new Guid("dd16896d-111d-4436-b0da-9c05ff6efd81");
        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.NodeLoad;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (first)
            {
                dropdownitems = new List<List<string>>();
                dropdownitems.Add(loadTypeOptions);
                dropdownitems.Add(Units.FilteredForceUnits);
                dropdownitems.Add(Units.FilteredLengthUnits);

                selecteditems = new List<string>();
                selecteditems.Add(_mode.ToString());
                selecteditems.Add(Units.ForceUnit.ToString());
                selecteditems.Add(Units.LengthUnitGeometry.ToString());

                first = false;
            }

            m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
        }
        public void SetSelected(int i, int j)
        {
            // change selected item
            selecteditems[i] = dropdownitems[i][j];

            if (i == 0) // change is made to the first dropdown list
            {
                switch (selecteditems[0])
                {
                    case "Node":
                        _mode = FoldMode.Node;
                        break;
                    case "Applied Displ":
                        _mode = FoldMode.Applied_Displ;
                        break;
                    case "Settlement":
                        _mode = FoldMode.Settlements;
                        break;
                }
            }
            else
            {
                switch (i)
                {
                    case 1:
                        forceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selecteditems[1]);
                        break;
                    case 2:
                        lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[2]);
                        break;
                }

                (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            }

            // update input params
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        private void UpdateUIFromSelectedItems()
        {
            _mode = (FoldMode)Enum.Parse(typeof(FoldMode), selecteditems[0]);
            forceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selecteditems[1]);
            lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[2]);

            CreateAttributes();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        #endregion

        #region Input and output
        readonly List<string> loadTypeOptions = new List<string>(new string[]
        {
            "Node",
            "Applied Displ",
            "Settlement"
        });

        // list of lists with all dropdown lists conctent
        List<List<string>> dropdownitems;
        // list of selected items
        List<string> selecteditems;
        // list of descriptions 
        List<string> spacerDescriptions = new List<string>(new string[]
        {
            "Load Type",
            "Unit",
        });

        private ForceUnit forceUnit = Units.ForceUnit;
        private LengthUnit lengthUnit = Units.LengthUnitGeometry;

        #endregion

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            IQuantity force = new Force(0, forceUnit);
            string forceUnitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
            IQuantity length = new Length(0, lengthUnit);
            string lengthUnitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
            string funit = forceUnitAbbreviation + ", " + forceUnitAbbreviation + lengthUnitAbbreviation;

            pManager.AddIntegerParameter("Load case", "LC", "Load case number (default 1)", GH_ParamAccess.item, 1);
            pManager.AddTextParameter("Node list", "No", "List of Nodes to apply load to." + System.Environment.NewLine +
                 "Node list should take the form:" + System.Environment.NewLine +
                 " 1 11 to 72 step 2 not (XY3 31 to 45)" + System.Environment.NewLine +
                 "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "Na", "Load Name", GH_ParamAccess.item);
            pManager.AddTextParameter("Direction", "Di", "Load direction (default z)." +
                    System.Environment.NewLine + "Accepted inputs are:" +
                    System.Environment.NewLine + "x" +
                    System.Environment.NewLine + "y" +
                    System.Environment.NewLine + "z" +
                    System.Environment.NewLine + "xx" +
                    System.Environment.NewLine + "yy" +
                    System.Environment.NewLine + "zz", GH_ParamAccess.item, "z");
            pManager.AddGenericParameter("Value [" + funit + "]", "V", "Load Value", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;

        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Node Load", "Ld", "GSA Node Load", GH_ParamAccess.item);
        }
        
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            
            GsaNodeLoad nodeLoad = new GsaNodeLoad();

            // Node load type
            switch (_mode)
            {
                case FoldMode.Node:
                    nodeLoad.NodeLoadType = GsaNodeLoad.NodeLoadTypes.NODE_LOAD;
                    break;
                case FoldMode.Applied_Displ:
                    nodeLoad.NodeLoadType = GsaNodeLoad.NodeLoadTypes.APPLIED_DISP;
                    break;
                case FoldMode.Settlements:
                    nodeLoad.NodeLoadType = GsaNodeLoad.NodeLoadTypes.SETTLEMENT;
                    break;
            }
            
            // 0 Load case
            int lc = 1;
            GH_Integer gh_lc = new GH_Integer();
            if (DA.GetData(0, ref gh_lc))
                GH_Convert.ToInt32(gh_lc, out lc, GH_Conversion.Both);
            nodeLoad.NodeLoad.Case = lc;

            // 1 element/beam list
            // check that user has not inputted Gsa geometry elements here
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(1, ref gh_typ))
            {
                string type = gh_typ.Value.ToString().ToUpper();
                if (type.StartsWith("GSA "))
                {
                    Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                        "You cannot input a Node/Element/Member in NodeList input!" + System.Environment.NewLine +
                        "Element list should take the form:" + System.Environment.NewLine +
                        "'1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)'" + System.Environment.NewLine +
                        "Refer to GSA help file for definition of lists and full vocabulary.");
                    return;
                }
            }
            string nodeList = "all"; 
            GH_String gh_nl = new GH_String();
            if (DA.GetData(1, ref gh_nl))
                GH_Convert.ToString(gh_nl, out nodeList, GH_Conversion.Both);
            nodeLoad.NodeLoad.Nodes = nodeList;

            // 3 Name
            string name = "";
            GH_String gh_name = new GH_String();
            if (DA.GetData(3, ref gh_name))
            {
                if (GH_Convert.ToString(gh_name, out name, GH_Conversion.Both))
                    nodeLoad.NodeLoad.Name = name;
            }

            // 3 direction
            string dir = "Z";
            Direction direc = Direction.Z;

            GH_String gh_dir = new GH_String();
            if (DA.GetData(3, ref gh_dir))
                GH_Convert.ToString(gh_dir, out dir, GH_Conversion.Both);
            dir = dir.ToUpper().Trim();
            if (dir == "X")
                direc = Direction.X;
            if (dir == "Y")
                direc = Direction.Y;
            if (dir == "XX")
                direc = Direction.XX;
            if (dir == "YY")
                direc = Direction.YY;
            if (dir == "ZZ")
                direc = Direction.ZZ;

            nodeLoad.NodeLoad.Direction = direc;

            double load = 0;
            if (_mode == FoldMode.Node)
            {
                switch (dir)
                {
                    case "X":
                    case "Y":
                    case "Z":
                        load = GetInput.Force(this, DA, 4, forceUnit).Newtons;
                        break;
                    case "XX":
                    case "YY":
                    case "ZZ":
                        load = GetInput.Force(this, DA, 4, forceUnit).Newtons * new Length(1, lengthUnit).Meters;
                        break;
                }
            }
            else
            {
                switch (dir)
                {
                    case "X":
                    case "Y":
                    case "Z":
                        load = GetInput.Length(this, DA, 4, lengthUnit).Meters;
                        break;
                    case "XX":
                    case "YY":
                    case "ZZ":
                        load = GetInput.Angle(this, DA, 4, AngleUnit.Radian).Radians;
                        break;
                }
            }

            nodeLoad.NodeLoad.Value = load;

            GsaLoad gsaLoad = new GsaLoad(nodeLoad);
            DA.SetData(0, new GsaLoadGoo(gsaLoad));
        }

        #region menu override
        private enum FoldMode
        {
            Node,
            Applied_Displ,
            Settlements
        }
        private bool first = true;
        private FoldMode _mode = FoldMode.Node;
        #endregion

        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            Util.GH.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            try // this will fail if user has an old version of the component
            {
                Util.GH.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);
            }
            catch (Exception) // we set the stored values like first initation of component
            {
                _mode = (FoldMode)reader.GetInt32("Mode");

                dropdownitems = new List<List<string>>();
                dropdownitems.Add(loadTypeOptions);
                dropdownitems.Add(Units.FilteredForceUnits);
                dropdownitems.Add(Units.FilteredLengthUnits);

                selecteditems = new List<string>();
                selecteditems.Add(reader.GetString("select"));
                selecteditems.Add(ForceUnit.Kilonewton.ToString());
                selecteditems.Add(LengthUnit.Meter.ToString());
            }
            first = false;

            UpdateUIFromSelectedItems();
            return base.Read(reader);
        }

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
        #endregion
        #region IGH_VariableParameterComponent null implementation
        void IGH_VariableParameterComponent.VariableParameterMaintenance()
        {
            IQuantity force = new Force(0, forceUnit);
            string forceUnitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
            IQuantity length = new Length(0, lengthUnit);
            string lengthUnitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
            string funit = forceUnitAbbreviation + ", " + forceUnitAbbreviation + lengthUnitAbbreviation;
            string lunit = lengthUnitAbbreviation + ", rad";

            if (_mode == FoldMode.Node)
            {
                Params.Input[4].Name = "Value [" + funit + "]";
            }
            else
            {
                Params.Input[4].Name = "Value [" + lunit + "]";
            }
        }
        #endregion
    }
}
