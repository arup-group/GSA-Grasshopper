using System;
using System.Collections.Generic;
using System.Linq;

using GsaAPI;

using GsaGH.Helpers.GsaApi.EnumMappings;
using GsaGH.Parameters;

using AlignmentType = GsaGH.Parameters.AlignmentType;
using DiagramType = GsaAPI.DiagramType;

namespace GsaGH.Helpers.GsaApi {
  internal static class Mappings {
    internal static readonly Dictionary<string, AlignmentType> _alignmentTypeMapping
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

    internal static readonly Dictionary<string, AnalysisOrder> _analysisOrderMapping
      = new Dictionary<string, AnalysisOrder>() {
        {
          "Linear", AnalysisOrder.LINEAR
        }, {
          "Quadratic", AnalysisOrder.QUADRATIC
        }, {
          "Rigid Diaphragm", AnalysisOrder.RIGID_DIAPHRAGM
        },{
          "Load Panel", AnalysisOrder.LOAD_PANEL
        },
      };

    internal static readonly Dictionary<string, ElementType> _elementTypeMapping
      = new Dictionary<string, ElementType>() {
        {
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
        },
        {
          "Load Panel", (ElementType)GSAElement.LoadPanelType
        },
      };

    internal static readonly Dictionary<string, MatType> _materialTypeMapping
      = new Dictionary<string, MatType>() {
        {
          "Custom", MatType.Custom
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

    internal static readonly Dictionary<string, MemberType> _memberTypeMapping
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

    internal static readonly Dictionary<string, Property2D_Type> _prop2dTypeMapping
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
          "Flat Plate", Property2D_Type.PLATE
        }, {
          "Shell", Property2D_Type.SHELL
        }, {
          "Curved Shell", Property2D_Type.CURVED_SHELL
        }, {
          "Torsion", Property2D_Type.TORSION
        }, {
          "Load Panel", Property2D_Type.LOAD
        },
      };

    internal static readonly Dictionary<string, Type> _springPropertyTypeMapping
      = new Dictionary<string, Type>() {
        { "Axial", typeof(AxialSpringProperty) },
        { "Compression-only", typeof(CompressionSpringProperty) },
        { "Connector", typeof(ConnectorSpringProperty) },
        { "Friction", typeof(FrictionSpringProperty) },
        { "Gap", typeof(GapSpringProperty) },
        { "General", typeof(GeneralSpringProperty) },
        { "Lockup", typeof(LockupSpringProperty) },
        { "Matrix", typeof(MatrixSpringProperty) },
        { "Tension-only", typeof(TensionSpringProperty) },
        { "Torsional", typeof(TorsionalSpringProperty) }
      };

    internal static readonly IList<DiagramTypeMapping> _diagramTypeMappingDisplacement
      = new List<DiagramTypeMapping>() {
        new DiagramTypeMapping("Elem. Trans. Ux", DiagramType.TranslationUx,
          ApiDiagramType.TranslationUx),
        new DiagramTypeMapping("Elem. Trans. Uy", DiagramType.TranslationUy,
          ApiDiagramType.TranslationUy),
        new DiagramTypeMapping("Elem. Trans. Uz", DiagramType.TranslationUz,
          ApiDiagramType.TranslationUz),
        new DiagramTypeMapping("Res. Trans. U", DiagramType.ResolvedTranslationU,
          ApiDiagramType.ResolvedTranslationU),
        new DiagramTypeMapping("Elem. Rot. Rxx", DiagramType.RotationRxx,
          ApiDiagramType.RotationRxx),
        new DiagramTypeMapping("Elem. Rot. Ryy", DiagramType.RotationRyy,
          ApiDiagramType.RotationRyy),
        new DiagramTypeMapping("Elem. Rot. Rzz", DiagramType.RotationRxx,
          ApiDiagramType.RotationRzz),
        new DiagramTypeMapping("Res. Rot. R", DiagramType.ResolvedRotationR,
          ApiDiagramType.ResolvedRotationR),
      };

    internal static readonly IList<DiagramTypeMapping> _diagramTypeMappingAssemblyDisplacement
      = new List<DiagramTypeMapping>() {
        new DiagramTypeMapping("Translation Ux", DiagramType.AssemblyDisplacementUx,
          ApiDiagramType.TranslationUx),
        new DiagramTypeMapping("Translation Uy", DiagramType.AssemblyDisplacementUy,
          ApiDiagramType.TranslationUy),
        new DiagramTypeMapping("Translation Uz", DiagramType.AssemblyDisplacementUz,
          ApiDiagramType.TranslationUz),
        new DiagramTypeMapping("Res. Translation |U|", DiagramType.ResolvedAssemblyDisplacementU,
          ApiDiagramType.ResolvedTranslationU),
        new DiagramTypeMapping("Rotation Rxx", DiagramType.AssemblyRotationRxx,
          ApiDiagramType.RotationRxx),
        new DiagramTypeMapping("Rotation Ryy", DiagramType.AssemblyRotationRxx,
          ApiDiagramType.RotationRyy),
        new DiagramTypeMapping("Rotation Rzz", DiagramType.AssemblyRotationRxx,
          ApiDiagramType.RotationRzz),
        new DiagramTypeMapping("Res. Rotation |R|", DiagramType.ResolvedAssemblyRotationR,
          ApiDiagramType.ResolvedRotationR)
      };

    internal static readonly IList<DiagramTypeMapping> _diagramTypeMappingAssemblyDrift
      = new List<DiagramTypeMapping>() {
        new DiagramTypeMapping("Drift Dx", DiagramType.AssemblyDriftX,
          ApiDiagramType.DriftX),
        new DiagramTypeMapping("Drift Dy", DiagramType.AssemblyDriftY,
          ApiDiagramType.DriftY),
        new DiagramTypeMapping("Res. Drift |D|", DiagramType.ResolvedAssemblyDrift,
          ApiDiagramType.ResolvedDrift),
        new DiagramTypeMapping("In-plane Drift", DiagramType.AssemblyDriftXY,
          ApiDiagramType.DriftXY)
      };

    internal static readonly IList<DiagramTypeMapping> _diagramTypeMappingAssemblyDriftIndex
      = new List<DiagramTypeMapping>() {
        new DiagramTypeMapping("Drift Index DIx", DiagramType.AssemblyDriftIndexX,
          ApiDiagramType.DriftIndexX),
        new DiagramTypeMapping("Drift Index DIy", DiagramType.AssemblyDriftIndexY,
          ApiDiagramType.DriftIndexY),
        new DiagramTypeMapping("Res. Drift Index |DI|", DiagramType.ResolvedAssemblyDriftIndex,
          ApiDiagramType.ResolvedDriftIndex),
        new DiagramTypeMapping("In-plane Drift Index", DiagramType.AssemblyDriftIndexXY,
          ApiDiagramType.DriftIndexXY)
      };

    internal static readonly IList<DiagramTypeMapping> _diagramTypeMappingAssemblyForce
      = new List<DiagramTypeMapping>() {
        new DiagramTypeMapping("Axial Fx", DiagramType.AssemblyAxialForceFx,
          ApiDiagramType.AxialForceFx),
        new DiagramTypeMapping("Shear Fy", DiagramType.AssemblyShearForceFy,
          ApiDiagramType.ShearForceFy),
        new DiagramTypeMapping("Shear Fz", DiagramType.AssemblyShearForceFz,
          ApiDiagramType.ShearForceFz),
        new DiagramTypeMapping("Torsion Mxx", DiagramType.AssemblyTorsionMxx,
          ApiDiagramType.TorsionMxx),
        new DiagramTypeMapping("Moment Myy", DiagramType.AssemblyMomentMyy,
          ApiDiagramType.MomentMyy),
        new DiagramTypeMapping("Moment Mzz", DiagramType.AssemblyMomentMzz,
          ApiDiagramType.MomentMzz)
      };

    internal static readonly IList<DiagramTypeMapping> _diagramTypeMappingForce
      = new List<DiagramTypeMapping>() {
        new DiagramTypeMapping("Axial Fx", DiagramType.AxialForceFx,
          ApiDiagramType.AxialForceFx),
        new DiagramTypeMapping("Shear Fy", DiagramType.ShearForceFy,
          ApiDiagramType.ShearForceFy),
        new DiagramTypeMapping("Shear Fz", DiagramType.ShearForceFz,
          ApiDiagramType.ShearForceFz),
        new DiagramTypeMapping("Res. Shear |Fyz|", DiagramType.ResolvedShearFyz,
          ApiDiagramType.ResolvedShearFyz),
        new DiagramTypeMapping("Torsion Mxx", DiagramType.TorsionMxx,
          ApiDiagramType.TorsionMxx),
        new DiagramTypeMapping("Moment Myy", DiagramType.MomentMyy,
          ApiDiagramType.MomentMyy),
        new DiagramTypeMapping("Moment Mzz", DiagramType.MomentMzz,
          ApiDiagramType.MomentMzz),
        new DiagramTypeMapping("Res. Moment |Myz|", DiagramType.ResolvedMomentMyz,
          ApiDiagramType.ResolvedMomentMyz), };

    internal static readonly IList<DiagramTypeMapping> _diagramTypeMappingStress
      = new List<DiagramTypeMapping>() {
        new DiagramTypeMapping("Axial A", DiagramType.AxialStressA,
          ApiDiagramType.AxialStressA),
        new DiagramTypeMapping("Shear Sy", DiagramType.ShearStressSy,
          ApiDiagramType.ShearStressSy),
        new DiagramTypeMapping("Shear Sz", DiagramType.ShearStressSz,
          ApiDiagramType.ShearStressSz),
        new DiagramTypeMapping("Bending By +z", DiagramType.BendingStressByPositiveZ,
          ApiDiagramType.BendingStressByPositiveZ),
        new DiagramTypeMapping("Bending By -z", DiagramType.BendingStressByNegativeZ,
          ApiDiagramType.BendingStressByNegativeZ),
        new DiagramTypeMapping("Bending Bz +y", DiagramType.BendingStressBzPositiveY,
          ApiDiagramType.BendingStressBzPositiveY),
        new DiagramTypeMapping("Bending Bz -y", DiagramType.BendingStressBzNegativeY,
          ApiDiagramType.BendingStressBzNegativeY),
        new DiagramTypeMapping("Combined C1", DiagramType.CombinedStressC1,
          ApiDiagramType.CombinedStressC1),
        new DiagramTypeMapping("Combined C2", DiagramType.CombinedStressC2,
          ApiDiagramType.CombinedStressC2), };

    internal static readonly IList<DiagramTypeMapping> _diagramTypeMappingLoads
      = new List<DiagramTypeMapping>() {
        new DiagramTypeMapping("Grid Point", DiagramType.LoadGridPoint,
          ApiDiagramType.LoadGridPoint),
        new DiagramTypeMapping("Grid Line", DiagramType.LoadGridLine,
          ApiDiagramType.LoadGridLine),
        new DiagramTypeMapping("Grid Area", DiagramType.LoadGridArea,
          ApiDiagramType.LoadGridArea),
        new DiagramTypeMapping("Nodal Force", DiagramType.LoadNodalForce,
          ApiDiagramType.LoadNodalForce),
        new DiagramTypeMapping("Nodal Moment", DiagramType.LoadNodalMoment,
          ApiDiagramType.LoadNodalMoment),
        new DiagramTypeMapping("Nodal Displ. Translation",
          DiagramType.LoadNodalDisplacementTranslation,
          ApiDiagramType.LoadNodalDisplacementTranslation),
        new DiagramTypeMapping("Nodal Displ. Rotation",
          DiagramType.LoadNodalDisplacementRotation,
          ApiDiagramType.LoadNodalDisplacementRotation),
        new DiagramTypeMapping("Nodal Settl. Translation",
          DiagramType.LoadNodalSettlementTranslation,
          ApiDiagramType.LoadNodalSettlementTranslation),
        new DiagramTypeMapping("Nodal Settl. Rotation", DiagramType.LoadNodalSettlementRotation,
          ApiDiagramType.LoadNodalSettlementRotation),
        new DiagramTypeMapping("Beam Point Force", DiagramType.Load1dPointForce,
          ApiDiagramType.Load1dPointForce),
        new DiagramTypeMapping("Beam Point Moment", DiagramType.Load1dPointMoment,
          ApiDiagramType.Load1dPointMoment),
        new DiagramTypeMapping("Beam Patch Force", DiagramType.Load1dPatchForce,
          ApiDiagramType.Load1dPatchForce),
        new DiagramTypeMapping("Beam Patch Moment", DiagramType.Load1dPatchMoment,
          ApiDiagramType.Load1dPatchMoment),
        new DiagramTypeMapping("Beam Prestress Force", DiagramType.Load1dPrestressForce,
          ApiDiagramType.Load1dPrestressForce),
        new DiagramTypeMapping("Beam Prestress Moment", DiagramType.Load1dPrestressMoment,
          ApiDiagramType.Load1dPrestressMoment),
        new DiagramTypeMapping("Beam Initial Strain", DiagramType.Load1dInitialStrain,
          ApiDiagramType.Load1dInitialStrain),
        new DiagramTypeMapping("Beam Lack Of Fit", DiagramType.Load1dLackOfFit,
          ApiDiagramType.Load1dLackOfFit),
        new DiagramTypeMapping("Beam Distortion Translation", DiagramType.Load1dDistortionTranslation,
          ApiDiagramType.Load1dDistortionTranslation),
        new DiagramTypeMapping("Beam Distortion Rotation", DiagramType.Load1dDistortionRotation,
          ApiDiagramType.Load1dDistortionRotation),
        new DiagramTypeMapping("Beam Thermal Uniform", DiagramType.Load1dThermalUniform,
          ApiDiagramType.Load1dThermalUniform),
        new DiagramTypeMapping("Beam Thermal Gradient", DiagramType.Load1dThermalGradient,
          ApiDiagramType.Load1dThermalGradient),
        new DiagramTypeMapping("2d FacePoint Force", DiagramType.Load2dFacePointForce,
          ApiDiagramType.Load2dFacePointForce),
        new DiagramTypeMapping("2d Face Pressure", DiagramType.Load2dFacePressure,
          ApiDiagramType.Load2dFacePressure),
        new DiagramTypeMapping("2d Edge Pressure", DiagramType.Load2dEdgePressure,
          ApiDiagramType.Load2dEdgePressure),
        new DiagramTypeMapping("2d Prestress Point Force", DiagramType.Load2dPreStressPointForce,
          ApiDiagramType.Load2dPreStressPointForce),
        new DiagramTypeMapping("2d Prestress Moment", DiagramType.Load2dPreStressMoment,
          ApiDiagramType.Load2dPreStressMoment),
        new DiagramTypeMapping("2d Strain", DiagramType.Load2dStrain,
          ApiDiagramType.Load2dStrain),
        new DiagramTypeMapping("2d Thermal Uniform", DiagramType.Load2dThermalUniform,
          ApiDiagramType.Load2dThermalUniform),
        new DiagramTypeMapping("2d Thermal Gradient", DiagramType.Load2dThermalGradient,
          ApiDiagramType.Load2dThermalGradient),
        new DiagramTypeMapping("3d Face Pressure", DiagramType.Load3dFacePressure,
          ApiDiagramType.Load3dFacePressure),
        new DiagramTypeMapping("3d Thermal Uniform", DiagramType.Load3dThermalUniform,
          ApiDiagramType.Load3dThermalUniform),
        new DiagramTypeMapping("3d Thermal Gradient", DiagramType.Load3dThermalGradient,
          ApiDiagramType.Load3dThermalGradient), };

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
          types = _alignmentTypeMapping.Keys.ToList();
          index = GetIndex(key, types);
          if (index != -1) {
            return _alignmentTypeMapping[types[index]];
          }

          break;

        case Type _ when t == typeof(AnalysisOrder):
          types = _analysisOrderMapping.Keys.ToList();
          index = GetIndex(key, types);
          if (index != -1) {
            return _analysisOrderMapping[types[index]];
          }

          break;

        case Type _ when t == typeof(ElementType):
          types = _elementTypeMapping.Keys.ToList();
          index = GetIndex(key, types);
          if (index != -1) {
            return _elementTypeMapping[types[index]];
          }

          break;

        case Type _ when t == typeof(MatType):
          types = _materialTypeMapping.Keys.ToList();
          index = GetIndex(key, types);
          if (index != -1) {
            return _materialTypeMapping[types[index]];
          }

          break;

        case Type _ when t == typeof(MemberType):
          types = _memberTypeMapping.Keys.ToList();
          index = GetIndex(key, types);
          if (index != -1) {
            return _memberTypeMapping[types[index]];
          }

          break;

        case Type _ when t == typeof(Property2D_Type):
          types = _prop2dTypeMapping.Keys.ToList();
          index = GetIndex(key, types);
          if (index != -1) {
            return _prop2dTypeMapping[types[index]];
          }

          break;
      }

      throw new ArgumentException();
    }
  }
}
