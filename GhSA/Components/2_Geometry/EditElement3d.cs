using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using System.Linq;
using Grasshopper;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to edit a 2D Element
  /// </summary>
  public class EditElement3d : GH_OasysComponent, IGH_PreviewObject
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("040f2915-543d-41ef-9a64-0c4055e47a63");
    public EditElement3d()
      : base("Edit 3D Element", "Elem3dEdit", "Modify GSA 3D Element",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat2())
    {
    }

    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.EditElem3d;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout


    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {

      pManager.AddGenericParameter("3D Element", "E3D", "GSA 3d Element to Modify", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Element3d Number", "ID", "Set Element Number. If ID is set it will replace any existing 3d Element in the model", GH_ParamAccess.list);
      pManager.AddGenericParameter("3D Property", "PV", "Change 3D Property. Input either a GSA 3D Property or an Integer to use a Property already defined in model", GH_ParamAccess.list);
      pManager.AddIntegerParameter("Element3d Group", "Gr", "Set Element Group", GH_ParamAccess.list);
      pManager.AddTextParameter("Element3d Name", "Na", "Set Name of Element", GH_ParamAccess.list);
      pManager.AddColourParameter("Element3d Colour", "Co", "Set Element Colour", GH_ParamAccess.list);
      pManager.AddBooleanParameter("Dummy Element", "Dm", "Set Element to Dummy", GH_ParamAccess.list);

      for (int i = 1; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;

      pManager.HideParameter(0);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("3D Element", "E3D", "Modified GSA 3D Element", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Number", "ID", "Get Element Number", GH_ParamAccess.list);
      pManager.AddMeshParameter("Analysis Mesh", "M", "Get Analysis Mesh. " + System.Environment.NewLine
          + "This will export a list of solid meshes representing each 3D element." + System.Environment.NewLine
          + "To get a combined mesh connect a GSA Element 3D to normal Mesh Parameter component to convert on the fly", GH_ParamAccess.item);
      pManager.HideParameter(2);
      pManager.AddGenericParameter("3D Property", "PA", "Get 3D Property. Input either a GSA 3D Property or an Integer to use a Property already defined in model", GH_ParamAccess.list);
      pManager.AddIntegerParameter("Group", "Gr", "Get Element Group", GH_ParamAccess.list);
      pManager.AddTextParameter("Element Type", "eT", "Get Element 3D Type." + System.Environment.NewLine
          + "Type can not be set; it is either Tetra4, Pyramid5, Wedge6 or Brick8", GH_ParamAccess.list);
      pManager.AddTextParameter("Name", "Na", "Set Element Name", GH_ParamAccess.list);
      pManager.AddColourParameter("Colour", "Co", "Get Element Colour", GH_ParamAccess.list);
      pManager.AddBooleanParameter("Dummy Element", "Dm", "Get if Element is Dummy", GH_ParamAccess.list);
      pManager.AddIntegerParameter("Parent Members", "pM", "Get Parent Member IDs in Model that Element was created from", GH_ParamAccess.list);
      pManager.AddIntegerParameter("Topology", "Tp", "Get the Element's original topology list referencing node IDs in Model that Element was created from", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaElement3d gsaElement3d = new GsaElement3d();
      if (DA.GetData(0, ref gsaElement3d))
      {
        if (gsaElement3d == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Element3D input is null"); }
        GsaElement3d elem = gsaElement3d.Duplicate();

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
          elem.ID = in_ids;
        }


        List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
        // 2 Prop3d
        //List<GsaProp2d> in_prop2Ds = new List<GsaProp2d>();
        if (DA.GetDataList(2, gh_types))
        {
          for (int i = 0; i < gh_types.Count; i++)
          {
            //        if (i > elem.Elements.Count)
            //            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "PA input List Length is longer than number of elements." + System.Environment.NewLine + "Excess PA's have been ignored");
            GH_ObjectWrapper gh_typ = gh_types[i];
            //        GsaProp2d prop2d = new GsaProp2d();
            //        if (gh_typ.Value is GsaProp2dGoo)
            //        {
            //            gh_typ.CastTo(ref prop2d);
            //            in_prop2Ds.Add(prop2d);
            //            elem.Elements[i].Property = 0;
            //        }
            //        else
            //        {
            if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
            {
              elem.PropertyIDs[i] = idd;
              //                elem.Elements[i].Property = idd;
              //                elem.Properties[i] = null;
            }
            //            else
            //            {
            //                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PA input to a 2D Property of reference integer");
            //                return;
            //            }
            //        }
          }
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

        // 4 name
        List<GH_String> ghnm = new List<GH_String>();
        if (DA.GetDataList(4, ghnm))
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

        // 5 Colour
        List<GH_Colour> ghcol = new List<GH_Colour>();
        if (DA.GetDataList(5, ghcol))
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

        // 6 Dummy
        List<GH_Boolean> ghdum = new List<GH_Boolean>();
        if (DA.GetDataList(6, ghdum))
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

        // convert mesh to output meshes
        List<Mesh> out_meshes = new List<Mesh>();
        Mesh x = elem.NgonMesh;

        List<MeshNgon> ngons = x.GetNgonAndFacesEnumerable().ToList();

        for (int i = 0; i < ngons.Count; i++)
        {
          Mesh m = new Mesh();
          m.Vertices.AddVertices(x.Vertices.ToList());
          List<int> faceindex = ngons[i].FaceIndexList().Select(u => (int)u).ToList();
          for (int j = 0; j < faceindex.Count; j++)
          {
            m.Faces.AddFace(x.Faces[faceindex[j]]);
          }
          m.Vertices.CullUnused();
          m.RebuildNormals();
          out_meshes.Add(m);
        }

        DA.SetData(0, new GsaElement3dGoo(elem));
        DA.SetDataList(1, elem.ID);
        DA.SetDataList(2, out_meshes);
        DA.SetDataList(4, elem.Groups);
        DA.SetDataList(5, elem.Types);
        DA.SetDataList(6, elem.Names);
        DA.SetDataList(7, elem.Colours);
        DA.SetDataList(8, elem.isDummies);
        DA.SetDataList(9, elem.ParentMembers);
        DA.SetDataTree(10, elem.TopologyIDs);
      }
    }
  }
}

