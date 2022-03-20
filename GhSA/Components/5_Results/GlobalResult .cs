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
                {
                    selecteditems.Add(AreaMomentOfInertiaUnit.FootToTheFourth.ToString());
                    selecteditems.Add(ForcePerLengthUnit.KilopoundForcePerFoot.ToString());
                }
                else
                {
                    selecteditems.Add(AreaMomentOfInertiaUnit.MeterToTheFourth.ToString());
                    selecteditems.Add(ForcePerLengthUnit.KilonewtonPerMeter.ToString());
                }

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
            pManager.AddGenericParameter("Result", "Res", "GSA Result", GH_ParamAccess.item);
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

            pManager.AddGenericParameter("Total Force X [" + forceUnitAbbreviation + "]", "ΣFx", "Sum of all Force Loads in GSA Model in X-direction", GH_ParamAccess.item);
            pManager.AddGenericParameter("Total Force Y [" + forceUnitAbbreviation + "]", "ΣFy", "Sum of all Force Loads in GSA Model in Y-direction", GH_ParamAccess.item);
            pManager.AddGenericParameter("Total Force Z [" + forceUnitAbbreviation + "]", "ΣFz", "Sum of all Force Loads in GSA Model in Z-direction", GH_ParamAccess.item);
            pManager.AddGenericParameter("Total Force |XYZ| [" + forceUnitAbbreviation + "]", "Σ|F|", "Sum of all Force Loads in GSA Model", GH_ParamAccess.item);
            pManager.AddGenericParameter("Total Moment XX [" + momentunitAbbreviation + "]", "ΣMxx", "Sum of all Moment Loads in GSA Model around X-axis", GH_ParamAccess.item);
            pManager.AddGenericParameter("Total Moment XX  [" + momentunitAbbreviation + "]", "ΣMyy", "Sum of all Moment Loads in GSA Model around Y-axis", GH_ParamAccess.item);
            pManager.AddGenericParameter("Total Moment XX  [" + momentunitAbbreviation + "]", "ΣMzz", "Sum of all Moment Loads in GSA Model around Z-axis", GH_ParamAccess.item);
            pManager.AddGenericParameter("Total Moment |XXYYZZ|  [" + momentunitAbbreviation + "]", "Σ|M|", "Sum of all Moment Loads in GSA Model", GH_ParamAccess.item);
            pManager.AddGenericParameter("Total Reaction X [" + forceUnitAbbreviation + "]", "ΣRx", "Sum of all Reaction Forces in GSA Model in X-direction", GH_ParamAccess.item);
            pManager.AddGenericParameter("Total Reaction Y [" + forceUnitAbbreviation + "]", "ΣRy", "Sum of all Reaction Forces in GSA Model in Y-direction", GH_ParamAccess.item);
            pManager.AddGenericParameter("Total Reaction Z [" + forceUnitAbbreviation + "]", "ΣRz", "Sum of all Reaction Forces in GSA Model in Z-direction", GH_ParamAccess.item);
            pManager.AddGenericParameter("Total Reaction |XYZ| [" + forceUnitAbbreviation + "]", "Σ|Rf|", "Sum of all Reaction Forces in GSA Model", GH_ParamAccess.item);
            pManager.AddGenericParameter("Total Reaction XX [" + momentunitAbbreviation + "]", "ΣRxx", "Sum of all Reaction Moments in GSA Model around X-axis", GH_ParamAccess.item);
            pManager.AddGenericParameter("Total Reaction XX  [" + momentunitAbbreviation + "]", "ΣRyy", "Sum of all Reaction Moments in GSA Model around Y-axis", GH_ParamAccess.item);
            pManager.AddGenericParameter("Total Reaction XX  [" + momentunitAbbreviation + "]", "ΣRzz", "Sum of all Reaction Moments in GSA Model around Z-axis", GH_ParamAccess.item);
            pManager.AddGenericParameter("Total Reaction |XXYYZZ|  [" + momentunitAbbreviation + "]", "Σ|Rm|", "Sum of all Reaction Moments in GSA Model", GH_ParamAccess.item);
            pManager.AddGenericParameter("Effective Mass X [" + massUnitAbbreviation + "]", "Σmx", "Effective Mass in GSA Model in X-direction", GH_ParamAccess.item);
            pManager.AddGenericParameter("Effective Mass Y [" + massUnitAbbreviation + "]", "Σmy", "Effective Mass in GSA Model in Y-direction", GH_ParamAccess.item);
            pManager.AddGenericParameter("Effective Mass Z [" + massUnitAbbreviation + "]", "Σmz", "Effective Mass in GSA Model in Z-direction", GH_ParamAccess.item);
            pManager.AddGenericParameter("Effective Mass |XYZ| [" + massUnitAbbreviation + "]", "Σ|m|", "Effective Mass in GSA Model", GH_ParamAccess.item);
            pManager.AddGenericParameter("Effective Inertia X [" + inertiaUnitAbbreviation + "]", "ΣIx", "Effective Inertia in GSA Model in X-direction", GH_ParamAccess.item);
            pManager.AddGenericParameter("Effective Inertia Y [" + inertiaUnitAbbreviation + "]", "ΣIy", "Effective Inertia in GSA Model in Y-direction", GH_ParamAccess.item);
            pManager.AddGenericParameter("Effective Inertia Z [" + inertiaUnitAbbreviation + "]", "ΣIz", "Effective Inertia in GSA Model in Z-direction", GH_ParamAccess.item);
            pManager.AddGenericParameter("Effective Inertia |XYZ| [" + inertiaUnitAbbreviation + "]", "Σ|I|", "Effective Inertia in GSA Model", GH_ParamAccess.item);
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
            // Result to work on
            GsaResult result = new GsaResult();

            // Get Model
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(0, ref gh_typ))
            {
                #region Inputs
                if (gh_typ.Value is GsaResultGoo)
                {
                    result = ((GsaResultGoo)gh_typ.Value).Value;
                    if (result.Type == GsaResult.ResultType.Combination)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Global Result only available for Analysis Cases");
                        return;
                    }
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Result");
                    return;
                }
                #endregion

                #region Get results from GSA
                // ### Get results ###
                //Get analysis case from model
                AnalysisCaseResult analysisCaseResult = result.AnalysisCaseResult;
                #endregion
                int i = 0;
                GsaResultQuantity f = ResultHelper.GetQuantityResult(analysisCaseResult.Global.TotalLoad, forceUnit);
                DA.SetData(i++, new GH_UnitNumber(f.X));
                DA.SetData(i++, new GH_UnitNumber(f.Y));
                DA.SetData(i++, new GH_UnitNumber(f.Z));
                DA.SetData(i++, new GH_UnitNumber(f.XYZ));

                GsaResultQuantity m = ResultHelper.GetQuantityResult(analysisCaseResult.Global.TotalLoad, momentUnit);
                DA.SetData(i++, new GH_UnitNumber(m.X));
                DA.SetData(i++, new GH_UnitNumber(m.Y));
                DA.SetData(i++, new GH_UnitNumber(m.Z));
                DA.SetData(i++, new GH_UnitNumber(m.XYZ));

                GsaResultQuantity rf = ResultHelper.GetQuantityResult(analysisCaseResult.Global.TotalReaction, forceUnit);
                DA.SetData(i++, new GH_UnitNumber(rf.X));
                DA.SetData(i++, new GH_UnitNumber(rf.Y));
                DA.SetData(i++, new GH_UnitNumber(rf.Z));
                DA.SetData(i++, new GH_UnitNumber(rf.XYZ));

                GsaResultQuantity rm = ResultHelper.GetQuantityResult(analysisCaseResult.Global.TotalReaction, momentUnit);
                DA.SetData(i++, new GH_UnitNumber(rm.X));
                DA.SetData(i++, new GH_UnitNumber(rm.Y));
                DA.SetData(i++, new GH_UnitNumber(rm.Z));
                DA.SetData(i++, new GH_UnitNumber(rm.XYZ));

                GsaResultQuantity mass = ResultHelper.GetQuantityResult(analysisCaseResult.Global.EffectiveMass, massUnit);
                DA.SetData(i++, new GH_UnitNumber(mass.X));
                DA.SetData(i++, new GH_UnitNumber(mass.Y));
                DA.SetData(i++, new GH_UnitNumber(mass.Z));
                DA.SetData(i++, new GH_UnitNumber(mass.XYZ));

                if (analysisCaseResult.Global.EffectiveInertia != null)
                {
                    GsaResultQuantity stiff = ResultHelper.GetQuantityResult(analysisCaseResult.Global.EffectiveInertia, inertiaUnit);
                    DA.SetData(i++, new GH_UnitNumber(stiff.X));
                    DA.SetData(i++, new GH_UnitNumber(stiff.Y));
                    DA.SetData(i++, new GH_UnitNumber(stiff.Z));
                    DA.SetData(i++, new GH_UnitNumber(stiff.XYZ));
                }
                else
                {
                    DA.SetData(i++, null);
                    DA.SetData(i++, null);
                    DA.SetData(i++, null);
                    DA.SetData(i++, null);
                }
                
                DA.SetData(i++, analysisCaseResult.Global.Mode);
                IQuantity mmass = new Mass(analysisCaseResult.Global.ModalMass, MassUnit.Kilogram);
                DA.SetData(i++, new GH_UnitNumber(mmass.ToUnit(massUnit)));

                IQuantity mstiff = new ForcePerLength(analysisCaseResult.Global.ModalStiffness, ForcePerLengthUnit.NewtonPerMeter);
                DA.SetData(i++, new GH_UnitNumber(mstiff.ToUnit(forcePerLengthUnit)));

                IQuantity geostiff = new ForcePerLength(analysisCaseResult.Global.ModalGeometricStiffness, ForcePerLengthUnit.NewtonPerMeter);
                DA.SetData(i++, new GH_UnitNumber(geostiff.ToUnit(forcePerLengthUnit)));

                DA.SetData(i++, new GH_UnitNumber(new Frequency(analysisCaseResult.Global.Frequency, FrequencyUnit.Hertz)));
                DA.SetData(i++, analysisCaseResult.Global.LoadFactor);
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
            Params.Output[i++].Name = "Total Force X [" + forceUnitAbbreviation + "]";
            Params.Output[i++].Name = "Total Force Y [" + forceUnitAbbreviation + "]";
            Params.Output[i++].Name = "Total Force Z [" + forceUnitAbbreviation + "]";
            Params.Output[i++].Name = "Total Force |XYZ| [" + forceUnitAbbreviation + "]";
            Params.Output[i++].Name = "Total Moment XX [" + momentunitAbbreviation + "]";
            Params.Output[i++].Name = "Total Moment YY [" + momentunitAbbreviation + "]";
            Params.Output[i++].Name = "Total Moment ZZ [" + momentunitAbbreviation + "]";
            Params.Output[i++].Name = "Total Moment |XXYYZZ| [" + momentunitAbbreviation + "]";
            
            Params.Output[i++].Name = "Total Reaction X [" + forceUnitAbbreviation + "]";
            Params.Output[i++].Name = "Total Reaction Y [" + forceUnitAbbreviation + "]";
            Params.Output[i++].Name = "Total Reaction Z [" + forceUnitAbbreviation + "]";
            Params.Output[i++].Name = "Total Reaction |XYZ| [" + forceUnitAbbreviation + "]";
            Params.Output[i++].Name = "Total Reaction XX [" + momentunitAbbreviation + "]";
            Params.Output[i++].Name = "Total Reaction YY [" + momentunitAbbreviation + "]";
            Params.Output[i++].Name = "Total Reaction ZZ [" + momentunitAbbreviation + "]";
            Params.Output[i++].Name = "Total Reaction |XXYYZZ| [" + momentunitAbbreviation + "]";

            Params.Output[i++].Name = "Effective Mass X [" + massUnitAbbreviation + "]";
            Params.Output[i++].Name = "Effective Mass Y [" + massUnitAbbreviation + "]";
            Params.Output[i++].Name = "Effective Mass Z [" + massUnitAbbreviation + "]";
            Params.Output[i++].Name = "Effective Mass |XYZ| [" + massUnitAbbreviation + "]";
            Params.Output[i++].Name = "Effective Inertia X [" + inertiaUnitAbbreviation + "]";
            Params.Output[i++].Name = "Effective Inertia Y [" + inertiaUnitAbbreviation + "]";
            Params.Output[i++].Name = "Effective Inertia Z [" + inertiaUnitAbbreviation + "]";
            Params.Output[i++].Name = "Effective Inertia |XYZ| [" + inertiaUnitAbbreviation + "]";
            i++;
            Params.Output[i++].Name = "Modal Mass [" + massUnitAbbreviation + "]";
            Params.Output[i++].Name = "Modal Stiffness [" + forceperlengthUnitAbbreviation + "]";
            Params.Output[i++].Name = "Modal Geometric Stiffness [" + forceperlengthUnitAbbreviation + "]";
        }
        #endregion  
    }
}

