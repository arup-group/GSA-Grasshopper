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

    internal static readonly ReadOnlyCollection<string> Stress1d
      = new ReadOnlyCollection<string>(new[] {
        "All",
        "Max Axial",
        "Max Shear Y",
        "Max Shear Z",
        "Max Bending Y+",
        "Max Bending Y-",
        "Max Bending Z+",
        "Max Bending Z-",
        "Max Combined C1",
        "Max Combined C2",
        "Min Axial",
        "Min Shear Y",
        "Min Shear Z",
        "Min Bending Y+",
        "Min Bending Y-",
        "Min Bending Z+",
        "Min Bending Z-",
        "Min Combined C1",
        "Min Combined C2",
      });

    internal static readonly ReadOnlyCollection<string> Stress1dDerived
      = new ReadOnlyCollection<string>(new[] {
        "All",
        "Max Shear Y",
        "Max Shear Z",
        "Max Torsional",
        "Max von Mises",
        "Min Shear Y",
        "Min Shear Z",
        "Min Torsional",
        "Min von Mises",
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

    internal static readonly ReadOnlyCollection<string> Tensor2InAxis
      = new ReadOnlyCollection<string>(new[] {
        "All",
        "Max Nx",
        "Max Ny",
        "Max Nxy",
        "Min Nx",
        "Min Ny",
        "Min Nxy",
      });

    internal static readonly ReadOnlyCollection<string> Tensor3Stresses
      = new ReadOnlyCollection<string>(new[] {
        "All",
        "Max xx",
        "Max yy",
        "Max zz",
        "Max xy",
        "Max yz",
        "Max zx",
        "Min xx",
        "Min yy",
        "Min zz",
        "Min xy",
        "Min yz",
        "Min zx",
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

    internal static U Stress1dExtremaKey<U>(
      IEntity1dResultSubset<IEntity1dStress, IStress1d, ResultStress1d<U>> resultSet, string key) {
      return key switch {
        "Max Axial" => resultSet.Max.Axial,
        "Max Shear Y" => resultSet.Max.ShearY,
        "Max Shear Z" => resultSet.Max.ShearZ,
        "Max Bending Y+" => resultSet.Max.BendingYyPositiveZ,
        "Max Bending Y-" => resultSet.Max.BendingYyNegativeZ,
        "Max Bending Z+" => resultSet.Max.BendingZzPositiveY,
        "Max Bending Z-" => resultSet.Max.BendingZzNegativeY,
        "Max Combined C1" => resultSet.Max.CombinedC1,
        "Max Combined C2" => resultSet.Max.CombinedC2,
        "Min Axial" => resultSet.Min.Axial,
        "Min Shear Y" => resultSet.Min.ShearY,
        "Min Shear Z" => resultSet.Min.ShearZ,
        "Min Bending Y+" => resultSet.Min.BendingYyNegativeZ,
        "Min Bending Y-" => resultSet.Min.BendingYyNegativeZ,
        "Min Bending Z+" => resultSet.Min.BendingZzPositiveY,
        "Min Bending Z-" => resultSet.Min.BendingZzNegativeY,
        "Min Combined C1" => resultSet.Min.CombinedC1,
        "Min Combined C2" => resultSet.Min.CombinedC2,
        _ => throw new ArgumentException("Extrema case not found"),
      };
    }

    internal static U Stress1dDerivedExtremaKey<U>(
      IEntity1dResultSubset<IEntity1dDerivedStress, IStress1dDerived, ResultDerivedStress1d<U>> resultSet, string key) {
      return key switch {
        "Max Shear Y" => resultSet.Max.ElasticShearY,
        "Max Shear Z" => resultSet.Max.ElasticShearZ,
        "Max Torsional" => resultSet.Max.Torsional,
        "Max von Mises" => resultSet.Max.VonMises,
        "Min Shear Y" => resultSet.Min.ElasticShearY,
        "Min Shear Z" => resultSet.Min.ElasticShearZ,
        "Min Torsional" => resultSet.Min.Torsional,
        "Min von Mises" => resultSet.Min.VonMises,
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
      IEntity2dResultSubset<T1, T2, ResultVector6<U>> resultSet, string key) where T1 : IEntity2dQuantity<T2>
    where T2 : IResultItem {
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
      IEntity1dResultSubset<T1, T2, ResultVector6<U>> resultSet, string key)
      where T1 : IEntity1dQuantity<T2> where T2 : IResultItem {
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
      IEntity1dResultSubset<T1, T2, ResultVector6<U>> resultSet, string key)
      where T1 : IEntity1dQuantity<T2> where T2 : IResultItem {
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

    internal static U StressExtremaKey<T1, T2, U>(
      IEntity2dResultSubset<T1, T2, ResultTensor3<U>> resultSet, string key) 
      where T1 : IEntity2dQuantity<T2> where T2 : IResultItem {
      return key switch {
        "Max xx" => resultSet.Max.Xx,
        "Max yy" => resultSet.Max.Yy,
        "Max zz" => resultSet.Max.Zz,
        "Max xy" => resultSet.Max.Xy,
        "Max yz" => resultSet.Max.Yz,
        "Max zx" => resultSet.Max.Zx,
        "Min xx" => resultSet.Min.Xx,
        "Min yy" => resultSet.Min.Yy,
        "Min zz" => resultSet.Min.Zz,
        "Min xy" => resultSet.Min.Xy,
        "Min yz" => resultSet.Min.Yz,
        "Min zx" => resultSet.Min.Zx,
        _ => throw new ArgumentException("Extrema case not found"),
      };
    }

    internal static U ForceExtremaKey<T1, T2, U>(
      IEntity2dResultSubset<T1, T2, ResultTensor2InAxis<U>> resultSet, string key) 
      where T1 : IEntity2dQuantity<T2> where T2 : IResultItem {
      return key switch {
        "Max Nx" => resultSet.Max.Nx,
        "Max Ny" => resultSet.Max.Ny,
        "Max Nxy" => resultSet.Max.Nxy,
        "Min Nx" => resultSet.Min.Nx,
        "Min Ny" => resultSet.Min.Ny,
        "Min Nxy" => resultSet.Min.Nxy,
        _ => throw new ArgumentException("Extrema case not found"),
      };
    }
  }
}