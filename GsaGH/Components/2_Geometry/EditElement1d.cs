using System;
using System.Collections.ObjectModel;
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

namespace GsaGH.Components
{
    /// <summary>
    /// Component to edit a 1D Element
    /// </summary>
    public class EditElement1d : GH_OasysComponent, IGH_PreviewObject
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("5aa4635c-b60e-4812-ab45-6af9437255e4");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.EditElem1d;

    public EditElement1d() : base("Edit 1D Element",
      "Elem1dEdit",
      "Modify GSA 1D Element",
      CategoryName.Name(),
      SubCategoryName.Cat2())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
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

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
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

    protected override void BeforeSolveInstance()
    {
      base.BeforeSolveInstance();
      Param_Number angleParameter = Params.Input[9] as Param_Number;
      if (angleParameter != null)
      {
        if (angleParameter.UseDegrees)
          this.AngleUnit = AngleUnit.Degree;
        else
          this.AngleUnit = AngleUnit.Radian;
      }
    }
    AngleUnit AngleUnit = AngleUnit.Radian;

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaElement1d gsaElement1d = new GsaElement1d();
      GsaElement1d elem = new GsaElement1d();
      if (DA.GetData(0, ref gsaElement1d))
      {
        if (gsaElement1d == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Element1D input is null"); }
        elem = gsaElement1d.Duplicate(true);
      }

      if (elem != null)
      {
        // #### inputs ####
        // 1 ID
        GH_Integer ghID = new GH_Integer();
        if (DA.GetData(1, ref ghID))
        {
          if (GH_Convert.ToInt32(ghID, out int id, GH_Conversion.Both))
            elem.Id = id;
        }

        // 2 curve
        GH_Line ghcrv = new GH_Line();
        if (DA.GetData(2, ref ghcrv))
        {
          Line crv = new Line();
          if (GH_Convert.ToLine(ghcrv, ref crv, GH_Conversion.Both))
          {
            LineCurve ln = new LineCurve(crv);
            elem.Line = ln;
          }
        }

        // 3 section
        GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
        if (DA.GetData(3, ref gh_typ))
        {
          GsaSection section = new GsaSection();
          if (gh_typ.Value is GsaSectionGoo)
          {
            gh_typ.CastTo(ref section);
          }
          else
          {
            if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
            {
              section = new GsaSection(idd);
            }
            else
            {
              AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PB input to a Section Property of reference integer");
              return;
            }
          }
          elem.Section = section;
        }

        // 4 Group
        GH_Integer ghgrp = new GH_Integer();
        if (DA.GetData(4, ref ghgrp))
        {
          if (GH_Convert.ToInt32(ghgrp, out int grp, GH_Conversion.Both))
            elem.Group = grp; //elem.Element.Group = grp;
        }

        // 5 type
        GH_String ghstring = new GH_String();
        if (DA.GetData(5, ref ghstring))
        {
          if (GH_Convert.ToInt32(ghstring, out int typeInt, GH_Conversion.Both))
            elem.Type = (ElementType)typeInt;
          else if (GH_Convert.ToString(ghstring, out string typestring, GH_Conversion.Both))
          {
            try
            {
              elem.Type = Mappings.GetElementType(typestring);
            }
            catch (ArgumentException)
            {
              AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to change Element Type");
            }
          }
        }

        // 6 offset
        GsaOffset offset = new GsaOffset();
        if (DA.GetData(6, ref offset))
        {
          elem.Offset = offset;
        }

        // 7 start release
        GsaBool6 start = new GsaBool6();
        if (DA.GetData(7, ref start))
        {
          elem.ReleaseStart = start;
        }

        // 8 end release
        GsaBool6 end = new GsaBool6();
        if (DA.GetData(8, ref end))
        {
          elem.ReleaseEnd = end;
        }

        // 9 orientation angle
        GH_Number ghangle = new GH_Number();
        if (DA.GetData(9, ref ghangle))
        {
          if (GH_Convert.ToDouble(ghangle, out double angle, GH_Conversion.Both))
            elem.OrientationAngle = new Angle(angle, this.AngleUnit);
        }

        // 10 orientation node
        gh_typ = new GH_ObjectWrapper();
        if (DA.GetData(10, ref gh_typ))
        {
          GsaNode node = new GsaNode();
          if (gh_typ.Value is GsaNodeGoo)
          {
            gh_typ.CastTo(ref node);
            elem.OrientationNode = node;
          }
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Unable to convert Orientation Node input to GsaNode");
          }
        }

        // 11 name
        GH_String ghnm = new GH_String();
        if (DA.GetData(11, ref ghnm))
        {
          if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both))
            elem.Name = name;
        }

        // 12 Colour
        GH_Colour ghcol = new GH_Colour();
        if (DA.GetData(12, ref ghcol))
        {
          if (GH_Convert.ToColor(ghcol, out System.Drawing.Color col, GH_Conversion.Both))
            elem.Colour = col;
        }

        // 13 Dummy
        GH_Boolean ghdum = new GH_Boolean();
        if (DA.GetData(13, ref ghdum))
        {
          if (GH_Convert.ToBoolean(ghdum, out bool dum, GH_Conversion.Both))
            elem.IsDummy = dum;
        }

        // #### outputs ####
        DA.SetData(0, new GsaElement1dGoo(elem));
        DA.SetData(1, elem.Id);
        DA.SetData(2, new GH_Line(elem.Line.Line));
        DA.SetData(3, new GsaSectionGoo(elem.Section));
        DA.SetData(4, elem.Group);
        DA.SetData(5, Mappings.ElementTypeMapping.FirstOrDefault(x => x.Value == elem.Type).Key);
        DA.SetData(6, new GsaOffsetGoo(elem.Offset));
        DA.SetData(7, new GsaBool6Goo(elem.ReleaseStart));
        DA.SetData(8, new GsaBool6Goo(elem.ReleaseEnd));
        DA.SetData(9, elem.OrientationAngle.As(AngleUnit.Degree));
        DA.SetData(10, new GsaNodeGoo(elem.OrientationNode));
        DA.SetData(11, elem.Name);
        DA.SetData(12, elem.Colour);
        DA.SetData(13, elem.IsDummy);

        try { DA.SetData(14, elem.ParentMember); } catch (Exception) { }
        DataTree<int> topo = new DataTree<int>();
        topo.AddRange(elem.API_Element.Topology, new GH_Path(elem.Id));
        DA.SetDataTree(15, topo);
      }
    }
  }
}
