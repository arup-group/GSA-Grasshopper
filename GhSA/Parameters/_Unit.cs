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
    public static class GsaUnit
    {
        

        private static bool setLength_Large = false;
        public static string LengthLarge
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
        public static string LengthSmall
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
        public static string LengthSection
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

        public static string Force
        {
            get { return m_force; }
            set { m_force = StringTestForce(value); }
        }
        public static string Mass
        {
            get { return m_mass; }
            set { m_mass = StringTestMass(value); }
        }
        public static string Temperature
        {
            get { return m_temperature; }
            set { m_temperature = StringTestTemperature(value); }
        }
        public static string Stress
        {
            get { return m_stress; }
            set { m_stress = StringTestStress(value); }
        }
        public static string Strain
        {
            get { return m_strain; }
            set { m_strain = StringTestStrain(value); }
        }
        public static string Velocity
        {
            get { return m_velocity; }
            set { m_velocity = StringTestVelocity(value); }
        }
        public static string Acceleration
        {
            get { return m_acceleration; }
            set { m_acceleration = StringTestAcceleration(value); }
        }
        public static string Energy
        {
            get { return m_energy; }
            set { m_energy = StringTestEnergy(value); }
        }
        public static string Angle
        {
            get { return m_angle; }
            set { m_angle = StringTestAngle(value); }
        }
        public static string TimeShort
        {
            get { return m_time_short; }
            set { m_time_short = StringTestTime(value, 0); }
        }
        public static string TimeMedium
        {
            get { return m_time_medium; }
            set { m_time_medium = StringTestTime(value, 1); }
        }
        public static string TimeLong
        {
            get { return m_time_long; }
            set { m_time_long = StringTestTime(value, 2); }
        }
        #region fields
        private static string m_force = "kN";
        private static string m_length_L = "m";
        private static string m_length_S = "mm";
        private static string m_length_section = "mm";
        private static string m_mass = "t";
        private static string m_temperature = "°C";
        private static string m_stress = "N/mm\xB2";
        private static string m_strain = "mε";
        private static string m_velocity = "m/s";
        private static string m_acceleration = "m/s\xB2";
        private static string m_energy = "MJ";
        private static string m_angle = "degree";
        private static string m_time_short = "s";
        private static string m_time_medium = "min";
        private static string m_time_long = "day";
        
        private static string m_RhinoUnitName = "";
        private static double m_ConversionToMeter;
        
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

        private static string StringTestForce(string unitname)
        {
            unitname = unitname.ToLower();
            if (unitname.Contains("kn") | unitname.Contains("kilon") | unitname.Contains("kilo n"))
                return "kN";
            if (unitname.Contains("mn") | unitname.Contains("megan") | unitname.Contains("mega n"))
                return "MN";
            if (unitname.Contains("lbf") | unitname.Contains("pound"))
                return "lbf";
            if (unitname.Contains("kpf") | unitname.Contains("kilop") | unitname.Contains("kilo p"))
                return "kpf";
            if (unitname.Contains("tf") | unitname.Contains("tonf") | unitname.Contains("ton f"))
                return "tf";
            if (unitname.Contains("n"))
                return "N";
            return "kN";
        }
        private static string StringTestMass(string unitname)
        {
            unitname = unitname.ToLower();
            if (unitname.Contains("kg") | unitname.Contains("kilog") | unitname.Contains("kilo g"))
                return "kg";
            if (unitname.Contains("t") | unitname.Contains("ton"))
                return "t";
            if (unitname.Contains("kt") | unitname.Contains("kilot") | unitname.Contains("kilo t"))
                return "kt";
            if (unitname.Contains("the long t"))
                return "Ton";
            if (unitname.Contains("slug"))
                return "slug";
            if (unitname.Contains("kip.s2/in") | unitname.Contains("kip s2/in") | unitname.Contains("kip s^2/in") | unitname.Contains("kip.s^2/in") | unitname.Contains("kip*s2/in") | unitname.Contains("kip*s^2/in"))
                return "kip·s\xB2/in";
            if (unitname.Contains("kip.s2/ft") | unitname.Contains("kip s2/ft") | unitname.Contains("kip s^2/ft") | unitname.Contains("kip.s^2/ft") | unitname.Contains("kip*s2/ft") | unitname.Contains("kip*s^2/ft"))
                return "kip·s\xB2/ft";
            if (unitname.Contains("lbf.s2/in") | unitname.Contains("lbf s2/in") | unitname.Contains("lbf s^2/in") | unitname.Contains("lbf.s^2/in") | unitname.Contains("lbf*s2/in") | unitname.Contains("lbf*s^2/in"))
                return "lbf·s\xB2/in";
            if (unitname.Contains("lbf.s2/ft") | unitname.Contains("lbf s2/ft") | unitname.Contains("lbf s^2/ft") | unitname.Contains("lbf.s^2/ft") | unitname.Contains("lbf*s2/ft") | unitname.Contains("lbf*s^2/ft"))
                return "lbf·s\xB2/ft";
            if (unitname.Contains("kip") | unitname.Contains("kilo p") | unitname.Contains("kilop"))
                return "kip";
            if (unitname.Contains("lb") | unitname.Contains("pound"))
                return "kpf";
            if (unitname.Contains("g"))
                return "g";
            return "t";
        }
        private static string StringTestTemperature(string unitname)
        {
            unitname = unitname.ToLower();
            if (unitname.Contains("c"))
                return "°C";
            if (unitname.Contains("k"))
                return "K";
            if (unitname.Contains("f"))
                return "°F";
            return "°C";
        }
        private static string StringTestStress(string unitname)
        {
            unitname = unitname.ToLower();
            if (unitname.Contains("kpa"))
                return "kPa";
            if (unitname.Contains("mpa"))
                return "MPa";
            if (unitname.Contains("gpa"))
                return "GPa";
            if (unitname.Contains("pa"))
                return "Pa";
            if (unitname.Contains("n/mm"))
                return "N/mm\xB2";
            if (unitname.Contains("n/m"))
                return "N/m\xB2";
            if (unitname.Contains("kip"))
                return "kip/in\xB2";
            if (unitname.Contains("psi"))
                return "psi";
            if (unitname.Contains("psf"))
                return "psf";
            if (unitname.Contains("ksi"))
                return "ksi";
            return "N/mm\xB2";
        }
        private static string StringTestStrain(string unitname)
        {
            unitname = unitname.ToLower();
            if (unitname.Contains("percentage e") | unitname.Contains("percentage ε") | unitname.Contains("percent e") | unitname.Contains("percent ε") | unitname.Contains("% e") | unitname.Contains("% ε") | unitname.Contains("%e") | unitname.Contains("%ε"))
                return "%ε";
            if (unitname.Contains("meter e") | unitname.Contains("meter ε") | unitname.Contains("m e") | unitname.Contains("m ε") | unitname.Contains("me") | unitname.Contains("mε"))
                return "mε";
            if (unitname.Contains("my") | unitname.Contains("mu") | unitname.Contains("mju") | unitname.Contains("mic") | unitname.Contains("µ"))
                return "µε";
            if (unitname.Contains("percentage") | unitname.Contains("percent") | unitname.Contains("%"))
                return "%";
            if (unitname.Contains("pro") | unitname.Contains("‰"))
                return "‰";
            if (unitname.Contains("e") | unitname.Contains("ε"))
                return "ε";
            return "ε";
        }
        private static string StringTestVelocity(string unitname)
        {
            unitname = unitname.ToLower();
            if (unitname.Contains("mm/s"))
                return "mm/s";
            if (unitname.Contains("cm/s"))
                return "cm/s";
            if (unitname.Contains("m/s"))
                return "m/s";
            if (unitname.Contains("ft/s"))
                return "ft/s";
            if (unitname.Contains("in/s"))
                return "in/s";
            if (unitname.Contains("km/h"))
                return "km/h";
            if (unitname.Contains("mph"))
                return "mph";
            return "m/s";
        }
        private static string StringTestAcceleration(string unitname)
        {
            unitname = unitname.ToLower();
            if (unitname.Contains("mm/s"))
                return "mm/s\xB2";
            if (unitname.Contains("cm/s"))
                return "cm/s\xB2";
            if (unitname.Contains("m/s"))
                return "m/s\xB2";
            if (unitname.Contains("ft/s"))
                return "ft/s\xB2";
            if (unitname.Contains("in/s"))
                return "in/s\xB2";
            if (unitname.Contains("%g"))
                return "%g";
            if (unitname.Contains("mil"))
                return "milli-g";
            if (unitname.Contains("g"))
                return "g";
            return "g";
        }
        private static string StringTestEnergy(string unitname)
        {
            unitname = unitname.ToLower();
            if (unitname.Contains("gj") | unitname.Contains("giga"))
                return "GJ";
            if (unitname.Contains("kj") | unitname.Contains("kilo"))
                return "KJ";
            if (unitname.Contains("mj") | unitname.Contains("mega"))
                return "MJ";
            if (unitname.Contains("j"))
                return "J";
            if (unitname.Contains("in"))
                return "in·lbf";
            if (unitname.Contains("ft"))
                return "ft·lbf";
            if (unitname.Contains("cal"))
                return "cal";
            if (unitname.Contains("btu"))
                return "Btu";
            return "MJ";
        }
        private static string StringTestAngle(string unitname)
        {
            unitname = unitname.ToLower();
            if (unitname.Contains("deg") | unitname.Contains("°"))
                return "Degree";
            if (unitname.Contains("grad"))
                return "Gradian";
            if (unitname.Contains("rad"))
                return "Radian";
            return "Degree";
        }
        private static string StringTestTime(string unitname, int default_out = -1)
        {
            unitname = unitname.ToLower();
            if (unitname.Contains("s"))
                return "s";
            if (unitname.Contains("m"))
                return "min";
            if (unitname.Contains("h"))
                return "hour";
            if (unitname.Contains("d"))
                return "day";
            if (unitname.Contains("y"))
                return "year";
            if (default_out == 0)
                return "s";
            if (default_out == 1)
                return "min";
            if (default_out == 2)
                return "day";
            return "s";
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
