using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits.Units;


namespace GsaGH.Components
{
  /// <summary>
  /// Component to create a GSA model from GWA string
  /// </summary>
  public class GwaModel : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("6f701c53-1531-45ef-9842-9356da59b590");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.GwaModel;
    public GwaModel()
       : base("Create GWA Model", "GWA", "Create a model from a GWA string.",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat0())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddTextParameter("GWA string", "GWA", "GWA string from GSA. Right-click on any data, and select copy all. Paste into notepad to check the data. \r\nThis input takes a a list of text strings that will automatically be joined. Construct a tree structure if you want to create multiple GSA files. \r\nThe syntax of the command is based on GWA syntax and the units follow the GWA unit syntax; –\r\nrefer to the “GSA Keywords” document for details", GH_ParamAccess.list);
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA", "(Optional) Existing GSA model to inject GWA command(s) into. Leave this input empty to create a new GSA model from GWA string.", GH_ParamAccess.item);
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaModelParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Interop.Gsa_10_1.ComAuto m = new Interop.Gsa_10_1.ComAuto();
      string temp = Path.GetTempPath() + Guid.NewGuid().ToString() + ".gwb";
      
      GsaModelGoo model = new GsaModelGoo();
      if (DA.GetData(1, ref model))
      {
        model.Value.Model.SaveAs(temp);
        m.Open(temp);
      }
      else
        m.NewFile();
      
      string gwa = "";
      List<string> strings = new List<string>();
      if (DA.GetDataList(0, strings))
        foreach (string s in strings)
          gwa += s + "\n";
      
      m.GwaCommand(gwa);
      m.SaveAs(temp);
      GsaModel gsaGH = new GsaModel();
      gsaGH.Model.Open(temp);
      DA.SetData(0, new GsaModelGoo(gsaGH));
    }
  }
}
