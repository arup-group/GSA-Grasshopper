using System;
using System.Drawing;
using System.Linq;

using GH_IO.Serialization;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;

using OasysUnits;

using Rhino.Geometry;

using AngleUnit = OasysUnits.Units.AngleUnit;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a 1D Element
  /// </summary>
  public class Edit1dElement : Section3dPreviewComponent {
    public override Guid ComponentGuid => new Guid("e0bae222-f7ac-4440-a146-2df8b66b2389");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Edit1dElement;
    private AngleUnit _angleUnit = AngleUnit.Radian;

    public Edit1dElement() : base("Edit 1D Element", "Elem1dEdit", "Modify GSA 1D Element",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    protected override void BeforeSolveInstance() {
      base.BeforeSolveInstance();
      if (Params.Input[9] is Param_Number angleParameter) {
        _angleUnit = angleParameter.UseDegrees ? AngleUnit.Degree : AngleUnit.Radian;
      }
    }

    public override bool Read(GH_IReader reader) {
      bool flag = base.Read(reader);
      if (Params.Input[3].Name == new GsaSectionParameter().Name) {
        Params.ReplaceInputParameter(new GsaPropertyParameter(), 3, true);
        Params.ReplaceOutputParameter(new GsaPropertyParameter(), 3);
      }
      Params.UpdateReleaseBool6Parameter();
      return flag;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaElement1dParameter(), GsaElement1dGoo.Name,
        GsaElement1dGoo.NickName,
        GsaElement1dGoo.Description + " to get or set information for. Leave blank to create a new "
        + GsaElement1dGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Number", "ID",
        "Set Element Number. If ID is set it will replace any existing 1D Element in the model",
        GH_ParamAccess.item);
      pManager.AddLineParameter("Line", "L", "Reposition Element Line", GH_ParamAccess.item);
      pManager.AddParameter(new GsaPropertyParameter(), "Section", "PB", "Set new Section or Spring Property",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Group", "Gr", "Set Element Group", GH_ParamAccess.item);
      pManager.AddTextParameter("Type", "eT",
        "Set Element Type" + Environment.NewLine + "Accepted inputs are:" + Environment.NewLine
        + "1: Bar" + Environment.NewLine + "2: Beam" + Environment.NewLine + "3: Spring"
        + Environment.NewLine + "9: Link" + Environment.NewLine + "10: Cable" + Environment.NewLine
        + "19: Spacer" + Environment.NewLine + "20: Strut" + Environment.NewLine + "21: Tie"
        + Environment.NewLine + "23: Rod" + Environment.NewLine + "24: Damper",
        GH_ParamAccess.item);

      pManager.AddParameter(new GsaOffsetParameter(), "Offset", "Of", "Set Element Offset",
        GH_ParamAccess.item);

      pManager.AddParameter(new GsaReleaseParameter(), "Start release", "⭰",
        "Set Release (Bool6) at Start of Element", GH_ParamAccess.item);
      pManager.AddParameter(new GsaReleaseParameter(), "End release", "⭲",
        "Set Release (Bool6) at End of Element", GH_ParamAccess.item);

      pManager.AddAngleParameter("Orientation Angle", "⭮A", "Set Element Orientation Angle",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaNodeParameter(), "Orientation Node", "⭮N",
        "Set Element Orientation Node", GH_ParamAccess.item);

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
      pManager.AddParameter(new GsaPropertyParameter());
      pManager.AddIntegerParameter("Group", "Gr", "Get Element Group", GH_ParamAccess.item);
      pManager.AddTextParameter("Type", "eT", "Get Element Type", GH_ParamAccess.item);

      pManager.AddParameter(new GsaOffsetParameter(), "Offset", "Of", "Get Element Offset",
        GH_ParamAccess.item);

      pManager.AddParameter(new GsaReleaseParameter(), "Start release", "⭰",
        "Get Release (Bool6) at Start of Element", GH_ParamAccess.item);
      pManager.AddParameter(new GsaReleaseParameter(), "End release", "⭲",
        "Get Release (Bool6) at End of Element", GH_ParamAccess.item);

      pManager.AddNumberParameter("Orientation Angle", "⭮A", "Get Element Orientation Angle",
        GH_ParamAccess.item);
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
        GH_ParamAccess.tree);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var elem = new GsaElement1d();

      GsaElement1dGoo element1dGoo = null;
      if (da.GetData(0, ref element1dGoo)) {
        elem = new GsaElement1d(element1dGoo.Value);
      }

      var ghString = new GH_String();
      if (da.GetData(5, ref ghString)) {
        if (GH_Convert.ToInt32(ghString, out int typeInt, GH_Conversion.Both)) {
          elem.ApiElement.Type = (ElementType)typeInt;
        } else {
          elem.ApiElement.Type = Mappings.GetElementType(ghString.Value);
        }
      }

      GsaPropertyGoo sectionGoo = null;
      if (da.GetData(3, ref sectionGoo)) {
        switch (sectionGoo.Value) {
          case GsaSection section:
            if (section.IsReferencedById && elem.ApiElement.Type == ElementType.SPRING) {
              elem.Section = null;
              elem.SpringProperty = new GsaSpringProperty(section.Id);
            } else {
              if (elem.ApiElement.Type == ElementType.SPRING) {
                this.AddRuntimeError("PB input must be a SpringProperty");
                return;
              }
              elem.Section = section;
              elem.SpringProperty = null;
            }

            break;

          case GsaSpringProperty springProperty:
            elem.ApiElement.Type = ElementType.SPRING;
            this.AddRuntimeRemark("ElementType changed to Spring");
            elem.Section = null;
            elem.SpringProperty = springProperty;
            break;
        }
      }

      int id = 0;
      if (da.GetData(1, ref id)) {
        elem.Id = id;
      }

      GH_Line ghcrv = null;
      if (da.GetData(2, ref ghcrv)) {
        elem.Line = new LineCurve(ghcrv.Value);
      }

      int group = 0;
      if (da.GetData(4, ref group)) {
        elem.ApiElement.Group = group;
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

      double angle = 0;
      if (da.GetData(9, ref angle)) {
        elem.OrientationAngle = new Angle(angle, _angleUnit);
      }

      GsaNodeGoo nodeGoo = null;
      if (da.GetData(10, ref nodeGoo)) {
        elem.OrientationNode = nodeGoo.Value;
      }

      string name = string.Empty;
      if (da.GetData(11, ref name)) {
        elem.ApiElement.Name = name;
      }

      Color colour = Color.Empty;
      if (da.GetData(12, ref colour)) {
        elem.ApiElement.Colour = colour;
      }

      bool dummy = false;
      if (da.GetData(13, ref dummy)) {
        elem.ApiElement.IsDummy = dummy;
      }

      if (Preview3dSection || elem.Section3dPreview != null) {
        elem.CreateSection3dPreview();
      }

      elem.UpdateReleasesPreview();

      da.SetData(0, new GsaElement1dGoo(elem));
      da.SetData(1, elem.Id);
      da.SetData(2, new GH_Line(elem.Line.Line));
      if (elem.ApiElement.Type == ElementType.SPRING) {
        da.SetData(3, new GsaSpringPropertyGoo(elem.SpringProperty));
      } else {
        da.SetData(3, new GsaSectionGoo(elem.Section));
      }
      da.SetData(4, elem.ApiElement.Group);
      da.SetData(5,
        Mappings._elementTypeMapping.FirstOrDefault(x => x.Value == elem.ApiElement.Type).Key);
      da.SetData(6, new GsaOffsetGoo(elem.Offset));
      da.SetData(7, new GsaBool6Goo(elem.ReleaseStart));
      da.SetData(8, new GsaBool6Goo(elem.ReleaseEnd));
      da.SetData(9, elem.OrientationAngle.Radians);
      da.SetData(10, new GsaNodeGoo(elem.OrientationNode));
      da.SetData(11, elem.ApiElement.Name);
      da.SetData(12, (Color)elem.ApiElement.Colour);
      da.SetData(13, elem.ApiElement.IsDummy);

      da.SetData(14, elem.ApiElement.ParentMember.Member);

      var topo = new DataTree<int>();
      topo.AddRange(elem.ApiElement.Topology, new GH_Path(elem.Id));
      da.SetDataTree(15, topo);
    }
  }
}
