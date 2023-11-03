using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters.Results;

namespace GsaGHTests.Parameters.Results {
  public static class TestsResultHelper {
    public static double ResultsHelper(
      INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> result,
      ResultVector6HelperEnum component, bool max) {
      double d = 0;
      ResultVector6<NodeExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultVector6HelperEnum.X:
          d = result.GetExtrema(extrema.X).X.Kilonewtons;
          break;

        case ResultVector6HelperEnum.Y:
          d = result.GetExtrema(extrema.Y).Y.Kilonewtons;
          break;

        case ResultVector6HelperEnum.Z:
          d = result.GetExtrema(extrema.Z).Z.Kilonewtons;
          break;

        case ResultVector6HelperEnum.Xyz:
          d = result.GetExtrema(extrema.Xyz).Xyz.Kilonewtons;
          break;

        case ResultVector6HelperEnum.Xx:
          d = result.GetExtrema(extrema.Xx).Xx.KilonewtonMeters;
          break;

        case ResultVector6HelperEnum.Yy:
          d = result.GetExtrema(extrema.Yy).Yy.KilonewtonMeters;
          break;

        case ResultVector6HelperEnum.Zz:
          d = result.GetExtrema(extrema.Zz).Zz.KilonewtonMeters;
          break;

        case ResultVector6HelperEnum.Xxyyzz:
          d = result.GetExtrema(extrema.Xxyyzz).Xxyyzz.KilonewtonMeters;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }

    public static double ResultsHelper(IInternalForce result, ResultVector6HelperEnum component) {
      double d = 0;
      switch (component) {
        case ResultVector6HelperEnum.X:
          d = result.X.Kilonewtons;
          break;

        case ResultVector6HelperEnum.Y:
          d = result.Y.Kilonewtons;
          break;

        case ResultVector6HelperEnum.Z:
          d = result.Z.Kilonewtons;
          break;

        case ResultVector6HelperEnum.Xyz:
          d = result.Xyz.Kilonewtons;
          break;

        case ResultVector6HelperEnum.Xx:
          d = result.Xx.KilonewtonMeters;
          break;

        case ResultVector6HelperEnum.Yy:
          d = result.Yy.KilonewtonMeters;
          break;

        case ResultVector6HelperEnum.Zz:
          d = result.Zz.KilonewtonMeters;
          break;

        case ResultVector6HelperEnum.Xxyyzz:
          d = result.Xxyyzz.KilonewtonMeters;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }

    public static double ResultsHelper(
      INodeResultSubset<IDisplacement, ResultVector6<NodeExtremaKey>> result,
      ResultVector6HelperEnum component, bool max) {
      double d = 0;
      ResultVector6<NodeExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultVector6HelperEnum.X:
          d = result.GetExtrema(extrema.X).X.Millimeters;
          break;

        case ResultVector6HelperEnum.Y:
          d = result.GetExtrema(extrema.Y).Y.Millimeters;
          break;

        case ResultVector6HelperEnum.Z:
          d = result.GetExtrema(extrema.Z).Z.Millimeters;
          break;

        case ResultVector6HelperEnum.Xyz:
          d = result.GetExtrema(extrema.Xyz).Xyz.Millimeters;
          break;

        case ResultVector6HelperEnum.Xx:
          d = result.GetExtrema(extrema.Xx).Xx.Radians;
          break;

        case ResultVector6HelperEnum.Yy:
          d = result.GetExtrema(extrema.Yy).Yy.Radians;
          break;

        case ResultVector6HelperEnum.Zz:
          d = result.GetExtrema(extrema.Zz).Zz.Radians;
          break;

        case ResultVector6HelperEnum.Xxyyzz:
          d = result.GetExtrema(extrema.Xxyyzz).Xxyyzz.Radians;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }

    public static double ResultsHelper(IDisplacement result, ResultVector6HelperEnum component) {
      double d = 0;
      switch (component) {
        case ResultVector6HelperEnum.X:
          d = result.X.Millimeters;
          break;

        case ResultVector6HelperEnum.Y:
          d = result.Y.Millimeters;
          break;

        case ResultVector6HelperEnum.Z:
          d = result.Z.Millimeters;
          break;

        case ResultVector6HelperEnum.Xyz:
          d = result.Xyz.Millimeters;
          break;

        case ResultVector6HelperEnum.Xx:
          d = result.Xx.Radians;
          break;

        case ResultVector6HelperEnum.Yy:
          d = result.Yy.Radians;
          break;

        case ResultVector6HelperEnum.Zz:
          d = result.Zz.Radians;
          break;

        case ResultVector6HelperEnum.Xxyyzz:
          d = result.Xxyyzz.Radians;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }

    public static double ResultsHelper(
      IElement1dResultSubset<IElement1dDisplacement, IDisplacement, ResultVector6<Element1dExtremaKey>> result,
      ResultVector6HelperEnum component, bool max) {
      double d = 0;
      ResultVector6<Element1dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultVector6HelperEnum.X:
          d = result.GetExtrema(extrema.X).X.Millimeters;
          break;

        case ResultVector6HelperEnum.Y:
          d = result.GetExtrema(extrema.Y).Y.Millimeters;
          break;

        case ResultVector6HelperEnum.Z:
          d = result.GetExtrema(extrema.Z).Z.Millimeters;
          break;

        case ResultVector6HelperEnum.Xyz:
          d = result.GetExtrema(extrema.Xyz).Xyz.Millimeters;
          break;

        case ResultVector6HelperEnum.Xx:
          d = result.GetExtrema(extrema.Xx).Xx.Radians;
          break;

        case ResultVector6HelperEnum.Yy:
          d = result.GetExtrema(extrema.Yy).Yy.Radians;
          break;

        case ResultVector6HelperEnum.Zz:
          d = result.GetExtrema(extrema.Zz).Zz.Radians;
          break;

        case ResultVector6HelperEnum.Xxyyzz:
          d = result.GetExtrema(extrema.Xxyyzz).Xxyyzz.Radians;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }

    public static double ResultsHelper(
      IElement1dResultSubset<IElement1dInternalForce, IInternalForce, ResultVector6<Element1dExtremaKey>> result,
      ResultVector6HelperEnum component, bool max) {
      double d = 0;
      ResultVector6<Element1dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultVector6HelperEnum.X:
          d = result.GetExtrema(extrema.X).X.Kilonewtons;
          break;

        case ResultVector6HelperEnum.Y:
          d = result.GetExtrema(extrema.Y).Y.Kilonewtons;
          break;

        case ResultVector6HelperEnum.Z:
          d = result.GetExtrema(extrema.Z).Z.Kilonewtons;
          break;

        case ResultVector6HelperEnum.Xyz:
          d = result.GetExtrema(extrema.Xyz).Xyz.Kilonewtons;
          break;

        case ResultVector6HelperEnum.Xx:
          d = result.GetExtrema(extrema.Xx).Xx.KilonewtonMeters;
          break;

        case ResultVector6HelperEnum.Yy:
          d = result.GetExtrema(extrema.Yy).Yy.KilonewtonMeters;
          break;

        case ResultVector6HelperEnum.Zz:
          d = result.GetExtrema(extrema.Zz).Zz.KilonewtonMeters;
          break;

        case ResultVector6HelperEnum.Xxyyzz:
          d = result.GetExtrema(extrema.Xxyyzz).Xxyyzz.KilonewtonMeters;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }
  }
}
