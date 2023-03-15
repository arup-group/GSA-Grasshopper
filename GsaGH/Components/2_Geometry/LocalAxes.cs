using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  /// Component to edit a Node
  /// </summary>
  public class LocalAxes : GH_OasysComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("4a322b8e-031a-4c90-b8df-b32d162a3274");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.LocalAxes;

    public LocalAxes() : base("Local Axis",
      "Axis",
      "Get Element1D or Member1D local axes",
      CategoryName.Name(),
      SubCategoryName.Cat2()) { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Element/Member 1D", "G1D", "Element1D or Member1D to get local axes for.", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddVectorParameter("Local X", "X", "Element1D or Member1D's local X-axis", GH_ParamAccess.list);
      pManager.AddVectorParameter("Local Y", "Y", "Element1D or Member1D's local X-axis", GH_ParamAccess.list);
      pManager.AddVectorParameter("Local Z", "Z", "Element1D or Member1D's local X-axis", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp)) {
        return;
      }

      GsaMember1d mem = null;
      GsaElement1d elem = null;
      var midPt = new Point3d();
      double size = 0;
      GsaLocalAxes axes;
      switch (ghTyp.Value)
      {
        case GsaMember1dGoo _:
        {
          ghTyp.CastTo(ref mem);
          if (mem == null) {
            this.AddRuntimeError("Input is null");
            return;
          }
          axes = mem.LocalAxes;
          if (axes == null) {
            var model = new GsaModel();
            model.Model = Helpers.Export.AssembleModel.Assemble(model, new List<GsaNode>(), new List<GsaElement1d>(), new List<GsaElement2d>(), new List<GsaElement3d>(), new List<GsaMember1d>() { mem }, new List<GsaMember2d>(), new List<GsaMember3d>(), new List<GsaSection>(), new List<GsaProp2d>(), new List<GsaProp3d>(), new List<GsaLoad>(), new List<GsaGridPlaneSurface>(), new List<GsaAnalysisTask>(), new List<GsaCombinationCase>(), LengthUnit.Meter, Length.Zero, false, null);

            axes = new GsaLocalAxes(model.Model.MemberDirectionCosine(1));
            this.AddRuntimeWarning("Members´s local axes might deviate from the local axes in the assembled GSA model.");
          }
          midPt = mem.PolyCurve.PointAtNormalizedLength(0.5);
          size = mem.PolyCurve.GetLength() * 0.05;
          break;
        }
        case GsaElement1dGoo _:
        {
          ghTyp.CastTo(ref elem);
          if (elem == null) {
            this.AddRuntimeError("Input is null");
            return;
          }
          axes = elem.LocalAxes;
          if (axes == null) {
            var model = new GsaModel();
            model.Model = Helpers.Export.AssembleModel.Assemble(model, new List<GsaNode>(), new List<GsaElement1d>() { elem }, new List<GsaElement2d>(), new List<GsaElement3d>(), new List<GsaMember1d>(), new List<GsaMember2d>(), new List<GsaMember3d>(), new List<GsaSection>(), new List<GsaProp2d>(), new List<GsaProp3d>(), new List<GsaLoad>(), new List<GsaGridPlaneSurface>(), new List<GsaAnalysisTask>(), new List<GsaCombinationCase>(), LengthUnit.Meter, Length.Zero, false, null);

            axes = new GsaLocalAxes(model.Model.ElementDirectionCosine(1));
            this.AddRuntimeWarning("Element´s local axes might deviate from the local axes in the assembled GSA model.");
          }
          midPt = elem.Line.PointAtNormalizedLength(0.5);
          size = elem.Line.GetLength() * 0.05;
          break;
        }
        default:
          this.AddRuntimeError("Unable to convert input to Element1D or Member1D");
          return;
      }

      Vector3d x = axes.X;
      Vector3d y = axes.Y;
      Vector3d z = axes.Z;

      da.SetData(0, x);
      da.SetData(1, y);
      da.SetData(2, z);
      _previewXaxis = new Line(midPt, x, size);
      _previewYaxis = new Line(midPt, y, size);
      _previewZaxis = new Line(midPt, z, size);
    }

    internal Line _previewXaxis;
    internal Line _previewYaxis;
    internal Line _previewZaxis;
    public override void DrawViewportWires(IGH_PreviewArgs args) {
      base.DrawViewportWires(args);

      // local axis
      if (_previewXaxis != null) {
        args.Display.DrawLine(_previewXaxis, System.Drawing.Color.FromArgb(255, 244, 96, 96), 3);
      }
      if (_previewYaxis != null) {
        args.Display.DrawLine(_previewYaxis, System.Drawing.Color.FromArgb(255, 96, 244, 96), 1);
      }
      if (_previewZaxis != null) {
        args.Display.DrawLine(_previewZaxis, System.Drawing.Color.FromArgb(255, 96, 96, 234), 1);
      }
    }
  }
}
