using System;

namespace GsaGH.Parameters {
  public interface IGsaProperty {
    int Id { get; }
    public bool IsReferencedById { get; }
    Guid Guid { get; }
  }
}
