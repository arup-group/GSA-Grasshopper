using System;
using System.IO;
using System.Reflection;
using Grasshopper.Documentation;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using Xunit;

namespace IntegrationTests
{
  internal class Helper
  {
    public static void TestGHPrimitives(IGH_Param param, object expected, int tolerance = 6)
    {
      if (expected.GetType() == typeof(string))
      {
        GH_String valOut = (GH_String)param.VolatileData.get_Branch(0)[0];
        Assert.Equal(expected, valOut.Value);
      }
      else if (expected.GetType() == typeof(int))
      {
        GH_Integer valOut = (GH_Integer)param.VolatileData.get_Branch(0)[0];
        Assert.Equal(expected, valOut.Value);
      }
      else if (expected.GetType() == typeof(double))
      {
        GH_Number valOut = (GH_Number)param.VolatileData.get_Branch(0)[0];
        Assert.Equal((double)expected, valOut.Value, tolerance);
      }
      else if (expected.GetType() == typeof(bool))
      {
        GH_Boolean valOut = (GH_Boolean)param.VolatileData.get_Branch(0)[0];
        Assert.Equal(expected, valOut.Value);
      }
      else if (expected.GetType() == typeof(bool[]))
      {
        for (int i = 0; i < ((bool[])expected).Length; i++)
        {
          GH_Boolean valOut = (GH_Boolean)param.VolatileData.get_Branch(0)[i];
          Assert.Equal(((bool[])expected)[i], valOut.Value);
        }
      }
      else if (expected.GetType() == typeof(string[]))
      {
        for (int i = 0; i < ((string[])expected).Length; i++)
        {
          GH_String valOut = (GH_String)param.VolatileData.get_Branch(0)[i];
          Assert.Equal(((string[])expected)[i], valOut.Value);
        }
      }
      else if (expected.GetType() == typeof(int[]))
      {
        for (int i = 0; i < ((int[])expected).Length; i++)
        {
          GH_Integer valOut = (GH_Integer)param.VolatileData.get_Branch(0)[i];
          Assert.Equal(((int[])expected)[i], valOut.Value);
        }
      }
      else if (expected.GetType() == typeof(double[]))
      {
        for (int i = 0; i < ((double[])expected).Length; i++)
        {
          GH_Number valOut = (GH_Number)param.VolatileData.get_Branch(0)[i];
          Assert.Equal(((double[])expected)[i], valOut.Value, tolerance);
        }
      }
      else
        Assert.True(false, "Expected type not found!");
    }
    public static GH_Document CreateDocument(string path)
    {
      GH_DocumentIO io = new GH_DocumentIO();

      Assert.True(File.Exists(path));
      Assert.True(io.Open(path));

      io.Document.NewSolution(true);
      return io.Document;
    }

    public static GH_Component FindComponent(GH_Document doc, string groupIdentifier)
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
                GH_Component comp = (GH_Component)obj2;
                Assert.NotNull(comp);
                comp.Params.Output[0].CollectData();
                return comp;
              }
            }
            Assert.True(false, "Unable to find component in group with Nickname " + groupIdentifier);
            return null;
          }
        }
      }
      Assert.True(false, "Unable to find group with Nickname " + groupIdentifier);
      return null;
    }

    public static IGH_Param FindParameter(GH_Document doc, string groupIdentifier)
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
                IGH_Param param = (IGH_Param)(object)obj2;
                Assert.NotNull(param);
                param.CollectData();
                return param;
              }
            }
            Assert.True(false, "Unable to find parameter in group with Nickname " + groupIdentifier);
            return null;
          }
        }
      }
      Assert.True(false, "Unable to find group with Nickname " + groupIdentifier);
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
