using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units.Helpers;
using OasysUnits.Units;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create a new Material
  /// </summary>
  public class CreateMaterial : GH_OasysDropDownComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("72bfce91-9204-4fe4-b81d-0036babf0c6d");
    public CreateMaterial()
      : base("Create Material", "Material", "Create GSA Material by reference to existing Type and Grade",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateMaterial;

    public CreateMaterial() : base(
      "Create" + GsaMaterialGoo.Name.Replace(" ", string.Empty),
      GsaMaterialGoo.Name.Replace(" ", string.Empty),
      "Create a " + GsaMaterialGoo.Description + " for a " + GsaSectionGoo.Description, // "Create GSA Material by reference to existing type and grade"
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat1())
    {
      this.Hidden = true; // sets the initial state of the component to hidden
    }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddIntegerParameter("Grade", "Gr", "Material Grade (default = 1)", GH_ParamAccess.item, 1);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaMaterialParameter());
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
    private static List<string> materialTypes = new List<string>() {
      "Generic",
      "Steel",
      "Concrete",
      "Timber",
      "Aluminium",
      "FRP",
      "Glass",
      "Fabric"
    };

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Material type" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      this.DropDownItems.Add(new List<string>(materialTypes));
      this.SelectedItems.Add(materialTypes[3]);

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropDownItems[i][j];

      base.UpdateUI();
    }
    #endregion
  }
}