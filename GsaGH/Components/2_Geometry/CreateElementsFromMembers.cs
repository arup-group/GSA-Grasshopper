using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using GH_IO.Serialization;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Helpers.Assembly;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;

using Elements = GsaGH.Helpers.Import.Elements;
using LengthUnit = OasysUnits.Units.LengthUnit;
using Nodes = GsaGH.Helpers.Import.Nodes;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a Node
  /// </summary>
  public class CreateElementsFromMembers : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("3de73a08-b72c-45e4-a650-e4c6515266c5");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateElementsFromMembers;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    internal ToleranceContextMenu ToleranceMenu { get; set; } = new ToleranceContextMenu();

    public CreateElementsFromMembers() : base("Create Elements from Members", "ElemFromMem",
      "Create Elements from Members", CategoryName.Name(), SubCategoryName.Cat2()) { }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      ToleranceMenu.AppendAdditionalMenuItems(this, menu, _lengthUnit);
    }

    public override bool Read(GH_IReader reader) {
      bool flag = base.Read(reader);
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);
      if (reader.ItemExists("Tolerance")) {
        double tol = reader.GetDouble("Tolerance");
        ToleranceMenu.Tolerance = new Length(tol, _lengthUnit);
      } else {
        ToleranceMenu.Tolerance = DefaultUnits.Tolerance;
      }

      ToleranceMenu.UpdateMessage(this, _lengthUnit);
      return flag;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
      ToleranceMenu.UpdateMessage(this, _lengthUnit);
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      int i = 0;
      Params.Input[i++].Name = "Nodes [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "1D Members [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "2D Members [" + unitAbbreviation + "]";
      Params.Input[i].Name = "3D Members [" + unitAbbreviation + "]";
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetDouble("Tolerance", ToleranceMenu.Tolerance.Value);
      return base.Write(writer);
    }

    protected override void BeforeSolveInstance() {
      base.BeforeSolveInstance();
      ToleranceMenu.UpdateMessage(this, _lengthUnit);
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      pManager.AddParameter(new GsaNodeParameter(), "Nodes [" + unitAbbreviation + "]", "No",
        "Nodes to be included in meshing", GH_ParamAccess.list);
      pManager.AddParameter(new GsaMember1dParameter(), "1D Members [" + unitAbbreviation + "]", "M1D",
        "1D Members to create 1D Elements from", GH_ParamAccess.list);
      pManager.AddParameter(new GsaMember2dParameter(), "2D Members [" + unitAbbreviation + "]", "M2D",
        "2D Members to create 2D Elements from", GH_ParamAccess.list);
      pManager.AddParameter(new GsaMember3dParameter(), "3D Members [" + unitAbbreviation + "]", "M3D",
        "3D Members to create 3D Elements from", GH_ParamAccess.list);

      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaNodeParameter(), "Nodes", "No", "GSA Nodes", GH_ParamAccess.list);
      pManager.HideParameter(0);
      pManager.AddParameter(new GsaElement1dParameter(), "1D Elements", "E1D", "GSA 1D Elements", GH_ParamAccess.list);
      pManager.AddParameter(new GsaElement2dParameter(), "2D Elements", "E2D", "GSA 2D Elements", GH_ParamAccess.list);
      pManager.AddParameter(new GsaElement3dParameter(), "3D Elements", "E3D", "GSA 3D Elements", GH_ParamAccess.list);
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA", "GSA Model with Elements and Members",
        GH_ParamAccess.item);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var ghTyp = new GH_ObjectWrapper();

      var nodeGoos = new List<GsaNodeGoo>();
      var nodes = new List<GsaNode>();
      if (da.GetDataList(0, nodeGoos)) {
        nodes = nodeGoos.ConvertAll(x => x.Value);
      }

      var member1dGoos = new List<GsaMember1dGoo>();
      var member1ds = new List<GsaMember1D>();
      if (da.GetDataList(1, member1dGoos)) {
        member1ds = member1dGoos.ConvertAll(x => x.Value);
      }

      var member2dGoos = new List<GsaMember2dGoo>();
      var member2ds = new List<GsaMember2D>();
      if (da.GetDataList(2, member2dGoos)) {
        member2ds = member2dGoos.ConvertAll(x => x.Value);
      }

      var member3dGoos = new List<GsaMember3dGoo>();
      var member3ds = new List<GsaMember3D>();
      if (da.GetDataList(3, member3dGoos)) {
        member3ds = member3dGoos.ConvertAll(x => x.Value);
      }

      // manually add a warning if no input is set, as all three inputs are optional
      if ((member1ds.Count < 1) & (member2ds.Count < 1) & (member3ds.Count < 1)) {
        this.AddRuntimeWarning("Input parameters failed to collect data");
        return;
      }

      var geometry = new GsaGeometry {
        Nodes = nodes,
        Member1ds = member1ds,
        Member2ds = member2ds,
        Member3ds = member3ds
      };
      var assembly = new ModelAssembly(null, null, null, geometry, null, null, null, _lengthUnit,
        ToleranceMenu.Tolerance, true, this);
      Model gsa = assembly.GetModel();

      var outModel = new GsaModel {
        ApiModel = gsa,
        ModelUnit = _lengthUnit,
      };

      ConcurrentBag<GsaNodeGoo> nodesOut = Nodes.GetNodes(outModel.ApiNodes, outModel.ModelUnit);
      var elements = new Elements(outModel);

      da.SetDataList(0, nodesOut.OrderBy(item => item.Value.Id));
      da.SetDataList(1, elements.Element1ds.OrderBy(item => item.Value.Id));
      da.SetDataList(2, elements.Element2ds.OrderBy(item => item.Value.Ids.First()));
      da.SetDataList(3, elements.Element3ds.OrderBy(item => item.Value.Ids.First()));
      da.SetData(4, new GsaModelGoo(outModel));

      ToleranceMenu.UpdateMessage(this, _lengthUnit);
    }
  }
}
