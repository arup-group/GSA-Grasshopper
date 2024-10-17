using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Components {
  internal enum AssemblyType {
    ByExplicitPositions,
    ByNumberOfPoints,
    BySpacingOfPoints,
    ByStorey
  }

  /// <summary>
  /// Component to create a new Assembly
  /// </summary>
  public class CreateAssembly : GH_OasysDropDownComponent {
    private static readonly IReadOnlyDictionary<AssemblyType, string> _assemblyTypes
      = new Dictionary<AssemblyType, string> {
        { AssemblyType.ByExplicitPositions, "Explicit positions" },
        { AssemblyType.ByNumberOfPoints, "Number of points" },
        { AssemblyType.BySpacingOfPoints, "Spacing of points" },
        { AssemblyType.ByStorey, "Storey" }
      };

    public override Guid ComponentGuid => new Guid("00eabd44-12cc-4c27-b924-82542602749f");
    public override GH_Exposure Exposure => GH_Exposure.quinary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateAssembly;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;
    private AssemblyType _mode = AssemblyType.ByNumberOfPoints;

    public CreateAssembly() : base("Create Assembly", "Assembly", "Create a GSA Assembly",
      CategoryName.Name(), SubCategoryName.Cat2()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      AssemblyType mode = GetModeBy(_selectedItems[0]);
      if (i == 0) {
        UpdateParameters(mode);
      }
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string lengthUnitAbr = Length.GetAbbreviation(_lengthUnit);

      SetInputProperties(5, "Extents y [" + lengthUnitAbr + "]", "Ey", "[Optional] Extents of the Assembly in y-direction", GH_ParamAccess.item);
      SetInputProperties(6, "Extents y [" + lengthUnitAbr + "]", "Ey", "[Optional] Extents of the Assembly in y-direction", GH_ParamAccess.item);

      int maxIndex = Params.Input.Count - 1;
      switch (_mode) {
        case AssemblyType.ByExplicitPositions:
          SetInputProperties(maxIndex, "Explicit positions [" + lengthUnitAbr + "]", "P", "List of explicit positions", GH_ParamAccess.list, false);
          break;

        case AssemblyType.ByNumberOfPoints:
          SetInputProperties(maxIndex, "Number", "N", "Number of points (default: 10)", GH_ParamAccess.item, true);
          break;

        case AssemblyType.BySpacingOfPoints:
          SetInputProperties(maxIndex, "Spacing", "Sp", "Spacing of points [" + lengthUnitAbr + "] (default: 1m)", GH_ParamAccess.item, true);
          break;

        case AssemblyType.ByStorey:
          SetInputProperties(maxIndex, "Storey List", "St", "List of storeys (default: 'all')", GH_ParamAccess.item, true);
          break;
      }

      if (_mode != AssemblyType.ByStorey) {
        SetInputProperties(maxIndex - 2, "Internal Topology", "IT", " List of nodes that define the curve of the Assembly", GH_ParamAccess.item, true);
        SetInputProperties(maxIndex - 1, "Curve Fit", "CF", "[Optional] Curve Fit for curved elements (default: 2)" + $"{Environment.NewLine}Lagrange Interpolation (2) or Circular Arc (1)", GH_ParamAccess.item, true);
      }
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Type",
        "Length Unit"
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(_assemblyTypes.Values.ToList());
      _selectedItems.Add(_assemblyTypes.Values.ElementAt(1));

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string lengthUnitAbr = Length.GetAbbreviation(_lengthUnit);

      pManager.AddTextParameter("Name", "Na", "[Optional] Assembly Name", GH_ParamAccess.item);
      pManager.AddParameter(new GsaElementMemberListParameter(), "List", "El",
        $"List of Assembly Entities (default: 'all'){Environment.NewLine}" +
        $"Element/Member list should take the form:{Environment.NewLine}" +
        $" 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" +
        $"{Environment.NewLine}Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Topology 1", "To1", "Node at the start of the Assembly", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Topology 2", "To2", "Node at the end of the Assembly", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Orientation Node", "⭮N", "Assembly Orientation Node", GH_ParamAccess.item);
      pManager.AddGenericParameter("Extents y [" + lengthUnitAbr + "]", "Ey", "[Optional] Extents of the Assembly in y-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Extents z [" + lengthUnitAbr + "]", "Ez", "[Optional] Extents of the Assembly in z-direction", GH_ParamAccess.item);
      pManager.AddParameter(new GsaNodeListParameter(), "Internal Topology", "IT", " List of nodes that define the curve of the Assembly", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Curve Fit", "CF", "[Optional] Curve Fit for curved elements (default: 2)" + $"{Environment.NewLine}Lagrange Interpolation (2) or Circular Arc (1)", GH_ParamAccess.item, 2);
      pManager.AddIntegerParameter("Number", "N", "Number of points (default: 10)", GH_ParamAccess.item, 10);

      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[5].Optional = true;
      pManager[6].Optional = true;
      pManager[7].Optional = true;
      pManager[8].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaAssemblyParameter());
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      string name = string.Empty;
      da.GetData(0, ref name);

      var entities = new EntityList() {
        Type = GsaAPI.EntityType.Element,
        Definition = "all"
      };
      GsaListGoo elementListGoo = null;
      if (da.GetData(1, ref elementListGoo)) {
        if (!elementListGoo.IsValid) {
          return;
        }
        entities = Inputs.GetElementOrMemberList(this, da, 1);
      }

      GH_Integer topology1 = null;
      da.GetData(2, ref topology1);

      GH_Integer topology2 = null;
      da.GetData(3, ref topology2);

      GH_Integer orientationNode = null;
      da.GetData(4, ref orientationNode);

      double extentsY = Input.UnitNumber(this, da, 5, _lengthUnit).As(LengthUnit.Meter);
      double extentsZ = Input.UnitNumber(this, da, 6, _lengthUnit).As(LengthUnit.Meter);

      List<int> internalTopology = null;
      int curveFit = 2;
      if (_mode != AssemblyType.ByStorey) {
        GsaListGoo internalTopologyList = null;
        if (da.GetData(7, ref internalTopologyList)) {
          if (!internalTopologyList.IsValid) {
            return;
          }

          internalTopology = internalTopologyList.Value.ExpandListDefinition();
        }

        da.GetData(8, ref curveFit);
      }

      GsaAssembly assembly;
      switch (_mode) {
        case AssemblyType.ByExplicitPositions:
          var positions = new SortedSet<double>();
          var dataList = new List<double>();
          if (da.GetDataList(9, dataList)) {
            foreach (double position in dataList) {
              positions.Add(position);
            }
          }

          AssemblyByExplicitPositions byExplicitPositions;
          if (internalTopology == null) {
            byExplicitPositions = new AssemblyByExplicitPositions(name, topology1.Value, topology2.Value, orientationNode.Value);
          } else {
            byExplicitPositions = new AssemblyByExplicitPositions(name, topology1.Value, topology2.Value, orientationNode.Value, internalTopology, (CurveFit)curveFit);
          }

          byExplicitPositions.Positions = positions;
          assembly = new GsaAssembly(byExplicitPositions);
          break;

        case AssemblyType.ByNumberOfPoints:
          int number = 10;
          da.GetData(9, ref number);

          AssemblyByNumberOfPoints byNumberOfPoints;
          if (internalTopology == null) {
            byNumberOfPoints = new AssemblyByNumberOfPoints(name, topology1.Value, topology2.Value, orientationNode.Value);
          } else {
            byNumberOfPoints = new AssemblyByNumberOfPoints(name, topology1.Value, topology2.Value, orientationNode.Value, internalTopology, (CurveFit)curveFit);
          }

          byNumberOfPoints.NumberOfPoints = number;

          assembly = new GsaAssembly(byNumberOfPoints);
          break;

        case AssemblyType.BySpacingOfPoints:
          double spacing = 1.0;
          da.GetData(9, ref spacing);

          AssemblyBySpacingOfPoints bySpacingOfPoints;
          if (internalTopology == null) {
            bySpacingOfPoints = new AssemblyBySpacingOfPoints(name, topology1.Value, topology2.Value, orientationNode.Value);
          } else {
            bySpacingOfPoints = new AssemblyBySpacingOfPoints(name, topology1.Value, topology2.Value, orientationNode.Value, internalTopology, (CurveFit)curveFit);
          }

          bySpacingOfPoints.Spacing = spacing;

          assembly = new GsaAssembly(bySpacingOfPoints);
          break;

        case AssemblyType.ByStorey:
          string storeyList = "all";
          da.GetData(7, ref storeyList);

          AssemblyByStorey byStorey;
          byStorey = new AssemblyByStorey(name, topology1.Value, topology2.Value, orientationNode.Value) {
            StoreyList = storeyList
          };

          if (internalTopology == null) {
          } else {
          }

          byStorey.StoreyList = storeyList;

          assembly = new GsaAssembly(byStorey);
          break;

        default:
          return;
      }

      assembly.ApiAssembly.EntityType = entities.Type;
      assembly.ApiAssembly.EntityList = entities.Definition;
      assembly.ApiAssembly.ExtentY = extentsY;
      assembly.ApiAssembly.ExtentZ = extentsZ;

      da.SetData(0, new GsaAssemblyGoo(assembly));
    }

    protected override void UpdateUIFromSelectedItems() {
      AssemblyType mode = GetModeBy(_selectedItems[0]);

      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);

      UpdateParameters(mode);

      base.UpdateUIFromSelectedItems();
    }

    private AssemblyType GetModeBy(string name) {
      foreach (KeyValuePair<AssemblyType, string> item in _assemblyTypes) {
        if (item.Value.Equals(name)) {
          return item.Key;
        }
      }
      throw new ArgumentException("Unable to convert " + name + " to Assembly Type");
    }

    private void SetInputProperties(int index, string name, string nickname, string description, GH_ParamAccess access, bool optional = true) {
      Params.Input[index].Name = name;
      Params.Input[index].NickName = nickname;
      Params.Input[index].Description = description;
      Params.Input[index].Access = access;
      Params.Input[index].Optional = optional;
    }

    private void UpdateParameters(AssemblyType mode) {
      if (mode == _mode) {
        return;
      }

      _assemblyTypes.TryGetValue(mode, out string eventName);
      RecordUndoEvent($"{eventName} Parameters");

      int maxIndex = Params.Input.Count - 1;

      switch (mode) {
        case AssemblyType.ByExplicitPositions:
        case AssemblyType.BySpacingOfPoints:
          Params.UnregisterInputParameter(Params.Input[maxIndex], true);
          break;

        case AssemblyType.ByNumberOfPoints:
          Params.UnregisterInputParameter(Params.Input[maxIndex], true);
          break;

        case AssemblyType.ByStorey:
          Params.UnregisterInputParameter(Params.Input[maxIndex], true);
          Params.UnregisterInputParameter(Params.Input[maxIndex - 1], true);
          Params.UnregisterInputParameter(Params.Input[maxIndex - 2], true);
          break;
      }

      if (maxIndex != 9) {
        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new Param_Integer());
      }

      switch (mode) {
        case AssemblyType.ByExplicitPositions:
        case AssemblyType.BySpacingOfPoints:
          Params.RegisterInputParam(new Param_Number());
          break;

        case AssemblyType.ByNumberOfPoints:
          Params.RegisterInputParam(new Param_Integer());
          break;

        case AssemblyType.ByStorey:
          Params.RegisterInputParam(new Param_String());
          break;
      }

      _mode = mode;
    }
  }
}
