using System;
using System.IO;

using GsaGH.UI.Helpers;

using Xunit;

namespace GsaGHTests.UI.Helpers {
  [Collection("GrasshopperFixture collection")]
  public class GrasshopperFileOpenerTests : IDisposable {
    private readonly string _tempDir;
    private readonly string _tempFilePath;

    public GrasshopperFileOpenerTests() {
      _tempDir = Path.Combine(Path.GetTempPath(), $"GrasshopperFileOpenerTests_{Guid.NewGuid()}");
      Directory.CreateDirectory(_tempDir);
      _tempFilePath = Path.Combine(_tempDir, "test.gh");
    }

    public void Dispose() {
      try {
        if (Directory.Exists(_tempDir)) {
          Directory.Delete(_tempDir, true);
        }
      } catch {
        // Ignore cleanup errors
      }
    }

    [Fact]
    public void Open_ReturnsFalse_WhenFileDoesNotExist() {
      string nonExistentPath = Path.Combine(_tempDir, "nonexistent.gh");

      bool result = GrasshopperFileOpener.Open(nonExistentPath);

      Assert.False(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Open_ReturnsFalse_WhenPathIsNullOrEmpty(string path) {
      bool result = GrasshopperFileOpener.Open(path);

      Assert.False(result);
    }

    [Fact]
    public void Open_ReturnsFalse_WhenFileIsInvalid() {
      File.WriteAllText(_tempFilePath, "invalid grasshopper content");

      bool result = GrasshopperFileOpener.Open(_tempFilePath);

      Assert.False(result);
    }
  }
}
