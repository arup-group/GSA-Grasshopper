using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GsaGH.Helpers.Export
{
  internal class GsaIntDictionary<T>
  {
    private IDictionary<int, T> _dictionary;
    private int _maxKey = 0;

    internal int Count => _dictionary.Count;
    internal ReadOnlyDictionary<int, T> Dictionary
    {
      get
      {
        return new ReadOnlyDictionary<int, T>(_dictionary);
      }
    }

    internal GsaIntDictionary(ReadOnlyDictionary<int, T> dictionary)
    {
      _dictionary = dictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    internal int AddValue(T value)
    {
      _maxKey++;
      _dictionary[_maxKey] = value;
      return _maxKey;
    }

    internal void SetValue(int key, T value)
    {
      _dictionary[key] = value;
      if (_maxKey <= key)
        _maxKey = key;
    }
  }

  internal class GsaGuidDictionary<T>
  {
    private IDictionary<int, T> _dictionary;
    private IDictionary<Guid, int> _GuidDictionary;
    private int _maxKey = 0;

    internal ReadOnlyDictionary<int, T> Dictionary
    {
      get
      {
        return new ReadOnlyDictionary<int, T>(_dictionary);
      }
    }

    internal GsaGuidDictionary(ReadOnlyDictionary<int, T> dictionary)
    {
      _dictionary = dictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
      _GuidDictionary = dictionary.ToDictionary(kvp => Guid.NewGuid(), kvp => kvp.Key);
    }

    internal int AddValue(Guid guid, T value)
    {
      if (_GuidDictionary.ContainsKey(guid))
        return _GuidDictionary[guid];
      _maxKey++;
      _dictionary[_maxKey] = value;
      _GuidDictionary[guid] = _maxKey;
      return _maxKey;
    }

    internal void SetValue(int key, Guid guid, T value, bool overwrite = false)
    {
      if (overwrite && !_GuidDictionary.ContainsKey(guid))
      {
        _dictionary[key] = value;
        _GuidDictionary[guid] = key;
        if (_maxKey <= key)
          _maxKey = key;
      }
    }
  }
}
