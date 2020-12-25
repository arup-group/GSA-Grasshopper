using System;
using NUnit.Framework;
using GhSA;
using GhSA.Parameters;
using Rhino.Geometry;
using GsaAPI;

namespace UnitTestGhSA
{
    public class ModelTests
    {
        [TestCase]
        // each test class much first initiate / load the GsaAPI using reflection
        public void InitiateAPI()
        {
            Assert.IsTrue(UnitTestGhSA.Helper.LoadRefs());
        }

        [TestCase]
        public void OpenModel()
        {
            // create new GH-GSA model 
            GsaModel m = new GsaModel();

            // get the GSA install path
            string installPath = GhSA.Util.Gsa.GsaPath.GetPath;

            // open existing GSA model (steel design sample)
            ReturnValue returnValue = m.Model.Open(installPath + "\\Samples\\Steel\\Steel_Design_Simple.gwb");

            Assert.AreSame(ReturnValue.GS_OK.ToString(), returnValue.ToString());
        }



    }
}