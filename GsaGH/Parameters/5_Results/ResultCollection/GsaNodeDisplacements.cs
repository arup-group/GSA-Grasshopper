using GsaGH.Parameters.Results;
using System;

namespace GsaGH.Parameters {
  
  public class GsaNodeDisplacements : IResults<GsaDisplacementQuantity> {
    public IResultDictionary<IResultCollection<GsaDisplacementQuantity>> ResultCache { 
      get => throw new NotImplementedException(); 
      set => throw new NotImplementedException(); 
    }

    public IResultSubset<GsaDisplacementQuantity> GetResultSet(string definition) {
      throw new NotImplementedException();
    }
  }
}
