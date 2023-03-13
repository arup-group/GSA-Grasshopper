﻿using System;
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
using Grasshopper.Kernel.Data;

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
      pManager.AddGenericParameter("Node/Element/Member/Result", "Geo", "Node, Element, Member or Point/Line/Mesh result to get ID for.", GH_ParamAccess.tree);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddPointParameter("Position", "P", "The (centre/mid) location(s) of the object(s)", GH_ParamAccess.tree);
      pManager.HideParameter(0);
      pManager.AddIntegerParameter("Index", "ID", "The objects ID(s)", GH_ParamAccess.tree);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // clear cached preview data
      _pts = new List<Point3d>();
      _txts = new List<string>();
      
      if (DA.GetDataTree(0, out GH_Structure<IGH_Goo> tree))
      {
        // component outputs
        var ids = new GH_Structure<GH_Integer>();
        var ghPts = new GH_Structure<GH_Point>();

        foreach (var path in tree.Paths) 
        {
          foreach (IGH_Goo goo in tree.get_Branch(path))
          {
            if (goo is GsaElement2dGoo e2d)
            {
              for (int i = 0; i < e2d.Value.Mesh.Faces.Count; i++)
              {
                _txts.Add(e2d.Value.Ids[i].ToString());
                ids.Append(new GH_Integer(e2d.Value.Ids[i]), path);
                _pts.Add(e2d.Value.Mesh.Faces.GetFaceCenter(i));
                ghPts.Append(new GH_Point(e2d.Value.Mesh.Faces.GetFaceCenter(i)), path);
              }
              continue;
            }

            if (goo is GsaElement3dGoo e3d)
            {
              for (int i = 0; i < e3d.Value.NgonMesh.Ngons.Count; i++)
              {
                _txts.Add(e3d.Value.Ids[i].ToString());
                ids.Append(new GH_Integer(e3d.Value.Ids[i]), path);
                _pts.Add(e3d.Value.NgonMesh.Ngons.GetNgonCenter(i));
                ghPts.Append(new GH_Point(e3d.Value.NgonMesh.Ngons.GetNgonCenter(i)), path);
              }
              continue;
            }

            if (goo is MeshResultGoo resMesh)
            {
              for (int i = 0; i < resMesh.ElementIds.Count; i++)
              {
                _txts.Add(resMesh.ElementIds[i].ToString());
                ids.Append(new GH_Integer(resMesh.ElementIds[i]), path);
                if (resMesh.Value.Ngons.Count > 0)
                { 
                  _pts.Add(resMesh.Value.Ngons.GetNgonCenter(i));
                  ghPts.Append(new GH_Point(resMesh.Value.Ngons.GetNgonCenter(i)), path);
                }
                else
                {
                  _pts.Add(resMesh.Value.Faces.GetFaceCenter(i));
                  ghPts.Append(new GH_Point(resMesh.Value.Faces.GetFaceCenter(i)), path);
                }
              }
              continue;
            }

            int id = 0;
            Point3d pt = Point3d.Unset;

            if (goo is GsaNodeGoo node)
            {
              id = node.Value.Id;
              pt = node.Value.Point;
            }

            if (goo is GsaElement1dGoo e1d)
            {
              id = e1d.Value.Id;
              pt = e1d.Value.Line.PointAtNormalizedLength(0.5);
            }

            if (goo is GsaMember1dGoo m1d)
            {
              id = m1d.Value.Id;
              pt = m1d.Value.PolyCurve.PointAtNormalizedLength(0.5);
            }

            if (goo is GsaMember2dGoo m2d)
            {
              id = m2d.Value.Id;
              m2d.Value.PolyCurve.TryGetPolyline(out Polyline pl);
              pt = pl.CenterPoint();
            }

            if (goo is GsaMember3dGoo m3d)
            {
              id = m3d.Value.Id;
              pt = m3d.Value.SolidMesh.GetBoundingBox(false).Center;
            }

            if (goo is PointResultGoo resPoint)
            {
              id = resPoint.NodeId;
              pt = resPoint.Value;
            }

            if (goo is LineResultGoo resLine)
            {
              id = resLine.ElementId;
              pt = resLine.Value.PointAt(0.5);
            }

            if (goo is VectorResultGoo resVector)
            {
              id = resVector.NodeId;
              pt = resVector.StartingPoint;
            }

            _txts.Add(id.ToString());
            ids.Append(new GH_Integer(id), path);
            _pts.Add(pt);
            ghPts.Append(new GH_Point(pt), path);
          }
        }

        DA.SetDataTree(0, ghPts);
        DA.SetDataTree(1, ids);
      }
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

    public override BoundingBox ClippingBox => _pts != null ? new BoundingBox(_pts) : base.ClippingBox;
  }
}
