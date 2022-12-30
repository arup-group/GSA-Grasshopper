using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components
{
  public class GetMeshResultsInfo : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("4089b9d6-490d-4491-b623-f99ed01630aa");
    public override GH_Exposure Exposure => GH_Exposure.quinary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Result2dInfo;

    public GetMeshResultsInfo() : base("MeshResultInfo",
      "MeshResInfo",
      "Get Element 2D or Element 3D Contour Result values",
      CategoryName.Name(),
      SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Mesh", "M", "Mesh with coloured result values", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddMeshParameter("Mesh", "M", "Mesh", GH_ParamAccess.item);
      pManager.AddPointParameter("Vertices", "V", "Mesh vertices", GH_ParamAccess.list);
      pManager.AddGenericParameter("Result Values", "R", "Result value at each Mesh Vertex as UnitNumber", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      MeshResultGoo res = null;
      DA.GetData(0, ref res);
      DA.SetData(0, res.Value);

      List<Point3d> vertices = new List<Point3d>();
      List<GH_UnitNumber> values = new List<GH_UnitNumber>();
      for (int i = 0; i < res.ResultValues.Count; i++)
      {
        values.AddRange(res.ResultValues[i].Select(r => new GH_UnitNumber(r)));

        if (res.Ngons[i])
        {
          uint[] vertindex = res.Value.Ngons[i].BoundaryVertexIndexList();
          List<Point3d> ngonVertices = new List<Point3d>();
          foreach (uint vert in vertindex)
            ngonVertices.Add(res.Value.Vertices[(int)vert]);

          // if result value at centre point exist, add centre point
          if (vertindex.Length != res.ResultValues[i].Count)
          {
            ngonVertices.Add(
              new Point3d(
                ngonVertices.Select(pt => pt.X).Average(),
                ngonVertices.Select(pt => pt.Y).Average(),
                ngonVertices.Select(pt => pt.Z).Average()));
          }

          vertices.AddRange(ngonVertices);
        }
        else
        {
          List<Point3d> regVertices = new List<Point3d>()
          {
            res.Value.Vertices[res.Value.Faces[i].A],
            res.Value.Vertices[res.Value.Faces[i].B],
            res.Value.Vertices[res.Value.Faces[i].C]
          };

          if (res.Value.Faces[i].IsQuad)
            regVertices.Add(res.Value.Vertices[res.Value.Faces[i].D]);

          // if result value at centre point exist, add centre point
          if (regVertices.Count != res.ResultValues[i].Count)
          {
            regVertices.Add(
              new Point3d(
                regVertices.Select(pt => pt.X).Average(),
                regVertices.Select(pt => pt.Y).Average(),
                regVertices.Select(pt => pt.Z).Average()));
          }

          vertices.AddRange(regVertices);
        }
      }

      DA.SetDataList(1, vertices);
      DA.SetDataList(2, values);
    }
  }
}
