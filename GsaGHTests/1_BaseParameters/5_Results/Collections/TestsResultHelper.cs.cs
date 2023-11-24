using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters.Results;
using static GsaGHTests.Parameters.Results.Element1dDerivedStressTests;
using static GsaGHTests.Parameters.Results.Element1dStressTests;

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

    public static double ResultsHelper(IStress1dDerived result, ResultDerivedStress1dHelperEnum component) {
      double d = 0;
      switch (component) {
        case ResultDerivedStress1dHelperEnum.ShearY:
          d = result.ElasticShearY.Megapascals;
          break;

        case ResultDerivedStress1dHelperEnum.ShearZ:
          d = result.ElasticShearZ.Megapascals;
          break;

        case ResultDerivedStress1dHelperEnum.Torsion:
          d = result.Torsional.Megapascals;
          break;

        case ResultDerivedStress1dHelperEnum.VonMises:
          d = result.VonMises.Megapascals;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }

    public static double ResultsHelper(IStress1d result, ResultStress1dHelperEnum component) {
      double d = 0;
      switch (component) {
        case ResultStress1dHelperEnum.Axial:
          d = result.Axial.Megapascals;
          break;

        case ResultStress1dHelperEnum.ShearY:
          d = result.ShearY.Megapascals;
          break;

        case ResultStress1dHelperEnum.ShearZ:
          d = result.ShearZ.Megapascals;
          break;

        case ResultStress1dHelperEnum.ByPos:
          d = result.BendingYyPositiveZ.Megapascals;
          break;

        case ResultStress1dHelperEnum.ByNeg:
          d = result.BendingYyNegativeZ.Megapascals;
          break;

        case ResultStress1dHelperEnum.BzPos:
          d = result.BendingZzPositiveY.Megapascals;
          break;

        case ResultStress1dHelperEnum.BzNeg:
          d = result.BendingZzNegativeY.Megapascals;
          break;

        case ResultStress1dHelperEnum.C1:
          d = result.CombinedC1.Megapascals;
          break;

        case ResultStress1dHelperEnum.C2:
          d = result.CombinedC2.Megapascals;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }

    public static double ResultsHelper(
      IEntity1dResultSubset<IEntity1dDisplacement, IDisplacement, ResultVector6<Entity1dExtremaKey>> result,
      ResultVector6HelperEnum component, bool max) {
      double d = 0;
      ResultVector6<Entity1dExtremaKey> extrema = max ? result.Max : result.Min;
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
      IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>> result,
      ResultVector6HelperEnum component, bool max) {
      double d = 0;
      ResultVector6<Entity1dExtremaKey> extrema = max ? result.Max : result.Min;
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

    public static double ResultsHelper(
      IEntity1dResultSubset<IEntity1dDerivedStress, IStress1dDerived, ResultDerivedStress1d<Entity1dExtremaKey>> result,
      ResultDerivedStress1dHelperEnum component, bool max) {
      double d = 0;
      ResultDerivedStress1d<Entity1dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultDerivedStress1dHelperEnum.ShearY:
          d = result.GetExtrema(extrema.ElasticShearY).ElasticShearY.Megapascals;
          break;

        case ResultDerivedStress1dHelperEnum.ShearZ:
          d = result.GetExtrema(extrema.ElasticShearZ).ElasticShearZ.Megapascals;
          break;

        case ResultDerivedStress1dHelperEnum.Torsion:
          d = result.GetExtrema(extrema.Torsional).Torsional.Megapascals;
          break;

        case ResultDerivedStress1dHelperEnum.VonMises:
          d = result.GetExtrema(extrema.VonMises).VonMises.Megapascals;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }

    public static double ResultsHelper(
      IEntity1dResultSubset<IEntity1dStress, IStress1d, ResultStress1d<Entity1dExtremaKey>> result,
      ResultStress1dHelperEnum component, bool max) {
      double d = 0;
      ResultStress1d<Entity1dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultStress1dHelperEnum.Axial:
          d = result.GetExtrema(extrema.Axial).Axial.Megapascals;
          break;

        case ResultStress1dHelperEnum.ShearY:
          d = result.GetExtrema(extrema.ShearY).ShearY.Megapascals;
          break;

        case ResultStress1dHelperEnum.ShearZ:
          d = result.GetExtrema(extrema.ShearZ).ShearZ.Megapascals;
          break;

        case ResultStress1dHelperEnum.ByPos:
          d = result.GetExtrema(extrema.BendingYyPositiveZ).BendingYyPositiveZ.Megapascals;
          break;

        case ResultStress1dHelperEnum.ByNeg:
          d = result.GetExtrema(extrema.BendingYyNegativeZ).BendingYyNegativeZ.Megapascals;
          break;

        case ResultStress1dHelperEnum.BzPos:
          d = result.GetExtrema(extrema.BendingZzPositiveY).BendingZzPositiveY.Megapascals;
          break;

        case ResultStress1dHelperEnum.BzNeg:
          d = result.GetExtrema(extrema.BendingZzNegativeY).BendingZzNegativeY.Megapascals;
          break;

        case ResultStress1dHelperEnum.C1:
          d = result.GetExtrema(extrema.CombinedC1).CombinedC1.Megapascals;
          break;

        case ResultStress1dHelperEnum.C2:
          d = result.GetExtrema(extrema.CombinedC2).CombinedC2.Megapascals;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }

    public static double ResultsHelper(
      IEntity2dResultSubset<IEntity2dQuantity<IDisplacement>, IDisplacement, ResultVector6<Entity2dExtremaKey>> result,
      ResultVector6HelperEnum component, bool max) {
      double d = 0;
      ResultVector6<Entity2dExtremaKey> extrema = max ? result.Max : result.Min;
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
      IEntity2dResultSubset<IEntity2dQuantity<IForce2d>, IForce2d, ResultTensor2InAxis<Entity2dExtremaKey>> result,
      ResultTensor2InAxisHelperEnum component, bool max) {
      double d = 0;
      ResultTensor2InAxis<Entity2dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultTensor2InAxisHelperEnum.Nx:
          d = result.GetExtrema(extrema.Nx).Nx.KilonewtonsPerMeter;
          break;

        case ResultTensor2InAxisHelperEnum.Ny:
          d = result.GetExtrema(extrema.Ny).Ny.KilonewtonsPerMeter;
          break;

        case ResultTensor2InAxisHelperEnum.Nxy:
          d = result.GetExtrema(extrema.Nxy).Nxy.KilonewtonsPerMeter;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }

    public static double ResultsHelper(IForce2d result, ResultTensor2InAxisHelperEnum component) {
      double d = 0;
      switch (component) {
        case ResultTensor2InAxisHelperEnum.Nx:
          d = result.Nx.KilonewtonsPerMeter;
          break;

        case ResultTensor2InAxisHelperEnum.Ny:
          d = result.Ny.KilonewtonsPerMeter;
          break;

        case ResultTensor2InAxisHelperEnum.Nxy:
          d = result.Nxy.KilonewtonsPerMeter;
          break;

      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }
  }
}
