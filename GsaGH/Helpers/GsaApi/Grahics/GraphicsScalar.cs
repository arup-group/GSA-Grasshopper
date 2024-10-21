using GsaGH.Parameters;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Helpers.GsaApi.Grahics {
  internal static class GraphicsScalar {
    /// <summary>
    /// For 3D visualisation stuff without results
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    internal static double ComputeScale(GsaModel model) {
      return ComputeScale(model, 1.0, false, 1.0);
    }

    /// <summary>
    /// For diagrams of results or loads
    /// </summary>
    /// <param name="model"></param>
    /// <param name="userScaleFactor"></param>
    /// <param name="autoScale"></param>
    /// <param name="unitScaleFactor"></param>
    /// <returns></returns>
    internal static double ComputeScale(GsaModel model, double userScaleFactor,
     bool autoScale, double unitScaleFactor) {
      double lengthScaleFactor = 1;
      if (!autoScale) {
        LengthUnit lengthUnit = model.ModelUnit;
        lengthScaleFactor = UnitConverter.Convert(1, lengthUnit, Length.BaseUnit);
      }

      // maxLength = 2.5% of bbox diagonal
      double modelScale = autoScale ? model.BoundingBox.Diagonal.Length * 0.025 : 1;
      return userScaleFactor * unitScaleFactor * lengthScaleFactor * modelScale;
    }
  }
}
