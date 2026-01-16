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

namespace GsaGHTests.Components {
  [Collection("GrasshopperFixture collection")]
  public class ComponentClassNameTests {
    // This test it used to enforce naming convention required by Docs Generator
    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public void ClassNamesEqualComponentNamesTest(string className, GH_Component component) {
      string componentName = component.Name;
      string classNameSplit = SplitCamelCase(className);
      componentName = componentName
        .Replace(" 1D", " 1d")
        .Replace(" 2D", " 2d")
        .Replace(" 3D", " 3d")
        .Replace(" and", " And")
        .Replace(" from", " From")
        .Replace(" to", " To")
        .Replace("GWA", "Gwa")
        .Replace("GSA", "Gsa");

      // Test component name is equal to class name
      Assert.Equal(componentName, classNameSplit);

      // Test component icon is equal to class name
      ResourceManager rm = Resources.ResourceManager;
      // Find icon with expected name in resources
      var iconExpected = (Bitmap)rm.GetObject(className);
      Assert.True(iconExpected != null, $"{className} not found in resources");
      PropertyInfo pInfo = component.GetType().GetProperty("Icon",
        BindingFlags.NonPublic | BindingFlags.Instance);
      var icon = (Bitmap)pInfo.GetValue(component, null);
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
      private readonly List<object[]> _data = GetAllComponents();

      public IEnumerator<object[]> GetEnumerator() {
        return _data.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
      }

      private static List<object[]> GetAllComponents() {
        var data = new List<object[]>();
        Type[] typelist = Assembly.GetAssembly(typeof(GsaGhInfo)).GetTypes();
        foreach (Type type in typelist) {
          if (type.Namespace == null) {
            continue;
          }

          if (type.Namespace.StartsWith("GsaGH.Components")
            && !type.Name.Contains("OBSOLETE")
            && !type.Name.Contains("<")
            && (type.BaseType.Name != "Enum")
            && type.Attributes.HasFlag(TypeAttributes.Public)
            && !type.Attributes.HasFlag(TypeAttributes.Abstract)) {
            data.Add(new object[] {
              type.Name,
              (GH_Component)Activator.CreateInstance(type),
            });
          }
        }

        return data;
      }
    }
  }
}
