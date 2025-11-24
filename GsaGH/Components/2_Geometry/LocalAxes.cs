using System;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers.Assembly;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a Node
  /// </summary>
  public class LocalAxes : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("4a322b8e-031a-4c90-b8df-b32d162a3274");
    public override GH_Exposure Exposure => GH_Exposure.septenary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    internal Line _previewXaxis;
    internal Line _previewYaxis;
    internal Line _previewZaxis;
    protected override Bitmap Icon => Resources.LocalAxes;
    public LocalAxes() : base("Local Axes", "Axes",
      "Get the local axes from a 1D Element or Member",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    public override void DrawViewportWires(IGH_PreviewArgs args) {
      base.DrawViewportWires(args);

      if (_previewXaxis != null) {
        args.Display.DrawLine(_previewXaxis, Color.FromArgb(255, 244, 96, 96), 3);
      }

      if (_previewYaxis != null) {
        args.Display.DrawLine(_previewYaxis, Color.FromArgb(255, 96, 244, 96), 1);
      }

      if (_previewZaxis != null) {
        args.Display.DrawLine(_previewZaxis, Color.FromArgb(255, 96, 96, 234), 1);
      }
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Element/Member 1D", "G1D",
        "Element1D or Member1D to get local axes for.", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddVectorParameter("Local X", "X", "Element1D or Member1D's local X-axis",
        GH_ParamAccess.item);
      pManager.AddVectorParameter("Local Y", "Y", "Element1D or Member1D's local X-axis",
        GH_ParamAccess.item);
      pManager.AddVectorParameter("Local Z", "Z", "Element1D or Member1D's local X-axis",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp)) {
        return;
      }

      GsaMember1D member;
      GsaElement1D element;
      Point3d midPt;
      double size;
      Parameters.LocalAxes axes;
      switch (ghTyp.Value) {
        case GsaMember1dGoo memberGoo: {
            if (memberGoo == null || memberGoo.Value == null) {
              this.AddRuntimeError("Input is null");
              return;
            }

            member = memberGoo.Value;
            axes = member.LocalAxes;
            if (axes == null) {
              var assembly = new ModelAssembly(member);
              var model = new GsaModel {
                ApiModel = assembly.GetModel()
              };

              axes = new Parameters.LocalAxes(model.ApiModel.MemberDirectionCosine(1));
              this.AddRuntimeWarning(
                "Members´s local axes might deviate from the local axes in the assembled GSA model.");
            }

            midPt = member.PolyCurve.PointAtNormalizedLength(0.5);
            size = member.PolyCurve.GetLength() * 0.05;
            break;
          }
        case GsaElement1dGoo elementGoo: {
            if (elementGoo == null || elementGoo.Value == null) {
              this.AddRuntimeError("Input is null");
              return;
            }

            element = elementGoo.Value;
            axes = element.LocalAxes;
            if (axes == null) {
              var assembly = new ModelAssembly(element);
              var model = new GsaModel() {
                ApiModel = assembly.GetModel()
              };

              axes = new Parameters.LocalAxes(model.ApiModel.ElementDirectionCosine(1));
              this.AddRuntimeWarning(
                "Element´s local axes might deviate from the local axes in the assembled GSA model.");
            }

            midPt = element.Line.PointAtNormalizedLength(0.5);
            size = element.Line.GetLength() * 0.05;
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
  }
}
