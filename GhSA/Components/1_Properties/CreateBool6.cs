using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to create a new Bool6
    /// </summary>
    public class CreateBool6 : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("f5909576-6796-4d6e-90d8-31a9b7ee6fb6");
        public CreateBool6()
          : base("Create Bool6", "Bool6", "Create GSA Bool6 to set releases and restraints",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat1())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateBool6;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            m_attributes = new UI.Bool6ComponentUI(this, SetBool, "Set 6 DOF" , x, y, z, xx, yy, zz);
        }

        public void SetBool(bool resx, bool resy, bool resz, bool resxx, bool resyy, bool reszz)
        {
            x = resx;
            y = resy;
            z = resz;
            xx = resxx;
            yy = resyy;
            zz = reszz;
        }

        #endregion

        #region Input and output
        bool x;
        bool y;
        bool z;
        bool xx;
        bool yy;
        bool zz;

        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            // we need to save all the items that we want to reappear when a GH file is saved and re-opened
            writer.SetBoolean("x", (bool)x);
            writer.SetBoolean("y", (bool)y);
            writer.SetBoolean("z", (bool)z);
            writer.SetBoolean("xx", (bool)xx);
            writer.SetBoolean("yy", (bool)yy);
            writer.SetBoolean("zz", (bool)zz);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            // when a GH file is opened we need to read in the data that was previously set by user
            x = (bool)reader.GetBoolean("x");
            y = (bool)reader.GetBoolean("y");
            z = (bool)reader.GetBoolean("z");
            xx = (bool)reader.GetBoolean("xx");
            yy = (bool)reader.GetBoolean("yy");
            zz = (bool)reader.GetBoolean("zz");
            // we need to recreate the custom UI again as this is created before this read IO is called
            // otherwise the component will not display the selected items on the canvas
            CreateAttributes();
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
            return base.Read(reader);
        }
        #endregion
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("X", "X", "X", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Y", "Y", "Y", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Z", "Z", "Z", GH_ParamAccess.item);
            pManager.AddBooleanParameter("XX", "XX", "XX", GH_ParamAccess.item);
            pManager.AddBooleanParameter("YY", "YY", "YY", GH_ParamAccess.item);
            pManager.AddBooleanParameter("ZZ", "ZZ", "ZZ", GH_ParamAccess.item);

            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Bool6", "B6", "GSA Bool6 to set releases or restraints", GH_ParamAccess.item);
            
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Boolean ghBolX = new GH_Boolean();
            if (DA.GetData(0, ref ghBolX))
                GH_Convert.ToBoolean(ghBolX, out x, GH_Conversion.Both); //use Grasshopper to convert, these methods covers many cases and are consistent
            GH_Boolean ghBolY = new GH_Boolean();
            if (DA.GetData(1, ref ghBolY))
                GH_Convert.ToBoolean(ghBolY, out y, GH_Conversion.Both);
            GH_Boolean ghBolZ = new GH_Boolean();
            if (DA.GetData(2, ref ghBolZ))
                GH_Convert.ToBoolean(ghBolZ, out z, GH_Conversion.Both);
            GH_Boolean ghBolXX = new GH_Boolean();
            if (DA.GetData(3, ref ghBolXX))
                GH_Convert.ToBoolean(ghBolXX, out xx, GH_Conversion.Both);
            GH_Boolean ghBolYY = new GH_Boolean();
            if (DA.GetData(4, ref ghBolYY))
                GH_Convert.ToBoolean(ghBolYY, out yy, GH_Conversion.Both);
            GH_Boolean ghBolZZ = new GH_Boolean();
            if (DA.GetData(5, ref ghBolZZ))
                GH_Convert.ToBoolean(ghBolZZ, out zz, GH_Conversion.Both);
            GsaBool6 bool6 = new GsaBool6
            {
                X = x,
                Y = y,
                Z = z,
                XX = xx,
                YY = yy,
                ZZ = zz
            };
            DA.SetData(0, new GsaBool6Goo(bool6.Duplicate())); // output as Goo-type for consistency. 
        }
    }
}

