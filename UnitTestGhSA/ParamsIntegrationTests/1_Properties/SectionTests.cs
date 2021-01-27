using System;
using NUnit.Framework;
using GhSA;
using GhSA.Parameters;
using Rhino.Geometry;
using GsaAPI;

namespace ParamsIntegrationTests
{
    public class SectionTests
    {
        [TestCase]
        public void TestCreateSection()
        {
            // create new section
            GsaSection sect = new GsaSection();
            
            // string defining the profile
            string profile = "STD CHS 200 10";
            double myarea = Math.Round(
                Math.PI / 4 * Math.Pow(200, 2) 
                - Math.PI / 4 * Math.Pow(200-2*10, 2),
                10);
            
            // set profile in section
            sect.Section.Profile = profile;

            double area = Math.Round(
                sect.Section.Area * Math.Pow(10, 6), // unit conversion
                10);
            Assert.AreEqual(myarea, area);

            // set other properties in section
            Section apiSection = new Section
            {
                MaterialAnalysisProperty = 1,
                MaterialGradeProperty = 2,
                MaterialType = MaterialType.CONCRETE,
                Name = "mariam",
                Pool = 4,

            };
            sect.Section = apiSection;

            Assert.AreEqual(1, sect.Section.MaterialAnalysisProperty);
            Assert.AreEqual(2, sect.Section.MaterialGradeProperty);
            Assert.AreEqual(MaterialType.CONCRETE.ToString(), 
                sect.Section.MaterialType.ToString());
            Assert.AreEqual("mariam", sect.Section.Name);
            Assert.AreEqual(4, sect.Section.Pool);
        }

        [TestCase]
        public void TestCreateSectionProfile()
        {
            // string defining the profile
            string profile = "STD R 15 20";
            double myarea = 15 * 20;
            // create new section with profile and ID
            GsaSection sect = new GsaSection(profile, 15);

            double area = sect.Section.Area * Math.Pow(10, 6); // unit conversion
            Assert.AreEqual(myarea, area);
            Assert.AreEqual(15, sect.ID);
        }

        [TestCase]
        public void TestCreateGsaSectionCat()
        {
            string profile = "CAT HE HE200.B";
            GsaSection section = new GsaSection(profile);

            double area = section.Section.Area * Math.Pow(10, 6);
            Assert.AreEqual(7808.121, area);
        }

        [TestCase]
        public void TestDuplicateSection()
        {
            string profile = "CAT HE HE200.B";
            double myarea1 = 7808.121;
            GsaSection orig = new GsaSection(profile);

            // set other properties in section
            orig.Section.MaterialAnalysisProperty = 1;
            orig.Section.MaterialGradeProperty = 2;
            orig.Section.MaterialType = MaterialType.STEEL;
            orig.Section.Name = "mariam";
            orig.Section.Pool = 12;

            // duplicate original
            GsaSection dup = orig.Duplicate();

            // make some changes to original
            string profile2 = "STD%R%15%20";
            double myarea2 = 15 * 20;
            orig.Section.Profile = profile2;
            orig.Section.MaterialAnalysisProperty = 4;
            orig.Section.MaterialGradeProperty = 6;
            orig.Section.MaterialType = MaterialType.TIMBER;
            orig.Section.Name = "kris";
            orig.Section.Pool = 99;

            double area2 = orig.Section.Area * Math.Pow(10, 6);
            Assert.AreEqual(profile2, orig.Section.Profile);
            Assert.AreEqual(myarea2, area2);

            double area1 = dup.Section.Area * Math.Pow(10, 6);
            Assert.AreEqual(profile, dup.Section.Profile);
            Assert.AreEqual(myarea1, area1);

            Assert.AreEqual(1, dup.Section.MaterialAnalysisProperty);
            Assert.AreEqual(2, dup.Section.MaterialGradeProperty);
            Assert.AreEqual(MaterialType.STEEL.ToString(),
                dup.Section.MaterialType.ToString());
            Assert.AreEqual("mariam", dup.Section.Name);
            Assert.AreEqual(12, dup.Section.Pool);

            Assert.AreEqual(4, orig.Section.MaterialAnalysisProperty);
            Assert.AreEqual(6, orig.Section.MaterialGradeProperty);
            Assert.AreEqual(MaterialType.TIMBER.ToString(),
                orig.Section.MaterialType.ToString());
            Assert.AreEqual("kris", orig.Section.Name);
            Assert.AreEqual(99, orig.Section.Pool);
        }

        [TestCase]
        public void TestDuplicateEmptySection()
        {
            GsaSection section = new GsaSection();

            GsaSection dup = section.Duplicate();
            Assert.IsNotNull(dup);
        }
    }
}