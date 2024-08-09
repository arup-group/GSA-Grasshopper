using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  public class SteelSectionPoolNames : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("6df7c85e-f629-47fd-ab38-7431407777e3");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.SteelSectionPoolNames;

    public SteelSectionPoolNames() : base("Steel Section Pool Names", "Pool Names",
      "Get or set the Steel Section Pool Names of a GSA Model", CategoryName.Name(),
      SubCategoryName.Cat0()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA",
        "Existing GSA model to get or set the Steel Section Pool Names for.",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("List of IDs", "IDs", "List of IDs for Steel Section Pool Names", GH_ParamAccess.list);
      pManager.AddTextParameter("Names", "Na", "List of Names for each Steel Section Pool Name ID)",
        GH_ParamAccess.list);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter());
      pManager.AddIntegerParameter("List of IDs", "IDs", "List of IDs for Steel Section Pool Names", GH_ParamAccess.list);
      pManager.AddTextParameter("Names", "Na", "List of Names for each section pool name ID)",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaSectionParameter());
      pManager[3].Access = GH_ParamAccess.tree;
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaModelGoo gooModel = null;
      GsaModel model = null;
      if (da.GetData(0, ref gooModel)) {
        model = gooModel.Value.Clone();
      }

      var ids = new List<int>();
      var ghTypes = new List<GH_ObjectWrapper>();
      if (da.GetDataList(1, ghTypes)) {
        foreach (GH_ObjectWrapper wrapper in ghTypes) {
          if (GH_Convert.ToInt32(wrapper?.Value, out int id, GH_Conversion.Both)) {
            ids.Add(id);
          }
        }
      }

      var sectionPools = new Dictionary<int, string>();
      ghTypes = new List<GH_ObjectWrapper>();
      if (da.GetDataList(2, ghTypes)) {
        if (ids.Count < ghTypes.Count) {
          this.AddRuntimeError("Number of IDs does is smaller than number of names");
          return;
        }
        if (ids.Count != ghTypes.Count) {
          this.AddRuntimeWarning("Number of IDs does not match number of names, skipped excess IDs/names");
        }

        int i = 0;
        foreach (GH_ObjectWrapper wrapper in ghTypes) {
          if (GH_Convert.ToString(wrapper, out string name, GH_Conversion.Both)) {
            sectionPools.Add(ids[i], name);
          }
          i++;
        }
      }

      if (ids.Count > 0) {
        model.ApiModel.SetSteelSectionPools(new ReadOnlyDictionary<int, string>(sectionPools));
      }

      var tree = new DataTree<GsaSectionGoo>();
      ReadOnlyDictionary<int, GsaSectionGoo> sections = model.Sections;
      ReadOnlyDictionary<int, string> modelSectionPools = model.ApiModel.SteelSectionPools();
      foreach (int id in modelSectionPools.Keys) {
        var poolSections = new List<GsaSectionGoo>();
        foreach (GsaSectionGoo section in sections.Values) {
          if (section.Value.ApiSection.Pool == id) {
            poolSections.Add(section);
          }
        }
        tree.AddRange(poolSections, new GH_Path(id));
      }

      da.SetData(0, new GsaModelGoo(model));
      da.SetDataList(1, modelSectionPools.Keys);
      da.SetDataList(2, modelSectionPools.Values);
      da.SetDataTree(3, tree);
    }
  }
}
