using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsaGH.Parameters._5_Results {
  public class GsaNodeDisplacement {
    internal Dictionary<string, GsaResultsValues> ACaseResultValues { get; private set; }
      = new Dictionary<string, GsaResultsValues>();

    internal void AddACaseValue(string nodelist, GsaResultsValues values) {
      ACaseResultValues.Add(nodelist, values);
    }
  }
}
