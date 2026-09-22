using System;
using System.Collections.Generic;

using GsaGH.UI;
using GsaGH.UI.Helpers;

using HtmlAgilityPack;

using Xunit;

namespace GsaGHTests.UI.Helpers {
  public class HttpFileHelperTests {
    [Theory]
    [InlineData("file.txt")]
    [InlineData("my_file-123.csv")]
    [InlineData("ąęśćżź.txt")]
    public void GetSafeFileName_ValidFileName_ReturnsSameName(string fileName) {
      string result = HttpFileHelper.GetSafeFileName(fileName);
      Assert.Equal(fileName, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetSafeFileName_NullOrWhitespace_ThrowsArgumentException(string fileName) {
      Assert.Throws<ArgumentException>(() => HttpFileHelper.GetSafeFileName(fileName));
    }

    [Theory]
    [InlineData(@"C:\file.txt")]
    [InlineData(@"/usr/file.txt")]
    public void GetSafeFileName_RootedPath_ThrowsArgumentException(string fileName) {
      Assert.Throws<ArgumentException>(() => HttpFileHelper.GetSafeFileName(fileName));
    }

    [Theory]
    [InlineData("file?.txt")]
    [InlineData("file<.txt")]
    [InlineData("file|.txt")]
    public void GetSafeFileName_InvalidChars_ThrowsArgumentException(string fileName) {
      Assert.Throws<ArgumentException>(() => HttpFileHelper.GetSafeFileName(fileName));
    }

    [Fact]
    public void GetFileEntry_NullLink_ThrowsArgumentNullException() {
      Assert.Throws<ArgumentNullException>(()
        => HttpFileHelper.GetFileEntry("http://example.com", null, new List<string>()));
    }

    [Fact]
    public void GetFileEntry_ValidAbsoluteUrl_ReturnsFileEntry() {
      var node = HtmlNode.CreateNode("<a href=\"http://example.com/file.csv\">file.csv</a>");
      var allowed = new List<string> {
        ".csv",
      };

      FileEntry entry = HttpFileHelper.GetFileEntry("http://example.com", node, allowed);

      Assert.NotNull(entry);
      Assert.Equal("file.csv", entry.Name);
      Assert.Equal("http://example.com/file.csv", entry.Url);
    }

    [Fact]
    public void GetFileEntry_ValidRelativeUrl_ReturnsFileEntry() {
      var node = HtmlNode.CreateNode("<a href=\"/data/file.csv\">file.csv</a>");
      var allowed = new List<string> {
        ".csv",
      };

      FileEntry entry = HttpFileHelper.GetFileEntry("http://example.com", node, allowed);

      Assert.NotNull(entry);
      Assert.Equal("file.csv", entry.Name);
      Assert.Equal("http://example.com/data/file.csv", entry.Url);
    }

    [Fact]
    public void GetFileEntry_NotAllowedExtension_ReturnsNull() {
      var node = HtmlNode.CreateNode("<a href=\"/data/file.txt\">file.txt</a>");
      var allowed = new List<string> {
        ".csv",
      };

      FileEntry entry = HttpFileHelper.GetFileEntry("http://example.com", node, allowed);

      Assert.Null(entry);
    }

    [Fact]
    public void GetFileEntry_EmptyHref_ReturnsNull() {
      var node = HtmlNode.CreateNode("<a href=\"\">file.csv</a>");
      var allowed = new List<string> {
        ".csv",
      };

      FileEntry entry = HttpFileHelper.GetFileEntry("http://example.com", node, allowed);

      Assert.Null(entry);
    }

    [Fact]
    public void GetFileEntry_FileNameFromInnerText_WhenFileNameMissing() {
      var node = HtmlNode.CreateNode("<a href=\"/data/?download=.csv\">myfile.csv</a>");
      var allowed = new List<string> {
        ".csv",
      };

      FileEntry entry = HttpFileHelper.GetFileEntry("http://example.com", node, allowed);

      Assert.NotNull(entry);
      Assert.Equal("myfile.csv", entry.Name);
    }

    [Fact]
    public void GetFileEntry_UrlWithEncodedCharacters_DecodesFileName() {
      var node = HtmlNode.CreateNode("<a href=\"/data/file%20name.csv\">file name.csv</a>");
      var allowed = new List<string> {
        ".csv",
      };

      FileEntry entry = HttpFileHelper.GetFileEntry("http://example.com", node, allowed);

      Assert.NotNull(entry);
      Assert.Equal("file name.csv", entry.Name);
    }

    [Fact]
    public void GetFileEntry_EmptyAllowedExtensions_ReturnsNull() {
      var node = HtmlNode.CreateNode("<a href=\"/data/file.csv\">file.csv</a>");
      var allowed = new List<string>();

      FileEntry entry = HttpFileHelper.GetFileEntry("http://example.com", node, allowed);

      Assert.Null(entry);
    }

    [Fact]
    public void GetFileEntry_CaseInsensitiveExtension_ReturnsFileEntry() {
      var node = HtmlNode.CreateNode("<a href=\"/data/file.CSV\">file.CSV</a>");
      var allowed = new List<string> {
        ".csv",
      };

      FileEntry entry = HttpFileHelper.GetFileEntry("http://example.com", node, allowed);

      Assert.NotNull(entry);
      Assert.Equal("file.CSV", entry.Name);
    }

    [Fact]
    public void GetFileEntry_MultipleExtensionsAllowed_WithMatchingExtension_ReturnsFileEntry() {
      var node = HtmlNode.CreateNode("<a href=\"/data/file.gwa\">file.gwa</a>");
      var allowed = new List<string> {
        ".gh",
        ".gwa",
        ".gwb",
      };

      FileEntry entry = HttpFileHelper.GetFileEntry("http://example.com", node, allowed);

      Assert.NotNull(entry);
      Assert.Equal("file.gwa", entry.Name);
    }
  }
}
