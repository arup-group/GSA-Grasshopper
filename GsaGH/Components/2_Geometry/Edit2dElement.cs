using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a 2D Element
  /// </summary>
  public class Edit2dElement : Section3dPreviewComponent {
    public override Guid ComponentGuid => new Guid("0b4ecb0e-ef8f-4b42-bcf2-de940594fada");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Edit2dElement;
    private AngleUnit _angleUnit = AngleUnit.Radian;

    public Edit2dElement() : base("Edit 2D Element", "Elem2dEdit", "Modify GSA 2D Element",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    protected override void BeforeSolveInstance() {
      base.BeforeSolveInstance();
      if (Params.Input[5] is Param_Number angleParameter) {
        _angleUnit = angleParameter.UseDegrees ? AngleUnit.Degree : AngleUnit.Radian;
      }
    }

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
      pManager.AddAngleParameter("Orientation Angle", "⭮A", "Set Element Orientation Angle",
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
      pManager.AddNumberParameter("Orientation Angle", "⭮A",
        "Get Element Orientation Angle in radians", GH_ParamAccess.list);
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

      // #### inputs ####
      // no good way of updating location of mesh on the fly //
      // suggest users re-create from scratch //

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

      // 5 orientation angle
      var ghangles = new List<GH_Number>();
      if (da.GetDataList(5, ghangles)) {
        elem.ApiElements.SetMembers(ghangles.Select(x => new Angle(x.Value, _angleUnit)).ToList());
      }

      // 6 name
      var ghnm = new List<GH_String>();
      if (da.GetDataList(6, ghnm)) {
        elem.ApiElements.SetMembers(ghnm.Select(x => x.Value).ToList());
      }

      // 7 Colour
      var ghcols = new List<GH_Colour>();
      if (da.GetDataList(7, ghcols)) {
        elem.ApiElements.SetMembers(ghcols.Select(x => x.Value).ToList());
        elem.UpdateMeshColours();
      }

      // 8 Dummy
      var ghdummies = new List<GH_Boolean>();
      if (da.GetDataList(8, ghdummies)) {
        elem.ApiElements.SetMembers(ghdummies.Select(x => x.Value).ToList());
      }

      if (Preview3dSection || elem.Section3dPreview != null) {
        elem.CreateSection3dPreview();
      }

      // #### outputs ####
      da.SetData(0, new GsaElement2dGoo(elem));
      da.SetDataList(1, elem.Ids);
      da.SetData(2, elem.Mesh);
      da.SetDataList(3, elem.Prop2ds == null ? null
        : new List<GsaProperty2dGoo>(elem.Prop2ds.Select(x => new GsaProperty2dGoo(x))));
      da.SetDataList(4, elem.ApiElements.Select(x => x.Group));
      da.SetDataList(5, elem.ApiElements.Select(x => x.TypeAsString()));
      da.SetDataList(6,
        new List<GsaOffsetGoo>(elem.Offsets.Select(x => new GsaOffsetGoo(x))));
      da.SetDataList(7, elem.OrientationAngles.Select(x => x.Radians));
      da.SetDataList(8, elem.ApiElements.Select(x => x.Name));
      da.SetDataList(9, elem.ApiElements.Select(x => (Color)x.Colour));
      da.SetDataList(10, elem.ApiElements.Select(x => x.IsDummy));
      try {
        da.SetDataList(11, elem.ApiElements.Select(x => x.ParentMember.Member).ToList());
      } catch (Exception) { }
      da.SetDataTree(12, elem.GetTopologyIDs());
    }
  }
}
