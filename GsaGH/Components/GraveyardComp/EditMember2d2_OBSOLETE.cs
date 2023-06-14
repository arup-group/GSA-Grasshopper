using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using Rhino.Collections;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a 2D Member
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class EditMember2d2_OBSOLETE : GH_OasysComponent, IGH_VariableParameterComponent {
    public override Guid ComponentGuid => new Guid("54bcb967-d3a1-4878-925b-5fd765d1b476");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditMem2d;

    public EditMember2d2_OBSOLETE() : base("Edit 2D Member", "Mem2dEdit", "Modify GSA 2D Member",
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
      pManager.AddParameter(new GsaMember2dParameter(), GsaMember2dGoo.Name,
        GsaMember2dGoo.NickName,
        GsaMember2dGoo.Description + " to get or set information for. Leave blank to create a new "
        + GsaMember2dGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member2d Number", "ID",
        "Set Member Number. If ID is set it will replace any existing 2d Member in the model",
        GH_ParamAccess.item);
      pManager.AddBrepParameter("Brep", "B",
        "Reposition Member Brep (non-planar geometry will be automatically converted to an average plane from exterior boundary control points)",
        GH_ParamAccess.item);
      pManager.AddPointParameter("Incl. Points", "(P)",
        "Add inclusion points (will automatically be projected onto Brep)", GH_ParamAccess.list);
      pManager.AddCurveParameter("Incl. Curves", "(C)",
        "Add inclusion curves (will automatically be made planar and projected onto brep, and converted to Arcs and Lines)",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaProp2dParameter(), "2D Property", "PA", "Set new 2D Property.",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member2d Group", "Gr", "Set Member 2d Group",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Member Type", "mT",
        "Set 2D Member Type" + Environment.NewLine
        + "Default is 1: Generic 2D - Accepted inputs are:" + Environment.NewLine + "4: Slab"
        + Environment.NewLine + "5: Wall" + Environment.NewLine + "7: Ribbed Slab"
        + Environment.NewLine + "12: Void-cutter", GH_ParamAccess.item);
      pManager.AddTextParameter("2D Element Type", "eT",
        "Set Member 2D Analysis Element Type" + Environment.NewLine + "Accepted inputs are:"
        + Environment.NewLine + "0: Linear - Tri3/Quad4 Elements (default)" + Environment.NewLine
        + "1: Quadratic - Tri6/Quad8 Elements" + Environment.NewLine + "2: Rigid Diaphragm",
        GH_ParamAccess.item);

      pManager.AddParameter(new GsaOffsetParameter(), "Offset", "Of", "Set Member Offset",
        GH_ParamAccess.item);

      pManager.AddNumberParameter("Mesh Size in model units", "Ms", "Set target mesh size",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Mesh With Others", "M/o", "Mesh with others?",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Member2d Name", "Na", "Set Name of Member2d", GH_ParamAccess.item);
      pManager.AddColourParameter("Member2d Colour", "Co", "Set Member 2d Colour",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Member", "Dm", "Set Member to Dummy",
        GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }

      pManager.HideParameter(0);
      pManager.HideParameter(2);
      pManager.HideParameter(3);
      pManager.HideParameter(4);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaMember2dParameter(), GsaMember2dGoo.Name,
        GsaMember2dGoo.NickName, GsaMember2dGoo.Description + " with applied changes.",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member Number", "ID", "Get Member Number", GH_ParamAccess.item);
      pManager.AddBrepParameter("Brep", "B", "Member Brep", GH_ParamAccess.item);
      pManager.HideParameter(2);
      pManager.AddPointParameter("Incl. Points", "(P)", "Get Inclusion points",
        GH_ParamAccess.list);
      pManager.HideParameter(3);
      pManager.AddCurveParameter("Incl. Curves", "(C)", "Get Inclusion curves",
        GH_ParamAccess.list);
      pManager.HideParameter(4);
      pManager.AddParameter(new GsaProp2dParameter(), "2D Property", "PA",
        "Get 2D Section Property", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member Group", "Gr", "Get Member Group", GH_ParamAccess.item);

      pManager.AddTextParameter("Member Type", "mT", "Get 2D Member Type", GH_ParamAccess.item);
      pManager.AddTextParameter("2D Element Type", "eT",
        "Get Member 2D Analysis Element Type" + Environment.NewLine
        + "0: Linear (Tri3/Quad4), 1: Quadratic (Tri6/Quad8), 2: Rigid Diaphragm",
        GH_ParamAccess.item);

      pManager.AddParameter(new GsaOffsetParameter(), "Offset", "Of", "Get Member Offset",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Mesh Size in model units", "Ms", "Get Member Mesh Size",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Mesh With Others", "M/o", "Get if to mesh with others",
        GH_ParamAccess.item);

      pManager.AddTextParameter("Member Name", "Na", "Get Name of Member", GH_ParamAccess.item);
      pManager.AddColourParameter("Member Colour", "Co", "Get Member Colour", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Member", "Dm", "Get if Member is Dummy",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Topology", "Tp",
        "Get the Member's original topology list referencing node IDs in Model that Model was created from",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var mem = new GsaMember2d();

      GsaMember2dGoo member2dGoo = null;
      if (da.GetData(0, ref member2dGoo)) {
        mem = member2dGoo.Value.Duplicate(true);
      }

      var ghId = new GH_Integer();
      if (da.GetData(1, ref ghId)) {
        if (GH_Convert.ToInt32(ghId, out int id, GH_Conversion.Both)) {
          mem.Id = id;
        }
      }

      Brep brep = mem.Brep;
      var ghBrep = new GH_Brep();
      var curveList = new CurveList(mem.InclusionLines ?? new List<PolyCurve>());
      var curves = curveList.ToList();
      var ghCurves = new List<GH_Curve>();
      var ghPoints = new List<GH_Point>();
      List<Point3d> points = mem.InclusionPoints;

      if (da.GetData(2, ref ghBrep) || da.GetDataList(3, ghPoints) || da.GetDataList(4, ghCurves)) {
        if (da.GetData(2, ref ghBrep)) {
          GH_Convert.ToBrep(ghBrep, ref brep, GH_Conversion.Both);
        }

        ghPoints = new List<GH_Point>();
        if (da.GetDataList(3, ghPoints)) {
          points = new List<Point3d>();
          foreach (GH_Point point in ghPoints) {
            var pt = new Point3d();
            if (GH_Convert.ToPoint3d(point, ref pt, GH_Conversion.Both)) {
              points.Add(pt);
            }
          }
        }

        ghCurves = new List<GH_Curve>();
        if (da.GetDataList(4, ghCurves)) {
          curves = new List<Curve>();
          foreach (GH_Curve curve in ghCurves) {
            Curve crv = null;
            if (GH_Convert.ToCurve(curve, ref crv, GH_Conversion.Both)) {
              curves.Add(crv);
            }
          }
        }

        mem = mem.UpdateGeometry(brep, curves, points);
      }

      GsaProp2dGoo prop2dGoo = null;
      if (da.GetData(5, ref prop2dGoo)) {
        mem.Prop2d = prop2dGoo.Value;
      }

      var ghGroup = new GH_Integer();
      if (da.GetData(6, ref ghGroup)) {
        if (GH_Convert.ToInt32(ghGroup, out int grp, GH_Conversion.Both)) {
          mem.Group = grp;
        }
      }

      var ghString = new GH_String();
      if (da.GetData(7, ref ghString)) {
        if (GH_Convert.ToInt32(ghString, out int typeInt, GH_Conversion.Both)) {
          mem.Type = (MemberType)typeInt;
        } else if (GH_Convert.ToString(ghString, out string typestring, GH_Conversion.Both)) {
          try {
            mem.Type = Mappings.GetMemberType(typestring);
          } catch (ArgumentException) {
            this.AddRuntimeError("Unable to change Member Type");
          }
        }
      }

      ghString = new GH_String();
      if (da.GetData(8, ref ghString)) {
        if (GH_Convert.ToInt32(ghString, out int typeInt, GH_Conversion.Both)) {
          mem.Type2D = (AnalysisOrder)typeInt;
        } else if (GH_Convert.ToString(ghString, out string typestring, GH_Conversion.Both)) {
          try {
            mem.Type2D = Mappings.GetAnalysisOrder(typestring);
          } catch (ArgumentException) {
            this.AddRuntimeError("Unable to change Analysis Element Type");
          }
        }
      }

      GsaOffsetGoo offset = null;
      if (da.GetData(9, ref offset)) {
        mem.Offset = offset.Value;
      }

      double meshSize = 0;
      if (da.GetData(10, ref meshSize)) {
        mem.MeshSize = meshSize;
      }

      var ghBoolean = new GH_Boolean();
      if (da.GetData(11, ref ghBoolean)) {
        if (GH_Convert.ToBoolean(ghBoolean, out bool mbool, GH_Conversion.Both)) {
          mem.MeshWithOthers = mbool;
        }
      }

      var ghName = new GH_String();
      if (da.GetData(12, ref ghName)) {
        if (GH_Convert.ToString(ghName, out string name, GH_Conversion.Both)) {
          mem.Name = name;
        }
      }

      var ghColour = new GH_Colour();
      if (da.GetData(13, ref ghColour)) {
        if (GH_Convert.ToColor(ghColour, out Color col, GH_Conversion.Both)) {
          mem.Colour = col;
        }
      }

      var ghDummy = new GH_Boolean();
      if (da.GetData(14, ref ghDummy)) {
        if (GH_Convert.ToBoolean(ghDummy, out bool dum, GH_Conversion.Both)) {
          mem.IsDummy = dum;
        }
      }

      da.SetData(0, new GsaMember2dGoo(mem));
      da.SetData(1, mem.Id);
      da.SetData(2, mem.Brep);
      da.SetDataList(3, mem.InclusionPoints);
      da.SetDataList(4, mem.InclusionLines);
      da.SetData(5, new GsaProp2dGoo(mem.Prop2d));
      da.SetData(6, mem.Group);
      da.SetData(7, Mappings.memberTypeMapping.FirstOrDefault(x => x.Value == mem.Type).Key);
      da.SetData(8, Mappings.analysisOrderMapping.FirstOrDefault(x => x.Value == mem.Type2D).Key);
      da.SetData(9, new GsaOffsetGoo(mem.Offset));
      da.SetData(10, mem.MeshSize);
      da.SetData(11, mem.MeshWithOthers);
      da.SetData(12, mem.Name);
      da.SetData(13, mem.Colour);
      da.SetData(14, mem.IsDummy);
      da.SetData(15, mem.ApiMember.Topology.ToString());
    }
  }
}
