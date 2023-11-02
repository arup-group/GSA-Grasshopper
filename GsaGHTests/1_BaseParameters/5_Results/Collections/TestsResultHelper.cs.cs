using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters.Results;

namespace GsaGHTests.Parameters.Results {
  public static class TestsResultHelper {
    public static double ResultsHelper(
      INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> result,
      NodeComponentHelperEnum component, bool max) {
      double d = 0;
      ResultVector6<NodeExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case NodeComponentHelperEnum.X:
          d = result.GetExtrema(extrema.X).X.Kilonewtons;
          break;

        case NodeComponentHelperEnum.Y:
          d = result.GetExtrema(extrema.Y).Y.Kilonewtons;
          break;

        case NodeComponentHelperEnum.Z:
          d = result.GetExtrema(extrema.Z).Z.Kilonewtons;
          break;

        case NodeComponentHelperEnum.Xyz:
          d = result.GetExtrema(extrema.Xyz).Xyz.Kilonewtons;
          break;

        case NodeComponentHelperEnum.Xx:
          d = result.GetExtrema(extrema.Xx).Xx.KilonewtonMeters;
          break;

        case NodeComponentHelperEnum.Yy:
          d = result.GetExtrema(extrema.Yy).Yy.KilonewtonMeters;
          break;

        case NodeComponentHelperEnum.Zz:
          d = result.GetExtrema(extrema.Zz).Zz.KilonewtonMeters;
          break;

        case NodeComponentHelperEnum.Xxyyzz:
          d = result.GetExtrema(extrema.Xxyyzz).Xxyyzz.KilonewtonMeters;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }

    public static double ResultsHelper(IInternalForce result, NodeComponentHelperEnum component) {
      double d = 0;
      switch (component) {
        case NodeComponentHelperEnum.X:
          d = result.X.Kilonewtons;
          break;

        case NodeComponentHelperEnum.Y:
          d = result.Y.Kilonewtons;
          break;

        case NodeComponentHelperEnum.Z:
          d = result.Z.Kilonewtons;
          break;

        case NodeComponentHelperEnum.Xyz:
          d = result.Xyz.Kilonewtons;
          break;

        case NodeComponentHelperEnum.Xx:
          d = result.Xx.KilonewtonMeters;
          break;

        case NodeComponentHelperEnum.Yy:
          d = result.Yy.KilonewtonMeters;
          break;

        case NodeComponentHelperEnum.Zz:
          d = result.Zz.KilonewtonMeters;
          break;

        case NodeComponentHelperEnum.Xxyyzz:
          d = result.Xxyyzz.KilonewtonMeters;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }

    public static double ResultsHelper(
      INodeResultSubset<IDisplacement, ResultVector6<NodeExtremaKey>> result,
      NodeComponentHelperEnum component, bool max) {
      double d = 0;
      ResultVector6<NodeExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case NodeComponentHelperEnum.X:
          d = result.GetExtrema(extrema.X).X.Millimeters;
          break;

        case NodeComponentHelperEnum.Y:
          d = result.GetExtrema(extrema.Y).Y.Millimeters;
          break;

        case NodeComponentHelperEnum.Z:
          d = result.GetExtrema(extrema.Z).Z.Millimeters;
          break;

        case NodeComponentHelperEnum.Xyz:
          d = result.GetExtrema(extrema.Xyz).Xyz.Millimeters;
          break;

        case NodeComponentHelperEnum.Xx:
          d = result.GetExtrema(extrema.Xx).Xx.Radians;
          break;

        case NodeComponentHelperEnum.Yy:
          d = result.GetExtrema(extrema.Yy).Yy.Radians;
          break;

        case NodeComponentHelperEnum.Zz:
          d = result.GetExtrema(extrema.Zz).Zz.Radians;
          break;

        case NodeComponentHelperEnum.Xxyyzz:
          d = result.GetExtrema(extrema.Xxyyzz).Xxyyzz.Radians;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }

    public static double ResultsHelper(IDisplacement result, NodeComponentHelperEnum component) {
      double d = 0;
      switch (component) {
        case NodeComponentHelperEnum.X:
          d = result.X.Millimeters;
          break;

        case NodeComponentHelperEnum.Y:
          d = result.Y.Millimeters;
          break;

        case NodeComponentHelperEnum.Z:
          d = result.Z.Millimeters;
          break;

        case NodeComponentHelperEnum.Xyz:
          d = result.Xyz.Millimeters;
          break;

        case NodeComponentHelperEnum.Xx:
          d = result.Xx.Radians;
          break;

        case NodeComponentHelperEnum.Yy:
          d = result.Yy.Radians;
          break;

        case NodeComponentHelperEnum.Zz:
          d = result.Zz.Radians;
          break;

        case NodeComponentHelperEnum.Xxyyzz:
          d = result.Xxyyzz.Radians;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }
  }
}
