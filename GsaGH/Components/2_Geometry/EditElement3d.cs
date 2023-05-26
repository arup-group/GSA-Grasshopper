using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a 3D Element
  /// </summary>
  public class EditElement3d : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("040f2915-543d-41ef-9a64-0c4055e47a63");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditElem3d;

    public EditElement3d() : base("Edit 3D Element", "Elem3dEdit", "Modify GSA 3D Element",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaElement3dParameter(), GsaElement3dGoo.Name,
        GsaElement3dGoo.NickName, GsaElement3dGoo.Description + " to get or set information for.",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Number", "ID", "Set Element Number", GH_ParamAccess.list);
      pManager.AddParameter(new GsaProp3dParameter(), "3D Property", "PV",
        "Change 3D Property. Input either a GSA 3D Property or an Integer to use a Property already defined in model",
        GH_ParamAccess.list);
      pManager.AddIntegerParameter("Element3d Group", "Gr", "Set Element Group",
        GH_ParamAccess.list);
      pManager.AddTextParameter("Element3d Name", "Na", "Set Name of Element", GH_ParamAccess.list);
      pManager.AddColourParameter("Element3d Colour", "Co", "Set Element Colour",
        GH_ParamAccess.list);
      pManager.AddBooleanParameter("Dummy Element", "Dm", "Set Element to Dummy",
        GH_ParamAccess.list);

      for (int i = 1; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }

      pManager.HideParameter(0);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaElement3dParameter(), GsaElement3dGoo.Name,
        GsaElement3dGoo.NickName, GsaElement3dGoo.Description + " with applied changes.",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Number", "ID", "Get Element Number", GH_ParamAccess.list);
      pManager.AddMeshParameter("Analysis Mesh", "M",
        "Get Analysis Mesh. " + Environment.NewLine
        + "This will export a list of solid meshes representing each 3D element."
        + Environment.NewLine
        + "To get a combined mesh connect a GSA Element 3D to normal Mesh Parameter component to convert on the fly",
        GH_ParamAccess.item);
      pManager.HideParameter(2);
      pManager.AddParameter(new GsaProp3dParameter(), "3D Property", "PV",
        "Get 3D Property. Either a GSA 3D Property or an Integer representing a Property already defined in model",
        GH_ParamAccess.list);
      pManager.AddIntegerParameter("Group", "Gr", "Get Element Group", GH_ParamAccess.list);
      pManager.AddTextParameter("Element Type", "eT",
        "Get Element 3D Type." + Environment.NewLine
        + "Type can not be set; it is either Tetra4, Pyramid5, Wedge6 or Brick8",
        GH_ParamAccess.list);
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
      var gsaElement3d = new GsaElement3d();
      if (!da.GetData(0, ref gsaElement3d)) {
        return;
      }

      if (gsaElement3d == null) {
        this.AddRuntimeWarning("Element3D input is null");
      }

      GsaElement3d elem = gsaElement3d.Duplicate(true);

      var ghId = new List<GH_Integer>();
      var inIds = new List<int>();
      if (da.GetDataList(1, ghId)) {
        for (int i = 0; i < ghId.Count; i++) {
          if (i > elem.ApiElements.Count) {
            this.AddRuntimeWarning("ID input List Length is longer than number of elements."
              + Environment.NewLine + "Excess ID's have been ignored");
            continue;
          }

          if (!GH_Convert.ToInt32(ghId[i], out int id, GH_Conversion.Both)) {
            continue;
          }

          if (inIds.Contains(id)) {
            if (id > 0) {
              this.AddRuntimeWarning("ID input(" + i + ") = " + id
                + " already exist in your input list." + Environment.NewLine
                + "You must provide a list of unique IDs, or set ID = 0 if you want to let GSA handle the numbering");
              continue;
            }
          }

          inIds.Add(id);
        }

        elem.Ids = inIds;
      }

      var ghTypes = new List<GH_ObjectWrapper>();
      if (da.GetDataList(2, ghTypes)) {
        var prop3Ds = new List<GsaProp3d>();
        for (int i = 0; i < ghTypes.Count; i++) {
          if (i > elem.ApiElements.Count) {
            this.AddRuntimeWarning("PA input List Length is longer than number of elements."
              + Environment.NewLine + "Excess PA's have been ignored");
          }

          GH_ObjectWrapper ghTyp = ghTypes[i];
          var prop3d = new GsaProp3d();
          if (ghTyp.Value is GsaProp3dGoo prop3DGoo) {
            prop3Ds.Add(prop3DGoo.Value.Duplicate());
          } else {
            if (GH_Convert.ToInt32(ghTyp.Value, out int id, GH_Conversion.Both)) {
              prop3Ds.Add(new GsaProp3d(id));
            } else {
              this.AddRuntimeError(
                "Unable to convert PA input to a 2D Property of reference integer");
              return;
            }
          }
        }

        elem.Prop3ds = prop3Ds;
      }

      var ghgrp = new List<GH_Integer>();
      if (da.GetDataList(3, ghgrp)) {
        var inGroups = new List<int>();
        for (int i = 0; i < ghgrp.Count; i++) {
          if (i > elem.ApiElements.Count) {
            this.AddRuntimeWarning("Group input List Length is longer than number of elements."
              + Environment.NewLine + "Excess Group numbers have been ignored");
            continue;
          }

          if (GH_Convert.ToInt32(ghgrp[i], out int grp, GH_Conversion.Both)) {
            inGroups.Add(grp);
          }
        }

        elem.Groups = inGroups;
      }

      var ghStrings = new List<GH_String>();
      if (da.GetDataList(4, ghStrings)) {
        var inNames = new List<string>();
        for (int i = 0; i < ghStrings.Count; i++) {
          if (i > elem.ApiElements.Count) {
            this.AddRuntimeWarning("Name input List Length is longer than number of elements."
              + Environment.NewLine + "Excess Names have been ignored");
            continue;
          }

          if (GH_Convert.ToString(ghStrings[i], out string name, GH_Conversion.Both)) {
            inNames.Add(name);
          }
        }

        elem.Names = inNames;
      }

      var ghColours = new List<GH_Colour>();
      if (da.GetDataList(5, ghColours)) {
        var inColours = new List<Color>();
        for (int i = 0; i < ghColours.Count; i++) {
          if (i > elem.ApiElements.Count) {
            this.AddRuntimeWarning("Colour input List Length is longer than number of elements."
              + Environment.NewLine + "Excess Colours have been ignored");
            continue;
          }

          if (GH_Convert.ToColor(ghColours[i], out Color col, GH_Conversion.Both)) {
            inColours.Add(col);
          }
        }

        elem.Colours = inColours;
      }

      var ghdum = new List<GH_Boolean>();
      if (da.GetDataList(6, ghdum)) {
        var inDummies = new List<bool>();
        for (int i = 0; i < ghdum.Count; i++) {
          if (i > elem.ApiElements.Count) {
            this.AddRuntimeWarning("Dummy input List Length is longer than number of elements."
              + Environment.NewLine + "Excess Dummy booleans have been ignored");
            continue;
          }

          if (GH_Convert.ToBoolean(ghdum[i], out bool dum, GH_Conversion.Both)) {
            inDummies.Add(dum);
          }
        }

        elem.IsDummies = inDummies;
      }

      var outMeshes = new List<Mesh>();
      Mesh x = elem.NgonMesh;

      var ngons = x.GetNgonAndFacesEnumerable().ToList();

      foreach (MeshNgon ngon in ngons) {
        var m = new Mesh();
        m.Vertices.AddVertices(x.Vertices.ToList());
        var faceindex = ngon.FaceIndexList().Select(u => (int)u).ToList();
        foreach (int index in faceindex) {
          m.Faces.AddFace(x.Faces[index]);
        }

        m.Vertices.CullUnused();
        m.RebuildNormals();
        outMeshes.Add(m);
      }

      da.SetData(0, new GsaElement3dGoo(elem));
      da.SetDataList(1, elem.Ids);
      da.SetDataList(2, outMeshes);
      da.SetDataList(3,
        new List<GsaProp3dGoo>(elem.Prop3ds.ConvertAll(prop3d => new GsaProp3dGoo(prop3d))));
      da.SetDataList(4, elem.Groups);
      da.SetDataList(5, elem.Types);
      da.SetDataList(6, elem.Names);
      da.SetDataList(7, elem.Colours);
      da.SetDataList(8, elem.IsDummies);
      da.SetDataList(9, elem.ParentMembers);
      da.SetDataTree(10, elem.TopologyIDs);
    }
  }
}
