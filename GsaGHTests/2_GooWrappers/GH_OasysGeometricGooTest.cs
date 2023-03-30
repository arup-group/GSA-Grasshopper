using System;
using System.Reflection;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GhOasysGeometricGooTest {
    [Theory]
    [InlineData(typeof(GsaElement1dGoo), typeof(GsaElement1d))]
    [InlineData(typeof(GsaElement2dGoo), typeof(GsaElement2d))]
    [InlineData(typeof(GsaElement3dGoo), typeof(GsaElement3d))]
    [InlineData(typeof(GsaMember1dGoo), typeof(GsaMember1d))]
    [InlineData(typeof(GsaMember2dGoo), typeof(GsaMember2d))]
    [InlineData(typeof(GsaMember3dGoo), typeof(GsaMember3d))]
    [InlineData(typeof(GsaNodeGoo), typeof(GsaNode))]
    [InlineData(typeof(GsaGridPlaneSurfaceGoo), typeof(GsaGridPlaneSurface))]
    public void GenericGH_OasysGeometricGooTest(Type gooType, Type wrapType) {
      object value = Activator.CreateInstance(wrapType);
      object[] parameters = {
        value,
      };

      object objectGoo = Activator.CreateInstance(gooType, parameters);
      gooType = objectGoo.GetType();

      IGH_Goo duplicate = ((IGH_Goo)objectGoo).Duplicate();
      Duplicates.AreEqual(duplicate, objectGoo);
      Assert.NotEqual(duplicate, objectGoo);

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
          Duplicates.AreEqual(value, gooValue);
          Assert.NotEqual(value, gooValue);

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

        if (gooProperty.Name != "Description")
          continue;
        string description = (string)gooProperty.GetValue(objectGoo, null);
        Assert.StartsWith("GSA ", description);
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
