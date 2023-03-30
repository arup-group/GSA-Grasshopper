using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GsaGH.Helpers.Export {

  internal class GsaGuidDictionary<T> {

    #region Properties + Fields
    internal int Count => _dictionary.Count;
    internal ReadOnlyDictionary<int, T> Dictionary => new ReadOnlyDictionary<int, T>(_dictionary);
    internal ReadOnlyDictionary<Guid, int> GuidDictionary => new ReadOnlyDictionary<Guid, int>(_guidDictionary);
    private readonly IDictionary<int, T> _dictionary;
    private readonly IDictionary<Guid, int> _guidDictionary;
    private int _firstEmptyKey = 1;
    #endregion Properties + Fields

    #region Internal Constructors
    internal GsaGuidDictionary(ReadOnlyDictionary<int, T> dictionary) {
      _dictionary = dictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
      _guidDictionary = dictionary.ToDictionary(kvp => Guid.NewGuid(), kvp => kvp.Key);
    }

    #endregion Internal Constructors

    #region Internal Methods
    internal int AddValue(Guid guid, T value) {
      if (_guidDictionary.ContainsKey(guid))
        return _guidDictionary[guid];
      while (_dictionary.ContainsKey(_firstEmptyKey))
        _firstEmptyKey++;
      _dictionary[_firstEmptyKey] = value;
      _guidDictionary[guid] = _firstEmptyKey;
      return _firstEmptyKey;
    }

    internal void SetValue(int key, Guid guid, T value) {
      _dictionary[key] = value;
      _guidDictionary[guid] = key;
    }

    #endregion Internal Methods
  }

  internal class GsaGuidIntListDictionary<T> {

    #region Properties + Fields
    internal int Count => _dictionary.Count;
    internal ReadOnlyDictionary<int, T> Dictionary => new ReadOnlyDictionary<int, T>(_dictionary);
    internal ReadOnlyDictionary<Guid, Collection<int>> GuidDictionary => new ReadOnlyDictionary<Guid, Collection<int>>(_guidDictionary);
    private readonly IDictionary<int, T> _dictionary;
    private readonly IDictionary<Guid, Collection<int>> _guidDictionary;
    private int _firstEmptyKey = 1;
    #endregion Properties + Fields

    #region Internal Constructors
    internal GsaGuidIntListDictionary(ReadOnlyDictionary<int, T> dictionary) {
      _dictionary = dictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
      _guidDictionary = dictionary.ToDictionary(kvp => Guid.NewGuid(), kvp => new Collection<int>() { kvp.Key });
    }

    #endregion Internal Constructors

    #region Internal Methods
    internal int AddValue(Guid guid, T value) {
      while (_dictionary.ContainsKey(_firstEmptyKey))
        _firstEmptyKey++;
      if (_guidDictionary.ContainsKey(guid)) {
        _dictionary[_firstEmptyKey] = value;
        _guidDictionary[guid].Add(_firstEmptyKey);
      }
      else {
        _dictionary[_firstEmptyKey] = value;
        _guidDictionary[guid] = new Collection<int>() { _firstEmptyKey };
      }

      return _firstEmptyKey;
    }

    internal void SetValue(int key, Guid guid, T value, bool overwrite) {
      _dictionary[key] = value;
      if (_guidDictionary.ContainsKey(guid) && !overwrite) {
        if (!_guidDictionary[guid].Contains(key))
          _guidDictionary[guid].Add(key);
      }
      else
        _guidDictionary[guid] = new Collection<int>() { key };
    }

    #endregion Internal Methods
  }

  internal class GsaIntDictionary<T> {

    #region Properties + Fields
    internal int Count => _dictionary.Count;
    internal ReadOnlyDictionary<int, T> Dictionary => new ReadOnlyDictionary<int, T>(_dictionary);
    private readonly IDictionary<int, T> _dictionary;
    private int _firstEmptyKey = 1;
    #endregion Properties + Fields

    #region Internal Constructors
    internal GsaIntDictionary(ReadOnlyDictionary<int, T> dictionary) => _dictionary = dictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

    #endregion Internal Constructors

    #region Internal Methods
    internal int AddValue(T value) {
      while (_dictionary.ContainsKey(_firstEmptyKey))
        _firstEmptyKey++;
      _dictionary[_firstEmptyKey] = value;
      return _firstEmptyKey;
    }

    internal void SetValue(int key, T value) => _dictionary[key] = value;

    #endregion Internal Methods
  }
}
