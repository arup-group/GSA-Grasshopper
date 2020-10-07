using System;
using System.Linq;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GhSA.Parameters;
using System.Data.SQLite;
using System.Data;

namespace GhSA.Util.Gsa
{
    /// <summary>
    /// Class containing functions to interface with SQLite db files.
    /// </summary>
    public class SqlReader
    {
        /// <summary>
        /// Method to set up a SQLite Connection to a specified .db3 file.
        /// Will return a SQLite connection to the aforementioned .db3 file database.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static SQLiteConnection Connection(string filePath)
        {
            return new SQLiteConnection($"URI=file:{filePath};mode=ReadOnly");
        }

        /// <summary>
        /// Method to extract a list of data from SQLite db3 file.
        /// Will output a list of strings.
        /// Choose type of list you want with type variable which specifies which table/data to extract;
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<string> GetListDataFromSQLite(string type, string filePath)
        {
            // Create empty list to work on:
            List<string> result = new List<string>();

            using (var db = Connection(filePath))
            {
                db.Open();
                //result = db.Query<string>("Select CAT_NAME from Catalogues").ToList();
                SQLiteCommand cmd = db.CreateCommand();
                cmd.CommandText = @"Select CAT_NAME from Catalogues";
                cmd.CommandType = CommandType.Text;
                SQLiteDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    result.Add(Convert.ToString(r["CAT_NAME"]));
                }
            }
            return result;
        }

    }
}
