using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysUnits;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a 2D Element
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class EditElement2d_OBSOLETE : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("e9611aa7-88c1-4b5b-83d6-d9629e21ad8a");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Edit2dElement;

    public EditElement2d_OBSOLETE() : base("Edit 2D Element", "Elem2dEdit", "Modify GSA 2D Element",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaElement2dParameter(), GsaElement2dGoo.Name,
        GsaElement2dGoo.NickName,
        GsaElement2dGoo.Description + " to get or set information for." + GsaElement2dGoo.Name,
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Element2d Number", "ID",
        "Set Element Number. If ID is set it will replace any existing 2D Element in the model",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaProperty2dParameter(), "2D Property", "PA",
        "Change 2D Property. Input either a GSA 2D Property or an Integer to use a Property already defined in model",
        GH_ParamAccess.list);
      pManager.AddIntegerParameter("Element2d Group", "Gr", "Set Element Group",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaOffsetParameter(), "Offset", "Of", "Set Element Offset",
        GH_ParamAccess.list);
      pManager.AddTextParameter("Element2d Name", "Na", "Set Name of Element", GH_ParamAccess.list);
      pManager.AddColourParameter("Element2d Colour", "Co", "Set Element Colour",
        GH_ParamAccess.list);
      pManager.AddBooleanParameter("Dummy Element", "Dm", "Set Element to Dummy",
        GH_ParamAccess.list);

      for (int i = 1; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }

      pManager.HideParameter(0);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaElement2dParameter(), GsaElement2dGoo.Name,
        GsaElement2dGoo.NickName, GsaElement2dGoo.Description + " with applied changes.",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Number", "ID", "Get Element Number", GH_ParamAccess.list);
      pManager.AddMeshParameter("Analysis Mesh", "M", "Get Analysis Mesh", GH_ParamAccess.item);
      pManager.HideParameter(2);
      pManager.AddParameter(new GsaProperty2dParameter(), "2D Property", "PA",
        "Get 2D Property. Input either a GSA 2D Property or an Integer to use a Property already defined in model",
        GH_ParamAccess.list);
      pManager.AddIntegerParameter("Group", "Gr", "Get Element Group", GH_ParamAccess.list);
      pManager.AddTextParameter("Element Type", "eT",
        "Get Element 2D Type." + Environment.NewLine
        + "Type can not be set; it is either Tri3 or Quad4" + Environment.NewLine
        + "depending on Rhino/Grasshopper mesh face type", GH_ParamAccess.list);
      pManager.AddParameter(new GsaOffsetParameter(), "Offset", "Of", "Get Element Offset",
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
      var elem = new GsaElement2d();

      GsaElement2dGoo element2dGoo = null;
      if (da.GetData(0, ref element2dGoo)) {
        elem = new GsaElement2d(element2dGoo.Value);
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
      var prop2dGoos = new List<GsaProperty2dGoo>();
      if (da.GetDataList(2, prop2dGoos)) {
        if (prop2dGoos.Count == 1) {
          elem.Prop2ds = new List<GsaProperty2d>();
          for (int i = 0; i < elem.ApiElements.Count; i++) {
            elem.Prop2ds.Add(prop2dGoos[0].Value);
          }
        } else {
          if (prop2dGoos.Count != elem.ApiElements.Count) {
            this.AddRuntimeWarning("PA input must either be a single Prop2d or a" +
              $"{Environment.NewLine}list matching the number of elements ({elem.ApiElements.Count})");
          }
          elem.Prop2ds = prop2dGoos.Select(x => x.Value).ToList();
        }
      }

      // 3 Group
      var ghGrps = new List<GH_Integer>();
      if (da.GetDataList(3, ghGrps)) {
        elem.ApiElements.SetMembers(ghGrps.Select(x => x.Value).ToList());
      }

      // 4 offset
      var offsetGoos = new List<GsaOffsetGoo>();
      if (da.GetDataList(4, offsetGoos)) {
        elem.ApiElements.SetMembers(offsetGoos.Select(x => x.Value).ToList());
      }

      // 5 name
      var ghnm = new List<GH_String>();
      if (da.GetDataList(5, ghnm)) {
        elem.ApiElements.SetMembers(ghnm.Select(x => x.Value).ToList());
      }

      // 6 Colour
      var ghcols = new List<GH_Colour>();
      if (da.GetDataList(6, ghcols)) {
        elem.ApiElements.SetMembers(ghcols.Select(x => x.Value).ToList());
      }

      // 7 Dummy
      var ghdummies = new List<GH_Boolean>();
      if (da.GetDataList(7, ghdummies)) {
        elem.ApiElements.SetMembers(ghdummies.Select(x => x.Value).ToList());
      }

      da.SetData(0, new GsaElement2dGoo(elem));
      da.SetDataList(1, elem.Ids);
      da.SetData(2, elem.Mesh);
      da.SetDataList(3,
        new List<GsaProperty2dGoo>(elem.Prop2ds.ConvertAll(prop2d => new GsaProperty2dGoo(prop2d))));
      da.SetDataList(4, elem.ApiElements.Select(x => x.Group));
      da.SetDataList(5, elem.ApiElements.Select(x => x.Type));
      da.SetDataList(6,
        new List<GsaOffsetGoo>(elem.Offsets.ConvertAll(offset => new GsaOffsetGoo(offset))));
      da.SetDataList(7, elem.ApiElements.Select(x => x.Name));
      da.SetDataList(8, elem.ApiElements.Select(x => (Color)x.Colour));
      da.SetDataList(9, elem.ApiElements.Select(x => x.IsDummy));
      try {
        da.SetData(10, elem.ApiElements[0].ParentMember.Member);
      } catch (Exception) { }
      da.SetDataTree(11, elem.GetTopologyIDs());
    }
  }
}
