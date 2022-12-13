using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GsaGH.Components.GraveyardComp
{
  internal class BaseReader
  {
    internal static bool Read(GH_IO.Serialization.GH_IReader reader, GH_Component owner, bool ignoreParamCountOverspill = false)
    {
      // GH_InstanceDescription
      string m_name = "";
      reader.TryGetString("Name", ref m_name);
      owner.Name= m_name;

      string m_nickname = "";
      reader.TryGetString("NickName", ref m_nickname);
      owner.NickName = m_nickname;

      string m_description = "";
      reader.TryGetString("Description", ref m_description);
      owner.Description = m_description;
      if (reader.ItemExists("InstanceGuid"))
      {
        owner.NewInstanceGuid(reader.GetGuid("InstanceGuid"));
      }
      else
      {
        owner.NewInstanceGuid(Guid.NewGuid());
      }

      // GH_DocumentObject
      int value = 0;
      reader.TryGetInt32("IconDisplay", ref value);
      switch (value)
      {
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
      {
        owner.SetIconOverride(reader.GetDrawingBitmap("IconOverride"));
      }

      //owner._valueTable = null;
      //GH_IReader gH_IReader = reader.FindChunk("ValueTable");
      //if (gH_IReader != null)
      //{
      //  EnsureValueTable();
      //  _valueTable.Read(gH_IReader);
      //}

      //owner.ValuesChanged();
      if (owner.Attributes != null)
      {
        GH_IReader gH_IReader2 = reader.FindChunk("Attributes");
        if (gH_IReader2 != null)
          owner.Attributes.Read(gH_IReader2);
        else
          reader.AddMessage("Attributes chunk is missing. Could be a hint something's wrong.", GH_Message_Type.info);
      }


      // GH_ActiveObject
      if (owner is IGH_PreviewObject)
      {
        bool value1 = false;
        if (reader.TryGetBoolean("Hidden", ref value1))
          ((IGH_PreviewObject)owner).Hidden = value1;
        else
        {
          bool value2 = true;
          if (reader.TryGetBoolean("Preview", ref value2))
            ((IGH_PreviewObject)owner).Hidden = !value2;
        }
      }

      // GH_Component
      owner.PrincipalParameterIndex = -1;
      if (reader.ItemExists("PrincipalIndex"))
        owner.PrincipalParameterIndex = reader.GetInt32("PrincipalIndex");

      // GH_ComponentParamServer
      bool m_mutableName = true;
      reader.TryGetBoolean("Mutable", ref m_mutableName);
      owner.MutableNickName= m_mutableName;
      bool m_locked = false;
      if (reader.ItemExists("Locked"))
        m_locked = reader.GetBoolean("Locked");

      if (reader.ItemExists("Enabled"))
        m_locked = !reader.GetBoolean("Enabled");
      owner.Locked = m_locked;

      int num = owner.Params.Input.Count - 1;
      bool flag = true;
      for (int i = 0; i <= num; i++)
      {
        GH_IReader gH_IReader = reader.FindChunk("param_input", i);
        if (gH_IReader == null)
        {
          if (!ignoreParamCountOverspill)
            reader.AddMessage("Input parameter chunk is missing. Archive is corrupt.", GH_Message_Type.error);
          continue;
        }

        GH_ParamAccess access = owner.Params.Input[i].Access;
        flag &= owner.Params.Input[i].Read(gH_IReader);
        if (!(owner.Params.Input[i] is Param_ScriptVariable))
          owner.Params.Input[i].Access = access;
      }

      int num2 = owner.Params.Output.Count - 1;
      for (int j = 0; j <= num2; j++)
      {
        GH_IReader gH_IReader2 = reader.FindChunk("param_output", j);
        if (gH_IReader2 == null)
        {
          if (!ignoreParamCountOverspill)
            reader.AddMessage("Output parameter chunk is missing. Archive is corrupt.", GH_Message_Type.error);
          continue;
        }

        GH_ParamAccess access2 = owner.Params.Output[j].Access;
        flag &= owner.Params.Output[j].Read(gH_IReader2);
        owner.Params.Output[j].Access = access2;
      }

      GH_IReader attributes = reader.FindChunk("Attributes");
      owner.Attributes.Bounds = (System.Drawing.RectangleF)attributes.Items[0].InternalData;
      owner.Attributes.Pivot = (System.Drawing.PointF)attributes.Items[1].InternalData;

      return true;
    }
  }
}
