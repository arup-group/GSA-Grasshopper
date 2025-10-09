using System;
using System.IO;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers;

using Xunit;

namespace IntegrationTests {
  internal class Helper {

    public static GH_Document CreateDocument(string path) {
      var io = new GH_DocumentIO();

      Assert.True(File.Exists(path));
      Assert.True(io.Open(path));

      io.Document.NewSolution(true);
      return io.Document;
    }

    public static GH_Component FindComponent(GH_Document doc, string groupIdentifier) {
      foreach (IGH_DocumentObject obj in doc.Objects) {
        if (obj is GH_Group group) {
          if (group.NickName == groupIdentifier) {
            Guid componentguid = group.ObjectIDs[0];

            foreach (IGH_DocumentObject obj2 in doc.Objects) {
              if (obj2.InstanceGuid == componentguid) {
                var comp = (GH_Component)obj2;
                Assert.NotNull(comp);
                comp.Params.Output[0].CollectData();
                return comp;
              }
            }

            Assert.Fail("Unable to find component in group with Nickname " + groupIdentifier);
            return null;
          }
        }
      }

      Assert.Fail("Unable to find group with Nickname " + groupIdentifier);
      return null;
    }

    public static IGH_Param FindParameter(GH_Document doc, string groupIdentifier) {
      foreach (IGH_DocumentObject obj in doc.Objects) {
        if (obj is GH_Group group) {
          if (group.NickName == groupIdentifier) {
            Guid componentguid = group.ObjectIDs[0];

            foreach (IGH_DocumentObject obj2 in doc.Objects) {
              if (obj2.InstanceGuid == componentguid) {
                var param = (IGH_Param)(object)obj2;
                Assert.NotNull(param);
                param.CollectData();
                return param;
              }
            }

            Assert.Fail("Unable to find parameter in group with Nickname " + groupIdentifier);
            return null;
          }
        }
      }

      Assert.Fail("Unable to find group with Nickname " + groupIdentifier);
      return null;
    }

    public static void TestGhPrimitives(IGH_Param param, object expected) {
      var comparer = new DoubleComparer();
      if (expected.GetType() == typeof(string)) {
        var valOut = (GH_String)param.VolatileData.get_Branch(0)[0];
        Assert.Equal(expected, valOut.Value);
      } else if (expected.GetType() == typeof(int)) {
        var valOut = (GH_Integer)param.VolatileData.get_Branch(0)[0];
        Assert.Equal(expected, valOut.Value);
      } else if (expected.GetType() == typeof(double)) {
        var valOut = (GH_Number)param.VolatileData.get_Branch(0)[0];
        Assert.Equal((double)expected, valOut.Value, comparer);
      } else if (expected.GetType() == typeof(bool)) {
        var valOut = (GH_Boolean)param.VolatileData.get_Branch(0)[0];
        Assert.Equal(expected, valOut.Value);
      } else if (expected.GetType() == typeof(bool[])) {
        for (int i = 0; i < ((bool[])expected).Length; i++) {
          var valOut = (GH_Boolean)param.VolatileData.get_Branch(0)[i];
          Assert.Equal(((bool[])expected)[i], valOut.Value);
        }
      } else if (expected.GetType() == typeof(string[])) {
        for (int i = 0; i < ((string[])expected).Length; i++) {
          var valOut = (GH_String)param.VolatileData.get_Branch(0)[i];
          Assert.Equal(((string[])expected)[i], valOut.Value);
        }
      } else if (expected.GetType() == typeof(int[])) {
        for (int i = 0; i < ((int[])expected).Length; i++) {
          var valOut = (GH_Integer)param.VolatileData.get_Branch(0)[i];
          Assert.Equal(((int[])expected)[i], valOut.Value);
        }
      } else if (expected.GetType() == typeof(double[])) {
        for (int i = 0; i < ((double[])expected).Length; i++) {
          var valOut = (GH_Number)param.VolatileData.get_Branch(0)[i];
          Assert.Equal(((double[])expected)[i], valOut.Value, comparer);
        }
      } else {
        Assert.Fail("Expected type not found!");
      }
    }

    public static void TestNoRuntimeMessagesInDocument(
      GH_Document doc, GH_RuntimeMessageLevel runtimeMessageLevel,
      string exceptComponentNamed = "") {
      foreach (IGH_DocumentObject obj in doc.Objects) {
        if (obj is GH_Component comp) {
          comp.CollectData();
          comp.Params.Output[0].CollectData();
          comp.Params.Output[0].VolatileData.get_Branch(0);

          bool skip = false;
          foreach (IGH_DocumentObject grp in doc.Objects) {
            if (grp is GH_Group group) {
              if (group.NickName == exceptComponentNamed) {
                if (comp.InstanceGuid == group.ObjectIDs[0]) {
                  skip = true;
                  break;
                }
              }
            }
          }

          if (comp.Name != exceptComponentNamed && !skip) {
            Assert.Empty(comp.RuntimeMessages(runtimeMessageLevel));
          }
        }
      }
    }
    
  }
}
