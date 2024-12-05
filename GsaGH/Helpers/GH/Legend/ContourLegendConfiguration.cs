using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using GH_IO.Serialization;

namespace GsaGH.Helpers.GH {
  public class ContourLegendConfiguration : IContourLegendConfiguration {
    public IReadOnlyCollection<string> Values { get; private set; } = new List<string>();
    public IReadOnlyCollection<int> ValuePositionsY { get; private set; } = new List<int>();
    public Bitmap Bitmap { get; private set; }
    public double Scale { get; private set; } = 1.0;
    public bool IsVisible { get; private set; } = true;

    public readonly int DefaultWidth = 15;
    public readonly int DefaultHeight = 120;
    /// <summary>
    ///   Key used to de/serialise scale of the legend
    /// </summary>
    public static string ScaleKey => "legendScale";
    /// <summary>
    ///   Key used to de/serialise visibility of the legend
    /// </summary>
    public static string VisibilityKey => "legend";

    public ContourLegendConfiguration() {
      ScaleBitmap();
    }

    /// <summary>
    ///   Sets the list of legend values.
    /// </summary>
    public void SetTextValues(List<string> values) {
      Values = values ?? throw new ArgumentNullException(nameof(values), "Values cannot be null.");
    }

    /// <summary>
    ///   Sets the Y positions for the legend values.
    /// </summary>
    public void SetValuePositionsY(List<int> positions) {
      ValuePositionsY
        = positions ?? throw new ArgumentNullException(nameof(positions), "Value positions cannot be null.");
    }

    public void SetLegendScale(double scale) {
      if (scale <= 0) {
        throw new ArgumentOutOfRangeException(nameof(scale), "Scale must be greater than zero.");
      }

      Scale = scale;
      ScaleBitmap();
    }

    public bool ToggleLegendVisibility() {
      IsVisible = !IsVisible;
      return IsVisible;
    }

    private void ScaleBitmap() {
      Bitmap = new Bitmap((int)(DefaultWidth * Scale), (int)(DefaultHeight * Scale));
    }

    public bool IsLegendDisplayable() {
      return Values.Any() && ValuePositionsY.Any() && IsVisible;
    }

    /// <summary>
    /// Deserializes the legend's visibility and scale values from an OI archive.
    /// </summary>
    /// <param name="reader">The reader instance for accessing serialized data.</param>
    public void DeserializeLegendState(GH_IReader reader) {
      ValidateSerializationObject(reader);

      if (reader.ItemExists(ScaleKey)) {
        SetLegendScale(reader.GetDouble(ScaleKey));
      }

      if (reader.ItemExists(VisibilityKey)) {
        IsVisible = reader.GetBoolean(VisibilityKey);
      }
    }

    /// <summary>
    /// Serializes the legend's visibility and scale values to an OI archive.
    /// </summary>
    /// <param name="writer">The writer instance for saving serialized data.</param>
    public void SerializeLegendState(GH_IWriter writer) {
      ValidateSerializationObject(writer);

      writer.SetDouble(ScaleKey, Scale);
      writer.SetBoolean(VisibilityKey, IsVisible);
    }

    /// <summary>
    /// Validates the provided serialization object.
    /// </summary>
    /// <param name="serializationObject">The serialization object to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown if the object is null.</exception>
    private static void ValidateSerializationObject(object serializationObject) {
      if (serializationObject == null) {
        throw new ArgumentNullException(nameof(serializationObject), "Serialization object cannot be null.");
      }
    }
  }
}
