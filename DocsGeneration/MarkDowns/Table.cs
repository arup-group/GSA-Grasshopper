using System;
using System.Collections.Generic;
using System.Linq;

namespace DocsGeneration.MarkDowns {
  public class Table {
    public const int IconWidth = 20;
    public const int NameWidth = 200;
    public const int DescriptionWidth = 1000;
    private const string EndLine = " |\n";
    private const string StartLine = "| ";

    public string NameLine { get; private set; } = string.Empty;
    private string Headers { get; set; } = string.Empty;
    private string Rows { get; set; } = string.Empty;
    private readonly List<int> _defaultHeaderWidths = new List<int>();
    private string _table = string.Empty;

    public Table(string name, int headingSize, List<int> columnWidths) {
      _defaultHeaderWidths = columnWidths;
      AddHeader(name, headingSize);
    }

    public void AddTableHeader(List<string> headers, List<int> imageWidths) {
      if (headers?.Count != _defaultHeaderWidths?.Count) {
        throw new ArgumentException("Headers and header widths must have the same number of elements.");
        return;
      }

      for (int i = 0; i < headers?.Count; i++) {
        string width = $"<img width=\"{imageWidths[i]}\"/>";
        string line = $"{StartLine}{width} {headers[i]}";
        int lineLength = line.Length - EndLine.Length;

        int missingSpaces = _defaultHeaderWidths[i] - lineLength;
        line += missingSpaces > 0 ? new string(' ', missingSpaces) : string.Empty;
        Headers += line;
      }

      Headers += EndLine;
      _defaultHeaderWidths?.ForEach(item => Headers += StartLine + new string('-', item) + " ");
      Headers += EndLine;
    }

    private void AddHeader(string name, int headingSize) {
      if (!string.IsNullOrEmpty(name)) {
        NameLine += $"{new string('#', headingSize)} {name}\n\n";
      }
    }

    public void AddRow(List<string> items) {
      for (int i = 0; i < items.Count; i++) {
        string row = items[i];
        string line = $"| {row} ";
        int lineLength = line.Length;
        int missingSpaces = _defaultHeaderWidths[i] - lineLength + EndLine.Length;
        line += missingSpaces > 0 ? new string(' ', missingSpaces) : string.Empty;
        Rows += line;
      }

      Rows += EndLine;
    }

    public string Finalise() {
      _table += NameLine;
      if (Rows.Length == 0) {
        return string.Empty;
      }

      _table += Headers;
      _table += Rows;
      return _table + "\n";
    }

    public static List<int> GetColumnsWidth(List<List<string>> table) {
      const int _columnMinWidth = 30;
      return Enumerable.Range(0, table[0].Count).Select(i
        => Math.Max(_columnMinWidth, table.Where(row => i < row.Count).Max(row => row[i].Length))).ToList();
    }
  }
}
