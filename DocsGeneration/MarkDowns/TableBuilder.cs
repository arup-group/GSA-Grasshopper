using System;
using System.Collections.Generic;
using System.Linq;

namespace DocsGeneration.MarkDowns {
  public static class TableBuilder {
    public enum TableType {
      InputOutput,
      IconOnly,
      Properties,
    }

    private static readonly List<string> _iconTableHeader = new List<string>() {
      "Icon",
    };
    private static readonly List<string> _inputOutputTableHeaders = new List<string>() {
      "Icon",
      "Type",
      "Name",
      "Description",
    };
    private static readonly List<string> _propertiesHeaders = new List<string>() {
      " ", // icon
      "Name",
      "Description",
    };

    private static readonly List<int> _inputOutputImageWidths = new List<int>() {
      Table.IconWidth,
      Table.NameWidth,
      Table.NameWidth,
      Table.DescriptionWidth,
    };
    private static readonly List<int> _propertiesImageWidths = new List<int>() {
      Table.IconWidth,
      Table.NameWidth,
      Table.DescriptionWidth,
    };
    private static readonly List<int> _iconWidth = new List<int>() {
      150,
    };

    public static string CreateTableString(
      string name, TableType tableType, List<List<string>> tempTable, int headingSize = 2) {
      switch (tableType) {
        case TableType.InputOutput:
          tempTable.Insert(0, _inputOutputTableHeaders); // add heading to calculate width of column

          var table = new Table(name, headingSize, GetColumnsWidth(tempTable));
          table.AddTableHeader(_inputOutputTableHeaders, _inputOutputImageWidths);
          tempTable.Skip(1).ToList().ForEach(row => table.AddRow(row));
          return table.Finalise();
        case TableType.IconOnly:
          tempTable.Insert(0, _iconTableHeader); // add heading to calculate width of column

          var iconTable = new Table(string.Empty, headingSize, GetColumnsWidth(tempTable));
          iconTable.AddTableHeader(_iconTableHeader, _iconWidth);
          iconTable.AddRow(tempTable[1]);
          return iconTable.Finalise();
        case TableType.Properties:
          tempTable.Insert(0, _propertiesHeaders); // add heading to calculate width of column

          var propertiesTable = new Table(name, headingSize, GetColumnsWidth(tempTable));
          propertiesTable.AddTableHeader(_propertiesHeaders, _propertiesImageWidths);
          tempTable.Skip(1).ToList().ForEach(row => propertiesTable.AddRow(row));
          return propertiesTable.Finalise();
        default:return null;
      }
    }

    public static List<int> GetColumnsWidth(List<List<string>> table) {
      const int _columnMinWidth = 30;
      return Enumerable.Range(0, table[0].Count).Select(i
        => Math.Max(_columnMinWidth, table.Where(row => i < row.Count).Max(row => row[i].Length))).ToList();
    }
  }
}
