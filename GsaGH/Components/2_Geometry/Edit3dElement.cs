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

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a 3D Element
  /// </summary>
  public class Edit3dElement : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("040f2915-543d-41ef-9a64-0c4055e47a63");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Edit3dElement;

    public Edit3dElement() : base("Edit 3D Element", "Elem3dEdit", "Modify GSA 3D Element",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaElement3dParameter(), GsaElement3dGoo.Name,
        GsaElement3dGoo.NickName, GsaElement3dGoo.Description + " to get or set information for.",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Number", "ID", "Set Element Number", GH_ParamAccess.list);
      pManager.AddParameter(new GsaProperty3dParameter(), "3D Property", "PV",
        "Change 3D Property. Input either a GSA 3D Property or an Integer to use a Property already defined in model",
        GH_ParamAccess.list);
      pManager.AddIntegerParameter("Element3d Group", "Gr", "Set Element Group",
        GH_ParamAccess.list);
      pManager.AddTextParameter("Element3d Name", "Na", "Set Name of Element", GH_ParamAccess.list);
      pManager.AddColourParameter("Element3d Colour", "Co", "Set Element Colour",
        GH_ParamAccess.list);
      pManager.AddBooleanParameter("Dummy Element", "Dm", "Set Element to Dummy",
        GH_ParamAccess.list);

      for (int i = 1; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }

      pManager.HideParameter(0);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaElement3dParameter(), GsaElement3dGoo.Name,
        GsaElement3dGoo.NickName, GsaElement3dGoo.Description + " with applied changes.",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Number", "ID", "Get Element Number", GH_ParamAccess.list);
      pManager.AddMeshParameter("Analysis Mesh", "M",
        "Get Analysis Mesh. " + Environment.NewLine
        + "This will export a list of solid meshes representing each 3D element."
        + Environment.NewLine
        + "To get a combined mesh connect a GSA Element 3D to normal Mesh Parameter component to convert on the fly",
        GH_ParamAccess.item);
      pManager.HideParameter(2);
      pManager.AddParameter(new GsaProperty3dParameter(), "3D Property", "PV",
        "Get 3D Property. Either a GSA 3D Property or an Integer representing a Property already defined in model",
        GH_ParamAccess.list);
      pManager.AddIntegerParameter("Group", "Gr", "Get Element Group", GH_ParamAccess.list);
      pManager.AddTextParameter("Element Type", "eT",
        "Get Element 3D Type." + Environment.NewLine
        + "Type can not be set; it is either Tetra4, Pyramid5, Wedge6 or Brick8",
        GH_ParamAccess.list);
      pManager.AddTextParameter("Name", "Na", "Set Element Name", GH_ParamAccess.list);
      pManager.AddColourParameter("Colour", "Co", "Get Element Colour", GH_ParamAccess.list);
      pManager.AddBooleanParameter("Dummy Element", "Dm", "Get if Element is Dummy",
        GH_ParamAccess.list);
      pManager.AddIntegerParameter("Parent Members", "pM",
        "Get Parent Member IDs in Model that Element was created from", GH_ParamAccess.list);
      pManager.AddIntegerParameter("Topology", "Tp",
        "Get the Element's original topology list referencing node IDs in Model that Element was created from",
        GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var elem = new GsaElement3d();

      GsaElement3dGoo element3dGoo = null;
      if (da.GetData(0, ref element3dGoo)) {
        elem = element3dGoo.Value.Clone();
      }

      // 1 ID
      var ghIds = new List<GH_Integer>();
      if (da.GetDataList(1, ghIds)) {
        if (ghIds.Count != elem.ApiElements.Count) {
          this.AddRuntimeError("ID input must be a list matching the number of elements " +
            $"({elem.ApiElements.Count})");
          return;
        }
        elem.Ids = ghIds.Select(x => x.Value).ToList();
      }

      // 2 property
      var prop3dGoos = new List<GsaProperty3dGoo>();
      if (da.GetDataList(2, prop3dGoos)) {
        if (prop3dGoos.Count == 1) {
          elem.Prop3ds = new List<GsaProperty3d>();
          for (int i = 0; i < elem.ApiElements.Count; i++) {
            elem.Prop3ds.Add(prop3dGoos[0].Value);
          }
        } else {
          if (prop3dGoos.Count != elem.ApiElements.Count) {
            this.AddRuntimeWarning("PA input must either be a single Prop2d or a" +
              $"{Environment.NewLine}list matching the number of elements ({elem.ApiElements.Count})");
          }
          elem.Prop3ds = prop3dGoos.Select(x => x.Value).ToList();
        }
      }

      // 3 Group
      var ghGrps = new List<GH_Integer>();
      if (da.GetDataList(3, ghGrps)) {
        if (ghGrps.Count == 1) {
          elem.Groups = new List<int>() { 
            ghGrps[0].Value
          };
        } else {
          if (ghGrps.Count != elem.ApiElements.Count) {
            this.AddRuntimeWarning("Gr input must either be a single Group ID or a" +
              $"{Environment.NewLine}list matching the number of elements ({elem.ApiElements.Count})");
          }
          elem.Groups = ghGrps.Select(x => x.Value).ToList();
        }
      }

      // 4 name
      var ghnm = new List<GH_String>();
      if (da.GetDataList(4, ghnm)) {
        if (ghnm.Count == 1) {
          elem.Names = new List<string>() {
            ghnm[0].Value
          };
        } else {
          if (ghnm.Count != elem.ApiElements.Count) {
            this.AddRuntimeWarning("Nm input must either be a single Text string or a" +
              $"{Environment.NewLine}list matching the number of elements ({elem.ApiElements.Count})");
          }
          elem.Names = ghnm.Select(x => x.Value).ToList();
        }
      }

      // 5 Colour
      var ghcols = new List<GH_Colour>();
      if (da.GetDataList(5, ghcols)) {
        if (ghcols.Count == 1) {
          elem.Colours = new List<Color>() {
            ghcols[0].Value
          };
        } else {
          if (ghcols.Count != elem.ApiElements.Count) {
            this.AddRuntimeWarning("Co input must either be a single Colour or a" +
              $"{Environment.NewLine}list matching the number of elements ({elem.ApiElements.Count})");
          }
          elem.Colours = ghcols.Select(x => x.Value).ToList();
        }
      }

      // 6 Dummy
      var ghdummies = new List<GH_Boolean>();
      if (da.GetDataList(6, ghdummies)) {
        if (ghdummies.Count == 1) {
          elem.IsDummies = new List<bool>() {
            ghdummies[0].Value
          };
        } else {
          if (ghdummies.Count != elem.ApiElements.Count) {
            this.AddRuntimeWarning("Dm input must either be a single Boolean or a" +
              $"{Environment.NewLine}list matching the number of elements ({elem.ApiElements.Count})");
          }
          elem.IsDummies = ghdummies.Select(x => x.Value).ToList();
        }
      }

      da.SetData(0, new GsaElement3dGoo(elem));
      da.SetDataList(1, elem.Ids);
      da.SetDataList(2, elem.DisplayMesh.ExplodeAtUnweldedEdges());
      da.SetDataList(3,
        new List<GsaProperty3dGoo>(elem.Prop3ds.Select(x => new GsaProperty3dGoo(x))));
      da.SetDataList(4, elem.Groups);
      da.SetDataList(5, elem.Types);
      da.SetDataList(6, elem.Names);
      da.SetDataList(7, elem.Colours);
      da.SetDataList(8, elem.IsDummies);
      da.SetDataList(9, elem.ParentMembers);
      da.SetDataTree(10, elem.TopologyIDs);
    }
  }
}
