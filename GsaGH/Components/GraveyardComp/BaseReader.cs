using System;
using System.Drawing;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;

namespace GsaGH.Components.GraveyardComp {
  internal class BaseReader {

    internal static bool Read(
      GH_IReader reader,
      GH_Component owner,
      bool ignoreParamCountOverspill = false) {
      string mName = "";
      reader.TryGetString("Name", ref mName);
      owner.Name = mName;

      string mNickname = "";
      reader.TryGetString("NickName", ref mNickname);
      owner.NickName = mNickname;

      string mDescription = "";
      reader.TryGetString("Description", ref mDescription);
      owner.Description = mDescription;
      owner.NewInstanceGuid(reader.ItemExists("InstanceGuid")
        ? reader.GetGuid("InstanceGuid")
        : Guid.NewGuid());

      int value = 0;
      reader.TryGetInt32("IconDisplay", ref value);
      switch (value) {
        case 1:
          owner.IconDisplayMode = GH_IconDisplayMode.name;
          break;

        case 2:
          owner.IconDisplayMode = GH_IconDisplayMode.icon;
          break;

        default:
          owner.IconDisplayMode = GH_IconDisplayMode.application;
          break;
      }

      if (reader.ItemExists("IconOverride"))
        owner.SetIconOverride(reader.GetDrawingBitmap("IconOverride"));

      if (owner.Attributes != null) {
        GH_IReader ghIReader2 = reader.FindChunk("Attributes");
        if (ghIReader2 != null)
          owner.Attributes.Read(ghIReader2);
        else
          reader.AddMessage("Attributes chunk is missing. Could be a hint something's wrong.",
            GH_Message_Type.info);
      }

      if (owner is IGH_PreviewObject previewObject) {
        bool value1 = false;
        if (reader.TryGetBoolean("Hidden", ref value1))
          previewObject.Hidden = value1;
        else {
          bool value2 = true;
          if (reader.TryGetBoolean("Preview", ref value2))
            previewObject.Hidden = !value2;
        }
      }

      owner.PrincipalParameterIndex = -1;
      if (reader.ItemExists("PrincipalIndex"))
        owner.PrincipalParameterIndex = reader.GetInt32("PrincipalIndex");

      bool mMutableName = true;
      reader.TryGetBoolean("Mutable", ref mMutableName);
      owner.MutableNickName = mMutableName;
      bool mLocked = false;
      if (reader.ItemExists("Locked"))
        mLocked = reader.GetBoolean("Locked");

      if (reader.ItemExists("Enabled"))
        mLocked = !reader.GetBoolean("Enabled");
      owner.Locked = mLocked;

      int num = owner.Params.Input.Count - 1;
      for (int i = 0; i <= num; i++) {
        GH_IReader ghIReader = reader.FindChunk("param_input", i);
        if (ghIReader == null) {
          if (!ignoreParamCountOverspill)
            reader.AddMessage("Input parameter chunk is missing. Archive is corrupt.",
              GH_Message_Type.error);
          continue;
        }

        GH_ParamAccess access = owner.Params.Input[i]
          .Access;
        owner.Params.Input[i]
          .Read(ghIReader);
        if (!(owner.Params.Input[i] is Param_ScriptVariable))
          owner.Params.Input[i]
            .Access = access;
      }

      int num2 = owner.Params.Output.Count - 1;
      for (int j = 0; j <= num2; j++) {
        GH_IReader ghIReader2 = reader.FindChunk("param_output", j);
        if (ghIReader2 == null) {
          if (!ignoreParamCountOverspill)
            reader.AddMessage("Output parameter chunk is missing. Archive is corrupt.",
              GH_Message_Type.error);
          continue;
        }

        GH_ParamAccess access2 = owner.Params.Output[j]
          .Access;
        owner.Params.Output[j]
          .Read(ghIReader2);
        owner.Params.Output[j]
          .Access = access2;
      }

      GH_IReader attributes = reader.FindChunk("Attributes");
      if (owner.Attributes == null)
        return true;

      owner.Attributes.Bounds = (RectangleF)attributes.Items[0]
        .InternalData;
      owner.Attributes.Pivot = (PointF)attributes.Items[1]
        .InternalData;

      return true;
    }
  }
}
