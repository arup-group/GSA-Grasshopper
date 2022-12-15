using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using static GsaGH.Parameters.GsaMaterial;
using static GsaGH.Parameters.GsaOffset;

namespace GsaGH.Helpers.GsaAPI
{
  internal class Mappings
  {
    internal static readonly Dictionary<string, AlignmentType> AlignmentTypeMapping = new Dictionary<string, AlignmentType>()
    {
     { "Centroid" , AlignmentType.Centroid },
     { "Top-Left", AlignmentType.Top_Left },
     { "Top-Centre" , AlignmentType.Top_Centre },
     { "Top-Center" , AlignmentType.Top_Centre },
     { "Top" , AlignmentType.Top_Centre },
     { "Top-Right" , AlignmentType.Top_Right },
     { "Mid-Left" , AlignmentType.Mid_Left },
     { "Left" , AlignmentType.Mid_Left },
     { "Mid-Right" , AlignmentType.Mid_Right },
     { "Right" , AlignmentType.Mid_Right },
     { "Bottom-Left" , AlignmentType.Bottom_Left },
     { "Bottom-Centre" , AlignmentType.Bottom_Centre },
     { "Bottom-Center" , AlignmentType.Bottom_Centre },
     { "Bottom" , AlignmentType.Bottom_Centre },
     { "Bottom-Right", AlignmentType.Bottom_Right }
    };

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

    internal static AlignmentType GetAlignmentType(string typestring)
    {
      return (AlignmentType)GetValue(typestring, typeof(AlignmentType));
    }

    internal static AnalysisOrder GetAnalysisOrder(string input)
    {
      return (AnalysisOrder)GetValue(input, typeof(AnalysisOrder));
    }

    internal static ElementType GetElementType(string typestring)
    {
      return (ElementType)GetValue(typestring, typeof(ElementType));
    }

    internal static MatType GetMatType(string typestring)
    {
      return (MatType)GetValue(typestring, typeof(MatType));
    }

    internal static MemberType GetMemberType(string typestring)
    {
      return (MemberType)GetValue(typestring, typeof(MemberType));
    }

    internal static Property2D_Type GetProperty2D_Type(string typestring)
    {
      return (Property2D_Type)GetValue(typestring, typeof(Property2D_Type));
    }

    private static Enum GetValue(string key, Type t)
    {
      List<string> types;
      int index;
      switch (t)
      {
        case Type _ when t == typeof(AlignmentType):
          types = AlignmentTypeMapping.Keys.ToList();
          index = GetIndex(key, types);
          if (index != -1)
            return AlignmentTypeMapping[types[index]];
          break;

        case Type _ when t == typeof(AnalysisOrder):
          types = AnalysisOrderMapping.Keys.ToList();
          index = GetIndex(key, types);
          if (index != -1)
            return AnalysisOrderMapping[types[index]];
          break;

        case Type _ when t == typeof(ElementType):
          types = ElementTypeMapping.Keys.ToList();
          index = GetIndex(key, types);
          if (index != -1)
            return ElementTypeMapping[types[index]];
          break;

        case Type _ when t == typeof(MatType):
          types = MaterialTypeMapping.Keys.ToList();
          index = GetIndex(key, types);
          if (index != -1)
            return MaterialTypeMapping[types[index]];
          break;

        case Type _ when t == typeof(MemberType):
          types = MemberTypeMapping.Keys.ToList();
          index = GetIndex(key, types);
          if (index != -1)
            return MemberTypeMapping[types[index]];
          break;

        case Type _ when t == typeof(Property2D_Type):
          types = Prop2dTypeMapping.Keys.ToList();
          index = GetIndex(key, types);
          if (index != -1)
            return Prop2dTypeMapping[types[index]];
          break;
      }
      throw new ArgumentException();
    }

    private static int GetIndex(string key, List<string> types)
    {
      return types.Select(v => v.ToLower().Replace(" ", string.Empty).Replace("-", string.Empty).Replace("_", string.Empty)).ToList().IndexOf(key.ToLower().Trim().Replace(" ", string.Empty).Replace("-", string.Empty).Replace("_", string.Empty));
    }
  }
}
