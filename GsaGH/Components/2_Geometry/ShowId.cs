using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using GsaGH.Helpers.GH;
using OasysGH;
using OasysGH.Components;
using OasysUnits.Units;
using Rhino.Geometry;
using OasysUnits;
using GsaAPI;
using Rhino.Collections;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to edit a Node
  /// </summary>
  public class ShowId : GH_OasysComponent, IGH_PreviewObject
  {
    private List<Point3d> _pts;
    private List<string> _txts;

    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("e01fde68-b591-4ada-b590-9506fc962114");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.ShowID;

    public ShowId() : base("ShowID",
      "ID",
      "Show the ID of a Node, Element, Member geometry or Result parameters",
      CategoryName.Name(),
      SubCategoryName.Cat2())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Node/Element/Member/Result", "Geo", "Node, Element, Member or Point/Line/Mesh result to get ID for.", GH_ParamAccess.list);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddPointParameter("Position", "P", "The (centre/mid) location(s) of the object(s)", GH_ParamAccess.list);
      pManager.HideParameter(0);
      pManager.AddIntegerParameter("Index", "ID", "The objects ID(s)", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      var gh_typ = new List<GH_ObjectWrapper>();
      _pts = new List<Point3d>();
      _txts = new List<string>();
      var ids = new List<int>();
      if (DA.GetDataList(0, gh_typ))
      {
        foreach (var goo in gh_typ) 
        {
          if (goo.Value is GsaNodeGoo node)
          {
            _txts.Add(node.Value.Id.ToString());
            ids.Add(node.Value.Id);
            _pts.Add(node.Value.Point);
            continue;
          }

          if (goo.Value is GsaElement1dGoo e1d)
          {
            _txts.Add(e1d.Value.Id.ToString());
            ids.Add(e1d.Value.Id);
            _pts.Add(e1d.Value.Line.PointAt(0.5));
            continue;
          }

          if (goo.Value is GsaElement2dGoo e2d)
          {
            for (int i = 0; i < e2d.Value.Mesh.Faces.Count; i++)
            {
              _txts.Add(e2d.Value.Ids[i].ToString());
              ids.Add(e2d.Value.Ids[i]);
              _pts.Add(e2d.Value.Mesh.Faces.GetFaceCenter(i));
            }
            continue;
          }

          if (goo.Value is GsaElement3dGoo e3d)
          {
            for (int i = 0; i < e3d.Value.NgonMesh.Faces.Count; i++)
            {
              _txts.Add(e3d.Value.Ids[i].ToString());
              ids.Add(e3d.Value.Ids[i]);
              _pts.Add(e3d.Value.NgonMesh.Faces.GetFaceCenter(i));
            }
            continue;
          }

          if (goo.Value is GsaMember1dGoo m1d)
          {
            _txts.Add(m1d.Value.Id.ToString());
            ids.Add(m1d.Value.Id);
            _pts.Add(m1d.Value.PolyCurve.PointAtNormalizedLength(0.5));
            continue;
          }

          if (goo.Value is GsaMember2dGoo m2d)
          {
            _txts.Add(m2d.Value.Id.ToString());
            ids.Add(m2d.Value.Id);
            var pl = new Polyline();
            m2d.Value.PolyCurve.TryGetPolyline(out pl);
            _pts.Add(pl.CenterPoint());
            continue;
          }

          if (goo.Value is GsaMember3dGoo m3d)
          {
            _txts.Add(m3d.Value.Id.ToString());
            ids.Add(m3d.Value.Id);
            
            _pts.Add(m3d.Value.SolidMesh.GetBoundingBox(false).Center);
            continue;
          }

          if (goo.Value is PointResultGoo resPoint)
          {
            _txts.Add(resPoint.NodeId.ToString());
            ids.Add(resPoint.NodeId);
            _pts.Add(resPoint.Value);
            continue;
          }

          if (goo.Value is LineResultGoo resLine)
          {
            _txts.Add(resLine.ElementId.ToString());
            ids.Add(resLine.ElementId);
            _pts.Add(resLine.Value.PointAt(0.5));
            continue;
          }

          if (goo.Value is MeshResultGoo resMesh)
          {
            for (int i = 0; i < resMesh.ElementIds.Count; i++)
            {
              _txts.Add(resMesh.ElementIds[i].ToString());
              ids.Add(resMesh.ElementIds[i]);
              _pts.Add(resMesh.Value.Faces.GetFaceCenter(i));
            }
            continue;
          }

          if (goo.Value is VectorResultGoo resVector)
          {
            _txts.Add(resVector.NodeId.ToString());
            ids.Add(resVector.NodeId);
            _pts.Add(resVector.StartingPoint);
            continue;
          }
        }
      }

      DA.SetDataList(0, _pts);
      DA.SetDataList(1, ids);
    }

    public override void DrawViewportWires(IGH_PreviewArgs args)
    {
      base.DrawViewportWires(args);

      if (_txts != null)
      {
        for(int i = 0; i < _txts.Count; i++)
        {
          var positionOnTheScreen = args.Viewport.WorldToClient(_pts[i]);
          args.Display.Draw2dText(_txts[i], this.Attributes.Selected ? args.WireColour_Selected : args.WireColour, positionOnTheScreen, true);
        }
      }
    }
  }
}
