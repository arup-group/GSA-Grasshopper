using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  // ReSharper disable once InconsistentNaming
  public class EditGsaTitles_OBSOLETE : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("72a2666a-aa89-47a5-a922-5e63fc9cd966");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Titles;

    public EditGsaTitles_OBSOLETE() : base("Edit GSA Titles",
          "Title",
      "Set GSA Titles for this document",
      CategoryName.Name(),
      SubCategoryName.Cat0()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddTextParameter("Job Number",
        "JN",
        "Set Job Number for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Initials",
        "Ini",
        "Set Initials for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Title",
        "Tit",
        "Set Title for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Sub Title",
        "Sub",
        "Set Sub Title for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Calculation",
        "Cal",
        "Set Calculation Heading for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Notes", "Nt", "Set Notes for this GSA Model", GH_ParamAccess.item);
      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i]
          .Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddTextParameter("Titles",
                                                                                         "Titles",
                                                                                         "List of all Titles in document",
                                                                                         GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      this.AddRuntimeRemark("It is currently not possible to set titles back into a GSA model."
        + Environment.NewLine
        + "Any chances made here will not be reflected in a your model."
        + Environment.NewLine
        + "You can currently use this to get information from an existing GSA model;"
        + Environment.NewLine
        + "when opening a model the values in this component will be set automatically");

      var ghString = new GH_String();
      if (da.GetData(0, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string title, GH_Conversion.Both)) {
          Titles.SetJobNumber(title);
        }
      }

      ghString = new GH_String();
      if (da.GetData(1, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string title, GH_Conversion.Both)) {
          Titles.SetInitials(title);
        }
      }

      ghString = new GH_String();
      if (da.GetData(2, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string title, GH_Conversion.Both)) {
          Titles.SetTitle(title);
        }
      }

      ghString = new GH_String();
      if (da.GetData(3, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string title, GH_Conversion.Both)) {
          Titles.SetSubTitle(title);
        }
      }

      ghString = new GH_String();
      if (da.GetData(4, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string title, GH_Conversion.Both)) {
          Titles.SetCalculation(title);
        }
      }

      ghString = new GH_String();
      if (da.GetData(5, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string title, GH_Conversion.Both)) {
          Titles.SetNotes(title);
        }
      }

      var titles = new List<string> {
        "Job Number: " + Titles.JobNumber,
        "Initials: " + Titles.Initials,
        "Title: " + Titles.Title,
        "Sub Title: " + Titles.SubTitle,
        "Calculation Header: " + Titles.Calculation,
        "Notes: " + Titles.Notes,
      };

      da.SetDataList(0, titles);
    }
  }
}
