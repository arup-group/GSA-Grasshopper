using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;

namespace GhSA.Util
{
    /// <summary>
    /// Class to hold units used in Grasshopper GSA file. 
    /// </summary>
    public static class Unit
    {
        
        
        private static bool setLength_Large = false;
        public static string Length_Large
        {
            get
            {
                if (!setLength_Large)
                {
                    if (RhinoDoc.ActiveDoc.ModelUnitSystem.GetHashCode() == 2 || RhinoDoc.ActiveDoc.ModelUnitSystem.GetHashCode() == 3
                        || RhinoDoc.ActiveDoc.ModelUnitSystem.GetHashCode() == 4 || RhinoDoc.ActiveDoc.ModelUnitSystem.GetHashCode() == 8 ||
                        RhinoDoc.ActiveDoc.ModelUnitSystem.GetHashCode() == 9)
                    {
                        m_length_L = RhinoUnitName(RhinoDoc.ActiveDoc.ModelUnitSystem);
                    }
                    // add convertion to meters if odd unit?
                }
                return m_length_L;
            }
            set 
            {
                setLength_Large = true;
                m_length_L = StringTestLength(value); 
            }
        }
        public static string Length_Small
        {
            get
            {
                if (m_length_S == "")
                {
                    if (RhinoDoc.ActiveDoc.ModelUnitSystem.GetHashCode() == 2 || RhinoDoc.ActiveDoc.ModelUnitSystem.GetHashCode() == 3
                        || RhinoDoc.ActiveDoc.ModelUnitSystem.GetHashCode() == 4 || RhinoDoc.ActiveDoc.ModelUnitSystem.GetHashCode() == 8 ||
                        RhinoDoc.ActiveDoc.ModelUnitSystem.GetHashCode() == 9)
                    {
                        m_length_S = RhinoUnitName(RhinoDoc.ActiveDoc.ModelUnitSystem);
                    }
                    // add convertion to meters if odd unit?
                }
                return m_length_S;
            }
            set { m_length_S = StringTestLength(value); }
        }
        public static string Length_Section
        {
            get { return m_length_section; }
            set { m_length_section = StringTestLength(value); }
        }
        public static string RhinoDocUnit
        {
            get
            {
                m_RhinoUnitName = RhinoUnitName(RhinoDoc.ActiveDoc.ModelUnitSystem);
                return m_RhinoUnitName;
            }
        }

        public static double RhinoDocFactorToMeter
        {
            get
            {
                m_ConversionToMeter = Rhino2meter(RhinoDoc.ActiveDoc.ModelUnitSystem);
                return m_ConversionToMeter;
            }
        }

        #region fields
        private static string m_length_L = "";
        private static string m_length_S = "";
        private static string m_length_section = "mm";
        private static string m_RhinoUnitName = "";
        private static double m_ConversionToMeter;
        // other units to be added
        #endregion
        /// <summary>
        /// Method to convert bad strings to accepted inputs
        /// if no match is found "m" is returned
        /// </summary>
        /// <param name="unitname"></param>
        /// <returns></returns>
        private static string StringTestLength(string unitname)
        {
            unitname = unitname.ToLower();
            if (unitname.Contains("mm") | unitname.Contains("milimet") | unitname.Contains("millimet"))
                return "mm";
            if (unitname.Contains("cm") | unitname.Contains("centimet"))
                return "cm";
            if (unitname.Contains("m"))
                return "m";
            if (unitname.Contains("in") | unitname.Contains("\""))
                return "in";
            if (unitname.Contains("ft") | unitname.Contains("feet") | unitname.Contains("foot") | unitname.Contains("′") | unitname.Contains("'"))
                return "ft";
            return "m";
        }

        /// <summary>
        /// Method to get the scaling factor from Rhino document units to meter
        /// 
        /// If Rhino units is meter then factor is 1. 
        /// 
        /// This method can handle all documented Rhino units, so this method can be used to convert into a known GSA unit
        /// </summary>
        /// <param name="rhinoUnits"></param>
        /// <returns></returns>
        private static double Rhino2meter(UnitSystem rhinoUnits)
        {
            List<int> id = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 });
            List<double> convertFact = new List<double>(new double[] { 1, 0.000001, 0.001, 0.01, 1.0, 1000.0, 0.0000000254,
                0.0000254, 0.0254, 0.305, 1610.0, 0, 0.0000000001, 0.000000001, 0.1, 10.0, 100.0, 1000000.0, 1000000000.0, 0.914});
            for (int i = 0; i < id.Count; i++)
                if (rhinoUnits.GetHashCode() == id[i])
                    return convertFact[i];
            return 0;
        }
        /// <summary>
        /// Method to retrieve current Rhino document unit name.
        /// 
        /// Note that names have been changed to suit GSA unit naming convetion.
        /// - Rhino unit "Meters" -> GSA unit "m"
        /// 
        /// Method will return GSA readable unit if of know type
        /// (mm, cm, m, in or ft)
        /// </summary>
        /// <param name="rhinoUnits"></param>
        /// <returns></returns>
        private static string RhinoUnitName(UnitSystem rhinoUnits)
        {
            List<int> id = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 });
            List<string> name = new List<string>(new string[] {"None", "Microns", "mm", "cm", "m", "km",
                "Microinches", "Mils", "in", "ft", "Miles", " ", "Angstroms", "Nanometers", "Decimeters", "Dekameters",
                "Hectometers", "Megameters", "Gigameters", "Yards" });
            for (int i = 0; i < id.Count; i++)
                if (rhinoUnits.GetHashCode() == id[i])
                    return name[i];
            return "";
        }
    }
}
