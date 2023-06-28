using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.GsaApi.EnumMappings;
using static GsaGH.Parameters.GsaMaterial;
using static GsaGH.Parameters.GsaOffset;
using Diagram = GsaGH.Parameters.Enums;

namespace GsaGH.Helpers.GsaApi {
  internal class Mappings {

    internal static readonly Dictionary<string, AlignmentType> alignmentTypeMapping
      = new Dictionary<string, AlignmentType>() {
        {
          "Centroid", AlignmentType.Centroid
        }, {
          "Top-Left", AlignmentType.TopLeft
        }, {
          "Top-Centre", AlignmentType.TopCentre
        }, {
          "Top-Center", AlignmentType.TopCentre
        }, {
          "Top", AlignmentType.TopCentre
        }, {
          "Top-Right", AlignmentType.TopRight
        }, {
          "Mid-Left", AlignmentType.MidLeft
        }, {
          "Left", AlignmentType.MidLeft
        }, {
          "Mid-Right", AlignmentType.MidRight
        }, {
          "Right", AlignmentType.MidRight
        }, {
          "Bottom-Left", AlignmentType.BottomLeft
        }, {
          "Bottom-Centre", AlignmentType.BottomCentre
        }, {
          "Bottom-Center", AlignmentType.BottomCentre
        }, {
          "Bottom", AlignmentType.BottomCentre
        }, {
          "Bottom-Right", AlignmentType.BottomRight
        },
      };

    internal static readonly Dictionary<string, AnalysisOrder> analysisOrderMapping
      = new Dictionary<string, AnalysisOrder>() {
        {
          "Linear", AnalysisOrder.LINEAR
        }, {
          "Quadratic", AnalysisOrder.QUADRATIC
        }, {
          "Rigid Diaphragm", AnalysisOrder.RIGID_DIAPHRAGM
        },
      };
    internal static readonly Dictionary<string, ElementType> elementTypeMapping
      = new Dictionary<string, ElementType>() {
        {
          "New", ElementType.NEW
        }, {
          "Undefined", ElementType.UNDEF
        }, {
          "Bar", ElementType.BAR
        }, {
          "Beam", ElementType.BEAM
        }, {
          "Spring", ElementType.SPRING
        }, {
          "Quad-4", ElementType.QUAD4
        }, {
          "Quad-8", ElementType.QUAD8
        }, {
          "Tri-3", ElementType.TRI3
        }, {
          "Tri-6", ElementType.TRI6
        }, {
          "Link", ElementType.LINK
        }, {
          "Cable", ElementType.CABLE
        }, {
          "Brick-8", ElementType.BRICK8
        }, {
          "Wedge-6", ElementType.WEDGE6
        }, {
          "Tetra-4", ElementType.TETRA4
        }, {
          "Spacer", ElementType.SPACER
        }, {
          "Strut", ElementType.STRUT
        }, {
          "Tie", ElementType.TIE
        }, {
          "Rod", ElementType.ROD
        }, {
          "Damper", ElementType.DAMPER
        }, {
          "Pyramid-5", ElementType.PYRAMID5
        }, {
          "Last Type", ElementType.LAST_TYPE
        }, {
          "1D", ElementType.ONE_D
        }, {
          "2D", ElementType.TWO_D
        }, {
          "3D", ElementType.THREE_D
        }, {
          "1D Section", ElementType.ONE_D_SECT
        }, {
          "2D Finite Element", ElementType.TWO_D_FE
        }, {
          "2D Load", ElementType.TWO_D_LOAD
        },
      };
    internal static readonly Dictionary<string, MatType> materialTypeMapping
      = new Dictionary<string, MatType>() {
        {
          "Generic", MatType.Generic
        }, {
          "Steel", MatType.Steel
        }, {
          "Concrete", MatType.Concrete
        }, {
          "Aluminium", MatType.Aluminium
        }, {
          "Glass", MatType.Glass
        }, {
          "FRP", MatType.Frp
        }, {
          "Timber", MatType.Timber
        }, {
          "Fabric", MatType.Fabric
        },
      };

    internal static readonly Dictionary<string, MemberType> memberTypeMapping
      = new Dictionary<string, MemberType>() {
        {
          "Undefined", MemberType.UNDEF
        }, {
          "Generic 1D", MemberType.GENERIC_1D
        }, {
          "Generic 2D", MemberType.GENERIC_2D
        }, {
          "Beam", MemberType.BEAM
        }, {
          "Column", MemberType.COLUMN
        }, {
          "Slab", MemberType.SLAB
        }, {
          "Wall", MemberType.WALL
        }, {
          "Cantilever", MemberType.CANTILEVER
        }, {
          "Ribbed Slab", MemberType.RIBSLAB
        }, {
          "Composite", MemberType.COMPOS
        }, {
          "Pile", MemberType.PILE
        }, {
          "Explicit", MemberType.EXPLICIT
        }, {
          "1D Void Cutter", MemberType.VOID_CUTTER_1D
        }, {
          "2D Void Cutter", MemberType.VOID_CUTTER_2D
        }, {
          "Generic 3D", MemberType.GENERIC_3D
        },
      };
    internal static readonly Dictionary<string, Property2D_Type> prop2dTypeMapping
      = new Dictionary<string, Property2D_Type>() {
        {
          "Undefined", Property2D_Type.UNDEF
        }, {
          "Plane Stress", Property2D_Type.PL_STRESS
        }, {
          "Plane Strain", Property2D_Type.PL_STRAIN
        }, {
          "Axis Symmetric", Property2D_Type.AXISYMMETRIC
        }, {
          "Fabric", Property2D_Type.FABRIC
        }, {
          "Plate", Property2D_Type.PLATE
        }, {
          "Shell", Property2D_Type.SHELL
        }, {
          "Curved Shell", Property2D_Type.CURVED_SHELL
        }, {
          "Torsion", Property2D_Type.TORSION
        }, {
          "Load Panel", Property2D_Type.LOAD
        }, {
          "Num Type", Property2D_Type.NUM_TYPE
        },
      };

    internal static readonly IList<DiagramTypeMapping> diagramTypeMappingForce
      = new List<DiagramTypeMapping>() {
        new DiagramTypeMapping("Axial Fx", DiagramType.AxialForceFx,
          Diagram.DiagramType.AxialForceFx),
        new DiagramTypeMapping("Shear Fy", DiagramType.ShearForceFy,
          Diagram.DiagramType.ShearForceFy),
        new DiagramTypeMapping("Shear Fz", DiagramType.ShearForceFz,
          Diagram.DiagramType.ShearForceFz),
        new DiagramTypeMapping("Res. Shear |Fyz|", DiagramType.ResolvedShearFyz,
          Diagram.DiagramType.ResolvedShearFyz),
        new DiagramTypeMapping("Torsion Mxx", DiagramType.TorsionMxx,
          Diagram.DiagramType.TorsionMxx),
        new DiagramTypeMapping("Moment Myy", DiagramType.MomentMyy, Diagram.DiagramType.MomentMyy),
        new DiagramTypeMapping("Moment Mzz", DiagramType.MomentMzz, Diagram.DiagramType.MomentMzz),
        new DiagramTypeMapping("Res. Moment |Myz|", DiagramType.ResolvedMomentMyz,
          Diagram.DiagramType.ResolvedMomentMyz), };

    internal static readonly IList<DiagramTypeMapping> diagramTypeMappingStress
      = new List<DiagramTypeMapping>() {
        new DiagramTypeMapping("Axial A", DiagramType.AxialStressA,
          Diagram.DiagramType.AxialStressA),
        new DiagramTypeMapping("Shear Sy", DiagramType.ShearStressSy,
          Diagram.DiagramType.ShearStressSy),
        new DiagramTypeMapping("Shear Sz", DiagramType.ShearStressSz,
          Diagram.DiagramType.ShearStressSz),
        new DiagramTypeMapping("Bending By +z", DiagramType.BendingStressByPositiveZ,
          Diagram.DiagramType.BendingStressByPositiveZ),
        new DiagramTypeMapping("Bending By -z", DiagramType.BendingStressByNegativeZ,
          Diagram.DiagramType.BendingStressByNegativeZ),
        new DiagramTypeMapping("Bending Bz +y", DiagramType.BendingStressBzPositiveY,
          Diagram.DiagramType.BendingStressBzPositiveY),
        new DiagramTypeMapping("Bending Bz -y", DiagramType.BendingStressBzNegativeY,
          Diagram.DiagramType.BendingStressBzNegativeY),
        new DiagramTypeMapping("Combined C1", DiagramType.CombinedStressC1,
          Diagram.DiagramType.CombinedStressC1),
        new DiagramTypeMapping("Combined C2", DiagramType.CombinedStressC2,
          Diagram.DiagramType.CombinedStressC2), };

    internal static AlignmentType GetAlignmentType(string typestring) {
      return (AlignmentType)GetValue(typestring, typeof(AlignmentType));
    }

    internal static AnalysisOrder GetAnalysisOrder(string input) {
      return (AnalysisOrder)GetValue(input, typeof(AnalysisOrder));
    }

    internal static ElementType GetElementType(string typestring) {
      return (ElementType)GetValue(typestring, typeof(ElementType));
    }

    internal static MatType GetMatType(string typestring) {
      return (MatType)GetValue(typestring, typeof(MatType));
    }

    internal static MemberType GetMemberType(string typestring) {
      return (MemberType)GetValue(typestring, typeof(MemberType));
    }

    internal static Property2D_Type GetProperty2D_Type(string typestring) {
      return (Property2D_Type)GetValue(typestring, typeof(Property2D_Type));
    }

    private static int GetIndex(string key, List<string> types) {
      return types
       .Select(v => v.ToLower().Replace(" ", string.Empty).Replace("-", string.Empty)
         .Replace("_", string.Empty)).ToList().IndexOf(key.ToLower().Trim()
         .Replace(" ", string.Empty).Replace("-", string.Empty).Replace("_", string.Empty));
    }

    private static Enum GetValue(string key, Type t) {
      List<string> types;
      int index;
      switch (t) {
        case Type _ when t == typeof(AlignmentType):
          types = alignmentTypeMapping.Keys.ToList();
          index = GetIndex(key, types);
          if (index != -1) {
            return alignmentTypeMapping[types[index]];
          }

          break;

        case Type _ when t == typeof(AnalysisOrder):
          types = analysisOrderMapping.Keys.ToList();
          index = GetIndex(key, types);
          if (index != -1) {
            return analysisOrderMapping[types[index]];
          }

          break;

        case Type _ when t == typeof(ElementType):
          types = elementTypeMapping.Keys.ToList();
          index = GetIndex(key, types);
          if (index != -1) {
            return elementTypeMapping[types[index]];
          }

          break;

        case Type _ when t == typeof(MatType):
          types = materialTypeMapping.Keys.ToList();
          index = GetIndex(key, types);
          if (index != -1) {
            return materialTypeMapping[types[index]];
          }

          break;

        case Type _ when t == typeof(MemberType):
          types = memberTypeMapping.Keys.ToList();
          index = GetIndex(key, types);
          if (index != -1) {
            return memberTypeMapping[types[index]];
          }

          break;

        case Type _ when t == typeof(Property2D_Type):
          types = prop2dTypeMapping.Keys.ToList();
          index = GetIndex(key, types);
          if (index != -1) {
            return prop2dTypeMapping[types[index]];
          }

          break;
      }

      throw new ArgumentException();
    }
  }
}
