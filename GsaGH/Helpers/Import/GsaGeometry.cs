using System.Collections.Generic;

using GsaGH.Parameters;

namespace GsaGH.Helpers.Import {
  internal class GsaGeometry {
    internal List<GsaNode> Nodes { get; set; } = new List<GsaNode>();
    internal List<GsaElement1D> Element1ds { get; set; } = new List<GsaElement1D>();
    internal List<GsaElement2D> Element2ds { get; set; } = new List<GsaElement2D>();
    internal List<GsaElement3D> Element3ds { get; set; } = new List<GsaElement3D>();
    internal List<GsaMember1D> Member1ds { get; set; } = new List<GsaMember1D>();
    internal List<GsaMember2D> Member2ds { get; set; } = new List<GsaMember2D>();
    internal List<GsaMember3D> Member3ds { get; set; } = new List<GsaMember3D>();
    internal List<GsaAssembly> Assemblies { get; set; } = new List<GsaAssembly>();

    internal bool IsNullOrEmpty() {
      return Nodes.IsNullOrEmpty() & Element1ds.IsNullOrEmpty() & Element2ds.IsNullOrEmpty() & Element3ds.IsNullOrEmpty() & Member1ds.IsNullOrEmpty() & Member2ds.IsNullOrEmpty() & Member3ds.IsNullOrEmpty() & Assemblies.IsNullOrEmpty();
    }
  }
}
