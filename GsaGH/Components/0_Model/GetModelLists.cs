using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to retrieve EntityLists from a GSA model
  /// </summary>
  public class GetModelLists : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("43495cf4-f2eb-4b14-9b1a-5f91972546ca");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GetModelLists;

    public GetModelLists() : base("Get Model Lists", "GetLists", "Get Entity Lists from GSA model",
      CategoryName.Name(), SubCategoryName.Cat0()) {
      Hidden = true;
    }

    // sets the initial state of the component to hidden

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA",
        "GSA model containing some lists", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaListParameter(), "GSA List", "L", "Lists from GSA Model",
        GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaModelGoo modelGoo = null;
      da.GetData(0, ref modelGoo);
      List<GsaList> lists = modelGoo.Value.GetLists();
      da.SetDataList(0, lists.Select(x => new GsaListGoo(x)));
    }
  }
}
