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
        /// Method to extract a list of catalogues from SQLite db3 file.
        /// Will output a list of strings.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<string> GetCataloguesDataFromSQLite(string filePath)
        {
            // Create empty list to work on:
            List<string> result = new List<string>();

            using (var db = Connection(filePath))
            {
                db.Open();
                SQLiteCommand cmd = db.CreateCommand();
                cmd.CommandText = @"Select CAT_NAME from Catalogues";
                cmd.CommandType = CommandType.Text;
                SQLiteDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    result.Add(Convert.ToString(r["CAT_NAME"]));
                }
                db.Close();
            }
            return result;
        }

        /// <summary>
        /// Method to extract a list of data from SQLite db3 file.
        /// Will output a list of strings.
        /// Choose type of list you want with type variable which specifies which table/data to extract;
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<string> GetTypesDataFromSQLite(string catalogue, string filePath)
        {
            // Create empty list to work on:
            List<string> result = new List<string>();

            using (var db = Connection(filePath))
            {
                db.Open();
                SQLiteCommand cmd = db.CreateCommand();
                cmd.CommandText = $"Select TYPE_NAME || ' -- ' || TYPE_ABR as TYPE_NAME from Types where TYPE_CAT_NUM = (Select CAT_NUM from Catalogues where CAT_NAME LIKE '%{catalogue}%' )";
                cmd.CommandType = CommandType.Text;
                SQLiteDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    result.Add(Convert.ToString(r["TYPE_NAME"]));
                }
                db.Close();
            }
            return result;
        }


        /// <summary>
        /// Method to extract a list of data from SQLite db3 file.
        /// Will output a list of strings.
        /// Choose type of list you want with type variable which specifies which table/data to extract;
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<string> GetSectionsDataFromSQLite(string type, string filePath)
        {
            // Create empty list to work on:
            List<string> result = new List<string>();

            using (var db = Connection(filePath))
            {
                db.Open();
                SQLiteCommand cmd = db.CreateCommand();
                cmd.CommandText = $"Select SECT_NAME from Sect where SECT_TYPE_NUM = (Select TYPE_NUM from Types where Type_Name LIKE '%{type}%'  )";
                cmd.CommandType = CommandType.Text;
                SQLiteDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    result.Add(Convert.ToString(r["SECT_NAME"]));
                }
                db.Close();
            }
            return result;
        }

    }
}
