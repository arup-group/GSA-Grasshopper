using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Microsoft.Data.Sqlite;

namespace GsaGH.Helpers.GsaApi {
  /// <summary>
  ///   Class containing functions to interface with SQLite db files.
  /// </summary>
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class MicrosoftSQLiteReader : MarshalByRefObject {
    private static readonly Lazy<MicrosoftSQLiteReader> lazy
      = new Lazy<MicrosoftSQLiteReader>(Initialize);
    public static MicrosoftSQLiteReader Instance => lazy.Value;

    public MicrosoftSQLiteReader() { }

    public static MicrosoftSQLiteReader Initialize() {
      string codeBase = Assembly.GetCallingAssembly().CodeBase;
      var uri = new UriBuilder(codeBase);
      string codeBasePath = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));

      string pluginPath = AddReferencePriority.PluginPath;

      var properties = new Dictionary<string, object>() {
        {
          "source", "Initialize"
        }, {
          "codeBasePath", codeBasePath
        }, {
          "pluginPath", pluginPath
        },
      };

      PostHog.Debug(properties);

      AppDomain.CurrentDomain.UnhandledException += UEHandler;

      // this is a temporary fix for TDA
      // needs more investigation!
      if (Assembly.GetEntryAssembly() != null
        && !Assembly.GetEntryAssembly().FullName.Contains("compute.geometry")) {
        Assembly.LoadFile(pluginPath + @"\Microsoft.Data.Sqlite.dll");
      }

      return new MicrosoftSQLiteReader();
    }

    /// <summary>
    ///   Method to set up a SQLite Connection to a specified .db3 file.
    ///   Will return a SQLite connection to the aforementioned .db3 file database.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public SqliteConnection Connection(string filePath) {
      string connectionString = $"Data Source={filePath};Mode=ReadOnly";
      return new SqliteConnection(connectionString);
    }

    /// <summary>
    ///   This method will return a list of double with values in [m] units and ordered as follows:
    ///   [0]: Depth
    ///   [1]: Width
    ///   [2]: Web THK
    ///   [3]: Flange THK
    ///   [4]: Root radius (only if section is not welded!)
    /// </summary>
    /// <param name="profileString"></param>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public List<double> GetCatalogueProfileValues(string profileString, string filePath) {
      var values = new List<double>();

      using (SqliteConnection db = Connection(filePath)) {
        db.Open();
        SqliteCommand cmd = db.CreateCommand();
        cmd.CommandText = "Select " + "SECT_DEPTH_DIAM || ' -- ' || " + "SECT_WIDTH || ' -- ' || "
          + "SECT_WEB_THICK || ' -- ' || " + "SECT_FLG_THICK || ' -- ' || " + "SECT_ROOT_RAD "
          + $"as SECT_NAME from Sect INNER JOIN Types ON Sect.SECT_TYPE_NUM = Types.TYPE_NUM where SECT_NAME = \"{profileString}\" ORDER BY SECT_DATE_ADDED";
        cmd.CommandType = CommandType.Text;
        var data = new List<string>();
        SqliteDataReader r = cmd.ExecuteReader();
        while (r.Read()) {
          string sqlData = Convert.ToString(r["SECT_NAME"]);
          data.Add(sqlData);
        }

        string[] vals = data[0].Split(new[] {
          " -- ",
        }, StringSplitOptions.None);
        r.Close();

        if (vals.Length <= 1) {
          cmd = db.CreateCommand();
          cmd.CommandText = "Select " + "SECT_DEPTH_DIAM || ' -- ' || " + "SECT_WIDTH || ' -- ' || "
            + "SECT_WEB_THICK || ' -- ' || " + "SECT_FLG_THICK "
            + $"as SECT_NAME from Sect INNER JOIN Types ON Sect.SECT_TYPE_NUM = Types.TYPE_NUM where SECT_NAME = \"{profileString}\" ORDER BY SECT_DATE_ADDED";
          cmd.CommandType = CommandType.Text;
          data = new List<string>();
          r = cmd.ExecuteReader();
          while (r.Read()) {
            string sqlData = Convert.ToString(r["SECT_NAME"]);
            data.Add(sqlData);
          }

          vals = data[0].Split(new[] {
            " -- ",
          }, StringSplitOptions.None);
          r.Close();
        }

        if (vals.Length <= 1) {
          cmd.CommandText = "Select " + "SECT_DEPTH_DIAM || ' -- ' || " + "SECT_WEB_THICK "
            + $"as SECT_NAME from Sect INNER JOIN Types ON Sect.SECT_TYPE_NUM = Types.TYPE_NUM where SECT_NAME = \"{profileString}\" ORDER BY SECT_DATE_ADDED";
          cmd.CommandType = CommandType.Text;
          data = new List<string>();
          r = cmd.ExecuteReader();
          while (r.Read()) {
            string sqlData = Convert.ToString(r["SECT_NAME"]);
            data.Add(sqlData);
          }

          vals = data[0].Split(new[] {
            " -- ",
          }, StringSplitOptions.None);
          r.Close();
        }

        db.Close();

        NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;

        values.AddRange(from val in vals
          where val != string.Empty
          select Convert.ToDouble(val, noComma));
      }

      return values;
    }

    /// <summary>
    ///   Get catalogue data from SQLite file (.db3). The method returns a tuple with:
    ///   Item1 = list of catalogue name (string)
    ///   where first item will be "All"
    ///   Item2 = list of catalogue number (int)
    ///   where first item will be "-1" representing All
    /// </summary>
    /// <param name="filePath">Path to SecLib.db3</param>
    /// <returns></returns>
    public Tuple<List<string>, List<int>> GetCataloguesDataFromSQLite(string filePath) {
      var catNames = new List<string>();
      var catNumber = new List<int>();

      using (SqliteConnection db = Connection(filePath)) {
        db.Open();
        SqliteCommand cmd = db.CreateCommand();
        cmd.CommandText = @"Select CAT_NAME || ' -- ' || CAT_NUM as CAT_NAME from Catalogues";

        cmd.CommandType = CommandType.Text;
        SqliteDataReader r = cmd.ExecuteReader();
        while (r.Read()) {
          string sqlData = Convert.ToString(r["CAT_NAME"]);
          catNames.Add(sqlData.Split(new[] {
            " -- ",
          }, StringSplitOptions.None)[0]);
          catNumber.Add(int.Parse(sqlData.Split(new[] {
            " -- ",
          }, StringSplitOptions.None)[1]));
        }

        db.Close();
      }

      catNames.Insert(0, "All");
      catNumber.Insert(0, -1);
      return new Tuple<List<string>, List<int>>(catNames, catNumber);
    }

    /// <summary>
    ///   Get a list of section profile strings from SQLite file (.db3). The method returns a string that includes type
    ///   abbriviation as accepted by GSA.
    /// </summary>
    /// <param name="type_numbers">List of types to get sections from</param>
    /// <param name="filePath">Path to SecLib.db3</param>
    /// <param name="inclSuperseeded">True if you want to include superseeded items</param>
    /// <returns></returns>
    public List<string> GetSectionsDataFromSQLite(
      List<int> type_numbers, string filePath, bool inclSuperseeded = false) {
      var sections = new List<string>();

      var types = new List<int>();
      if (type_numbers[0] == -1) {
        Tuple<List<string>, List<int>> typeData
          = GetTypesDataFromSQLite(-1, filePath, inclSuperseeded);
        types = typeData.Item2;
        types.RemoveAt(0);
      } else {
        types = type_numbers;
      }

      using (SqliteConnection db = Connection(filePath)) {
        foreach (int type in types) {
          db.Open();
          SqliteCommand cmd = db.CreateCommand();

          cmd.CommandText = inclSuperseeded ?
            $"Select Types.TYPE_ABR || ' ' || SECT_NAME || ' -- ' || SECT_DATE_ADDED as SECT_NAME from Sect INNER JOIN Types ON Sect.SECT_TYPE_NUM = Types.TYPE_NUM where SECT_TYPE_NUM = {type} ORDER BY SECT_AREA" :
            $"Select Types.TYPE_ABR || ' ' || SECT_NAME as SECT_NAME from Sect INNER JOIN Types ON Sect.SECT_TYPE_NUM = Types.TYPE_NUM where SECT_TYPE_NUM = {type} and not (SECT_SUPERSEDED = True or SECT_SUPERSEDED = TRUE or SECT_SUPERSEDED = 1) ORDER BY SECT_AREA";

          cmd.CommandType = CommandType.Text;
          SqliteDataReader r = cmd.ExecuteReader();
          while (r.Read()) {
            if (inclSuperseeded) {
              string full = Convert.ToString(r["SECT_NAME"]);
              // BSI-IPE IPEAA80 -- 2017-09-01 00:00:00.000
              string profile = full.Split(new[] {
                " -- ",
              }, StringSplitOptions.None)[0];
              string date = full.Split(new[] {
                " -- ",
              }, StringSplitOptions.None)[1];
              date = date.Replace("-", string.Empty);
              date = date.Substring(0, 8);
              sections.Add(profile + " " + date);
            } else {
              string profile = Convert.ToString(r["SECT_NAME"]);
              // BSI-IPE IPEAA80
              sections.Add(profile);
            }
          }

          db.Close();
        }
      }

      sections.Sort();

      sections.Insert(0, "All");

      return sections;
    }

    /// <summary>
    ///   Get section type data from SQLite file (.db3). The method returns a tuple with:
    ///   Item1 = list of type name (string)
    ///   where first item will be "All"
    ///   Item2 = list of type number (int)
    ///   where first item will be "-1" representing All
    /// </summary>
    /// <param name="catalogue_number">
    ///   Catalogue number to get section types from. Input -1 in first item of the input list to
    ///   get all types
    /// </param>
    /// <param name="filePath">Path to SecLib.db3</param>
    /// <param name="inclSuperseeded">True if you want to include superseeded items</param>
    /// <returns></returns>
    public Tuple<List<string>, List<int>> GetTypesDataFromSQLite(
      int catalogue_number, string filePath, bool inclSuperseeded = false) {
      var typeNames = new List<string>();
      var typeNumber = new List<int>();

      var catNumbers = new List<int>();
      if (catalogue_number == -1) {
        Tuple<List<string>, List<int>> catalogueData = GetCataloguesDataFromSQLite(filePath);
        catNumbers = catalogueData.Item2;
        catNumbers.RemoveAt(0);
      } else {
        catNumbers.Add(catalogue_number);
      }

      using (SqliteConnection db = Connection(filePath)) {
        foreach (int cat in catNumbers) {
          db.Open();
          SqliteCommand cmd = db.CreateCommand();
          cmd.CommandText = inclSuperseeded ?
            $"Select TYPE_NAME || ' -- ' || TYPE_NUM as TYPE_NAME from Types where TYPE_CAT_NUM = {cat}" :
            $"Select TYPE_NAME || ' -- ' || TYPE_NUM as TYPE_NAME from Types where TYPE_CAT_NUM = {cat} and not (TYPE_SUPERSEDED = True or TYPE_SUPERSEDED = TRUE or TYPE_SUPERSEDED = 1)";
          cmd.CommandType = CommandType.Text;
          SqliteDataReader r = cmd.ExecuteReader();
          while (r.Read()) {
            string sqlData = Convert.ToString(r["TYPE_NAME"]);
            typeNames.Add(sqlData.Split(new[] {
              " -- ",
            }, StringSplitOptions.None)[0]);
            typeNumber.Add(int.Parse(sqlData.Split(new[] {
              " -- ",
            }, StringSplitOptions.None)[1]));
          }

          db.Close();
        }
      }

      typeNames.Insert(0, "All");
      typeNumber.Insert(0, -1);
      return new Tuple<List<string>, List<int>>(typeNames, typeNumber);
    }

    public override object InitializeLifetimeService() {
      return null;
    }

    [HandleProcessCorruptedStateExceptions] // access violation
    private static void UEHandler(object sender, UnhandledExceptionEventArgs e) {
      var ex = e.ExceptionObject as Exception;

      string assemblies = AppDomain.CurrentDomain.GetAssemblies()
       .Aggregate(string.Empty, (current, ass) => current + ass + "; ");

      PostHog.Debug(new Dictionary<string, object>() {
        {
          "source", "UEHandler"
        }, {
          "exception", ex?.ToString()
        }, {
          "assemblies", assemblies
        },
      });

      throw new Exception(ex?.ToString() + "     " + assemblies);
    }
  }
}
