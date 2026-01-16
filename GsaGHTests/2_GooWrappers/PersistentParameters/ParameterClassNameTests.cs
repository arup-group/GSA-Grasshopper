using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

using Grasshopper.Kernel;

using GsaGH;
using GsaGH.Properties;

using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class ParameterClassNameTests {
    // This test it used to enforce naming convention required by Docs Generator
    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public void ClassNamesEqualComponentNamesTest(string className, IGH_Param param) {
      string paramName = param.Name;
      string classNameSplit = SplitCamelCase(className.Replace("Gsa", string.Empty));
      paramName = paramName
        .Replace(" 1D", " 1d")
        .Replace(" 2D", " 2d")
        .Replace(" 3D", " 3d")
        .Replace(" and", " And")
        .Replace(" from", " From")
        .Replace("GWA", "Gwa")
        .Replace("GSA", "Gsa");

      // Test component name is equal to class name
      if (paramName.Contains("Parameter")) {
        paramName = paramName.Replace("Parameter", "").Trim();
      }
      Assert.Equal(paramName + " Parameter", classNameSplit);
      // Test component icon is equal to class name
      ResourceManager rm = Resources.ResourceManager;
      string iconName = paramName.Replace(" ", string.Empty) + "Param";
      // Find icon with expected name in resources
      var iconExpected = (Bitmap)rm.GetObject(iconName);
      if (iconExpected.ToString() != "ParamProperty") {
        Assert.True(iconExpected != null, $"{iconName} not found in resources");
      }
      PropertyInfo pInfo = param.GetType().GetProperty("Icon",
        BindingFlags.NonPublic | BindingFlags.Instance);
      var icon = (Bitmap)pInfo.GetValue(param, null);
      Assert.Equal(iconExpected.RawFormat.Guid, icon.RawFormat.Guid);
    }

    private static string SplitCamelCase(string s) {
      // `CreateModel` => `Create Model`
      var r = new Regex(@"
        (?<=[A-Z])(?=[A-Z][a-z]) |
        (?<=[^A-Z])(?=[A-Z]) |
        (?<=[A-Za-z])(?=[^A-Za-z])",
        RegexOptions.IgnorePatternWhitespace);
      return r.Replace(s, " ").Replace("Bool 6", "Bool6");
    }

    public class TestDataGenerator : IEnumerable<object[]> {
      private readonly List<object[]> _data = GetAllParameters();

      public IEnumerator<object[]> GetEnumerator() {
        return _data.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
      }

      private static List<object[]> GetAllParameters() {
        var data = new List<object[]>();
        Type[] typelist = Assembly.GetAssembly(typeof(GsaGhInfo)).GetTypes();
        foreach (Type type in typelist) {
          if (type.BaseType == null || !type.BaseType.Name.StartsWith("GH_OasysPersistent")) {
            continue;
          }

          var param = (IGH_Param)Activator.CreateInstance(type, null);
          if (param.Exposure == GH_Exposure.hidden) {
            continue;
          }

          data.Add(new object[] {
            type.Name,
            param
          });
        }

        return data;
      }
    }
  }
}
