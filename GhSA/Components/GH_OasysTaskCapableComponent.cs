
using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using GsaGH.Helpers;

namespace GsaGH.Components
{
  public abstract class GH_OasysTaskCapableComponent<T> : GH_TaskCapableComponent<T>
  {
    public GH_OasysTaskCapableComponent(string name, string nickname, string description, string category, string subCategory) : base(name, nickname, description, category, subCategory)
    {
    }

    public override void AddedToDocument(GH_Document document)
    {
      string eventName = this.Name + "_AddedToDocument";
      _ = PostHog.SendToPostHog(eventName);

      base.AddedToDocument(document);
    }

    public override void RemovedFromDocument(GH_Document document)
    {
      string eventName = this.Name + "_RemovedFromDocument";
      Dictionary<string, object> properties = new Dictionary<string, object>()
      {
        { "runCount", this.RunCount },
      };
      _ = PostHog.SendToPostHog(eventName, properties);

      base.RemovedFromDocument(document);
    }
  }
}
