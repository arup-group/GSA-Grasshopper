using System;
using System.Collections.Generic;
using System.Drawing;

using GH_IO.Serialization;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

using OasysUnits;

using Rhino.Geometry;

namespace GsaGH.Components {
  public class CreateGridPlane : GH_OasysDropDownComponent {
    private enum FoldMode {
      General,
      Storey,
    }

    public override Guid ComponentGuid => new Guid("95c9281a-739b-4480-a2d0-8b04ab0250bd");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateGridPlane;
    private readonly List<string> _type = new List<string>(new string[] {
      "General",
      "Storey",
    });
    private FoldMode _mode = FoldMode.General;

    public CreateGridPlane() : base("Create Grid Plane", "GridPlane", "Create GSA Grid Plane",
      CategoryName.Name(), SubCategoryName.Cat3()) { }

    public override bool Read(GH_IReader reader) {
      if (reader.ItemExists("Mode")) {
        _mode = (FoldMode)reader.GetInt32("Mode");
        InitialiseDropdowns();
      }

      if (_mode != FoldMode.Storey || Params.Input.Count >= 5) {
        return base.Read(reader);
      }

      Params.RegisterInputParam(new Param_GenericObject());
      Params.RegisterInputParam(new Param_GenericObject());

      return base.Read(reader);
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      if (i == 0) {
        switch (_selectedItems[0]) {
          case "General":
            Mode1Clicked();
            break;

          case "Storey":
            Mode2Clicked();
            break;
        }
      }

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      Params.Input[2].Name = "Grid Elevation in model units";

      if (_mode != FoldMode.Storey) {
        return;
      }

      if (Params.Input.Count < 5) {
        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new Param_GenericObject());
      }

      Params.Input[4].NickName = "tA";
      Params.Input[4].Name = "Tolerance Above";
      Params.Input[4].Description = "Tolerance Above Grid Plane";
      Params.Input[4].Access = GH_ParamAccess.item;
      Params.Input[4].Optional = true;

      Params.Input[5].NickName = "tB";
      Params.Input[5].Name = "Tolerance Below";
      Params.Input[5].Description = "Tolerance Below Grid Plane";
      Params.Input[5].Access = GH_ParamAccess.item;
      Params.Input[5].Optional = true;
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] {
        "Type",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(_type);
      _selectedItems.Add(_mode.ToString());

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Plane", "P",
        "Plane for Axis and Grid Plane definition. Note that an XY-plane will be created with an axis origin Z = 0 "
        + "and the height location will be controlled by Grid Plane elevation. For all none-XY plane inputs, the Grid Plane elevation will be 0",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Grid Plane ID", "ID",
        "GSA Grid Plane ID. Setting this will replace any existing Grid Planes in GSA model",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Grid Elevation in model units", "Ev",
        "Grid Elevation [Optional]. Note that this value will be added to Plane origin location in the plane's normal axis direction.",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Grid Plane Name", GH_ParamAccess.item);

      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;

      _mode = FoldMode.General;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaGridPlaneSurfaceParameter(), "Grid Plane", "GP", "GSA Grid Plane",
        GH_ParamAccess.item);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      Plane pln = Plane.WorldXY;

      var ghPlane = new GH_Plane();
      if (da.GetData(0, ref ghPlane)) {
        GH_Convert.ToPlane(ghPlane, ref pln, GH_Conversion.Both);
      }

      var gps = new GsaGridPlaneSurface(pln, Params.Input[0].SourceCount == 0);

      var ghInteger = new GH_Integer();
      if (da.GetData(1, ref ghInteger)) {
        GH_Convert.ToInt32(ghInteger, out int id, GH_Conversion.Both);
        gps.GridPlaneId = id;
      }

      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(2, ref ghTyp)) {
        string elevationIn = ghTyp.Value.ToString();
        double elevation = 0;
        if (elevationIn != string.Empty && elevationIn.ToLower() != "0") {
          if (Length.TryParse(elevationIn, out Length newElevation)) {
            gps.SetElevation(newElevation);
          } else if (double.TryParse(elevationIn, out elevation)) {
            gps.SetElevation(elevation);
          } else {
            this.AddRuntimeError("Unable to parse elevation input to Length or number");
            return;
          }
        }
      }

      var ghString = new GH_String();
      if (da.GetData(3, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string name, GH_Conversion.Both)) {
          gps.GridPlane.Name = name;
        }
      }

      if (_mode == FoldMode.General) {
        gps.GridPlane.IsStoreyType = false;
      } else {
        gps.GridPlane.IsStoreyType = true;
        ghTyp = new GH_ObjectWrapper();
        if (da.GetData(4, ref ghTyp)) {
          string tolIn = ghTyp.Value.ToString();
          if (tolIn != string.Empty && tolIn.ToLower() != "auto") {
            try {
              Length.Parse(tolIn);
              gps.StoreyToleranceAbove = tolIn;
            } catch (Exception e) {
              if (double.TryParse(tolIn, out double _)) {
                gps.StoreyToleranceAbove = tolIn;
              } else {
                this.AddRuntimeWarning(e.Message);
              }
            }
          }
        }

        ghTyp = new GH_ObjectWrapper();
        if (da.GetData(5, ref ghTyp)) {
          string tolIn = ghTyp.Value.ToString();
          if (tolIn != string.Empty && tolIn.ToLower() != "auto") {
            try {
              var newTolerance = Length.Parse(tolIn);
              gps.StoreyToleranceBelow = tolIn;
            } catch (Exception e) {
              if (double.TryParse(tolIn, out double _)) {
                gps.StoreyToleranceBelow = tolIn;
              } else {
                this.AddRuntimeWarning(e.Message);
              }
            }
          }
        }
      }

      da.SetData(0, new GsaGridPlaneSurfaceGoo(gps));
    }

    protected override void UpdateUIFromSelectedItems() {
      _mode = (FoldMode)Enum.Parse(typeof(FoldMode), _selectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }

    private void Mode1Clicked() {
      if (_mode == FoldMode.General) {
        return;
      }

      RecordUndoEvent("General Parameters");
      _mode = FoldMode.General;

      while (Params.Input.Count > 4) {
        Params.UnregisterInputParameter(Params.Input[4], true);
      }
    }

    private void Mode2Clicked() {
      if (_mode == FoldMode.Storey) {
        return;
      }

      RecordUndoEvent("Storey Parameters");
      _mode = FoldMode.Storey;

      Params.RegisterInputParam(new Param_GenericObject());
      Params.RegisterInputParam(new Param_GenericObject());
    }
  }
}
