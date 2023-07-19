using System;
using GsaAPI;

namespace GsaGH.Parameters {
  public class GsaDiaphragmLoad : GsaFaceLoad {
    public GsaDiaphragmLoad() : base() { }
    internal int AndrewsID { get; set; }

    public override string ToString() {
      return string.Join(" ", "Diaphragm", FaceLoad.Name.Trim()).Trim().Replace("  ", " ");
    }
  }
}
