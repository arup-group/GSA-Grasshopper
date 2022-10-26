using System;
using System.Collections.Generic;

namespace GsaGH.Util.GH
{
  internal class DeSerialization
  {
    internal static GH_IO.Serialization.GH_IWriter writeDropDownComponents(ref GH_IO.Serialization.GH_IWriter writer, List<List<string>> DropDownItems, List<string> SelectedItems, List<string> SpacerDescriptions)
    {
      // to save the dropdownlist content, spacer list and selection list 
      // loop through the lists and save number of lists as well
      bool dropdown = false;
      if (DropDownItems != null)
      {
        writer.SetInt32("dropdownCount", DropDownItems.Count);
        for (int i = 0; i < DropDownItems.Count; i++)
        {
          writer.SetInt32("dropdowncontentsCount" + i, DropDownItems[i].Count);
          for (int j = 0; j < DropDownItems[i].Count; j++)
            writer.SetString("dropdowncontents" + i + j, DropDownItems[i][j]);
        }
        dropdown = true;
      }
      writer.SetBoolean("dropdown", dropdown);

      // spacer list
      bool spacer = false;
      if (SpacerDescriptions != null)
      {
        writer.SetInt32("spacerCount", SpacerDescriptions.Count);
        for (int i = 0; i < SpacerDescriptions.Count; i++)
          writer.SetString("spacercontents" + i, SpacerDescriptions[i]);
        spacer = true;
      }
      writer.SetBoolean("spacer", spacer);

      // selection list
      bool select = false;
      if (SelectedItems != null)
      {
        writer.SetInt32("selectionCount", SelectedItems.Count);
        for (int i = 0; i < SelectedItems.Count; i++)
          writer.SetString("selectioncontents" + i, SelectedItems[i]);
        select = true;
      }
      writer.SetBoolean("select", select);

      return writer;
    }

    internal static void readDropDownComponents(ref GH_IO.Serialization.GH_IReader reader, ref List<List<string>> DropDownItems, ref List<string> SelectedItems, ref List<string> SpacerDescriptions)
    {
      // dropdown content list
      if (reader.GetBoolean("dropdown"))
      {
        int dropdownCount = reader.GetInt32("dropdownCount");
        DropDownItems = new List<List<string>>();
        for (int i = 0; i < dropdownCount; i++)
        {
          int dropdowncontentsCount = reader.GetInt32("dropdowncontentsCount" + i);
          List<string> tempcontent = new List<string>();
          for (int j = 0; j < dropdowncontentsCount; j++)
            tempcontent.Add(reader.GetString("dropdowncontents" + i + j));
          DropDownItems.Add(tempcontent);
        }
      }
      else
        throw new Exception("Component doesnt have 'dropdown' content stored");

      // spacer list
      if (reader.GetBoolean("spacer"))
      {
        int dropdownspacerCount = reader.GetInt32("spacerCount");
        SpacerDescriptions = new List<string>();
        for (int i = 0; i < dropdownspacerCount; i++)
          SpacerDescriptions.Add(reader.GetString("spacercontents" + i));
      }

      // selection list
      if (reader.GetBoolean("select"))
      {
        int selectionsCount = reader.GetInt32("selectionCount");
        SelectedItems = new List<string>();
        for (int i = 0; i < selectionsCount; i++)
          SelectedItems.Add(reader.GetString("selectioncontents" + i));
      }
    }
  }
}
