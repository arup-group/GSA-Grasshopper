using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to edit a Material and ouput the information
    /// </summary>
    public class EditMaterial : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("865f73c7-a057-481a-834b-c7e12873dd39");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.EditMaterial;

    public EditMaterial() : base("Edit Material",
      "MaterialEdit",
      "Modify GSA Material",
      CategoryName.Name(),
      SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaMaterialParameter(), GsaMaterialGoo.Name, GsaMaterialGoo.NickName, GsaMaterialGoo.Description + " to get or set information for. Leave blank to create a new " + GsaMaterialGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Analysis Property", "An", "Set Material Analysis Property Number (0 -> 'from Grade'", GH_ParamAccess.item);
      pManager.AddTextParameter("Material Type", "mT", "Set Material Type" + Environment.NewLine +
          "Input either text string or integer:"
          + Environment.NewLine + "Generic : 0"
          + Environment.NewLine + "Steel : 1"
          + Environment.NewLine + "Concrete : 2"
          + Environment.NewLine + "Aluminium : 3"
          + Environment.NewLine + "Glass : 4"
          + Environment.NewLine + "FRP : 5"
          + Environment.NewLine + "Timber : 7"
          + Environment.NewLine + "Fabric : 8", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Material Grade", "Grd", "Set Material Grade", GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaMaterialParameter(), GsaMaterialGoo.Name, GsaMaterialGoo.NickName, GsaMaterialGoo.Description + " with applied changes.", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Analysis Property", "An", "Get Material Analysis Property (0 -> 'from Grade')", GH_ParamAccess.item);
      pManager.AddTextParameter("Material Type", "mT", "Get Material Type", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Material Grade", "Grd", "Get Material Grade", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaMaterial gsaMaterial = new GsaMaterial();
      GsaMaterial material = new GsaMaterial();
      if (DA.GetData(0, ref gsaMaterial))
      {
        material = gsaMaterial.Duplicate();
      }

      if (material != null)
      {
        // #### inputs ####
        // 1 Analysis Property
        GH_Integer ghID = new GH_Integer();
        if (DA.GetData(1, ref ghID))
        {
          if (GH_Convert.ToInt32(ghID, out int id, GH_Conversion.Both))
            material.AnalysisProperty = id;
        }

        // 2 Material type
        MaterialType matType = MaterialType.CONCRETE;
        GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
        if (DA.GetData(2, ref gh_typ))
        {
          if (gh_typ.Value is MaterialType)
            gh_typ.CastTo(ref matType);
          if (gh_typ.Value is GH_Integer)
          {
            int typ = 2;
            GH_Convert.ToInt32(gh_typ, out typ, GH_Conversion.Both);
            if (typ == 1)
              material.MaterialType = GsaMaterial.MatType.STEEL;
            if (typ == 2)
              material.MaterialType = GsaMaterial.MatType.CONCRETE;
            if (typ == 5)
              material.MaterialType = GsaMaterial.MatType.FRP;
            if (typ == 3)
              material.MaterialType = GsaMaterial.MatType.ALUMINIUM;
            if (typ == 7)
              material.MaterialType = GsaMaterial.MatType.TIMBER;
            if (typ == 4)
              material.MaterialType = GsaMaterial.MatType.GLASS;
            if (typ == 8)
              material.MaterialType = GsaMaterial.MatType.FABRIC;
            if (typ == 0)
              material.MaterialType = GsaMaterial.MatType.GENERIC;
          }
          else if (gh_typ.Value is GH_String)
          {
            string typ = "CONCRETE";
            GH_Convert.ToString(gh_typ, out typ, GH_Conversion.Both);
            if (typ.ToUpper() == "STEEL")
              material.MaterialType = GsaMaterial.MatType.STEEL;
            if (typ.ToUpper() == "CONCRETE")
              material.MaterialType = GsaMaterial.MatType.CONCRETE;
            if (typ.ToUpper() == "FRP")
              material.MaterialType = GsaMaterial.MatType.FRP;
            if (typ.ToUpper() == "ALUMINIUM")
              material.MaterialType = GsaMaterial.MatType.ALUMINIUM;
            if (typ.ToUpper() == "TIMBER")
              material.MaterialType = GsaMaterial.MatType.TIMBER;
            if (typ.ToUpper() == "GLASS")
              material.MaterialType = GsaMaterial.MatType.GLASS;
            if (typ.ToUpper() == "FABRIC")
              material.MaterialType = GsaMaterial.MatType.FABRIC;
            if (typ.ToUpper() == "GENERIC")
              material.MaterialType = GsaMaterial.MatType.GENERIC;
          }
          else
          {
            this.AddRuntimeError("Unable to convert Material Type input");
            return;
          }
        }

        // 3 grade
        int grd = 0;
        if (DA.GetData(3, ref grd))
        {
          material.GradeProperty = grd;
        }

        //#### outputs ####
        DA.SetData(0, new GsaMaterialGoo(material));
        DA.SetData(1, material.AnalysisProperty);
        string mate = material.MaterialType.ToString();
        mate = Char.ToUpper(mate[0]) + mate.Substring(1).ToLower().Replace("_", " ");
        DA.SetData(2, mate);
        DA.SetData(3, material.GradeProperty);
      }
      else
        this.AddRuntimeError("Material is Null");
    }
  }
}
