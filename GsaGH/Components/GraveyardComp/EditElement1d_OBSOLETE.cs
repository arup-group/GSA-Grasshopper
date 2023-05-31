using System;
using System.Collections.ObjectModel;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;
using Line = Rhino.Geometry.Line;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a 1D Element
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class EditElement1d_OBSOLETE : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("aeb5f765-8721-41fc-a1b4-cfd78e05ce67");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditElem1d;

    public EditElement1d_OBSOLETE() : base("Edit 1D Element", "Elem1dEdit", "Modify GSA 1D Element",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaElement1dParameter(), GsaElement1dGoo.Name,
        GsaElement1dGoo.NickName,
        GsaElement1dGoo.Description + " to get or set information for. Leave blank to create a new "
        + GsaElement1dGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Number", "ID",
        "Set Element Number. If ID is set it will replace any existing 1D Element in the model",
        GH_ParamAccess.item);
      pManager.AddLineParameter("Line", "L", "Reposition Element Line", GH_ParamAccess.item);
      pManager.AddParameter(new GsaSectionParameter(), "Section", "PB", "Set new Section Property",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Group", "Gr", "Set Element Group", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Type", "eT",
        "Set Element Type" + Environment.NewLine + "Accepted inputs are:" + Environment.NewLine
        + "1: Bar" + Environment.NewLine + "2: Beam" + Environment.NewLine + "3: Spring"
        + Environment.NewLine + "9: Link" + Environment.NewLine + "10: Cable" + Environment.NewLine
        + "19: Spacer" + Environment.NewLine + "20: Strut" + Environment.NewLine + "21: Tie"
        + Environment.NewLine + "23: Rod" + Environment.NewLine + "24: Damper",
        GH_ParamAccess.item);

      pManager.AddParameter(new GsaOffsetParameter(), "Offset", "Of", "Set Element Offset",
        GH_ParamAccess.item);

      pManager.AddParameter(new GsaBool6Parameter(), "Start release", "⭰",
        "Set Release (Bool6) at Start of Element", GH_ParamAccess.item);
      pManager.AddParameter(new GsaBool6Parameter(), "End release", "⭲",
        "Set Release (Bool6) at End of Element", GH_ParamAccess.item);

      pManager.AddNumberParameter("Orientation Angle", "⭮A",
        "Set Element Orientation Angle in degrees", GH_ParamAccess.item);
      pManager.AddGenericParameter("Orientation Node", "⭮N", "Set Element Orientation Node",
        GH_ParamAccess.item);

      pManager.AddTextParameter("Name", "Na", "Set Element Name", GH_ParamAccess.item);
      pManager.AddColourParameter("Colour", "Co", "Set Element Colour", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Element", "Dm", "Set Element to Dummy",
        GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }

      pManager.HideParameter(0);
      pManager.HideParameter(2);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaElement1dParameter(), GsaElement1dGoo.Name,
        GsaElement1dGoo.NickName, GsaElement1dGoo.Description + " with applied changes.",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Number", "ID",
        "Get Element Number. If ID is set it will replace any existing 1D Element in the model",
        GH_ParamAccess.item);
      pManager.AddLineParameter("Line", "L", "Element Line", GH_ParamAccess.item);
      pManager.HideParameter(2);
      pManager.AddParameter(new GsaSectionParameter(), "Section", "PB", "Get Section Property",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Group", "Gr", "Get Element Group", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Type", "eT", "Get Element Type", GH_ParamAccess.item);

      pManager.AddParameter(new GsaOffsetParameter(), "Offset", "Of", "Get Element Offset",
        GH_ParamAccess.item);

      pManager.AddParameter(new GsaBool6Parameter(), "Start release", "⭰",
        "Get Release (Bool6) at Start of Element", GH_ParamAccess.item);
      pManager.AddParameter(new GsaBool6Parameter(), "End release", "⭲",
        "Get Release (Bool6) at End of Element", GH_ParamAccess.item);

      pManager.AddNumberParameter("Orientation Angle", "⭮A",
        "Get Element Orientation Angle in degrees", GH_ParamAccess.item);
      pManager.AddGenericParameter("Orientation Node", "⭮N", "Get Element Orientation Node",
        GH_ParamAccess.item);

      pManager.AddTextParameter("Name", "Na", "Get Element Name", GH_ParamAccess.item);
      pManager.AddColourParameter("Colour", "Co", "Get Element Colour", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Element", "Dm", "Get if Element is Dummy",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Parent Members", "pM",
        "Get Parent Member IDs in Model that Element was created from", GH_ParamAccess.list);
      pManager.AddIntegerParameter("Topology", "Tp",
        "Get the Element's original topology list referencing node IDs in Model that Element was created from",
        GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var elem = new GsaElement1d();

      GsaElement1dGoo element1dGoo = null;
      if (da.GetData(0, ref element1dGoo)) {
        elem = element1dGoo.Value.Duplicate(true);
      }

      var ghId = new GH_Integer();
      if (da.GetData(1, ref ghId)) {
        if (GH_Convert.ToInt32(ghId, out int id, GH_Conversion.Both)) {
          elem.Id = id;
        }
      }

      var ghLine = new GH_Line();
      if (da.GetData(2, ref ghLine)) {
        var curve = new Line();
        if (GH_Convert.ToLine(ghLine, ref curve, GH_Conversion.Both)) {
          var ln = new LineCurve(curve);
          elem.Line = ln;
        }
      }

      GsaSectionGoo sectionGoo = null;
      if (da.GetData(3, ref sectionGoo)) {
        elem.Section = sectionGoo.Value;
      }

      var ghgrp = new GH_Integer();
      if (da.GetData(4, ref ghgrp)) {
        if (GH_Convert.ToInt32(ghgrp, out int grp, GH_Conversion.Both)) {
          elem.Group = grp;
        }
      }

      var ghinteg = new GH_Integer();
      if (da.GetData(5, ref ghinteg)) {
        if (GH_Convert.ToInt32(ghinteg, out int type, GH_Conversion.Both)) {
          elem.Type = (ElementType)type;
        }
      }

      GsaOffsetGoo offset = null;
      if (da.GetData(6, ref offset)) {
        elem.Offset = offset.Value;
      }

      GsaBool6Goo start = null;
      if (da.GetData(7, ref start)) {
        elem.ReleaseStart = start.Value;
      }

      GsaBool6Goo end = null;
      if (da.GetData(8, ref end)) {
        elem.ReleaseEnd = end.Value;
      }

      var ghangle = new GH_Number();
      if (da.GetData(9, ref ghangle)) {
        if (GH_Convert.ToDouble(ghangle, out double angle, GH_Conversion.Both)) {
          elem.OrientationAngle = new Angle(angle, AngleUnit.Degree);
        }
      }

      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(10, ref ghTyp)) {
        if (ghTyp.Value is GsaNodeGoo nodeGoo) {
          elem.OrientationNode = nodeGoo.Value;
        } else {
          this.AddRuntimeWarning("Unable to convert Orientation Node input to GsaNode");
        }
      }

      var ghnm = new GH_String();
      if (da.GetData(11, ref ghnm)) {
        if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both)) {
          elem.Name = name;
        }
      }

      var ghcol = new GH_Colour();
      if (da.GetData(12, ref ghcol)) {
        if (GH_Convert.ToColor(ghcol, out Color col, GH_Conversion.Both)) {
          elem.Colour = col;
        }
      }

      var ghdum = new GH_Boolean();
      if (da.GetData(13, ref ghdum)) {
        if (GH_Convert.ToBoolean(ghdum, out bool dum, GH_Conversion.Both)) {
          elem.IsDummy = dum;
        }
      }

      da.SetData(0, new GsaElement1dGoo(elem));
      da.SetData(1, elem.Id);
      da.SetData(2, elem.Line.Line);
      da.SetData(3, new GsaSectionGoo(elem.Section));
      da.SetData(4, elem.Group);
      da.SetData(5, elem.Type);
      da.SetData(6, new GsaOffsetGoo(elem.Offset));
      da.SetData(7, new GsaBool6Goo(elem.ReleaseStart));
      da.SetData(8, new GsaBool6Goo(elem.ReleaseEnd));
      da.SetData(9, elem.OrientationAngle.Degrees);
      da.SetData(10, new GsaNodeGoo(elem.OrientationNode));
      da.SetData(11, elem.Name);
      da.SetData(12, elem.Colour);
      da.SetData(13, elem.IsDummy);

      try {
        da.SetData(14, elem.ParentMember);
      } catch (Exception) {
        // ignored
      }

      da.SetDataList(15, new Collection<int>(elem.ApiElement.Topology));
    }
  }
}
