using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsaGH.Parameters {
  public abstract class GsaProperty : IGsaObject {
    public int Id { get; set; } = 0;
    public bool IsReferencedById { get; set; } = false;
    public virtual Guid Guid { get; set; } = Guid.NewGuid();
    public virtual GsaMaterial Material { get; set; }

    public abstract IGsaObject Clone();
    public abstract void DuplicateApiObject();
  }
}
