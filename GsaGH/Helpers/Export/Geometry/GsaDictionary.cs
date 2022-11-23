using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GsaGH.Helpers
{
  internal class GsaDictionary<T>
  {
    private IDictionary<int, T> _dictionary;
    private int _maxKey = 0;

    internal ReadOnlyDictionary<int, T> Dictionary
    {
      get
      {
        return new ReadOnlyDictionary<int, T>(_dictionary);
      }
    }

    internal GsaDictionary(ReadOnlyDictionary<int, T> dictionary)
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
}
