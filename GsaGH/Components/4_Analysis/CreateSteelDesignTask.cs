using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;

using GsaAPI;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

using EntityType = GsaGH.Parameters.EntityType;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create a GSA Steel Design Task
  /// </summary>
  public class CreateSteelDesignTask : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("17012fb0-9a62-49fe-98d0-1cac1f47bcde");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateSteelDesignTask;

    private readonly List<string> _designObjectives = new List<string>(new[] {
        "Sustainable",
        "Min Cost",
        "Min Depth",
        "Min Weight",
      });

    public CreateSteelDesignTask() : base(
      "Create Steel Design Task",
      GsaDesignTaskGoo.NickName.Replace(" ", string.Empty),
      "Create a GSA Steel Design Task", CategoryName.Name(), SubCategoryName.Cat4()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Primary",
        "Secondary"
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(_designObjectives);
      _selectedItems.Add(_dropDownItems[0][3]);

      _dropDownItems.Add(_designObjectives);
      _selectedItems.Add(_dropDownItems[1][2]);

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddTextParameter("Name", "Na", "Task Name", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Number", "ID",
        "Set Task Number. If ID is set it will replace any existing DesignTasks in the model",
        GH_ParamAccess.item, 0);
      pManager.AddGenericParameter("Definition", "Def",
        "[Default = 'All'] Definition as text or list of object (Materials, Sections, Members)", GH_ParamAccess.list);
      pManager[2].Optional = true;
      pManager.AddIntegerParameter("CombinationCase", "CC", "Combination Case ID", GH_ParamAccess.item);
      pManager.AddNumberParameter("Target Utilisation", "η", "Target overall utilisation (upper)", GH_ParamAccess.item, 0.9);
      pManager.AddNumberParameter("Lower limit", "ηₘᵢₙ", "Lower utilisation limit (inefficiency warning)", GH_ParamAccess.item, 0.05);
      pManager.AddBooleanParameter("Grouped Design", "Grp", "If true, Members with the same pool are assigned the same section", GH_ParamAccess.item, false);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaDesignTaskParameter());
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      string name = string.Empty;
      da.GetData(0, ref name);
      var steelDesignTask = new GsaSteelDesignTask(name);

      int id = 0;
      da.GetData(1, ref id);
      steelDesignTask.Id = id;

      if (Params.Input[2].SourceCount == 0) {
        steelDesignTask.ApiTask.ListDefinition = "All";
      } else {
        var list = new GsaList() {
          EntityType = EntityType.Member
        };

        List<object> listGooObjects = Inputs.GetGooObjectsForLists(this, da, 2, EntityType.Member);
        try {
          list.SetListGooObjects(listGooObjects);
        } catch (ArgumentException) {
          string message
                = "Invalid member list\n\nThe member list should take the form:\n 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (Z4 XY55)\nwhere:\nGn  ->  Members in group n\nPn  ->  Members of property n\nPB(n)  ->  1D beam, bar, rod, strut and tie members (of property n)\nPA(n)  ->  2D members (of property n)n\nM(n)  ->  Members (of analysis material n)\nMS(n)  ->  Steel members (of grade n)\nMC(n)  ->  Concrete members (of grade n)\nMP(n)  ->  FRP members (of grade n)\nXn  ->  Members on global X line through node n\nYn  ->  ditto for Y\nZn  ->  ditto for Z\nXYn  ->  Members on global XY plane passing through node n\nYZn  ->  ditto for YZ\nZXn  ->  ditto for ZX\n\n* may be used in place of a member or property number\nto refer to the highest numbered member or property.";
          this.AddRuntimeError(message);
          return;
        }

        steelDesignTask.List = list;
      }

      int caseId = 0;
      da.GetData(3, ref caseId);
      steelDesignTask.ApiTask.CombinationCaseId = caseId;

      double targetUtil = 0.9;
      da.GetData(4, ref targetUtil);
      steelDesignTask.ApiTask.UpperTargetUtilisationLimit = targetUtil;

      double lowerLimit = 0.05;
      da.GetData(5, ref lowerLimit);
      steelDesignTask.ApiTask.LowerTargetUtilisationLimit = lowerLimit;

      bool group = false;
      da.GetData(6, ref group);
      steelDesignTask.ApiTask.GroupSectionsByPool = group;

      steelDesignTask.ApiTask.PrimaryObjective = GetDesignObjective(_selectedItems[0]);
      steelDesignTask.ApiTask.SecondaryObjective = GetDesignObjective(_selectedItems[1]);

      da.SetData(0, new GsaDesignTaskGoo(steelDesignTask));
    }

    private SteelDesignObjective GetDesignObjective(string s) {
      return s switch {
        "Sustainable" => SteelDesignObjective.MaxSustainability,
        "Min Cost" => SteelDesignObjective.MinCost,
        "Min Depth" => SteelDesignObjective.MinDepth,
        "Min Weight" => SteelDesignObjective.MinWeight,
        _ => throw new ArgumentException($"Unknown SteelDesignObjective `{s}`"),
      };
    }
  }
}
