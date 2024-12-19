using System;
using System.Windows.Forms;

using GH_IO.Serialization;

using GsaGH.Helpers.GH;

using OasysGH.Components;

namespace GsaGH.Helpers {
  public class ContourLegendManager {
    public readonly ContourLegendConfiguration Configuration;
    public readonly ContourLegend Legend;
    public readonly LegendMenuBuilder MenuBuilder;

    public static ContourLegendManager GetDefault() {
      var _configuration = ContourLegendConfiguration.GetDefault();
      var _legend = new ContourLegend(_configuration);
      var _menuBuilder = new LegendMenuBuilder();

      return new ContourLegendManager(_configuration, _legend, _menuBuilder);
    }

    public ContourLegendManager(
      ContourLegendConfiguration configuration, ContourLegend legend, LegendMenuBuilder menuBuilder) {
      Configuration = configuration;
      Legend = legend;
      MenuBuilder = menuBuilder;
    }

    //menu
    public ToolStripMenuItem CreateMenu(GH_OasysDropDownComponent component, Action updateUI) {
      LegendMenuBuilder.SetLegendScaleDelegate setScaleDelegate = UpdateScale;
      return MenuBuilder.CreateLegendToolStripMenuItem(component, updateUI, setScaleDelegate, Configuration.Scale);
    }

    //configuration
    public void UpdateScale(double scale) {
      Configuration.Scale = scale;
    }

    public void DeserialiseLegendState(GH_IReader reader) {
      Configuration.DeserializeLegendState(reader);
    }

    public void SerialiseLegendState(GH_IWriter writer) {
      Configuration.SerializeLegendState(writer);
    }
  }
}
