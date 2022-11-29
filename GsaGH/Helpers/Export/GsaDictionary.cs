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
    internal ReadOnlyDictionary<int, T> Dictionary => new ReadOnlyDictionary<int, T>(_dictionary);

    internal GsaIntDictionary(ReadOnlyDictionary<int, T> dictionary)
    {
      _dictionary = dictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
      if (_dictionary.Count > 0)
        _maxKey = _dictionary.Keys.Max();
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
    private IDictionary<Guid, int> _guidDictionary;
    private int _maxKey = 0;

    internal int Count => _dictionary.Count;
    internal ReadOnlyDictionary<int, T> Dictionary => new ReadOnlyDictionary<int, T>(_dictionary);
    internal ReadOnlyDictionary<Guid, int> GuidDictionary => new ReadOnlyDictionary<Guid, int>(_guidDictionary);

    internal GsaGuidDictionary(ReadOnlyDictionary<int, T> dictionary)
    {
      _dictionary = dictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
      _guidDictionary = dictionary.ToDictionary(kvp => Guid.NewGuid(), kvp => kvp.Key);
      if (_dictionary.Count > 0)
        _maxKey = _dictionary.Keys.Max();
    }

    internal int AddValue(Guid guid, T value)
    {
      if (_guidDictionary.ContainsKey(guid))
        return _guidDictionary[guid];
      _maxKey++;
      _dictionary[_maxKey] = value;
      _guidDictionary[guid] = _maxKey;
      return _maxKey;
    }

    internal void SetValue(int key, Guid guid, T value)
    {
      _dictionary[key] = value;
      _guidDictionary[guid] = key;
      if (_maxKey <= key)
        _maxKey = key;
    }
  }

  internal class GsaGuidIntListDictionary<T>
  {
    private IDictionary<int, T> _dictionary;
    private IDictionary<Guid, Collection<int>> _guidDictionary;
    private int _maxKey = 0;

    internal int Count => _dictionary.Count;
    internal ReadOnlyDictionary<int, T> Dictionary => new ReadOnlyDictionary<int, T>(_dictionary);
    internal ReadOnlyDictionary<Guid, Collection<int>> GuidDictionary => new ReadOnlyDictionary<Guid, Collection<int>>(_guidDictionary);

    internal GsaGuidIntListDictionary(ReadOnlyDictionary<int, T> dictionary)
    {
      _dictionary = dictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
      _guidDictionary = dictionary.ToDictionary(kvp => Guid.NewGuid(), kvp => new Collection<int>() { kvp.Key });
      if (_dictionary.Count > 0)
        _maxKey = _dictionary.Keys.Max();
    }

    internal int AddValue(Guid guid, T value)
    {
      _maxKey++;
      if (_guidDictionary.ContainsKey(guid))
      {
        _dictionary[_maxKey] = value;
        _guidDictionary[guid].Add(_maxKey);
      }
      else
      {
        _dictionary[_maxKey] = value;
        _guidDictionary[guid] = new Collection<int>() { _maxKey };
      }
      return _maxKey;
    }

    internal void SetValue(int key, Guid guid, T value, bool overwrite)
    {
      _dictionary[key] = value;
      if (_maxKey <= key)
        _maxKey = key;
      if (_guidDictionary.ContainsKey(guid) && !overwrite)
      {
        if (!_guidDictionary[guid].Contains(key))
          _guidDictionary[guid].Add(key);
      }
      else
        _guidDictionary[guid] = new Collection<int>() { key };
    }
  }
}
