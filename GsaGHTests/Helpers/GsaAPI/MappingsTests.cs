using GsaAPI;
using GsaGH.Helpers.GsaApi;
using Xunit;
using static GsaGH.Parameters.GsaMaterial;

namespace GsaGHTests.Helpers.GsaAPITests
{
    [Collection("GrasshopperFixture collection")]
    public class MappingsTests
    {
        [Theory]
        [InlineData("Linear", 0)]
        [InlineData("Quadratic", 1)]
        [InlineData("Rigid Diaphragm", 2)]
        [InlineData("rIgId diAphrAgm", 2)]
        public void GetAnalysisOrderTest(string input, int expected)
        {
            // Act
            AnalysisOrder actual = Mappings.GetAnalysisOrder(input);

            // Assert
            Assert.Equal(expected, (int)actual);
        }

        [Theory]
        [InlineData("New", -1)]
        [InlineData(" New", -1)]
        [InlineData("Undefined", 0)]
        [InlineData(" Undefined  ", 0)]
        [InlineData("Bar", 1)]
        [InlineData("bar", 1)]
        [InlineData("Beam", 2)]
        [InlineData("bEAM", 2)]
        [InlineData("Spring", 3)]
        [InlineData("SPRING", 3)]
        [InlineData("Quad-4", 5)]
        [InlineData("qUaD-4", 5)]
        [InlineData("Quad-8", 6)]
        [InlineData("Quad8", 6)]
        [InlineData("Tri-3", 7)]
        [InlineData("tri3", 7)]
        [InlineData("Tri-6", 8)]
        [InlineData("TRI6", 8)]
        [InlineData("Link", 9)]
        [InlineData("Cable", 10)]
        [InlineData("Brick-8", 12)]
        [InlineData("Wedge-6", 14)]
        [InlineData("Tetra-4", 16)]
        [InlineData("Spacer", 19)]
        [InlineData("Strut", 20)]
        [InlineData("Tie", 21)]
        [InlineData("Rod", 23)]
        [InlineData("Damper", 24)]
        [InlineData("Pyramid-5", 26)]
        [InlineData("Last Type", 26)]
        [InlineData("1D", 27)]
        [InlineData("2D", 28)]
        [InlineData("3D", 29)]
        [InlineData("1D Section", 30)]
        [InlineData("2D Finite Element", 31)]
        [InlineData("2d FinitEelement", 31)]
        [InlineData("2D Load", 32)]
        [InlineData("2dLoad", 32)]
        public void GetElementTypeTest(string input, int expected)
        {
            // Act
            ElementType actual = Mappings.GetElementType(input);

            // Assert
            Assert.Equal(expected, (int)actual);
        }

        [Theory]
        [InlineData("Undefined", -2)]
        [InlineData("None", -1)]
        [InlineData("Generic", 0)]
        [InlineData("Steel", 1)]
        [InlineData("Concrete", 2)]
        [InlineData("Aluminium", 3)]
        [InlineData("Glass", 4)]
        [InlineData("FRP", 5)]
        [InlineData("Rebar", 6)]
        [InlineData("Timber", 7)]
        [InlineData("Fabric", 8)]
        [InlineData("Soil", 9)]
        [InlineData("Numeric Material", 10)]
        [InlineData("Compound", 0x100)]
        [InlineData("Bar", 0x1000)]
        [InlineData("Tendon", 4352)]
        [InlineData("FRP Bar", 4608)]
        [InlineData("CFRP", 4864)]
        [InlineData("GFRP", 5120)]
        [InlineData("AFRP", 5376)]
        [InlineData("ARGFRP", 5632)]
        [InlineData("Bar Material", 65280)]
        public void GetMatTypeTest(string input, int expected)
        {
            // Act
            MatType actual = Mappings.GetMatType(input);

            // Assert
            Assert.Equal(expected, (int)actual);
        }

        [Theory]
        [InlineData("Undefined", -1)]
        [InlineData("Generic 1D", 0)]
        [InlineData("Generic 2D", 1)]
        [InlineData("Beam", 2)]
        [InlineData("Column", 3)]
        [InlineData("Slab", 4)]
        [InlineData("Wall", 5)]
        [InlineData("Cantilever", 6)]
        [InlineData("Ribbed Slab", 7)]
        [InlineData("Composite", 8)]
        [InlineData("Pile", 9)]
        [InlineData("Explicit", 10)]
        [InlineData("1D Void Cutter", 11)]
        [InlineData("2D Void Cutter", 12)]
        [InlineData("Generic 3D", 13)]
        public void GetMemberType(string input, int expected)
        {
            // Act
            MemberType actual = Mappings.GetMemberType(input);

            // Assert
            Assert.Equal(expected, (int)actual);
        }

        [Theory]
        [InlineData("Undefined", 0)]
        [InlineData("Plane Stress", 1)]
        [InlineData("Plane Strain", 2)]
        [InlineData("Axis Symmetric", 3)]
        [InlineData("Fabric", 4)]
        [InlineData("Plate", 5)]
        [InlineData("Shell", 6)]
        [InlineData("Curved Shell", 7)]
        [InlineData("Torsion", 8)]
        [InlineData("Load Panel", 10)]
        [InlineData("Num Type", 11)]
        public void GetProperty2D_TypeTest(string input, int expected)
        {
            // Act
            Property2D_Type actual = Mappings.GetProperty2D_Type(input);

            // Assert
            Assert.Equal(expected, (int)actual);
        }
    }
}
