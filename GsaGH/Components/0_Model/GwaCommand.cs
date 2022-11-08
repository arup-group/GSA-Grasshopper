using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create a GSA model from GWA string
  /// </summary>
  public class GwaCommand : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("6f701c53-1531-45ef-9842-9356da59b590");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.GwaModel;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override string HtmlHelp_Source()
    {
      string help = "GOTO:https://docs.oasys-software.com/structural/gsa/references/comautomation.html#gwacommand-function";
      return help;
    }

    public GwaCommand() : base(
      "GWA Command",
      "GWA", "Create a model from a GWA string, inject data into a model using GWA command, or retrieve model data or results through a GWA command.",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat0())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA", "(Optional) Existing GSA model to inject GWA command(s) into. Leave this input empty to create a new GSA model from GWA string.", GH_ParamAccess.item);
      pManager[0].Optional = true;
      pManager.AddTextParameter("GWA string", "GWA", "GWA string from GSA. Right-click on any data, and select copy all. Paste into notepad to check the data. \r\nThis input takes a a list of text strings that will automatically be joined. Construct a tree structure if you want to create multiple GSA files. \r\nThe syntax of the command is based on GWA syntax and the units follow the GWA unit syntax; –\r\nRefer to the “GSA Keywords” document for details.\r\nNote that locale is set to use '.' as decimal separator.\r\nRight-click -> Help for further infor on GWA Commands.", GH_ParamAccess.list);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaModelParameter());
      pManager.AddGenericParameter("Returned result", "R", "The 'variant' return value from executing a GWA command issued to GSA. \r\nThe syntax of the command is based on GWA syntax and the units follow the GWA unit syntax; –\r\nRefer to the “GSA Keywords” document for details.\r\nNote that locale is set to use '.' as decimal separator.\r\nRight-click -> Help for further infor on GWA Commands.", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Interop.Gsa_10_1.ComAuto m = new Interop.Gsa_10_1.ComAuto();
      string temp = Path.GetTempPath() + Guid.NewGuid().ToString() + ".gwb";
      
      GsaModelGoo model = null;
      if (DA.GetData(0, ref model))
      {
        model.Value.Model.SaveAs(temp);
        m.Open(temp);
      }
      else
        m.NewFile();

      m.SetLocale(Interop.Gsa_10_1.Locale.LOC_EN_GB);

      string gwa = "";
      List<string> strings = new List<string>();
      if (DA.GetDataList(1, strings))
        foreach (string s in strings)
          gwa += s + "\n";

      DA.SetData(1, m.GwaCommand(gwa));
      m.SaveAs(temp);
      GsaModel gsaGH = new GsaModel();
      gsaGH.Model.Open(temp);
      DA.SetData(0, new GsaModelGoo(gsaGH));
      m.Close();
      m = null;
    }
  }
}
