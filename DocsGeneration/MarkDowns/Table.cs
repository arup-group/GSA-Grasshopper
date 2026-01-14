using System;
using System.Collections.Generic;

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
        line = AdjustLineWithWhitespaces(line, i);
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
        line = AdjustLineWithWhitespaces(line, i);
        Rows += line;
      }

      Rows += EndLine;
    }

    private string AdjustLineWithWhitespaces(string line, int i) {
      int lineLength = line.Length - EndLine.Length;
      int missingSpaces = _defaultHeaderWidths[i] - lineLength;
      line += missingSpaces > 0 ? new string(' ', missingSpaces) : string.Empty;
      return line;
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
  }
}
