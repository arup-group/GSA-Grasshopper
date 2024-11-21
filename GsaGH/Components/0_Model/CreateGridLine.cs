using System;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Units.Helpers;

using OasysUnits.Units;

using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create a new <see cref="GsaGridLine" />
  /// </summary>
  public class CreateGridLine : GH_OasysComponent, IGH_PreviewObject {
    public override Guid ComponentGuid => new Guid("2f28e2d2-5e6b-4931-ae3a-f27e471e053c");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateGridLine;

    public CreateGridLine() : base("Create Grid Line", "CreateGridLine", "Create a GSA Grid Line from a line or arc.",
      CategoryName.Name(), SubCategoryName.Cat0()) { }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddCurveParameter("Curve", "C", "Straight line or circular arc to create a GSA Grid Line",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Label", "L", "The name by which the grid line is referred", GH_ParamAccess.item);
      pManager[1].Optional = true;
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
      GsaGridLine gsaGridLine = null;
      if (!da.GetData(0, ref curve)) {
        return;
      }

      LengthUnit rhinoUnits = RhinoUnit.GetRhinoLengthUnit();
      var ghLine = new GH_Line();
      var ghArc = new GH_Arc();
      if (ghLine.CastFrom(curve)) {
        Line line = ghLine.Value;
        // project onto WorldXY
        line.FromZ = 0;
        line.ToZ = 0;
        gsaGridLine = new GsaGridLine(line, label);
      } else if (ghArc.CastFrom(curve)) {
        Arc arc = ghArc.Value;
        // project onto WorldXY
        Point3d startPoint = arc.StartPoint;
        startPoint.Z = 0;
        Point3d midPoint = arc.MidPoint;
        midPoint.Z = 0;
        Point3d endPoint = arc.EndPoint;
        endPoint.Z = 0;
        gsaGridLine = new GsaGridLine(arc, label);
      } else {
        string message = "Invalid input geometry, curve needs to be a straight line or a circular arc.";
        this.AddRuntimeWarning(message);
        return;
      }

      var goo = new GsaGridLineGoo(gsaGridLine);
      da.SetData(0, goo);
    }
  }
}
