using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public interface IResultSubset<IResult> : IResultDictionary<IResult> {
    public IResult Max { get; set; }
    public IResult Min { get; set; }
    public List<int> Ids { get; }
  } 
}
