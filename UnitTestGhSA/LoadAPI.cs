using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GhSA;
using NUnit.Framework;

namespace UnitTestGhSA
{
    public class Helper
    {
        public static void LoadAPI()
        {
            string installPath = GhSA.Util.Gsa.GsaPath.GetPath;
            Assembly ass1 = Assembly.LoadFile(installPath + "\\GsaAPI.dll");
            Assembly ass2 = Assembly.LoadFile(installPath + "\\System.Data.SQLite.dll");
        }
    }
}
