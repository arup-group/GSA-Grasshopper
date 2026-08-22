using System.Collections.Generic;
using System.Linq;

using GsaGH.UI;
using GsaGH.UI.Helpers;

using Xunit;

namespace GsaGHTests.UI.Helpers {
  public class ExampleFileRepositoryTests {
    public ExampleFileRepositoryTests() {
      ExampleFileRepository.Reset();
    }

    private static List<FileEntry> CreateFileEntries(params string[] names) {
      return names.Select(n => new FileEntry {
        Name = n,
        Url = $"http://example.com/{n}",
      }).ToList();
    }

    [Fact]
    public void SetFiles_SetsOnlyOnce_GetAllFilesReturnsFirstSet() {
      List<FileEntry> first = CreateFileEntries("first.gh", "second.gh");
      List<FileEntry> second = CreateFileEntries("third.gh");

      ExampleFileRepository.SetFiles(first);
      ExampleFileRepository.SetFiles(second); // should be ignored

      List<FileEntry> result = ExampleFileRepository.GetAllFiles();

      Assert.Equal(2, result.Count);
      Assert.Contains(result, f => f.Name == "first.gh");
      Assert.Contains(result, f => f.Name == "second.gh");
      Assert.DoesNotContain(result, f => f.Name == "third.gh");
    }

    [Fact]
    public void SetFiles_StoresEmptyList_WhenNullPassed() {
      ExampleFileRepository.SetFiles(null);

      Assert.Empty(ExampleFileRepository.GetAllFiles());
    }

    [Fact]
    public void SetFiles_StoresEmptyList_WhenEmptyListPassed() {
      ExampleFileRepository.SetFiles(new List<FileEntry>());

      Assert.Empty(ExampleFileRepository.GetAllFiles());
    }

    [Fact]
    public void GetAllFiles_ReturnsEmptyList_WhenNotInitialized() {
      Assert.Empty(ExampleFileRepository.GetAllFiles());
    }

    [Fact]
    public void GetAllFiles_ReturnsCopy_NotSameReference() {
      ExampleFileRepository.SetFiles(CreateFileEntries("a.gh"));

      List<FileEntry> result1 = ExampleFileRepository.GetAllFiles();
      List<FileEntry> result2 = ExampleFileRepository.GetAllFiles();

      Assert.NotSame(result1, result2);
    }

    [Fact]
    public void Reset_ClearsStoredFiles() {
      ExampleFileRepository.SetFiles(CreateFileEntries("a.gh"));

      ExampleFileRepository.Reset();

      Assert.Empty(ExampleFileRepository.GetAllFiles());
    }

    [Fact]
    public void Reset_AllowsSetFiles_ToBeCalledAgain() {
      List<FileEntry> first = CreateFileEntries("a.gh");
      List<FileEntry> second = CreateFileEntries("b.gh", "c.gh");
      ExampleFileRepository.SetFiles(first);

      ExampleFileRepository.Reset();
      ExampleFileRepository.SetFiles(second);

      List<FileEntry> result = ExampleFileRepository.GetAllFiles();
      Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetFileEntriesByKeywords_NullOrEmptyKeywords_ReturnsEmpty() {
      ExampleFileRepository.SetFiles(CreateFileEntries("Footfall-example.gh"));

      IEnumerable<FileEntry> nullResult = ExampleFileRepository.GetFileEntriesByKeywords(null);
      IEnumerable<FileEntry> emptyResult = ExampleFileRepository.GetFileEntriesByKeywords(new List<string>());

      Assert.Empty(nullResult);
      Assert.Empty(emptyResult);
    }

    [Fact]
    public void GetFileEntriesByKeywords_FindsCaseInsensitivePartialMatches() {
      ExampleFileRepository.SetFiles(CreateFileEntries("Footfall-example.gh", "Model_foot.gh", "Otherfile.gwa"));

      var matches = ExampleFileRepository.GetFileEntriesByKeywords(new List<string> {
        "footfall",
      }).ToList();
      Assert.Single(matches);
      Assert.Equal("Footfall-example.gh", matches[0].Name);

      var multiMatches = ExampleFileRepository.GetFileEntriesByKeywords(new List<string> {
        "foot",
      }).ToList();
      Assert.Equal(2, multiMatches.Count);
      Assert.Contains(multiMatches, f => f.Name == "Footfall-example.gh");
      Assert.Contains(multiMatches, f => f.Name == "Model_foot.gh");
    }
  }
}
