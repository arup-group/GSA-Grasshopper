using Eto.Forms;
using System;
using System.Collections.Generic;

namespace GsaGhDocs.MarkDowns {
  public class Table {
    public string Name { get; set; }
    public List<string> Headers { get; set; }
    private string _table = string.Empty;
    private bool _hasRows = false;
    public Table(string name, List<string> headers) {
      Name = name;
      if (!string.IsNullOrEmpty(name)) {
        _table += $"## {name}\n\n";
      }

      Headers = headers;
      foreach (string header in Headers) {
        _table += $"|{header} ";
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
          .Replace("|", string.Empty);
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
