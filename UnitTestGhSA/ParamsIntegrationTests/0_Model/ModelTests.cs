using System;
using NUnit.Framework;
using GsaGH;
using GsaGH.Parameters;
using Rhino.Geometry;
using GsaAPI;

namespace ParamsIntegrationTests
{
    public class ModelTests
    {
        [TestCase]
        public void TestOpenModel()
        {
            // create new GH-GSA model 
            GsaModel m = new GsaModel();

            string tempPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            tempPath = System.IO.Path.Combine(tempPath, "Oasys", "GsaGrasshopper");
            string file = tempPath + "\\Samples\\Env.gwb";

            // open existing GSA model (steel design sample)
            ReturnValue returnValue = m.Model.Open(file);

            Assert.AreSame(ReturnValue.GS_OK.ToString(), returnValue.ToString());
        }

        [TestCase]
        public void TestSaveModel()
        {
            // create new GH-GSA model 
            GsaModel m = new GsaModel();

            // get the GSA install path
            string installPath = GsaGH.Util.Gsa.InstallationFolderPath.GetPath;

            // open existing GSA model (steel design sample)
            m.Model.Open(installPath + "\\UnitTests\\Steel_Design_Simple.gwb");

            // save file to temp location
            string tempfilename = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Oasys") + "GSA-Grasshopper_temp.gwb";
            ReturnValue returnValue = m.Model.SaveAs(tempfilename);

            Assert.AreSame(ReturnValue.GS_OK.ToString(), returnValue.ToString());
        }

        [TestCase]
        public void TestDuplicateModel()
        {
            // create new GH-GSA model 
            GsaModel m = new GsaModel();

            // get the GSA install path
            string installPath = GsaGH.Util.Gsa.InstallationFolderPath.GetPath;

            // open existing GSA model (steel design sample)
            ReturnValue returnValue = m.Model.Open(installPath + "\\Samples\\Steel\\Steel_Design_Simple.gwb");

            // get original GUID
            Guid originalGUID = m.GUID;

            // clone model
            GsaModel clone = m.Clone();

            // get clone GUID
            Guid cloneGUID = clone.GUID;
            Assert.AreNotEqual(cloneGUID, originalGUID);

            // duplicate model
            GsaModel dup = m.Duplicate();

            // get duplicate GUID
            Guid dupGUID = dup.GUID;
            Assert.AreEqual(dupGUID, originalGUID);
        }
    }
}