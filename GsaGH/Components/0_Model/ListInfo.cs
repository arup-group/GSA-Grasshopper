using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get information of an EntityList
  /// </summary>
  public class ListInfo : GH_OasysDropDownComponent, IGH_PreviewObject {
    public override Guid ComponentGuid => new Guid("2fb6f3b8-275b-452c-9387-bdf7ab9b7827");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.ListInfo;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;

    public ListInfo() : base("List Info", "ListInfo",
      "Get information of like ID, Name, Type and Definition, as well as all objects (Nodes, Elements, Members or Cases) from a GSA List",
      CategoryName.Name(), SubCategoryName.Cat0()) { }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      Params.Output[4].Name = "List Objects in [" + unitAbbreviation + "]";
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] {
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // Length
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void BeforeSolveInstance() {
      if (Params.Output.Count == 5) {
        Params.RegisterOutputParam(new Param_Integer());
        Params.Output[5].Name = "Expand List";
        Params.Output[5].NickName = "Exp";
        Params.Output[5].Description = "Expanded list IDs";
        Params.Output[5].Access = GH_ParamAccess.list;
        VariableParameterMaintenance();
      }
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaListParameter());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddIntegerParameter("Index", "ID",
        "List Number if the list ever belonged to a GSA Model", GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "List Name", GH_ParamAccess.item);
      pManager.AddTextParameter("Type", "Ty", "Entity Type", GH_ParamAccess.item);
      pManager.AddTextParameter("Definition", "Def", "List Definition", GH_ParamAccess.item);
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      pManager.AddGenericParameter("List Objects in [" + unitAbbreviation + "]", "Obj",
        "Expanded objects contained in the input list", GH_ParamAccess.list);
      pManager.AddIntegerParameter("Expand List", "Exp",
        "Expanded list IDs", GH_ParamAccess.list);
    }

    protected override void SolveInternal(IGH_DataAccess DA) {
      var list = new GsaList();

      GsaListGoo listGoo = null;
      DA.GetData(0, ref listGoo);
      list = listGoo.Value;

      DA.SetData(0, list.Id);
      DA.SetData(1, list.Name);
      DA.SetData(2, list.EntityType.ToString());
      DA.SetData(3, list.Definition);
      DA.SetDataList(4, list.GetListObjects(_lengthUnit));
      try {
        DA.SetDataList(5, list.ExpandListDefinition());
      } catch (ArgumentException e) {
        if (e.Message == "Invalid EntityList definition") {
          this.AddRuntimeRemark($"Unable to expand list '{list.Definition}'");
        } else {
          throw e;
        }
      }
    }
  }
}
