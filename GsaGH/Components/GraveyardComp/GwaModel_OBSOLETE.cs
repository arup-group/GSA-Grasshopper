using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  /// Component to create a GSA model from GWA string
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class GwaModel_OBSOLETE : GH_OasysComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("6f701c53-1531-45ef-9842-9356da59b590");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GwaModel;

    public GwaModel_OBSOLETE()
      : base("Create GWA Model", "GWA", "Create a model from a GWA string.",
        CategoryName.Name(),
        SubCategoryName.Cat0()) {
          Hidden = true;
    } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddTextParameter("GWA string", "GWA", "GWA string from GSA. Right-click on any data, and select copy all. Paste into notepad to check the data. \r\nThis input takes a a list of text strings that will automatically be joined. Construct a tree structure if you want to create multiple GSA files. \r\nThe syntax of the command is based on GWA syntax and the units follow the GWA unit syntax; –\r\nrefer to the “GSA Keywords” document for details", GH_ParamAccess.list);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var m = new Interop.Gsa_10_1.ComAuto();
      m.NewFile();
      string gwa = "";
      var strings = new List<string>();
      if (da.GetDataList(0, strings)) gwa = strings.Aggregate(gwa, (current, s) => current + (s + "\n"));

      m.GwaCommand(gwa);
      string temp = Path.GetTempPath() + Guid.NewGuid() + ".gwb";
      m.SaveAs(temp);
      var gsaGh = new GsaModel();
      gsaGh.Model.Open(temp);
      da.SetData(0, gsaGh);
    }
  }
}
