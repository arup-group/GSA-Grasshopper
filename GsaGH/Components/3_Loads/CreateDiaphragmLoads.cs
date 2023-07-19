using System;
using System.Collections.Generic;
using System.Drawing;
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
using EntityType = GsaGH.Parameters.EntityType;

namespace GsaGH.Components {
  public class CreateDiaphragmLoads : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("34df9639-ba4f-4275-8c53-c1c39d7b35ef");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    //protected override Bitmap Icon => Resources.FaceLoad;

    private PressureUnit _forcePerAreaUnit = DefaultUnits.ForcePerAreaUnit;

    public CreateDiaphragmLoads() : base("Create Diaphragm Load", "DiaphragmLoad", "Create GSA Diaphragm Load",
      CategoryName.Name(), SubCategoryName.Cat3()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _forcePerAreaUnit
          = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[0]);
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Pressure.GetAbbreviation(_forcePerAreaUnit);
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
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerArea));
      _selectedItems.Add(Pressure.GetAbbreviation(_forcePerAreaUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Pressure.GetAbbreviation(_forcePerAreaUnit);

      pManager.AddIntegerParameter("Load case", "LC", "Load case number (default 1)",
        GH_ParamAccess.item, 1);
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
      pManager.AddParameter(new GsaLoadParameter(), "Diaphragm Load", "Ld", "GSA Diaphragm Load",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var faceLoad = new GsaFaceLoad();
      int loadCase = 1;
      var ghLc = new GH_Integer();
      if (da.GetData(0, ref ghLc)) {
        GH_Convert.ToInt32(ghLc, out loadCase, GH_Conversion.Both);
      }

      faceLoad.FaceLoad.Case = loadCase;

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
              this.AddRuntimeRemark(
                "Member loading in GsaGH will automatically find child elements created from parent member with the load still being applied to elements." + Environment.NewLine + "If you save the file and continue working in GSA please note that the member-loading relationship will be lost.");

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
                faceLoad.FaceLoad.Elements = elemList;
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

      faceLoad.FaceLoad.Type = GsaAPI.FaceLoadType.CONSTANT;

      bool prj = false;
      var ghPrj = new GH_Boolean();
      if (da.GetData(5, ref ghPrj)) {
        GH_Convert.ToBoolean(ghPrj, out prj, GH_Conversion.Both);
      }

      faceLoad.FaceLoad.IsProjected = prj;

      var load1 = (Pressure)Input.UnitNumber(this, da, 6, _forcePerAreaUnit);
      faceLoad.FaceLoad.SetValue(0, load1.NewtonsPerSquareMeter);

      da.SetData(0, new GsaLoadGoo(faceLoad));
    }

    protected override void UpdateUIFromSelectedItems() {
      _forcePerAreaUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
