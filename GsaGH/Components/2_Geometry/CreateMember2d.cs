using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create new 2D Member
  /// </summary>
  public class CreateMember2d : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("d996b426-9655-4abf-af0d-3e206d252b00");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateMem2d;

    public CreateMember2d() : base("Create 2D Member", "Mem2D", "Create GSA Member 2D",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddBrepParameter("Brep", "B",
        "Planar Brep (non-planar geometry will be automatically converted to an average plane of exterior boundary control points))",
        GH_ParamAccess.item);
      pManager.AddPointParameter("Incl. Points", "(P)",
        "Inclusion points (will automatically be projected onto Brep)", GH_ParamAccess.list);
      pManager.AddCurveParameter("Incl. Curves", "(C)",
        "Inclusion curves (will automatically be made planar and projected onto brep, and converted to Arcs and Lines)",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaProp2dParameter());
      pManager.AddNumberParameter("Mesh Size in model units", "Ms", "Target mesh size",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Internal Offset", "IO",
        "Set Automatic Internal Offset of Member", GH_ParamAccess.item, false);

      pManager.HideParameter(0);
      pManager.HideParameter(1);
      pManager.HideParameter(2);

      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaMember2dParameter());
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GH_Brep ghbrep = null;
      da.GetData(0, ref ghbrep);

      var points = new List<Point3d>();
      var ghpts = new List<GH_Point>();
      if (da.GetDataList(1, ghpts)) {
        points = ghpts.Select(pt => pt.Value).ToList();
      }

      var crvs = new List<Curve>();
      var ghcrvs = new List<GH_Curve>();
      if (da.GetDataList(2, ghcrvs)) {
        crvs = ghcrvs.Select(crv => crv.Value).ToList();
      }

      var mem = new GsaMember2d();
      mem = new GsaMember2d(ghbrep.Value, crvs, points);

      GsaProp2dGoo prop2dGoo = null;
      if (da.GetData(3, ref prop2dGoo)) {
          mem.Prop2d = prop2dGoo.Value;
      }

      double meshSize = 0;
      if (da.GetData(4, ref meshSize)) {
        mem.MeshSize = meshSize;
      }

      bool internalOffset = false;
      if (da.GetData(5, ref internalOffset)) {
        mem.AutomaticInternalOffset = internalOffset;
      }

      da.SetData(0, new GsaMember2dGoo(mem));
    }
  }
}
