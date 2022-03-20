using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using UnitsNet.Units;
using UnitsNet;
using System.Linq;
using Oasys.Units;
using GsaGH.Util.GH;
using GsaGH.Util.Gsa;
using UnitsNet.GH;
using Grasshopper;
using Grasshopper.Kernel.Data;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to retrieve non-geometric objects from a GSA model
    /// </summary>
    public class ReactionForce : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("4f06d674-c736-4d9c-89d9-377bc424c547");
        public ReactionForce()
          : base("Reaction Forces", "ReacForce", "Reaction Force result values",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat5())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.ReactionForces;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (first)
            {
                dropdownitems = new List<List<string>>();
                selecteditems = new List<string>();

                // force
                dropdownitems.Add(Units.FilteredForceUnits);
                selecteditems.Add(forceUnit.ToString());

                // moment
                dropdownitems.Add(Units.FilteredMomentUnits);
                selecteditems.Add(momentUnit.ToString());


                first = false;
            }
            m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
        }
        public void SetSelected(int i, int j)
        {
            // change selected item
            selecteditems[i] = dropdownitems[i][j];
            if (i == 0)
                forceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selecteditems[i]);
            else if (i == 1)
                momentUnit = (MomentUnit)Enum.Parse(typeof(MomentUnit), selecteditems[i]);

            // update name of inputs (to display unit on sliders)
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        private void UpdateUIFromSelectedItems()
        {
            forceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selecteditems[0]);
            momentUnit = (MomentUnit)Enum.Parse(typeof(MomentUnit), selecteditems[1]);

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
            "Force Unit",
            "Moment Unit"
        });
        private bool first = true;
        private ForceUnit forceUnit = Units.ForceUnit;
        private MomentUnit momentUnit = Units.MomentUnit;
        string forceunitAbbreviation;
        string momentunitAbbreviation;
        #endregion

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Result", "Res", "GSA Result", GH_ParamAccess.list);
            pManager.AddTextParameter("Node filter list", "No", "Filter results by list." + System.Environment.NewLine +
                "Node list should take the form:" + System.Environment.NewLine +
                " 1 11 to 72 step 2 not (XY3 31 to 45)" + System.Environment.NewLine +
                "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            IQuantity force = new Force(0, forceUnit);
            forceunitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
            momentunitAbbreviation = Moment.GetAbbreviation(momentUnit);

            string note = System.Environment.NewLine + "DataTree organised as { CaseID ; (Permutation) } where each" +
                System.Environment.NewLine + "branch contains a list matching the NodeIDs in the ID output.";

            pManager.AddGenericParameter("Force X [" + forceunitAbbreviation + "]", "Fx", "Reaction Forces in X-direction in Global Axis." + note, GH_ParamAccess.tree);
            pManager.AddGenericParameter("Force Y [" + forceunitAbbreviation + "]", "Fy", "Reaction Forces in Y-direction in Global Axis." + note, GH_ParamAccess.tree);
            pManager.AddGenericParameter("Force Z [" + forceunitAbbreviation + "]", "Fz", "Reaction Forces in Z-direction in Global Axis." + note, GH_ParamAccess.tree);
            pManager.AddGenericParameter("Force |XYZ| [" + forceunitAbbreviation + "]", "|F|", "Combined |XYZ| Reaction Forces in Global Axis." + note, GH_ParamAccess.tree);
            pManager.AddGenericParameter("Moment XX [" + momentunitAbbreviation + "]", "Mxx", "Reaction Moments around X-axis in Global Axis." + note, GH_ParamAccess.tree);
            pManager.AddGenericParameter("Moment YY [" + momentunitAbbreviation + "]", "Myy", "Reaction Moments Y-axis in Global Axis." + note, GH_ParamAccess.tree);
            pManager.AddGenericParameter("Moment ZZ [" + momentunitAbbreviation + "]", "Mzz", "Reaction Moments Z-axis in Global Axis." + note, GH_ParamAccess.tree);
            pManager.AddGenericParameter("Moment |XYZ| [" + momentunitAbbreviation + "]", "|M|", "Combined |XXYYZZ| Reaction Moments in Global Axis." + note, GH_ParamAccess.tree);
            pManager.AddTextParameter("Nodes IDs", "ID", "Node IDs for each result value", GH_ParamAccess.list);
        }
        

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Result to work on
            GsaResult result = new GsaResult();

            // Get filer case
            string nodeList = "All";
            GH_String gh_Type = new GH_String();
            if (DA.GetData(1, ref gh_Type))
                GH_Convert.ToString(gh_Type, out nodeList, GH_Conversion.Both);

            // data trees to output
            DataTree<GH_UnitNumber> out_transX = new DataTree<GH_UnitNumber>();
            DataTree<GH_UnitNumber> out_transY = new DataTree<GH_UnitNumber>();
            DataTree<GH_UnitNumber> out_transZ = new DataTree<GH_UnitNumber>();
            DataTree<GH_UnitNumber> out_transXYZ = new DataTree<GH_UnitNumber>();
            DataTree<GH_UnitNumber> out_rotX = new DataTree<GH_UnitNumber>();
            DataTree<GH_UnitNumber> out_rotY = new DataTree<GH_UnitNumber>();
            DataTree<GH_UnitNumber> out_rotZ = new DataTree<GH_UnitNumber>();
            DataTree<GH_UnitNumber> out_rotXYZ = new DataTree<GH_UnitNumber>();

            // Get Model
            List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
            if (DA.GetDataList(0, gh_types))
            {
                List<GsaResult> results = new List<GsaResult>();
                
                for (int i = 0; i < gh_types.Count; i++)
                {
                    GH_ObjectWrapper gh_typ = gh_types[i];
                    if (gh_typ.Value is GsaResultGoo)
                    {
                        result = ((GsaResultGoo)gh_typ.Value).Value;
                    }
                    else
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Result");
                        return;
                    }
                    
                    List<GsaResultsValues> vals = result.NodeReactionForceValues(nodeList, forceUnit, momentUnit);

                    for (int permutation = 0; permutation < vals.Count; permutation++)
                    {
                        GH_Path p = p = new GH_Path(result.CaseID, permutation + 1);

                        List<GH_UnitNumber> transX = new List<GH_UnitNumber>();
                        List<GH_UnitNumber> transY = new List<GH_UnitNumber>();
                        List<GH_UnitNumber> transZ = new List<GH_UnitNumber>();
                        List<GH_UnitNumber> transXYZ = new List<GH_UnitNumber>();
                        List<GH_UnitNumber> rotX = new List<GH_UnitNumber>();
                        List<GH_UnitNumber> rotY = new List<GH_UnitNumber>();
                        List<GH_UnitNumber> rotZ = new List<GH_UnitNumber>();
                        List<GH_UnitNumber> rotXYZ = new List<GH_UnitNumber>();

                        Parallel.For(0, 2, item => // split into two tasks
                        {
                            if (item == 0)
                            {
                                foreach (ConcurrentDictionary<int, GsaResultQuantity> res in vals[permutation].xyzResults.Values)
                                {
                                    GsaResultQuantity values = res[0]; // there is only one result per node
                                    transX.Add(new GH_UnitNumber(values.X.ToUnit(forceUnit))); // use ToUnit to capture changes in dropdown
                                    transY.Add(new GH_UnitNumber(values.Y.ToUnit(forceUnit)));
                                    transZ.Add(new GH_UnitNumber(values.Z.ToUnit(forceUnit)));
                                    transXYZ.Add(new GH_UnitNumber(values.XYZ.ToUnit(forceUnit)));
                                }
                            }
                            if (item == 1)
                            {
                                foreach (ConcurrentDictionary<int, GsaResultQuantity> res in vals[permutation].xxyyzzResults.Values)
                                {
                                    GsaResultQuantity values = res[0]; // there is only one result per node
                                    rotX.Add(new GH_UnitNumber(values.X.ToUnit(momentUnit))); // use ToUnit to capture changes in dropdown
                                    rotY.Add(new GH_UnitNumber(values.Y.ToUnit(momentUnit)));
                                    rotZ.Add(new GH_UnitNumber(values.Z.ToUnit(momentUnit)));
                                    rotXYZ.Add(new GH_UnitNumber(values.XYZ.ToUnit(momentUnit)));
                                }
                            }
                        });

                        out_transX.AddRange(transX, p);
                        out_transY.AddRange(transY, p);
                        out_transZ.AddRange(transZ, p);
                        out_transXYZ.AddRange(transXYZ, p);
                        out_rotX.AddRange(rotX, p);
                        out_rotY.AddRange(rotY, p);
                        out_rotZ.AddRange(rotZ, p);
                        out_rotXYZ.AddRange(rotXYZ, p);
                    }

                    if (i == 0)
                        DA.SetDataList(8, vals.First().xyzResults.Keys.ToList());
                }

                DA.SetDataTree(0, out_transX);
                DA.SetDataTree(1, out_transY);
                DA.SetDataTree(2, out_transZ);
                DA.SetDataTree(3, out_transXYZ);
                DA.SetDataTree(4, out_rotX);
                DA.SetDataTree(5, out_rotY);
                DA.SetDataTree(6, out_rotZ);
                DA.SetDataTree(7, out_rotXYZ);
                
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

            first = false;
            UpdateUIFromSelectedItems();

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
            IQuantity force = new Force(0, forceUnit);
            forceunitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
            momentunitAbbreviation = Moment.GetAbbreviation(momentUnit);

            int i = 0;
            Params.Output[i++].Name = "Force X [" + forceunitAbbreviation + "]";
            Params.Output[i++].Name = "Force Y [" + forceunitAbbreviation + "]";
            Params.Output[i++].Name = "Force Z [" + forceunitAbbreviation + "]";
            Params.Output[i++].Name = "Force |XYZ| [" + forceunitAbbreviation + "]";
            Params.Output[i++].Name = "Moment XX [" + momentunitAbbreviation + "]";
            Params.Output[i++].Name = "Moment YY [" + momentunitAbbreviation + "]";
            Params.Output[i++].Name = "Moment ZZ [" + momentunitAbbreviation + "]";
            Params.Output[i++].Name = "Moment |XXYYZZ| [" + momentunitAbbreviation + "]";
        }
        #endregion  
    }
}

