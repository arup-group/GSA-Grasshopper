﻿using System.Collections.Generic;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;

namespace GsaGH.Helpers {
  internal class PostHog {

    internal static void Debug(Dictionary<string, object> properties) {
      const string eventName = "Debug";
      _ = OasysGH.Helpers.PostHog.SendToPostHog(PluginInfo.Instance, eventName, properties);
    }

    internal static void Gwa(string gwa, bool existingModel) {
      string[] commands = gwa.Split('\n');
      foreach (string command in commands) {
        if (command == string.Empty) {
          continue;
        }

        string key = command.Split('.')[0].Split(',')[0].Split('\t')[0].Split(' ')[0];
        if (key == string.Empty) {
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
      IGsaLoad load, ReferenceType refType, string subType = "-") {
      const string eventName = "Load";
      bool objLoad = refType != ReferenceType.None;
      var properties = new Dictionary<string, object>() {
        {
          "loadType", load.GetType().ToString()
            .Replace("Gsa", string.Empty).Replace("Load", string.Empty)
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
      CaseType caseType, int dimension, GsaResultValues.ResultType resultType,
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
      CaseType caseType, int dimension, string resultType, string subType = "-") {
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
