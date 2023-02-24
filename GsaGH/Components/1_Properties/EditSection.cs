using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to edit a Section and ouput the information
  /// </summary>
  public class EditSection : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("28dcadbd-4735-4110-8c30-931b37ec5f5a");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.EditSection;

    public EditSection() : base("Edit Section",
      "SectionEdit",
      "Modify GSA Section",
      CategoryName.Name(),
      SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
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

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaSectionParameter(), GsaSectionGoo.Name, GsaSectionGoo.NickName, GsaSectionGoo.Description + " with applied changes.", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Section Number", "ID", "Original Section number (ID) if the Section ever belonged to a GSA Model", GH_ParamAccess.item);
      pManager.AddTextParameter("Section Profile", "Pf", "Profile describtion", GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter(), GsaMaterialGoo.Name, GsaMaterialGoo.NickName, "Get " + GsaMaterialGoo.Name, GH_ParamAccess.item);
      pManager.AddParameter(new GsaSectionModifierParameter(), GsaSectionModifierGoo.Name, GsaSectionModifierGoo.NickName, "Set " + GsaSectionModifierGoo .Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Section Pool", "Po", "Get Section pool", GH_ParamAccess.item);
      pManager.AddTextParameter("Section Name", "Na", "Get Section name", GH_ParamAccess.item);
      pManager.AddColourParameter("Section Colour", "Co", "Get Section colour", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaSection sect = new GsaSection();
      GsaSection gsaSection = new GsaSection();
      if (DA.GetData(0, ref sect))
      {
        gsaSection = sect.Duplicate();
      }

      if (gsaSection != null)
      {
        // #### input ####

        // 1 ID
        GH_Integer ghID = new GH_Integer();
        if (DA.GetData(1, ref ghID))
        {
          if (GH_Convert.ToInt32(ghID, out int id, GH_Conversion.Both))
            gsaSection.Id = id;
        }

        // 2 profile
        string profile = "";
        if (DA.GetData(2, ref profile))
          gsaSection.Profile = profile;

        // 3 Material
        GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
        if (DA.GetData(3, ref gh_typ))
        {
          GsaMaterial material = new GsaMaterial();
          if (gh_typ.Value is GsaMaterialGoo)
          {
            gh_typ.CastTo(ref material);
            gsaSection.Material = material;
          }
          else
          {
            if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
              gsaSection.MaterialID = idd;
            else
            {
              this.AddRuntimeError("Unable to convert PB input to a Section Property of reference integer");
              return;
            }
          }
        }

        // 4 Section modifier
        gh_typ = new GH_ObjectWrapper();
        if (DA.GetData(4, ref gh_typ))
        {
          GsaSectionModifier modifier = new GsaSectionModifier();
          if (gh_typ.Value is GsaSectionModifierGoo)
          {
            gh_typ.CastTo(ref modifier);
            gsaSection.Modifier = modifier;
          }
        }

        // 5 section pool
        int pool = 0; //prop.Prop2d.Thickness;
        if (DA.GetData(5, ref pool))
        {
          gsaSection.Pool = pool;
        }

        // 6 name
        GH_String ghnm = new GH_String();
        if (DA.GetData(6, ref ghnm))
        {
          if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both))
            gsaSection.Name = name;
        }

        // 7 Colour
        GH_Colour ghcol = new GH_Colour();
        if (DA.GetData(7, ref ghcol))
        {
          if (GH_Convert.ToColor(ghcol, out System.Drawing.Color col, GH_Conversion.Both))
            gsaSection.Colour = col;
        }

        // #### outputs ####
        string prof = (gsaSection.API_Section == null) ? "--" : gsaSection.Profile;
        int poo = (gsaSection.API_Section == null) ? 0 : gsaSection.Pool;
        string nm = (gsaSection.API_Section == null) ? "--" : gsaSection.Name;
        ValueType colour = (gsaSection.API_Section == null) ? null : gsaSection.API_Section.Colour;

        DA.SetData(0, new GsaSectionGoo(gsaSection));
        DA.SetData(1, gsaSection.Id);
        DA.SetData(2, prof);
        DA.SetData(3, new GsaMaterialGoo(new GsaMaterial(gsaSection))); // to implemented GsaMaterial
        DA.SetData(4, new GsaSectionModifierGoo(gsaSection.Modifier));
        DA.SetData(5, poo);
        DA.SetData(6, nm);
        DA.SetData(7, colour);
      }
      else
        this.AddRuntimeError("Section is Null");
    }
  }
}
