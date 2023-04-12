using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to get information from a GSA List
  /// </summary>
  public class ListInfo : GH_OasysDropDownComponent, IGH_PreviewObject
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("2fb6f3b8-275b-452c-9387-bdf7ab9b7827");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.ListInfo;

    public ListInfo() : base("List Info",
      "ListInfo",
      "Get information of like ID, Name, Type and Definition, as well as all objects (Nodes, Elements, Members or Cases) from a GSA List",
      CategoryName.Name(),
      SubCategoryName.Cat0())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaListParameter());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddIntegerParameter("Index", "ID", "List Number if the list ever belonged to a GSA Model", GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "List Name", GH_ParamAccess.item);
      pManager.AddTextParameter("Type", "Ty", "Entity Type", GH_ParamAccess.item);
      pManager.AddTextParameter("Definition", "Def", "List Definition", GH_ParamAccess.item);
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      pManager.AddGenericParameter("List Objects [" + unitAbbreviation + "]", "Obj", "Expanded objects contained in the input list", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      var list = new GsaList();
      if (DA.GetData(0, ref list))
      {
        DA.SetData(0, list.Id);
        DA.SetData(1, list.Name);
        DA.SetData(2, list.EntityType.ToString());
        DA.SetData(3, list.Definition);
        DA.SetDataList(4, list.GetListObjects(_lengthUnit));
      }
    }

    #region Custom UI
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;

    protected override void InitialiseDropdowns()
    {
      _spacerDescriptions = new List<string>(new string[]
        {
          "Unit"
        });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // Length
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      _selectedItems[i] = _dropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      Params.Output[4].Name = "List Objects [" + unitAbbreviation + "]";
    }
    #endregion
  }
}

