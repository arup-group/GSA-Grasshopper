using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.Export;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.Graphics;
using GsaGH.Helpers.GsaApi.EnumMappings;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using Rhino.Geometry;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Components {
  public class Preview3dSections : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("a3f80eb4-c876-4582-ad7a-d2bb9acf5c8d");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Preview3dSections;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    private GsaSection3dPreview _analysisSection3dPreview;
    private GsaSection3dPreview _designSection3dPreview;

    public Preview3dSections() : base("Preview 3D Sections", "Preview3d",
      "Show the 3D cross-section of 1D/2D GSA Elements and Members in a GSA model.",
      CategoryName.Name(), SubCategoryName.Cat6()) { }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Unit",
        "Settings",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
      base.UpdateUI();
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Element/Member 1D/2D", "Geo",
        "Element1D, Element2D, Member1D or Member2D to preview the 3D cross-section for." +
        $"{Environment.NewLine}You can also input models or lists with geometry to preview.", GH_ParamAccess.list);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddMeshParameter("AnalysisLayer Mesh", "AM", "Analysis layer 3D Section Mesh",
        GH_ParamAccess.item);
      pManager.HideParameter(0);
      pManager.AddLineParameter("AnalysisLayer Outlines", "ALs", "The Analyis layer 3D Sections' outlines",
        GH_ParamAccess.list);
      pManager.HideParameter(1);
      pManager.AddMeshParameter("DesignLayer Mesh", "DM", "Design layer 3D Section Mesh",
        GH_ParamAccess.item);
      pManager.HideParameter(2);
      pManager.AddLineParameter("DesignLayer Outlines", "DLs", "The Design layer 3D Sections' outlines",
        GH_ParamAccess.list);
      pManager.HideParameter(3);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var ghTypes = new List<GH_ObjectWrapper>();
      if (da.GetDataList(0, ghTypes)) {
        var elem1ds = new List<GsaElement1d>();
        var elem2ds = new List<GsaElement2d>();
        var mem1ds = new List<GsaMember1d>();
        var mem2ds = new List<GsaMember2d>();
        var models = new List<GsaModel>();
        var lists = new List<GsaList>();
        for (int i = 0; i < ghTypes.Count; i++) {
          GH_ObjectWrapper ghTyp = ghTypes[i];
          if (ghTyp == null) {
            this.AddRuntimeWarning(
              "Geometry input (index: " + i + ") is null and has been ignored");
            continue;
          }

          switch (ghTyp.Value) {
            case GsaModelGoo modelGoo:
              models.Add(modelGoo.Value);
              break;

            case GsaListGoo listGoo:
              lists.Add(listGoo.Value);
              break;

            case GsaElement1dGoo element1dGoo:
              elem1ds.Add(element1dGoo.Value);
              break;

            case GsaElement2dGoo element2dGoo:
              elem2ds.Add(element2dGoo.Value);
              break;

            case GsaMember1dGoo member1dGoo:
              mem1ds.Add(member1dGoo.Value.Duplicate());
              break;

            case GsaMember2dGoo member2dGoo:
              mem2ds.Add(member2dGoo.Value.Duplicate());
              break;

            default: {
                string type = ghTyp.Value.GetType().ToString();
                type = type.Replace("GsaGH.Parameters.", string.Empty);
                type = type.Replace("Goo", string.Empty);
                this.AddRuntimeError("Unable to convert Geometry input parameter of type " + type
                  + Environment.NewLine
                  + " to Element1D, Element2D, Member1D, or Member2D");
                continue;
              }
          }
        }

        if (!(elem1ds.Count > 0)) {
          elem1ds = null;
        }

        if (!(elem2ds.Count > 0)) {
          elem2ds = null;
        }

        if (!(mem1ds.Count > 0)) {
          mem1ds = null;
        }

        if (!(mem2ds.Count > 0)) {
          mem2ds = null;
        }

        if (models is null & elem1ds is null & elem2ds is null & mem1ds is null & mem2ds is null) {
          this.AddRuntimeWarning("Input parameter failed to collect data");
          return;
        }

        var model = new GsaModel();
        model.Model.UiUnits().LengthLarge = UnitMapping.GetApiUnit(_lengthUnit);
        if (models != null) {
          if (models.Count > 0) {
            model = models.Count > 1
              ? MergeModels.MergeModel(models, this, Length.Zero) :
              models[0].Clone();
          }
        }

        // Assemble model
        GsaAPI.Model previewModel = Assembler.AssembleForPreview(
          model, lists, elem1ds, elem2ds, mem1ds, mem2ds, _lengthUnit);

        _analysisSection3dPreview = null;
        _designSection3dPreview = null;

        var steps = new List<int> {
        0, 1,
        };
        Parallel.ForEach(steps, i => {
          switch (i) {
            case 0:
              if (previewModel.Elements().Count == 0) {
                break;
              }

              _analysisSection3dPreview =
                new GsaSection3dPreview(previewModel, model.ModelUnit, Layer.Analysis);
              break;

            case 1:
              if (previewModel.Members().Count == 0) {
                break;
              }

              _designSection3dPreview =
                new GsaSection3dPreview(previewModel, model.ModelUnit, Layer.Design);
              break;
          }
        });

        if (_analysisSection3dPreview != null) {
          da.SetData(0, _analysisSection3dPreview.Mesh);
          da.SetDataList(1, _analysisSection3dPreview.Outlines);
        }

        if (_designSection3dPreview != null) {
          da.SetData(2, _designSection3dPreview.Mesh);
          da.SetDataList(3, _designSection3dPreview.Outlines);
        }
      }
    }

    public override void DrawViewportMeshes(IGH_PreviewArgs args) {
      if (_analysisSection3dPreview != null) {
        args.Display.DrawMeshFalseColors(_analysisSection3dPreview.Mesh);
      }

      if (_designSection3dPreview != null) {
        args.Display.DrawMeshFalseColors(_designSection3dPreview.Mesh);
      }
    }

    public override void DrawViewportWires(IGH_PreviewArgs args) {
      if (_analysisSection3dPreview != null) {
        if (Attributes.Selected) {
          args.Display.DrawLines(_analysisSection3dPreview.Outlines, Colours.Element1dSelected,
            2);
        } else {
          args.Display.DrawLines(_analysisSection3dPreview.Outlines, Colours.Element1d, 1);
        }
      }

      if (_designSection3dPreview != null) {
        if (Attributes.Selected) {
          args.Display.DrawLines(_designSection3dPreview.Outlines, Colours.Member2dEdgeSelected,
            2);
        } else {
          args.Display.DrawLines(_designSection3dPreview.Outlines, Colours.Member2dEdge, 1);
        }
      }
    }
  }
}
