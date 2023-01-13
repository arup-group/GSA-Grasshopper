using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaGH.Helpers.Export;
using Xunit;

namespace GsaGHTests.Helpers.Export
{
  public class GsaDictionaryTests
  {
    #region IntDictionary
    [Fact]
    public void GsaIntDictionaryFromEmptyAddTest()
    {
      Dictionary<int, string> existingDict = new Dictionary<int, string>();
      GsaIntDictionary<string> dictionary = new GsaIntDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      int expectedID = dictionary.AddValue("first");

      Assert.Equal(1, expectedID);
      Assert.Equal(1, dictionary.Count);
      Assert.Equal("first", dictionary.Dictionary[1]);
    }

    [Fact]
    public void GsaIntDictionaryWithExistingAddTest()
    {
      Dictionary<int, string> existingDict = new Dictionary<int, string>
      {
        { 1, "first" },
        { 5, "second" }
      };
      GsaIntDictionary<string> dictionary = new GsaIntDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      int expectedID = dictionary.AddValue("myFirst");

      Assert.Equal(2, expectedID);
      Assert.Equal(3, dictionary.Count);
      Assert.Equal("myFirst", dictionary.Dictionary[2]);
    }

    [Fact]
    public void GsaIntDictionaryWithExistingSetTest()
    {
      Dictionary<int, string> existingDict = new Dictionary<int, string>
      {
        { 1, "first" },
        { 5, "second" }
      };
      GsaIntDictionary<string> dictionary = new GsaIntDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      dictionary.SetValue(3, "myFirst");

      Assert.Equal(3, dictionary.Count);
      Assert.Equal("myFirst", dictionary.Dictionary[3]);
    }

    [Fact]
    public void GsaIntDictionaryWithExistingSetOverwriteTest()
    {
      Dictionary<int, string> existingDict = new Dictionary<int, string>
      {
        { 1, "first" },
        { 5, "second" }
      };
      GsaIntDictionary<string> dictionary = new GsaIntDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      dictionary.SetValue(5, "myFirst");

      Assert.Equal(2, dictionary.Count);
      Assert.Equal("myFirst", dictionary.Dictionary[5]);
    }
    #endregion

    #region Guid dictionary
    [Fact]
    public void GsaGuidDictionaryFromEmptyAddTest()
    {
      Dictionary<int, string> existingDict = new Dictionary<int, string>();
      GsaGuidDictionary<string> dictionary = new GsaGuidDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      Guid guid = Guid.NewGuid();
      int expectedID = dictionary.AddValue(guid, "first");

      Assert.Equal(1, expectedID);
      Assert.Equal(1, dictionary.Count);
      Assert.Equal("first", dictionary.Dictionary[1]);

      // try to add with same guid but different object
      Assert.Equal(expectedID, dictionary.AddValue(guid, "second"));
      Assert.Equal("first", dictionary.Dictionary[expectedID]);
    }

    [Fact]
    public void GsaGuidDictionaryWithExistingAddTest()
    {
      Dictionary<int, string> existingDict = new Dictionary<int, string>
      {
        { 1, "first" },
        { 5, "second" }
      };
      GsaGuidDictionary<string> dictionary = new GsaGuidDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      Guid guid = Guid.NewGuid();
      int expectedID = dictionary.AddValue(guid, "myFirst");

      Assert.Equal(2, expectedID);
      Assert.Equal(3, dictionary.Count);
      Assert.Equal("myFirst", dictionary.Dictionary[2]);

      // try to add with same guid but different object
      Assert.Equal(expectedID, dictionary.AddValue(guid, "second"));
      Assert.Equal("myFirst", dictionary.Dictionary[expectedID]);
    }

    [Fact]
    public void GsaGuidDictionaryWithExistingSetTest()
    {
      Dictionary<int, string> existingDict = new Dictionary<int, string>
      {
        { 1, "first" },
        { 5, "second" }
      };
      GsaGuidDictionary<string> dictionary = new GsaGuidDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      Guid guid = Guid.NewGuid();
      dictionary.SetValue(3, guid, "myFirst");

      Assert.Equal(3, dictionary.Count);
      Assert.Equal("myFirst", dictionary.Dictionary[3]);
    }

    [Fact]
    public void GsaGuidtDictionaryWithExistingSetOverwriteTest()
    {
      Dictionary<int, string> existingDict = new Dictionary<int, string>
      {
        { 1, "first" },
        { 5, "second" }
      };
      GsaGuidDictionary<string> dictionary = new GsaGuidDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      Guid guid = Guid.NewGuid();
      dictionary.SetValue(5, guid, "myFirst");

      Assert.Equal(2, dictionary.Count);
      Assert.Equal("myFirst", dictionary.Dictionary[5]);

      // try set without overwrite bool
      dictionary.SetValue(5, guid, "mySecond");
      Assert.Equal("mySecond", dictionary.Dictionary[5]);
    }

    [Fact]
    public void GsaGuidtDictionaryWithExistingTestMax()
    {
      Dictionary<int, string> existingDict = new Dictionary<int, string>
      {
        { 1, "first" },
      };
      GsaGuidDictionary<string> dictionary = new GsaGuidDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      // creating the dictionary with one value should add next at key = 2
      Guid guid2 = Guid.NewGuid();
      dictionary.SetValue(2, guid2, "second");

      // overwriting existing item should not bump up max
      Guid guid3 = Guid.NewGuid();
      dictionary.SetValue(2, guid3, "mySecond");
      Guid guid4 = Guid.NewGuid();
      Assert.Equal(3, dictionary.AddValue(guid4, "myThird"));

      // setting a new highest key should bump up max
      Guid guid5 = Guid.NewGuid();
      dictionary.SetValue(4, guid5, "myNewHighest");
      Guid guid6 = Guid.NewGuid();
      Assert.Equal(5, dictionary.AddValue(guid6, "myFifth"));
    }
    #endregion

    #region Guid list<int> dictionary
    [Fact]
    public void GsaGuidIntListDictionaryFromEmptyAddTest()
    {
      Dictionary<int, string> existingDict = new Dictionary<int, string>();
      GsaGuidIntListDictionary<string> dictionary = new GsaGuidIntListDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      Guid guid = Guid.NewGuid();
      List<int> expectedIDs = new List<int>();
      expectedIDs.Add(dictionary.AddValue(guid, "first"));
      expectedIDs.Add(dictionary.AddValue(guid, "second"));

      Assert.Equal(1, expectedIDs[0]);
      Assert.Equal(2, expectedIDs[1]);
      Assert.Equal(2, dictionary.Count);
      Assert.Equal("first", dictionary.Dictionary[1]);
      Assert.Equal("second", dictionary.Dictionary[2]);
    }

    [Fact]
    public void GsaGuidIntListDictionaryWithExistingAddTest()
    {
      Dictionary<int, string> existingDict = new Dictionary<int, string>
      {
        { 1, "first" },
        { 5, "second" }
      };
      GsaGuidIntListDictionary<string> dictionary = new GsaGuidIntListDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      Guid guid = Guid.NewGuid();
      List<int> expectedIDs = new List<int>();
      expectedIDs.Add(dictionary.AddValue(guid, "myFirst"));
      expectedIDs.Add(dictionary.AddValue(guid, "mySecond"));

      Assert.Equal(2, expectedIDs[0]);
      Assert.Equal(3, expectedIDs[1]);
      Assert.Equal(4, dictionary.Count);
      Assert.Equal("myFirst", dictionary.Dictionary[2]);
      Assert.Equal("mySecond", dictionary.Dictionary[3]);
    }

    [Fact]
    public void GsaGuidIntListDictionaryWithExistingSetTest()
    {
      Dictionary<int, string> existingDict = new Dictionary<int, string>
      {
        { 1, "first" },
        { 5, "second" }
      };
      GsaGuidIntListDictionary<string> dictionary = new GsaGuidIntListDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      Guid guid = Guid.NewGuid();
      dictionary.SetValue(5, guid, "myFirst", true);
      dictionary.AddValue(guid, "mySecond");

      Assert.Equal(3, dictionary.Count);
      Assert.Equal("myFirst", dictionary.Dictionary[5]);
      Assert.Equal(2, dictionary.GuidDictionary[guid].Count);

      Assert.Equal("mySecond", dictionary.Dictionary[2]);
    }

    [Fact]
    public void GsaGuidIntListDictionaryWithExistingSetOverwriteTest()
    {
      Dictionary<int, string> existingDict = new Dictionary<int, string>
      {
        { 1, "first" },
        { 5, "second" }
      };
      GsaGuidIntListDictionary<string> dictionary = new GsaGuidIntListDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      Guid guid = Guid.NewGuid();
      dictionary.SetValue(3, guid, "myFirst", true);

      Assert.Equal(3, dictionary.Count);
      Assert.Equal("myFirst", dictionary.Dictionary[3]);
      Assert.Equal(3, dictionary.Dictionary.Count);
      Assert.Single(dictionary.GuidDictionary[guid]);

      dictionary.SetValue(3, guid, "mySecond", true);
      Assert.Equal("mySecond", dictionary.Dictionary[3]);
      Assert.Single(dictionary.GuidDictionary[guid]);
    }

    [Fact]
    public void GsaGuidIntListDictionaryWithExistingTestMax()
    {
      Dictionary<int, string> existingDict = new Dictionary<int, string>
      {
        { 1, "first" },
      };
      GsaGuidIntListDictionary<string> dictionary = new GsaGuidIntListDictionary<string>(new ReadOnlyDictionary<int, string>(existingDict));

      // creating the dictionary with one value should add next at key = 2
      Guid guid2 = Guid.NewGuid();
      dictionary.SetValue(2, guid2, "second", false);

      // overwriting existing item should not bump up max
      Guid guid3 = Guid.NewGuid();
      dictionary.SetValue(2, guid3, "mySecond", true);
      Guid guid4 = Guid.NewGuid();
      Assert.Equal(3, dictionary.AddValue(guid4, "myThird"));

      // setting a new highest key should bump up max
      Guid guid5 = Guid.NewGuid();
      dictionary.SetValue(4, guid5, "myNewHighest", true);
      Guid guid6 = Guid.NewGuid();
      Assert.Equal(5, dictionary.AddValue(guid6, "myFifth"));
    }
    #endregion
  }
}
