using System;
using System.Linq;
using GsaGH.Helpers.GsaApi;

namespace GsaGH.Parameters {
  public abstract class Property {
    public int Id { get; set; } = 0;
    public bool IsReferencedById { get; set; } = false;
    public Guid Guid { get; set; } = Guid.NewGuid();
    public GsaMaterial Material { get; set; }
    public virtual string MaterialType => 
      Material == null ? string.Empty : Mappings.materialTypeMapping.FirstOrDefault(x => x.Value == Material.MaterialType).Key;
  }
}
