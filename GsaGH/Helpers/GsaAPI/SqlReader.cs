using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace GsaGH.Helpers.GsaAPI
{
  /// <summary>
  /// Class containing functions to interface with SQLite db files.
  /// In case of problems loading SQLite the singleton is executed in a separate AppDomain.
  /// </summary>
  public class SqlReader : MarshalByRefObject
  {
    public static SqlReader Instance { get { return lazy.Value; } }
    private static readonly Lazy<SqlReader> lazy = new Lazy<SqlReader>(() => Initialize());

    [HandleProcessCorruptedStateExceptions] // access violation
    static void UEHandler(object sender, UnhandledExceptionEventArgs e)
    {
      var ex = e.ExceptionObject as Exception;

      string assemblies = "";
      foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
        assemblies += ass.ToString() + "; ";

      throw new Exception(ex.ToString() + "     " + assemblies);
    }

    public static SqlReader Initialize()
    {
      string codeBase = Assembly.GetCallingAssembly().CodeBase;
      UriBuilder uri = new UriBuilder(codeBase);
      string path = Uri.UnescapeDataString(uri.Path);
      try
      {
        AppDomain.CurrentDomain.UnhandledException += UEHandler;
        Assembly SQLite = Assembly.LoadFile(Path.GetDirectoryName(path) + @"\System.Data.SQLite.dll");

        return new SqlReader();
      }
      // try using a second AppDomain
      catch (Exception)
      {
        // Get the full name of the EXE assembly.
        string exeAssembly = Assembly.GetCallingAssembly().FullName;

        // Construct and initialize settings for a second AppDomain.
        AppDomainSetup ads = new AppDomainSetup();
        ads.ApplicationBase = Path.GetDirectoryName(path);
        ads.PrivateBinPath = @"x64";
        ads.DisallowBindingRedirects = false;
        ads.DisallowCodeDownload = true;
        ads.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

        // Create the second AppDomain.
        AppDomain ad = AppDomain.CreateDomain("SQLite AppDomain", null, ads);
        ad.UnhandledException += UEHandler;

        // Create an instance of MarshalbyRefType in the second AppDomain.
        // A proxy to the object is returned.
        SqlReader reader = (SqlReader)ad.CreateInstanceAndUnwrap(exeAssembly, typeof(SqlReader).FullName);
        return reader;
      }
    }

    public override object InitializeLifetimeService()
    {
      // disable the leasing and then the object is only reclaimed when the AppDomain is unloaded
      return null;
    }

    public SqlReader()
    {
    }

    /// <summary>
    /// Method to set up a SQLite Connection to a specified .db3 file.
    /// Will return a SQLite connection to the aforementioned .db3 file database.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public SQLiteConnection Connection(string filePath)
    {
      return new SQLiteConnection($"URI=file:{filePath};mode=ReadOnly");
    }

    /// <summary>
    /// Get catalogue data from SQLite file (.db3). The method returns a tuple with:
    /// Item1 = list of catalogue name (string)
    /// where first item will be "All"
    /// Item2 = list of catalogue number (int)
    /// where first item will be "-1" representing All
    /// </summary>
    /// <param name="filePath">Path to SecLib.db3</param>
    /// <returns></returns>
    public Tuple<List<string>, List<int>> GetCataloguesDataFromSQLite(string filePath)
    {

      // Create empty lists to work on:
      List<string> catNames = new List<string>();
      List<int> catNumber = new List<int>();

      using (var db = this.Connection(filePath))
      {
        db.Open();
        SQLiteCommand cmd = db.CreateCommand();
        cmd.CommandText = @"Select CAT_NAME || ' -- ' || CAT_NUM as CAT_NAME from Catalogues";

        cmd.CommandType = CommandType.Text;
        SQLiteDataReader r = cmd.ExecuteReader();
        while (r.Read())
        {
          // get data
          string sqlData = Convert.ToString(r["CAT_NAME"]);

          // split text string
          // example: British -- 2
          catNames.Add(sqlData.Split(new string[] { " -- " }, StringSplitOptions.None)[0]);
          catNumber.Add(Int32.Parse(sqlData.Split(new string[] { " -- " }, StringSplitOptions.None)[1]));
        }
        db.Close();
      }
      catNames.Insert(0, "All");
      catNumber.Insert(0, -1);
      return new Tuple<List<string>, List<int>>(catNames, catNumber);
    }

    /// <summary>
    /// Get section type data from SQLite file (.db3). The method returns a tuple with:
    /// Item1 = list of type name (string)
    /// where first item will be "All"
    /// Item2 = list of type number (int)
    /// where first item will be "-1" representing All
    /// </summary>
    /// <param name="catalogue_number">Catalogue number to get section types from. Input -1 in first item of the input list to get all types</param>
    /// <param name="filePath">Path to SecLib.db3</param>
    /// <param name="inclSuperseeded">True if you want to include superseeded items</param>
    /// <returns></returns>
    public Tuple<List<string>, List<int>> GetTypesDataFromSQLite(int catalogue_number, string filePath, bool inclSuperseeded = false)
    {
      // Create empty lists to work on:
      List<string> typeNames = new List<string>();
      List<int> typeNumber = new List<int>();

      // get Catalogue numbers if input is -1 (All catalogues)
      List<int> catNumbers = new List<int>();
      if (catalogue_number == -1)
      {
        Tuple<List<string>, List<int>> catalogueData = this.GetCataloguesDataFromSQLite(filePath);
        catNumbers = catalogueData.Item2;
        catNumbers.RemoveAt(0); // remove -1 from beginning of list
      }
      else
        catNumbers.Add(catalogue_number);

      using (var db = this.Connection(filePath))
      {
        for (int i = 0; i < catNumbers.Count; i++)
        {
          int cat = catNumbers[i];

          db.Open();
          SQLiteCommand cmd = db.CreateCommand();
          if (inclSuperseeded)
            cmd.CommandText = $"Select TYPE_NAME || ' -- ' || TYPE_NUM as TYPE_NAME from Types where TYPE_CAT_NUM = {cat}";
          else
            cmd.CommandText = $"Select TYPE_NAME || ' -- ' || TYPE_NUM as TYPE_NAME from Types where TYPE_CAT_NUM = {cat} and not (TYPE_SUPERSEDED = True or TYPE_SUPERSEDED = TRUE or TYPE_SUPERSEDED = 1)";
          cmd.CommandType = CommandType.Text;
          SQLiteDataReader r = cmd.ExecuteReader();
          while (r.Read())
          {
            // get data
            string sqlData = Convert.ToString(r["TYPE_NAME"]);

            // split text string
            // example: Universal Beams -- 51
            typeNames.Add(sqlData.Split(new string[] { " -- " }, StringSplitOptions.None)[0]);
            typeNumber.Add(Int32.Parse(sqlData.Split(new string[] { " -- " }, StringSplitOptions.None)[1]));
          }
          db.Close();
        }
      }
      typeNames.Insert(0, "All");
      typeNumber.Insert(0, -1);
      return new Tuple<List<string>, List<int>>(typeNames, typeNumber);
    }

    /// <summary>
    /// Get a list of section profile strings from SQLite file (.db3). The method returns a string that includes type abbriviation as accepted by GSA. 
    /// </summary>
    /// <param name="type_numbers">List of types to get sections from</param>
    /// <param name="filePath">Path to SecLib.db3</param>
    /// <param name="inclSuperseeded">True if you want to include superseeded items</param>
    /// <returns></returns>
    public List<string> GetSectionsDataFromSQLite(List<int> type_numbers, string filePath, bool inclSuperseeded = false)
    {
      // Create empty list to work on:
      List<string> sections = new List<string>();

      List<int> types = new List<int>();
      if (type_numbers[0] == -1)
      {
        Tuple<List<string>, List<int>> typeData = this.GetTypesDataFromSQLite(-1, filePath, inclSuperseeded);
        types = typeData.Item2;
        types.RemoveAt(0); // remove -1 from beginning of list
      }
      else
        types = type_numbers;

      using (var db = this.Connection(filePath))
      {
        // get section name
        for (int i = 0; i < types.Count; i++)
        {
          int type = types[i];
          db.Open();
          SQLiteCommand cmd = db.CreateCommand();

          if (inclSuperseeded)
            cmd.CommandText = $"Select Types.TYPE_ABR || ' ' || SECT_NAME || ' -- ' || SECT_DATE_ADDED as SECT_NAME from Sect INNER JOIN Types ON Sect.SECT_TYPE_NUM = Types.TYPE_NUM where SECT_TYPE_NUM = {type} ORDER BY SECT_AREA";
          else
            cmd.CommandText = $"Select Types.TYPE_ABR || ' ' || SECT_NAME as SECT_NAME from Sect INNER JOIN Types ON Sect.SECT_TYPE_NUM = Types.TYPE_NUM where SECT_TYPE_NUM = {type} and not (SECT_SUPERSEDED = True or SECT_SUPERSEDED = TRUE or SECT_SUPERSEDED = 1) ORDER BY SECT_AREA";

          cmd.CommandType = CommandType.Text;
          SQLiteDataReader r = cmd.ExecuteReader();
          while (r.Read())
          {
            if (inclSuperseeded)
            {
              string full = Convert.ToString(r["SECT_NAME"]);
              // BSI-IPE IPEAA80 -- 2017-09-01 00:00:00.000
              string profile = full.Split(new string[] { " -- " }, StringSplitOptions.None)[0];
              string date = full.Split(new string[] { " -- " }, StringSplitOptions.None)[1];
              date = date.Replace("-", "");
              date = date.Substring(0, 8);
              sections.Add(profile + " " + date);
            }
            else
            {
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
    /// This method will return a list of double with values in [m] units and ordered as follows:
    /// [0]: Depth
    /// [1]: Width
    /// [2]: Web THK
    /// [3]: Flange THK
    /// [4]: Root radius (only if section is not welded!)
    /// </summary>
    /// <param name="profileString"></param>
    /// <returns></returns>
    public List<double> GetCatalogueProfileValues(string profileString, string filePath)
    {
      // Create empty lists to work on:
      List<double> values = new List<double>();

      using (var db = this.Connection(filePath))
      {
        db.Open();
        SQLiteCommand cmd = db.CreateCommand();
        cmd.CommandText = $"Select " +
          $"SECT_DEPTH_DIAM || ' -- ' || " +
          $"SECT_WIDTH || ' -- ' || " +
          $"SECT_WEB_THICK || ' -- ' || " +
          $"SECT_FLG_THICK || ' -- ' || " +
          $"SECT_ROOT_RAD " +
          $"as SECT_NAME from Sect INNER JOIN Types ON Sect.SECT_TYPE_NUM = Types.TYPE_NUM where SECT_NAME = \"{profileString}\" ORDER BY SECT_DATE_ADDED";
        cmd.CommandType = CommandType.Text;
        List<string> data = new List<string>();
        SQLiteDataReader r = cmd.ExecuteReader();
        while (r.Read())
        {
          // get data
          string sqlData = Convert.ToString(r["SECT_NAME"]);

          // split text string
          // example (IPE100): 0.1 --  0.055 -- 0.0041 -- 0.0057 -- 0.007
          data.Add(sqlData);
        }
        string[] vals = data[0].Split(new string[] { " -- " }, StringSplitOptions.None);
        r.Close();

        // Welded Sections
        if (vals.Length <= 1)
        {
          cmd = db.CreateCommand();
          cmd.CommandText = $"Select " +
            $"SECT_DEPTH_DIAM || ' -- ' || " +
            $"SECT_WIDTH || ' -- ' || " +
            $"SECT_WEB_THICK || ' -- ' || " +
            $"SECT_FLG_THICK " +
            $"as SECT_NAME from Sect INNER JOIN Types ON Sect.SECT_TYPE_NUM = Types.TYPE_NUM where SECT_NAME = \"{profileString}\" ORDER BY SECT_DATE_ADDED";
          cmd.CommandType = CommandType.Text;
          data = new List<string>();
          r = cmd.ExecuteReader();
          while (r.Read())
          {
            string sqlData = Convert.ToString(r["SECT_NAME"]);
            data.Add(sqlData);
          }
          vals = data[0].Split(new string[] { " -- " }, StringSplitOptions.None);
          r.Close();
        }

        // CHS Sections
        if (vals.Length <= 1)
        {
          cmd.CommandText = $"Select " +
            $"SECT_DEPTH_DIAM || ' -- ' || " +
            $"SECT_WEB_THICK " +
            $"as SECT_NAME from Sect INNER JOIN Types ON Sect.SECT_TYPE_NUM = Types.TYPE_NUM where SECT_NAME = \"{profileString}\" ORDER BY SECT_DATE_ADDED";
          cmd.CommandType = CommandType.Text;
          data = new List<string>();
          r = cmd.ExecuteReader();
          while (r.Read())
          {
            // get data
            string sqlData = Convert.ToString(r["SECT_NAME"]);

            // split text string
            // example (CHS457x12.5): 0.457 -- 0.0125
            data.Add(sqlData);
          }
          vals = data[0].Split(new string[] { " -- " }, StringSplitOptions.None);
          r.Close();
        }
        db.Close();

        NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;

        foreach (string val in vals)
          if (val != "")
            values.Add(Convert.ToDouble(val, noComma));
      }
      return values;
    }
  }
}
