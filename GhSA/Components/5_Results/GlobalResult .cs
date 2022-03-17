using System;
using System.Collections.Generic;
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

namespace GsaGH.Components
{
    /// <summary>
    /// Component to retrieve non-geometric objects from a GSA model
    /// </summary>
    public class GlobalResult : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("14fc0d64-7e34-47e8-bb6d-129029732474");
        public GlobalResult()
          : base("Global Results", "GlobalResult", "Get Global Results from GSA model",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat5())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.ResultGlobal;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        #endregion
        public override void CreateAttributes()
        {
            if (first)
            {
                dropdownitems = new List<List<string>>();
                dropdownitems.Add(Units.FilteredForceUnits);
                dropdownitems.Add(Units.FilteredMomentUnits);
                dropdownitems.Add(Units.FilteredMassUnits);
                dropdownitems.Add(Units.FilteredAreaMomentOfInertiaUnits);
                dropdownitems.Add(Units.FilteredForcePerLengthUnits);

                selecteditems = new List<string>();
                selecteditems.Add(Units.ForceUnit.ToString());
                selecteditems.Add(Units.MomentUnit.ToString());
                selecteditems.Add(Units.MassUnit.ToString());
                if (Units.LengthUnitGeometry == LengthUnit.Foot | Units.LengthUnitGeometry == LengthUnit.Inch)
                    selecteditems.Add(AreaMomentOfInertiaUnit.InchToTheFourth.ToString());
                else
                    selecteditems.Add(AreaMomentOfInertiaUnit.MeterToTheFourth.ToString());
                selecteditems.Add(Units.ForcePerLengthUnit.ToString());

                first = false;
            }

            m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
        }
        public void SetSelected(int i, int j)
        {
            // change selected item
            selecteditems[i] = dropdownitems[i][j];

            switch (i)
            {
                case 0:
                    forceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selecteditems[i]);
                    break;
                case 1:
                    momentUnit = (MomentUnit)Enum.Parse(typeof(MomentUnit), selecteditems[i]);
                    break;
                case 2:
                    massUnit = (MassUnit)Enum.Parse(typeof(MassUnit), selecteditems[i]);
                    break;
                case 3:
                    inertiaUnit = (AreaMomentOfInertiaUnit)Enum.Parse(typeof(AreaMomentOfInertiaUnit), selecteditems[i]);
                    break;
                case 4:
                    forcePerLengthUnit = (ForcePerLengthUnit)Enum.Parse(typeof(ForcePerLengthUnit), selecteditems[i]);
                    break;
            }
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        private void UpdateUIFromSelectedItems()
        {
            forceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selecteditems[0]);
            momentUnit = (MomentUnit)Enum.Parse(typeof(MomentUnit), selecteditems[1]);
            massUnit = (MassUnit)Enum.Parse(typeof(MassUnit), selecteditems[2]);
            inertiaUnit = (AreaMomentOfInertiaUnit)Enum.Parse(typeof(AreaMomentOfInertiaUnit), selecteditems[3]);
            forcePerLengthUnit = (ForcePerLengthUnit)Enum.Parse(typeof(ForcePerLengthUnit), selecteditems[4]);

            CreateAttributes();
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
            "Moment Unit",
            "Mass Unit",
            "Inertia Unit",
            "Stiffness Unit",
        });

        private ForceUnit forceUnit = Units.ForceUnit;
        private MomentUnit momentUnit = Units.MomentUnit;
        private MassUnit massUnit = Units.MassUnit;
        private AreaMomentOfInertiaUnit inertiaUnit = Units.SectionAreaMomentOfInertiaUnit;
        private ForcePerLengthUnit forcePerLengthUnit = Units.ForcePerLengthUnit;
        bool first = true;
        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA Model", "GSA", "GSA model containing some results", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Load Case", "LC", "Load Case (default 1)", GH_ParamAccess.item, 1);
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            IQuantity force = new Force(0, forceUnit);
            string forceUnitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
            string momentunitAbbreviation = Oasys.Units.Moment.GetAbbreviation(momentUnit);
            IQuantity mass = new Mass(0, massUnit);
            string massUnitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
            IQuantity inertia = new AreaMomentOfInertia(0, inertiaUnit);
            string inertiaUnitAbbreviation = string.Concat(inertia.ToString().Where(char.IsLetter));
            IQuantity forceperlength = new ForcePerLength(0, forcePerLengthUnit);
            string forceperlengthUnitAbbreviation = string.Concat(forceperlength.ToString().Where(char.IsLetter));

            pManager.AddVectorParameter("Total Force Loads [" + forceUnitAbbreviation + "]", "ΣF", "Sum of all Force Loads in GSA Model", GH_ParamAccess.item);
            pManager.AddVectorParameter("Total Moment Loads [" + momentunitAbbreviation + "]", "ΣM", "Sum of all Moment Loads in GSA Model", GH_ParamAccess.item);
            pManager.AddVectorParameter("Total Force Reactions [" + forceUnitAbbreviation + "]", "ΣRf", "Sum of all Rection Forces in GSA Model", GH_ParamAccess.item);
            pManager.AddVectorParameter("Total Moment Reactions [" + momentunitAbbreviation + "]", "ΣRm", "Sum of all Reaction Moments in GSA Model", GH_ParamAccess.item);
            pManager.AddVectorParameter("Effective Mass [" + massUnitAbbreviation + "]", "Σkg", "Effective Mass in GSA Model", GH_ParamAccess.item);
            pManager.AddVectorParameter("Effective Inertia [" + inertiaUnitAbbreviation + "]", "ΣI", "Effective Inertia in GSA Model", GH_ParamAccess.item);
            pManager.AddNumberParameter("Mode", "Mo", "Mode number if LC is a dynamic task", GH_ParamAccess.item);
            pManager.AddGenericParameter("Modal Mass [" + massUnitAbbreviation + "]", "MM", "Modal Mass of selected LoadCase / mode", GH_ParamAccess.item);
            pManager.AddGenericParameter("Modal Stiffness [" + forceperlengthUnitAbbreviation + "]", "MS", "Modal Stiffness of selected LoadCase / mode", GH_ParamAccess.item);
            pManager.AddGenericParameter("Modal Geometric Stiffness [" + forceperlengthUnitAbbreviation + "]", "MGS", "Modal Geometric Stiffness of selected LoadCase / mode", GH_ParamAccess.item);
            pManager.AddGenericParameter("Frequency [Hz]", "f", "Frequency of selected LoadCase / mode", GH_ParamAccess.item);
            pManager.AddNumberParameter("Load Factor", "LF", "Load Factor for selected LoadCase / mode", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Model to work on
            GsaModel gsaModel = new GsaModel();

            // Get Model
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(0, ref gh_typ))
            {
                #region Inputs
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
                #endregion

                #region Get results from GSA
                // ### Get results ###
                //Get analysis case from model
                AnalysisCaseResult analysisCaseResult = null;
                gsaModel.Model.Results().TryGetValue(analCase, out analysisCaseResult);
                if (analysisCaseResult == null)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No results exist for Analysis Case " + analCase + " in file");
                    return;
                }
                #endregion

                Vector3d force = ResultHelper.GetResult(analysisCaseResult.Global.TotalLoad, forceUnit);
                Vector3d moment = ResultHelper.GetResult(analysisCaseResult.Global.TotalLoad, momentUnit);
                Vector3d reaction = ResultHelper.GetResult(analysisCaseResult.Global.TotalReaction, forceUnit);
                Vector3d reactionmoment = ResultHelper.GetResult(analysisCaseResult.Global.TotalReaction, momentUnit);
                Vector3d effMass = ResultHelper.GetResult(analysisCaseResult.Global.EffectiveMass, massUnit);

                Vector3d effStiff;
                if (analysisCaseResult.Global.EffectiveInertia != null)
                {
                    effStiff = ResultHelper.GetResult(analysisCaseResult.Global.EffectiveInertia, inertiaUnit);
                }
                else
                    effStiff = new Vector3d();

                Mass mass = new Mass(analysisCaseResult.Global.ModalMass, MassUnit.Kilogram);
                Mass out_Mass = new Mass(mass.As(massUnit), massUnit);

                ForcePerLength modstiff = new ForcePerLength(analysisCaseResult.Global.ModalStiffness, ForcePerLengthUnit.NewtonPerMeter);
                ForcePerLength out_modstiff = new ForcePerLength(modstiff.As(forcePerLengthUnit), forcePerLengthUnit);

                ForcePerLength geostiff = new ForcePerLength(analysisCaseResult.Global.ModalGeometricStiffness, ForcePerLengthUnit.NewtonPerMeter);
                ForcePerLength out_geostiff = new ForcePerLength(geostiff.As(forcePerLengthUnit), forcePerLengthUnit);


                DA.SetData(0, force);
                DA.SetData(1, moment);
                DA.SetData(2, reaction);
                DA.SetData(3, reactionmoment);
                DA.SetData(4, effMass);
                DA.SetData(5, effStiff);
                DA.SetData(6, analysisCaseResult.Global.Mode);
                DA.SetData(7, new GH_UnitNumber(out_Mass));
                DA.SetData(8, new GH_UnitNumber(out_modstiff));
                DA.SetData(9, new GH_UnitNumber(out_geostiff));
                DA.SetData(10, new GH_UnitNumber(new Frequency(analysisCaseResult.Global.Frequency, FrequencyUnit.Hertz)));
                DA.SetData(11, analysisCaseResult.Global.LoadFactor);
            }
        }
        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);
            
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
            string forceUnitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
            string momentunitAbbreviation = Oasys.Units.Moment.GetAbbreviation(momentUnit);
            IQuantity mass = new Mass(0, massUnit);
            string massUnitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
            IQuantity inertia = new AreaMomentOfInertia(0, inertiaUnit);
            string inertiaUnitAbbreviation = string.Concat(inertia.ToString().Where(char.IsLetter));
            IQuantity forceperlength = new ForcePerLength(0, forcePerLengthUnit);
            string forceperlengthUnitAbbreviation = string.Concat(forceperlength.ToString().Where(char.IsLetter));

            int i = 0;
            Params.Output[i++].Name = "Total Force Loads [" + forceUnitAbbreviation + "]";
            Params.Output[i++].Name = "Total Moment Loads [" + momentunitAbbreviation + "]";
            Params.Output[i++].Name = "Total Force Reactions [" + forceUnitAbbreviation + "]";
            Params.Output[i++].Name = "Total Moment Reactions [" + momentunitAbbreviation + "]";
            Params.Output[i++].Name = "Effective Mass [" + massUnitAbbreviation + "]";
            Params.Output[i++].Name = "Effective Inertia [" + inertiaUnitAbbreviation + "]";
            i++;
            Params.Output[i++].Name = "Modal Mass [" + massUnitAbbreviation + "]";
            Params.Output[i++].Name = "Modal Stiffness [" + forceperlengthUnitAbbreviation + "]";
            Params.Output[i++].Name = "Modal Geometric Stiffness [" + forceperlengthUnitAbbreviation + "]";
        }
        #endregion  
    }
}

