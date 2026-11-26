using System;
using System.IO;
using System.Linq;

using GH_IO.Serialization;

using Grasshopper.Kernel;

using GsaGH.Parameters;

using GsaGHTests.Helper;

using OasysUnits.Units;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaModelPersistenceTests {
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void InternalisedModelSurvivesArchiveRoundTrip(bool xml) {
      var model = new GsaModel();
      model.ApiModel.Open(GsaFile.SteelDesignSimple);
      model.FileNameAndPath = @"Z:\missing\original.gwb";
      model.ModelUnit = LengthUnit.Foot;
      var parameter = new GsaModelParameter();
      parameter.CreateAttributes();
      parameter.PersistentData.Append(new GsaModelGoo(model));
      var archive = new GH_Archive();

      Assert.True(archive.AppendObject(parameter, "Model"));
      var restoredArchive = new GH_Archive();
      Assert.True(xml ? restoredArchive.Deserialize_Xml(archive.Serialize_Xml())
        : restoredArchive.Deserialize_Binary(archive.Serialize_Binary()));
      var restoredParameter = new GsaModelParameter();
      Assert.True(restoredArchive.ExtractObject(restoredParameter, "Model"));

      Assert.Equal(1, restoredParameter.PersistentData.DataCount);
      GsaModel restored = Assert.IsType<GsaModelGoo>(
        restoredParameter.PersistentData.AllData(true).Single()).Value;
      Assert.NotNull(restored);
      Assert.NotSame(model.ApiModel, restored.ApiModel);
      Assert.NotEmpty(restored.ApiModel.Nodes());
      Assert.Equal(model.ApiModel.Nodes().Count, restored.ApiModel.Nodes().Count);
      Assert.Equal(model.ApiModel.Elements().Count, restored.ApiModel.Elements().Count);
      Assert.Equal(model.ApiModel.Sections().Count, restored.ApiModel.Sections().Count);
      Assert.Equal(model.ApiModel.AnalysisTasks().Count, restored.ApiModel.AnalysisTasks().Count);
      Assert.Equal(model.ApiModel.AnalysisCases().Count, restored.ApiModel.AnalysisCases().Count);
      Assert.NotEmpty(restored.ApiModel.LoadCases());
      Assert.Equal(model.ApiModel.LoadCases().Count, restored.ApiModel.LoadCases().Count);
      Assert.Equal(model.ApiModel.BeamLoads().Count, restored.ApiModel.BeamLoads().Count);
      Assert.Equal(model.ApiModel.GravityLoads().Count, restored.ApiModel.GravityLoads().Count);
      Assert.NotEmpty(model.ApiModel.Results());
      Assert.Equal(model.ApiModel.Results().Count, restored.ApiModel.Results().Count);
      Assert.Equal(model.ApiModel.CombinationCaseResults().Count,
        restored.ApiModel.CombinationCaseResults().Count);
      Assert.Equal(LengthUnit.Foot, restored.ModelUnit);
      Assert.Equal(model.FileNameAndPath, restored.FileNameAndPath);
      restored.ApiModel.Dispose();
      model.ApiModel.Dispose();
    }

    [Fact]
    public void SnapshotDoesNotNeedSourceFile() {
      string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".gwb");
      try {
        File.Copy(GsaFile.SteelDesignSimple, path);
        var model = new GsaModel();
        model.ApiModel.Open(path);
        model.FileNameAndPath = path;
        var original = new GsaModelGoo(model);
        var chunk = new GH_LooseChunk("Model");
        Assert.True(original.Write(chunk));
        int nodes = model.ApiModel.Nodes().Count;
        model.ApiModel.Dispose();
        File.Delete(path);

        var restored = new GsaModelGoo();
        Assert.True(restored.Read(chunk));
        Assert.Null(restored.PersistenceError);
        Assert.Equal(nodes, restored.Value.ApiModel.Nodes().Count);
        Assert.True(restored.Write(new GH_LooseChunk("Model")));
        restored.Value.ApiModel.Dispose();
      } finally {
        File.Delete(path);
      }
    }

    [Theory]
    [InlineData("legacy")]
    [InlineData("missing")]
    [InlineData("version")]
    [InlineData("corrupt")]
    [InlineData("metadata")]
    [InlineData("unit")]
    [InlineData("empty")]
    [InlineData("oversized")]
    public void InvalidSnapshotReportsCanvasError(string kind) {
      var chunk = new GH_LooseChunk("Model");
      if (kind != "legacy") {
        chunk.SetInt32("ModelSnapshotVersion", kind == "version" ? 999 : 1);
        if (kind != "missing") {
          chunk.SetByteArray("ModelSnapshot", kind == "oversized"
            ? new byte[GsaModelGoo.MaximumSnapshotBytes + 1]
            : kind == "empty" ? Array.Empty<byte>() : new byte[] { 1, 2, 3 });
        }
        if (kind != "metadata") {
          chunk.SetInt32("ModelUnit", kind == "unit" ? -1 : (int)LengthUnit.Meter);
          chunk.SetString("FileNameAndPath", string.Empty);
        }
      }

      var previous = new GsaModel();
      var goo = new GsaModelGoo(previous);
      Assert.True(goo.Read(chunk));
      previous.ApiModel.Dispose();
      Assert.Null(goo.Value);
      Assert.False(goo.IsValid);
      Assert.Contains("Cannot restore internalised GSA model", goo.IsValidWhyNot);
      var parameter = new GsaModelParameter();
      parameter.CreateAttributes();
      parameter.PersistentData.Append((GsaModelGoo)goo.Duplicate());
      parameter.CollectData();
      Assert.Contains(goo.PersistenceError, parameter.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void FailedSnapshotCannotBeSilentlyResaved() {
      var chunk = new GH_LooseChunk("Model");
      Assert.True(new GsaModelGoo().Write(chunk));

      var restored = new GsaModelGoo();
      Assert.True(restored.Read(chunk));
      Assert.False(restored.IsValid);
      Assert.Contains("model is empty", restored.PersistenceError);

      var parameter = new GsaModelParameter();
      parameter.CreateAttributes();
      parameter.PersistentData.Append(restored);
      var archive = new GH_Archive();
      archive.AppendObject(parameter, "Model");
      Assert.Equal(GH_Message_Type.error, archive.WorstCaseMessageType);
      var restoredParameter = new GsaModelParameter();
      Assert.True(archive.ExtractObject(restoredParameter, "Model"));
      restoredParameter.CollectData();
      Assert.NotEmpty(restoredParameter.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void EmptyLegacyParameterStillLoads() {
      var parameter = new GsaModelParameter();
      parameter.CreateAttributes();
      var archive = new GH_Archive();
      Assert.True(archive.AppendObject(parameter, "Model"));

      var restored = new GsaModelParameter();
      Assert.True(archive.ExtractObject(restored, "Model"));
      restored.CollectData();
      Assert.Equal(0, restored.PersistentData.DataCount);
      Assert.Empty(restored.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void TemporarySnapshotCleanupFailureDoesNotEscape() {
      string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".gwb");
      File.WriteAllText(path, "snapshot");
      try {
        using (new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) {
          string warning = GsaModelGoo.CleanupTemporarySnapshot(path);
          Assert.NotNull(warning);
          Assert.Contains("Could not remove temporary GSA snapshot", warning);
          Assert.True(File.Exists(path));
        }
        Assert.True(File.Exists(path));
      } finally {
        File.Delete(path);
      }
    }
  }
}
