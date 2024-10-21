using System;
using System.Collections.Generic;
using System.Drawing;

using GH_IO.Serialization;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.Graphics;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.UI;

using Rhino;
using Rhino.DocObjects;

namespace GsaGH.Components {
  public class PreviewDeformed3dSections : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("f1a7f1b4-8c34-43c0-a4f0-6dd207cbf48b");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.PreviewDeformed3dSections;
    private double _defScale = 250;
    private double _maxValue = 1000;
    private double _minValue;
    private int _noDigits;
    private Section3dPreview _section3dPreview;
    public PreviewDeformed3dSections() : base("Preview Deformed 3D Sections", "DeformedPreview3d",
      "Show the deformed 3D cross-section of 1D/2D GSA Elements and Members from a GSA Result.",
      CategoryName.Name(), SubCategoryName.Cat6()) { }

    public override void CreateAttributes() {
      if (!_isInitialised) {
        InitialiseDropdowns();
      }
      m_attributes = new SliderComponentAttributes(this, SetVal, SetMaxMin, _defScale, _minValue, _maxValue, _noDigits,
        "Scale");
      _isInitialised = true;
    }

    protected override void InitialiseDropdowns() {
      _isInitialised = true;
    }

    public override bool Read(GH_IReader reader) {
      _noDigits = reader.GetInt32("noDec");
      _maxValue = reader.GetDouble("valMax");
      _minValue = reader.GetDouble("valMin");
      _defScale = reader.GetDouble("val");
      return base.Read(reader);
    }

    public void SetMaxMin(double max, double min) {
      _maxValue = max;
      _minValue = min;
    }

    public override void SetSelected(int i, int j) { }

    public void SetVal(double value) {
      _defScale = value;
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetInt32("noDec", _noDigits);
      writer.SetDouble("valMax", _maxValue);
      writer.SetDouble("valMin", _minValue);
      writer.SetDouble("val", _defScale);
      return base.Write(writer);
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaElementMemberListParameter());
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddMeshParameter("Mesh", "M", "Analysis layer 3D Section Mesh",
        GH_ParamAccess.item);
      pManager.AddLineParameter("Outlines", "Ls", "The Analyis layer 3D Sections' outlines",
        GH_ParamAccess.list);
      pManager.HideParameter(0);
      pManager.HideParameter(1);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var ghTyp = new GH_ObjectWrapper();
      da.GetData(0, ref ghTyp);
      GsaResult result = Inputs.GetResultInput(this, ghTyp);
      if (result == null) {
        return;
      }

      string elementlist = Inputs.GetElementListDefinition(this, da, 1, result.Model);
      _section3dPreview = new Section3dPreview(result, elementlist, _defScale);

      da.SetData(0, _section3dPreview.Mesh);
      da.SetDataList(1, _section3dPreview.Outlines);

      PostHog.Result(result.CaseType, 1, "Displacement", "DeformedSection3d");
    }

    public override void DrawViewportMeshes(IGH_PreviewArgs args) {
      if (_section3dPreview != null) {
        args.Display.DrawMeshFalseColors(_section3dPreview.Mesh);
      }
    }

    public override void DrawViewportWires(IGH_PreviewArgs args) {
      if (_section3dPreview != null) {
        if (Attributes.Selected) {
          args.Display.DrawLines(_section3dPreview.Outlines, Colours.Element1dSelected,
            2);
        } else {
          args.Display.DrawLines(_section3dPreview.Outlines, Colours.Element1d, 1);
        }
      }
    }

    public override bool IsBakeCapable => _section3dPreview != null;
    public override void BakeGeometry(RhinoDoc doc, ObjectAttributes att, List<Guid> obj_ids) {
      base.BakeGeometry(doc, att, obj_ids);
      if (_section3dPreview == null) {
        return;
      }

      var gH_BakeUtility = new GH_BakeUtility(OnPingDocument());
      _section3dPreview.BakeGeometry(ref gH_BakeUtility, doc, att);
    }
  }
}
