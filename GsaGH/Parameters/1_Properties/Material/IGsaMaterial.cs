using System;

namespace GsaGH.Parameters {
  public interface IGsaMaterial {
    string Name { get; set; }
    int Id { get; set; }
    Guid Guid { get; }
    MatType Type { get; }
    IGsaMaterial Duplicate();
  }
}
