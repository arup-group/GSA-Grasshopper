using System;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Helpers {
  public static class DictionaryExtension {

    public static T TryGetKeyFrom<T>(this Dictionary<T, string> dictionary, string @value) {
      Dictionary<T, string>.ValueCollection dictionaryValues = dictionary.Values;
      Dictionary<T, string>.KeyCollection dictionaryKeys = dictionary.Keys;
      for (int i = 0; i < dictionaryValues.Count; i++) {
        if (dictionaryValues.ElementAt(i).Equals(@value)) {
          return dictionaryKeys.ElementAt(i);
        }
      }

      throw new Exception();
    }
  }
}
