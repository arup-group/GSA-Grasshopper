using System;
using System.Collections.Generic;
using System.Reflection;

using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaMaterialGooTests {
    [Fact]
    public void GsaConcreteMaterialTest() {
      string designCode = DesignCode.GetConcreteDesignCodeNames()[8];
      List<string> gradeNames = GsaMaterialFactory.GetGradeNames(
        MatType.Concrete, designCode, string.Empty);
      var material = (GsaMaterial)GsaMaterialFactory.CreateStandardMaterial(
        MatType.Concrete, gradeNames[3], designCode);
      GsaMaterialGooTest(new GsaMaterialGoo(material));
    }

    [Fact]
    public void GsaSteelMaterialTest() {
      string designCode = DesignCode.GetSteelDesignCodeNames()[8];
      List<string> gradeNames = GsaMaterialFactory.GetGradeNames(
        MatType.Steel, string.Empty, designCode);
      var material = (GsaMaterial)GsaMaterialFactory.CreateStandardMaterial(
        MatType.Steel, gradeNames[3], designCode);
      GsaMaterialGooTest(new GsaMaterialGoo(material));
    }

    [Theory]
    [InlineData(MatType.Aluminium)]
    [InlineData(MatType.Fabric)]
    [InlineData(MatType.Frp)]
    [InlineData(MatType.Glass)]
    [InlineData(MatType.Timber)]
    public void GsaMaterialTest(MatType type) {
      List<string> gradeNames = GsaMaterialFactory.GetGradeNames(
        type, string.Empty, string.Empty);
      var material = (GsaMaterial)GsaMaterialFactory.CreateStandardMaterial(
        type, gradeNames[1]);
      GsaMaterialGooTest(new GsaMaterialGoo(material));
    }


    private void GsaMaterialGooTest(GsaMaterialGoo objectGoo, bool excludeGuid = false) {
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
