using System;
using System.Reflection;
using GsaGHTests.Helpers;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using Xunit;

namespace GsaGHTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GH_OasysGooTest
  {
    [Theory]
    [InlineData(typeof(GsaAnalysisCaseGoo), typeof(GsaAnalysisCase))]
    [InlineData(typeof(GsaAnalysisTaskGoo), typeof(GsaAnalysisTask))]
    //[InlineData(typeof(GsaBool6Goo), typeof(GsaBool6))]
    public void GenericGH_OasysGooTest(Type gooType, Type wrapType)
    {
      // Create the actual API object
      Object value = Activator.CreateInstance(wrapType);
      Object[] parameters = { value };

      // Create GH_OasysGoo<API_Object> 
      Object objectGoo = Activator.CreateInstance(gooType, parameters);
      gooType = objectGoo.GetType();

      // Trigger the IGH_Goo Duplicate() method
      IGH_Goo duplicate = ((IGH_Goo)objectGoo).Duplicate();
      // check that they are equal
      Duplicates.AreEqual(duplicate, objectGoo);
      // check that they are not the same object (same pointer in memory)
      Assert.NotEqual(duplicate, objectGoo);

      bool hasValue = false;
      bool hasToString = false;
      bool hasName = false;
      bool hasNickName = false;
      bool hasDescription = false;

      // we can't cast directly to objectGoo.Value, so we do this instead
      PropertyInfo[] gooPropertyInfo = gooType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
      foreach (PropertyInfo gooProperty in gooPropertyInfo)
      {
        if (gooProperty.Name == "Value")
        {
          Object gooValue = gooProperty.GetValue(objectGoo, null);
          // here check that the value in the goo object is a duplicate of the original object
          Duplicates.AreEqual(value, gooValue);
          // check that they are not the same object (same pointer in memory)
          Assert.NotEqual(value, gooValue);

          hasValue = true;

          // this could be used to test the content of the wrapped object, if they share any members/fields
          //Type typeSource = gooValue.GetType();
          //PropertyInfo[] wrappedPropertyInfo = typeSource.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
          //foreach (PropertyInfo wrapProperty in wrappedPropertyInfo)
          //{

          //}
        }

        // check the grasshopper tostring method (when you hover over the input/output)
        if (gooProperty.Name == "TypeName")
        {
          string typeName = (string)gooProperty.GetValue(objectGoo, null);
          Assert.StartsWith("GSA " + typeName + " {", objectGoo.ToString());
          hasToString = true;
        }

        // check the name, input/output parameters
        if (gooProperty.Name == "Name")
        {
          string name = (string)gooProperty.GetValue(objectGoo, null);
          // require a name longer than 3 characters (stud being the shortest accepted)
          Assert.True(name.Length > 3);
          hasName = true;
        }
        // check the nickname, input/output parameters
        if (gooProperty.Name == "NickName")
        {
          string nickName = (string)gooProperty.GetValue(objectGoo, null);
          // require a nickname not longer than 3 characters excluding dots
          nickName = nickName.Replace(".", string.Empty);
          Assert.True(nickName.Length < 4);
          Assert.True(nickName.Length > 0);
          hasNickName = true;
        }
        if (gooProperty.Name == "Description")
        {
          string description = (string)gooProperty.GetValue(objectGoo, null);
          // require a description to start with "GSA"
          Assert.StartsWith("GSA ", description);
          Assert.True(description.Length > 10);
          hasDescription = true;
        }
      }

      Assert.True(hasValue);
      Assert.True(hasToString);
      Assert.True(hasName);
      Assert.True(hasNickName);
      Assert.True(hasDescription);
    }
  }
}
