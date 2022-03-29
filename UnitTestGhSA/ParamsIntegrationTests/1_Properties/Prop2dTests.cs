using System;
using NUnit.Framework;
using GsaGH;
using GsaGH.Parameters;
using Rhino.Geometry;
using GsaAPI;

namespace ParamsIntegrationTests
{
    public class Prop2dTests
    {
        [TestCase]
        public void TestCreateProp2d()
        {
            // create new api property
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

            // create new 2D property
            GsaProp2d prop = new GsaProp2d
            {
                AxisProperty = apiProp.AxisProperty,
                Name = apiProp.Name,
                Description = apiProp.Description,
                Type = apiProp.Type
            };
            GsaMaterial mat = new GsaMaterial((int)apiProp.MaterialType)
            {
                AnalysisProperty = apiProp.MaterialAnalysisProperty,
                GradeProperty = apiProp.MaterialGradeProperty,
            };
            prop.Material = mat;

            Assert.AreEqual(-1, prop.AxisProperty);
            Assert.AreEqual(4, prop.Material.GradeProperty);
            Assert.AreEqual(42, prop.Material.AnalysisProperty);
            Assert.AreEqual(MaterialType.GENERIC.ToString(),
                prop.Material.MaterialType.ToString());
            Assert.AreEqual("mariam", prop.Name);
            Assert.AreEqual("awesome property", prop.Description);
            Assert.AreEqual(Property2D_Type.LOAD.ToString(),
                prop.Type.ToString());
            Assert.AreEqual(0, prop.ID);
        }

        [TestCase]
        public void TestDuplicateProp2d()
        {
            Prop2D apiPropOriginal = new Prop2D
            {
                AxisProperty = 0,
                MaterialGradeProperty = 2,
                MaterialAnalysisProperty = 13,
                MaterialType = MaterialType.UNDEF,
                Name = "mariam",
                Description = "awesome property",
                Type = Property2D_Type.SHELL
            };

            // create new 2D property
            GsaProp2d orig = new GsaProp2d(14)
            {
                AxisProperty = apiPropOriginal.AxisProperty,
                Name = apiPropOriginal.Name,
                Description = apiPropOriginal.Description,
                Type = apiPropOriginal.Type
            };
            GsaMaterial mat = new GsaMaterial((int)apiPropOriginal.MaterialType)
            {
                AnalysisProperty = apiPropOriginal.MaterialAnalysisProperty,
                GradeProperty = apiPropOriginal.MaterialGradeProperty,
            };
            orig.Material = mat;


            // duplicate prop
            GsaProp2d dup = orig.Duplicate();

            // make some changes to original
            orig.ID = 4;

            orig.AxisProperty = 1;
            orig.Material.GradeProperty = 4;
            orig.Material.AnalysisProperty = 42;
            orig.Material.MaterialType = GsaMaterial.MatType.FABRIC;
            orig.Name = "kris";
            orig.Description = "less cool property";
            orig.Type = Property2D_Type.CURVED_SHELL;

            Assert.AreEqual(0, dup.AxisProperty);
            Assert.AreEqual(2, dup.Material.GradeProperty);
            Assert.AreEqual(13, dup.Material.AnalysisProperty);
            Assert.AreEqual(MaterialType.UNDEF.ToString(),
                dup.Material.MaterialType.ToString());
            Assert.AreEqual("mariam", dup.Name);
            Assert.AreEqual("awesome property", dup.Description);
            Assert.AreEqual(Property2D_Type.SHELL.ToString(),
                dup.Type.ToString());
            Assert.AreEqual(14, dup.ID);

            Assert.AreEqual(-1, orig.AxisProperty);
            Assert.AreEqual(4, orig.Material.GradeProperty);
            Assert.AreEqual(42, orig.Material.AnalysisProperty);
            Assert.AreEqual(MaterialType.FABRIC.ToString(),
                orig.Material.MaterialType.ToString());
            Assert.AreEqual("kris", orig.Name);
            Assert.AreEqual("less cool property", orig.Description);
            Assert.AreEqual(Property2D_Type.CURVED_SHELL.ToString(),
                orig.Type.ToString());
            Assert.AreEqual(4, orig.ID);
        }
    }
}