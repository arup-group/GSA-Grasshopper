using System;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaAPI;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  /// Component to edit a 1D Element
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class EditElement1d2_OBSOLETE : GH_OasysComponent {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("5aa4635c-b60e-4812-ab45-6af9437255e4");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.EditElem1d;

    public EditElement1d2_OBSOLETE() : base("Edit 1D Element",
      "Elem1dEdit",
      "Modify GSA 1D Element",
      CategoryName.Name(),
      SubCategoryName.Cat2()) { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaElement1dParameter(), GsaElement1dGoo.Name, GsaElement1dGoo.NickName, GsaElement1dGoo.Description + " to get or set information for. Leave blank to create a new " + GsaElement1dGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Number", "ID", "Set Element Number. If ID is set it will replace any existing 1D Element in the model", GH_ParamAccess.item);
      pManager.AddLineParameter("Line", "L", "Reposition Element Line", GH_ParamAccess.item);
      pManager.AddParameter(new GsaSectionParameter(), "Section", "PB", "Set new Section Property", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Group", "Gr", "Set Element Group", GH_ParamAccess.item);
      pManager.AddTextParameter("Type", "eT", "Set Element Type" + Environment.NewLine +
          "Accepted inputs are:" + Environment.NewLine +
          "1: Bar" + Environment.NewLine +
          "2: Beam" + Environment.NewLine +
          "3: Spring" + Environment.NewLine +
          "9: Link" + Environment.NewLine +
          "10: Cable" + Environment.NewLine +
          "19: Spacer" + Environment.NewLine +
          "20: Strut" + Environment.NewLine +
          "21: Tie" + Environment.NewLine +
          "23: Rod" + Environment.NewLine +
          "24: Damper", GH_ParamAccess.item);

      pManager.AddParameter(new GsaOffsetParameter(), "Offset", "Of", "Set Element Offset", GH_ParamAccess.item);

      pManager.AddParameter(new GsaBool6Parameter(), "Start release", "⭰", "Set Release (Bool6) at Start of Element", GH_ParamAccess.item);
      pManager.AddParameter(new GsaBool6Parameter(), "End release", "⭲", "Set Release (Bool6) at End of Element", GH_ParamAccess.item);

      pManager.AddAngleParameter("Orientation Angle", "⭮A", "Set Element Orientation Angle", GH_ParamAccess.item);
      pManager.AddParameter(new GsaNodeParameter(), "Orientation Node", "⭮N", "Set Element Orientation Node", GH_ParamAccess.item);

      pManager.AddTextParameter("Name", "Na", "Set Element Name", GH_ParamAccess.item);
      pManager.AddColourParameter("Colour", "Co", "Set Element Colour", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Element", "Dm", "Set Element to Dummy", GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
      pManager.HideParameter(0);
      pManager.HideParameter(2);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaElement1dParameter(), GsaElement1dGoo.Name, GsaElement1dGoo.NickName, GsaElement1dGoo.Description + " with applied changes.", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Number", "ID", "Get Element Number. If ID is set it will replace any existing 1D Element in the model", GH_ParamAccess.item);
      pManager.AddLineParameter("Line", "L", "Element Line", GH_ParamAccess.item);
      pManager.HideParameter(2);
      pManager.AddParameter(new GsaSectionParameter(), "Section", "PB", "Get Section Property", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Group", "Gr", "Get Element Group", GH_ParamAccess.item);
      pManager.AddTextParameter("Type", "eT", "Get Element Type", GH_ParamAccess.item);

      pManager.AddParameter(new GsaOffsetParameter(), "Offset", "Of", "Get Element Offset", GH_ParamAccess.item);

      pManager.AddParameter(new GsaBool6Parameter(), "Start release", "⭰", "Get Release (Bool6) at Start of Element", GH_ParamAccess.item);
      pManager.AddParameter(new GsaBool6Parameter(), "End release", "⭲", "Get Release (Bool6) at End of Element", GH_ParamAccess.item);

      pManager.AddNumberParameter("Orientation Angle", "⭮A", "Get Element Orientation Angle", GH_ParamAccess.item);
      pManager.AddGenericParameter("Orientation Node", "⭮N", "Get Element Orientation Node", GH_ParamAccess.item);

      pManager.AddTextParameter("Name", "Na", "Get Element Name", GH_ParamAccess.item);
      pManager.AddColourParameter("Colour", "Co", "Get Element Colour", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Element", "Dm", "Get if Element is Dummy", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Parent Members", "pM", "Get Parent Member IDs in Model that Element was created from", GH_ParamAccess.list);
      pManager.AddIntegerParameter("Topology", "Tp", "Get the Element's original topology list referencing node IDs in Model that Element was created from", GH_ParamAccess.tree);
    }
    #endregion

    protected override void BeforeSolveInstance() {
      base.BeforeSolveInstance();
      if (!(Params.Input[9] is Param_Number angleParameter)) {
        return;
      }

      _angleUnit = angleParameter.UseDegrees
        ? AngleUnit.Degree
        : AngleUnit.Radian;
    }

    private AngleUnit _angleUnit = AngleUnit.Radian;

    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaElement1d = new GsaElement1d();
      var elem = new GsaElement1d();
      if (da.GetData(0, ref gsaElement1d)) {
        if (gsaElement1d == null) {
          this.AddRuntimeWarning("Element1D input is null");
        }
        elem = gsaElement1d.Duplicate();
      }

      if (elem == null) {
        return;
      }

      var ghId = new GH_Integer();
      if (da.GetData(1, ref ghId)) {
        if (GH_Convert.ToInt32(ghId, out int id, GH_Conversion.Both))
          elem.Id = id;
      }

      var ghLine = new GH_Line();
      if (da.GetData(2, ref ghLine)) {
        var line = new Line();
        if (GH_Convert.ToLine(ghLine, ref line, GH_Conversion.Both)) {
          var ln = new LineCurve(line);
          elem.Line = ln;
        }
      }

      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(3, ref ghTyp)) {
        var section = new GsaSection();
        if (ghTyp.Value is GsaSectionGoo) {
          ghTyp.CastTo(ref section);
        }
        else {
          if (GH_Convert.ToInt32(ghTyp.Value, out int idd, GH_Conversion.Both)) {
            section = new GsaSection(idd);
          }
          else {
            this.AddRuntimeError("Unable to convert PB input to a Section Property of reference integer");
            return;
          }
        }
        elem.Section = section;
      }

      var ghGroup = new GH_Integer();
      if (da.GetData(4, ref ghGroup)) {
        if (GH_Convert.ToInt32(ghGroup, out int grp, GH_Conversion.Both))
          elem.Group = grp; //elem.Element.Group = grp;
      }

      var ghString = new GH_String();
      if (da.GetData(5, ref ghString)) {
        if (GH_Convert.ToInt32(ghString, out int typeInt, GH_Conversion.Both))
          elem.Type = (ElementType)typeInt;
        else if (GH_Convert.ToString(ghString, out string typestring, GH_Conversion.Both)) {
          try {
            elem.Type = Mappings.GetElementType(typestring);
          }
          catch (ArgumentException) {
            this.AddRuntimeError("Unable to change Element Type");
          }
        }
      }

      var offset = new GsaOffset();
      if (da.GetData(6, ref offset)) {
        elem.Offset = offset;
      }

      var start = new GsaBool6();
      if (da.GetData(7, ref start)) {
        elem.ReleaseStart = start;
      }

      var end = new GsaBool6();
      if (da.GetData(8, ref end)) {
        elem.ReleaseEnd = end;
      }

      var ghAngle = new GH_Number();
      if (da.GetData(9, ref ghAngle)) {
        if (GH_Convert.ToDouble(ghAngle, out double angle, GH_Conversion.Both))
          elem.OrientationAngle = new Angle(angle, _angleUnit);
      }

      ghTyp = new GH_ObjectWrapper();
      if (da.GetData(10, ref ghTyp)) {
        var node = new GsaNode();
        if (ghTyp.Value is GsaNodeGoo) {
          ghTyp.CastTo(ref node);
          elem.OrientationNode = node;
        }
        else {
          this.AddRuntimeWarning("Unable to convert Orientation Node input to GsaNode");
        }
      }

      var ghName = new GH_String();
      if (da.GetData(11, ref ghName)) {
        if (GH_Convert.ToString(ghName, out string name, GH_Conversion.Both))
          elem.Name = name;
      }

      var ghColour = new GH_Colour();
      if (da.GetData(12, ref ghColour)) {
        if (GH_Convert.ToColor(ghColour, out System.Drawing.Color col, GH_Conversion.Both))
          elem.Colour = col;
      }

      var ghDummy = new GH_Boolean();
      if (da.GetData(13, ref ghDummy)) {
        if (GH_Convert.ToBoolean(ghDummy, out bool dum, GH_Conversion.Both))
          elem.IsDummy = dum;
      }

      da.SetData(0, new GsaElement1dGoo(elem));
      da.SetData(1, elem.Id);
      da.SetData(2, new GH_Line(elem.Line.Line));
      da.SetData(3, new GsaSectionGoo(elem.Section));
      da.SetData(4, elem.Group);
      da.SetData(5, Mappings.ElementTypeMapping.FirstOrDefault(x => x.Value == elem.Type).Key);
      da.SetData(6, new GsaOffsetGoo(elem.Offset));
      da.SetData(7, new GsaBool6Goo(elem.ReleaseStart));
      da.SetData(8, new GsaBool6Goo(elem.ReleaseEnd));
      da.SetData(9, elem.OrientationAngle.As(AngleUnit.Degree));
      da.SetData(10, new GsaNodeGoo(elem.OrientationNode));
      da.SetData(11, elem.Name);
      da.SetData(12, elem.Colour);
      da.SetData(13, elem.IsDummy);

      try { da.SetData(14, elem.ParentMember); }
      catch (Exception) {
        // ignored
      }

      var topo = new DataTree<int>();
      topo.AddRange(elem.ApiElement.Topology, new GH_Path(elem.Id));
      da.SetDataTree(15, topo);
    }
  }
}
