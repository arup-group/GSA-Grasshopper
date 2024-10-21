using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using GsaGH.Helpers;

using Xunit;

namespace GsaGHTests.Helpers.Export {
  public class GsaDictionaryTests {

    [Fact]
    public void GsaGuidDictionaryFromEmptyAddTest() {
      var existingDict = new Dictionary<int, string>();
      var dictionary
        = new GsaGuidDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      var guid = Guid.NewGuid();
      int expectedId = dictionary.AddValue(guid, "first");

      Assert.Equal(1, expectedId);
      Assert.Equal(1, dictionary.Count);
      Assert.Equal("first", dictionary.ReadOnlyDictionary[1]);

      Assert.Equal(expectedId, dictionary.AddValue(guid, "second"));
      Assert.Equal("first", dictionary.ReadOnlyDictionary[expectedId]);
    }

    [Fact]
    public void GsaGuidDictionaryWithExistingAddTest() {
      var existingDict = new Dictionary<int, string> {
        {
          1, "first"
        }, {
          5, "second"
        },
      };
      var dictionary
        = new GsaGuidDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      var guid = Guid.NewGuid();
      int expectedId = dictionary.AddValue(guid, "myFirst");

      Assert.Equal(6, expectedId);
      Assert.Equal(3, dictionary.Count);
      Assert.Equal("myFirst", dictionary.ReadOnlyDictionary[expectedId]);

      Assert.Equal(expectedId, dictionary.AddValue(guid, "second"));
      Assert.Equal("myFirst", dictionary.ReadOnlyDictionary[expectedId]);
    }

    [Fact]
    public void GsaGuidDictionaryWithExistingSetTest() {
      var existingDict = new Dictionary<int, string> {
        {
          1, "first"
        }, {
          5, "second"
        },
      };
      var dictionary
        = new GsaGuidDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      var guid = Guid.NewGuid();
      dictionary.SetValue(3, guid, "myFirst");

      Assert.Equal(3, dictionary.Count);
      Assert.Equal("myFirst", dictionary.ReadOnlyDictionary[3]);
    }

    [Fact]
    public void GsaGuidIntListDictionaryFromEmptyAddTest() {
      var existingDict = new Dictionary<int, string>();
      var dictionary
        = new GsaGuidIntListDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      var guid = Guid.NewGuid();
      var expectedIDs = new List<int> {
        dictionary.AddValue(guid, "first"),
        dictionary.AddValue(guid, "second"),
      };

      Assert.Equal(1, expectedIDs[0]);
      Assert.Equal(2, expectedIDs[1]);
      Assert.Equal(2, dictionary.Count);
      Assert.Equal("first", dictionary.ReadOnlyDictionary[1]);
      Assert.Equal("second", dictionary.ReadOnlyDictionary[2]);
    }

    [Fact]
    public void GsaGuidIntListDictionaryWithExistingAddTest() {
      var existingDict = new Dictionary<int, string> {
        {
          1, "first"
        }, {
          5, "second"
        },
      };
      var dictionary
        = new GsaGuidIntListDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      var guid = Guid.NewGuid();
      var expectedIDs = new List<int> {
        dictionary.AddValue(guid, "myFirst"),
        dictionary.AddValue(guid, "mySecond"),
      };

      Assert.Equal(2, expectedIDs[0]);
      Assert.Equal(3, expectedIDs[1]);
      Assert.Equal(4, dictionary.Count);
      Assert.Equal("myFirst", dictionary.ReadOnlyDictionary[2]);
      Assert.Equal("mySecond", dictionary.ReadOnlyDictionary[3]);
    }

    [Fact]
    public void GsaGuidIntListDictionaryWithExistingSetOverwriteTest() {
      var existingDict = new Dictionary<int, string> {
        {
          1, "first"
        }, {
          5, "second"
        },
      };
      var dictionary
        = new GsaGuidIntListDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      var guid = Guid.NewGuid();
      dictionary.SetValue(3, guid, "myFirst", true);

      Assert.Equal(3, dictionary.Count);
      Assert.Equal("myFirst", dictionary.ReadOnlyDictionary[3]);
      Assert.Equal(3, dictionary.ReadOnlyDictionary.Count);
      Assert.Single(dictionary.GuidDictionary[guid]);

      dictionary.SetValue(3, guid, "mySecond", true);
      Assert.Equal("mySecond", dictionary.ReadOnlyDictionary[3]);
      Assert.Single(dictionary.GuidDictionary[guid]);
    }

    [Fact]
    public void GsaGuidIntListDictionaryWithExistingSetTest() {
      var existingDict = new Dictionary<int, string> {
        {
          1, "first"
        }, {
          5, "second"
        },
      };
      var dictionary
        = new GsaGuidIntListDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      var guid = Guid.NewGuid();
      dictionary.SetValue(5, guid, "myFirst", true);
      dictionary.AddValue(guid, "mySecond");

      Assert.Equal(3, dictionary.Count);
      Assert.Equal("myFirst", dictionary.ReadOnlyDictionary[5]);
      Assert.Equal(2, dictionary.GuidDictionary[guid].Count);

      Assert.Equal("mySecond", dictionary.ReadOnlyDictionary[2]);
    }

    [Fact]
    public void GsaGuidIntListDictionaryWithExistingTestMax() {
      var existingDict = new Dictionary<int, string> {
        {
          1, "first"
        },
      };
      var dictionary
        = new GsaGuidIntListDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      var guid2 = Guid.NewGuid();
      dictionary.SetValue(2, guid2, "second", false);

      var guid3 = Guid.NewGuid();
      dictionary.SetValue(2, guid3, "mySecond", true);
      var guid4 = Guid.NewGuid();
      Assert.Equal(3, dictionary.AddValue(guid4, "myThird"));

      var guid5 = Guid.NewGuid();
      dictionary.SetValue(4, guid5, "myNewHighest", true);
      var guid6 = Guid.NewGuid();
      Assert.Equal(5, dictionary.AddValue(guid6, "myFifth"));
    }

    [Fact]
    public void GsaGuidtDictionaryWithExistingSetOverwriteTest() {
      var existingDict = new Dictionary<int, string> {
        {
          1, "first"
        }, {
          5, "second"
        },
      };
      var dictionary
        = new GsaGuidDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      var guid = Guid.NewGuid();
      dictionary.SetValue(5, guid, "myFirst");

      Assert.Equal(2, dictionary.Count);
      Assert.Equal("myFirst", dictionary.ReadOnlyDictionary[5]);

      dictionary.SetValue(5, guid, "mySecond");
      Assert.Equal("mySecond", dictionary.ReadOnlyDictionary[5]);
    }

    [Fact]
    public void GsaGuidtDictionaryWithExistingTestMax() {
      var existingDict = new Dictionary<int, string> {
        {
          1, "first"
        },
      };
      var dictionary
        = new GsaGuidDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      var guid2 = Guid.NewGuid();
      dictionary.SetValue(2, guid2, "second");

      var guid3 = Guid.NewGuid();
      dictionary.SetValue(2, guid3, "mySecond");
      var guid4 = Guid.NewGuid();
      Assert.Equal(3, dictionary.AddValue(guid4, "myThird"));

      var guid5 = Guid.NewGuid();
      dictionary.SetValue(4, guid5, "myNewHighest");
      var guid6 = Guid.NewGuid();
      Assert.Equal(5, dictionary.AddValue(guid6, "myFifth"));
    }

    [Fact]
    public void GsaIntDictionaryFromEmptyAddTest() {
      var existingDict = new Dictionary<int, string>();
      var dictionary
        = new GsaIntDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      int expectedId = dictionary.AddValue("first");

      Assert.Equal(1, expectedId);
      Assert.Equal(1, dictionary.Count);
      Assert.Equal("first", dictionary.ReadOnlyDictionary[1]);
    }

    [Fact]
    public void GsaIntDictionaryWithExistingAddTest() {
      var existingDict = new Dictionary<int, string> {
        {
          1, "first"
        }, {
          5, "second"
        },
      };
      var dictionary
        = new GsaIntDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      int expectedId = dictionary.AddValue("myFirst");

      Assert.Equal(2, expectedId);
      Assert.Equal(3, dictionary.Count);
      Assert.Equal("myFirst", dictionary.ReadOnlyDictionary[2]);
    }

    [Fact]
    public void GsaIntDictionaryWithExistingSetOverwriteTest() {
      var existingDict = new Dictionary<int, string> {
        {
          1, "first"
        }, {
          5, "second"
        },
      };
      var dictionary
        = new GsaIntDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      dictionary.SetValue(5, "myFirst");

      Assert.Equal(2, dictionary.Count);
      Assert.Equal("myFirst", dictionary.ReadOnlyDictionary[5]);
    }

    [Fact]
    public void GsaIntDictionaryWithExistingSetTest() {
      var existingDict = new Dictionary<int, string> {
        {
          1, "first"
        }, {
          5, "second"
        },
      };
      var dictionary
        = new GsaIntDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      dictionary.SetValue(3, "myFirst");

      Assert.Equal(3, dictionary.Count);
      Assert.Equal("myFirst", dictionary.ReadOnlyDictionary[3]);
    }
  }
}
