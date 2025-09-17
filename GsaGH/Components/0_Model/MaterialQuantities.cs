using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;

using OasysUnits;

namespace GsaGH.Components {
  public class MaterialQuantities : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("dcf67a4b-6443-429f-8d68-95af69493809");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.MaterialQuantities;

    public MaterialQuantities() : base("Material Quantities", "MatBoQ",
      "Get Quantities for Standard and Custom Materials from a GSA model", CategoryName.Name(),
      SubCategoryName.Cat0()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] {
        "Layer",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(new List<string>() {
        "Analysis", "Design"
      });
      _selectedItems.Add(_dropDownItems[0][1]);

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter());
      pManager.AddParameter(new GsaElementMemberListParameter());
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("Steel Quantities", "SQ",
        "Total weight of Steel Materials from GSA Model\nGrafted by Material ID.",
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("Concrete Quantities", "CQ",
        "Total weight of Concrete Materials from GSA Model\nGrafted by Material ID.",
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("FRP Quantities", "PQ",
        "Total weight of FRP Materials from GSA Model\nGrafted by Material ID.",
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("Aluminium Quantities", "AQ",
        "Total weight of Aluminium Materials from GSA Model\nGrafted by Material ID.",
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("Timber Quantities", "TQ",
        "Total weight of Timber Materials from GSA Model\nGrafted by Material ID.",
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("Glass Quantities", "GQ",
        "Total weight of Glass Materials from GSA Model\nGrafted by Material ID.",
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("Custom Quantities", "CsQ",
        "Total weight of Custom Analysis Materials from GSA Model\nGrafted by Material ID.",
        GH_ParamAccess.tree);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      GsaModelGoo modelGoo = null;
      da.GetData(0, ref modelGoo);
      string list = _selectedItems[0] == "Analysis"
        ? Inputs.GetElementListDefinition(this, da, 1, modelGoo.Value)
        : Inputs.GetMemberListDefinition(this, da, 1, modelGoo.Value);
      Layer layer = _selectedItems[0] == "Analysis"
        ? Layer.Analysis : Layer.Design;
      var quantities = new GsaGH.Helpers.Import.MaterialQuantities(modelGoo.Value, layer, list, this);
      var steel = new DataTree<GH_UnitNumber>();
      foreach (KeyValuePair<int, Mass> kvp in quantities.SteelQuantities) {
        var path = new GH_Path(kvp.Key, RunCount);
        steel.Add(new GH_UnitNumber(kvp.Value), path);
      }

      var concrete = new DataTree<GH_UnitNumber>();
      foreach (KeyValuePair<int, Mass> kvp in quantities.ConcreteQuantities) {
        var path = new GH_Path(kvp.Key, RunCount);
        concrete.Add(new GH_UnitNumber(kvp.Value), path);
      }

      var frp = new DataTree<GH_UnitNumber>();
      foreach (KeyValuePair<int, Mass> kvp in quantities.FrpQuantities) {
        var path = new GH_Path(kvp.Key, RunCount);
        frp.Add(new GH_UnitNumber(kvp.Value), path);
      }

      var aluminium = new DataTree<GH_UnitNumber>();
      foreach (KeyValuePair<int, Mass> kvp in quantities.AluminiumQuantities) {
        var path = new GH_Path(kvp.Key, RunCount);
        aluminium.Add(new GH_UnitNumber(kvp.Value), path);
      }

      var timber = new DataTree<GH_UnitNumber>();
      foreach (KeyValuePair<int, Mass> kvp in quantities.TimberQuantities) {
        var path = new GH_Path(kvp.Key, RunCount);
        timber.Add(new GH_UnitNumber(kvp.Value), path);
      }

      var glass = new DataTree<GH_UnitNumber>();
      foreach (KeyValuePair<int, Mass> kvp in quantities.GlassQuantities) {
        var path = new GH_Path(kvp.Key, RunCount);
        glass.Add(new GH_UnitNumber(kvp.Value), path);
      }

      var custom = new DataTree<GH_UnitNumber>();
      foreach (KeyValuePair<int, Mass> kvp in quantities.CustomMaterialQuantities) {
        var path = new GH_Path(kvp.Key, RunCount);
        custom.Add(new GH_UnitNumber(kvp.Value), path);
      }



      da.SetDataTree(0, steel);
      da.SetDataTree(1, concrete);
      da.SetDataTree(2, frp);
      da.SetDataTree(3, aluminium);
      da.SetDataTree(4, timber);
      da.SetDataTree(5, glass);
      da.SetDataTree(6, custom);
    }
  }
}
