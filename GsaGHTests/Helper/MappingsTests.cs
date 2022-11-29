using GsaAPI;
using GsaGH.Helpers;
using Xunit;
using static GsaGH.Parameters.GsaMaterial;

namespace GsaGHTests.Helpers
{
  [Collection("GrasshopperFixture collection")]
  public class MappingsTests
  {
    [Theory]
    [InlineData("Linear", AnalysisOrder.LINEAR)]
    [InlineData("Quadratic", AnalysisOrder.QUADRATIC)]
    [InlineData("Rigid Diaphragm", AnalysisOrder.RIGID_DIAPHRAGM)]
    [InlineData("rIgId diAphrAgm", AnalysisOrder.RIGID_DIAPHRAGM)]
    public void GetAnalysisOrderTest(string input, AnalysisOrder expected)
    {
      // Act
      AnalysisOrder actual = Mappings.GetAnalysisOrder(input);

      // Assert
      Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("New", ElementType.NEW)]
    [InlineData(" New", ElementType.NEW)]
    [InlineData("Undefined", ElementType.UNDEF)]
    [InlineData(" Undefined  ", ElementType.UNDEF)]
    [InlineData("Bar", ElementType.BAR)]
    [InlineData("bar", ElementType.BAR)]
    [InlineData("Beam", ElementType.BEAM)]
    [InlineData("bEAM", ElementType.BEAM)]
    [InlineData("Spring", ElementType.SPRING)]
    [InlineData("SPRING", ElementType.SPRING)]
    [InlineData("Quad-4", ElementType.QUAD4)]
    [InlineData("qUaD-4", ElementType.QUAD4)]
    [InlineData("Quad-8", ElementType.QUAD8)]
    [InlineData("Quad8", ElementType.QUAD8)]
    [InlineData("Tri-3", ElementType.TRI3)]
    [InlineData("tri3", ElementType.TRI3)]
    [InlineData("Tri-6", ElementType.TRI6)]
    [InlineData("TRI6", ElementType.TRI6)]
    [InlineData("Link", ElementType.LINK)]
    [InlineData("Cable", ElementType.CABLE)]
    [InlineData("Brick-8", ElementType.BRICK8)]
    [InlineData("Wedge-6", ElementType.WEDGE6)]
    [InlineData("Tetra-4", ElementType.TETRA4)]
    [InlineData("Spacer", ElementType.SPACER)]
    [InlineData("Strut", ElementType.STRUT)]
    [InlineData("Tie", ElementType.TIE)]
    [InlineData("Rod", ElementType.ROD)]
    [InlineData("Damper", ElementType.DAMPER)]
    [InlineData("Pyramid-5", ElementType.PYRAMID5)]
    [InlineData("Last Type", ElementType.LAST_TYPE)]
    [InlineData("1D", ElementType.ONE_D)]
    [InlineData("2D", ElementType.TWO_D)]
    [InlineData("3D", ElementType.THREE_D)]
    [InlineData("1D Section", ElementType.ONE_D_SECT)]
    [InlineData("2D Finite Element", ElementType.TWO_D_FE)]
    [InlineData("2d FinitEelement", ElementType.TWO_D_FE)]
    [InlineData("2D Load", ElementType.TWO_D_LOAD)]
    [InlineData("2dLoad", ElementType.TWO_D_LOAD)]
    public void GetElementTypeTest(string input, ElementType expected)
    {
      // Act
      ElementType actual = Mappings.GetElementType(input);

      // Assert
      Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Undefined", MatType.UNDEF)]
    [InlineData("None", MatType.NONE)]
    [InlineData("Generic", MatType.GENERIC)]
    [InlineData("Steel", MatType.STEEL)]
    [InlineData("Concrete", MatType.CONCRETE)]
    [InlineData("Aluminium", MatType.ALUMINIUM)]
    [InlineData("Glass", MatType.GLASS)]
    [InlineData("FRP", MatType.FRP)]
    [InlineData("Rebar", MatType.REBAR)]
    [InlineData("Timber", MatType.TIMBER)]
    [InlineData("Fabric", MatType.FABRIC)]
    [InlineData("Soil", MatType.SOIL)]
    [InlineData("Numeric Material", MatType.NUM_MT)]
    [InlineData("Compound", MatType.COMPOUND)]
    [InlineData("Bar", MatType.BAR)]
    [InlineData("Tendon", MatType.TENDON)]
    [InlineData("FRP Bar", MatType.FRPBAR)]
    [InlineData("CFRP", MatType.CFRP)]
    [InlineData("GFRP", MatType.GFRP)]
    [InlineData("AFRP", MatType.AFRP)]
    [InlineData("ARGFRP", MatType.ARGFRP)]
    [InlineData("Bar Material", MatType.BARMAT)]
    public void GetMatTypeTest(string input, MatType expected)
    {
      // Act
      MatType actual = Mappings.GetMatType(input);

      // Assert
      Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Undefined", MemberType.UNDEF)]
    [InlineData("Generic 1D", MemberType.GENERIC_1D)]
    [InlineData("Generic 2D", MemberType.GENERIC_2D)]
    [InlineData("Beam", MemberType.BEAM)]
    [InlineData("Column", MemberType.COLUMN)]
    [InlineData("Slab", MemberType.SLAB)]
    [InlineData("Wall", MemberType.WALL)]
    [InlineData("Cantilever", MemberType.CANTILEVER)]
    [InlineData("Ribbed Slab", MemberType.RIBSLAB)]
    [InlineData("Composite", MemberType.COMPOS)]
    [InlineData("Pile", MemberType.PILE)]
    [InlineData("Explicit", MemberType.EXPLICIT)]
    [InlineData("1D Void Cutter", MemberType.VOID_CUTTER_1D)]
    [InlineData("2D Void Cutter", MemberType.VOID_CUTTER_2D)]
    [InlineData("Generic 3D", MemberType.GENERIC_3D)]
    public void GetMemberTypeest(string input, MemberType expected)
    {
      // Act
      MemberType actual = Mappings.GetMemberType(input);

      // Assert
      Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Undefined", Property2D_Type.UNDEF)]
    [InlineData("Plane Stress", Property2D_Type.PL_STRESS)]
    [InlineData("Plane Strain", Property2D_Type.PL_STRAIN)]
    [InlineData("Axis Symmetric", Property2D_Type.AXISYMMETRIC)]
    [InlineData("Fabric", Property2D_Type.FABRIC)]
    [InlineData("Plate", Property2D_Type.PLATE)]
    [InlineData("Shell", Property2D_Type.SHELL)]
    [InlineData("Curved Shell", Property2D_Type.CURVED_SHELL)]
    [InlineData("Torsion", Property2D_Type.TORSION)]
    [InlineData("Load Panel", Property2D_Type.LOAD)]
    [InlineData("Num Type", Property2D_Type.NUM_TYPE)]
    public void GetProperty2D_TypeTest(string input, Property2D_Type expected)
    {
      // Act
      Property2D_Type actual = Mappings.GetProperty2D_Type(input);

      // Assert
      Assert.Equal(expected, actual);
    }
  }
}
