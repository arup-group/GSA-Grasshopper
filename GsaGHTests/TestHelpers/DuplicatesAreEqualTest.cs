using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Xunit;

namespace GsaGHTests.Helpers {
  public class Duplicates {

    public static bool AreEqual(object objA, object objB, List<string> excluded = null) {
      Assert.Equal(objA.ToString(), objB.ToString());

      Type typeA = objA.GetType();
      Type typeB = objB.GetType();

      PropertyInfo[] propertyInfoA
        = typeA.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      PropertyInfo[] propertyInfoB
        = typeB.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

      for (int i = 0; i < propertyInfoA.Length; i++) {

        PropertyInfo propertyA = propertyInfoA[i];
        PropertyInfo propertyB = propertyInfoB[i];

        if (IsExcluded(propertyA, excluded)) {
          continue;
        }

        if (!propertyA.CanWrite && !propertyB.CanWrite) {
          continue;
        } else if (!propertyA.CanWrite || !propertyB.CanWrite) {
          Assert.Equal(objA, objB);
        }

        object objPropertyValueA;
        object objPropertyValueB;
        Type propertyTypeA = propertyA.PropertyType;
        Type propertyTypeB = propertyB.PropertyType;

        try {

          objPropertyValueA = propertyA.GetValue(objA, null);
          objPropertyValueB = propertyB.GetValue(objB, null);

          if (propertyTypeA.IsInterface) {
            if (objPropertyValueA != null) {
              propertyTypeA = objPropertyValueA.GetType();
            }
          }

          if (propertyTypeB.IsInterface) {
            if (objPropertyValueB != null) {
              propertyTypeB = objPropertyValueB.GetType();
            }
          }

          if (typeof(IEnumerable).IsAssignableFrom(propertyTypeA)
            && !typeof(string).IsAssignableFrom(propertyTypeA)) {
            if (typeof(IEnumerable).IsAssignableFrom(propertyTypeB)
              && !typeof(string).IsAssignableFrom(propertyTypeB)) {
              if (objPropertyValueA == null || objPropertyValueB == null) {
                Assert.Equal(objPropertyValueA, objPropertyValueB);
              } else {
                IEnumerable<object> enumerableA = ((IEnumerable)objPropertyValueA).Cast<object>();
                IEnumerable<object> enumerableB = ((IEnumerable)objPropertyValueB).Cast<object>();

                Type enumrableTypeA = null;
                Type enumrableTypeB = null;
                if (enumerableA.GetType().GetGenericArguments().Length > 0) {
                  enumrableTypeA = enumerableA.GetType().GetGenericArguments()[0];
                }

                if (enumerableB.GetType().GetGenericArguments().Length > 0) {
                  enumrableTypeB = enumerableB.GetType().GetGenericArguments()[0];
                }

                Assert.Equal(enumrableTypeA, enumrableTypeB);

                if (enumrableTypeA.ToString() is "System.Object") {
                  if (enumerableA.Any()) {
                    enumrableTypeA = enumerableA.First().GetType();
                  } else {
                    continue;
                  }
                }

                if (enumrableTypeB.ToString() is "System.Object") {
                  if (enumerableB.Any()) {
                    enumrableTypeB = enumerableB.First().GetType();
                  } else {
                    continue;
                  }
                }

                Type genericListTypeA = typeof(List<>).MakeGenericType(enumrableTypeA);
                Type genericListTypeB = typeof(List<>).MakeGenericType(enumrableTypeB);
                Assert.Equal(genericListTypeA, genericListTypeB);

                IEnumerator<object> enumeratorB = enumerableB.GetEnumerator();

                using (IEnumerator<object> enumeratorA = enumerableA.GetEnumerator()) {
                  while (enumeratorA.MoveNext()) {
                    Assert.True(enumeratorB.MoveNext());
                    AreEqual(enumeratorA.Current, enumeratorB.Current, excluded);
                  }
                }
              }
            } else {
              Assert.Equal(objPropertyValueA, objPropertyValueB);
            }
          } else if (propertyTypeA.IsValueType || propertyTypeA.IsEnum
            || propertyTypeA.Equals(typeof(string))) {
            if (IsExcluded(propertyA, excluded)) {
              continue;
            }

            Assert.Equal(objPropertyValueA, objPropertyValueB);
          } else if (objPropertyValueA == null || objPropertyValueB == null) {
            Assert.Equal(objPropertyValueA, objPropertyValueB);
          } else {
            AreEqual(objPropertyValueA, objPropertyValueB, excluded);
          }
        } catch (TargetParameterCountException) { }
      }

      return true;
    }
    private static bool IsExcluded(PropertyInfo info, List<string> excluded) {
      if (excluded == null) {
        return false;
      }

      foreach (string property in excluded) {
        if (info.Name == property) {
          return true;
        }
      }
      return false;
    }
  }
}
