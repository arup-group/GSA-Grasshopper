
using System;
using Grasshopper.Kernel;
using GsaGH.Helpers;

namespace GsaGH.Components
{
  public abstract class PostHogComponent : GH_Component
  {
    public PostHogComponent(string name, string nickname, string description, string category, string subCategory) : base(name, nickname, description, category, subCategory)
    {
    }

    public override void AddedToDocument(GH_Document document)
    {
      string eventName = this.Name + "_AddedToDocument";
      _ = PostHog.SendToPostHog(eventName);

      base.AddedToDocument(document);
    }
  }
}
