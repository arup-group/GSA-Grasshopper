using GhSA.Parameters;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;

namespace GhSA.Components
{
    /// <summary>
    /// Component to edit a 1D Element
    /// </summary>
    public class EditElement1d : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("aeb5f765-8721-41fc-a1b4-cfd78e05ce67");
        public EditElement1d()
          : base("Edit 1D Element", "Elem1dEdit", "Modify GSA 1D Element",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat2())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.EditElem1D;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout


        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            
            pManager.AddGenericParameter("1D Element", "E1D", "GSA 1D Element to Modify", GH_ParamAccess.item);
            pManager.AddLineParameter("Line", "L", "Reposition Element Line", GH_ParamAccess.item);
            pManager.AddGenericParameter("Section", "PB", "Change Section Property", GH_ParamAccess.item);
            pManager.AddGenericParameter("Offset", "Of", "Set Element Offset", GH_ParamAccess.item);
            pManager.AddGenericParameter("Start release", "B6s", "Set Release (Bool6) at Start of Element", GH_ParamAccess.item);
            pManager.AddGenericParameter("End release", "B6e", "Set Release (Bool6) at End of Element", GH_ParamAccess.item);
            pManager.AddNumberParameter("Orientation Angle", "OrA", "Set Element Orientation Angle in degrees", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Orientation Node", "OrN", "Set Element Orientation Node (ID referring to node number in model)", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Type", "Ty", "Set Element Type", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Number", "ID", "Set Element Number. If ID is set it will replace any existing 1D Element in the model", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "Na", "Set Element Name", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Group", "Gr", "Set Element Group", GH_ParamAccess.item);
            pManager.AddColourParameter("Colour", "Co", "Set Element Colour", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
            pManager[8].Optional = true;
            pManager[9].Optional = true;
            pManager[10].Optional = true;
            pManager[11].Optional = true;
            pManager[12].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("1D Element", "E1D", "Modified GSA 1D Element", GH_ParamAccess.item);
            pManager.AddLineParameter("Line", "L", "Element Line", GH_ParamAccess.item);
            pManager.AddGenericParameter("Section", "PB", "Get Section Property. Input either a GSA Section or an Integer to use a Section already defined in model", GH_ParamAccess.item);
            pManager.AddGenericParameter("Offset", "Of", "Get Element Offset", GH_ParamAccess.item);
            pManager.AddGenericParameter("Start release", "B6s", "Get Release (Bool6) at Start of Element", GH_ParamAccess.item);
            pManager.AddGenericParameter("End release", "B6e", "Get Release (Bool6) at End of Element", GH_ParamAccess.item);
            pManager.AddNumberParameter("Orientation Angle", "OrA", "Get Element Orientation Angle in degrees", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Orientation Node", "OrN", "Get Element Orientation Node (ID referring to node number in model)", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Type", "Ty", "Get Element Type", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Number", "ID", "Get Element Number. If ID is set it will replace any existing 1D Element in the model", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "Na", "Get Element Name", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Group", "Gr", "Get Element Group", GH_ParamAccess.item);
            pManager.AddColourParameter("Colour", "Co", "Get Element Colour", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Parent Members", "pM", "Get Parent Member IDs in Model that Element was created from", GH_ParamAccess.list);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaElement1d gsaElement1d = new GsaElement1d();
            if (DA.GetData(0, ref gsaElement1d))
            {
                GsaElement1d elem = gsaElement1d;

                // #### inputs ####
                // 1 curve
                GH_Line ghcrv = new GH_Line();
                if (DA.GetData(1, ref ghcrv))
                {
                    Line crv = new Line();
                    if (GH_Convert.ToLine(ghcrv, ref crv, GH_Conversion.Both))
                    {
                        LineCurve ln = new LineCurve(crv);
                        GsaElement1d tmpelem = new GsaElement1d(ln)
                        {
                            ID = elem.ID,
                            Element = elem.Element,
                            ReleaseEnd = elem.ReleaseEnd,
                            ReleaseStart = elem.ReleaseStart
                        };
                        elem = tmpelem;
                    }
                }

                // 2 section
                GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
                if (DA.GetData(2, ref gh_typ))
                {
                    GsaSection section = new GsaSection();
                    if (gh_typ.Value is GsaSectionGoo)
                        gh_typ.CastTo(ref section);
                    else
                    {
                        if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
                            section.ID = idd;
                        else
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PB input to a Section Property of reference integer");
                            return;
                        }
                    }
                    elem.Section = section;
                }

                // 3 offset
                GsaOffset offset = new GsaOffset();
                if (DA.GetData(3, ref offset))
                {
                    elem.Element.Offset.X1 = offset.X1;
                    elem.Element.Offset.X2 = offset.X2;
                    elem.Element.Offset.Y = offset.Y;
                    elem.Element.Offset.Z = offset.Z;
                }

                // 4 start release
                GsaBool6 start = new GsaBool6();
                if (DA.GetData(4, ref start))
                {
                    elem.ReleaseStart = start; //should handle setting the release in elem.Element.SetRelease
                }

                // 5 end release
                GsaBool6 end = new GsaBool6();
                if (DA.GetData(5, ref end))
                {
                    elem.ReleaseEnd = end; //should handle setting the release in elem.Element.SetRelease
                }

                // 6 orientation angle
                GH_Number ghangle = new GH_Number();
                if (DA.GetData(6, ref ghangle))
                {
                    if (GH_Convert.ToDouble(ghangle, out double angle, GH_Conversion.Both))
                        elem.Element.OrientationAngle = angle;
                }

                // 7 orientation node
                GH_Integer ghori = new GH_Integer();
                if (DA.GetData(7, ref ghori))
                {
                    if (GH_Convert.ToInt32(ghori, out int orient, GH_Conversion.Both))
                        elem.Element.OrientationNode = orient;
                }

                // 8 type
                GH_Integer ghinteg = new GH_Integer();
                if (DA.GetData(8, ref ghinteg))
                {
                    if (GH_Convert.ToInt32(ghinteg, out int type, GH_Conversion.Both))
                        elem.Element.Type = Util.Gsa.GsaToModel.Element1dType(type);
                }

                // 9 ID
                GH_Integer ghID = new GH_Integer();
                if (DA.GetData(9, ref ghID))
                {
                    if (GH_Convert.ToInt32(ghID, out int id, GH_Conversion.Both))
                        elem.ID = id;
                }

                // 10 name
                GH_String ghnm = new GH_String();
                if (DA.GetData(10, ref ghnm))
                {
                    if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both))
                        elem.Element.Name = name;
                }

                // 11 Group
                GH_Integer ghgrp = new GH_Integer();
                if (DA.GetData(11, ref ghgrp))
                {
                    if (GH_Convert.ToInt32(ghgrp, out int grp, GH_Conversion.Both))
                        elem.Element.Group = grp;
                }

                // 12 Colour
                GH_Colour ghcol = new GH_Colour();
                if (DA.GetData(12, ref ghcol))
                {
                    if (GH_Convert.ToColor(ghcol, out System.Drawing.Color col, GH_Conversion.Both))
                        elem.Element.Colour = col;
                }

                // #### outputs ####
                DA.SetData(0, new GsaElement1dGoo(elem));

                DA.SetData(1, elem.Line);
                DA.SetData(2, elem.Section);

                GsaOffset offset1 = new GsaOffset
                {
                    X1 = elem.Element.Offset.X1,
                    X2 = elem.Element.Offset.X2,
                    Y = elem.Element.Offset.Y,
                    Z = elem.Element.Offset.Z
                };
                DA.SetData(3, offset1);

                DA.SetData(4, elem.ReleaseStart);
                DA.SetData(5, elem.ReleaseEnd);

                DA.SetData(6, elem.Element.OrientationAngle);
                DA.SetData(7, elem.Element.OrientationNode);

                DA.SetData(8, elem.Element.Type);

                DA.SetData(9, elem.ID);
                DA.SetData(10, elem.Element.Name);
                DA.SetData(11, elem.Element.Group);
                DA.SetData(12, elem.Element.Colour);

                try { DA.SetData(13, elem.Element.ParentMember.Member); } catch (Exception) { }
                //DA.SetData(16, gsaElement1d.Element.IsDummy);
            }
        }
    }
}

