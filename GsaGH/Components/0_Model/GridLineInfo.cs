using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  /// Component to retrieve Grid Lines from a GSA model
  /// </summary>
  public class GridLineInfo : GH_OasysComponent, IGH_PreviewObject {
    public override Guid ComponentGuid => new Guid("5f287f54-e461-4579-b414-9298f213074b");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GridLineInfo;

    public GridLineInfo() : base("Grid Line Info", "GridLineInfo",
      "Get the information of a GSA Grid Line", CategoryName.Name(), SubCategoryName.Cat0()) { }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaGridLineParameter());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddTextParameter("Label", "Lb", "The name by which the grid line is referred", GH_ParamAccess.item);
      pManager.AddPointParameter("Starting Point", "Pt", "The start of a straight line or the centre of a circular arc",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Length", "L", "The length of a straight line or the radius of a circular arc",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Shape", "S", "Specifies whether the grid line is a straight line or circular arc", GH_ParamAccess.item);
      pManager.AddNumberParameter("Orientation", "θ1", "The angle of inclination of a straight line or the start angle of a circular arc",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Angle", "θ2", "The end angle of a circular arc (not required for straight grid lines)",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaGridLineGoo gridGoo = null;
      da.GetData(0, ref gridGoo);
      da.SetData(0, gridGoo.Value.GridLine.Label);
      var pt = new Point3d(gridGoo.Value.GridLine.X, gridGoo.Value.GridLine.Y, 0);
      da.SetData(1, pt);
      da.SetData(2, gridGoo.Value.GridLine.Length);
      da.SetData(3, gridGoo.Value.GridLine.Shape);
      da.SetData(4, gridGoo.Value.GridLine.Theta1);
      da.SetData(5, gridGoo.Value.GridLine.Theta2);
    }
  }
}
