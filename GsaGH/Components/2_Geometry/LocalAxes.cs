using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Util.Gsa.ToGSA;
using OasysGH;
using OasysGH.Components;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to edit a Node
  /// </summary>
  public class LocalAxes : GH_OasysComponent, IGH_PreviewObject
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("4a322b8e-031a-4c90-b8df-b32d162a3274");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.LocalAxes;

    public LocalAxes() : base("Local Axis",
      "Axis",
      "Get Element1D or Member1D local axes",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat2())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Element/Member 1D", "G1D", "Element1D or Member1D to get local axes for.", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddVectorParameter("Local X", "X", "Element1D or Member1D's local X-axis", GH_ParamAccess.list);
      pManager.AddVectorParameter("Local Y", "Y", "Element1D or Member1D's local X-axis", GH_ParamAccess.list);
      pManager.AddVectorParameter("Local Z", "Z", "Element1D or Member1D's local X-axis", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        GsaMember1d mem = null;
        GsaElement1d elem = null;
        Point3d midPt = new Point3d();
        double size = 0;
        GsaLocalAxes axes;
        if (gh_typ.Value is GsaMember1dGoo)
        {
          gh_typ.CastTo(ref mem);
          if (mem == null)
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input is null");
            return;
          }
          axes = mem.LocalAxes;
          if (axes == null)
          {
            GsaModel model = new GsaModel();
            model.Model = Util.Gsa.ToGSA.Assemble.AssembleModel(model, new List<GsaNode>(), new List<GsaElement1d>(), new List<GsaElement2d>(), new List<GsaElement3d>(), new List<GsaMember1d>() { mem }, new List<GsaMember2d>(), new List<GsaMember3d>(), new List<GsaSection>(), new List<GsaProp2d>(), new List<GsaProp3d>(), new List<GsaLoad>(), new List<GsaGridPlaneSurface>(), new List<GsaAnalysisTask>(), new List<GsaCombinationCase>(), LengthUnit.Meter);

            axes = new GsaLocalAxes(model.Model.MemberDirectionCosine(1).ToList(), 0);
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Members´s local axes might deviate from the local axes in the assembled GSA model.");
          }
          midPt = mem.PolyCurve.PointAtNormalizedLength(0.5);
          size = mem.PolyCurve.GetLength() * 0.05;
        }
        else if (gh_typ.Value is GsaElement1dGoo)
        {
          gh_typ.CastTo(ref elem);
          if (elem == null)
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input is null");
            return;
          }
          axes = elem.LocalAxes;
          if (axes == null)
          {
            GsaModel model = new GsaModel();
            model.Model = Util.Gsa.ToGSA.Assemble.AssembleModel(model, new List<GsaNode>(), new List<GsaElement1d>() { elem }, new List<GsaElement2d>(), new List<GsaElement3d>(), new List<GsaMember1d>(), new List<GsaMember2d>(), new List<GsaMember3d>(), new List<GsaSection>(), new List<GsaProp2d>(), new List<GsaProp3d>(), new List<GsaLoad>(), new List<GsaGridPlaneSurface>(), new List<GsaAnalysisTask>(), new List<GsaCombinationCase>(), LengthUnit.Meter);

            axes = new GsaLocalAxes(model.Model.ElementDirectionCosine(1).ToList(), 0);
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Element´s local axes might deviate from the local axes in the assembled GSA model.");
          }
          midPt = elem.Line.PointAtNormalizedLength(0.5);
          size = elem.Line.GetLength() * 0.05;
        }
        else
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert input to Element1D or Member1D");
          return;
        }
        Vector3d x = axes.X;
        Vector3d y = axes.Y;
        Vector3d z = axes.Z;
        DA.SetData(0, x);
        DA.SetData(1, y);
        DA.SetData(2, z);
        previewXaxis = new Line(midPt, x, size);
        previewYaxis = new Line(midPt, y, size);
        previewZaxis = new Line(midPt, z, size);
      }
    }

    internal Line previewXaxis;
    internal Line previewYaxis;
    internal Line previewZaxis;
    public override void DrawViewportWires(IGH_PreviewArgs args)
    {
      base.DrawViewportWires(args);

      // local axis
      if (previewXaxis != null)
      {
        args.Display.DrawLine(previewXaxis, System.Drawing.Color.FromArgb(255, 244, 96, 96), 3);
      }
      if (previewYaxis != null)
      {
        args.Display.DrawLine(previewYaxis, System.Drawing.Color.FromArgb(255, 96, 244, 96), 1);
      }
      if (previewZaxis != null)
      {
        args.Display.DrawLine(previewZaxis, System.Drawing.Color.FromArgb(255, 96, 96, 234), 1);
      }
    }
  }
}
