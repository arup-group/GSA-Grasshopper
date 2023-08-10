using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsaGH.Parameters.Enums {
  public class LoadCase {
    public enum LoadCaseType {
      Dead = 11,
      Soil = 12,
      Notional = 19,
      Prestress = 21,
      Live = 0x1F,
      LiveRoof = 0x20,
      Wind = 33,
      Snow = 34,
      Rain = 35,
      Thermal = 36,
      Equivalent = 39,
      Accidental = 41,
      EarthquakeRSA = 51,
      EarthquakeAccTors = 53,
      EarthquakeStatic = 52
    }
  }
}
