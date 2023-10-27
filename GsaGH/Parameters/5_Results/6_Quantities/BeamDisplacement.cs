using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class BeamDisplacement : IBeamDisplacement {
    public ICollection<IDisplacement> Displacements { get; }
    public ICollection<double> Positions { get; }

    internal BeamDisplacement(ICollection<Double6> result, ICollection<double> positions) {
      foreach (Double6 item in result) {
        Displacements.Add(new Displacement(item));
      }
      Positions = positions;  
    }

    //private Angle CreateAngle(double val) {
    //  // TO-DO: GSA-5351 remove NaN and Infinity values from GsaAPI results
    //  if (!double.IsNaN(val)) {
    //    return !double.IsInfinity(val)
    //      ? new Angle(val, AngleUnit.Radian)
    //      : (double.IsPositiveInfinity(val)
    //        ? new Angle(360, AngleUnit.Degree)
    //        : new Angle(-360, AngleUnit.Degree));
    //  } else {
    //    return Angle.Zero;
    //  }
    //}
  }
}
