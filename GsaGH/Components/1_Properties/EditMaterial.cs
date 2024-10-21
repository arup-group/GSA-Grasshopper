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
    public override Guid ComponentGuid => new Guid("70750300-2ec7-42a1-88ea-9e189e767544");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditMaterial;

    public EditMaterial() : base("Edit Material", "EditMat", "Modify a GSA Material",
      CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaMaterialParameter());
      pManager.AddIntegerParameter("Material ID", "ID",
        "[Optional] Set Material ID corrosponding to the desired ID in the material type's table " +
        "(Steel, Concrete, etc).", GH_ParamAccess.item);
      pManager.AddTextParameter("Material Name", "Na", "[Optional] Set Material Name",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter(), "Analysis Material", "AM",
        GsaMaterialGoo.Description + "[Optional] Input another Material to overwrite the analysis" +
        " material properties."
        + GsaMaterialGoo.Name, GH_ParamAccess.item);
      pManager.AddGenericParameter("Material Type", "mT",
        "[Optional] Set Material Type for a Custom Material (only)." + Environment.NewLine +
        "Input either text string or integer:"
        + Environment.NewLine + "Generic : 0" + Environment.NewLine + "Steel : 1"
        + Environment.NewLine + "Concrete : 2" + Environment.NewLine + "Aluminium : 3"
        + Environment.NewLine + "Glass : 4" + Environment.NewLine + "FRP : 5" + Environment.NewLine
        + "Timber : 7" + Environment.NewLine + "Fabric : 8", GH_ParamAccess.item);

      for (int i = 1; i < pManager.ParamCount; i++) {
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
      pManager.AddParameter(new GsaMaterialParameter(), "Custom Material", "Mat",
        "A copy of this material as a Custom material.", GH_ParamAccess.item);
      pManager.AddTextParameter("Material Type", "mT", "Get Material Type", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaMaterial material = null;
      GsaMaterialGoo materialGoo = null;
      if (da.GetData(0, ref materialGoo)) {
        material = GsaMaterialFactory.CreateMaterial(materialGoo.Value);
      }

      GsaMaterial customMaterial = GsaMaterialFactory.CreateMaterial(material);
      int id = 0;
      if (da.GetData(1, ref id)) {
        material.Id = id;
      }

      string name = string.Empty;
      if (da.GetData(2, ref name)) {
        material.Name = name;
      }

      GsaMaterialGoo customMaterialGoo = null;
      if (da.GetData(3, ref customMaterialGoo)) {
        customMaterial = GsaMaterialFactory.CreateMaterial(customMaterialGoo.Value);
        material.AnalysisMaterial = customMaterial.AnalysisMaterial;
      }

      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(4, ref ghTyp)) {
        if (!(material is GsaCustomMaterial)) {
          this.AddRuntimeWarning("MaterialType can only be changed for Custom Materials");
        }

        MatType type = material.MaterialType;
        switch (ghTyp.Value) {
          case GH_Integer ghInt: {
              switch (ghInt.Value) {
                case 1:
                  type = MatType.Steel;
                  break;

                case 2:
                  type = MatType.Concrete;
                  break;

                case 5:
                  type = MatType.Frp;
                  break;

                case 3:
                  type = MatType.Aluminium;
                  break;

                case 7:
                  type = MatType.Timber;
                  break;

                case 4:
                  type = MatType.Glass;
                  break;

                case 8:
                  type = MatType.Fabric;
                  break;

                case 0:
                  type = MatType.Custom;
                  break;
              }

              break;
            }

          case GH_String ghString: {
              switch (ghString.Value.ToUpper()) {
                case "STEEL":
                  type = MatType.Steel;
                  break;

                case "CONCRETE":
                  type = MatType.Concrete;
                  break;

                case "FRP":
                  type = MatType.Frp;
                  break;

                case "ALUMINIUM":
                  type = MatType.Aluminium;
                  break;

                case "TIMBER":
                  type = MatType.Timber;
                  break;

                case "GLASS":
                  type = MatType.Glass;
                  break;

                case "FABRIC":
                  type = MatType.Fabric;
                  break;

                case "GENERIC":
                case "CUSTOM":
                  type = MatType.Custom;
                  break;
              }

              break;
            }

          default:
            this.AddRuntimeError("Unable to convert Material Type input");
            return;
        }

        if (material is GsaCustomMaterial) {
          material.MaterialType = type;
        }

        customMaterial = new GsaCustomMaterial(customMaterial.AnalysisMaterial, id, type);
        if (type != material.MaterialType) {
          customMaterial.Name = $"Created from {material.MaterialType} {material.Name}";
        }
      }

      da.SetData(0, new GsaMaterialGoo(material));
      da.SetData(1, material.Id);
      da.SetData(2, material.Name);
      da.SetData(3, new GsaMaterialGoo(customMaterial));
      da.SetData(4, material.MaterialType.ToString());
    }
  }
}
