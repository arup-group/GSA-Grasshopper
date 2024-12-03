using System.Collections.Generic;
using System.Drawing;

using GH_IO.Serialization;

namespace GsaGH.Helpers {
  public interface IContourLegendConfiguration {
    IReadOnlyCollection<string> Values { get; }
    IReadOnlyCollection<int> ValuePositionsY { get; }
    Bitmap Bitmap { get; }
    double Scale { get; }
    bool IsVisible { get; }
    void SetTextValues(List<string> values);
    void SetValuePositionsY(List<int> positions);
    void SetLegendScale(double scale);
    bool IsLegendDisplayable();
    bool ToggleLegendVisibility();
    void DeserializeLegendState(GH_IReader reader);
    void SerializeLegendState(GH_IWriter writer);
  }
}
