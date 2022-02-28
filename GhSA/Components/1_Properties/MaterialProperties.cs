using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using UnitsNet;
using UnitsNet.GH;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to get geometric properties of a section
    /// </summary>
    public class GetMaterialProperties : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("7504a99f-a4e2-4e30-8251-de31ea83e8cb");
        public GetMaterialProperties()
          : base("Material Properties", "MatProp", "Get GSA Material Properties for Elastic Isotropic material type",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat1())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.MaterialProperties;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout

        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Material", "Mat", "GSA Material of Elastic Isotropic type", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Elastic Modulus", "E", "Elastic Modulus of the elastic isotropic material", GH_ParamAccess.item);
            pManager.AddNumberParameter("Poisson's Ratio", "ν", "Poisson's Ratio of the elastic isotropic material", GH_ParamAccess.item);
            pManager.AddGenericParameter("Density", "ρ", "Density of the elastic isotropic material", GH_ParamAccess.item);
            pManager.AddGenericParameter("Thermal Expansion", "α", "Thermal Expansion Coefficient of the elastic isotropic material", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaMaterial gsaMaterial = null;
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(0, ref gh_typ))
            {
                if (gh_typ.Value is GsaMaterialGoo)
                    gh_typ.CastTo(ref gsaMaterial);
            }
            if (gsaMaterial != null)
            {
                if (gsaMaterial.AnalysisMaterial == null)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input material not of elastic isotropic type");
                    return;
                }

                Pressure eModulus = new Pressure(gsaMaterial.AnalysisMaterial.ElasticModulus, UnitSystem.SI);
                eModulus = new Pressure(eModulus.As(Units.StressUnit), Units.StressUnit);
                DA.SetData(0, new GH_UnitNumber(eModulus));

                DA.SetData(1, gsaMaterial.AnalysisMaterial.PoissonsRatio);

                Density density = new Density(gsaMaterial.AnalysisMaterial.Density, UnitSystem.SI);
                density = new Density(density.As(Units.DensityUnit), Units.DensityUnit);
                DA.SetData(2, new GH_UnitNumber(density));

                CoefficientOfThermalExpansion deltaT = new CoefficientOfThermalExpansion(gsaMaterial.AnalysisMaterial.CoefficientOfThermalExpansion, UnitSystem.SI);
                deltaT = new CoefficientOfThermalExpansion(deltaT.As(Units.CoefficientOfThermalExpansionUnit), Units.CoefficientOfThermalExpansionUnit);
                DA.SetData(3, new GH_UnitNumber(deltaT));
            }
        }
    }
}

