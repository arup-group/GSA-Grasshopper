using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GsaGH.Helpers.GH {
  public class Legend {
    public IReadOnlyCollection<string> Values { get; private set; } = new List<string>();
    public IReadOnlyCollection<int> ValuePositionsY { get; private set; } = new List<int>();
    public bool IsVisible { get; private set; } = true;
    public Bitmap Bitmap { get; private set; }
    public double Scale { get; private set; } = 1.0;

    private const int DefaultWidth = 15;
    private const int DefaultHeight = 120;

    /// <summary>
    ///   Sets the list of legend values.
    /// </summary>
    public void SetValues(List<string> values) {
      Values = values ?? throw new ArgumentNullException(nameof(values), "Values cannot be null.");
    }

    /// <summary>
    ///   Sets the Y positions for the legend values.
    /// </summary>
    public void SetValuePositionsY(List<int> positions) {
      ValuePositionsY
        = positions ?? throw new ArgumentNullException(nameof(positions), "Value positions cannot be null.");
    }

    /// <summary>
    ///   Sets the scale factor for the legend.
    /// </summary>
    public void SetScale(double scale) {
      if (scale <= 0) {
        throw new ArgumentOutOfRangeException(nameof(scale), "Scale must be greater than zero.");
      }

      Scale = scale;
    }

    /// <summary>
    ///   Sets the visibility of the legend.
    ///   Use only for deserialisation!
    /// </summary>
    /// <param name="isVisible">The desired visibility state.</param>
    public void SetVisibility(bool isVisible) {
      IsVisible = isVisible;
    }

    /// <summary>
    ///   Toggles the visibility of the legend and returns the new state.
    /// </summary>
    public bool ToggleVisibility() {
      IsVisible = !IsVisible;
      return IsVisible;
    }

    /// <summary>
    ///   Creates a new Bitmap for the legend with the given width and height.
    /// </summary>
    public void CreateBitmap(int width = DefaultWidth, int height = DefaultHeight) {
      if (width <= 0) {
        throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than zero.");
      }

      if (height <= 0) {
        throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than zero.");
      }

      Bitmap = new Bitmap((int)(width * Scale), (int)(height * Scale));
    }

    /// <summary>
    ///   Determines if the legend is displayable.
    /// </summary>
    public bool IsDisplayable() {
      return Values.Any() && IsVisible;
    }
  }
}
