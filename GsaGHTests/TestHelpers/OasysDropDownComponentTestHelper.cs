using System;
using System.Collections.Generic;
using System.IO;

using Grasshopper.Kernel;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Helpers {
  public class OasysDropDownComponentTestHelper {

    public static void ChangeDropDownTest(
      GH_OasysDropDownComponent comp, bool ignoreSpacerDescriptionsCount = false) {
      Assert.True(comp.IsInitialised);
      if (!ignoreSpacerDescriptionsCount) {
        Assert.Equal(comp.DropDownItems.Count, comp.SpacerDescriptions.Count);
      }

      Assert.Equal(comp.DropDownItems.Count, comp.SelectedItems.Count);

      for (int i = 0; i < comp.DropDownItems.Count; i++) {
        comp.SetSelected(i, 0);

        for (int j = 0; j < comp.DropDownItems[i].Count; j++) {
          comp.SetSelected(i, j);
          TestDeserialize(comp);
          Assert.Equal(comp.SelectedItems[i], comp.DropDownItems[i][j]);
        }
      }
    }

    public static void TestDeserialize(GH_OasysComponent comp, string customIdentifier = "") {
      comp.CreateAttributes();

      var doc = new GH_Document();
      doc.AddObject(comp, true);

      var serialize = new GH_DocumentIO {
        Document = doc,
      };
      var originalComponent = (GH_Component)serialize.Document.Objects[0];
      originalComponent.Attributes.PerformLayout();
      originalComponent.ExpireSolution(true);
      originalComponent.Params.Output[0].CollectData();

      string path = Path.Combine(Environment.CurrentDirectory, "GH-Test-Files");
      Directory.CreateDirectory(path);
      Type myType = comp.GetType();
      string pathFileName = Path.Combine(path, myType.Name) + customIdentifier + ".gh";
      Assert.True(serialize.SaveQuiet(pathFileName));

      var deserialize = new GH_DocumentIO();
      Assert.True(deserialize.Open(pathFileName));

      var deserializedComponent = (GH_Component)deserialize.Document.Objects[0];
      deserializedComponent.Attributes.PerformLayout();
      deserializedComponent.ExpireSolution(true);
      deserializedComponent.Params.Output[0].CollectData();

      Duplicates.AreEqual(originalComponent, deserializedComponent, new List<string>() { "Guid", "Capacity" });
      doc.Dispose();
    }
  }
}
