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
  ///   Component to edit a 3D Member
  /// </summary>
  public class Edit3dMember : GH_OasysComponent, IGH_VariableParameterComponent {
    public override Guid ComponentGuid => new Guid("e7d66219-2243-4108-9d6e-4a84dbf07d55");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Edit3dMember;

    public Edit3dMember() : base("Edit 3D Member", "Mem3dEdit", "Modify GSA 3D Member",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index) {
      return false;
    }

    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index) {
      return false;
    }

    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index) {
      return null;
    }

    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) {
      return false;
    }

    public void VariableParameterMaintenance() { }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaMember3dParameter(), GsaMember3dGoo.Name,
        GsaMember3dGoo.NickName,
        GsaMember3dGoo.Description + " to get or set information for. Leave blank to create a new "
        + GsaMember3dGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member3d Number", "ID",
        "Set Member Number. If ID is set it will replace any existing 3d Member in the model",
        GH_ParamAccess.item);
      pManager.AddGeometryParameter("Solid", "S", "Reposition Solid Geometry - Closed Brep or Mesh",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaProperty3dParameter(), "3D Property", "PV", "Set new 3D Property.",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Mesh Size in model units", "Ms", "Set Member Mesh Size",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Mesh With Others", "M/o", "Mesh with others?",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Member3d Name", "Na", "Set Name of Member3d", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member3d Group", "Gr", "Set Member 3d Group",
        GH_ParamAccess.item);
      pManager.AddColourParameter("Member3d Colour", "Co", "Set Member 3d Colour",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Member", "Dm", "Set Member to Dummy",
        GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }

      pManager.HideParameter(0);
      pManager.HideParameter(2);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaMember3dParameter(), GsaMember3dGoo.Name,
        GsaMember3dGoo.NickName, GsaMember3dGoo.Description + " with applied changes.",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member Number", "ID", "Get Member Number", GH_ParamAccess.item);
      pManager.AddMeshParameter("Solid Mesh", "M", "Member Solid Mesh", GH_ParamAccess.item);
      pManager.HideParameter(2);
      pManager.AddParameter(new GsaProperty3dParameter(), "3D Property", "PV", "Get 3D Property",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Mesh Size in model units", "Ms", "Get Target mesh size",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Mesh With Others", "M/o", "Get if to mesh with others",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Member Name", "Na", "Get Name of Member", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member Group", "Gr", "Get Member Group", GH_ParamAccess.item);
      pManager.AddColourParameter("Member Colour", "Co", "Get Member Colour", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Member", "Dm", "Get if Member is Dummy",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Topology", "Tp",
        "Get the Member's original topology list referencing node IDs in Model that Model was created from",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var mem = new GsaMember3d();

      GsaMember3dGoo member3dGoo = null;
      if (da.GetData(0, ref member3dGoo)) {
        mem = new GsaMember3d(member3dGoo.Value);
      }

      int id = 0;
      if (da.GetData(1, ref id)) {
        mem.Id = id;
      }

      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(2, ref ghTyp)) {
        var brep = new Brep();
        var mesh = new Mesh();
        if (GH_Convert.ToBrep(ghTyp.Value, ref brep, GH_Conversion.Both)) {
          mem.UpdateGeometry(brep);
        } else if (GH_Convert.ToMesh(ghTyp.Value, ref mesh, GH_Conversion.Both)) {
          mem.UpdateGeometry(mesh);
        } else {
          this.AddRuntimeError("Unable to convert Geometry input to a 3D Member");
          return;
        }
      }

      GsaProperty3dGoo prop3dGoo = null;
      if (da.GetData(3, ref prop3dGoo)) {
        mem.Prop3d = prop3dGoo.Value;
      }

      double meshSize = 0;
      if (da.GetData(4, ref meshSize)) {
        mem.ApiMember.MeshSize = meshSize;
      }

      bool intersector = false;
      if (da.GetData(5, ref intersector)) {
        mem.ApiMember.IsIntersector = intersector;
      }

      string name = string.Empty;
      if (da.GetData(6, ref name)) {
        mem.ApiMember.Name = name;
      }

      int group = 0;
      if (da.GetData(7, ref group)) {
        mem.ApiMember.Group = group;
      }

      Color colour = Color.Empty;
      if (da.GetData(8, ref colour)) {
        mem.ApiMember.Colour = colour;
      }

      bool dummy = false;
      if (da.GetData(9, ref dummy)) {
        mem.ApiMember.IsDummy = dummy;
      }

      mem.UpdatePreview();

      da.SetData(0, new GsaMember3dGoo(mem));
      da.SetData(1, mem.Id);
      da.SetData(2, mem.SolidMesh);
      da.SetData(3, new GsaProperty3dGoo(mem.Prop3d));
      da.SetData(4, mem.ApiMember.MeshSize);
      da.SetData(5, mem.ApiMember.IsIntersector);
      da.SetData(6, mem.ApiMember.Name);
      da.SetData(7, mem.ApiMember.Group);
      da.SetData(8, (Color)mem.ApiMember.Colour);
      da.SetData(9, mem.ApiMember.IsDummy);
      da.SetData(10, mem.ApiMember.Topology);
    }
  }
}
