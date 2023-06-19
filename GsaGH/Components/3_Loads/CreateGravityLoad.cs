using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using Rhino.Geometry;
using EntityType = GsaGH.Parameters.EntityType;

namespace GsaGH.Components {
  public class CreateGravityLoad : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("f9099874-92fa-4608-b4ed-a788df85a407");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GravityLoad;

    public CreateGravityLoad() : base("Create Gravity Load", "GravityLoad",
      "Create GSA Gravity Load", CategoryName.Name(), SubCategoryName.Cat3()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddIntegerParameter("Load case", "LC", "Load case number (by default 1)",
        GH_ParamAccess.item, 1);
      pManager.AddGenericParameter("Element list", "El",
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
      int loadCase = 1;
      var ghLc = new GH_Integer();
      if (da.GetData(0, ref ghLc)) {
        GH_Convert.ToInt32(ghLc, out loadCase, GH_Conversion.Both);
      }

      gravityLoad.GravityLoad.Case = loadCase;

      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(1, ref ghTyp)) {
        gravityLoad.GravityLoad.Elements = string.Empty;
        if (ghTyp.Value is GsaElement1dGoo goo) {
          gravityLoad._refObjectGuid = goo.Value.Guid;
          gravityLoad._referenceType = ReferenceType.Element;
        }

        switch (ghTyp.Value) {
          case GsaListGoo value: {
              if (value.Value.EntityType == EntityType.Element
                || value.Value.EntityType == EntityType.Member) {
                gravityLoad._refList = value.Value;
                gravityLoad._referenceType = ReferenceType.List;
              } else {
                this.AddRuntimeWarning(
                  "List must be of type Element or Member to apply to beam loading");
              }

            if (value.Value.EntityType == EntityType.Member) {
              this.AddRuntimeRemark(
                "Member list applied to loading in GsaGH will automatically find child elements created from parent member with the load still being applied to elements." + Environment.NewLine + "If you save the file and continue working in GSA please note that the member-loading relationship will be lost.");
            }

              break;
            }
          case GsaElement2dGoo value: {
              gravityLoad._refObjectGuid = value.Value.Guid;
              gravityLoad._referenceType = ReferenceType.Element;
              break;
            }
          case GsaMember1dGoo value: {
            gravityLoad._refObjectGuid = value.Value.Guid;
            gravityLoad._referenceType = ReferenceType.MemberChildElements;
            this.AddRuntimeRemark(
              "Member loading in GsaGH will automatically find child elements created from parent member with the load still being applied to elements." + Environment.NewLine + "If you save the file and continue working in GSA please note that the member-loading relationship will be lost.");
            break;
          }
          case GsaMember2dGoo value: {
            gravityLoad._refObjectGuid = value.Value.Guid;
            gravityLoad._referenceType = ReferenceType.MemberChildElements;
            this.AddRuntimeRemark(
              "Member loading in GsaGH will automatically find child elements created from parent member with the load still being applied to elements." + Environment.NewLine + "If you save the file and continue working in GSA please note that the member-loading relationship will be lost.");
            break;
          }
          case GsaMember3dGoo value: {
            gravityLoad._refObjectGuid = value.Value.Guid;
            gravityLoad._referenceType = ReferenceType.MemberChildElements;
            this.AddRuntimeRemark(
              "Member loading in GsaGH will automatically find child elements created from parent member with the load still being applied to elements." + Environment.NewLine + "If you save the file and continue working in GSA please note that the member-loading relationship will be lost.");
            break;
          }
          case GsaMaterialGoo value: {
              if (value.Value.Id != 0) {
                this.AddRuntimeWarning(
                "Reference Material must be a Custom Material");
                return;
              }
              gravityLoad._refObjectGuid = value.Value.Guid;
              gravityLoad._referenceType = ReferenceType.Property;
              break;
            }
          case GsaSectionGoo value: {
              gravityLoad._refObjectGuid = value.Value.Guid;
              gravityLoad._referenceType = ReferenceType.Property;
              break;
            }
          case GsaProp2dGoo value: {
              gravityLoad._refObjectGuid = value.Value.Guid;
              gravityLoad._referenceType = ReferenceType.Property;
              break;
            }
          case GsaProp3dGoo value: {
              gravityLoad._refObjectGuid = value.Value.Guid;
              gravityLoad._referenceType = ReferenceType.Property;
              break;
            }
          default: {
              if (GH_Convert.ToString(ghTyp.Value, out string elemList, GH_Conversion.Both)) {
                gravityLoad.GravityLoad.Elements = elemList;
              }

              break;
            }
        }
      } else {
        gravityLoad.GravityLoad.Elements = "All";
      }

      var ghName = new GH_String();
      if (da.GetData(2, ref ghName)) {
        if (GH_Convert.ToString(ghName, out string name, GH_Conversion.Both)) {
          gravityLoad.GravityLoad.Name = name;
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
          "Just a friendly note that your gravity vector is pointing upwards and that is not normal.");
      }

      gravityLoad.GravityLoad.Factor = factor;

      var gsaLoad = new GsaLoad(gravityLoad);
      da.SetData(0, new GsaLoadGoo(gsaLoad));
    }
  }
}
