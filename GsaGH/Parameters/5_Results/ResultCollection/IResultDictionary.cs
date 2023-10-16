using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public interface IResultDictionary<IResult> {
    public IDictionary<int, IResultCollection<IResult>> Results { get; set; }
  }
}
