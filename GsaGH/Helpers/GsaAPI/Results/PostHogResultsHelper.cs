using GsaGH.Parameters;
using System.Collections.Generic;
using System.Data;

namespace GsaGH.Helpers
{
  internal class PostHogResultsHelper
  {
    internal static void PostHog(GsaResult.CaseType caseType, int dimension, GsaResultsValues.ResultType resultType, string subType = "-")
    {
      string eventName = "Result";
      Dictionary<string, object> properties = new Dictionary<string, object>()
        {
          { "caseType", caseType.ToString() },
          { "elementType", dimension },
          { "resultType", resultType.ToString() },
          { "resultSubType", subType },
        };
      _ = OasysGH.Helpers.PostHog.SendToPostHog(PluginInfo.Instance, eventName, properties);
    }

    internal static void PostHog(GsaResult.CaseType caseType, int dimension, string resultType, string subType = "-")
    {
      string eventName = "Result";
      Dictionary<string, object> properties = new Dictionary<string, object>()
        {
          { "caseType", caseType.ToString() },
          { "elementType", dimension },
          { "resultType", resultType },
          { "resultSubType", subType },
        };
      _ = OasysGH.Helpers.PostHog.SendToPostHog(PluginInfo.Instance, eventName, properties);
    }
  }
}
