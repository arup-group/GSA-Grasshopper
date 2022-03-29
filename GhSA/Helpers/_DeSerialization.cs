using System;
using System.Collections.Generic;

namespace GsaGH.Util.GH
{
    internal class DeSerialization
    {
        internal static GH_IO.Serialization.GH_IWriter writeDropDownComponents(ref GH_IO.Serialization.GH_IWriter writer, List<List<string>> dropdownitems, List<string> selecteditems, List<string> spacerDescriptions)
        {
            // to save the dropdownlist content, spacer list and selection list 
            // loop through the lists and save number of lists as well
            bool dropdown = false;
            if (dropdownitems != null)
            {
                writer.SetInt32("dropdownCount", dropdownitems.Count);
                for (int i = 0; i < dropdownitems.Count; i++)
                {
                    writer.SetInt32("dropdowncontentsCount" + i, dropdownitems[i].Count);
                    for (int j = 0; j < dropdownitems[i].Count; j++)
                        writer.SetString("dropdowncontents" + i + j, dropdownitems[i][j]);
                }
                dropdown = true;
            }
            writer.SetBoolean("dropdown", dropdown);

            // spacer list
            bool spacer = false;
            if (spacerDescriptions != null)
            {
                writer.SetInt32("spacerCount", spacerDescriptions.Count);
                for (int i = 0; i < spacerDescriptions.Count; i++)
                    writer.SetString("spacercontents" + i, spacerDescriptions[i]);
                spacer = true;
            }
            writer.SetBoolean("spacer", spacer);

            // selection list
            bool select = false;
            if (selecteditems != null)
            {
                writer.SetInt32("selectionCount", selecteditems.Count);
                for (int i = 0; i < selecteditems.Count; i++)
                    writer.SetString("selectioncontents" + i, selecteditems[i]);
                select = true;
            }
            writer.SetBoolean("select", select);

            return writer;
        }

        internal static void readDropDownComponents(ref GH_IO.Serialization.GH_IReader reader, ref List<List<string>> dropdownitems, ref List<string> selecteditems, ref List<string> spacerDescriptions)
        {
            // dropdown content list
            if (reader.GetBoolean("dropdown"))
            {
                int dropdownCount = reader.GetInt32("dropdownCount");
                dropdownitems = new List<List<string>>();
                for (int i = 0; i < dropdownCount; i++)
                {
                    int dropdowncontentsCount = reader.GetInt32("dropdowncontentsCount" + i);
                    List<string> tempcontent = new List<string>();
                    for (int j = 0; j < dropdowncontentsCount; j++)
                        tempcontent.Add(reader.GetString("dropdowncontents" + i + j));
                    dropdownitems.Add(tempcontent);
                }
            }
            else
                throw new Exception("Component doesnt have 'dropdown' content stored");

            // spacer list
            if (reader.GetBoolean("spacer"))
            {
                int dropdownspacerCount = reader.GetInt32("spacerCount");
                spacerDescriptions = new List<string>();
                for (int i = 0; i < dropdownspacerCount; i++)
                    spacerDescriptions.Add(reader.GetString("spacercontents" + i));
            }

            // selection list
            if (reader.GetBoolean("select"))
            {
                int selectionsCount = reader.GetInt32("selectionCount");
                selecteditems = new List<string>();
                for (int i = 0; i < selectionsCount; i++)
                    selecteditems.Add(reader.GetString("selectioncontents" + i));
            }
        }
    }
}
