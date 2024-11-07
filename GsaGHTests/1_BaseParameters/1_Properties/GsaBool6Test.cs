using System.Collections.Generic;

using Grasshopper.Kernel;

using GsaGH.Helpers.Assembly;
using GsaGH.Parameters;

using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaBool6Test {

    [Fact]
    public void AssembleWithElementTest() {
      var e1d = new GsaElement1d(new LineCurve(new Point3d(0, 0, 0), new Point3d(10, 0, 0))) {
        ReleaseStart = new GsaBool6(true, false, true, false, true, false),
        ReleaseEnd = new GsaBool6(false, true, false, true, false, true),
      };

      var assembly = new ModelAssembly(e1d);
      GsaAPI.Model assembled = assembly.GetModel();

      var startAssembled = new GsaBool6(assembled.Elements()[1].Release(0));
      var endAssembled = new GsaBool6(assembled.Elements()[1].Release(1));

      _ = Duplicates.AreEqual(e1d.ReleaseStart, startAssembled);
      _ = Duplicates.AreEqual(e1d.ReleaseEnd, endAssembled);
    }

    [Fact]
    public void AssembleWitMemberTest() {
      var m1d = new GsaMember1d(new LineCurve(new Point3d(0, 0, 0), new Point3d(10, 0, 0))) {
        ReleaseStart = new GsaBool6(true, false, true, false, true, false),
        ReleaseEnd = new GsaBool6(false, true, false, true, false, true),
      };

      var assembly = new ModelAssembly(m1d);
      GsaAPI.Model assembled = assembly.GetModel();

      var startAssembled = new GsaBool6(assembled.Members()[1].GetEndRelease(0).Releases);
      var endAssembled = new GsaBool6(assembled.Members()[1].GetEndRelease(1).Releases);

      _ = Duplicates.AreEqual(m1d.ReleaseStart, startAssembled);
      _ = Duplicates.AreEqual(m1d.ReleaseEnd, endAssembled);
    }

    [Theory]
    [InlineData(true, true, true, true, true, true)]
    [InlineData(false, false, false, false, false, false)]
    public void ConstructorTest(bool x, bool y, bool z, bool xx, bool yy, bool zz) {
      var b6 = new GsaBool6(x, y, z, xx, yy, zz);

      Assert.Equal(x, b6.X);
      Assert.Equal(y, b6.Y);
      Assert.Equal(z, b6.Z);
      Assert.Equal(xx, b6.Xx);
      Assert.Equal(yy, b6.Yy);
      Assert.Equal(zz, b6.Zz);
    }

    [Theory]
    [InlineData(true, true, true, true, true, true)]
    [InlineData(false, false, false, false, false, false)]
    public void DuplicateTest(bool x, bool y, bool z, bool xx, bool yy, bool zz) {
      var original = new GsaBool6(x, y, z, xx, yy, zz);

      var duplicate = new GsaBool6(original);

      _ = Duplicates.AreEqual(original, duplicate);

      duplicate.X = false;
      duplicate.Y = true;
      duplicate.Z = false;
      duplicate.Xx = true;
      duplicate.Yy = false;
      duplicate.Zz = true;

      Assert.Equal(x, original.X);
      Assert.Equal(y, original.Y);
      Assert.Equal(z, original.Z);
      Assert.Equal(xx, original.Xx);
      Assert.Equal(yy, original.Yy);
      Assert.Equal(zz, original.Zz);
    }

    [Fact]
    public void ReleaseParameterInfoIsCorrect() {
      var param = new GsaReleaseParameter();
      Assert.Equal(GsaReleaseParameterInfo.Name, param.Name);
      Assert.Equal(GsaReleaseParameterInfo.Description, param.Description);
      Assert.Equal(GsaReleaseParameterInfo.NickName, param.NickName);
      Assert.Equal(GsaReleaseParameterInfo.Description, param.InstanceDescription);
      Assert.Equal(GsaReleaseParameterInfo.Name, param.TypeName);
    }

    [Fact]
    public void ReleaseParameterReportErrorForWongInput() {
      var param = new GsaReleaseParameter();
      param.CreateAttributes();
      ComponentTestHelper.SetInput(param, "wrong");
      IList<string> message = param.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error);
      Assert.Single(message);
      Assert.Equal("Data conversion failed from Text to Bool6", message[0]);
      param.ClearData();
      ComponentTestHelper.SetInput(param, "FFTTRR");
      message = param.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error);
      Assert.Single(message);
      Assert.Equal("Unable to convert string to Bool6, character t not recognised", message[0]);
    }

    [Fact]
    public void RestraintParameterInfoIsCorrect() {
      var param = new GsaRestraintParameter();
      Assert.Equal(GsaRestraintParameterInfo.Name, param.Name);
      Assert.Equal(GsaRestraintParameterInfo.Description, param.Description);
      Assert.Equal(GsaRestraintParameterInfo.NickName, param.NickName);
      Assert.Equal(GsaRestraintParameterInfo.Description, param.InstanceDescription);
      Assert.Equal(GsaRestraintParameterInfo.Name, param.TypeName);
    }

    [Fact]
    public void RestraintParameterReportErrorForWongInput() {
      var param = new GsaRestraintParameter();
      param.CreateAttributes();
      ComponentTestHelper.SetInput(param, "wrong");
      IList<string> message = param.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error);
      Assert.Single(message);
      Assert.Equal("Data conversion failed from Text to Bool6", message[0]);
      param.ClearData();
      ComponentTestHelper.SetInput(param, "FFTTRR");
      message = param.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error);
      Assert.Single(message);
      Assert.Equal("Unable to convert string to Bool6, character t not recognised", message[0]);

    }
  }
}
