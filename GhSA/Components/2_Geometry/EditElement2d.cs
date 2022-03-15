using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using System.Linq;
using UnitsNet;

namespace GsaGH.Components
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

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.EditElem2d;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout


        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            
            pManager.AddGenericParameter("2D Element", "E2D", "GSA 2D Element to Modify", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Element2d Number", "ID", "Set Element Number. If ID is set it will replace any existing 2D Element in the model", GH_ParamAccess.list);
            pManager.AddGenericParameter("2D Property", "PA", "Change 2D Property. Input either a GSA 2D Property or an Integer to use a Property already defined in model", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Element2d Group", "Gr", "Set Element Group", GH_ParamAccess.list);
            pManager.AddGenericParameter("Offset", "Of", "Set Element Offset", GH_ParamAccess.list);
            pManager.AddTextParameter("Element2d Name", "Na", "Set Name of Element", GH_ParamAccess.list);
            pManager.AddColourParameter("Element2d Colour", "Co", "Set Element Colour", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Dummy Element", "Dm", "Set Element to Dummy", GH_ParamAccess.list);

            for (int i = 1; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;

            pManager.HideParameter(0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("2D Element", "E2D", "Modified GSA 2d Element", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Number", "ID", "Get Element Number", GH_ParamAccess.list);
            pManager.AddMeshParameter("Analysis Mesh", "M", "Get Analysis Mesh", GH_ParamAccess.item);
            pManager.HideParameter(2);
            pManager.AddGenericParameter("2D Property", "PA", "Get 2D Property. Input either a GSA 2D Property or an Integer to use a Property already defined in model", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Group", "Gr", "Get Element Group", GH_ParamAccess.list);
            pManager.AddTextParameter("Element Type", "eT", "Get Element 2D Type." + System.Environment.NewLine
                + "Type can not be set; it is either Tri3 or Quad4" + System.Environment.NewLine
                + "depending on Rhino/Grasshopper mesh face type", GH_ParamAccess.list);
            pManager.AddGenericParameter("Offset", "Of", "Get Element Offset", GH_ParamAccess.list);
            pManager.AddTextParameter("Name", "Na", "Set Element Name", GH_ParamAccess.list);
            pManager.AddColourParameter("Colour", "Co", "Get Element Colour", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Dummy Element", "Dm", "Get if Element is Dummy", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Parent Members", "pM", "Get Parent Member IDs in Model that Element was created from", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Topology", "Tp", "Get the Element's original topology list referencing node IDs in Model that Element was created from", GH_ParamAccess.list);
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
                        if (i > elem.API_Elements.Count)
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
                if (DA.GetDataList(2, gh_types))
                {
                    List<GsaProp2d> prop2Ds = new List<GsaProp2d>();
                    for (int i = 0; i< gh_types.Count; i++)
                    {
                        if (i > elem.API_Elements.Count)
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "PA input List Length is longer than number of elements." + System.Environment.NewLine + "Excess PA's have been ignored");
                        GH_ObjectWrapper gh_typ = gh_types[i];
                        GsaProp2d prop2d = new GsaProp2d();
                        if (gh_typ.Value is GsaProp2dGoo)
                        {
                            gh_typ.CastTo(ref prop2d);
                            prop2Ds.Add(prop2d);
                        }
                        else
                        {
                            if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
                            {
                                prop2Ds.Add(new GsaProp2d(idd));
                            }
                            else
                            {
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PA input to a 2D Property of reference integer");
                                return;
                            }
                        }
                    }
                    elem.Properties = prop2Ds;
                }

                // 3 Group
                List<GH_Integer> ghgrp = new List<GH_Integer>();
                if (DA.GetDataList(3, ghgrp))
                {
                    List<int> in_groups = new List<int>();
                    for (int i = 0; i < ghgrp.Count; i++)
                    {
                        if (i > elem.API_Elements.Count)
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Group input List Length is longer than number of elements." + System.Environment.NewLine + "Excess Group numbers have been ignored");
                            continue;
                        }
                        if (GH_Convert.ToInt32(ghgrp[i], out int grp, GH_Conversion.Both))
                            in_groups.Add(grp);
                    }
                    elem.Groups = in_groups;
                }


                // 4 offset
                gh_types = new List<GH_ObjectWrapper>();
                if (DA.GetDataList(4, gh_types))
                {
                    List<GsaOffset> in_offsets = new List<GsaOffset>();
                    for (int i = 0; i < gh_types.Count; i++)
                    {
                        if (i > elem.API_Elements.Count)
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Offset input List Length is longer than number of elements." + System.Environment.NewLine + "Excess Offsets have been ignored");
                        GH_ObjectWrapper gh_typ = gh_types[i];
                        GsaOffset offset = new GsaOffset();
                        if (gh_typ.Value is GsaOffsetGoo)
                            gh_typ.CastTo(ref offset);
                        else
                        {
                            if (GH_Convert.ToDouble(gh_typ.Value, out double z, GH_Conversion.Both))
                            {
                                offset.Z = new Length(z, Units.LengthUnitGeometry);
                                string unitAbbreviation = string.Concat(offset.Z.ToString().Where(char.IsLetter));
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Offset input converted to Z-offset in [" + unitAbbreviation + "]"
                                    + System.Environment.NewLine + "Note that this is based on your unit settings and may be changed to a different unit if you share this file or change your 'Length - geometry' unit settings");
                            }
                            else
                            {
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert Offset input to Offset or double");
                                return;
                            }
                        }
                        in_offsets.Add(offset);
                    }
                    elem.Offsets = in_offsets;
                }

                // 5 name
                List<GH_String> ghnm = new List<GH_String>();
                if (DA.GetDataList(5, ghnm))
                {
                    List<string> in_names = new List<string>();
                    for (int i = 0; i < ghnm.Count; i++)
                    {
                        if (i > elem.API_Elements.Count)
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Name input List Length is longer than number of elements." + System.Environment.NewLine + "Excess Names have been ignored");
                            continue;
                        }
                        if (GH_Convert.ToString(ghnm[i], out string name, GH_Conversion.Both))
                            in_names.Add(name);
                    }
                    elem.Names = in_names;
                }
                
                // 6 Colour
                List<GH_Colour> ghcol = new List<GH_Colour>();
                if (DA.GetDataList(6, ghcol))
                {
                    List<System.Drawing.Color> in_colours = new List<System.Drawing.Color>();
                    for (int i = 0; i < ghcol.Count; i++)
                    {
                        if (i > elem.API_Elements.Count)
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Colour input List Length is longer than number of elements." + System.Environment.NewLine + "Excess Colours have been ignored");
                            continue;
                        }
                        if (GH_Convert.ToColor(ghcol[i], out System.Drawing.Color col, GH_Conversion.Both))
                            in_colours.Add(col);
                    }
                    elem.Colours = in_colours;
                }

                // 7 Dummy
                List<GH_Boolean> ghdum = new List<GH_Boolean>();
                
                if (DA.GetDataList(7, ghdum))
                {
                    List<bool> in_dummies = new List<bool>();
                    for (int i = 0; i < ghdum.Count; i++)
                    {
                        if (i > elem.API_Elements.Count)
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Dummy input List Length is longer than number of elements." + System.Environment.NewLine + "Excess Dummy booleans have been ignored");
                            continue;
                        }
                        if (GH_Convert.ToBoolean(ghdum[i], out bool dum, GH_Conversion.Both))
                            in_dummies.Add(dum);
                    }
                    elem.isDummies = in_dummies;
                }
                
                // #### outputs ####
                DA.SetData(0, new GsaElement2dGoo(elem));
                DA.SetDataList(1, elem.ID);
                DA.SetData(2, elem.Mesh);
                DA.SetDataList(3, new List<GsaProp2dGoo>(elem.Properties.ConvertAll(prop2d => new GsaProp2dGoo(prop2d))));
                DA.SetDataList(4, elem.Groups);
                DA.SetDataList(5, elem.Types);
                DA.SetDataList(6, new List<GsaOffsetGoo>(elem.Offsets.ConvertAll(offset => new GsaOffsetGoo(offset))));
                DA.SetDataList(7, elem.Names);
                DA.SetDataList(8, elem.Colours);
                DA.SetDataList(9, elem.isDummies);
                DA.SetDataList(10, elem.ParentMembers);
                DA.SetDataTree(11, elem.TopologyIDs);
            }
        }
    }
}
