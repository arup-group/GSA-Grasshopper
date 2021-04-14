using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GhSA.Parameters;
using System.Resources;
using System.Linq;
using Rhino.Collections;

namespace GhSA.Components
{
    /// <summary>
    /// Component to edit a 2D Element
    /// </summary>
    public class EditElement2d : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("e9611aa7-88c1-4b5b-83d6-d9629e21ad8a");
        public EditElement2d()
          : base("Edit 2D Element", "Elem2dEdit", "Modify GSA 2D Element",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat2())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.EditElem2D;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout


        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            
            pManager.AddGenericParameter("2D Element", "E2D", "GSA 2D Element to Modify", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Element2d Number", "ID", "Set Element Number. If ID is set it will replace any existing 2d Element in the model", GH_ParamAccess.list);
            pManager.AddGenericParameter("2D Property", "PA", "Change 2D Property. Input either a GSA 2D Property or an Integer to use a Section already defined in model", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Element2d Group", "Gr", "Set Element Group", GH_ParamAccess.list);
            pManager.AddGenericParameter("Offset", "Of", "Set Element Offset", GH_ParamAccess.list);
            pManager.AddTextParameter("Element2d Name", "Na", "Set Name of Element", GH_ParamAccess.list);
            pManager.AddColourParameter("Element2d Colour", "Co", "Set Element Colour", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Dummy Element", "Dm", "Set Element to Dummy", GH_ParamAccess.list);

            for (int i = 1; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("2D Element", "E2D", "Modified GSA 2d Element", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Number", "ID", "Get Element Number", GH_ParamAccess.list);
            pManager.AddMeshParameter("Analysis Mesh", "M", "Get Analysis Mesh", GH_ParamAccess.item);
            pManager.HideParameter(2);
            pManager.AddGenericParameter("2D Property", "PA", "Get 2D Property. Input either a GSA 2D Property or an Integer to use a Section already defined in model", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Group", "Gr", "Get Element Group", GH_ParamAccess.list);
            pManager.AddTextParameter("Element Type", "eT", "Get Element 2D Type." + System.Environment.NewLine
                + "Type can not be set; it is either Tri3 or Quad4" + System.Environment.NewLine
                + "depending on Rhino/Grasshopper mesh face type", GH_ParamAccess.list);
            pManager.AddGenericParameter("Offset", "Of", "Get Element Offset", GH_ParamAccess.list);
            pManager.AddTextParameter("Name", "Na", "Set Element Name", GH_ParamAccess.list);
            pManager.AddColourParameter("Colour", "Co", "Get Element Colour", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Dummy Element", "Dm", "Get if Element is Dummy", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Parent Members", "pM", "Get Parent Member IDs in Model that Element was created from", GH_ParamAccess.list);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaElement2d gsaElement2d = new GsaElement2d();
            if (DA.GetData(0, ref gsaElement2d))
            {
                if (gsaElement2d == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Element2D input is null"); }
                GsaElement2d elem = gsaElement2d.Duplicate();

                // #### inputs ####

                // no good way of updating location of mesh on the fly // 
                // suggest users re-create from scratch //

                // 1 ID
                List<GH_Integer> ghID = new List<GH_Integer>();
                List<int> in_ids = new List<int>();
                if (DA.GetDataList(1, ghID))
                {
                    for (int i = 0; i < ghID.Count; i++)
                    {
                        if (i > elem.Elements.Count)
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ID input List Length is longer than number of elements." + System.Environment.NewLine + "Excess ID's have been ignored");
                            continue;
                        }
                        if (GH_Convert.ToInt32(ghID[i], out int id, GH_Conversion.Both))
                        {
                            if (in_ids.Contains(id))
                            {
                                if (id > 0)
                                {
                                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "ID input(" + i + ") = " + id + " already exist in your input list." + System.Environment.NewLine + "You must provide a list of unique IDs, or set ID = 0 if you want to let GSA handle the numbering");
                                    continue;
                                }
                            }
                            in_ids.Add(id);
                        }
                    }
                }

                // 2 section
                List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
                List<GsaProp2d> in_prop2Ds = new List<GsaProp2d>();
                if (DA.GetDataList(2, gh_types))
                {
                    for (int i = 0; i< gh_types.Count; i++)
                    {
                        if (i > elem.Elements.Count)
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "PA input List Length is longer than number of elements." + System.Environment.NewLine + "Excess PA's have been ignored");
                        GH_ObjectWrapper gh_typ = gh_types[i];
                        GsaProp2d prop2d = new GsaProp2d();
                        if (gh_typ.Value is GsaProp2dGoo)
                        {
                            gh_typ.CastTo(ref prop2d);
                            in_prop2Ds.Add(prop2d);
                        }
                        else
                        {
                            if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
                                elem.Elements[i].Property = idd;
                            else
                            {
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PA input to a 2D Property of reference integer");
                                return;
                            }
                        }
                    }
                }

                // 3 Group
                List<GH_Integer> ghgrp = new List<GH_Integer>();
                List<int> in_groups = new List<int>();
                if (DA.GetDataList(3, ghgrp))
                {
                    for (int i = 0; i < ghgrp.Count; i++)
                    {
                        if (i > elem.Elements.Count)
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Group input List Length is longer than number of elements." + System.Environment.NewLine + "Excess Group numbers have been ignored");
                            continue;
                        }
                        if (GH_Convert.ToInt32(ghgrp[i], out int grp, GH_Conversion.Both))
                            in_groups.Add(grp);
                    }
                }


                // 4 offset
                gh_types = new List<GH_ObjectWrapper>();
                List<GsaOffset> in_offsets = new List<GsaOffset>();
                if (DA.GetDataList(4, gh_types))
                {
                    for (int i = 0; i < gh_types.Count; i++)
                    {
                        if (i > elem.Elements.Count)
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Offset input List Length is longer than number of elements." + System.Environment.NewLine + "Excess Offsets have been ignored");
                        GH_ObjectWrapper gh_typ = gh_types[i];
                        GsaOffset offset = new GsaOffset();
                        if (gh_typ.Value is GsaOffsetGoo)
                            gh_typ.CastTo(ref offset);
                        else
                        {
                            if (GH_Convert.ToDouble(gh_typ.Value, out double z, GH_Conversion.Both))
                                offset.Z = z;
                            else
                            {
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert Offset input to Offset or double");
                                return;
                            }
                        }
                        in_offsets.Add(offset);
                    }
                }

                

                // 5 name
                List<GH_String> ghnm = new List<GH_String>();
                List<string> in_names = new List<string>();
                if (DA.GetDataList(5, ghnm))
                {
                    for (int i = 0; i < ghnm.Count; i++)
                    {
                        if (i > elem.Elements.Count)
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Name input List Length is longer than number of elements." + System.Environment.NewLine + "Excess Names have been ignored");
                            continue;
                        }
                        if (GH_Convert.ToString(ghnm[i], out string name, GH_Conversion.Both))
                            in_names.Add(name);
                    }
                }

                
                // 6 Colour
                List<GH_Colour> ghcol = new List<GH_Colour>();
                List<System.Drawing.Color> in_colours = new List<System.Drawing.Color>();
                if (DA.GetDataList(6, ghcol))
                {
                    for (int i = 0; i < ghcol.Count; i++)
                    {
                        if (i > elem.Elements.Count)
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Colour input List Length is longer than number of elements." + System.Environment.NewLine + "Excess Colours have been ignored");
                            continue;
                        }
                        if (GH_Convert.ToColor(ghcol[i], out System.Drawing.Color col, GH_Conversion.Both))
                            in_colours.Add(col);
                    }
                }

                // 7 Dummy
                List<GH_Boolean> ghdum = new List<GH_Boolean>();
                List<bool> in_dummies = new List<bool>();
                if (DA.GetDataList(7, ghdum))
                {
                    for (int i = 0; i < ghdum.Count; i++)
                    {
                        if (i > elem.Elements.Count)
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Dummy input List Length is longer than number of elements." + System.Environment.NewLine + "Excess Dummy booleans have been ignored");
                            continue;
                        }
                        if (GH_Convert.ToBoolean(ghdum[i], out bool dum, GH_Conversion.Both))
                            in_dummies.Add(dum);
                    }
                }
                

                // loop through all elements and set collected lists.
                // handle too short or too long input lists here
                // for short lists copy last item
                for (int i = 0; i < elem.Elements.Count; i++)
                {
                    if (in_prop2Ds.Count > 0)
                    {
                        if (i < in_prop2Ds.Count)
                            elem.Properties[i] = in_prop2Ds[i];
                        else
                            elem.Properties[i] = in_prop2Ds[in_prop2Ds.Count - 1];
                    }
                    if (in_offsets.Count > 0)
                    {
                        if (i < in_offsets.Count)
                            elem.Elements[i].Offset.Z = in_offsets[i].Z;
                        else
                            elem.Elements[i].Offset.Z = in_offsets[in_offsets.Count - 1].Z;
                    }
                    if (in_ids.Count > 0)
                    {
                        if (i < in_ids.Count)
                            elem.ID[i] = in_ids[i];
                        else
                            elem.ID[i] = 0; // do not set ID (element number) as it must be unique
                    }
                    if (in_names.Count > 0)
                    {
                        if (i < in_names.Count)
                            elem.Elements[i].Name = in_names[i];
                        else
                            elem.Elements[i].Name = in_names[in_names.Count - 1];
                    }
                    if (in_groups.Count > 0)
                    {
                        if (i < in_groups.Count)
                            elem.Elements[i].Group = in_groups[i];
                        else
                            elem.Elements[i].Group = in_groups[in_groups.Count - 1];
                    }
                    if (in_colours.Count > 0)
                    {
                        if (i < in_colours.Count)
                            elem.Elements[i].Colour = in_colours[i];
                        else
                            elem.Elements[i].Colour = in_colours[in_colours.Count - 1];
                    }
                    if (in_dummies.Count > 0)
                    {
                        if (i < in_dummies.Count)
                            elem.Elements[i].IsDummy = in_dummies[i];
                        else
                            elem.Elements[i].IsDummy = in_dummies[in_dummies.Count - 1];
                    }
                }


                // #### outputs ####

                DA.SetData(0, new GsaElement2dGoo(elem));
                DA.SetDataList(1, elem.ID);
                DA.SetData(2, elem.Mesh);

                List<GsaOffset> out_offsets = new List<GsaOffset>();
                List<string> type = new List<string>();
                List<string> out_names = new List<string>();
                List<int> out_groups = new List<int>();
                List<System.Drawing.Color> out_colours = new List<System.Drawing.Color>();
                List<int> pmems = new List<int>();
                List<bool> out_dummies = new List<bool>();
                for (int i = 0; i < elem.Elements.Count; i++)
                {
                    GsaOffset offset1 = new GsaOffset
                    {
                        Z = elem.Elements[i].Offset.Z
                    };
                    out_offsets.Add(offset1);
                    type.Add(elem.Elements[i].TypeAsString());
                    out_names.Add(elem.Elements[i].Name);
                    out_groups.Add(elem.Elements[i].Group);
                    out_colours.Add((System.Drawing.Color)elem.Elements[i].Colour);
                    out_dummies.Add(elem.Elements[i].IsDummy);
                    try { pmems.Add(elem.Elements[i].ParentMember.Member); } catch (Exception) { pmems.Add(0); }
                    ;
                }
                DA.SetDataList(3, elem.Properties);
                DA.SetDataList(4, out_groups);
                DA.SetDataList(5, type);
                DA.SetDataList(6, out_offsets);
                DA.SetDataList(7, out_names);
                DA.SetDataList(8, out_colours);
                DA.SetDataList(9, out_dummies);
                DA.SetDataList(10, pmems);
            }
        }
    }
}

