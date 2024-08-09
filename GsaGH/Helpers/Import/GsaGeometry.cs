using System.Collections.Generic;

using GsaGH.Parameters;

namespace GsaGH.Helpers.Import {
  internal class GsaGeometry {
    internal List<GsaNode> Nodes { get; set; } = new List<GsaNode>();
    internal List<GsaElement1d> Element1ds { get; set; } = new List<GsaElement1d>();
    internal List<GsaElement2d> Element2ds { get; set; } = new List<GsaElement2d>();
    internal List<GsaElement3d> Element3ds { get; set; } = new List<GsaElement3d>();
    internal List<GsaMember1d> Member1ds { get; set; } = new List<GsaMember1d>();
    internal List<GsaMember2d> Member2ds { get; set; } = new List<GsaMember2d>();
    internal List<GsaMember3d> Member3ds { get; set; } = new List<GsaMember3d>();
    internal List<GsaAssembly> Assemblies { get; set; } = new List<GsaAssembly>();

    internal bool IsNullOrEmpty() {
      return Nodes.IsNullOrEmpty() & Element1ds.IsNullOrEmpty() & Element2ds.IsNullOrEmpty() & Element3ds.IsNullOrEmpty() & Member1ds.IsNullOrEmpty() & Member2ds.IsNullOrEmpty() & Member3ds.IsNullOrEmpty() & Assemblies.IsNullOrEmpty();
    }
  }
}
