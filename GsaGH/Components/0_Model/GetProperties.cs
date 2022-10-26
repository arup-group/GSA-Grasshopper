using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  public class GetProperties : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("f5926fb3-06e5-4b18-b037-6234fff16586");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.GetSection;

    public GetProperties() : base("Get Model Properties",
      "GetProps",
      "Get Sections, 2D Properties and Springs from GSA model",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat0())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA", "GSA model containing some properties", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaSectionParameter(), "Sections", "PB", "Section Properties from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaProp2dParameter(), "2D Properties", "PA", "2D Properties from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaProp3dParameter(), "3D Properties", "PV", "3D Properties from GSA Model", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaModel gsaModel = new GsaModel();
      if (DA.GetData(0, ref gsaModel))
      {
        Model model = gsaModel.Model;

        List<GsaSectionGoo> sections = Util.Gsa.FromGSA.GetSections(model.Sections(), model.AnalysisMaterials(), model.SectionModifiers());
        List<GsaProp2dGoo> prop2Ds = Util.Gsa.FromGSA.GetProp2ds(model.Prop2Ds(), model.AnalysisMaterials());
        List<GsaProp3dGoo> prop3Ds = Util.Gsa.FromGSA.GetProp3ds(model.Prop3Ds(), model.AnalysisMaterials());

        DA.SetDataList(0, sections);
        DA.SetDataList(1, prop2Ds);
        DA.SetDataList(2, prop3Ds);
      }
    }
  }
}
