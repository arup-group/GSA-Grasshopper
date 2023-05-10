using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a Material and ouput the information
  /// </summary>
  public class EditMaterial : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("33d14120-7355-414b-96d9-b85d64290d49");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditMaterial;

    public EditMaterial() : base("Edit Material", "MaterialEdit", "Modify GSA Material",
      CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaMaterialParameter(), GsaMaterialGoo.Name,
        GsaMaterialGoo.NickName,
        GsaMaterialGoo.Description + " to get or set information for. Leave blank to create a new "
        + GsaMaterialGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Analysis Property", "An",
        "Set Material Analysis Property Number (0 -> 'from Grade')", GH_ParamAccess.item);
      pManager.AddTextParameter("Material Name", "Na", "Material Name of Custom Material",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Material Type", "mT",
        "Set Material Type" + Environment.NewLine + "Input either text string or integer:"
        + Environment.NewLine + "Generic : 0" + Environment.NewLine + "Steel : 1"
        + Environment.NewLine + "Concrete : 2" + Environment.NewLine + "Aluminium : 3"
        + Environment.NewLine + "Glass : 4" + Environment.NewLine + "FRP : 5" + Environment.NewLine
        + "Timber : 7" + Environment.NewLine + "Fabric : 8", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Material Grade", "Grd", "Set Material Grade",
        GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaMaterialParameter(), GsaMaterialGoo.Name,
        GsaMaterialGoo.NickName, GsaMaterialGoo.Description + " with applied changes.",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Analysis Property", "An",
        "Get Material Analysis Property (0 -> 'from Grade')", GH_ParamAccess.item);
      pManager.AddTextParameter("Material Name", "Na", "Material Name of Custom Material",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Material Type", "mT", "Get Material Type", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Material Grade", "Grd", "Get Material Grade",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaMaterial = new GsaMaterial();
      var material = new GsaMaterial();
      if (da.GetData(0, ref gsaMaterial)) {
        material = gsaMaterial.Duplicate();
      }

      if (material != null) {
        var ghId = new GH_Integer();
        if (da.GetData(1, ref ghId)) {
          if (GH_Convert.ToInt32(ghId, out int id, GH_Conversion.Both)) {
            material.Id = id;
          }
        }

        string name = "";
        if (da.GetData(2, ref name)) {
          if (material.AnalysisMaterial == null) {
            this.AddRuntimeWarning("Currently only Custom Materials support material names.");
          } else {
            material.AnalysisMaterial.Name = name;
          }
        }

        var ghTyp = new GH_ObjectWrapper();
        if (da.GetData(3, ref ghTyp)) {
          switch (ghTyp.Value) {
            case GH_Integer ghInt: {
                switch (ghInt.Value) {
                  case 1:
                    material.MaterialType = GsaMaterial.MaterialType.Steel;
                    break;

                  case 2:
                    material.MaterialType = GsaMaterial.MaterialType.Concrete;
                    break;

                  case 5:
                    material.MaterialType = GsaMaterial.MaterialType.Frp;
                    break;

                  case 3:
                    material.MaterialType = GsaMaterial.MaterialType.Aluminium;
                    break;

                  case 7:
                    material.MaterialType = GsaMaterial.MaterialType.Timber;
                    break;

                  case 4:
                    material.MaterialType = GsaMaterial.MaterialType.Glass;
                    break;

                  case 8:
                    material.MaterialType = GsaMaterial.MaterialType.Fabric;
                    break;

                  case 0:
                    material.MaterialType = GsaMaterial.MaterialType.Generic;
                    break;
                }

                break;
              }

            case GH_String ghString: {
                switch (ghString.Value.ToUpper()) {
                  case "STEEL":
                    material.MaterialType = GsaMaterial.MaterialType.Steel;
                    break;

                  case "CONCRETE":
                    material.MaterialType = GsaMaterial.MaterialType.Concrete;
                    break;

                  case "FRP":
                    material.MaterialType = GsaMaterial.MaterialType.Frp;
                    break;

                  case "ALUMINIUM":
                    material.MaterialType = GsaMaterial.MaterialType.Aluminium;
                    break;

                  case "TIMBER":
                    material.MaterialType = GsaMaterial.MaterialType.Timber;
                    break;

                  case "GLASS":
                    material.MaterialType = GsaMaterial.MaterialType.Glass;
                    break;

                  case "FABRIC":
                    material.MaterialType = GsaMaterial.MaterialType.Fabric;
                    break;

                  case "GENERIC":
                    material.MaterialType = GsaMaterial.MaterialType.Generic;
                    break;
                }

                break;
              }

            default:
              this.AddRuntimeError("Unable to convert Material Type input");
              return;
          }
        }

        int grd = 0;
        if (da.GetData(4, ref grd)) {
          material.Id = grd;
        }

        da.SetData(0, new GsaMaterialGoo(material));
        da.SetData(1, material.Id);
        string mate = material.MaterialType.ToString();
        mate = char.ToUpper(mate[0]) + mate.Substring(1).ToLower().Replace("_", " ");
        string analysisMaterialName = "";
        if (material.AnalysisMaterial != null) {
          analysisMaterialName = material.AnalysisMaterial.Name;
        }
        da.SetData(2, analysisMaterialName);
        da.SetData(3, mate);
        da.SetData(4, material.Id);
      } else {
        this.AddRuntimeError("Material is null");
      }
    }
  }
}
