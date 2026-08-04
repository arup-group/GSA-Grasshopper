using System;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.UI.Helpers {
  /// <summary>
  ///   In-memory repository for example files. Initialized once at startup.
  /// </summary>
  public class ExampleFileRepository {
    private static List<FileEntry> _files = new List<FileEntry>();
    private static bool _isInitialized;

    private ExampleFileRepository() { }

    /// <summary>
    ///   Sets the list of files. Can only be called once; subsequent calls are ignored.
    /// </summary>
    public static void SetFiles(List<FileEntry> files) {
      if (_isInitialized) {
        return;
      }

      _files = files ?? new List<FileEntry>();
      _isInitialized = true;
    }

    /// <summary>
    ///   Returns all stored files.
    /// </summary>
    public static List<FileEntry> GetAllFiles() {
      return new List<FileEntry>(_files);
    }

    /// <summary>
    ///   Returns files whose names contain any of the given keywords (case-insensitive).
    /// </summary>
    public static IEnumerable<FileEntry> GetFileEntriesByKeywords(List<string> keywords) {
      if (keywords == null || keywords.Count == 0) {
        return new List<FileEntry>(); // return empty list to avoid unnecessary showing all files
      }

      return _files.Where(f => keywords.Any(k => f.Name.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();
    }

    /// <summary>
    ///   Resets the repository. Intended for testing only.
    /// </summary>
    internal static void Reset() {
      _files = new List<FileEntry>();
      _isInitialized = false;
    }
  }
}
