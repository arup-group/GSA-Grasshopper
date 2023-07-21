using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Gradient;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.Graphics;
using GsaGH.Helpers.GsaApi;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.UI;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Display;
using Rhino.Geometry;
using AngleUnit = OasysUnits.Units.AngleUnit;
using EnergyUnit = OasysUnits.Units.EnergyUnit;
using ForceUnit = OasysUnits.Units.ForceUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;
using Line = Rhino.Geometry.Line;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get Element1D results
  /// </summary>
  public class ShowDeformed3dSections : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("f1a7f1b4-8c34-43c0-a4f0-6dd207cbf48b");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.ShowSection3d;
    private double _defScale = 250;
    private double _maxValue = 1000;
    private double _minValue;
    private int _noDigits;
    private GsaSection3dPreview _section3dPreview;
    public ShowDeformed3dSections() : base("1D Contour Results", "ContourElem1d",
      "Displays GSA 1D Element Results as Contour", CategoryName.Name(), SubCategoryName.Cat5()) { }

    public override void CreateAttributes() {
      m_attributes = new SliderComponentAttributes(this, SetVal, SetMaxMin, _defScale, _minValue, _maxValue, _noDigits,
        "Scale");
      _isInitialised = true;
    }
    protected override void InitialiseDropdowns() { }
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
      pManager.AddGenericParameter("Element filter list", "El",
        "Filter results by list (by default 'all')" + Environment.NewLine
        + "Input a GSA List or a text string taking the form:" + Environment.NewLine
        + " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)"
        + Environment.NewLine
        + "Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item);
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

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaResultGoo resultGoo = null;
      da.GetData(0, ref resultGoo);
      GsaResult result = resultGoo.Value;

      string elementlist = Inputs.GetElementListNameForesults(this, da, 1);
      _section3dPreview = new GsaSection3dPreview(result, elementlist, _defScale);

      da.SetData(0, _section3dPreview.Mesh);
      da.SetDataList(1, _section3dPreview.Outlines);

      PostHog.Result(result.Type, 1, "Displacement", "DeformedSection3d");
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
  }
}
