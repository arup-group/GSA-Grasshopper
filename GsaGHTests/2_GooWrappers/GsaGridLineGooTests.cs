using System;
using System.Collections.Generic;
using System.Reflection;

using Grasshopper.Kernel.Types;

using GsaGH.Parameters;

using GsaGHTests.Helpers;
using GsaGHTests.Model;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaGridLineGooTests {
    private CreateGridLineTestHelper _helper;

    public GsaGridLineGooTests() {
      _helper = new CreateGridLineTestHelper();
    }

    [Fact]
    public void ArcGridLineGooTest() {
      var arc = new Arc(new Point3d(0, 0, 0), 0.5, Math.PI / 4);
      var gridline = new GsaGridLine(arc, "Arc");
      GsaGridLineGooTest(new GsaGridLineGoo(gridline));
    }

    [Fact]
    public void LineGridLineGooTest() {
      var line = new Line(new Point3d(10, 15, 0), new Point3d(20, 15, 0));
      var gridline = new GsaGridLine(line, "Line");
      GsaGridLineGooTest(new GsaGridLineGoo(gridline));
    }

    [Fact]
    public void GsaGridLineGooLinePreviewTest() {
      _helper.CreateComponentWithLineInput();
      _helper.GetGridLineOutput();
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(_helper.GetComponent());
    }

    [Fact]
    public void GsaGridLineGooArcPreviewTest() {
      _helper.CreateComponentWithArcInput();
      _helper.GetGridLineOutput();
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(_helper.GetComponent());
    }

    private void GsaGridLineGooTest(GsaGridLineGoo objectGoo, bool excludeGuid = false) {
      object value = objectGoo.Value;
      Type gooType = objectGoo.GetType();

      IGH_Goo duplicate = ((IGH_Goo)objectGoo).Duplicate();
      List<string> excluded = excludeGuid ? new List<string>() { "Guid" } : null;
      Duplicates.AreEqual(objectGoo, duplicate, excluded);

      bool hasValue = false;
      bool hasToString = false;
      bool hasName = false;
      bool hasNickName = false;
      bool hasDescription = false;

      PropertyInfo[] gooPropertyInfo
        = gooType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
      foreach (PropertyInfo gooProperty in gooPropertyInfo) {
        if (gooProperty.Name == "Value") {
          object gooValue = gooProperty.GetValue(objectGoo, null);
          Duplicates.AreEqual(value, gooValue, excluded);

          MethodInfo methodInfo = gooValue.GetType().GetMethod("Clone");
          if (methodInfo != null) {
            object cloneValue = methodInfo.Invoke(gooValue, null);
            cloneValue = methodInfo.Invoke(gooValue, null);
            Assert.NotSame(gooValue, cloneValue);
            Duplicates.AreEqual(gooValue, cloneValue, excluded);
          }

          ConstructorInfo duplicateConstructor = gooValue.GetType()
            .GetConstructor(new Type[] { gooValue.GetType() }); // new GsaObj(GsaObj other);
          object duplicateValue = duplicateConstructor.Invoke(new object[] { gooValue });
          Duplicates.AreEqual(gooValue, duplicateValue, new List<string>() { "Guid" });

          hasValue = true;
        }

        if (gooProperty.Name == "TypeName") {
          string typeName = (string)gooProperty.GetValue(objectGoo, null);
          Assert.StartsWith("GSA " + typeName + " (", objectGoo.ToString());
          hasToString = true;
        }

        if (gooProperty.Name == "Name") {
          string name = (string)gooProperty.GetValue(objectGoo, null);
          Assert.True(name.Length > 3);
          hasName = true;
        }

        if (gooProperty.Name == "NickName") {
          string nickName = (string)gooProperty.GetValue(objectGoo, null);
          nickName = nickName.Replace(".", string.Empty);
          Assert.True(nickName.Length < 4);
          Assert.True(nickName.Length > 0);
          hasNickName = true;
        }

        if (gooProperty.Name != "Description") {
          continue;
        }

        string description = (string)gooProperty.GetValue(objectGoo, null);
        Assert.StartsWith("GSA", description);
        Assert.True(description.Length > 7);
        hasDescription = true;
      }

      Assert.True(hasValue);
      Assert.True(hasToString);
      Assert.True(hasName);
      Assert.True(hasNickName);
      Assert.True(hasDescription);
    }
  }
}
