using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components
{
    public class EditGsaTitles_OBSOLETE : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("72a2666a-aa89-47a5-a922-5e63fc9cd966");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Titles;

    public EditGsaTitles_OBSOLETE()
      : base("Edit GSA Titles", "Title", "Set GSA Titles for this document",
            CategoryName.Name(),
            SubCategoryName.Cat0())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Custom UI
    //This region overrides the typical component layout


    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddTextParameter("Job Number", "JN", "Set Job Number for this GSA Model", GH_ParamAccess.item);
      pManager.AddTextParameter("Initials", "Ini", "Set Initials for this GSA Model", GH_ParamAccess.item);
      pManager.AddTextParameter("Title", "Tit", "Set Title for this GSA Model", GH_ParamAccess.item);
      pManager.AddTextParameter("Sub Title", "Sub", "Set Sub Title for this GSA Model", GH_ParamAccess.item);
      pManager.AddTextParameter("Calculation", "Cal", "Set Calculation Heading for this GSA Model", GH_ParamAccess.item);
      pManager.AddTextParameter("Notes", "Nt", "Set Notes for this GSA Model", GH_ParamAccess.item);
      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddTextParameter("Titles", "Titles", "List of all Titles in document", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      this.AddRuntimeRemark("It is currently not possible to set titles back into a GSA model."
          + Environment.NewLine + "Any chances made here will not be reflected in a your model."
          + Environment.NewLine + "You can currently use this to get information from an existing GSA model;"
          + Environment.NewLine + "when opening a model the values in this component will be set automatically");


      GH_String ghstr = new GH_String();
      if (DA.GetData(0, ref ghstr))
      {
        if (GH_Convert.ToString(ghstr, out string title, GH_Conversion.Both))
        {
          Titles.SetJobNumber(title);
        }
      }

      ghstr = new GH_String();
      if (DA.GetData(1, ref ghstr))
      {
        if (GH_Convert.ToString(ghstr, out string title, GH_Conversion.Both))
        {
          Titles.SetInitials(title);
        }
      }
      ghstr = new GH_String();
      if (DA.GetData(2, ref ghstr))
      {
        if (GH_Convert.ToString(ghstr, out string title, GH_Conversion.Both))
        {
          Titles.SetTitle(title);
        }
      }
      ghstr = new GH_String();
      if (DA.GetData(3, ref ghstr))
      {
        if (GH_Convert.ToString(ghstr, out string title, GH_Conversion.Both))
        {
          Titles.SetSubTitle(title);
        }
      }
      ghstr = new GH_String();
      if (DA.GetData(4, ref ghstr))
      {
        if (GH_Convert.ToString(ghstr, out string title, GH_Conversion.Both))
        {
          Titles.SetCalculation(title);
        }
      }
      ghstr = new GH_String();
      if (DA.GetData(5, ref ghstr))
      {
        if (GH_Convert.ToString(ghstr, out string title, GH_Conversion.Both))
        {
          Titles.SetNotes(title);
        }
      }

      List<string> titles = new List<string>
            {
                "Job Number: " + Titles.JobNumber,
                "Initials: " + Titles.Initials,
                "Title: " + Titles.Title,
                "Sub Title: " + Titles.SubTitle,
                "Calculation Header: " + Titles.Calculation,
                "Notes: " + Titles.Notes
            };

      DA.SetDataList(0, titles);

    }
  }
}

