using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsaGH.Parameters {
  public class GsaNodeDisplacements {
    internal Dictionary<string, GsaResultsValues> ACaseResultValues { get; private set; }
      = new Dictionary<string, GsaResultsValues>();

    internal void AddAnalysisCaseValue(string nodelist, GsaResultsValues values) {
      ACaseResultValues.Add(nodelist, values);
    }
  }
}
