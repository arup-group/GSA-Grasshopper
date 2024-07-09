using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create new 2D Element
  /// </summary>
  public class Create2dElement : Section3dPreviewComponent {
    public override Guid ComponentGuid => new Guid("8f83d32a-c2df-4f47-9cfc-d2d4253703e1");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    protected override Bitmap Icon => Resources.Create2dElement;

    public Create2dElement() : base("Create 2D Element", "Elem2D", "Create GSA 2D Element",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddMeshParameter("Mesh", "M", "Mesh to create GSA Element", GH_ParamAccess.item);
      pManager.AddParameter(new GsaProperty2dParameter());
      pManager[1].Optional = true;
      pManager.HideParameter(0);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaElement2dParameter());
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GH_Mesh ghmesh = null;
      da.GetData(0, ref ghmesh);

      GsaProperty2dGoo prop2dGoo = null;
      bool prop2dAssigned = da.GetData(1, ref prop2dGoo);
      bool isLoadPanel = prop2dAssigned && (prop2dGoo.Value.ApiProp2d.Type == Property2D_Type.LOAD);
      var elem = new GsaElement2d(ghmesh.Value, isLoadPanel);
      if (prop2dAssigned) {
        var prop2Ds = new List<GsaProperty2d>();
        for (int i = 0; i < elem.ApiElements.Count; i++) {
          prop2Ds.Add(prop2dGoo.Value);
        }

        elem.Prop2ds = prop2Ds;
        if (Preview3dSection) {
          elem.CreateSection3dPreview();
        }
      }

      da.SetData(0, new GsaElement2dGoo(elem));
    }
  }
}
