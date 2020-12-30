using System;
using NUnit.Framework;
using GhSA;
using GhSA.Parameters;
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

            // get the GSA install path
            string installPath = GhSA.Util.Gsa.GsaPath.GetPath;

            // open existing GSA model (steel design sample)
            ReturnValue returnValue = m.Model.Open(installPath + "\\Samples\\Steel\\Steel_Design_Simple.gwb");

            Assert.AreSame(ReturnValue.GS_OK.ToString(), returnValue.ToString());
        }

        [TestCase]
        public void TestSaveModel()
        {
            // create new GH-GSA model 
            GsaModel m = new GsaModel();

            // get the GSA install path
            string installPath = GhSA.Util.Gsa.GsaPath.GetPath;

            // open existing GSA model (steel design sample)
            m.Model.Open(installPath + "\\Samples\\Steel\\Steel_Design_Simple.gwb");

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
            string installPath = GhSA.Util.Gsa.GsaPath.GetPath;

            // open existing GSA model (steel design sample)
            ReturnValue returnValue = m.Model.Open(installPath + "\\Samples\\Steel\\Steel_Design_Simple.gwb");

            // get original GUID
            Guid originalGUID = m.GUID;

            // duplicate model
            GsaModel dup = m.Duplicate();

            // get duplicated GUID
            Guid dupGUID = dup.GUID;
            Assert.AreNotEqual(dupGUID, originalGUID);
        }
    }
}