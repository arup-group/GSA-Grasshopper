using System;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GH_GeometricGooResultTests {

    [Theory]
    [InlineData(typeof(LineResultGoo))]
    [InlineData(typeof(MeshResultGoo))]
    [InlineData(typeof(PointResultGoo))]
    [InlineData(typeof(VectorResultGoo))]
    public void GH_OasysComponentTest(Type t) {
      var param = (IGH_GeometricGoo)Activator.CreateInstance(t);
      
      Assert.True(param.Boundingbox.IsValid);
      Assert.True(param.IsValid);
      Assert.NotNull(param.TypeDescription);
      Assert.NotNull(param.TypeName);

      var preview = (IGH_PreviewData)param;
      Assert.True(preview.ClippingBox.IsValid);
    }
  }
}
