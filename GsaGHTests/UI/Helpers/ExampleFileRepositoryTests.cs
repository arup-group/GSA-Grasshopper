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

    [Fact]
    public void SetFiles_SetsOnlyOnce_GetAllFilesReturnsFirstSet() {
      var first = new List<FileEntry>() {
        new FileEntry() {
          Name = "first.gh",
          Url = "u1",
        },
        new FileEntry() {
          Name = "second.gh",
          Url = "u2",
        },
      };
      var second = new List<FileEntry>() {
        new FileEntry() {
          Name = "third.gh",
          Url = "u3",
        },
      };

      ExampleFileRepository.SetFiles(first);
      ExampleFileRepository.SetFiles(second); // should be ignored

      List<FileEntry> result = ExampleFileRepository.GetAllFiles();

      Assert.Equal(2, result.Count);
      Assert.Contains(result, f => f.Name == "first.gh");
      Assert.Contains(result, f => f.Name == "second.gh");
      Assert.DoesNotContain(result, f => f.Name == "third.gh");
    }

    [Fact]
    public void GetFileEntriesByKeywords_NullOrEmptyKeywords_ReturnsEmpty() {
      var files = new List<FileEntry>() {
        new FileEntry() {
          Name = "Footfall-example.gh",
          Url = "u1",
        },
      };

      ExampleFileRepository.SetFiles(files);

      IEnumerable<FileEntry> nullResult = ExampleFileRepository.GetFileEntriesByKeywords(null);
      IEnumerable<FileEntry> emptyResult = ExampleFileRepository.GetFileEntriesByKeywords(new List<string>());

      Assert.Empty(nullResult);
      Assert.Empty(emptyResult);
    }

    [Fact]
    public void GetFileEntriesByKeywords_FindsCaseInsensitivePartialMatches() {
      var files = new List<FileEntry>() {
        new FileEntry() {
          Name = "Footfall-example.gh",
          Url = "u1",
        },
        new FileEntry() {
          Name = "Model_foot.gh",
          Url = "u2",
        },
        new FileEntry() {
          Name = "Otherfile.gwa",
          Url = "u3",
        },
      };

      ExampleFileRepository.SetFiles(files);

      var matches = ExampleFileRepository.GetFileEntriesByKeywords(new List<string>() {
        "footfall",
      }).ToList();
      Assert.Single(matches);
      Assert.Equal("Footfall-example.gh", matches[0].Name);

      var multiMatches = ExampleFileRepository.GetFileEntriesByKeywords(new List<string>() {
        "foot",
      }).ToList();
      Assert.Equal(2, multiMatches.Count);
      Assert.Contains(multiMatches, f => f.Name == "Footfall-example.gh");
      Assert.Contains(multiMatches, f => f.Name == "Model_foot.gh");
    }
  }
}
