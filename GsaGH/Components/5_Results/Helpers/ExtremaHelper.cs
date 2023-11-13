using System;
using System.Collections.ObjectModel;
using GsaGH.Parameters.Results;

namespace GsaGH.Components.Helpers {
  internal class ExtremaHelper {
    internal static readonly ReadOnlyCollection<string> Footfall
      = new ReadOnlyCollection<string>(new[] {
        "All",
        "Max Response Factor",
        "Max Peak Velocity",
        "Max RMS Velocity",
        "Max Peak Acceleration",
        "Max RMS Acceleration",
        "Max Critical Frequency",
        "Min Response Factor",
        "Min Peak Velocity",
        "Min RMS Velocity",
        "Min Peak Acceleration",
        "Min RMS Acceleration",
        "Min Critical Frequency",
      });

    internal static readonly ReadOnlyCollection<string> Vector6Displacements
      = new ReadOnlyCollection<string>(new[] {
        "All",
        "Max Ux",
        "Max Uy",
        "Max Uz",
        "Max |U|",
        "Max Rxx",
        "Max Ryy",
        "Max Rzz",
        "Max |R|",
        "Min Ux",
        "Min Uy",
        "Min Uz",
        "Min |U|",
        "Min Rxx",
        "Min Ryy",
        "Min Rzz",
        "Min |R|",
      });

    internal static readonly ReadOnlyCollection<string> Vector6InternalForces
      = new ReadOnlyCollection<string>(new[] {
        "All",
        "Max Fx",
        "Max Fy",
        "Max Fz",
        "Max |Fyz|",
        "Max Mxx",
        "Max Myy",
        "Max Mzz",
        "Max |Myz|",
        "Min Fx",
        "Min Fy",
        "Min Fz",
        "Min |Fyz|",
        "Min Mxx",
        "Min Myy",
        "Min Mzz",
        "Min |Myz|",
      });

    internal static readonly ReadOnlyCollection<string> Vector6ReactionForces
      = new ReadOnlyCollection<string>(new[] {
        "All",
        "Max Fx",
        "Max Fy",
        "Max Fz",
        "Max |F|",
        "Max Mxx",
        "Max Myy",
        "Max Mzz",
        "Max |M|",
        "Min Fx",
        "Min Fy",
        "Min Fz",
        "Min |F|",
        "Min Mxx",
        "Min Myy",
        "Min Mzz",
        "Min |M|",
      });

    internal static U FootfallExtremaKey<U>(
      INodeResultSubset<IFootfall, ResultFootfall<U>> resultSet, string key) {
      return key switch {
        "Max Response Factor" => resultSet.Max.MaximumResponseFactor,
        "Max Peak Velocity" => resultSet.Max.PeakVelocity,
        "Max RMS Velocity" => resultSet.Max.RmsVelocity,
        "Max Peak Acceleration" => resultSet.Max.PeakAcceleration,
        "Max RMS Acceleration" => resultSet.Max.RmsAcceleration,
        "Max Critical Frequency" => resultSet.Max.CriticalFrequency,
        "Min Response Factor" => resultSet.Min.MaximumResponseFactor,
        "Min Peak Velocity" => resultSet.Min.PeakVelocity,
        "Min RMS Velocity" => resultSet.Min.RmsVelocity,
        "Min Peak Acceleration" => resultSet.Min.PeakAcceleration,
        "Min RMS Acceleration" => resultSet.Min.RmsAcceleration,
        "Min Critical Frequency" => resultSet.Min.CriticalFrequency,
        _ => throw new ArgumentException("Extrema case not found"),
      };
    }

    internal static U DisplacementExtremaKey<T, U>(
      INodeResultSubset<T, ResultVector6<U>> resultSet, string key) where T : IResultItem {
      return key switch {
        "Max Ux" => resultSet.Max.X,
        "Max Uy" => resultSet.Max.Y,
        "Max Uz" => resultSet.Max.Z,
        "Max |U|" => resultSet.Max.Xyz,
        "Max Rxx" => resultSet.Max.Xx,
        "Max Ryy" => resultSet.Max.Yy,
        "Max Rzz" => resultSet.Max.Zz,
        "Max |R|" => resultSet.Max.Xxyyzz,
        "Min Ux" => resultSet.Min.X,
        "Min Uy" => resultSet.Min.Y,
        "Min Uz" => resultSet.Min.Z,
        "Min |U|" => resultSet.Min.Xyz,
        "Min Rxx" => resultSet.Min.Xx,
        "Min Ryy" => resultSet.Min.Yy,
        "Min Rzz" => resultSet.Min.Zz,
        "Min |R|" => resultSet.Min.Xxyyzz,
        _ => throw new ArgumentException("Extrema case not found"),
      };
    }

    internal static U DisplacementExtremaKey<T1, T2, U>(
      IElement1dResultSubset<T1, T2, ResultVector6<U>> resultSet, string key)
      where T1 : IElement1dQuantity<T2> where T2 : IResultItem {
      return key switch {
        "Max Ux" => resultSet.Max.X,
        "Max Uy" => resultSet.Max.Y,
        "Max Uz" => resultSet.Max.Z,
        "Max |U|" => resultSet.Max.Xyz,
        "Max Rxx" => resultSet.Max.Xx,
        "Max Ryy" => resultSet.Max.Yy,
        "Max Rzz" => resultSet.Max.Zz,
        "Max |R|" => resultSet.Max.Xxyyzz,
        "Min Ux" => resultSet.Min.X,
        "Min Uy" => resultSet.Min.Y,
        "Min Uz" => resultSet.Min.Z,
        "Min |U|" => resultSet.Min.Xyz,
        "Min Rxx" => resultSet.Min.Xx,
        "Min Ryy" => resultSet.Min.Yy,
        "Min Rzz" => resultSet.Min.Zz,
        "Min |R|" => resultSet.Min.Xxyyzz,
        _ => throw new ArgumentException("Extrema case not found"),
      };
    }

    internal static U InternalForceExtremaKey<T, U>(
      INodeResultSubset<T, ResultVector6<U>> resultSet, string key) where T : IResultItem {
      return key switch {
        "Max Fx" => resultSet.Max.X,
        "Max Fy" => resultSet.Max.Y,
        "Max Fz" => resultSet.Max.Z,
        "Max |Fyz|" => resultSet.Max.Xyz,
        "Max Mxx" => resultSet.Max.Xx,
        "Max Myy" => resultSet.Max.Yy,
        "Max Mzz" => resultSet.Max.Zz,
        "Max |Myz|" => resultSet.Max.Xxyyzz,
        "Min Fx" => resultSet.Min.X,
        "Min Fy" => resultSet.Min.Y,
        "Min Fz" => resultSet.Min.Z,
        "Min |Fyz|" => resultSet.Min.Xyz,
        "Min Mxx" => resultSet.Min.Xx,
        "Min Myy" => resultSet.Min.Yy,
        "Min Mzz" => resultSet.Min.Zz,
        "Min |Myz|" => resultSet.Min.Xxyyzz,
        _ => throw new ArgumentException("Extrema case not found"),
      };
    }

    internal static U ReactionForceExtremaKey<T, U>(
      INodeResultSubset<T, ResultVector6<U>> resultSet, string key) where T : IResultItem {
      return key switch {
        "Max Fx" => resultSet.Max.X,
        "Max Fy" => resultSet.Max.Y,
        "Max Fz" => resultSet.Max.Z,
        "Max |F|" => resultSet.Max.Xyz,
        "Max Mxx" => resultSet.Max.Xx,
        "Max Myy" => resultSet.Max.Yy,
        "Max Mzz" => resultSet.Max.Zz,
        "Max |M|" => resultSet.Max.Xxyyzz,
        "Min Fx" => resultSet.Min.X,
        "Min Fy" => resultSet.Min.Y,
        "Min Fz" => resultSet.Min.Z,
        "Min |F|" => resultSet.Min.Xyz,
        "Min Mxx" => resultSet.Min.Xx,
        "Min Myy" => resultSet.Min.Yy,
        "Min Mzz" => resultSet.Min.Zz,
        "Min |M|" => resultSet.Min.Xxyyzz,
        _ => throw new ArgumentException("Extrema case not found"),
      };
    }

    internal static U InternalForceExtremaKey<T1, T2, U>(
      IElement1dResultSubset<T1, T2, ResultVector6<U>> resultSet, string key)
      where T1 : IElement1dQuantity<T2> where T2 : IResultItem {
      return key switch {
        "Max Fx" => resultSet.Max.X,
        "Max Fy" => resultSet.Max.Y,
        "Max Fz" => resultSet.Max.Z,
        "Max |Fyz|" => resultSet.Max.Xyz,
        "Max Mxx" => resultSet.Max.Xx,
        "Max Myy" => resultSet.Max.Yy,
        "Max Mzz" => resultSet.Max.Zz,
        "Max |Myz|" => resultSet.Max.Xxyyzz,
        "Min Fx" => resultSet.Min.X,
        "Min Fy" => resultSet.Min.Y,
        "Min Fz" => resultSet.Min.Z,
        "Min |Fyz|" => resultSet.Min.Xyz,
        "Min Mxx" => resultSet.Min.Xx,
        "Min Myy" => resultSet.Min.Yy,
        "Min Mzz" => resultSet.Min.Zz,
        "Min |Myz|" => resultSet.Min.Xxyyzz,
        _ => throw new ArgumentException("Extrema case not found"),
      };
    }
  }
}
