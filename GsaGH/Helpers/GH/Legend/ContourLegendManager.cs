using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using GH_IO.Serialization;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;

using OasysGH.Components;

namespace GsaGH.Helpers {
  public class ContourLegendManager {
    private readonly IContourLegendConfiguration _configuration;
    private readonly IContourLegend _legend;
    private readonly LegendMenuBuilder _menuBuilder;

    public ContourLegendManager() {
      _configuration = new ContourLegendConfiguration();
      _legend = new ContourLegend(_configuration);
      _menuBuilder = new LegendMenuBuilder();
    }

    public ContourLegendManager(
      IContourLegendConfiguration configuration, IContourLegend legend, LegendMenuBuilder menuBuilder) {
      _configuration = configuration;
      _legend = legend;
      _menuBuilder = menuBuilder;
    }

    //drawing
    public void DrawLegend(
      IGH_PreviewArgs args, string title, string bottomText,
      List<(int startY, int endY, Color gradientColor)> gradients) {
      _legend.DrawLegendRectangle(args, title, bottomText, gradients);
    }

    //menu
    public ToolStripMenuItem CreateMenu(GH_OasysDropDownComponent component, Action updateUI) {
      LegendMenuBuilder.SetLegendScaleDelegate setScaleDelegate = UpdateScale;
      return _menuBuilder.CreateLegendToolStripMenuItem(component, updateUI, setScaleDelegate, _configuration.Scale);
    }

    //configuration
    public void UpdateScale(double scale) {
      _configuration.SetLegendScale(scale);
    }

    public double GetScale() {
      return _configuration.Scale;
    }

    public bool ToggleVisibility() {
      return _configuration.ToggleLegendVisibility();
    }

    public bool IsLegendVisible => _configuration.IsVisible;

    public void DeserialiseLegendState(GH_IReader reader) {
      _configuration.DeserializeLegendState(reader);
    }

    public void SerialiseLegendState(GH_IWriter writer) {
      _configuration.SerializeLegendState(writer);
    }

    public (int Width, int Height) GetBitmapSize() {
      return (_configuration.Bitmap.Width, _configuration.Bitmap.Height);
    }

    public void SetTextValues(List<string> values) {
      _configuration.SetTextValues(values);
    }

    public void SetPositionYValues(List<int> values) {
      _configuration.SetValuePositionsY(values);
    }
  }

}
