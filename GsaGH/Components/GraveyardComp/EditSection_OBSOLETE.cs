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
  // ReSharper disable once InconsistentNaming
  public class EditSection_OBSOLETE : GH_OasysComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("27dcadbd-4735-4110-8c30-931b37ec5f5a");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.EditSection;

    public EditSection_OBSOLETE()
      : base("Edit Section",
        "SectionEdit",
        "Modify GSA Section",
        CategoryName.Name(),
        SubCategoryName.Cat1()) {
          Hidden = true;
    } // sets the initial state of the component to hidden
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Section", "PB", "GSA Section to get or set information for", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Section Number", "ID", "Set Section Number. If ID is set it will replace any existing 2D Property in the model", GH_ParamAccess.item);
      pManager.AddTextParameter("Section Profile", "Pf", "Profile name following GSA naming convention (eg 'STD I 1000 500 15 25')", GH_ParamAccess.item);
      pManager.AddGenericParameter("Material", "Ma", "Set GSA Material or reference existing material by ID", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Section Pool", "Po", "Set Section pool", GH_ParamAccess.item);
      pManager.AddTextParameter("Section Name", "Na", "Set Section name", GH_ParamAccess.item);
      pManager.AddColourParameter("Section Colour", "Co", "Set Section colour", GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("Section", "PB", "GSA Section with changes", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Section Number", "ID", "Original Section number (ID) if Section ever belonged to a GSA Model", GH_ParamAccess.item);
      pManager.AddTextParameter("Section Profile", "Pf", "Profile describtion", GH_ParamAccess.item);
      pManager.AddGenericParameter("Material", "Ma", "GSA Material", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Section Pool", "Po", "Section pool", GH_ParamAccess.item);
      pManager.AddTextParameter("Section Name", "Na", "Section name", GH_ParamAccess.item);
      pManager.AddColourParameter("Section Colour", "Co", "Section colour", GH_ParamAccess.item);

    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var sect = new GsaSection();
      var gsaSection = new GsaSection();
      if (da.GetData(0, ref sect)) {
        gsaSection = sect.Duplicate();
      }

      if (gsaSection == null) {
        return;
      }
      var ghId = new GH_Integer();
      if (da.GetData(1, ref ghId)) {
        if (GH_Convert.ToInt32(ghId, out int id, GH_Conversion.Both))
          gsaSection.Id = id;
      }

      string profile = "";
      if (da.GetData(2, ref profile))
        gsaSection.Profile = profile;

      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(3, ref ghTyp)) {
        var material = new GsaMaterial();
        if (ghTyp.Value is GsaMaterialGoo) {
          ghTyp.CastTo(ref material);
          gsaSection.Material = material;
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
      int pool = 0; //prop.Prop2d.Thickness;
      if (da.GetData(4, ref pool)) {
        gsaSection.Pool = pool;
      }

      var ghString = new GH_String();
      if (da.GetData(5, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string name, GH_Conversion.Both))
          gsaSection.Name = name;
      }

      var ghColour = new GH_Colour();
      if (da.GetData(6, ref ghColour)) {
        if (GH_Convert.ToColor(ghColour, out System.Drawing.Color col, GH_Conversion.Both))
          gsaSection.Colour = col;
      }

      string prof = (gsaSection.API_Section == null) ? "--" : gsaSection.Profile;
      int poo = (gsaSection.API_Section == null) ? 0 : gsaSection.Pool;
      string nm = (gsaSection.API_Section == null) ? "--" : gsaSection.Name;
      ValueType colour = gsaSection.API_Section?.Colour;

      da.SetData(0, new GsaSectionGoo(gsaSection));
      da.SetData(1, gsaSection.Id);
      da.SetData(2, prof);
      da.SetData(3, new GsaMaterialGoo(new GsaMaterial(gsaSection))); // to implemented GsaMaterial
      da.SetData(4, poo);
      da.SetData(5, nm);
      da.SetData(6, colour);
    }
  }
}

