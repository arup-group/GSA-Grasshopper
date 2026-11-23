using System;
using System.IO;

using GH_IO.Serialization;

using Grasshopper.Kernel.Types;

using GsaAPI;

using OasysGH;
using OasysGH.Parameters;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaModel" /> can be used in Grasshopper.
  /// </summary>
  public class GsaModelGoo : GH_OasysGoo<GsaModel> {
    public static string Description => "GSA Model";
    public static string Name => "Model";
    public static string NickName => "GSA";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    public override string IsValidWhyNot => PersistenceError ?? base.IsValidWhyNot;
    internal string PersistenceError { get; private set; }
    internal const int MaximumSnapshotBytes = 100 * 1024 * 1024;
    private const int SnapshotVersion = 1;

    public GsaModelGoo() : this(null) { }

    public GsaModelGoo(GsaModel item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaModelGoo(Value) { PersistenceError = PersistenceError };
    }

    public override bool Write(GH_IWriter writer) {
      string path = Path.Combine(Path.GetTempPath(), System.Guid.NewGuid() + ".gwb");
      try {
        if (Value?.ApiModel == null) {
          throw new InvalidDataException(PersistenceError ?? "The GSA model is empty.");
        }

        // Save a clone: SaveAs changes the native model's file association.
        using (Model snapshot = Value.ApiModel.Clone()) {
          ReturnValue status = snapshot.SaveAs(path);
          if (status != ReturnValue.GS_OK) {
            throw new IOException($"GSA could not save the internalised model: {status}.");
          }
        }

        long length = new FileInfo(path).Length;
        ValidateSnapshotLength(length);
        writer.SetInt32("ModelSnapshotVersion", SnapshotVersion);
        writer.SetByteArray("ModelSnapshot", File.ReadAllBytes(path));
        writer.SetInt32("ModelUnit", (int)Value.ModelUnit);
        writer.SetString("FileNameAndPath", Value.FileNameAndPath ?? string.Empty);
        return true;
      } catch (Exception ex) when (IsPersistenceException(ex)) {
        string message = $"Cannot internalise GSA model: {ex.Message}";
        writer.SetString("ModelPersistenceError", message);
        writer.AddMessage(message, GH_Message_Type.error);
        // Grasshopper discards the item if false is returned, including its error marker.
        return true;
      } finally {
        File.Delete(path);
      }
    }

    public override bool Read(GH_IReader reader) {
      Value = null;
      PersistenceError = null;
      string path = Path.Combine(Path.GetTempPath(), System.Guid.NewGuid() + ".gwb");
      try {
        if (reader.ItemExists("ModelPersistenceError")) {
          throw new InvalidDataException(reader.GetString("ModelPersistenceError"));
        }
        if (!reader.ItemExists("ModelSnapshotVersion") || !reader.ItemExists("ModelSnapshot")) {
          throw new InvalidDataException(
            "No embedded model data. Reconnect the original model and internalise it again.");
        }
        int version = 0;
        if (!reader.TryGetInt32("ModelSnapshotVersion", ref version) || version != SnapshotVersion) {
          throw new InvalidDataException("Unsupported embedded GSA model version.");
        }

        byte[] bytes = reader.GetByteArray("ModelSnapshot");
        ValidateSnapshotLength(bytes.LongLength);
        int unit = 0;
        string fileName = null;
        if (!reader.TryGetInt32("ModelUnit", ref unit) || !Enum.IsDefined(typeof(LengthUnit), unit)
          || !reader.TryGetString("FileNameAndPath", ref fileName)) {
          throw new InvalidDataException("Missing or invalid embedded model metadata.");
        }

        File.WriteAllBytes(path, bytes);
        using (var snapshot = new Model()) {
          snapshot.Open(path);
          Value = new GsaModel(snapshot.Clone()) {
            ModelUnit = (LengthUnit)unit,
            FileNameAndPath = fileName
          };
        }
      } catch (Exception ex) when (IsPersistenceException(ex)) {
        PersistenceError = $"Cannot restore internalised GSA model: {ex.Message}";
        reader.AddMessage(PersistenceError, GH_Message_Type.error);
      } finally {
        File.Delete(path);
      }

      // Retain an invalid item so the parameter can report the error on the canvas.
      return true;
    }

    private static bool IsPersistenceException(Exception ex) {
      return ex is IOException || ex is InvalidDataException || ex is UnauthorizedAccessException
        || ex is GsaApiException || ex is ArgumentException || ex is InvalidCastException;
    }

    private static void ValidateSnapshotLength(long length) {
      if (length <= 0 || length > MaximumSnapshotBytes) {
        throw new InvalidDataException("Embedded GSA models must be between 1 byte and 100 MiB.");
      }
    }
  }
}
