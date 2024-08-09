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
  public class PropertyQuantities : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("fad210a6-8982-49fe-a026-b5f32df2a71d");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.PropertyQuantities;

    public PropertyQuantities() : base("Property Quantities", "PropBoQ",
      "Get Quantities for Sections, and 2D Properties from a GSA model",
      CategoryName.Name(), SubCategoryName.Cat0()) {
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
      pManager.AddGenericParameter("Section Quantities", "PBQ",
        "Total Length per Section Property from GSA Model\nGrafted by Section ID.",
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("2D Property Quantities", "PAQ",
        "Total Area per 2D Property from GSA Model.\nGrafted by Property ID.",
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
      var quantities = new GsaGH.Helpers.Import.PropertyQuantities(modelGoo.Value, layer, list, this);
      var sections = new DataTree<GH_UnitNumber>();
      foreach (KeyValuePair<int, Length> kvp in quantities.SectionQuantities) {
        var path = new GH_Path(kvp.Key);
        sections.Add(new GH_UnitNumber(kvp.Value), path);
      }

      var prop2ds = new DataTree<GH_UnitNumber>();
      foreach (KeyValuePair<int, Area> kvp in quantities.Property2dQuantities) {
        var path = new GH_Path(kvp.Key);
        prop2ds.Add(new GH_UnitNumber(kvp.Value), path);
      }

      da.SetDataTree(0, sections);
      da.SetDataTree(1, prop2ds);
    }
  }
}
