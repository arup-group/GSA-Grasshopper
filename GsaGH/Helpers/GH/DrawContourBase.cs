using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using GH_IO.Serialization;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Special;

using GsaGH.Helpers.Graphics;
using GsaGH.Parameters.Results;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.UI;
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Helpers.GH {
  public class DrawContourBase : GH_OasysDropDownComponent {
    public DrawContourBase(string name, string nickname, string description, string category, string subCategory) :
      base(name, nickname, description, category, subCategory) { }

    public override Guid ComponentGuid { get; }
    public override OasysPluginInfo PluginInfo { get; }

    protected readonly ContourLegendManager _contourLegendMenager = new ContourLegendManager();
    protected List<(int startY, int endY, Color gradientColor)> _gradients
      = new List<(int startY, int endY, Color gradientColor)>();
    protected string _resType;
    protected string _case = string.Empty;
    protected double _defScale = 250;
    protected double _maxValue = 1000;
    protected double _minValue;
    protected int _noDigits;
    protected bool _slider = true;
    protected LengthUnit _lengthResultUnit = DefaultUnits.LengthUnitResult;
    protected EnvelopeMethod _envelopeType = EnvelopeMethod.Absolute;
    protected List<ToolStripMenuItem> extraUnitMenus = new List<ToolStripMenuItem>();

    protected override void RegisterInputParams(GH_InputParamManager pManager) { }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      IQuantity length = new Length(0, _lengthResultUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Colours", "LC", "Legend Colours", GH_ParamAccess.list);
      pManager.AddGenericParameter("Values [" + lengthunitAbbreviation + "]", "LT", "Legend Values",
        GH_ParamAccess.list);
    }

    public override bool Read(GH_IReader reader) {
      _slider = reader.GetBoolean("slider");
      _noDigits = reader.GetInt32("noDec");
      _maxValue = reader.GetDouble("valMax");
      _minValue = reader.GetDouble("valMin");
      _defScale = reader.GetDouble("val");
      _contourLegendMenager.DeserialiseLegendState(reader);
      if (reader.ItemExists("envelope")) {
        _envelopeType = (EnvelopeMethod)Enum.Parse(typeof(EnvelopeMethod), reader.GetString("envelope"));
      }

      _lengthResultUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("length"));

      return base.Read(reader);
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetBoolean("slider", _slider);
      writer.SetInt32("noDec", _noDigits);
      writer.SetDouble("valMax", _maxValue);
      writer.SetDouble("valMin", _minValue);
      writer.SetDouble("val", _defScale);
      writer.SetString("length", Length.GetAbbreviation(_lengthResultUnit));
      writer.SetString("envelope", _envelopeType.ToString());
      _contourLegendMenager.SerialiseLegendState(writer);
      return base.Write(writer);
    }

    public override void CreateAttributes() {
      if (!_isInitialised) {
        InitialiseDropdowns();
      }

      m_attributes = new DropDownSliderComponentAttributes(this, SetSelected, _dropDownItems, _selectedItems, _slider,
        SetVal, SetMaxMin, _defScale, _maxValue, _minValue, _noDigits, _spacerDescriptions);
    }

    public override void SetSelected(int i, int j) { }

    protected override void SolveInternal(IGH_DataAccess da) { }

    protected override void InitialiseDropdowns() { }

    public override void DrawViewportWires(IGH_PreviewArgs args) {
      base.DrawViewportWires(args);
      _contourLegendMenager.DrawLegend(args, _resType, _case, _gradients);
    }

    public void SetVal(double value) {
      _defScale = value;
    }

    public void SetMaxMin(double max, double min) {
      _maxValue = max;
      _minValue = min;
    }

    public void ClearLegendGradients() {
      _gradients = new List<(int startY, int endY, Color gradientColor)>();
    }

    public void AddLegendValues(List<string> values, List<int> positionsY) {
      _contourLegendMenager.SetTextValues(values);
      _contourLegendMenager.SetPositionYValues(positionsY);
    }

    public ToolStripMenuItem GetLegendMenu() {
      return _contourLegendMenager.CreateMenu(this, () => base.UpdateUI());
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);

      ToolStripMenuItem envelopeMenu = GenerateToolStripMenuItem.GetEnvelopeSubMenuItem(_envelopeType, UpdateEnvelope);
      menu.Items.Add(envelopeMenu);

      Menu_AppendItem(menu, "Show Legend", ShowLegend, true, _contourLegendMenager.IsLegendVisible);

      var gradient = new GH_GradientControl();
      gradient.CreateAttributes();
      var extract = new ToolStripMenuItem("Extract Default Gradient", gradient.Icon_24x24, (s, e) => CreateGradient());
      menu.Items.Add(extract);

      ToolStripMenuItem lengthUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Displacement",
        EngineeringUnits.Length, Length.GetAbbreviation(_lengthResultUnit), UpdateLength);

      var unitsMenu = new ToolStripMenuItem("Select Units", Resources.ModelUnits);
      unitsMenu.DropDownItems.AddRange(new ToolStripItem[] {
        lengthUnitsMenu,
      });
      unitsMenu.DropDownItems.AddRange(extraUnitMenus.ToArray());
      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;
      menu.Items.Add(unitsMenu);
      ToolStripMenuItem legendScaleMenu = GetLegendMenu();
      menu.Items.Add(legendScaleMenu);

      Menu_AppendSeparator(menu);
    }

    internal void GenerateUnitMenuItems(List<ToolStripMenuItem> items) {
      extraUnitMenus.AddRange(items);
    }

    internal GH_GradientControl CreateGradient(GH_Document doc = null) {
      doc ??= OnPingDocument();
      var gradient = new GH_GradientControl();
      gradient.CreateAttributes();

      gradient.Gradient = Colours.Stress_Gradient();
      gradient.Gradient.NormalizeGrips();
      gradient.Params.Input[0].AddVolatileData(new GH_Path(0), 0, -1);
      gradient.Params.Input[1].AddVolatileData(new GH_Path(0), 0, 1);
      gradient.Params.Input[2].AddVolatileDataList(new GH_Path(0), new List<double>() {
        -1,
        -0.666,
        -0.333,
        0,
        0.333,
        0.666,
        1,
      });

      gradient.Attributes.Pivot = new PointF(Attributes.Bounds.X - gradient.Attributes.Bounds.Width - 50,
        Params.Input[2].Attributes.Bounds.Y - (gradient.Attributes.Bounds.Height / 4) - 6);

      doc.AddObject(gradient, false);
      Params.Input[2].RemoveAllSources();
      Params.Input[2].AddSource(gradient.Params.Output[0]);

      UpdateUI();
      return gradient;
    }

    internal void ShowLegend(object sender, EventArgs e) {
      _contourLegendMenager.ToggleVisibility();
      ExpirePreview(true);
    }

    internal void UpdateEnvelope(string type) {
      _envelopeType = (EnvelopeMethod)Enum.Parse(typeof(EnvelopeMethod), type);
      ExpirePreview(true);
      base.UpdateUI();
    }

    internal void UpdateLength(string unit) {
      _lengthResultUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }

    protected void ReDrawComponent() {
      var pivot = new PointF(Attributes.Pivot.X, Attributes.Pivot.Y);
      CreateAttributes();
      Attributes.Pivot = pivot;
      Attributes.ExpireLayout();
      Attributes.PerformLayout();
    }
  }
}
