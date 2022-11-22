using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create a new Material
  /// </summary>
  public class CreateMaterial : GH_OasysDropDownComponent
  {
    public static List<string> MaterialTypes = new List<string>() {
      "Generic",
      "Steel",
      "Concrete",
      "Timber",
      "Aluminium",
      "FRP",
      "Glass",
      "Fabric"
    };

    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("40641747-cfb1-4dab-b060-b9dd344d3ac3");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateMaterial;

    public CreateMaterial() : base(
      "Create" + GsaMaterialGoo.Name.Replace(" ", string.Empty),
      GsaMaterialGoo.Name.Replace(" ", string.Empty),
      "Create a " + GsaMaterialGoo.Description + " for a " + GsaSectionGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat1())
    {
      this.Hidden = true; // sets the initial state of the component to hidden
    }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddIntegerParameter("Grade", "Grd", "Material Grade (default = 1)", GH_ParamAccess.item, 1);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaMaterialParameter(), "Material", "Mat", "GSA Standard Material (reference)", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaMaterial material = new GsaMaterial();

      GH_Integer gh_grade = new GH_Integer();
      if (DA.GetData(0, ref gh_grade))
      {
        int grade = 1;
        GH_Convert.ToInt32(gh_grade, out grade, GH_Conversion.Both);
        material.GradeProperty = grade;
      }

      switch (this.SelectedItems[0])
      {
        case "Steel":
          material.MaterialType = GsaMaterial.MatType.STEEL;
          break;
        case "Concrete":
          material.MaterialType = GsaMaterial.MatType.CONCRETE;
          break;
        case "Timber":
          material.MaterialType = GsaMaterial.MatType.TIMBER;
          break;
        case "Aluminium":
          material.MaterialType = GsaMaterial.MatType.ALUMINIUM;
          break;
        case "FRP":
          material.MaterialType = GsaMaterial.MatType.FRP;
          break;
        case "Glass":
          material.MaterialType = GsaMaterial.MatType.GLASS;
          break;
        case "Fabric":
          material.MaterialType = GsaMaterial.MatType.FABRIC;
          break;
        case "Generic":
        default:
          material.MaterialType = GsaMaterial.MatType.GENERIC;
          break;
      }
      DA.SetData(0, new GsaMaterialGoo(material));
    }

    #region Custom UI
    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Material type" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      this.DropDownItems.Add(new List<string>(MaterialTypes));
      this.SelectedItems.Add(MaterialTypes[3]);

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      base.UpdateUI();
    }
    #endregion
  }
}