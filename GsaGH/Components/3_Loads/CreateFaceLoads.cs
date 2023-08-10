using System;
using System.Collections.Generic;
using System.Drawing;
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
using OasysUnits.Units;
using EntityType = GsaGH.Parameters.EntityType;

namespace GsaGH.Components {
  public class CreateFaceLoads : GH_OasysDropDownComponent {
    private enum FoldMode {
      Uniform,
      Variable,
      Point,
      Edge, //note implementation of edge-load is not yet supported in GsaAPI
    }

    public override Guid ComponentGuid => new Guid("c4ad7a1e-350b-48b2-b636-24b6ef7bd0f3");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.FaceLoad;
    private readonly List<string> _loadTypeOptions = new List<string>(new[] {
      "Uniform",
      "Variable",
      "Point",
      //"Edge" note implementation of edge-load is not yet supported in GsaAPI
    });
    private bool _duringLoad;
    private PressureUnit _forcePerAreaUnit = DefaultUnits.ForcePerAreaUnit;
    private FoldMode _mode = FoldMode.Uniform;

    public CreateFaceLoads() : base("Create Face Load", "FaceLoad", "Create GSA Face Load",
      CategoryName.Name(), SubCategoryName.Cat3()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0) {
        switch (_selectedItems[0]) {
          case "Uniform":
            Mode1Clicked();
            break;

          case "Variable":
            Mode2Clicked();
            break;

          case "Point":
            Mode3Clicked();
            break;

          case "Edge":
            Mode4Clicked();
            break;
        }
      } else {
        _forcePerAreaUnit
          = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[1]);
      }

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Pressure.GetAbbreviation(_forcePerAreaUnit);

      switch (_mode) {
        case FoldMode.Uniform:
          Params.Input[5].NickName = "Pj";
          Params.Input[5].Name = "Projected";
          Params.Input[5].Description = "Projected (default not)";
          Params.Input[5].Access = GH_ParamAccess.item;
          Params.Input[5].Optional = true;

          Params.Input[6].NickName = "V";
          Params.Input[6].Name = "Value [" + unitAbbreviation + "]";
          Params.Input[6].Description = "Load Value";
          Params.Input[6].Access = GH_ParamAccess.item;
          Params.Input[6].Optional = false;
          break;

        case FoldMode.Variable:
          Params.Input[5].NickName = "Pj";
          Params.Input[5].Name = "Projected";
          Params.Input[5].Description = "Projected (default not)";
          Params.Input[5].Access = GH_ParamAccess.item;
          Params.Input[5].Optional = true;

          Params.Input[6].NickName = "V1";
          Params.Input[6].Name = "Value 1 [" + unitAbbreviation + "]";
          Params.Input[6].Description = "Load Value Corner 1";
          Params.Input[6].Access = GH_ParamAccess.item;
          Params.Input[6].Optional = true;

          Params.Input[7].NickName = "V2";
          Params.Input[7].Name = "Value 2 [" + unitAbbreviation + "]";
          Params.Input[7].Description = "Load Value Corner 2";
          Params.Input[7].Access = GH_ParamAccess.item;
          Params.Input[7].Optional = true;

          Params.Input[8].NickName = "V3";
          Params.Input[8].Name = "Value 3 [" + unitAbbreviation + "]";
          Params.Input[8].Description = "Load Value Corner 3";
          Params.Input[8].Access = GH_ParamAccess.item;
          Params.Input[8].Optional = true;

          Params.Input[9].NickName = "V4";
          Params.Input[9].Name = "Value 4 [" + unitAbbreviation + "]";
          Params.Input[9].Description = "Load Value Corner 4";
          Params.Input[9].Access = GH_ParamAccess.item;
          Params.Input[9].Optional = true;
          break;

        case FoldMode.Point:
          Params.Input[5].NickName = "Pj";
          Params.Input[5].Name = "Projected";
          Params.Input[5].Description = "Projected (default not)";
          Params.Input[5].Access = GH_ParamAccess.item;
          Params.Input[5].Optional = true;

          Params.Input[6].NickName = "V";
          Params.Input[6].Name = "Value [" + unitAbbreviation + "]";
          Params.Input[6].Description = "Load Value Corner 1";
          Params.Input[6].Access = GH_ParamAccess.item;
          Params.Input[6].Optional = false;

          Params.Input[7].NickName = "r";
          Params.Input[7].Name = "Position r";
          Params.Input[7].Description
            = "The position r of the point load to be specified in ( r , s )" + Environment.NewLine
            + "coordinates based on two-dimensional shape function." + Environment.NewLine
            + " • Coordinates vary from −1 to 1 for Quad 4 and Quad 8." + Environment.NewLine
            + " • Coordinates vary from 0 to 1 for Triangle 3 and Triangle 6";
          Params.Input[7].Access = GH_ParamAccess.item;
          Params.Input[7].Optional = true;

          Params.Input[8].NickName = "s";
          Params.Input[8].Name = "Position s";
          Params.Input[8].Description
            = "The position s of the point load to be specified in ( r , s )" + Environment.NewLine
            + "coordinates based on two-dimensional shape function." + Environment.NewLine
            + " • Coordinates vary from −1 to 1 for Quad 4 and Quad 8." + Environment.NewLine
            + " • Coordinates vary from 0 to 1 for Triangle 3 and Triangle 6";
          Params.Input[8].Access = GH_ParamAccess.item;
          Params.Input[8].Optional = true;
          break;

        case FoldMode.Edge:
          Params.Input[5].NickName = "Ed";
          Params.Input[5].Name = "Edge";
          Params.Input[5].Description = "Edge (1, 2, 3 or 4)";
          Params.Input[5].Access = GH_ParamAccess.item;
          Params.Input[5].Optional = false;

          Params.Input[6].NickName = "V1";
          Params.Input[6].Name = "Value 1 [" + unitAbbreviation + "]";
          Params.Input[6].Description = "Load Value Corner 1";
          Params.Input[6].Access = GH_ParamAccess.item;
          Params.Input[6].Optional = false;

          Params.Input[7].NickName = "V2";
          Params.Input[7].Name = "Value 2 [" + unitAbbreviation + "]";
          Params.Input[7].Description = "Load Value Corner 2";
          Params.Input[7].Access = GH_ParamAccess.item;
          Params.Input[7].Optional = false;
          break;
      }
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Type",
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(_loadTypeOptions);
      _selectedItems.Add(_mode.ToString());

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerArea));
      _selectedItems.Add(Pressure.GetAbbreviation(_forcePerAreaUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Pressure.GetAbbreviation(_forcePerAreaUnit);

      pManager.AddParameter(new GsaLoadCaseParameter());
      pManager.AddGenericParameter("Element list", "G2D",
        "List, Custom Material, 2D Property, 2D Elements or 2D Members to apply load to; either input Prop2d, Element2d, or Member2d, or a text string."
        + Environment.NewLine + "Text string with Element list should take the form:"
        + Environment.NewLine
        + " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)"
        + Environment.NewLine
        + "Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Load Name", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Axis", "Ax",
        "Load axis (default Local). " + Environment.NewLine + "Accepted inputs are:"
        + Environment.NewLine + "0 : Global" + Environment.NewLine + "-1 : Local",
        GH_ParamAccess.item, -1);
      pManager.AddTextParameter("Direction", "Di",
        "Load direction (default z)." + Environment.NewLine + "Accepted inputs are:"
        + Environment.NewLine + "x" + Environment.NewLine + "y" + Environment.NewLine + "z",
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
      pManager.AddParameter(new GsaLoadParameter(), "Face Load", "Ld", "GSA Face Load",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var faceLoad = new GsaFaceLoad();
      
      GsaLoadCaseGoo loadCaseGoo = null;
      if (da.GetData(0, ref loadCaseGoo)) {
        faceLoad.LoadCase = loadCaseGoo.Value;
      } else {
        faceLoad.LoadCase = new GsaLoadCase(1);
      }

      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(1, ref ghTyp)) {
        switch (ghTyp.Value) {
          case GsaListGoo value: {
              if (value.Value.EntityType == EntityType.Element
                || value.Value.EntityType == EntityType.Member) {
                faceLoad.ReferenceList = value.Value;
                faceLoad.ReferenceType = ReferenceType.List;
              } else {
                this.AddRuntimeWarning(
                  "List must be of type Element or Member to apply to face loading");
              }

              if (value.Value.EntityType == EntityType.Member) {
                this.AddRuntimeRemark(
                  "Member list applied to loading in GsaGH will automatically find child elements created from parent member with the load still being applied to elements." + Environment.NewLine + "If you save the file and continue working in GSA please note that the member-loading relationship will be lost.");
              }

              break;
            }
          case GsaElement2dGoo value: {
              faceLoad.RefObjectGuid = value.Value.Guid;
              faceLoad.ReferenceType = ReferenceType.Element;
              break;
            }
          case GsaMember2dGoo value: {
              faceLoad.RefObjectGuid = value.Value.Guid;
              faceLoad.ReferenceType = ReferenceType.MemberChildElements;
              if (_mode != FoldMode.Uniform) {
                this.AddRuntimeWarning(
                  "Member loading will not automatically redistribute non-linear loading to child elements." + Environment.NewLine + "Any non-uniform loading made from Members is likely not what you are after. Please check the load in GSA.");
              } else {
                this.AddRuntimeRemark(
                  "Member loading in GsaGH will automatically find child elements created from parent member with the load still being applied to elements." + Environment.NewLine + "If you save the file and continue working in GSA please note that the member-loading relationship will be lost.");
              }

              break;
            }
          case GsaMaterialGoo value: {
              if (value.Value.Id != 0) {
                this.AddRuntimeWarning(
                "Reference Material must be a Custom Material");
                return;
              }
              faceLoad.RefObjectGuid = value.Value.Guid;
              faceLoad.ReferenceType = ReferenceType.Property;
              break;
            }
          case GsaProp2dGoo value: {
              faceLoad.RefObjectGuid = value.Value.Guid;
              faceLoad.ReferenceType = ReferenceType.Property;
              break;
            }
          default: {
              if (GH_Convert.ToString(ghTyp.Value, out string elemList, GH_Conversion.Both)) {
                faceLoad.FaceLoad.EntityList = elemList;
              }

              break;
            }
        }
      }

      var ghName = new GH_String();
      if (da.GetData(2, ref ghName)) {
        if (GH_Convert.ToString(ghName, out string name, GH_Conversion.Both)) {
          faceLoad.FaceLoad.Name = name;
        }
      }

      faceLoad.FaceLoad.AxisProperty
        = 0; //Note there is currently a bug/undocumented in GsaAPI that cannot translate an integer into axis type (Global, Local or edformed local)
      var ghAx = new GH_Integer();
      if (da.GetData(3, ref ghAx)) {
        GH_Convert.ToInt32(ghAx, out int axis, GH_Conversion.Both);
        if (axis == 0 || axis == -1) {
          faceLoad.FaceLoad.AxisProperty = axis;
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
      }

      faceLoad.FaceLoad.Direction = direc;

      switch (_mode) {
        case FoldMode.Uniform:
          if (_mode == FoldMode.Uniform) {
            faceLoad.FaceLoad.Type = GsaAPI.FaceLoadType.CONSTANT;

            bool prj = false;
            var ghPrj = new GH_Boolean();
            if (da.GetData(5, ref ghPrj)) {
              GH_Convert.ToBoolean(ghPrj, out prj, GH_Conversion.Both);
            }

            faceLoad.FaceLoad.IsProjected = prj;

            var load1 = (Pressure)Input.UnitNumber(this, da, 6, _forcePerAreaUnit);
            faceLoad.FaceLoad.SetValue(0, load1.NewtonsPerSquareMeter);
          }

          break;

        case FoldMode.Variable:
          if (_mode == FoldMode.Variable) {
            faceLoad.FaceLoad.Type = GsaAPI.FaceLoadType.GENERAL;

            bool prj = false;
            var ghPrj = new GH_Boolean();
            if (da.GetData(5, ref ghPrj)) {
              GH_Convert.ToBoolean(ghPrj, out prj, GH_Conversion.Both);
            }

            faceLoad.FaceLoad.IsProjected = prj;
            faceLoad.FaceLoad.SetValue(0,
              ((Pressure)Input.UnitNumber(this, da, 6, _forcePerAreaUnit, true))
             .NewtonsPerSquareMeter);
            faceLoad.FaceLoad.SetValue(1,
              ((Pressure)Input.UnitNumber(this, da, 7, _forcePerAreaUnit, true))
             .NewtonsPerSquareMeter);
            faceLoad.FaceLoad.SetValue(2,
              ((Pressure)Input.UnitNumber(this, da, 8, _forcePerAreaUnit, true))
             .NewtonsPerSquareMeter);
            faceLoad.FaceLoad.SetValue(3,
              ((Pressure)Input.UnitNumber(this, da, 9, _forcePerAreaUnit, true))
             .NewtonsPerSquareMeter);
          }

          break;

        case FoldMode.Point:
          if (_mode == FoldMode.Point) {
            faceLoad.FaceLoad.Type = GsaAPI.FaceLoadType.POINT;

            bool prj = false;
            var ghPrj = new GH_Boolean();
            if (da.GetData(5, ref ghPrj)) {
              GH_Convert.ToBoolean(ghPrj, out prj, GH_Conversion.Both);
            }

            faceLoad.FaceLoad.IsProjected = prj;

            double r = 0;
            da.GetData(7, ref r);
            double s = 0;
            da.GetData(8, ref s);
            faceLoad.FaceLoad.SetValue(0,
              ((Pressure)Input.UnitNumber(this, da, 6, _forcePerAreaUnit)).NewtonsPerSquareMeter);
            this.AddRuntimeWarning("Warning: the position cannot be set in GsaAPI at the moment");
          }

          break;

        case FoldMode.Edge:
          if (_mode == FoldMode.Edge) {
            int edge = 1;
            da.GetData(5, ref edge);

            faceLoad.FaceLoad.SetValue(0,
              ((Pressure)Input.UnitNumber(this, da, 6, _forcePerAreaUnit)).NewtonsPerSquareMeter);
            faceLoad.FaceLoad.SetValue(1,
              Params.Input[7].SourceCount != 0 ?
                ((Pressure)Input.UnitNumber(this, da, 7, _forcePerAreaUnit)).NewtonsPerSquareMeter :
                faceLoad.FaceLoad.Value(0));

            this.AddRuntimeWarning("Warning: edge-load is not yet supported in GsaAPI");
          }

          break;

        default: throw new ArgumentOutOfRangeException();
      }

      da.SetData(0, new GsaLoadGoo(faceLoad));
    }

    protected override void UpdateUIFromSelectedItems() {
      _mode = (FoldMode)Enum.Parse(typeof(FoldMode), _selectedItems[0]);
      _duringLoad = true;
      switch (_selectedItems[0]) {
        case "Uniform":
          Mode1Clicked();
          break;

        case "Variable":
          Mode2Clicked();
          break;

        case "Point":
          Mode3Clicked();
          break;

        case "Edge":
          Mode4Clicked();
          break;
      }

      _duringLoad = false;
      _forcePerAreaUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }

    private void Mode1Clicked() {
      if (!_duringLoad && _mode == FoldMode.Uniform) {
        return;
      }

      RecordUndoEvent("Uniform Parameters");
      _mode = FoldMode.Uniform;

      if (_mode == FoldMode.Edge) {
        while (Params.Input.Count > 5) {
          Params.UnregisterInputParameter(Params.Input[5], true);
        }

        Params.RegisterInputParam(new Param_Boolean());
        Params.RegisterInputParam(new Param_Number());
      } else {
        while (Params.Input.Count > 6) {
          Params.UnregisterInputParameter(Params.Input[6], true);
        }

        Params.RegisterInputParam(new Param_Number());
      }
    }

    private void Mode2Clicked() {
      if (!_duringLoad && _mode == FoldMode.Variable) {
        return;
      }

      RecordUndoEvent("Variable Parameters");
      _mode = FoldMode.Variable;

      if (_mode == FoldMode.Edge) {
        while (Params.Input.Count > 5) {
          Params.UnregisterInputParameter(Params.Input[5], true);
        }

        Params.RegisterInputParam(new Param_Boolean());
        Params.RegisterInputParam(new Param_Number());
        Params.RegisterInputParam(new Param_Number());
        Params.RegisterInputParam(new Param_Number());
        Params.RegisterInputParam(new Param_Number());
      } else {
        while (Params.Input.Count > 6) {
          Params.UnregisterInputParameter(Params.Input[6], true);
        }

        Params.RegisterInputParam(new Param_Number());
        Params.RegisterInputParam(new Param_Number());
        Params.RegisterInputParam(new Param_Number());
        Params.RegisterInputParam(new Param_Number());
      }
    }

    private void Mode3Clicked() {
      if (!_duringLoad && _mode == FoldMode.Point) {
        return;
      }

      RecordUndoEvent("Point Parameters");
      _mode = FoldMode.Point;

      if (_mode == FoldMode.Edge) {
        while (Params.Input.Count > 5) {
          Params.UnregisterInputParameter(Params.Input[5], true);
        }

        Params.RegisterInputParam(new Param_Boolean());
        Params.RegisterInputParam(new Param_Number());
        Params.RegisterInputParam(new Param_Number());
        Params.RegisterInputParam(new Param_Number());
      } else {
        while (Params.Input.Count > 6) {
          Params.UnregisterInputParameter(Params.Input[6], true);
        }

        Params.RegisterInputParam(new Param_Number());
        Params.RegisterInputParam(new Param_Number());
        Params.RegisterInputParam(new Param_Number());
      }
    }

    private void Mode4Clicked() {
      if (!_duringLoad && _mode == FoldMode.Edge) {
        return;
      }

      RecordUndoEvent("Edge Parameters");
      _mode = FoldMode.Edge;

      while (Params.Input.Count > 5) {
        Params.UnregisterInputParameter(Params.Input[5], true);
      }

      Params.RegisterInputParam(new Param_Number());
      Params.RegisterInputParam(new Param_Number());
      Params.RegisterInputParam(new Param_Number());
    }
  }
}
