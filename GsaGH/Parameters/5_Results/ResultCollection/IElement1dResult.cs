using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public interface IElement1dResult<IResult> { 
    ICollection<IResult> Result { get; set; }
  } 
}
