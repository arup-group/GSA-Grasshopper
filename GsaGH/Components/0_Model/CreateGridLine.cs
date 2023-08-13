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
      pManager.AddIntegerParameter("Index", "ID",
        "(Optional) Grid Line Number - set this to 0 to append it to the end of the list of lists," +
        "or set the ID to overwrite an existing Grid Line.", GH_ParamAccess.item, 0);
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
        if (ghLine.CastFrom(curve)) {
          Line line = ghLine.Value;
          line.FromZ = 0;
          line.ToZ = 0;

          gridLine = new GridLine(label) {
            Shape = GridLineShape.Line,
            X = line.From.X,
            Y = line.From.Y,
            Length = line.Length,
            Theta1 = Vector3d.VectorAngle(new Vector3d(1, 0, 0), line.UnitTangent)
          };
        } else if (ghArc.CastFrom(curve)) {
          Point3d startPoint = ghArc.Value.StartPoint;
          startPoint.Z = 0;
          Point3d midPoint = ghArc.Value.MidPoint;
          midPoint.Z = 0;
          Point3d endPoint = ghArc.Value.EndPoint;
          endPoint.Z = 0;
          var arc = new Arc(startPoint, midPoint, endPoint);

          gridLine = new GridLine(label) {
            Shape = GridLineShape.Arc,
            X = arc.StartPoint.X,
            Y = arc.EndPoint.Y,
            Length = arc.Length,
            Theta1 = arc.StartAngle,
            Theta2 = arc.EndAngle
          };
        } else {
          string message = "Invalid input geometry, curve needs to be a line or an arc.";
          this.AddRuntimeWarning(message);
          return;
        }

        int id = 0;
        da.GetData(2, ref id);

        da.SetData(0, new GsaGridLineGoo(new GsaGridLine(id, gridLine)));
      }
    }
  }
}