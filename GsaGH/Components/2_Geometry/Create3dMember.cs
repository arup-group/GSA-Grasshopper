using System;
using System.Drawing;

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
  ///   Component to create new 3d Member
  /// </summary>
  public class Create3dMember : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("08a48fa5-8aaa-43fb-a095-9142794684f7");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Create3dMember;

    public Create3dMember() : base("Create 3D Member", "Mem3D", "Create GSA Member 3D",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGeometryParameter("Solid", "S", "Solid Geometry - Closed Brep or Mesh",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaProperty3dParameter());
      pManager.AddNumberParameter("Mesh Size in model units", "Ms", "Targe mesh size",
        GH_ParamAccess.item);

      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaMember3dParameter());
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaMember3d member;
      var brep = new Brep();
      var mesh = new Mesh();

      var ghTyp = new GH_ObjectWrapper();
      da.GetData(0, ref ghTyp);
      if (GH_Convert.ToBrep(ghTyp.Value, ref brep, GH_Conversion.Both)) {
        member = new GsaMember3d(brep);
      } else if (GH_Convert.ToMesh(ghTyp.Value, ref mesh, GH_Conversion.Both)) {
        member = new GsaMember3d(mesh);
      } else {
        this.AddRuntimeError($"Unable to convert Geometry input ({ghTyp.GetTypeName()}) to a 3D Member");
        return;
      }

      GsaProperty3dGoo prop3dGoo = null;
      if (da.GetData(1, ref prop3dGoo)) {
        member.Prop3d = prop3dGoo.Value;
      }

      double meshSize = 0;
      if (da.GetData(2, ref meshSize)) {
        member.ApiMember.MeshSize = meshSize;
      }

      da.SetData(0, new GsaMember3dGoo(member));
    }
  }
}
