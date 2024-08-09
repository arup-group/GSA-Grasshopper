using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  public class CreateSteelSectionPool : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("dd0195f5-48cd-467a-8b8c-3e88e1b695f5");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateSteelSectionPool;

    public CreateSteelSectionPool() : base("Create Steel Section Pool", "Section Pool",
      "Create a GSA Steel Section Pool", CategoryName.Name(),
      SubCategoryName.Cat4()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddIntegerParameter("Section Pool ID", "PID", "Gsa Steel Section Pool ID", GH_ParamAccess.item);
      pManager.AddParameter(new GsaSectionParameter());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaSectionParameter());
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      int pid = 0;
      da.GetData(0, ref pid);

      GsaSectionGoo sectionGoo = null;
      da.GetData(1, ref sectionGoo);
      var section = new GsaSection(sectionGoo.Value);
      section.ApiSection.Pool = pid;
      da.SetData(0, new GsaSectionGoo(section));
    }
  }
}
