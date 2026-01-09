using System;
using System.Collections.Generic;
using System.Reflection;

using Grasshopper.Kernel.Types;

using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GhOasysGooTest {
    [Theory]
    // 0_Model
    [InlineData(typeof(GsaListGoo), typeof(GsaList))]
    [InlineData(typeof(GsaModelGoo), typeof(GsaModel), true)]
    //[InlineData(typeof(GsaGridLineGoo), typeof(GsaGridLine))]
    // 1_Properties
    [InlineData(typeof(GsaBool6Goo), typeof(GsaBool6))]
    [InlineData(typeof(GsaOffsetGoo), typeof(GsaOffset))]
    //[InlineData(typeof(GsaMaterialGoo), typeof(GsaConcreteMaterial))]
    //[InlineData(typeof(GsaMaterialGoo), typeof(GsaCustomMaterial))]
    //[InlineData(typeof(GsaMaterialGoo), typeof(GsaFabricMaterial))]
    //[InlineData(typeof(GsaMaterialGoo), typeof(GsaFrpMaterial))]
    //[InlineData(typeof(GsaMaterialGoo), typeof(GsaGlassMaterial))]
    //[InlineData(typeof(GsaMaterialGoo), typeof(GsaSteelMaterial))]
    [InlineData(typeof(GsaProperty2dGoo), typeof(GsaProperty2d))]
    [InlineData(typeof(GsaProperty3dGoo), typeof(GsaProperty3d))]
    [InlineData(typeof(GsaSectionGoo), typeof(GsaSection))]
    [InlineData(typeof(GsaSectionModifierGoo), typeof(GsaSectionModifier))]
    [InlineData(typeof(GsaProperty2dModifierGoo), typeof(GsaProperty2dModifier))]
    [InlineData(typeof(GsaSpringPropertyGoo), typeof(GsaSpringProperty))]
    // 2_Geometry
    [InlineData(typeof(GsaElement1dGoo), typeof(GsaElement1d))]
    [InlineData(typeof(GsaElement2dGoo), typeof(GsaElement2d))]
    [InlineData(typeof(GsaElement3dGoo), typeof(GsaElement3d))]
    [InlineData(typeof(GsaMember1dGoo), typeof(GsaMember1d))]
    [InlineData(typeof(GsaMember2dGoo), typeof(GsaMember2d))]
    [InlineData(typeof(GsaMember3dGoo), typeof(GsaMember3d))]
    [InlineData(typeof(GsaAssemblyGoo), typeof(GsaAssembly))]
    [InlineData(typeof(GsaNodeGoo), typeof(GsaNode))]
    [InlineData(typeof(GsaEffectiveLengthOptionsGoo), typeof(GsaEffectiveLengthOptions))]
    // 3_Loads
    [InlineData(typeof(GsaGridPlaneSurfaceGoo), typeof(GsaGridPlaneSurface))]
    [InlineData(typeof(GsaLoadGoo), typeof(GsaBeamLoad))]
    [InlineData(typeof(GsaLoadGoo), typeof(GsaBeamThermalLoad))]
    [InlineData(typeof(GsaLoadGoo), typeof(GsaFaceLoad))]
    [InlineData(typeof(GsaLoadGoo), typeof(GsaFaceThermalLoad))]
    [InlineData(typeof(GsaLoadGoo), typeof(GsaGravityLoad))]
    [InlineData(typeof(GsaLoadGoo), typeof(GsaGridAreaLoad))]
    [InlineData(typeof(GsaLoadGoo), typeof(GsaGridLineLoad))]
    [InlineData(typeof(GsaLoadGoo), typeof(GsaGridPointLoad))]
    [InlineData(typeof(GsaLoadGoo), typeof(GsaNodeLoad))]
    [InlineData(typeof(GsaLoadCaseGoo), typeof(GsaLoadCase))]

    // 4_Analysis
    [InlineData(typeof(GsaAnalysisCaseGoo), typeof(GsaAnalysisCase))]
    //[InlineData(typeof(GsaAnalysisTaskGoo), typeof(GsaAnalysisTask))]
    [InlineData(typeof(GsaCombinationCaseGoo), typeof(GsaCombinationCase))]

    // 5_Results
    //[InlineData(typeof(GsaResultGoo), typeof(GsaResult))]
    //[InlineData(typeof(LineResultGoo), typeof(Line))]
    //[InlineData(typeof(MeshResultGoo), typeof(Mesh))]
    //[InlineData(typeof(PointResultGoo), typeof(Point3d))]

    // 6_Display
    [InlineData(typeof(GsaDiagramGoo), typeof(GsaArrowheadDiagram))]
    [InlineData(typeof(GsaDiagramGoo), typeof(GsaLineDiagram))]
    [InlineData(typeof(GsaDiagramGoo), typeof(GsaVectorDiagram))]
    [InlineData(typeof(GsaAnnotationGoo), typeof(GsaAnnotation3d))]
    [InlineData(typeof(GsaAnnotationGoo), typeof(GsaAnnotationDot))]
    public void GenericGH_OasysGooTest(
      Type gooType, Type wrapType, bool excludeGuid = false) {
      object value = Activator.CreateInstance(wrapType, true);
      object[] parameters = {
        value,
      };

      object objectGoo = Activator.CreateInstance(gooType, parameters);
      gooType = objectGoo.GetType();

      IGH_Goo duplicate = ((IGH_Goo)objectGoo).Duplicate();
      List<string> excluded = excludeGuid ? new List<string>() { "Guid" } : null;
      Duplicates.AreEqual(objectGoo, duplicate, excluded);

      bool hasValue = false;
      bool hasToString = false;
      bool hasName = false;
      bool hasNickName = false;
      bool hasDescription = false;

      PropertyInfo[] gooPropertyInfo
        = gooType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
      foreach (PropertyInfo gooProperty in gooPropertyInfo) {
        if (gooProperty.Name == "Value") {
          object gooValue = gooProperty.GetValue(objectGoo, null);
          Duplicates.AreEqual(value, gooValue, excluded);

          MethodInfo methodInfo = gooValue.GetType().GetMethod("Clone");
          if (methodInfo != null) {
            object cloneValue = methodInfo.Invoke(gooValue, null);
            cloneValue = methodInfo.Invoke(gooValue, null);
            Assert.NotSame(gooValue, cloneValue);
            Duplicates.AreEqual(gooValue, cloneValue, excluded);
          }

          methodInfo = gooValue.GetType().GetMethod("Duplicate");
          if (methodInfo != null) {
            object duplicateValue = methodInfo.Invoke(gooValue, null); // .Duplicate();
            Duplicates.AreEqual(gooValue, duplicateValue);
          } else {
            ConstructorInfo duplicateConstructor = gooValue.GetType()
              .GetConstructor(new Type[] { gooValue.GetType() }); // new GsaObj(GsaObj other);
            object duplicateValue = duplicateConstructor.Invoke(new object[] { gooValue });
            Duplicates.AreEqual(gooValue, duplicateValue, new List<string>() { "Guid" });
          }

          hasValue = true;
        }

        if (gooProperty.Name == "TypeName") {
          string typeName = (string)gooProperty.GetValue(objectGoo, null);
          Assert.StartsWith("GSA " + typeName + " (", objectGoo.ToString());
          hasToString = true;
        }

        if (gooProperty.Name == "Name") {
          string name = (string)gooProperty.GetValue(objectGoo, null);
          Assert.True(name.Length > 3);
          hasName = true;
        }

        if (gooProperty.Name == "NickName") {
          string nickName = (string)gooProperty.GetValue(objectGoo, null);
          nickName = nickName.Replace(".", string.Empty);
          Assert.True(nickName.Length < 4);
          Assert.True(nickName.Length > 0);
          hasNickName = true;
        }

        if (gooProperty.Name != "Description") {
          continue;
        }

        string description = (string)gooProperty.GetValue(objectGoo, null);
        Assert.StartsWith("GSA", description);
        Assert.True(description.Length > 7);
        hasDescription = true;
      }

      Assert.True(hasValue);
      Assert.True(hasToString);
      Assert.True(hasName);
      Assert.True(hasNickName);
      Assert.True(hasDescription);
    }
  }
}
