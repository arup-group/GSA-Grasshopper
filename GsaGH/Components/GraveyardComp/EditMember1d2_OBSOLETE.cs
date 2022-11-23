using System;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using System.Linq;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysUnits;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysUnits.Units;
using OasysGH;
using GsaGH.Helpers.GH;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to edit a 1D Member
    /// </summary>
    public class EditMember1d2_OBSOLETE : GH_OasysComponent, IGH_PreviewObject
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("2a121578-f9ff-4d80-ae90-2982faa425a6");
    public EditMember1d2_OBSOLETE()
      : base("Edit 1D Member", "Mem1dEdit", "Modify GSA 1D Member",
            CategoryName.Name(),
            SubCategoryName.Cat2())
    {
    }

    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.EditMem1d;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout


    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {

      pManager.AddGenericParameter("1D Member", "M1D", "GSA 1D Member to Modify", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member1d Number", "ID", "Set Member Number. If ID is set it will replace any existing 1D Member in the model", GH_ParamAccess.item);
      pManager.AddCurveParameter("Curve", "C", "Member Curve", GH_ParamAccess.item);
      pManager.AddGenericParameter("Section", "PB", "Change Section Property", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member1d Group", "Gr", "Set Member 1D Group", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member Type", "mT", "Set 1D Member Type" + Environment.NewLine +
          "Default is 0: Generic 1D - Accepted inputs are:" + Environment.NewLine +
          "2: Beam" + Environment.NewLine +
          "3: Column" + Environment.NewLine +
          "6: Cantilever" + Environment.NewLine +
          "8: Compos" + Environment.NewLine +
          "9: Pile" + Environment.NewLine +
          "11: Void cutter", GH_ParamAccess.item);
      pManager.AddIntegerParameter("1D Element Type", "eT", "Set Element 1D Type" + Environment.NewLine +
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
      pManager.AddGenericParameter("Offset", "Of", "Set Member Offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Start release", "⭰", "Set Release (Bool6) at Start of Member", GH_ParamAccess.item);
      pManager.AddGenericParameter("End release", "⭲", "Set Release (Bool6) at End of Member", GH_ParamAccess.item);
      pManager.AddNumberParameter("Orientation Angle", "⭮A", "Set Member Orientation Angle in degrees", GH_ParamAccess.item);
      pManager.AddGenericParameter("Orientation Node", "⭮N", "Set Member Orientation Node", GH_ParamAccess.item);
      Length ms = new Length(1, DefaultUnits.LengthUnitGeometry);
      pManager.AddGenericParameter("Mesh Size [" + ms.ToString("a") + "]", "Ms", "Set Member Mesh Size", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Mesh With Others", "M/o", "Mesh with others?", GH_ParamAccess.item);

      pManager.AddTextParameter("Member1d Name", "Na", "Set Name of Member1d", GH_ParamAccess.item);
      pManager.AddColourParameter("Member1d Colour", "Co", "Set Member 1D Colour", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Member", "Dm", "Set Member to Dummy", GH_ParamAccess.item);

      for (int i = 1; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;

      pManager.HideParameter(0);
      pManager.HideParameter(2);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("1D Member", "M1D", "Modified GSA 1D Member", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member1d Number", "ID", "Get Member Number", GH_ParamAccess.item);
      pManager.AddCurveParameter("Curve", "C", "Member Curve", GH_ParamAccess.item);
      pManager.HideParameter(2);
      pManager.AddGenericParameter("Section", "PB", "Change Section Property. Input either a GSA Section or an Integer to use a Section already defined in model", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member Group", "Gr", "Get Member Group", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member Type", "mT", "Get 1D Member Type", GH_ParamAccess.item);
      pManager.AddIntegerParameter("1D Element Type", "eT", "Get Element 1D Type", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset", "Of", "Get Member Offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Start release", "⭰", "Get Release (Bool6) at Start of Member", GH_ParamAccess.item);
      pManager.AddGenericParameter("End release", "⭲", "Get Release (Bool6) at End of Member", GH_ParamAccess.item);
      pManager.AddNumberParameter("Orientation Angle", "⭮A", "Get Member Orientation Angle in degrees", GH_ParamAccess.item);
      pManager.AddGenericParameter("Orientation Node", "⭮N", "Get Member Orientation Node", GH_ParamAccess.item);
      pManager.AddGenericParameter("Mesh Size", "Ms", "Get Member Mesh Size", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Mesh With Others", "M/o", "Get if to mesh with others", GH_ParamAccess.item);

      pManager.AddTextParameter("Member Name", "Na", "Get Name of Member1d", GH_ParamAccess.item);

      pManager.AddColourParameter("Member Colour", "Co", "Get Member Colour", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Member", "Dm", "Get it Member is Dummy", GH_ParamAccess.item);
      pManager.AddTextParameter("Topology", "Tp", "Get the Member's original topology list referencing node IDs in Model that Model was created from", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaMember1d gsaMember1d = new GsaMember1d();
      if (DA.GetData(0, ref gsaMember1d))
      {
        if (gsaMember1d == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Member1D input is null"); }
        GsaMember1d mem = gsaMember1d.Duplicate();

        // #### inputs ####
        // 1 ID
        GH_Integer ghID = new GH_Integer();
        if (DA.GetData(1, ref ghID))
        {
          if (GH_Convert.ToInt32(ghID, out int id, GH_Conversion.Both))
            mem.Id = id;
        }

        // 2 curve
        GH_Curve ghcrv = new GH_Curve();
        if (DA.GetData(2, ref ghcrv))
        {
          Curve crv = null;
          if (GH_Convert.ToCurve(ghcrv, ref crv, GH_Conversion.Both))
          {
            if (crv is PolyCurve)
              mem.PolyCurve = (PolyCurve)crv;
            else
            {
              GsaMember1d tempMem = new GsaMember1d(crv);
              mem.PolyCurve = tempMem.PolyCurve;
            }
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
          mem.Section = section;
        }

        // 4 Group
        GH_Integer ghgrp = new GH_Integer();
        if (DA.GetData(4, ref ghgrp))
        {
          if (GH_Convert.ToInt32(ghgrp, out int grp, GH_Conversion.Both))
            mem.Group = grp;
        }

        // 5 type
        GH_Integer ghint = new GH_Integer();
        if (DA.GetData(5, ref ghint))
        {
          if (GH_Convert.ToInt32(ghint, out int type, GH_Conversion.Both))
            mem.Type = (MemberType)type;
        }

        // 6 element type
        GH_Integer ghinteg = new GH_Integer();
        if (DA.GetData(6, ref ghinteg))
        {
          if (GH_Convert.ToInt32(ghinteg, out int type, GH_Conversion.Both))
            mem.Type1D = (ElementType)type;
        }

        // 7 offset
        GsaOffset offset = new GsaOffset();
        if (DA.GetData(7, ref offset))
        {
          mem.Offset = offset;
        }

        // 8 start release
        GsaBool6 start = new GsaBool6();
        if (DA.GetData(8, ref start))
        {
          mem.ReleaseStart = start;
        }

        // 9 end release
        GsaBool6 end = new GsaBool6();
        if (DA.GetData(9, ref end))
        {
          mem.ReleaseEnd = end;
        }

        // 10 orientation angle
        GH_Number ghangle = new GH_Number();
        if (DA.GetData(10, ref ghangle))
        {
          if (GH_Convert.ToDouble(ghangle, out double angle, GH_Conversion.Both))
            mem.OrientationAngle = new Angle(angle, AngleUnit.Degree);
        }

        // 11 orientation node
        gh_typ = new GH_ObjectWrapper();
        if (DA.GetData(11, ref gh_typ))
        {
          GsaNode node = new GsaNode();
          if (gh_typ.Value is GsaNodeGoo)
          {
            gh_typ.CastTo(ref node);
            mem.OrientationNode = node;
          }
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Unable to convert Orientation Node input to GsaNode");
          }
        }

        // 12 mesh size
        GH_Number ghmsz = new GH_Number();
        if (Params.Input[12].Sources.Count > 0)
        {
          mem.MeshSize = ((Length)Input.UnitNumber(this, DA, 12, DefaultUnits.LengthUnitGeometry, true)).Meters;
          if (DefaultUnits.LengthUnitGeometry != OasysUnits.Units.LengthUnit.Meter)
            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Mesh size input set in [" + string.Concat(mem.MeshSize.ToString().Where(char.IsLetter)) + "]. "
                + Environment.NewLine + "Note that this is based on your unit settings and may be changed to a different unit if you share this file or change your 'Length - geometry' unit settings. Use a UnitNumber input to use a specific unit.");
        }

        // 13 mesh with others
        GH_Boolean ghbool = new GH_Boolean();
        if (DA.GetData(13, ref ghbool))
        {
          if (GH_Convert.ToBoolean(ghbool, out bool mbool, GH_Conversion.Both))
          {
            if (mem.MeshWithOthers != mbool)
              mem.MeshWithOthers = mbool;
          }
        }

        // 14 name
        GH_String ghnm = new GH_String();
        if (DA.GetData(14, ref ghnm))
        {
          if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both))
            mem.Name = name;
        }

        // 15 Colour
        GH_Colour ghcol = new GH_Colour();
        if (DA.GetData(15, ref ghcol))
        {
          if (GH_Convert.ToColor(ghcol, out System.Drawing.Color col, GH_Conversion.Both))
            mem.Colour = col;
        }

        // 16 Dummy
        GH_Boolean ghdum = new GH_Boolean();
        if (DA.GetData(16, ref ghdum))
        {
          if (GH_Convert.ToBoolean(ghdum, out bool dum, GH_Conversion.Both))
            mem.IsDummy = dum;
        }

        // #### outputs ####
        DA.SetData(0, new GsaMember1dGoo(mem));
        DA.SetData(1, mem.Id);
        DA.SetData(2, mem.PolyCurve);
        DA.SetData(3, new GsaSectionGoo(mem.Section));
        DA.SetData(4, mem.Group);
        DA.SetData(5, mem.Type);
        DA.SetData(6, mem.Type1D);

        DA.SetData(7, new GsaOffsetGoo(mem.Offset));

        DA.SetData(8, mem.ReleaseStart);
        DA.SetData(9, mem.ReleaseEnd);

        DA.SetData(10, mem.OrientationAngle);
        DA.SetData(11, new GsaNodeGoo(mem.OrientationNode));

        DA.SetData(12, new GH_UnitNumber(new Length(mem.MeshSize, LengthUnit.Meter).ToUnit(DefaultUnits.LengthUnitGeometry)));
        DA.SetData(13, mem.MeshWithOthers);

        DA.SetData(14, mem.Name);

        DA.SetData(15, mem.Colour);
        DA.SetData(16, mem.IsDummy);
        DA.SetData(17, mem.ApiMember.Topology.ToString());
      }
    }
  }
}
