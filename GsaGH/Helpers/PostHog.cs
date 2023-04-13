using System.Collections.Generic;
using GsaGH.Parameters;

namespace GsaGH.Helpers {
  internal class PostHog {

    internal static void Debug(Dictionary<string, object> properties) {
      const string eventName = "Debug";
      _ = OasysGH.Helpers.PostHog.SendToPostHog(PluginInfo.Instance, eventName, properties);
    }

    internal static void Gwa(string gwa, bool existingModel) {
      string[] commands = gwa.Split('\n');
      foreach (string command in commands) {
        if (command == "") {
          continue;
        }

        string key = command.Split('.')[0].Split(',')[0].Split('\t')[0].Split(' ')[0];
        if (key == "") {
          continue;
        }

        const string eventName = "GwaCommand";
        var properties = new Dictionary<string, object>() {
          {
            key, command
          }, {
            "existingModel", existingModel
          },
        };
        _ = OasysGH.Helpers.PostHog.SendToPostHog(PluginInfo.Instance, eventName, properties);
      }
    }

    internal static void Load(
      GsaLoad.LoadTypes loadType, ReferenceType refType, string subType = "-") {
      const string eventName = "Load";
      bool objLoad = refType != ReferenceType.None;
      var properties = new Dictionary<string, object>() {
        {
          "loadType", loadType.ToString()
        }, {
          "objectLoad", objLoad
        }, {
          "refType", refType.ToString()
        }, {
          "loadSubType", subType
        },
      };
      _ = OasysGH.Helpers.PostHog.SendToPostHog(PluginInfo.Instance, eventName, properties);
    }

    internal static void Load(bool refType, string subType = "-") {
      const string eventName = "Load";
      var properties = new Dictionary<string, object>() {
        {
          "loadType", "Node"
        }, {
          "objectLoad", refType
        }, {
          "refType", refType ? "Node" : "None"
        }, {
          "loadSubType", subType
        },
      };
      _ = OasysGH.Helpers.PostHog.SendToPostHog(PluginInfo.Instance, eventName, properties);
    }

    internal static void Result(
      GsaResult.CaseType caseType, int dimension, GsaResultsValues.ResultType resultType,
      string subType = "-") {
      const string eventName = "Result";
      var properties = new Dictionary<string, object>() {
        {
          "caseType", caseType.ToString()
        }, {
          "elementType", dimension
        }, {
          "resultType", resultType.ToString()
        }, {
          "resultSubType", subType
        },
      };
      _ = OasysGH.Helpers.PostHog.SendToPostHog(PluginInfo.Instance, eventName, properties);
    }

    internal static void Result(
      GsaResult.CaseType caseType, int dimension, string resultType, string subType = "-") {
      const string eventName = "Result";
      var properties = new Dictionary<string, object>() {
        {
          "caseType", caseType.ToString()
        }, {
          "elementType", dimension
        }, {
          "resultType", resultType
        }, {
          "resultSubType", subType
        },
      };
      _ = OasysGH.Helpers.PostHog.SendToPostHog(PluginInfo.Instance, eventName, properties);
    }
  }
}
