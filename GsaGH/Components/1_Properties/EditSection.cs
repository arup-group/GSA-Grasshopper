using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  /// Component to edit a Section and ouput the information
  /// </summary>
  public class EditSection : GH_OasysComponent {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("28dcadbd-4735-4110-8c30-931b37ec5f5a");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.EditSection;

    public EditSection() : base("Edit Section",
      "SectionEdit",
      "Modify GSA Section",
      CategoryName.Name(),
      SubCategoryName.Cat1()) {
        Hidden = true;
    } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaSectionParameter(), GsaSectionGoo.Name, GsaSectionGoo.NickName, GsaSectionGoo.Description + " to get or set information for. Leave blank to create a new " + GsaSectionGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Section Number", "ID", "Set Section Number. If ID is set it will replace any existing 2D Property in the model", GH_ParamAccess.item);
      pManager.AddTextParameter("Section Profile", "Pf", "Profile name following GSA naming convention (eg 'STD I 1000 500 15 25')", GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter(), GsaMaterialGoo.Name, GsaMaterialGoo.NickName, "Set " + GsaMaterialGoo.Name, GH_ParamAccess.item);
      pManager.AddParameter(new GsaSectionModifierParameter(), GsaSectionModifierGoo.Name, GsaSectionModifierGoo.NickName, "Set " + GsaSectionModifierGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Section Pool", "Po", "Set Section pool", GH_ParamAccess.item);
      pManager.AddTextParameter("Section Name", "Na", "Set Section name", GH_ParamAccess.item);
      pManager.AddColourParameter("Section Colour", "Co", "Set Section colour", GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaSectionParameter(), GsaSectionGoo.Name, GsaSectionGoo.NickName, GsaSectionGoo.Description + " with applied changes.", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Section Number", "ID", "Original Section number (ID) if the Section ever belonged to a GSA Model", GH_ParamAccess.item);
      pManager.AddTextParameter("Section Profile", "Pf", "Profile describtion", GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter(), GsaMaterialGoo.Name, GsaMaterialGoo.NickName, "Get " + GsaMaterialGoo.Name, GH_ParamAccess.item);
      pManager.AddParameter(new GsaSectionModifierParameter(), GsaSectionModifierGoo.Name, GsaSectionModifierGoo.NickName, "Set " + GsaSectionModifierGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Section Pool", "Po", "Get Section pool", GH_ParamAccess.item);
      pManager.AddTextParameter("Section Name", "Na", "Get Section name", GH_ParamAccess.item);
      pManager.AddColourParameter("Section Colour", "Co", "Get Section colour", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var sect = new GsaSection();
      var gsaSection = new GsaSection();
      if (da.GetData(0, ref sect)) {
        gsaSection = sect.Duplicate();
      }

      if (gsaSection != null) {
        // #### input ####

        // 1 ID
        var ghId = new GH_Integer();
        if (da.GetData(1, ref ghId)) {
          if (GH_Convert.ToInt32(ghId, out int id, GH_Conversion.Both))
            gsaSection.Id = id;
        }

        // 2 profile
        string profile = "";
        if (da.GetData(2, ref profile))
          gsaSection.Profile = profile;

        // 3 Material
        var ghTyp = new GH_ObjectWrapper();
        if (da.GetData(3, ref ghTyp)) {
          if (ghTyp.Value is GsaMaterialGoo) {
            ghTyp.CastTo(out GsaMaterial material);
            gsaSection.Material = material ?? new GsaMaterial();
          }
          else {
            if (GH_Convert.ToInt32(ghTyp.Value, out int idd, GH_Conversion.Both))
              gsaSection.MaterialID = idd;
            else {
              this.AddRuntimeError("Unable to convert PB input to a Section Property of reference integer");
              return;
            }
          }
        }

        // 4 Section modifier
        ghTyp = new GH_ObjectWrapper();
        if (da.GetData(4, ref ghTyp)) {
          if (ghTyp.Value is GsaSectionModifierGoo) {
            ghTyp.CastTo(out GsaSectionModifier modifier);
            gsaSection.Modifier = modifier ?? new GsaSectionModifier();
          }
        }

        // 5 section pool
        int pool = 0; //prop.Prop2d.Thickness;
        if (da.GetData(5, ref pool)) {
          gsaSection.Pool = pool;
        }

        // 6 name
        var ghnm = new GH_String();
        if (da.GetData(6, ref ghnm)) {
          if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both))
            gsaSection.Name = name;
        }

        // 7 Colour
        var ghcol = new GH_Colour();
        if (da.GetData(7, ref ghcol)) {
          if (GH_Convert.ToColor(ghcol, out System.Drawing.Color col, GH_Conversion.Both))
            gsaSection.Colour = col;
        }

        // #### outputs ####
        string prof = (gsaSection.API_Section == null) ? "--" : gsaSection.Profile;
        int poo = (gsaSection.API_Section == null) ? 0 : gsaSection.Pool;
        string nm = (gsaSection.API_Section == null) ? "--" : gsaSection.Name;
        ValueType colour = gsaSection.API_Section?.Colour;

        da.SetData(0, new GsaSectionGoo(gsaSection));
        da.SetData(1, gsaSection.Id);
        da.SetData(2, prof);
        da.SetData(3, new GsaMaterialGoo(new GsaMaterial(gsaSection))); // to implemented GsaMaterial
        da.SetData(4, new GsaSectionModifierGoo(gsaSection.Modifier));
        da.SetData(5, poo);
        da.SetData(6, nm);
        da.SetData(7, colour);
      }
      else
        this.AddRuntimeError("Section is Null");
    }
  }
}
