using GsaAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GsaGH.Parameters.GsaMaterial;

namespace GsaGH.Helpers.GsaAPI
{
    internal class Mappings
    {
        internal static readonly Dictionary<string, MatType> MaterialTypeMapping = new Dictionary<string, MatType>()
    {
      { "Undefined", MatType.UNDEF },
      { "None", MatType.NONE },
      { "Generic", MatType.GENERIC },
      { "Steel", MatType.STEEL },
      { "Concrete", MatType.CONCRETE },
      { "Aluminium", MatType.ALUMINIUM },
      { "Glass", MatType.GLASS },
      { "FRP", MatType.FRP },
      { "Rebar", MatType.REBAR },
      { "Timber", MatType.TIMBER },
      { "Fabric", MatType.FABRIC },
      { "Soil", MatType.SOIL },
      { "Numeric Material", MatType.NUM_MT },
      { "Compound", MatType.COMPOUND },
      { "Bar", MatType.BAR },
      { "Tendon", MatType.TENDON },
      { "FRP Bar", MatType.FRPBAR },
      { "CFRP", MatType.CFRP },
      { "GFRP", MatType.GFRP },
      { "AFRP", MatType.AFRP },
      { "ARGFRP", MatType.ARGFRP },
      { "Bar Material", MatType.BARMAT }
    };

        internal static readonly Dictionary<string, Property2D_Type> Prop2dTypeMapping = new Dictionary<string, Property2D_Type>()
    {
      { "Undefined", Property2D_Type.UNDEF },
      { "Plane Stress", Property2D_Type.PL_STRESS },
      { "Plane Strain", Property2D_Type.PL_STRAIN },
      { "Axis Symmetric", Property2D_Type.AXISYMMETRIC },
      { "Fabric", Property2D_Type.FABRIC },
      { "Plate", Property2D_Type.PLATE },
      { "Shell", Property2D_Type.SHELL },
      { "Curved Shell", Property2D_Type.CURVED_SHELL },
      { "Torsion", Property2D_Type.TORSION },
      { "Load Panel", Property2D_Type.LOAD },
      { "Num Type", Property2D_Type.NUM_TYPE }
    };

        internal static readonly Dictionary<string, ElementType> ElementTypeMapping = new Dictionary<string, ElementType>()
    {
      { "New", ElementType.NEW },
      { "Undefined", ElementType.UNDEF },
      { "Bar", ElementType.BAR },
      { "Beam", ElementType.BEAM },
      { "Spring", ElementType.SPRING },
      { "Quad-4", ElementType.QUAD4 },
      { "Quad-8", ElementType.QUAD8 },
      { "Tri-3", ElementType.TRI3 },
      { "Tri-6", ElementType.TRI6 },
      { "Link", ElementType.LINK },
      { "Cable", ElementType.CABLE },
      { "Brick-8", ElementType.BRICK8 },
      { "Wedge-6", ElementType.WEDGE6 },
      { "Tetra-4", ElementType.TETRA4 },
      { "Spacer", ElementType.SPACER },
      { "Strut", ElementType.STRUT },
      { "Tie", ElementType.TIE },
      { "Rod", ElementType.ROD },
      { "Damper", ElementType.DAMPER },
      { "Pyramid-5", ElementType.PYRAMID5 },
      { "Last Type", ElementType.LAST_TYPE },
      { "1D", ElementType.ONE_D },
      { "2D", ElementType.TWO_D },
      { "3D", ElementType.THREE_D },
      { "1D Section", ElementType.ONE_D_SECT },
      { "2D Finite Element", ElementType.TWO_D_FE },
      { "2D Load", ElementType.TWO_D_LOAD }
    };

        internal static readonly Dictionary<string, AnalysisOrder> AnalysisOrderMapping = new Dictionary<string, AnalysisOrder>()
    {
      { "Linear", AnalysisOrder.LINEAR },
      { "Quadratic", AnalysisOrder.QUADRATIC },
      { "Rigid Diaphragm", AnalysisOrder.RIGID_DIAPHRAGM }
    };

        internal static readonly Dictionary<string, MemberType> MemberTypeMapping = new Dictionary<string, MemberType>()
    {
      { "Undefined", MemberType.UNDEF },
      { "Generic 1D", MemberType.GENERIC_1D },
      { "Generic 2D", MemberType.GENERIC_2D },
      { "Beam", MemberType.BEAM },
      { "Column", MemberType.COLUMN },
      { "Slab", MemberType.SLAB },
      { "Wall", MemberType.WALL },
      { "Cantilever", MemberType.CANTILEVER },
      { "Ribbed Slab", MemberType.RIBSLAB },
      { "Composite", MemberType.COMPOS },
      { "Pile", MemberType.PILE },
      { "Explicit", MemberType.EXPLICIT },
      { "1D Void Cutter", MemberType.VOID_CUTTER_1D },
      { "2D Void Cutter", MemberType.VOID_CUTTER_2D },
      { "Generic 3D", MemberType.GENERIC_3D },
    };
    }
}
