using System;
using System.Collections.Generic;

using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters.Results;

using Newtonsoft.Json.Linq;

using OasysUnits;

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
      return value;
    }

    public static double ResultsHelper(IEntity0dResultSubset<IInternalForce,
      ResultVector6<Entity0dExtremaKey>> result, ResultVector6 component, bool max) {
      double d = 0;
      ResultVector6<Entity0dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultVector6.X:
          d = result.GetExtrema(extrema.X).X.Kilonewtons;
          break;

        case ResultVector6.Y:
          d = result.GetExtrema(extrema.Y).Y.Kilonewtons;
          break;

        case ResultVector6.Z:
          d = result.GetExtrema(extrema.Z).Z.Kilonewtons;
          break;

        case ResultVector6.Xyz:
          d = result.GetExtrema(extrema.Xyz).Xyz.Kilonewtons;
          break;

        case ResultVector6.Xx:
          d = result.GetExtrema(extrema.Xx).Xx.KilonewtonMeters;
          break;

        case ResultVector6.Yy:
          d = result.GetExtrema(extrema.Yy).Yy.KilonewtonMeters;
          break;

        case ResultVector6.Zz:
          d = result.GetExtrema(extrema.Zz).Zz.KilonewtonMeters;
          break;

        case ResultVector6.Xxyyzz:
          d = result.GetExtrema(extrema.Xxyyzz).Xxyyzz.KilonewtonMeters;
          break;
      }

      return d;
    }

    public static double ResultsHelper(IInternalForce result, ResultVector6 component) {
      double d = 0;
      switch (component) {
        case ResultVector6.X:
          d = result.X.Kilonewtons;
          break;

        case ResultVector6.Y:
          d = result.Y.Kilonewtons;
          break;

        case ResultVector6.Z:
          d = result.Z.Kilonewtons;
          break;

        case ResultVector6.Xyz:
          d = result.Xyz.Kilonewtons;
          break;

        case ResultVector6.Xx:
          d = result.Xx.KilonewtonMeters;
          break;

        case ResultVector6.Yy:
          d = result.Yy.KilonewtonMeters;
          break;

        case ResultVector6.Zz:
          d = result.Zz.KilonewtonMeters;
          break;

        case ResultVector6.Xxyyzz:
          d = result.Xxyyzz.KilonewtonMeters;
          break;
      }

      return d;
    }

    public static double? ResultsHelper(IEntity0dResultSubset<IReactionForce,
      ResultVector6<Entity0dExtremaKey>> result, ResultVector6 component, bool max) {
      double? d = 0;
      ResultVector6<Entity0dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultVector6.X:
          d = ((Force)result.GetExtrema(extrema.X).X).Kilonewtons;
          break;

        case ResultVector6.Y:
          d = ((Force)result.GetExtrema(extrema.Y).Y).Kilonewtons;
          break;

        case ResultVector6.Z:
          d = ((Force)result.GetExtrema(extrema.Z).Z).Kilonewtons;
          break;

        case ResultVector6.Xyz:
          d = ((Force)result.GetExtrema(extrema.Xyz).Xyz).Kilonewtons;
          break;

        case ResultVector6.Xx:
          d = ((Moment)result.GetExtrema(extrema.Xx).Xx).KilonewtonMeters;
          break;

        case ResultVector6.Yy:
          d = ((Moment)result.GetExtrema(extrema.Yy).Yy).KilonewtonMeters;
          break;

        case ResultVector6.Zz:
          d = ((Moment)result.GetExtrema(extrema.Zz).Zz).KilonewtonMeters;
          break;

        case ResultVector6.Xxyyzz:
          d = ((Moment)result.GetExtrema(extrema.Xxyyzz).Xxyyzz).KilonewtonMeters;
          break;
      }

      if (d == null) {
        return null;
      }

      return d;
    }

    public static double? ResultsHelper(IReactionForce result, ResultVector6 component) {
      double? d = null;
      switch (component) {
        case ResultVector6.X:
          if (result.X.HasValue && !double.IsNaN(result.X.Value.Value)) {
            d = ((Force)result.X).Kilonewtons;
          }
          break;

        case ResultVector6.Y:
          if (result.Y.HasValue && !double.IsNaN(result.Y.Value.Value)) {
            d = ((Force)result.Y).Kilonewtons;
          }
          break;

        case ResultVector6.Z:
          if (result.Z.HasValue && !double.IsNaN(result.Z.Value.Value)) {
            d = ((Force)result.Z).Kilonewtons;
          }
          break;

        case ResultVector6.Xyz:
          if (result.Xyz.HasValue && !double.IsNaN(result.Xyz.Value.Value)) {
            d = ((Force)result.Xyz).Kilonewtons;
          }
          break;

        case ResultVector6.Xx:
          if (result.Xx.HasValue && !double.IsNaN(result.Xx.Value.Value)) {
            d = ((Moment)result.Xx).KilonewtonMeters;
          }
          break;

        case ResultVector6.Yy:
          if (result.Yy.HasValue && !double.IsNaN(result.Yy.Value.Value)) {
            d = ((Moment)result.Yy).KilonewtonMeters;
          }
          break;

        case ResultVector6.Zz:
          if (result.Zz.HasValue && !double.IsNaN(result.Zz.Value.Value)) {
            d = ((Moment)result.Zz).KilonewtonMeters;
          }
          break;

        case ResultVector6.Xxyyzz:
          if (result.Xxyyzz.HasValue && !double.IsNaN(result.Xxyyzz.Value.Value)) {
            d = ((Moment)result.Xxyyzz).KilonewtonMeters;
          }
          break;
      }

      if (d == null) {
        return null;
      }

      return d;
    }

    public static double ResultsHelper(
      IEntity0dResultSubset<IDisplacement, ResultVector6<Entity0dExtremaKey>> result,
      ResultVector6 component, bool max) {
      double d = 0;
      ResultVector6<Entity0dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultVector6.X:
          d = result.GetExtrema(extrema.X).X.Millimeters;
          break;

        case ResultVector6.Y:
          d = result.GetExtrema(extrema.Y).Y.Millimeters;
          break;

        case ResultVector6.Z:
          d = result.GetExtrema(extrema.Z).Z.Millimeters;
          break;

        case ResultVector6.Xyz:
          d = result.GetExtrema(extrema.Xyz).Xyz.Millimeters;
          break;

        case ResultVector6.Xx:
          d = result.GetExtrema(extrema.Xx).Xx.Radians;
          break;

        case ResultVector6.Yy:
          d = result.GetExtrema(extrema.Yy).Yy.Radians;
          break;

        case ResultVector6.Zz:
          d = result.GetExtrema(extrema.Zz).Zz.Radians;
          break;

        case ResultVector6.Xxyyzz:
          d = result.GetExtrema(extrema.Xxyyzz).Xxyyzz.Radians;
          break;
      }

      return d;
    }

    public static double ResultsHelper(IDisplacement result, ResultVector6 component) {
      double d = 0;
      switch (component) {
        case ResultVector6.X:
          d = result.X.Millimeters;
          break;

        case ResultVector6.Y:
          d = result.Y.Millimeters;
          break;

        case ResultVector6.Z:
          d = result.Z.Millimeters;
          break;

        case ResultVector6.Xyz:
          d = result.Xyz.Millimeters;
          break;

        case ResultVector6.Xx:
          d = result.Xx.Radians;
          break;

        case ResultVector6.Yy:
          d = result.Yy.Radians;
          break;

        case ResultVector6.Zz:
          d = result.Zz.Radians;
          break;

        case ResultVector6.Xxyyzz:
          d = result.Xxyyzz.Radians;
          break;
      }

      return d;
    }

    public static double ResultsHelper(Drift result, DriftResultVector component) {
      double d = 0;
      switch (component) {
        case DriftResultVector.X:
          d = result.X.Millimeters;
          break;

        case DriftResultVector.Y:
          d = result.Y.Millimeters;
          break;

        case DriftResultVector.Xy:
          d = result.Xy.Millimeters;
          break;
      }

      return d;
    }

    public static double ResultsHelper(DriftIndex result, DriftResultVector component) {
      double d = 0;
      switch (component) {
        case DriftResultVector.X:
          d = result.X.Value;
          break;

        case DriftResultVector.Y:
          d = result.Y.Value;
          break;

        case DriftResultVector.Xy:
          d = result.Xy.Value;
          break;
      }

      return d;
    }

    public static double ResultsHelper(ITranslation result, ResultVector6 component) {
      double d = 0;
      switch (component) {
        case ResultVector6.X:
          d = result.X.Millimeters;
          break;

        case ResultVector6.Y:
          d = result.Y.Millimeters;
          break;

        case ResultVector6.Z:
          d = result.Z.Millimeters;
          break;

        case ResultVector6.Xyz:
          d = result.Xyz.Millimeters;
          break;
      }

      return d;
    }

    public static double ResultsHelper(IStress result, ResultTensor3 component) {
      double d = 0;
      switch (component) {
        case ResultTensor3.Xx:
          d = result.Xx.Megapascals;
          break;

        case ResultTensor3.Yy:
          d = result.Yy.Megapascals;
          break;

        case ResultTensor3.Zz:
          d = result.Zz.Megapascals;
          break;

        case ResultTensor3.Xy:
          d = result.Xy.Megapascals;
          break;

        case ResultTensor3.Yz:
          d = result.Yz.Megapascals;
          break;

        case ResultTensor3.Zx:
          d = result.Zx.Megapascals;
          break;
      }

      return d;
    }

    public static double ResultsHelper(IStress1dDerived result, ResultDerivedStress1d component) {
      double d = 0;
      switch (component) {
        case ResultDerivedStress1d.ShearY:
          d = result.ElasticShearY.Megapascals;
          break;

        case ResultDerivedStress1d.ShearZ:
          d = result.ElasticShearZ.Megapascals;
          break;

        case ResultDerivedStress1d.Torsion:
          d = result.Torsional.Megapascals;
          break;

        case ResultDerivedStress1d.VonMises:
          d = result.VonMises.Megapascals;
          break;
      }

      return d;
    }

    public static double ResultsHelper(IStress1d result, ResultStress1d component) {
      double d = 0;
      switch (component) {
        case ResultStress1d.Axial:
          d = result.Axial.Megapascals;
          break;

        case ResultStress1d.ShearY:
          d = result.ShearY.Megapascals;
          break;

        case ResultStress1d.ShearZ:
          d = result.ShearZ.Megapascals;
          break;

        case ResultStress1d.ByPos:
          d = result.BendingYyPositiveZ.Megapascals;
          break;

        case ResultStress1d.ByNeg:
          d = result.BendingYyNegativeZ.Megapascals;
          break;

        case ResultStress1d.BzPos:
          d = result.BendingZzPositiveY.Megapascals;
          break;

        case ResultStress1d.BzNeg:
          d = result.BendingZzNegativeY.Megapascals;
          break;

        case ResultStress1d.C1:
          d = result.CombinedC1.Megapascals;
          break;

        case ResultStress1d.C2:
          d = result.CombinedC2.Megapascals;
          break;
      }

      return d;
    }

    public static double ResultsHelper(
      IEntity1dResultSubset<IDisplacement, ResultVector6<Entity1dExtremaKey>> result,
      ResultVector6 component, bool max) {
      double d = 0;
      ResultVector6<Entity1dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultVector6.X:
          d = result.GetExtrema(extrema.X).X.Millimeters;
          break;

        case ResultVector6.Y:
          d = result.GetExtrema(extrema.Y).Y.Millimeters;
          break;

        case ResultVector6.Z:
          d = result.GetExtrema(extrema.Z).Z.Millimeters;
          break;

        case ResultVector6.Xyz:
          d = result.GetExtrema(extrema.Xyz).Xyz.Millimeters;
          break;

        case ResultVector6.Xx:
          d = result.GetExtrema(extrema.Xx).Xx.Radians;
          break;

        case ResultVector6.Yy:
          d = result.GetExtrema(extrema.Yy).Yy.Radians;
          break;

        case ResultVector6.Zz:
          d = result.GetExtrema(extrema.Zz).Zz.Radians;
          break;

        case ResultVector6.Xxyyzz:
          d = result.GetExtrema(extrema.Xxyyzz).Xxyyzz.Radians;
          break;
      }

      return d;
    }

    public static double ResultsHelper(
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> result,
      ResultVector6 component, bool max) {
      double d = 0;
      ResultVector6<Entity1dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultVector6.X:
          d = result.GetExtrema(extrema.X).X.Kilonewtons;
          break;

        case ResultVector6.Y:
          d = result.GetExtrema(extrema.Y).Y.Kilonewtons;
          break;

        case ResultVector6.Z:
          d = result.GetExtrema(extrema.Z).Z.Kilonewtons;
          break;

        case ResultVector6.Xyz:
          d = result.GetExtrema(extrema.Xyz).Xyz.Kilonewtons;
          break;

        case ResultVector6.Xx:
          d = result.GetExtrema(extrema.Xx).Xx.KilonewtonMeters;
          break;

        case ResultVector6.Yy:
          d = result.GetExtrema(extrema.Yy).Yy.KilonewtonMeters;
          break;

        case ResultVector6.Zz:
          d = result.GetExtrema(extrema.Zz).Zz.KilonewtonMeters;
          break;

        case ResultVector6.Xxyyzz:
          d = result.GetExtrema(extrema.Xxyyzz).Xxyyzz.KilonewtonMeters;
          break;
      }

      return d;
    }

    public static double ResultsHelper(
      IEntity1dResultSubset<IStress1dDerived, ResultDerivedStress1d<Entity1dExtremaKey>> result,
      ResultDerivedStress1d component, bool max) {
      double d = 0;
      ResultDerivedStress1d<Entity1dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultDerivedStress1d.ShearY:
          d = result.GetExtrema(extrema.ElasticShearY).ElasticShearY.Megapascals;
          break;

        case ResultDerivedStress1d.ShearZ:
          d = result.GetExtrema(extrema.ElasticShearZ).ElasticShearZ.Megapascals;
          break;

        case ResultDerivedStress1d.Torsion:
          d = result.GetExtrema(extrema.Torsional).Torsional.Megapascals;
          break;

        case ResultDerivedStress1d.VonMises:
          d = result.GetExtrema(extrema.VonMises).VonMises.Megapascals;
          break;
      }

      return d;
    }

    public static double ResultsHelper(
      IEntity1dResultSubset<IStress1d, ResultStress1d<Entity1dExtremaKey>> result,
      ResultStress1d component, bool max) {
      double d = 0;
      ResultStress1d<Entity1dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultStress1d.Axial:
          d = result.GetExtrema(extrema.Axial).Axial.Megapascals;
          break;

        case ResultStress1d.ShearY:
          d = result.GetExtrema(extrema.ShearY).ShearY.Megapascals;
          break;

        case ResultStress1d.ShearZ:
          d = result.GetExtrema(extrema.ShearZ).ShearZ.Megapascals;
          break;

        case ResultStress1d.ByPos:
          d = result.GetExtrema(extrema.BendingYyPositiveZ).BendingYyPositiveZ.Megapascals;
          break;

        case ResultStress1d.ByNeg:
          d = result.GetExtrema(extrema.BendingYyNegativeZ).BendingYyNegativeZ.Megapascals;
          break;

        case ResultStress1d.BzPos:
          d = result.GetExtrema(extrema.BendingZzPositiveY).BendingZzPositiveY.Megapascals;
          break;

        case ResultStress1d.BzNeg:
          d = result.GetExtrema(extrema.BendingZzNegativeY).BendingZzNegativeY.Megapascals;
          break;

        case ResultStress1d.C1:
          d = result.GetExtrema(extrema.CombinedC1).CombinedC1.Megapascals;
          break;

        case ResultStress1d.C2:
          d = result.GetExtrema(extrema.CombinedC2).CombinedC2.Megapascals;
          break;
      }

      return d;
    }

    public static double ResultsHelper(
      IMeshResultSubset<IMeshQuantity<IDisplacement>, IDisplacement, ResultVector6<Entity2dExtremaKey>> result,
      ResultVector6 component, bool max) {
      double d = 0;
      ResultVector6<Entity2dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultVector6.X:
          d = result.GetExtrema(extrema.X).X.Millimeters;
          break;

        case ResultVector6.Y:
          d = result.GetExtrema(extrema.Y).Y.Millimeters;
          break;

        case ResultVector6.Z:
          d = result.GetExtrema(extrema.Z).Z.Millimeters;
          break;

        case ResultVector6.Xyz:
          d = result.GetExtrema(extrema.Xyz).Xyz.Millimeters;
          break;

        case ResultVector6.Xx:
          d = result.GetExtrema(extrema.Xx).Xx.Radians;
          break;

        case ResultVector6.Yy:
          d = result.GetExtrema(extrema.Yy).Yy.Radians;
          break;

        case ResultVector6.Zz:
          d = result.GetExtrema(extrema.Zz).Zz.Radians;
          break;

        case ResultVector6.Xxyyzz:
          d = result.GetExtrema(extrema.Xxyyzz).Xxyyzz.Radians;
          break;
      }

      return d;
    }

    public static double ResultsHelper(
      IMeshResultSubset<IMeshQuantity<ITranslation>, ITranslation, ResultVector3InAxis<Entity2dExtremaKey>> result,
      ResultVector6 component, bool max) {
      double d = 0;
      ResultVector3InAxis<Entity2dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultVector6.X:
          d = result.GetExtrema(extrema.X).X.Millimeters;
          break;

        case ResultVector6.Y:
          d = result.GetExtrema(extrema.Y).Y.Millimeters;
          break;

        case ResultVector6.Z:
          d = result.GetExtrema(extrema.Z).Z.Millimeters;
          break;

        case ResultVector6.Xyz:
          d = result.GetExtrema(extrema.Xyz).Xyz.Millimeters;
          break;
      }

      return d;
    }

    public static double ResultsHelper(
      IMeshResultSubset<IMeshQuantity<IForce2d>, IForce2d, ResultTensor2InAxis<Entity2dExtremaKey>> result,
      ResultTensor2InAxis component, bool max) {
      double d = 0;
      ResultTensor2InAxis<Entity2dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultTensor2InAxis.Nx:
          d = result.GetExtrema(extrema.Nx).Nx.KilonewtonsPerMeter;
          break;

        case ResultTensor2InAxis.Ny:
          d = result.GetExtrema(extrema.Ny).Ny.KilonewtonsPerMeter;
          break;

        case ResultTensor2InAxis.Nxy:
          d = result.GetExtrema(extrema.Nxy).Nxy.KilonewtonsPerMeter;
          break;
      }

      return d;
    }

    public static double ResultsHelper(
      IMeshResultSubset<IMeshQuantity<IMoment2d>, IMoment2d, ResultTensor2AroundAxis<Entity2dExtremaKey>> result,
      ResultTensor2AroundAxis component, bool max) {
      double d = 0;
      ResultTensor2AroundAxis<Entity2dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultTensor2AroundAxis.Mx:
          d = result.GetExtrema(extrema.Mx).Mx.Kilonewtons;
          break;

        case ResultTensor2AroundAxis.My:
          d = result.GetExtrema(extrema.My).My.Kilonewtons;
          break;

        case ResultTensor2AroundAxis.Mxy:
          d = result.GetExtrema(extrema.Mxy).Mxy.Kilonewtons;
          break;
        case ResultTensor2AroundAxis.WoodArmerX:
          d = result.GetExtrema(extrema.WoodArmerX).WoodArmerX.Kilonewtons;
          break;
        case ResultTensor2AroundAxis.WoodArmerY:
          d = result.GetExtrema(extrema.WoodArmerY).WoodArmerY.Kilonewtons;
          break;
      }

      return d;
    }

    public static double ResultsHelper(
      IMeshResultSubset<IMeshQuantity<IShear2d>, IShear2d, ResultVector2<Entity2dExtremaKey>> result,
      ResultVector2 component, bool max) {
      double d = 0;
      ResultVector2<Entity2dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultVector2.Qx:
          d = result.GetExtrema(extrema.Qx).Qx.KilonewtonsPerMeter;
          break;

        case ResultVector2.Qy:
          d = result.GetExtrema(extrema.Qy).Qy.KilonewtonsPerMeter;
          break;
      }

      return d;
    }

    public static double ResultsHelper(IForce2d result, ResultTensor2InAxis component) {
      double d = 0;
      switch (component) {
        case ResultTensor2InAxis.Nx:
          d = result.Nx.KilonewtonsPerMeter;
          break;

        case ResultTensor2InAxis.Ny:
          d = result.Ny.KilonewtonsPerMeter;
          break;

        case ResultTensor2InAxis.Nxy:
          d = result.Nxy.KilonewtonsPerMeter;
          break;
      }

      return d;
    }

    public static double ResultsHelper(IMoment2d result, ResultTensor2AroundAxis component) {
      double d = 0;
      switch (component) {
        case ResultTensor2AroundAxis.Mx:
          d = result.Mx.Kilonewtons;
          break;

        case ResultTensor2AroundAxis.My:
          d = result.My.Kilonewtons;
          break;

        case ResultTensor2AroundAxis.Mxy:
          d = result.Mxy.Kilonewtons;
          break;
        case ResultTensor2AroundAxis.WoodArmerX:
          d = result.WoodArmerX.Kilonewtons;
          break;
        case ResultTensor2AroundAxis.WoodArmerY:
          d = result.WoodArmerY.Kilonewtons;
          break;
      }

      return d;
    }

    public static double ResultsHelper(IShear2d result, ResultVector2 component) {
      double d = 0;
      switch (component) {
        case ResultVector2.Qx:
          d = result.Qx.KilonewtonsPerMeter;
          break;

        case ResultVector2.Qy:
          d = result.Qy.KilonewtonsPerMeter;
          break;
      }

      return d;
    }

    public static double ResultsHelper(
      IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> result,
      ResultTensor3 component, bool max) {
      double d = 0;
      ResultTensor3<Entity2dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultTensor3.Xx:
          d = result.GetExtrema(extrema.Xx).Xx.Megapascals;
          break;

        case ResultTensor3.Yy:
          d = result.GetExtrema(extrema.Yy).Yy.Megapascals;
          break;

        case ResultTensor3.Zz:
          d = result.GetExtrema(extrema.Zz).Zz.Megapascals;
          break;

        case ResultTensor3.Xy:
          d = result.GetExtrema(extrema.Xy).Xy.Megapascals;
          break;

        case ResultTensor3.Yz:
          d = result.GetExtrema(extrema.Yz).Yz.Megapascals;
          break;

        case ResultTensor3.Zx:
          d = result.GetExtrema(extrema.Zx).Zx.Megapascals;
          break;
      }

      return d;
    }

    public static double ResultsHelper(AssemblyDisplacements result, ResultVector6 component, bool max) {
      double d = 0;
      ResultVector6<Entity1dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultVector6.X:
          d = result.GetExtrema(extrema.X).X.Millimeters;
          break;

        case ResultVector6.Y:
          d = result.GetExtrema(extrema.Y).Y.Millimeters;
          break;

        case ResultVector6.Z:
          d = result.GetExtrema(extrema.Z).Z.Millimeters;
          break;

        case ResultVector6.Xyz:
          d = result.GetExtrema(extrema.Xyz).Xyz.Millimeters;
          break;

        case ResultVector6.Xx:
          d = result.GetExtrema(extrema.Xx).Xx.Radians;
          break;

        case ResultVector6.Yy:
          d = result.GetExtrema(extrema.Yy).Yy.Radians;
          break;

        case ResultVector6.Zz:
          d = result.GetExtrema(extrema.Zz).Zz.Radians;
          break;

        case ResultVector6.Xxyyzz:
          d = result.GetExtrema(extrema.Xxyyzz).Xxyyzz.Radians;
          break;
      }

      return d;
    }

    public static double ResultsHelper(AssemblyDrifts result, DriftResultVector component, bool max) {
      double d = 0;
      DriftResultVector<Entity1dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case DriftResultVector.X:
          d = result.GetExtrema(extrema.X).X.Millimeters;
          break;

        case DriftResultVector.Y:
          d = result.GetExtrema(extrema.Y).Y.Millimeters;
          break;

        case DriftResultVector.Xy:
          d = result.GetExtrema(extrema.Xy).Xy.Millimeters;
          break;
      }

      return d;
    }

    public static double ResultsHelper(AssemblyDriftIndices result, DriftResultVector component, bool max) {
      double d = 0;
      DriftResultVector<Entity1dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case DriftResultVector.X:
          d = result.GetExtrema(extrema.X).X.DecimalFractions;
          break;

        case DriftResultVector.Y:
          d = result.GetExtrema(extrema.Y).Y.DecimalFractions;
          break;

        case DriftResultVector.Xy:
          d = result.GetExtrema(extrema.Xy).Xy.DecimalFractions;
          break;
      }

      return d;
    }

    public static double ResultsHelper(AssemblyForcesAndMoments result, ResultVector6 component, bool max) {
      double d = 0;
      ResultVector6<Entity1dExtremaKey> extrema = max ? result.Max : result.Min;
      switch (component) {
        case ResultVector6.X:
          d = result.GetExtrema(extrema.X).X.Kilonewtons;
          break;

        case ResultVector6.Y:
          d = result.GetExtrema(extrema.Y).Y.Kilonewtons;
          break;

        case ResultVector6.Z:
          d = result.GetExtrema(extrema.Z).Z.Kilonewtons;
          break;

        case ResultVector6.Xyz:
          d = result.GetExtrema(extrema.Xyz).Xyz.Kilonewtons;
          break;

        case ResultVector6.Xx:
          d = result.GetExtrema(extrema.Xx).Xx.KilonewtonMeters;
          break;

        case ResultVector6.Yy:
          d = result.GetExtrema(extrema.Yy).Yy.KilonewtonMeters;
          break;

        case ResultVector6.Zz:
          d = result.GetExtrema(extrema.Zz).Zz.KilonewtonMeters;
          break;

        case ResultVector6.Xxyyzz:
          d = result.GetExtrema(extrema.Xxyyzz).Xxyyzz.KilonewtonMeters;
          break;
      }

      return d;
    }

    internal static List<double> ResultsHelper(IDictionary<int, IReactionForce> result, ResultVector6 component) {
      var d = new List<double>();
      switch (component) {
        case ResultVector6.X:
          foreach (IReactionForce force in result.Values) {
            d.Add(((Force)force.X).Kilonewtons);
          }
          break;

        case ResultVector6.Y:
          foreach (IReactionForce force in result.Values) {
            d.Add(((Force)force.Y).Kilonewtons);
          }
          break;

        case ResultVector6.Z:
          foreach (IReactionForce force in result.Values) {
            d.Add(((Force)force.Z).Kilonewtons);
          }
          break;

        case ResultVector6.Xyz:
          foreach (IReactionForce force in result.Values) {
            d.Add(((Force)force.Xyz).Kilonewtons);
          }
          break;

        case ResultVector6.Xx:
          foreach (IReactionForce force in result.Values) {
            d.Add(((Moment)force.Xx).KilonewtonMeters);
          }
          break;

        case ResultVector6.Yy:
          foreach (IReactionForce force in result.Values) {
            d.Add(((Moment)force.Yy).KilonewtonMeters);
          }
          break;

        case ResultVector6.Zz:
          foreach (IReactionForce force in result.Values) {
            d.Add(((Moment)force.Zz).KilonewtonMeters);
          }
          break;

        case ResultVector6.Xxyyzz:
          foreach (IReactionForce force in result.Values) {
            d.Add(((Moment)force.Xxyyzz).KilonewtonMeters);
          }
          break;
      }

      return d;
    }
  }
}
