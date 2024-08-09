using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get GSA design task information
  /// </summary>
  public class DesignTaskInfo : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("d5afc8d1-cf31-451c-846d-55612c438ad5");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.DesignTaskInfo;

    public DesignTaskInfo() : base("Design Task Info", "DesignTaskInfo", "Get GSA Steel Design Tasks information",
      CategoryName.Name(), SubCategoryName.Cat4()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaDesignTaskParameter());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddTextParameter("Name", "Na", "Task Name", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Number", "ID",
        "Set Task Number. If ID is set it will replace any existing DesignTasks in the model",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaMemberListParameter(), "Definition", "Def",
        "Members List definition", GH_ParamAccess.item);
      pManager.AddIntegerParameter("CombinationCase", "CC", "Combination Case ID",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Target Utilisation", "η",
        "Target overall utilisation (upper)", GH_ParamAccess.item);
      pManager.AddNumberParameter("Lower limit", "ηₘᵢₙ",
        "Lower utilisation limit (inefficiency warning)", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Grouped Design", "Grp",
        "If true, Members with the same pool are assigned the same section", GH_ParamAccess.item);
      pManager.AddTextParameter("Primary Objective", "1st",
        "Primary design optimisation objective", GH_ParamAccess.item);
      pManager.AddTextParameter("Secondary Objective", "2nd",
        "Secondary design optimisation objective", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaSteelDesignTask dt;
      GsaDesignTaskGoo taskGoo = null;
      da.GetData(0, ref taskGoo);
      dt = (GsaSteelDesignTask)taskGoo.Value;
      da.SetData(0, dt.Name);
      da.SetData(1, dt.Id);
      if (dt.List != null) {
        da.SetData(2, new GsaListGoo(dt.List));
      } else {
        var list = new GsaList(dt.Name, dt.ApiTask.ListDefinition, GsaAPI.EntityType.Member);
        da.SetData(2, new GsaListGoo(list));
      }

      da.SetData(3, dt.ApiTask.CombinationCaseId);
      da.SetData(4, dt.ApiTask.UpperTargetUtilisationLimit);
      da.SetData(5, dt.ApiTask.LowerTargetUtilisationLimit);
      da.SetData(6, dt.ApiTask.GroupSectionsByPool);
      da.SetData(7, dt.ApiTask.PrimaryObjective);
      da.SetData(8, dt.ApiTask.SecondaryObjective);
    }
  }
}
