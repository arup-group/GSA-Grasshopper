using System;
using System.Collections.Generic;
using System.Linq;

using GH_IO.Serialization;

namespace GsaGH.Helpers.GH {
  public class ContourLegendConfiguration {
    /// <summary>
    ///   Key used to de/serialise scale of the legend
    /// </summary>
    public const string ScaleKey = "legendScale";
    /// <summary>
    ///   Key used to de/serialise visibility of the legend
    /// </summary>
    public const string VisibilityKey = "legend";
    /// <summary>
    ///   Key used to de/serialise width of the legend
    /// </summary>
    public const string WidthKey = "legendWidth";
    /// <summary>
    ///   Key used to de/serialise height of the legend
    /// </summary>
    public const string HeightKey = "legendHeight";
    public IReadOnlyCollection<string> Values { get; private set; } = new List<string>();
    public IReadOnlyCollection<int> ValuePositionsY { get; private set; } = new List<int>();
    public bool IsVisible { get; private set; } = true;
    public double Scale {
      get => _scale;
      set {
        if (value <= 0) {
          throw new ArgumentOutOfRangeException(nameof(_scale), "Scale must be greater than zero.");
        }

        _scale = value;
      }
    }
    public int Width {
      get => _width;
      set {
        if (value <= 0) {
          throw new ArgumentOutOfRangeException(nameof(_width), "Width must be greater than zero.");
        }

        _width = value;
      }
    }
    public int Height {
      get => _height;
      set {
        if (value <= 0) {
          throw new ArgumentOutOfRangeException(nameof(_height), "Height must be greater than zero.");
        }

        _height = value;
      }
    }
    /// <summary>
    ///   The width of legend rectangle - not Bitmap!
    /// </summary>
    public int ActualWidth => (int)(Width * Scale);
    /// <summary>
    ///   The height of legend rectangle - not Bitmap!
    /// </summary>
    public int ActualHeight => (int)(Height * Scale);
    private int _width = 120;
    private int _height = 120;
    private double _scale = 1.0;

    public static ContourLegendConfiguration GetDefault() {
      return new ContourLegendConfiguration(120, 110, 1.0d);
    }

    public ContourLegendConfiguration(int width, int height, double scale) {
      Height = height;
      Width = width;
      Scale = scale;
    }

    /// <summary>
    ///   Sets the list of legend values.
    /// </summary>
    public ContourLegendConfiguration SetTextValues(List<string> values) {
      Values = values ?? throw new ArgumentNullException(nameof(values), "Values cannot be null.");
      return this;
    }

    /// <summary>
    ///   Sets the Y positions for the legend values.
    /// </summary>
    public ContourLegendConfiguration SetValuePositionsY(List<int> positions) {
      ValuePositionsY
        = positions ?? throw new ArgumentNullException(nameof(positions), "Value positions cannot be null.");
      return this;
    }

    public bool ToggleLegendVisibility() {
      IsVisible = !IsVisible;
      return IsVisible;
    }

    public bool IsLegendDisplayable() {
      return Values.Any() && ValuePositionsY.Any() && IsVisible;
    }

    /// <summary>
    /// Deserializes the legend's visibility and scale values from an OI archive.
    /// </summary>
    /// <param name="reader">The reader instance for accessing serialized data.</param>
    public void DeserializeLegendState(GH_IReader reader) {
      if (reader.ItemExists(ScaleKey)) {
        Scale = reader.GetDouble(ScaleKey);
      }

      if (reader.ItemExists(VisibilityKey)) {
        IsVisible = reader.GetBoolean(VisibilityKey);
      }

      if (reader.ItemExists(WidthKey)) {
        Width = reader.GetInt32(WidthKey);
      }

      if (reader.ItemExists(HeightKey)) {
        Height = reader.GetInt32(HeightKey);
      }
    }

    /// <summary>
    /// Serializes the legend's visibility and scale values to an OI archive.
    /// </summary>
    /// <param name="writer">The writer instance for saving serialized data.</param>
    public void SerializeLegendState(GH_IWriter writer) {
      writer.SetDouble(ScaleKey, Scale);
      writer.SetBoolean(VisibilityKey, IsVisible);
      writer.SetInt32(WidthKey, Width);
      writer.SetInt32(HeightKey, Height);
    }
  }
}
