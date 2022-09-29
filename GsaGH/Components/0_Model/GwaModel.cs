using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Parameters;


namespace GsaGH.Components
{
  /// <summary>
  /// Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  public class GwaModel : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("6f701c53-1531-45ef-9842-9356da59b590");
    public GwaModel()
       : base("Create GWA Model", "GWA", "Create a model from a GWA string.",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat0())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.GwaModel;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddTextParameter("GWA string", "GWA", "GWA string from GSA. Right-click on any data, and select copy all. Paste into notepad to check the data. \r\nThis input takes a a list of text strings that will automatically be joined. Construct a tree structure if you want to create multiple GSA files. \r\nThe syntax of the command is based on GWA syntax and the units follow the GWA unit syntax; –\r\nrefer to the “GSA Keywords” document for details", GH_ParamAccess.list);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaModelParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Interop.Gsa_10_1.ComAuto m = new Interop.Gsa_10_1.ComAuto();
      m.NewFile();
      string gwa = "";
      List<string> strings = new List<string>();
      if (DA.GetDataList(0, strings))
        foreach (string s in strings)
          gwa += s + "\n";
      
      m.GwaCommand(gwa);
      string temp = Path.GetTempPath() + Guid.NewGuid().ToString() + ".gwb";
      m.SaveAs(temp);
      GsaModel gsaGH = new GsaModel();
      gsaGH.Model.Open(temp);
      DA.SetData(0, gsaGH);
    }
  }
}
