using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Grasshopper.Kernel.Types;
using Xunit;
using OasysGH.Components;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rhino.Geometry;
using GsaGHTests.Model;
using GsaGHTests.Components.Properties;
using GsaAPI;
using OasysUnits.Units;
using OasysUnits;
using System.Linq;

namespace GsaGHTests.Helpers.Export
{
  public partial class AssembleModelTests
  {
    public void TestAnalysisMaterial(GsaMaterial expected, int expectedId, GsaModel actualModel)
    {
      ReadOnlyDictionary<int, GsaAPI.AnalysisMaterial> apiMaterials = actualModel.Model.AnalysisMaterials();
      Assert.True(apiMaterials.ContainsKey(expectedId), "Analysis material with id " + expectedId + " is not present in model");

      AnalysisMaterial api = apiMaterials[expectedId];
      Assert.Equal(expected.AnalysisMaterial.CoefficientOfThermalExpansion, api.CoefficientOfThermalExpansion);
      Assert.Equal(expected.AnalysisMaterial.Density, api.Density);
      Assert.Equal(expected.AnalysisMaterial.ElasticModulus, api.ElasticModulus);
      Assert.Equal(expected.AnalysisMaterial.PoissonsRatio, api.PoissonsRatio);
    }

    public void TestSection(GsaSection expected, int expectedId, GsaModel actualModel)
    {
      ReadOnlyDictionary<int, GsaAPI.Section> apiSections = actualModel.Model.Sections();
      Assert.True(apiSections.ContainsKey(expectedId), "Section with id " + expectedId + " is not present in model");

      Section api = apiSections[expectedId];
      Assert.Equal(expected.API_Section.Profile, api.Profile);

      if (api.MaterialAnalysisProperty > 0)
        TestAnalysisMaterial(expected.Material, api.MaterialAnalysisProperty, actualModel);
      else
        Assert.Null(expected.Material.AnalysisMaterial);
    }

    public void TestProp2d(GsaProp2d expected, int expectedId, GsaModel actualModel)
    {
      ReadOnlyDictionary<int, GsaAPI.Prop2D> apiProp2ds = actualModel.Model.Prop2Ds();
      Assert.True(apiProp2ds.ContainsKey(expectedId), "Prop2d with id " + expectedId + " is not present in model");

      Prop2D api = apiProp2ds[expectedId];
      Assert.Equal(expected.API_Prop2d.Description, api.Description);

      if (api.MaterialAnalysisProperty > 0)
        TestAnalysisMaterial(expected.Material, api.MaterialAnalysisProperty, actualModel);
      else
        Assert.Null(expected.Material.AnalysisMaterial);
    }

    public void TestProp3d(GsaProp3d expected, int expectedId, GsaModel actualModel)
    {
      ReadOnlyDictionary<int, GsaAPI.Prop3D> apiProp3ds = actualModel.Model.Prop3Ds();
      Assert.True(apiProp3ds.ContainsKey(expectedId), "Prop3d with id " + expectedId + " is not present in model");

      Prop3D api = apiProp3ds[expectedId];

      if (api.MaterialAnalysisProperty > 0)
        TestAnalysisMaterial(expected.Material, api.MaterialAnalysisProperty, actualModel);
      else
        Assert.Null(expected.Material.AnalysisMaterial);
    }

    public void TestElement1d(GsaElement1d expected, LengthUnit unit, int expectedId, GsaModel actualModel)
    {
      ReadOnlyDictionary<int, GsaAPI.Element> apiElements = actualModel.Model.Elements();
      Assert.True(apiElements.ContainsKey(expectedId), "Element with id " + expectedId + " is not present in model");

      ReadOnlyDictionary<int, GsaAPI.Node> apiNodes = actualModel.Model.Nodes();
      Element api = apiElements[expectedId];
      Node apiStart = apiNodes[api.Topology[0]];
      Node apiEnd = apiNodes[api.Topology[1]];
      Assert.Equal(apiStart.Position.X, new Length(expected.Line.PointAtStart.X, unit).Meters);
      Assert.Equal(apiStart.Position.Y, new Length(expected.Line.PointAtStart.Y, unit).Meters);
      Assert.Equal(apiStart.Position.Z, new Length(expected.Line.PointAtStart.Z, unit).Meters);
      Assert.Equal(apiEnd.Position.X, new Length(expected.Line.PointAtEnd.X, unit).Meters);
      Assert.Equal(apiEnd.Position.Y, new Length(expected.Line.PointAtEnd.Y, unit).Meters);
      Assert.Equal(apiEnd.Position.Z, new Length(expected.Line.PointAtEnd.Z, unit).Meters);

      TestSection(expected.Section, api.Property, actualModel);
    }

    public void TestElement2d(GsaElement2d expected, LengthUnit unit, List<int> expectedIds, GsaModel actualModel)
    {
      ReadOnlyDictionary<int, GsaAPI.Element> apiElements = actualModel.Model.Elements();
      ReadOnlyDictionary<int, GsaAPI.Node> apiNodes = actualModel.Model.Nodes();
      int j = 0;

      foreach (int id in expectedIds)
      {
        Assert.True(apiElements.ContainsKey(id), "Element with id " + id + " is not present in model");

        Element api = apiElements[id];

        List<int> topoInts = expected.TopoInt[j++];
        int i = 0;
        foreach (int topo in api.Topology)
        {
          Node apiNode = apiNodes[topo];
          Point3d pt = expected.Topology[topoInts[i++]];
          Assert.Equal(apiNode.Position.X, new Length(pt.X, unit).Meters);
          Assert.Equal(apiNode.Position.Y, new Length(pt.Y, unit).Meters);
          Assert.Equal(apiNode.Position.Z, new Length(pt.Z, unit).Meters);
        }

        // take last item if lists doesnt match in length
        GsaProp2d prop = (i > expected.Properties.Count - 1) ? expected.Properties.Last() : expected.Properties[i];

        TestProp2d(prop, api.Property, actualModel);
      }
    }
  }
}
