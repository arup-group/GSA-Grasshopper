using System;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

using Rhino.Geometry;

using EntityType = GsaGH.Parameters.EntityType;

namespace GsaGH.Components {
  public class CreateGravityLoad : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("f9099874-92fa-4608-b4ed-a788df85a407");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateGravityLoad;

    public CreateGravityLoad() : base("Create Gravity Load", "GravityLoad",
      "Create GSA Gravity Load", CategoryName.Name(), SubCategoryName.Cat3()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaLoadCaseParameter());
      pManager.AddGenericParameter("Loadable Objects", "Geo",
        "Lists, Custom Materials, Properties, Elements or Members to apply load to; either input Section, Prop2d, Prop3d, Element1d, Element2d, Member1d, Member2d or Member3d, or a text string."
        + Environment.NewLine + "Text string with Element list should take the form:"
        + Environment.NewLine
        + " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)"
        + Environment.NewLine
        + "Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Load Name", GH_ParamAccess.item);
      pManager.AddVectorParameter("Gravity factor", "G", "Gravity vector factor (default z = -1)",
        GH_ParamAccess.item, new Vector3d(0, 0, -1));
      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaLoadParameter(), "Gravity load", "Ld", "GSA Gravity Load",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var gravityLoad = new GsaGravityLoad();

      var loadcase = new GsaLoadCase(1);
      GsaLoadCaseGoo loadCaseGoo = null;
      if (da.GetData(0, ref loadCaseGoo)) {
        if (loadCaseGoo.Value != null) {
          loadcase = loadCaseGoo.Value;
        }
      }

      gravityLoad.LoadCase = loadcase;

      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(1, ref ghTyp)) {
        gravityLoad.ApiLoad.EntityList = string.Empty;
        if (ghTyp.Value is GsaElement1dGoo goo) {
          gravityLoad.RefObjectGuid = goo.Value.Guid;
          gravityLoad.ReferenceType = ReferenceType.Element;
        }

        switch (ghTyp.Value) {
          case GsaListGoo value:
            if (value.Value.EntityType == EntityType.Element
              || value.Value.EntityType == EntityType.Member) {
              gravityLoad.ReferenceList = value.Value;
              gravityLoad.ReferenceType = ReferenceType.List;
            } else {
              this.AddRuntimeWarning(
                "List must be of type Element or Member to apply to beam loading");
            }
            break;

          case GsaElement1dGoo value:
            gravityLoad.RefObjectGuid = value.Value.Guid;
            gravityLoad.ReferenceType = ReferenceType.Element;
            gravityLoad.ApiLoad.EntityType = GsaAPI.EntityType.Element;
            break;

          case GsaElement2dGoo value:
            gravityLoad.RefObjectGuid = value.Value.Guid;
            gravityLoad.ReferenceType = ReferenceType.Element;
            gravityLoad.ApiLoad.EntityType = GsaAPI.EntityType.Element;
            break;

          case GsaMember1dGoo value:
            gravityLoad.RefObjectGuid = value.Value.Guid;
            gravityLoad.ReferenceType = ReferenceType.Member;
            gravityLoad.ApiLoad.EntityType = GsaAPI.EntityType.Member;
            break;

          case GsaMember2dGoo value:
            gravityLoad.RefObjectGuid = value.Value.Guid;
            gravityLoad.ReferenceType = ReferenceType.Member;
            gravityLoad.ApiLoad.EntityType = GsaAPI.EntityType.Member;
            break;

          case GsaMember3dGoo value:
            gravityLoad.RefObjectGuid = value.Value.Guid;
            gravityLoad.ReferenceType = ReferenceType.Member;
            gravityLoad.ApiLoad.EntityType = GsaAPI.EntityType.Member;
            break;

          case GsaMaterialGoo value:
            if (value.Value.Id != 0) {
              this.AddRuntimeWarning(
              "Reference Material must be a Custom Material");
              return;
            }
            gravityLoad.RefObjectGuid = value.Value.Guid;
            gravityLoad.ReferenceType = ReferenceType.Property;
            gravityLoad.ApiLoad.EntityType = GsaAPI.EntityType.Element;
            break;

          case GsaSectionGoo value:
            gravityLoad.RefObjectGuid = value.Value.Guid;
            gravityLoad.ReferenceType = ReferenceType.Property;
            gravityLoad.ApiLoad.EntityType = GsaAPI.EntityType.Element;
            break;

          case GsaProperty2dGoo value:
            gravityLoad.RefObjectGuid = value.Value.Guid;
            gravityLoad.ReferenceType = ReferenceType.Property;
            gravityLoad.ApiLoad.EntityType = GsaAPI.EntityType.Element;
            break;

          case GsaProperty3dGoo value:
            gravityLoad.RefObjectGuid = value.Value.Guid;
            gravityLoad.ReferenceType = ReferenceType.Property;
            gravityLoad.ApiLoad.EntityType = GsaAPI.EntityType.Element;
            break;

          default:
            if (GH_Convert.ToString(ghTyp.Value, out string elemList, GH_Conversion.Both)) {
              gravityLoad.ApiLoad.EntityType = GsaAPI.EntityType.Element;
              gravityLoad.ApiLoad.EntityList = elemList;
              if (gravityLoad.ApiLoad.EntityList != elemList && elemList.ToLower() != "all") {
                gravityLoad.ApiLoad.EntityList = $"\"{elemList}\"";
              }
            }

            break;

        }
      } else {
        gravityLoad.ApiLoad.EntityList = "All";
      }

      var ghName = new GH_String();
      if (da.GetData(2, ref ghName)) {
        if (GH_Convert.ToString(ghName, out string name, GH_Conversion.Both)) {
          gravityLoad.ApiLoad.Name = name;
        }
      }

      var vect = new Vector3d(0, 0, -1);
      var ghFactor = new GH_Vector();
      if (da.GetData(3, ref ghFactor)) {
        GH_Convert.ToVector3d(ghFactor, ref vect, GH_Conversion.Both);
      }

      var factor = new GsaAPI.Vector3() {
        X = vect.X,
        Y = vect.Y,
        Z = vect.Z,
      };

      if (vect.Z > 0) {
        this.AddRuntimeRemark(
          "Just a friendly reminder that your gravity vector is pointing upwards and that is not normal.");
      }

      gravityLoad.ApiLoad.Factor = factor;

      da.SetData(0, new GsaLoadGoo(gravityLoad));
    }
  }
}
