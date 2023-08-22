using System.Collections.Generic;

namespace GsaGhDocs.Helpers {
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
        _table += $"|{item.Replace("\n", string.Empty)} ";
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
