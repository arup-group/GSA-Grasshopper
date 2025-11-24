using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers;
using GsaGH.Helpers.Assembly;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.Graphics;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Units;

using OasysUnits;

using LengthUnit = OasysUnits.Units.LengthUnit;
namespace GsaGH.Components {
  public class Preview3dSections : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("a3f80eb4-c876-4582-ad7a-d2bb9acf5c8d");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Preview3dSections;
    private Section3dPreview _analysisSection3dPreview;
    private Section3dPreview _designSection3dPreview;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;

    public Preview3dSections() : base("Preview 3D Sections", "Preview3d",
      "Show the 3D cross-section of 1D/2D GSA Elements and Members in a GSA model.",
      CategoryName.Name(), SubCategoryName.Cat6()) { }

    protected override void InitialiseDropdowns() {
      // this has been a drop down component before
      _spacerDescriptions = new List<string>();
      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _isInitialised = true;
    }

    public override void SetSelected(int i, int j) {
      // this has been a drop down component before
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

    protected override void BeforeSolveInstance() {
      // reset preview
      _analysisSection3dPreview = null;
      _designSection3dPreview = null;
      base.BeforeSolveInstance();
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var unitsName = new HashSet<string>();
      var ghTypes = new List<GH_ObjectWrapper>();
      if (da.GetDataList(0, ghTypes)) {
        var elem1ds = new List<GsaElement1D>();
        var elem2ds = new List<GsaElement2D>();
        var mem1ds = new List<GsaMember1D>();
        var mem2ds = new List<GsaMember2D>();
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
              unitsName.Add(Length.GetAbbreviation(modelGoo.Value.ModelUnit));
              models.Add(modelGoo.Value);
              break;

            case GsaListGoo listGoo:
              lists.Add(listGoo.Value);
              break;

            case GsaElement1dGoo element1dGoo:
              unitsName.Add(Length.GetAbbreviation(element1dGoo.Value.LengthUnit));
              elem1ds.Add(element1dGoo.Value);
              break;

            case GsaElement2dGoo element2dGoo:
              unitsName.Add(Length.GetAbbreviation(element2dGoo.Value.LengthUnit));
              elem2ds.Add(element2dGoo.Value);
              break;

            case GsaMember1dGoo member1dGoo:
              unitsName.Add(Length.GetAbbreviation(member1dGoo.Value.LengthUnit));
              mem1ds.Add(member1dGoo.Value);
              break;

            case GsaMember2dGoo member2dGoo:
              unitsName.Add(Length.GetAbbreviation(member2dGoo.Value.LengthUnit));
              mem2ds.Add(member2dGoo.Value);
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

        if (unitsName.Count > 1) {
          this.AddRuntimeError("Multiple length units detected which is not allowed");
        }

        _lengthUnit = OasysUnitsSetup.Default.UnitParser.Parse<LengthUnit>(unitsName.First());

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
        if (models != null) {
          if (models.Count > 0) {
            model = models.Count > 1
              ? MergeModels.MergeModel(models, this, Length.Zero) :
              models[0].Clone();
          }
        }

        // Assemble model
        var assembly = new ModelAssembly(model, lists, elem1ds, elem2ds, mem1ds, mem2ds, _lengthUnit);
        GsaAPI.Model previewModel = assembly.GetModel();

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
                new Section3dPreview(previewModel, _lengthUnit, Layer.Analysis);
              break;

            case 1:
              if (previewModel.Members().Count == 0) {
                break;
              }

              _designSection3dPreview =
                new Section3dPreview(previewModel, _lengthUnit, Layer.Design);
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
