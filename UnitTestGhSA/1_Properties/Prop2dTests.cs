using System;
using NUnit.Framework;
using GhSA;
using GhSA.Parameters;
using Rhino.Geometry;
using GsaAPI;

namespace UnitTestGhSA
{
    public class Prop2dTests
    {
        [TestCase]
        public void TestCreateProp2d()
        {
            // create new 2D property
            GsaProp2d prop = new GsaProp2d();

            Prop2D apiProp = new Prop2D
            {
                AxisProperty = 1,
                MaterialGradeProperty = 4,
                MaterialAnalysisProperty = 42,
                MaterialType = MaterialType.GENERIC,
                Name = "mariam",
                Description = "awesome property",
                Type = Property2D_Type.LOAD
            };
            prop.Prop2d = apiProp;

            Assert.AreEqual(1, prop.Prop2d.AxisProperty);
            Assert.AreEqual(4, prop.Prop2d.MaterialGradeProperty);
            Assert.AreEqual(42, prop.Prop2d.MaterialAnalysisProperty);
            Assert.AreEqual(MaterialType.GENERIC.ToString(),
                prop.Prop2d.MaterialType.ToString());
            Assert.AreEqual("mariam", prop.Prop2d.Name);
            Assert.AreEqual("awesome property", prop.Prop2d.Description);
            Assert.AreEqual(Property2D_Type.LOAD.ToString(),
                prop.Prop2d.Type.ToString());
            Assert.AreEqual(0, prop.ID);
        }

        [TestCase]
        public void TestDuplicateProp2d()
        {
            // create new 2D property
            GsaProp2d orig = new GsaProp2d
            {
                ID = 14,
                Prop2d = new Prop2D
                {
                    AxisProperty = 0,
                    MaterialGradeProperty = 2,
                    MaterialAnalysisProperty = 13,
                    MaterialType = MaterialType.UNDEF,
                    Name = "mariam",
                    Description = "awesome property",
                    Type = Property2D_Type.SHELL
                }
            };

            // duplicate prop
            GsaProp2d dup = orig.Duplicate();

            // make some changes to original
            orig.ID = 4;
            Prop2D apiProp = new Prop2D
            {
                AxisProperty = 1,
                MaterialGradeProperty = 4,
                MaterialAnalysisProperty = 42,
                MaterialType = MaterialType.FABRIC,
                Name = "kris",
                Description = "less cool property",
                Type = Property2D_Type.CURVED_SHELL
            };
            orig.Prop2d = apiProp;

            Assert.AreEqual(0, dup.Prop2d.AxisProperty);
            Assert.AreEqual(2, dup.Prop2d.MaterialGradeProperty);
            Assert.AreEqual(13, dup.Prop2d.MaterialAnalysisProperty);
            Assert.AreEqual(MaterialType.UNDEF.ToString(),
                dup.Prop2d.MaterialType.ToString());
            Assert.AreEqual("mariam", dup.Prop2d.Name);
            Assert.AreEqual("awesome property", dup.Prop2d.Description);
            Assert.AreEqual(Property2D_Type.SHELL.ToString(),
                dup.Prop2d.Type.ToString());
            Assert.AreEqual(14, dup.ID);

            Assert.AreEqual(1, orig.Prop2d.AxisProperty);
            Assert.AreEqual(4, orig.Prop2d.MaterialGradeProperty);
            Assert.AreEqual(42, orig.Prop2d.MaterialAnalysisProperty);
            Assert.AreEqual(MaterialType.FABRIC.ToString(),
                orig.Prop2d.MaterialType.ToString());
            Assert.AreEqual("kris", orig.Prop2d.Name);
            Assert.AreEqual("less cool property", orig.Prop2d.Description);
            Assert.AreEqual(Property2D_Type.CURVED_SHELL.ToString(),
                orig.Prop2d.Type.ToString());
            Assert.AreEqual(4, orig.ID);
        }
    }
}