using System.Collections.Generic;
using System.Reflection;

using GsaGH.Helpers.GH;

using Rhino;
using Rhino.DocObjects;

using Xunit;

namespace GsaGHTests.Helpers {
  [Collection("GrasshopperFixture collection")]
  public class TextWrapperTests {

    [Fact]
    public void WrapText_ShouldWrapTextCorrectly() {
      string inputText = "This is a test sentence that should be wrapped correctly.";
      int maxWidth = 100;
      int fontSize = 10;

      string result = TextWrapper.WrapText(inputText, maxWidth, fontSize);

      Assert.NotNull(result);
      Assert.Contains("\n", result);
      Assert.Equal("This is a test\nsentence that\nshould be\nwrapped\ncorrectly.", result);
    }

    [Fact]
    public void GetCachedTextWidth_ShouldCacheTextWidthCorrectly() {
      string inputText = "This is a test sentence that should be wrapped correctly.";
      int fontSize = 10;
      int maxWidth = 100;

      string text1 = TextWrapper.WrapText(inputText, maxWidth, fontSize);
      string text2 = TextWrapper.WrapText(inputText, maxWidth, fontSize);

      Assert.Single(GetPrivateFieldValue());
    }

    private Dictionary<(string, float), float> GetPrivateFieldValue() {
      FieldInfo fieldInfo
        = typeof(TextWrapper).GetField("textWidthCache", BindingFlags.NonPublic | BindingFlags.Static);
      return (Dictionary<(string, float), float>)fieldInfo.GetValue(null);
    }

    // Testowanie metody GetFontName
    [Fact]
    public void GetFontName_ShouldReturnFontNameFromRhino() {
      RhinoDoc rhinoDoc = CreateRhinoDocument("test");
      string fontName = TextWrapper.GetFontName(rhinoDoc);
      CloseRhinoDoc(rhinoDoc);
      Assert.Equal("Arial", fontName);
    }

    [Fact]
    public void GetFontName_ShouldReturnDefaultWhenRhinoDocIsNull() {
      string fontName = TextWrapper.GetFontName();
      Assert.Equal("Arial", fontName);
    }

    private static RhinoDoc CreateRhinoDocument(string modelTemplateFileName) {
      var rhinooDoc = RhinoDoc.Create(modelTemplateFileName);
      rhinooDoc.DimStyles.Current.Font = new Font("Test");

      return rhinooDoc;
    }

    private static void CloseRhinoDoc(RhinoDoc rhinoDoc) {
      rhinoDoc.Dispose();
    }
  }
}
