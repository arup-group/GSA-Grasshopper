using System;
using System.Drawing;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a 3D Member
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class EditMember3d_OBSOLETE : GH_OasysComponent, IGH_PreviewObject {
    public override Guid ComponentGuid => new Guid("955e573d-7608-4ac6-b436-54135f7714f6");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Edit3dMember;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;

    public EditMember3d_OBSOLETE() : base("Edit 3D Member", "Mem3dEdit", "Modify GSA 3D Member",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);

      var unitsMenu = new ToolStripMenuItem("Select unit", Resources.ModelUnits) {
        Enabled = true,
        ImageScaling = ToolStripItemImageScaling.SizeToFit,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => Update(unit)) {
          Checked = unit == Length.GetAbbreviation(_lengthUnit),
          Enabled = true,
        };
        unitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }

    public override bool Read(GH_IReader reader) {
      if (reader.ItemExists("LengthUnit")) {
        _lengthUnit
          = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("LengthUnit"));
        return base.Read(reader) || Params.ReadAllParameterData(reader);
      }

      _lengthUnit = DefaultUnits.LengthUnitGeometry;
      return base.Read(reader);
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetString("LengthUnit", _lengthUnit.ToString());
      return base.Write(writer);
    }

    protected override void BeforeSolveInstance() {
      Message = Length.GetAbbreviation(_lengthUnit);
    }

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
      pManager.AddGenericParameter("Mesh Size [" + Length.GetAbbreviation(_lengthUnit) + "]", "Ms",
        "Set Member Mesh Size", GH_ParamAccess.item);
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
      pManager.AddGenericParameter("Mesh Size [" + Length.GetAbbreviation(_lengthUnit) + "]", "Ms",
        "Get Targe mesh size", GH_ParamAccess.item);
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

      var ghId = new GH_Integer();
      if (da.GetData(1, ref ghId)) {
        if (GH_Convert.ToInt32(ghId, out int id, GH_Conversion.Both)) {
          mem.Id = id;
        }
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

      if (Params.Input[4].Sources.Count > 0) {
        mem.ApiMember.MeshSize = ((Length)Input.UnitNumber(this, da, 4, _lengthUnit, true)).Meters;
      }

      var ghBoolean = new GH_Boolean();
      if (da.GetData(5, ref ghBoolean)) {
        if (GH_Convert.ToBoolean(ghBoolean, out bool mbool, GH_Conversion.Both)) {
          if (mem.ApiMember.IsIntersector != mbool) {
            mem.ApiMember.IsIntersector = mbool;
          }
        }
      }

      var ghName = new GH_String();
      if (da.GetData(6, ref ghName)) {
        if (GH_Convert.ToString(ghName, out string name, GH_Conversion.Both)) {
          mem.ApiMember.Name = name;
        }
      }

      var ghGroup = new GH_Integer();
      if (da.GetData(7, ref ghGroup)) {
        if (GH_Convert.ToInt32(ghGroup, out int grp, GH_Conversion.Both)) {
          mem.ApiMember.Group = grp;
        }
      }

      var ghColour = new GH_Colour();
      if (da.GetData(8, ref ghColour)) {
        if (GH_Convert.ToColor(ghColour, out Color col, GH_Conversion.Both)) {
          mem.ApiMember.Colour = col;
        }
      }

      var ghDummy = new GH_Boolean();
      if (da.GetData(9, ref ghDummy)) {
        if (GH_Convert.ToBoolean(ghDummy, out bool dum, GH_Conversion.Both)) {
          mem.ApiMember.IsDummy = dum;
        }
      }

      da.SetData(0, new GsaMember3dGoo(mem));
      da.SetData(1, mem.Id);
      da.SetData(2, mem.SolidMesh);
      da.SetData(3, new GsaProperty3dGoo(mem.Prop3d));
      da.SetData(4,
        new GH_UnitNumber(new Length(mem.ApiMember.MeshSize, LengthUnit.Meter).ToUnit(_lengthUnit)));
      da.SetData(5, mem.ApiMember.IsIntersector);
      da.SetData(6, mem.ApiMember.Name);
      da.SetData(7, mem.ApiMember.Group);
      da.SetData(8, mem.ApiMember.Colour);
      da.SetData(9, mem.ApiMember.IsDummy);
      da.SetData(10, mem.ApiMember.Topology);
    }

    private void Update(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      Message = unit;
      ExpireSolution(true);
    }
  }
}
