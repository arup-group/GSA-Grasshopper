using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI.Materials;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a Material and ouput the information
  /// </summary>
  public class EditMaterial2_OBSOLETE : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("33d14120-7355-414b-96d9-b85d64290d49");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditMaterial;

    public EditMaterial2_OBSOLETE() : base("Edit Material", "MaterialEdit", "Modify GSA Material",
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
      pManager.AddIntegerParameter("Material ID", "ID",
        "Get the Material's ID in its respective table (Steel, Concrete, etc)", GH_ParamAccess.item);
      pManager.AddTextParameter("Material Name", "Na", "Get the Material's Name",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Material Type", "mT", "Get Material Type", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Material Grade", "Grd", "Get Material Grade",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var material = new GsaMaterial();

      GsaMaterialGoo materialGoo = null;
      if (da.GetData(0, ref materialGoo)) {
        material = materialGoo.Value.Duplicate();
      }

      int id = 0;
      if (da.GetData(1, ref id)) {
        material.Id = id;
      }

      if (material.IsCustom) {
        var ghTyp = new GH_ObjectWrapper();
        if (da.GetData(3, ref ghTyp)) {
          GsaMaterial.MatType type = material.MaterialType;
          switch (ghTyp.Value) {
            case GH_Integer ghInt: {
                switch (ghInt.Value) {
                  case 1:
                    type = GsaMaterial.MatType.Steel;
                    break;

                  case 2:
                    type = GsaMaterial.MatType.Concrete;
                    break;

                  case 5:
                    type = GsaMaterial.MatType.Frp;
                    break;

                  case 3:
                    type = GsaMaterial.MatType.Aluminium;
                    break;

                  case 7:
                    type = GsaMaterial.MatType.Timber;
                    break;

                  case 4:
                    type = GsaMaterial.MatType.Glass;
                    break;

                  case 8:
                    type = GsaMaterial.MatType.Fabric;
                    break;

                  case 0:
                    type = GsaMaterial.MatType.Generic;
                    break;
                }

                break;
              }

            case GH_String ghString: {
                switch (ghString.Value.ToUpper()) {
                  case "STEEL":
                    type = GsaMaterial.MatType.Steel;
                    break;

                  case "CONCRETE":
                    type = GsaMaterial.MatType.Concrete;
                    break;

                  case "FRP":
                    type = GsaMaterial.MatType.Frp;
                    break;

                  case "ALUMINIUM":
                    type = GsaMaterial.MatType.Aluminium;
                    break;

                  case "TIMBER":
                    type = GsaMaterial.MatType.Timber;
                    break;

                  case "GLASS":
                    type = GsaMaterial.MatType.Glass;
                    break;

                  case "FABRIC":
                    type = GsaMaterial.MatType.Fabric;
                    break;

                  case "GENERIC":
                    type = GsaMaterial.MatType.Generic;
                    break;
                }

                break;
              }

            default:
              this.AddRuntimeError("Unable to convert Material Type input");
              return;
          }

          material = new GsaMaterial(material.AnalysisMaterial, id, type);
        }
      }

      string name = "";
      if (da.GetData(2, ref name)) {
        material.Name = name;
      }

      int grd = 0;
      if (da.GetData(4, ref grd)) {
        material.Id = grd;
      }

      da.SetData(0, new GsaMaterialGoo(material));
      da.SetData(1, material.Id);
      da.SetData(2, material.Name);
      da.SetData(3, material.MaterialType.ToString());
      da.SetData(4, material.Id);
    }
  }
}
