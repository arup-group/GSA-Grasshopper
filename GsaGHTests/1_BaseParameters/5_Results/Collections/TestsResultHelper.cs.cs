using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters.Results;
using OasysUnits;
using System;

namespace GsaGHTests.Parameters.Results {
  public static class TestsResultHelper {
    public static double Envelope(double? x, double? y, EnvelopeMethod envelope) {
      return Envelope((double)x, (double)y, envelope);
    }

    public static double Envelope(double x, double y, EnvelopeMethod envelope) {
      double value = 0;
      switch (envelope) {
        case EnvelopeMethod.Maximum:
          value = Math.Max(x, y);
          break;

        case EnvelopeMethod.Minimum:
          value = Math.Min(x, y);
          break;

        case EnvelopeMethod.Absolute:
          value = Math.Max(Math.Abs(x), Math.Abs(y));
          break;

        case EnvelopeMethod.SignedAbsolute:
          if (Math.Abs(x) > Math.Abs(y)) {
            value = x;
          } else {
            value = y;
          }

          break;
      }
      return ResultHelper.RoundToSignificantDigits(value, 4);
    }
    public static double ResultsHelper(INodeResultSubset<IInternalForce,
      ResultVector6<NodeExtremaKey>> result, ResultVector6HelperEnum component, bool max) {
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

    public static double? ResultsHelper(INodeResultSubset<IReactionForce,
      ResultVector6<NodeExtremaKey>> result, ResultVector6HelperEnum component, bool max) {
      double? d = 0;
      ResultVector6<NodeExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultVector6HelperEnum.X:
          d = ((Force)result.GetExtrema(extrema.X).X).Kilonewtons;
          break;

        case ResultVector6HelperEnum.Y:
          d = ((Force)result.GetExtrema(extrema.Y).Y).Kilonewtons;
          break;

        case ResultVector6HelperEnum.Z:
          d = ((Force)result.GetExtrema(extrema.Z).Z).Kilonewtons;
          break;

        case ResultVector6HelperEnum.Xyz:
          d = ((Force)result.GetExtrema(extrema.Xyz).Xyz).Kilonewtons;
          break;

        case ResultVector6HelperEnum.Xx:
          d = ((Moment)result.GetExtrema(extrema.Xx).Xx).KilonewtonMeters;
          break;

        case ResultVector6HelperEnum.Yy:
          d = ((Moment)result.GetExtrema(extrema.Yy).Yy).KilonewtonMeters;
          break;

        case ResultVector6HelperEnum.Zz:
          d = ((Moment)result.GetExtrema(extrema.Zz).Zz).KilonewtonMeters;
          break;

        case ResultVector6HelperEnum.Xxyyzz:
          d = ((Moment)result.GetExtrema(extrema.Xxyyzz).Xxyyzz).KilonewtonMeters;
          break;
      }

      if (d == null) {
        return null;
      }

      return ResultHelper.RoundToSignificantDigits((double)d, 4);
    }

    public static double? ResultsHelper(IReactionForce result, ResultVector6HelperEnum component) {
      double? d = 0;
      switch (component) {
        case ResultVector6HelperEnum.X:
          if (result.X == null) {
            d = null;
          } else {
            d = ((Force)result.X).Kilonewtons;
          }
          break;

        case ResultVector6HelperEnum.Y:
          if (result.Y == null) {
            d = null;
          } else {
            d = ((Force)result.Y).Kilonewtons;
          }
          break;

        case ResultVector6HelperEnum.Z:
          if (result.Z == null) {
            d = null;
          } else {
            d = ((Force)result.Z).Kilonewtons;
          }
          break;

        case ResultVector6HelperEnum.Xyz:
          if (result.Xyz == null) {
            d = null;
          } else {
            d = ((Force)result.Xyz).Kilonewtons;
          }
          break;

        case ResultVector6HelperEnum.Xx:
          if (result.Xx == null) {
            d = null;
          } else {
            d = ((Moment)result.Xx).KilonewtonMeters;
          }
          break;

        case ResultVector6HelperEnum.Yy:
          if (result.Yy == null) {
            d = null;
          } else {
            d = ((Moment)result.Yy).KilonewtonMeters;
          }
          break;

        case ResultVector6HelperEnum.Zz:
          if (result.Zz == null) {
            d = null;
          } else {
            d = ((Moment)result.Zz).KilonewtonMeters;
          }
          break;

        case ResultVector6HelperEnum.Xxyyzz:
          if (result.Xxyyzz == null) {
            d = null;
          } else {
            d = ((Moment)result.Xxyyzz).KilonewtonMeters;
          }
          break;
      }

      if (d == null) {
        return null;
      }

      return ResultHelper.RoundToSignificantDigits((double)d, 4);
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

    public static double ResultsHelper(ITranslation result, ResultVector6HelperEnum component) {
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
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }

    public static double ResultsHelper(IStress result, ResultTensor3HelperEnum component) {
      double d = 0;
      switch (component) {
        case ResultTensor3HelperEnum.Xx:
          d = result.Xx.Megapascals;
          break;

        case ResultTensor3HelperEnum.Yy:
          d = result.Yy.Megapascals;
          break;

        case ResultTensor3HelperEnum.Zz:
          d = result.Zz.Megapascals;
          break;

        case ResultTensor3HelperEnum.Xy:
          d = result.Xy.Megapascals;
          break;

        case ResultTensor3HelperEnum.Yz:
          d = result.Yz.Megapascals;
          break;

        case ResultTensor3HelperEnum.Zx:
          d = result.Zx.Megapascals;
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
      IEntity1dResultSubset<IDisplacement, ResultVector6<Entity1dExtremaKey>> result,
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
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> result,
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
      IEntity1dResultSubset<IStress1dDerived, ResultDerivedStress1d<Entity1dExtremaKey>> result,
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
      IEntity1dResultSubset<IStress1d, ResultStress1d<Entity1dExtremaKey>> result,
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
      IMeshResultSubset<IMeshQuantity<IDisplacement>, IDisplacement, ResultVector6<Entity2dExtremaKey>> result,
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
      IMeshResultSubset<IMeshQuantity<ITranslation>, ITranslation, ResultVector3InAxis<Entity2dExtremaKey>> result,
      ResultVector6HelperEnum component, bool max) {
      double d = 0;
      ResultVector3InAxis<Entity2dExtremaKey> extrema = max ? result.Max : result.Min;
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
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }

    public static double ResultsHelper(
      IMeshResultSubset<IMeshQuantity<IForce2d>, IForce2d, ResultTensor2InAxis<Entity2dExtremaKey>> result,
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

    public static double ResultsHelper(
      IMeshResultSubset<IMeshQuantity<IMoment2d>, IMoment2d, ResultTensor2AroundAxis<Entity2dExtremaKey>> result,
      ResultTensor2AroundAxisHelperEnum component, bool max) {
      double d = 0;
      ResultTensor2AroundAxis<Entity2dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultTensor2AroundAxisHelperEnum.Mx:
          d = result.GetExtrema(extrema.Mx).Mx.Kilonewtons;
          break;

        case ResultTensor2AroundAxisHelperEnum.My:
          d = result.GetExtrema(extrema.My).My.Kilonewtons;
          break;

        case ResultTensor2AroundAxisHelperEnum.Mxy:
          d = result.GetExtrema(extrema.Mxy).Mxy.Kilonewtons;
          break;
        case ResultTensor2AroundAxisHelperEnum.WoodArmerX:
          d = result.GetExtrema(extrema.WoodArmerX).WoodArmerX.Kilonewtons;
          break;
        case ResultTensor2AroundAxisHelperEnum.WoodArmerY:
          d = result.GetExtrema(extrema.WoodArmerY).WoodArmerY.Kilonewtons;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }

    public static double ResultsHelper(
      IMeshResultSubset<IMeshQuantity<IShear2d>, IShear2d, ResultVector2<Entity2dExtremaKey>> result,
      ResultVector2HelperEnum component, bool max) {
      double d = 0;
      ResultVector2<Entity2dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultVector2HelperEnum.Qx:
          d = result.GetExtrema(extrema.Qx).Qx.KilonewtonsPerMeter;
          break;

        case ResultVector2HelperEnum.Qy:
          d = result.GetExtrema(extrema.Qy).Qy.KilonewtonsPerMeter;
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

    public static double ResultsHelper(IMoment2d result, ResultTensor2AroundAxisHelperEnum component) {
      double d = 0;
      switch (component) {
        case ResultTensor2AroundAxisHelperEnum.Mx:
          d = result.Mx.Kilonewtons;
          break;

        case ResultTensor2AroundAxisHelperEnum.My:
          d = result.My.Kilonewtons;
          break;

        case ResultTensor2AroundAxisHelperEnum.Mxy:
          d = result.Mxy.Kilonewtons;
          break;
        case ResultTensor2AroundAxisHelperEnum.WoodArmerX:
          d = result.WoodArmerX.Kilonewtons;
          break;
        case ResultTensor2AroundAxisHelperEnum.WoodArmerY:
          d = result.WoodArmerY.Kilonewtons;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }

    public static double ResultsHelper(IShear2d result, ResultVector2HelperEnum component) {
      double d = 0;
      switch (component) {
        case ResultVector2HelperEnum.Qx:
          d = result.Qx.KilonewtonsPerMeter;
          break;

        case ResultVector2HelperEnum.Qy:
          d = result.Qy.KilonewtonsPerMeter;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }

    public static double ResultsHelper(
      IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> result,
      ResultTensor3HelperEnum component, bool max) {
      double d = 0;
      ResultTensor3<Entity2dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultTensor3HelperEnum.Xx:
          d = result.GetExtrema(extrema.Xx).Xx.Megapascals;
          break;

        case ResultTensor3HelperEnum.Yy:
          d = result.GetExtrema(extrema.Yy).Yy.Megapascals;
          break;

        case ResultTensor3HelperEnum.Zz:
          d = result.GetExtrema(extrema.Zz).Zz.Megapascals;
          break;

        case ResultTensor3HelperEnum.Xy:
          d = result.GetExtrema(extrema.Xy).Xy.Megapascals;
          break;

        case ResultTensor3HelperEnum.Yz:
          d = result.GetExtrema(extrema.Yz).Yz.Megapascals;
          break;

        case ResultTensor3HelperEnum.Zx:
          d = result.GetExtrema(extrema.Zx).Zx.Megapascals;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }
  }
}
