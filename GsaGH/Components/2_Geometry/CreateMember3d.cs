using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  /// Component to create new 3d Member
  /// </summary>
  public class CreateMember3d : GH_OasysComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("08a48fa5-8aaa-43fb-a095-9142794684f7");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateMem3d;

    public CreateMember3d() : base("Create 3D Member",
      "Mem3D",
      "Create GSA Member 3D",
      CategoryName.Name(),
      SubCategoryName.Cat2()) { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGeometryParameter("Solid", "S", "Solid Geometry - Closed Brep or Mesh", GH_ParamAccess.item);
      pManager.AddParameter(new GsaProp3dParameter());
      pManager.AddNumberParameter("Mesh Size in model units", "Ms", "Targe mesh size", GH_ParamAccess.item);

      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaMember3dParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp)) {
        return;
      }

      if (ghTyp == null) {
        this.AddRuntimeWarning("Solid input is null");
      }
      var mem = new GsaMember3d();
      var brep = new Brep();
      var mesh = new Mesh();
      if (GH_Convert.ToBrep(ghTyp.Value, ref brep, GH_Conversion.Both)) {
        if (brep.IsValid) {
          try {
            mem = new GsaMember3d(brep);
          }
          catch (Exception e) {
            this.AddRuntimeWarning(e.Message);
            return;
          }
        }
        else {
          this.AddRuntimeWarning("S input is not a valid Brep geometry");
          return;
        }
      }
      else if (GH_Convert.ToMesh(ghTyp.Value, ref mesh, GH_Conversion.Both)) {
        try {
          mem = new GsaMember3d(mesh);
        }
        catch (Exception e) {
          this.AddRuntimeWarning(e.Message);
          return;
        }
      }
      else {
        this.AddRuntimeError("Unable to convert Geometry input to a 3D Member");
        return;
      }

      // 1 prop3d to be implemented GsaAPI
      ghTyp = new GH_ObjectWrapper();
      var prop3d = new GsaProp3d();
      if (da.GetData(1, ref ghTyp)) {
        switch (ghTyp.Value)
        {
          case GsaProp3dGoo _:
            ghTyp.CastTo(ref prop3d);
            mem.Prop3d = prop3d;
            break;
          case GsaMaterialGoo _:
          {
            var mat = new GsaMaterial();
            ghTyp.CastTo(ref mat);
            prop3d = new GsaProp3d(mat);
            mem.Prop3d = prop3d;
            break;
          }
          default: {
            if (GH_Convert.ToInt32(ghTyp.Value, out int id, GH_Conversion.Both))
              mem.Prop3d = new GsaProp3d(id);
            else {
              this.AddRuntimeWarning("Unable to convert PA input to a 2D Property of reference integer");
            }

            break;
          }
        }
      }

      // 2 mesh size
      double meshSize = 0;
      if (da.GetData(2, ref meshSize))
        mem.MeshSize = meshSize;

      da.SetData(0, new GsaMember3dGoo(mem));
    }
  }
}
