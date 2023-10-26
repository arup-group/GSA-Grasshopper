namespace GsaGH.Parameters.Results {
  public class NodeExtremaVector6 : IResultVector6<NodeExtremaKey, NodeExtremaKey>, IResultExtrema {
    public NodeExtremaKey X { get; internal set; }
    public NodeExtremaKey Y { get; internal set; }
    public NodeExtremaKey Z { get; internal set; }
    public NodeExtremaKey Xyz { get; internal set; }
    public NodeExtremaKey Xx { get; internal set; }
    public NodeExtremaKey Yy { get; internal set; }
    public NodeExtremaKey Zz { get; internal set; }
    public NodeExtremaKey Xxyyzz { get; internal set; }
  }
}
