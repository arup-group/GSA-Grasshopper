using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using Rhino.Geometry;
using Line = Rhino.Geometry.Line;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create a new EntityList
  /// </summary>
  public class CreateGridLine : GH_OasysComponent, IGH_PreviewObject {
    public override Guid ComponentGuid => new Guid("2f28e2d2-5e6b-4931-ae3a-f27e471e053c");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateGridLine;

    public CreateGridLine() : base("Create Grid Line", "CreateGridLine",
      "Create a GSA Grid Line from a line or arc.", CategoryName.Name(), SubCategoryName.Cat0()) { }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddCurveParameter("Curve", "C", "Line or Arc to create a GSA Grid Line", GH_ParamAccess.item);
      pManager.AddTextParameter("Label", "L", "Grid Line Label", GH_ParamAccess.item);
      pManager.AddNumberParameter("Pattern", "P", "Pattern", GH_ParamAccess.item);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaGridLineParameter());
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      string label = "";
      var ghLabel = new GH_String();
      if (da.GetData(1, ref ghLabel)) {
        GH_Convert.ToString(ghLabel, out label, GH_Conversion.Both);
      }

      GH_Curve curve = null;
      if (da.GetData(0, ref curve)) {
        var ghLine = new GH_Line();
        var ghArc = new GH_Arc();
        GridLine gridLine;
        var polyCurve = new PolyCurve();
        if (ghLine.CastFrom(curve)) {
          Line line = ghLine.Value;
          // project onto WorldXY
          line.FromZ = 0;
          line.ToZ = 0;
          if (line.Length == 0) {
            string message = "Invalid input geometry, projected line has zero length.";
            this.AddRuntimeWarning(message);
            return;
          }
          gridLine = GsaGridLine.FromLine(line, label);
          polyCurve.Append(line);
        } else if (ghArc.CastFrom(curve)) {
          Arc arc = ghArc.Value;
          // project onto WorldXY
          Point3d startPoint = arc.StartPoint;
          startPoint.Z = 0;
          Point3d midPoint = arc.MidPoint;
          midPoint.Z = 0;
          Point3d endPoint = arc.EndPoint;
          endPoint.Z = 0;
          if (arc.Length == 0) {
            string message = "Invalid input geometry, projected arc has zero length.";
            this.AddRuntimeWarning(message);
            return;
          }
          gridLine = GsaGridLine.FromArc(arc, label);
          polyCurve.Append(arc);
        } else {
          string message = "Invalid input geometry, curve needs to be a line or an arc.";
          this.AddRuntimeWarning(message);
          return;
        }

        var ghPattern = new GH_Number();
        int pattern = 0;
        if (da.GetData(2, ref ghPattern)) {
          GH_Convert.ToInt32(ghPattern, out pattern, GH_Conversion.Both);
        }

        da.SetData(0, new GsaGridLineGoo(new GsaGridLine(gridLine, polyCurve, pattern)));
      }
    }
  }
}
