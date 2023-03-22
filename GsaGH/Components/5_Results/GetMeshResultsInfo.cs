using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using Rhino.Geometry;

namespace GsaGH.Components {
  public class GetMeshResultsInfo : GH_OasysComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("4089b9d6-490d-4491-b623-f99ed01630aa");
    public override GH_Exposure Exposure => GH_Exposure.quinary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Result2dInfo;

    public GetMeshResultsInfo() : base("MeshResultInfo",
      "MeshResInfo",
      "Get Element 2D or Element 3D Contour Result values",
      CategoryName.Name(),
      SubCategoryName.Cat5()) {
        Hidden = true;
    }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Result Mesh", "M", "Mesh with coloured result values", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddMeshParameter("Mesh", "M", "Mesh", GH_ParamAccess.item);
      pManager.AddPointParameter("Vertices", "V", "Mesh vertices", GH_ParamAccess.list);
      pManager.AddGenericParameter("Result Values", "R", "Result value at each Mesh Vertex as UnitNumber", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      MeshResultGoo res = null;
      da.GetData(0, ref res);
      da.SetData(0, res.Value);

      var vertices = new List<Point3d>();
      var values = new List<GH_UnitNumber>();
      for (int i = 0; i < res.ResultValues.Count; i++) {
        vertices.AddRange(res.Vertices[i]);
        values.AddRange(res.ResultValues[i].Select(r => new GH_UnitNumber(r)));
        if (res.Vertices[i].Count >= res.ResultValues[i].Count) {
          continue;
        }

        double x = 0;
        double y = 0;
        double z = 0;
        foreach (Point3d p in res.Vertices[i]) {
          x += p.X;
          y += p.Y;
          z += p.Z;
        }
        var pt = new Point3d(
          x / res.Vertices[i].Count,
          y / res.Vertices[i].Count,
          z / res.Vertices[i].Count);
        vertices.Add(pt);
      }

      da.SetDataList(1, vertices);
      da.SetDataList(2, values);
    }
  }
}
