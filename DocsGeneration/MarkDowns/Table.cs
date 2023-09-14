using Eto.Forms;
using System;
using System.Collections.Generic;

namespace DocsGeneration.MarkDowns {
  public class Table {
    public const int IconWidth = 20;
    public const int NameWidth = 200;
    public const int DescriptionWidth = 1000;

    public string Name { get; set; }
    public List<string> Headers { get; set; }
    private string _table = string.Empty;
    private bool _hasRows = false;
    public Table(string name, int headingSize, List<string> headers, List<int> columnWidths) {
      Name = name;
      if (!string.IsNullOrEmpty(name)) {
        _table += $"{new string('#', headingSize)} {name}\n\n";
      }

      Headers = headers;
      for (int i = 0; i < headers.Count; i++) {
        string width = $"<img width=\"{columnWidths[i]}\"/>";
        _table += $"|{width} {headers[i]} ";
      }

      _table += $"|\n";
      foreach (string header in Headers) {
        _table += $"| ----------- ";
      }

      _table += $"|\n";
    }

    public void AddRow(List<string> items) {
      _hasRows = true;
      foreach (string item in items) {
        string row = item
          .Replace(Environment.NewLine, "<br />")
          .Replace("|", "&#124;");
        _table += $"|{row} ";
      }

      _table += $"|\n";
    }

    public string Finalise() {
      if (_hasRows) {
        return _table + "\n";
      }

      return string.Empty;
    }
  }
}
