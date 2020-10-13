using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;

using Grasshopper.Kernel.Parameters;
using GsaAPI;
using GhSA.Parameters;
using System.Resources;
using System.Linq;

namespace GhSA.Components
{
    /// <summary>
    /// Component to create a new Prop2d
    /// </summary>
    public class NodeResults : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("7a8250b8-781e-4aeb-a05b-1b1fce9fbe3b");
        public NodeResults()
          : base("Node Results", "NodeResult", "Get GSA Node Results",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat5())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        //protected override Bitmap Icon => Resources.CrossSections;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            m_attributes = new UI.DropDownComponentUI(this, SetSelected, dropdownitems, selecteditem, "Result Type");
        }

        public void SetSelected(string selected)
        {
            selecteditem = selected;
            switch (selected)
            {
                case "Displacement":
                    Mode1Clicked();
                    break;
                case "Reaction":
                    Mode2Clicked();
                    break;
                case "Spring Force":
                    Mode3Clicked();
                    break;
                case "Constraint":
                    Mode4Clicked();
                    break;
            }
        }
        #endregion

        #region Input and output
        readonly List<string> dropdownitems = new List<string>(new string[]
        {
            "Displacement",
            "Reaction",
            "Spring Force"
            //"Constraint"
        });

        string selecteditem = "Displacement";

        #endregion

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA Model", "GSA", "GSA model containing some results", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Load Case", "LC", "Load Case (default 1)", GH_ParamAccess.item, 1);
            pManager.AddTextParameter("Node filter list", "No", "Filter results by list." + System.Environment.NewLine +
                "Node list should take the form:" + System.Environment.NewLine +
                " 1 11 to 72 step 2 not (XY3 31 to 45)" + System.Environment.NewLine +
                "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
            pManager.AddColourParameter("Colour", "Col", "Optional list of colours to override default colours." +
                System.Environment.NewLine + "A new gradient will be created from the input list of colours", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddVectorParameter("Translation", "U", "X, Y, Z translation values (" + Util.GsaUnit.LengthSmall + ")", GH_ParamAccess.list);
            pManager.AddVectorParameter("Rotation", "R", "XX, YY, ZZ rotation values (" + Util.GsaUnit.Angle + ")", GH_ParamAccess.list);
            pManager.AddPointParameter("Point", "P", "Position", GH_ParamAccess.list);
            pManager.AddGenericParameter("Colours", "C", "Legend Colours", GH_ParamAccess.list);
            pManager.AddGenericParameter("Values", "T", "Legend Values (" + Util.GsaUnit.LengthSmall + ")", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Model to work on
            GsaModel gsaModel = new GsaModel();
            
            // Get Model
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(0, ref gh_typ))
            {
                if (gh_typ.Value is GsaModelGoo)
                    gh_typ.CastTo(ref gsaModel);
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Model");
                    return;
                }

                // Get analysis case 
                GH_Integer gh_aCase = new GH_Integer();
                DA.GetData(1, ref gh_aCase);
                int analCase = 1;
                GH_Convert.ToInt32(gh_aCase, out analCase, GH_Conversion.Both);

                // Get node filter list
                GH_String gh_noList = new GH_String();
                DA.GetData(2, ref gh_noList);
                string nodeList = "all";
                GH_Convert.ToString(gh_noList, out nodeList, GH_Conversion.Both);

                // Get colours
                List<Grasshopper.Kernel.Types.GH_Colour> gh_Colours = new List<Grasshopper.Kernel.Types.GH_Colour>();
                List<System.Drawing.Color> colors = new List<System.Drawing.Color>();
                if (DA.GetDataList(3, gh_Colours))
                {
                    for (int i = 0; i < gh_Colours.Count; i++)
                    {
                        System.Drawing.Color color = new System.Drawing.Color();
                        GH_Convert.ToColor_Primary(gh_Colours[i], ref color);
                        colors.Add(color);
                    }
                }
                Grasshopper.GUI.Gradient.GH_Gradient gH_Gradient = UI.Colour.Deflection_Gradient(colors);

                //Get analysis case from model
                AnalysisCaseResult analysisCaseResult = null;
                gsaModel.Model.Results().TryGetValue(analCase, out analysisCaseResult);
                if (analysisCaseResult == null)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Analysis Case " + analCase + " does not exist in file,");
                    return;
                }
                IReadOnlyDictionary<int, NodeResult> results = analysisCaseResult.NodeResults(nodeList);

                List<Vector3d> xyz = new List<Vector3d>();
                List<Vector3d> xxyyzz = new List<Vector3d>();

                double dmax = 0;
                double unitfactor = 1;

                foreach (var key in results.Keys)
                {
                    NodeResult result;
                    Double6 values = null;
                    double d = 0;
                    
                    results.TryGetValue(key, out result);
                    switch (_mode)
                    {
                        case (FoldMode.Displacement):
                            values = result.Displacement;
                            unitfactor = 1;
                            break;
                        case (FoldMode.Reaction):
                            values = result.Reaction;
                            unitfactor = 1000;
                            break;
                        case (FoldMode.SpringForce):
                            values = result.SpringForce;
                            unitfactor = 1000;
                            break;
                        case (FoldMode.Constraint):
                            values = result.Constraint;
                            break;
                    }
                    switch (_disp)
                    {
                        case (DisplayValue.X):
                            d = values.X;
                            break;
                        case (DisplayValue.Y):
                            d = values.Y;
                            break;
                        case (DisplayValue.Z):
                            d = values.Z;
                            break;
                        case (DisplayValue.XX):
                            d = values.XX;
                            break;
                        case (DisplayValue.YY):
                            d = values.YY;
                            break;
                        case (DisplayValue.ZZ):
                            d = values.ZZ;
                            break;
                    }
                    if (Math.Abs(d) > dmax)
                        dmax = Math.Abs(d);
                        
                    xyz.Add(new Vector3d(values.X / unitfactor, values.Y / unitfactor, values.Z / unitfactor));
                    xxyyzz.Add(new Vector3d(values.XX / unitfactor, values.YY / unitfactor, values.ZZ / unitfactor));
                }

                //Find Colour and Values for legend output
                
                double t = 0;
                List<double> ts = new List<double>();
                List<System.Drawing.Color> cs = new List<System.Drawing.Color>();
                for (int i = 0; i < gH_Gradient.GripCount; i++)
                {
                    t = dmax / ((double)gH_Gradient.GripCount - 1.0) * (double)i;
                    t = Math.Round(t, 3);
                    ts.Add(t);
                    cs.Add(gH_Gradient.ColourAt(1.0 / ((double)gH_Gradient.GripCount - 1.0) * (double)i));
                }

                List<GsaNode> nodes = Util.Gsa.GsaImport.GsaGetPoint(gsaModel.Model, nodeList, false);
                List<Point3d> pts = new List<Point3d>();
                List<System.Drawing.Color> col = new List<System.Drawing.Color>();
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i] != null)
                    {
                        pts.Add(nodes[i].Point);
                        t = xyz[i].Z * unitfactor;
                        if (dmax != 0)
                            col.Add(gH_Gradient.ColourAt(t / dmax));
                        else
                            col.Add(System.Drawing.Color.Transparent);
                    }
                }
                //nodes.Select(x => x.Point).ToList()
                
                DA.SetDataList(0, xyz);
                DA.SetDataList(1, xxyyzz);
                DA.SetDataList(2, pts);
                DA.SetDataList(3, col);
                DA.SetDataList(4, ts);
            }
        }
        
        #region menu override
        private enum FoldMode
        {
            Displacement,
            Reaction,
            SpringForce,
            Constraint
        }
        private FoldMode _mode = FoldMode.Displacement;

        private enum DisplayValue
        {
            X,
            Y,
            Z,
            XX,
            YY,
            ZZ
        }
        private DisplayValue _disp = DisplayValue.Z;

        private void Mode1Clicked()
        {
            if (_mode == FoldMode.Displacement)
                return;

            RecordUndoEvent(_mode.ToString() + " Parameters");
            _mode = FoldMode.Displacement;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode2Clicked()
        {
            if (_mode == FoldMode.Reaction)
                return;

            RecordUndoEvent(_mode.ToString() + " Parameters");
            _mode = FoldMode.Reaction;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode3Clicked()
        {
            if (_mode == FoldMode.SpringForce)
                return;

            RecordUndoEvent(_mode.ToString() + " Parameters");
            _mode = FoldMode.SpringForce;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        private void Mode4Clicked()
        {
            if (_mode == FoldMode.Constraint)
                return;

            RecordUndoEvent(_mode.ToString() + " Parameters");
            _mode = FoldMode.Constraint;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        #endregion
        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetInt32("Mode", (int)_mode);
            writer.SetString("select", selecteditem);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            _mode = (FoldMode)reader.GetInt32("Mode");
            selecteditem = reader.GetString("select");
            this.CreateAttributes();
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
            
            if (_mode == FoldMode.Displacement)
            {
                Params.Output[0].NickName = "U";
                Params.Output[0].Name = "Translation";
                Params.Output[0].Description = "X, Y, Z Translation values (" + Util.GsaUnit.LengthSmall + ")";

                Params.Output[1].NickName = "R";
                Params.Output[1].Name = "Rotation";
                Params.Output[1].Description = "XX, YY, ZZ Rotation values (" + Util.GsaUnit.Angle + ")";
            }

            if (_mode == FoldMode.Reaction | _mode == FoldMode.SpringForce)
            {
                Params.Output[0].NickName = "F";
                Params.Output[0].Name = "Force";
                Params.Output[0].Description = "X, Y, Z Force values (" + Util.GsaUnit.LengthSmall + ")";

                Params.Output[1].NickName = "M";
                Params.Output[1].Name = "Moment";
                Params.Output[1].Description = "XX, YY, ZZ Moment values (" + Util.GsaUnit.Angle + ")";
            }
        }
        #endregion  
    }
}