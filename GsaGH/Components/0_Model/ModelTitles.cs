using System;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  // ReSharper disable once InconsistentNaming
  public class ModelTitles : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("a0e55eed-fc36-4919-8107-9caad67d5681");
    public override GH_Exposure Exposure => GH_Exposure.septenary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.ModelTitles;

    public ModelTitles() : base("Model Titles", "Titles",
      "Get or set the titles in a GSA Model", CategoryName.Name(), SubCategoryName.Cat0()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA",
        "Existing GSA model to get or set titles for.",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Job Number", "JN", "Set Job Number for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Initials", "Ini", "Set Initials for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Title", "Tit", "Set Title for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Sub Title", "Sub", "Set Sub Title for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Calculation", "Cal", "Set Calculation Heading for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Notes", "Nt", "Set Notes for this GSA Model", GH_ParamAccess.item);
      for (int i = 1; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter());
      pManager.AddTextParameter("Job Number", "JN", "Get Job Number for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Initials", "Ini", "Get Initials for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Title", "Tit", "Get Title for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Sub Title", "Sub", "Get Sub Title for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Calc Header", "CH", "Get Calculation Heading for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Notes", "Nt", "Get Notes for this GSA Model", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaModelGoo gooModel = null;
      GsaModel model = null;
      if (da.GetData(0, ref gooModel)) {
        model = gooModel.Value.Clone();
      }
      GsaAPI.Titles titles = model.Titles;

      var ghString = new GH_String();
      if (da.GetData(1, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string jobNumber, GH_Conversion.Both)) {
          titles.JobNumber = jobNumber;
        }
      }

      ghString = new GH_String();
      if (da.GetData(2, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string initials, GH_Conversion.Both)) {
          titles.Initials = initials;
        }
      }

      ghString = new GH_String();
      if (da.GetData(3, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string title, GH_Conversion.Both)) {
          titles.Title = title;
        }
      }

      ghString = new GH_String();
      if (da.GetData(4, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string subTitle, GH_Conversion.Both)) {
          titles.SubTitle = subTitle;
        }
      }

      ghString = new GH_String();
      if (da.GetData(5, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string calcHeader, GH_Conversion.Both)) {
          titles.Calculation = calcHeader;
        }
      }

      ghString = new GH_String();
      if (da.GetData(6, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string notes, GH_Conversion.Both)) {
          titles.Notes = notes;
        }
      }

      da.SetData(0, new GsaModelGoo(model));
      da.SetData(1, titles.JobNumber);
      da.SetData(2, titles.Initials);
      da.SetData(3, titles.Title);
      da.SetData(4, titles.SubTitle);
      da.SetData(5, titles.Calculation);
      da.SetData(6, titles.Notes);
    }
  }
}
