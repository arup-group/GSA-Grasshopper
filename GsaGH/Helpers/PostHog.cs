using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using GsaGH.Parameters;
using GsaGH.Parameters.Enums;

namespace GsaGH.Helpers {
  internal class PostHog {
    // Tracks component instances without preventing garbage collection.
    private static readonly ConditionalWeakTable<object, object> _instanceTracking =
      new ConditionalWeakTable<object, object>();

    private static void TrackOnce(object componentInstance, Action postHogAction) {
      if (!_instanceTracking.TryGetValue(componentInstance, out _)) {
        postHogAction();
        _instanceTracking.Add(componentInstance, true);
      }
    }

    internal static void TrackGwaOnce(object componentInstance, string gwa, bool existingModel) {
      TrackOnce(componentInstance, () => Gwa(gwa, existingModel));
    }

    internal static void TrackDiagramOnce(
      object componentInstance, string diagramType, CaseType caseType, string type, string subTypes,
      EntityType entityType) {
      TrackOnce(componentInstance, () => Diagram(diagramType, caseType, type, subTypes, entityType));
    }

    internal static void TrackDiagramOnce(
      object componentInstance, string diagramType, string caseId, string type, List<GsaAPI.DiagramType> subTypes,
      EntityType entityType) {
      TrackOnce(componentInstance, () => Diagram(diagramType, caseId, type, subTypes, entityType));
    }

    internal static void TrackResultOnce(
      object componentInstance, CaseType caseType, int dimension, string resultType, string subType = "-") {
      TrackOnce(componentInstance, () => Result(caseType, dimension, resultType, subType));
    }

    internal static void TrackLoadOnce(
      object componentInstance, IGsaLoad load, ReferenceType refType, string subType = "-") {
      TrackOnce(componentInstance, () => Load(load, refType, subType));
    }

    internal static void TrackLoadOnce(object componentInstance, bool refType, string subType = "-") {
      TrackOnce(componentInstance, () => Load(refType, subType));
    }

    internal static void TrackModelIOOnce(object componentInstance, string operation, int sizeOrCount) {
      TrackOnce(componentInstance, () => OasysGH.Helpers.PostHog.ModelIO(PluginInfo.Instance, operation, sizeOrCount));
    }

    private static void SendEvent(string eventName, Dictionary<string, object> properties) {
      _ = OasysGH.Helpers.PostHog.SendToPostHog(PluginInfo.Instance, eventName, properties);
    }

    internal static void Debug(Dictionary<string, object> properties) {
      const string eventName = "Debug";
      SendEvent(eventName, properties);
    }

    internal static void Diagram(string diagramType, CaseType caseType, string type, string subTypes, EntityType entityType) {
      const string eventName = "Diagram";
      var properties = new Dictionary<string, object>() {
        {
          "diagramType", diagramType
        }, {
          "caseType", caseType.ToString()
        }, {
          "type", type
        }, {
          "subType", subTypes
        }, {
          "entityType", entityType.ToString()
        },
      };
      SendEvent(eventName, properties);
    }

    internal static void Diagram(
      string diagramType, string caseId, string type, List<GsaAPI.DiagramType> subTypes, EntityType entityType) {
      CaseType caseType = caseId.StartsWith("L") ? CaseType.LoadCase
        : caseId.StartsWith("A") ? CaseType.AnalysisCase : CaseType.CombinationCase;
      List<string> subType = subTypes.ConvertAll(x => x.ToString());
      Diagram(diagramType, caseType, type, string.Join(";", subTypes), entityType);
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
        SendEvent(eventName, properties);
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
      SendEvent(eventName, properties);
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
      SendEvent(eventName, properties);
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
      SendEvent(eventName, properties);
    }
  }
}
