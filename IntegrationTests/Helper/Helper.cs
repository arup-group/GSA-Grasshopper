using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using Xunit;

namespace IntegrationTests
{
  internal class Helper
  {
    public static GH_Component FindComponentInDocumentByGroup(GH_Document doc, string groupIdentifier)
    {
      foreach (var obj in doc.Objects)
      {
        if (obj is GH_Group group)
        {
          if (group.NickName == groupIdentifier)
          {
            Guid componentguid = group.ObjectIDs[0];

            foreach (var obj2 in (doc.Objects))
            {
              if (obj2.InstanceGuid == componentguid)
              {
                return (GH_Component)obj2;
              }
            }
          }
        }
      }
      return null;
    }

    public static GH_Param<T> FindComponentInDocumentByGroup<T>(GH_Document doc, string groupIdentifier) where T : class, IGH_Goo
    {
      foreach (var obj in doc.Objects)
      {
        if (obj is GH_Group group)
        {
          if (group.NickName == groupIdentifier)
          {
            Guid componentguid = group.ObjectIDs[0];

            foreach (var obj2 in (doc.Objects))
            {
              if (obj2.InstanceGuid == componentguid)
              {
                return (GH_Param<T>)obj2;
              }
            }
          }
        }
      }
      return null;
    }

    public static void TestNoRuntimeMessagesInDocument(GH_Document doc, GH_RuntimeMessageLevel runtimeMessageLevel, string exceptComponentNamed = "")
    {
      foreach (var obj in doc.Objects)
      {
        if (obj is GH_Component comp)
        {
          comp.CollectData();
          comp.Params.Output[0].CollectData();
          comp.Params.Output[0].VolatileData.get_Branch(0);
          if (comp.Name != exceptComponentNamed)
            Assert.Empty(comp.RuntimeMessages(runtimeMessageLevel));
        }
      }
    }
  }
}
