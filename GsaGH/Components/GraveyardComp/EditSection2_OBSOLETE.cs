﻿using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a Section and ouput the information
  /// </summary>
  public class EditSection2_OBSOLETE : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("28dcadbd-4735-4110-8c30-931b37ec5f5a");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditSection;

    public EditSection2_OBSOLETE() : base("Edit Section", "SectionEdit", "Modify GSA Section",
      CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaSectionParameter(), GsaSectionGoo.Name, GsaSectionGoo.NickName,
        GsaSectionGoo.Description + " to get or set information for. Leave blank to create a new "
        + GsaSectionGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Section Number", "ID",
        "Set Section Number. If ID is set it will replace any existing 2D Property in the model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Section Profile", "Pf",
        "Profile name following GSA naming convention (eg 'STD I 1000 500 15 25')",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter(), GsaMaterialGoo.Name,
        GsaMaterialGoo.NickName, "Set " + GsaMaterialGoo.Name, GH_ParamAccess.item);
      pManager.AddParameter(new GsaSectionModifierParameter(), GsaSectionModifierGoo.Name,
        GsaSectionModifierGoo.NickName, "Set " + GsaSectionModifierGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Section Pool", "Po", "Set Section pool", GH_ParamAccess.item);
      pManager.AddTextParameter("Section Name", "Na", "Set Section name", GH_ParamAccess.item);
      pManager.AddColourParameter("Section Colour", "Co", "Set Section colour",
        GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaSectionParameter(), GsaSectionGoo.Name, GsaSectionGoo.NickName,
        GsaSectionGoo.Description + " with applied changes.", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Section Number", "ID",
        "Original Section number (ID) if the Section ever belonged to a GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Section Profile", "Pf", "Profile describtion",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter(), GsaMaterialGoo.Name,
        GsaMaterialGoo.NickName, "Get " + GsaMaterialGoo.Name, GH_ParamAccess.item);
      pManager.AddParameter(new GsaSectionModifierParameter(), GsaSectionModifierGoo.Name,
        GsaSectionModifierGoo.NickName, "Set " + GsaSectionModifierGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Section Pool", "Po", "Get Section pool", GH_ParamAccess.item);
      pManager.AddTextParameter("Section Name", "Na", "Get Section name", GH_ParamAccess.item);
      pManager.AddColourParameter("Section Colour", "Co", "Get Section colour",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var section = new GsaSection();

      GsaSectionGoo sectionGoo = null;
      if (da.GetData(0, ref sectionGoo)) {
        section = sectionGoo.Value.Duplicate();
      }

      if (section != null) {
        var ghId = new GH_Integer();
        if (da.GetData(1, ref ghId)) {
          if (GH_Convert.ToInt32(ghId, out int id, GH_Conversion.Both)) {
            section.Id = id;
          }
        }

        string profile = "";
        if (da.GetData(2, ref profile)) {
          section.Profile = profile;
        }

        GsaMaterialGoo materialGoo = null;
        if (da.GetData(3, ref materialGoo)) {
          section.Material = materialGoo.Value;
        }

        GsaSectionModifierGoo modifierGoo = null;
        if (da.GetData(4, ref modifierGoo)) {
          section.Modifier = modifierGoo.Value;
        }

        int pool = 0;
        if (da.GetData(5, ref pool)) {
          section.Pool = pool;
        }

        var ghString = new GH_String();
        if (da.GetData(6, ref ghString)) {
          if (GH_Convert.ToString(ghString, out string name, GH_Conversion.Both)) {
            section.Name = name;
          }
        }

        var ghColour = new GH_Colour();
        if (da.GetData(7, ref ghColour)) {
          if (GH_Convert.ToColor(ghColour, out Color col, GH_Conversion.Both)) {
            section.Colour = col;
          }
        }

        string prof = (section.ApiSection == null) ? "--" : section.Profile;
        int poo = (section.ApiSection == null) ? 0 : section.Pool;
        string nm = (section.ApiSection == null) ? "--" : section.Name;
        ValueType colour = section.ApiSection?.Colour;

        da.SetData(0, new GsaSectionGoo(section));
        da.SetData(1, section.Id);
        da.SetData(2, prof);
        da.SetData(3, new GsaMaterialGoo(new GsaMaterial(section)));
        da.SetData(4, new GsaSectionModifierGoo(section.Modifier));
        da.SetData(5, poo);
        da.SetData(6, nm);
        da.SetData(7, colour);
      } else {
        this.AddRuntimeError("Section is Null");
      }
    }
  }
}