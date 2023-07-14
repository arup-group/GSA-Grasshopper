using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using System;
using System.Collections.Generic;
using System.Drawing;
using EntityType = GsaGH.Parameters.EntityType;

namespace GsaGH.Components {
  public class CreateBeamLoads : GH_OasysDropDownComponent {
    private enum FoldMode {
      Point,
      Uniform,
      Linear,
      Patch,
      Trilinear,
    }

    public override Guid ComponentGuid => new Guid("e034b346-a6e8-4dd1-b12c-6104baa2586e");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.BeamLoad;
    private readonly List<string> _loadTypeOptions = new List<string>(new[] {
      "Point",
      "Uniform",
      "Linear",
      "Patch",
      "Trilinear",
    });
    private bool _duringLoad;
    private ForcePerLengthUnit _forcePerLengthUnit = DefaultUnits.ForcePerLengthUnit;
    private FoldMode _mode = FoldMode.Uniform;
    private EntityType _entityType = EntityType.Member;

    public CreateBeamLoads() : base("Create Beam Load", "BeamLoad", "Create GSA Beam Load",
      CategoryName.Name(), SubCategoryName.Cat3()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0) {
        switch (_selectedItems[1]) {
          case "Point":
            Mode1Clicked();
            break;

          case "Uniform":
            Mode2Clicked();
            break;

          case "Linear":
            Mode3Clicked();
            break;

          case "Patch":
            Mode4Clicked();
            break;

          case "Trilinear":
            Mode5Clicked();
            break;
        }
      } else if (i == 1) {
        switch (_selectedItems[0]) {
          case "Element":
            //Mode1Clicked();
            break;

          case "Member":
            //Mode2Clicked();
            break;
        }
      } else {
        _forcePerLengthUnit
          = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), _selectedItems[1]);
      }

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = ForcePerLength.GetAbbreviation(_forcePerLengthUnit);

      switch (_mode) {
        case FoldMode.Point:
          Params.Input[6].NickName = "V";
          Params.Input[6].Name = "Value [" + unitAbbreviation + "]";
          Params.Input[6].Description = "Load Value";
          Params.Input[6].Access = GH_ParamAccess.item;
          Params.Input[6].Optional = false;

          Params.Input[7].NickName = "t";
          Params.Input[7].Name = "Position (%)";
          Params.Input[7].Description = "Line parameter where point load act (between 0.0 and 1.0)";
          Params.Input[7].Access = GH_ParamAccess.item;
          Params.Input[7].Optional = true;
          break;

        case FoldMode.Uniform:
          Params.Input[6].NickName = "V";
          Params.Input[6].Name = "Value [" + unitAbbreviation + "]";
          Params.Input[6].Description = "Load Value";
          Params.Input[6].Access = GH_ParamAccess.item;
          Params.Input[6].Optional = false;
          break;

        case FoldMode.Linear:
          Params.Input[6].NickName = "V1";
          Params.Input[6].Name = "Value Start [" + unitAbbreviation + "]";
          Params.Input[6].Description = "Load Value at Beam Start";
          Params.Input[6].Access = GH_ParamAccess.item;
          Params.Input[6].Optional = true;

          Params.Input[7].NickName = "V2";
          Params.Input[7].Name = "Value End [" + unitAbbreviation + "]";
          Params.Input[7].Description = "Load Value at Beam End";
          Params.Input[7].Access = GH_ParamAccess.item;
          Params.Input[7].Optional = true;
          break;

        case FoldMode.Patch:
          Params.Input[6].NickName = "V1";
          Params.Input[6].Name = "Load t1 [" + unitAbbreviation + "]";
          Params.Input[6].Description = "Load Value at Position 1";
          Params.Input[6].Access = GH_ParamAccess.item;
          Params.Input[6].Optional = true;

          Params.Input[7].NickName = "t1";
          Params.Input[7].Name = "Position 1 [%]";
          Params.Input[7].Description
            = "Line parameter where patch load begins (between 0.0 and 1.0, but less than t2)";
          Params.Input[7].Access = GH_ParamAccess.item;
          Params.Input[7].Optional = true;

          Params.Input[8].NickName = "V2";
          Params.Input[8].Name = "Load t2 [" + unitAbbreviation + "]";
          Params.Input[8].Description = "Load Value at Position 2";
          Params.Input[8].Access = GH_ParamAccess.item;
          Params.Input[8].Optional = true;

          Params.Input[9].NickName = "t2";
          Params.Input[9].Name = "Position 2 [%]";
          Params.Input[9].Description
            = "Line parameter where patch load ends (between 0.0 and 1.0, but bigger than t1)";
          Params.Input[9].Access = GH_ParamAccess.item;
          Params.Input[9].Optional = true;
          break;

        case FoldMode.Trilinear:
          Params.Input[6].NickName = "V1";
          Params.Input[6].Name = "Load t1 [" + unitAbbreviation + "]";
          Params.Input[6].Description = "Load Value at Position 1";
          Params.Input[6].Access = GH_ParamAccess.item;
          Params.Input[6].Optional = true;

          Params.Input[7].NickName = "t1";
          Params.Input[7].Name = "Position 1 [%]";
          Params.Input[7].Description
            = "Line parameter where L1 applies (between 0.0 and 1.0, but less than t2)";
          Params.Input[7].Access = GH_ParamAccess.item;
          Params.Input[7].Optional = true;

          Params.Input[8].NickName = "V2";
          Params.Input[8].Name = "Load t2 [" + unitAbbreviation + "]";
          Params.Input[8].Description = "Load Value at Position 2";
          Params.Input[8].Access = GH_ParamAccess.item;
          Params.Input[8].Optional = true;

          Params.Input[9].NickName = "t2";
          Params.Input[9].Name = "Position 2 [%]";
          Params.Input[9].Description
            = "Line parameter where L2 applies (between 0.0 and 1.0, but bigger than t1)";
          Params.Input[9].Access = GH_ParamAccess.item;
          Params.Input[9].Optional = true;
          break;
      }
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Type",
        "EntityType",
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(_loadTypeOptions);
      _selectedItems.Add(_mode.ToString());

      _dropDownItems.Add(new List<string>(new[] {
        "Element",
        "Member"
      }));
      _selectedItems.Add(_entityType.ToString());

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerLength));
      _selectedItems.Add(ForcePerLength.GetAbbreviation(_forcePerLengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = ForcePerLength.GetAbbreviation(_forcePerLengthUnit);

      pManager.AddIntegerParameter("Load case", "LC", "Load case number (default 1)",
        GH_ParamAccess.item, 1);
      pManager.AddGenericParameter("Element list", "G1D",
        "List, Custom Material, Section, 1D Elements or 1D Members to apply load to; either input Section, Element1d, or Member1d, or a text string."
        + Environment.NewLine + "Text string with Element list should take the form:"
        + Environment.NewLine
        + " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)"
        + Environment.NewLine
        + "Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Load Name", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Axis", "Ax",
        "Load axis (default Global). " + Environment.NewLine + "Accepted inputs are:"
        + Environment.NewLine + "0 : Global" + Environment.NewLine + "-1 : Local",
        GH_ParamAccess.item, 0);
      pManager.AddTextParameter("Direction", "Di",
        "Load direction (default z)." + Environment.NewLine + "Accepted inputs are:"
        + Environment.NewLine + "x" + Environment.NewLine + "y" + Environment.NewLine + "z"
        + Environment.NewLine + "xx" + Environment.NewLine + "yy" + Environment.NewLine + "zz",
        GH_ParamAccess.item, "z");
      pManager.AddBooleanParameter("Projected", "Pj", "Projected (default not)",
        GH_ParamAccess.item, false);
      pManager.AddNumberParameter("Value [" + unitAbbreviation + "]", "V", "Load Value",
        GH_ParamAccess.item);

      pManager[0].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
      pManager[5].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaLoadParameter(), "Beam Load", "Ld", "GSA Beam Load",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var beamLoad = new GsaBeamLoad();

      int loadCase = 1;
      var ghLc = new GH_Integer();
      if (da.GetData(0, ref ghLc)) {
        GH_Convert.ToInt32(ghLc, out loadCase, GH_Conversion.Both);
      }

      beamLoad.BeamLoad.Case = loadCase;

      if (_entityType == EntityType.Element) {
        beamLoad.ReferenceType = ReferenceType.Element;
      } else if (_entityType == EntityType.Member) {
        beamLoad.ReferenceType = ReferenceType.Member;
      } else {
        throw new ArgumentException("Entity type " + _entityType.ToString() + " not supported.");
      }

      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(1, ref ghTyp)) {
        switch (ghTyp.Value) {
          case GsaListGoo listGoo:
            if (listGoo.Value.EntityType == EntityType.Element
              || listGoo.Value.EntityType == EntityType.Member) {
              beamLoad.ReferenceList = listGoo.Value;
              beamLoad.ReferenceType = ReferenceType.List;
            } else {
              this.AddRuntimeWarning(
                "List must be of type Element or Member to apply to beam loading");
            }

            if (listGoo.Value.EntityType == EntityType.Member) {
              this.AddRuntimeRemark(
                "Member list applied to loading in GsaGH will automatically find child elements created from parent member with the load still being applied to elements." + Environment.NewLine + "If you save the file and continue working in GSA please note that the member-loading relationship will be lost.");
            }
            break;


          case GsaElement1dGoo element1dGoo:
            if (_entityType != EntityType.Element) {
              this.AddRuntimeWarning("Beam loads can only be applied to elements matching the selected enttiy type.");
              break;
            }
            beamLoad.RefObjectGuid = element1dGoo.Value.Guid;
            beamLoad.BeamLoad.EntityType = GsaAPI.EntityType.Element;
            break;

          case GsaMember1dGoo member1dGoo:
            if (_entityType != EntityType.Member) {
              this.AddRuntimeWarning("Beam loads can only be applied to members matching the selected enttiy type.");
              break;
            }
            beamLoad.RefObjectGuid = member1dGoo.Value.Guid;
            beamLoad.BeamLoad.EntityType = GsaAPI.EntityType.Member;
            if (_mode != FoldMode.Uniform) {
              this.AddRuntimeWarning(
                "Member loading will not automatically redistribute non-linear loading to child elements." + Environment.NewLine + "Any non-uniform loading made from Members is likely not what you are after. Please check the load in GSA.");
            }
            break;

          case GsaMaterialGoo materialGoo:
            if (materialGoo.Value.Id != 0) {
              this.AddRuntimeWarning(
              "Reference Material must be a Custom Material");
              return;
            }
            beamLoad.RefObjectGuid = materialGoo.Value.Guid;
            beamLoad.ReferenceType = ReferenceType.Property;
            break;

          case GsaSectionGoo sectionGoo:
            beamLoad.RefObjectGuid = sectionGoo.Value.Guid;
            beamLoad.ReferenceType = ReferenceType.Property;
            break;

          default:
            if (GH_Convert.ToString(ghTyp.Value, out string beamList, GH_Conversion.Both)) {
              beamLoad.BeamLoad.EntityList = beamList;
            }
            break;
        }
      }

      var ghName = new GH_String();
      if (da.GetData(2, ref ghName)) {
        if (GH_Convert.ToString(ghName, out string name, GH_Conversion.Both)) {
          beamLoad.BeamLoad.Name = name;
        }
      }

      beamLoad.BeamLoad.AxisProperty
        = 0; //Note there is currently a bug/undocumented in GsaAPI that cannot translate an integer into axis type (Global, Local or edformed local)
      var ghAx = new GH_Integer();
      if (da.GetData(3, ref ghAx)) {
        GH_Convert.ToInt32(ghAx, out int axis, GH_Conversion.Both);
        if (axis == 0 || axis == -1) {
          beamLoad.BeamLoad.AxisProperty = axis;
        }
      }

      string dir = "Z";
      GsaAPI.Direction direc = GsaAPI.Direction.Z;

      var ghDir = new GH_String();
      if (da.GetData(4, ref ghDir)) {
        GH_Convert.ToString(ghDir, out dir, GH_Conversion.Both);
      }

      dir = dir.ToUpper().Trim();
      switch (dir) {
        case "X":
          direc = GsaAPI.Direction.X;
          break;

        case "Y":
          direc = GsaAPI.Direction.Y;
          break;

        case "XX":
          direc = GsaAPI.Direction.XX;
          break;

        case "YY":
          direc = GsaAPI.Direction.YY;
          break;

        case "ZZ":
          direc = GsaAPI.Direction.ZZ;
          break;
      }

      beamLoad.BeamLoad.Direction = direc;

      bool prj = false;
      var ghPrj = new GH_Boolean();
      if (da.GetData(5, ref ghPrj)) {
        GH_Convert.ToBoolean(ghPrj, out prj, GH_Conversion.Both);
      }

      beamLoad.BeamLoad.IsProjected = prj;

      var load1 = (ForcePerLength)Input.UnitNumber(this, da, 6, _forcePerLengthUnit);

      switch (_mode) {
        case FoldMode.Point:
          if (_mode == FoldMode.Point) {
            beamLoad.BeamLoad.Type = GsaAPI.BeamLoadType.POINT;
            double pos = 0;
            if (da.GetData(7, ref pos)) {
              pos *= -1;
            }

            beamLoad.BeamLoad.SetValue(0, load1.NewtonsPerMeter);
            beamLoad.BeamLoad.SetPosition(0, pos);
          }

          break;

        case FoldMode.Uniform:
          if (_mode == FoldMode.Uniform) {
            beamLoad.BeamLoad.Type = GsaAPI.BeamLoadType.UNIFORM;
            beamLoad.BeamLoad.SetValue(0, load1.NewtonsPerMeter);
          }

          break;

        case FoldMode.Linear:
          if (_mode == FoldMode.Linear) {
            beamLoad.BeamLoad.Type = GsaAPI.BeamLoadType.LINEAR;
            var load2 = (ForcePerLength)Input.UnitNumber(this, da, 7, _forcePerLengthUnit);
            beamLoad.BeamLoad.SetValue(0, load1.NewtonsPerMeter);
            beamLoad.BeamLoad.SetValue(1, load2.NewtonsPerMeter);
          }

          break;

        case FoldMode.Patch:
          if (_mode == FoldMode.Patch) {
            beamLoad.BeamLoad.Type = GsaAPI.BeamLoadType.PATCH;
            double pos1 = 0;
            if (da.GetData(7, ref pos1)) {
              pos1 *= -1;
            }

            double pos2 = 1;
            if (da.GetData(9, ref pos2)) {
              pos2 *= -1;
            }

            var load2 = (ForcePerLength)Input.UnitNumber(this, da, 8, _forcePerLengthUnit);
            beamLoad.BeamLoad.SetValue(0, load1.NewtonsPerMeter);
            beamLoad.BeamLoad.SetValue(1, load2.NewtonsPerMeter);
            beamLoad.BeamLoad.SetPosition(0, pos1);
            beamLoad.BeamLoad.SetPosition(1, pos2);
          }

          break;

        case FoldMode.Trilinear:
          if (_mode == FoldMode.Trilinear) {
            beamLoad.BeamLoad.Type = GsaAPI.BeamLoadType.TRILINEAR;
            double pos1 = 0;
            if (da.GetData(7, ref pos1)) {
              pos1 *= -1;
            }

            double pos2 = 1;
            if (da.GetData(9, ref pos2)) {
              pos2 *= -1;
            }

            var load2 = (ForcePerLength)Input.UnitNumber(this, da, 8, _forcePerLengthUnit);
            beamLoad.BeamLoad.SetValue(0, load1.NewtonsPerMeter);
            beamLoad.BeamLoad.SetValue(1, load2.NewtonsPerMeter);
            beamLoad.BeamLoad.SetPosition(0, pos1);
            beamLoad.BeamLoad.SetPosition(1, pos2);
          }

          break;
      }

      da.SetData(0, new GsaLoadGoo(beamLoad));
    }

    protected override void UpdateUIFromSelectedItems() {
      _mode = (FoldMode)Enum.Parse(typeof(FoldMode), _selectedItems[0]);
      _duringLoad = true;
      switch (_selectedItems[0]) {
        case "Point":
          Mode1Clicked();
          break;

        case "Uniform":
          Mode2Clicked();
          break;

        case "Linear":
          Mode3Clicked();
          break;

        case "Patch":
          Mode4Clicked();
          break;

        case "Trilinear":
          Mode5Clicked();
          break;
      }

      _duringLoad = false;
      _forcePerLengthUnit
        = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), _selectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }

    private void Mode1Clicked() {
      if (!_duringLoad && _mode == FoldMode.Point) {
        return;
      }

      RecordUndoEvent("Point Parameters");
      _mode = FoldMode.Point;

      while (Params.Input.Count > 7) {
        Params.UnregisterInputParameter(Params.Input[7], true);
      }

      Params.RegisterInputParam(new Param_GenericObject());
    }

    private void Mode2Clicked() {
      if (!_duringLoad && _mode == FoldMode.Uniform) {
        return;
      }

      RecordUndoEvent("Uniform Parameters");
      _mode = FoldMode.Uniform;

      while (Params.Input.Count > 7) {
        Params.UnregisterInputParameter(Params.Input[7], true);
      }
    }

    private void Mode3Clicked() {
      if (!_duringLoad && _mode == FoldMode.Linear) {
        return;
      }

      RecordUndoEvent("Linear Parameters");
      _mode = FoldMode.Linear;

      while (Params.Input.Count > 7) {
        Params.UnregisterInputParameter(Params.Input[7], true);
      }

      Params.RegisterInputParam(new Param_GenericObject());
    }

    private void Mode4Clicked() {
      if (!_duringLoad && _mode == FoldMode.Patch) {
        return;
      }

      RecordUndoEvent("Patch Parameters");
      _mode = FoldMode.Patch;

      if (_mode == FoldMode.Trilinear) {
        return;
      }

      while (Params.Input.Count > 7) {
        Params.UnregisterInputParameter(Params.Input[7], true);
      }

      Params.RegisterInputParam(new Param_Number());
      Params.RegisterInputParam(new Param_GenericObject());
      Params.RegisterInputParam(new Param_Number());
    }

    private void Mode5Clicked() {
      if (!_duringLoad && _mode == FoldMode.Trilinear) {
        return;
      }

      RecordUndoEvent("Trilinear Parameters");
      _mode = FoldMode.Trilinear;

      if (_mode == FoldMode.Patch) {
        return;
      }

      while (Params.Input.Count > 7) {
        Params.UnregisterInputParameter(Params.Input[7], true);
      }

      Params.RegisterInputParam(new Param_Number());
      Params.RegisterInputParam(new Param_GenericObject());
      Params.RegisterInputParam(new Param_Number());
    }
  }
}
