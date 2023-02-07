using GsaGH.Parameters;
using System.Collections.Generic;
using System.Data;

namespace GsaGH.Helpers
{
  internal class PostHog
  {
    internal static void Result(GsaResult.CaseType caseType, int dimension, GsaResultsValues.ResultType resultType, string subType = "-")
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

    internal static void Result(GsaResult.CaseType caseType, int dimension, string resultType, string subType = "-")
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

    internal static void Load(GsaLoad.LoadTypes loadType, ReferenceType refType, string subType = "-")
    {
      string eventName = "Load";
      bool objLoad = refType != ReferenceType.None;
      Dictionary<string, object> properties = new Dictionary<string, object>()
        {
          { "loadType", loadType.ToString() },
          { "objectLoad", objLoad },
          { "refType", refType.ToString() },
          { "loadSubType", subType },
        };
      _ = OasysGH.Helpers.PostHog.SendToPostHog(PluginInfo.Instance, eventName, properties);
    }
    internal static void Load(bool refType, string subType = "-")
    {
      string eventName = "Load";
      Dictionary<string, object> properties = new Dictionary<string, object>()
        {
          { "loadType", "Node" },
          { "objectLoad", refType },
          { "refType", refType ? "Node" : "None" },
          { "loadSubType", subType },
        };
      _ = OasysGH.Helpers.PostHog.SendToPostHog(PluginInfo.Instance, eventName, properties);
    }

    internal static void GWA(string gwa, bool existingModel)
    {
      string[] commands = gwa.Split('\n');
      foreach (string command in commands)
      {
        if (command == "") { continue; }
        string key = command.Split('.')[0].Split(',')[0].Split('\t')[0].Split(' ')[0];
        if (key == "") { continue; }
        string eventName = "GwaCommand";
        Dictionary<string, object> properties = new Dictionary<string, object>()
        {
          { key, command },
          { "existingModel", existingModel },
        };
        _ = OasysGH.Helpers.PostHog.SendToPostHog(GsaGH.PluginInfo.Instance, eventName, properties);
      }
    }
  }
}
