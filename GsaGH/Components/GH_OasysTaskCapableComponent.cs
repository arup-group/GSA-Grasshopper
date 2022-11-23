
using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using GsaGH.Helpers;
using OasysGH.Components;
using OasysGH.Helpers;
using Rhino.PlugIns;

namespace GsaGH.Components
{
  public abstract class GH_OasysTaskCapableComponent<T> : GH_TaskCapableComponent<T>
  {
    public GH_OasysTaskCapableComponent(string name, string nickname, string description, string category, string subCategory) : base(name, nickname, description, category, subCategory)
    {
    }

    public override void AddedToDocument(GH_Document document)
    {
      PostHog.AddedToDocument(this, GsaGH.PluginInfo.Instance);
      base.AddedToDocument(document);
    }

    public override void RemovedFromDocument(GH_Document document)
    {
      PostHog.RemovedFromDocument(this, GsaGH.PluginInfo.Instance);
      base.RemovedFromDocument(document);
    }
  }
}
