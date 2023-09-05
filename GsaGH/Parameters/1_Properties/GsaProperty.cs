using System;
using System.Linq;
using GsaGH.Helpers.GsaApi;

namespace GsaGH.Parameters {
  public abstract class GsaProperty {
    public int Id { get; set; } = 0;
    public bool IsReferencedById { get; set; } = false;
    public virtual Guid Guid { get; set; } = Guid.NewGuid();
    public virtual GsaMaterial Material { get; set; }
    public virtual string MaterialType => 
      Material == null ? string.Empty : Mappings.materialTypeMapping.FirstOrDefault(x => x.Value == Material.MaterialType).Key;
  }
}
