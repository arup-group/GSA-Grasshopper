using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create new 1D Element
  /// </summary>
  public class Create1dElement : Section3dPreviewComponent {
    public override Guid ComponentGuid => new Guid("88c58aae-4cd8-4d37-b63f-d828571e6941");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    protected override Bitmap Icon => Resources.Create1dElement;

    public Create1dElement() : base("Create 1D Element", "Elem1D", "Create GSA 1D Element",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddLineParameter("Line", "L", "Line to create GSA Element", GH_ParamAccess.item);
      pManager.AddParameter(new GsaSectionParameter());
      pManager[1].Optional = true;
      pManager.HideParameter(0);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaElement1dParameter());
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GH_Line ghln = null;
      da.GetData(0, ref ghln);
      var elem = new GsaElement1d(new LineCurve(ghln.Value));

      GsaSectionGoo sectionGoo = null;
      if (da.GetData(1, ref sectionGoo)) {
        elem.Section = sectionGoo.Value;
        if (Preview3dSection) {
          elem.Section3dPreview = new Section3dPreview(elem);
        }
      }

      da.SetData(0, new GsaElement1dGoo(elem));
    }
  }
}
