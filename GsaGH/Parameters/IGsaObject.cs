using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsaGH.Parameters {
  public interface IGsaObject {
    IGsaObject Clone();
    void DuplicateApiObject();
  }
}
