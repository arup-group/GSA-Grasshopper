using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsaGH.Helpers.GH {
  public struct Range {
    public int Min;
    public int Max;
    public int Length;
    public Range(int min, int max) {
      Min = min;
      Max = max;
      Length = max - min;
    }
  }
}
