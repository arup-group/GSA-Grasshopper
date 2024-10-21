using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using GsaAPI;

using GsaGH.Helpers;
using GsaGH.Helpers.GsaApi;

using OasysGH.Units;

using OasysUnits;
using OasysUnits.Units;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters {
  /// <summary>
  /// A spring is a general type of element which can be used to model both simple springs and more sophisticated types of behaviour. Spring properties describe those behaviours.
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-pr-spring/">Spring Properties</see> to read more.</para>
  /// </summary>
  public class GsaSpringProperty : IGsaProperty {
    public Guid Guid { get; set; } = Guid.NewGuid();
    public int Id { get; set; } = 0;
    public bool IsReferencedById { get; set; } = false;
    public SpringProperty ApiProperty { get; internal set; }

    public GsaSpringProperty() {
      ApiProperty = new AxialSpringProperty();
    }

    public GsaSpringProperty(int id) {
      Id = id;
      IsReferencedById = true;
    }

    public GsaSpringProperty(GsaSpringProperty other) {
      Id = other.Id;
      IsReferencedById = other.IsReferencedById;
      if (!IsReferencedById) {
        ApiProperty = other.DuplicateApiObject();
      }
    }

    internal GsaSpringProperty(SpringProperty property) {
      ApiProperty = property;
    }

    internal GsaSpringProperty(KeyValuePair<int, SpringProperty> property) {
      Id = property.Key;
      ApiProperty = property.Value;
    }

    public override string ToString() {
      string ps = (Id > 0) ? "PS" + Id : string.Empty;
      if (IsReferencedById) {
        return (Id > 0) ? $"{ps} (referenced)" : string.Empty; ;
      }

      string name = ApiProperty.Name;
      string type = Mappings._springPropertyTypeMapping.FirstOrDefault(x => x.Value == ApiProperty.GetType()).Key;
      string values = SpringValuesToString();
      return string.Join(" ", ps, type, name, values).TrimSpaces();
    }

    internal SpringProperty DuplicateApiObject() {
      SpringProperty property;
      switch (ApiProperty) {
        case AxialSpringProperty axialSpringProperty:
          property = new AxialSpringProperty {
            Stiffness = axialSpringProperty.Stiffness
          };
          break;

        case TorsionalSpringProperty torsionalSpringProperty:
          property = new TorsionalSpringProperty {
            Stiffness = torsionalSpringProperty.Stiffness
          };
          break;

        case GeneralSpringProperty generalSpringProperty:
          property = new GeneralSpringProperty {
            StiffnessX = generalSpringProperty.StiffnessX,
            StiffnessY = generalSpringProperty.StiffnessY,
            StiffnessZ = generalSpringProperty.StiffnessZ,
            StiffnessXX = generalSpringProperty.StiffnessXX,
            StiffnessYY = generalSpringProperty.StiffnessYY,
            StiffnessZZ = generalSpringProperty.StiffnessZZ,
            SpringCurveX = generalSpringProperty.SpringCurveX,
            SpringCurveY = generalSpringProperty.SpringCurveY,
            SpringCurveZ = generalSpringProperty.SpringCurveZ,
            SpringCurveXX = generalSpringProperty.SpringCurveXX,
            SpringCurveYY = generalSpringProperty.SpringCurveYY,
            SpringCurveZZ = generalSpringProperty.SpringCurveZZ
          };
          break;

        case MatrixSpringProperty matrixSpringProperty:
          property = new MatrixSpringProperty {
            SpringMatrix = matrixSpringProperty.SpringMatrix
          };
          break;

        case TensionSpringProperty tensionSpringProperty:
          property = new TensionSpringProperty {
            Stiffness = tensionSpringProperty.Stiffness
          };
          break;

        case CompressionSpringProperty compressionSpringProperty:
          property = new CompressionSpringProperty {
            Stiffness = compressionSpringProperty.Stiffness
          };
          break;

        case ConnectorSpringProperty connectorSpringProperty:
          property = new ConnectorSpringProperty();
          break;

        case LockupSpringProperty lockupSpringProperty:
          property = new LockupSpringProperty {
            Stiffness = lockupSpringProperty.Stiffness,
            PositiveLockup = lockupSpringProperty.PositiveLockup,
            NegativeLockup = lockupSpringProperty.NegativeLockup
          };
          break;

        case GapSpringProperty gapSpringProperty:
          property = new GapSpringProperty {
            Stiffness = gapSpringProperty.Stiffness
          };
          break;

        case FrictionSpringProperty frictionSpringProperty:
          property = new FrictionSpringProperty {
            StiffnessX = frictionSpringProperty.StiffnessX,
            StiffnessY = frictionSpringProperty.StiffnessY,
            StiffnessZ = frictionSpringProperty.StiffnessZ,
            FrictionCoefficient = frictionSpringProperty.FrictionCoefficient
          };
          break;

        default:
          return null;
      }

      // workaround to handle that Color is non-nullable type
      if ((Color)ApiProperty.Colour != Color.FromArgb(0, 0, 0)) {
        property.Colour = ApiProperty.Colour;
      }

      property.DampingRatio = ApiProperty.DampingRatio;
      property.Name = ApiProperty.Name;

      return property;
    }

    private string SpringValuesToString() {
      string value = string.Empty;
      RotationalStiffnessUnit _rotationalStiffnessUnit = RotationalStiffnessUnit.NewtonMeterPerRadian;
      LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;
      ForcePerLengthUnit _stiffnessUnit = DefaultUnits.ForcePerLengthUnit;
      switch (ApiProperty) {
        case AxialSpringProperty axial:
          value = new ForcePerLength(axial.Stiffness, ForcePerLengthUnit.NewtonPerMeter)
            .ToUnit(_stiffnessUnit).ToString().Replace(" ", string.Empty);
          break;

        case TensionSpringProperty tension:
          value = new ForcePerLength(tension.Stiffness, ForcePerLengthUnit.NewtonPerMeter)
            .ToUnit(_stiffnessUnit).ToString().Replace(" ", string.Empty);
          break;

        case CompressionSpringProperty compression:
          value = new ForcePerLength(compression.Stiffness, ForcePerLengthUnit.NewtonPerMeter)
            .ToUnit(_stiffnessUnit).ToString().Replace(" ", string.Empty);
          break;

        case GapSpringProperty gap:
          value = new ForcePerLength(gap.Stiffness, ForcePerLengthUnit.NewtonPerMeter)
            .ToUnit(_stiffnessUnit).ToString().Replace(" ", string.Empty);
          break;

        case TorsionalSpringProperty torsional:
          value = new RotationalStiffness(torsional.Stiffness, RotationalStiffnessUnit.NewtonMeterPerRadian)
            .ToUnit(_rotationalStiffnessUnit).ToString().Replace(" ", string.Empty);
          break;

        case GeneralSpringProperty general:
          string x = GeneralSpringToString("X", general.SpringCurveX, general.StiffnessX);
          string y = GeneralSpringToString("Y", general.SpringCurveY, general.StiffnessY);
          string z = GeneralSpringToString("Z", general.SpringCurveZ, general.StiffnessZ);
          string xx = GeneralRotationalSpringToString(
            "XX", general.SpringCurveXX, general.StiffnessXX);
          string yy = GeneralRotationalSpringToString(
            "YY", general.SpringCurveYY, general.StiffnessYY);
          string zz = GeneralRotationalSpringToString(
            "ZZ", general.SpringCurveZZ, general.StiffnessZZ);
          value = string.Join(" ", x, y, z, xx, yy, zz);
          break;

        case MatrixSpringProperty matrix:
          value = $"MatrixID:{matrix.SpringMatrix}";
          break;

        case LockupSpringProperty lockup:
          value = "+ve:" + new Length((double)lockup.PositiveLockup, LengthUnit.Meter)
            .ToUnit(_lengthUnit).ToString().Replace(" ", string.Empty)
            + " -ve:" + new Length((double)lockup.NegativeLockup, LengthUnit.Meter)
            .ToUnit(_lengthUnit).ToString().Replace(" ", string.Empty);
          break;

        case FrictionSpringProperty friction:
          string fx = GeneralSpringToString("X", null, friction.StiffnessX);
          string fy = GeneralSpringToString("Y", null, friction.StiffnessY);
          string fz = GeneralSpringToString("Z", null, friction.StiffnessZ);
          string coeff = friction.FrictionCoefficient == 0 ? string.Empty
            : $"Coeff.:{friction.FrictionCoefficient}";
          value = string.Join(" ", fx, fy, fz, coeff);
          break;
        case ConnectorSpringProperty connector:
        default:
          break;
      }

      string damping = ApiProperty.DampingRatio == 0 ? string.Empty
        : new Ratio(ApiProperty.DampingRatio, RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent).ToString().Replace(" ", string.Empty);

      return string.Join(" ", value, damping).Replace(",", string.Empty).TrimSpaces();
    }

    private string GeneralSpringToString(string prefix, int? curve, double? stiffness) {
      if (curve == null && stiffness == null) {
        return string.Empty;
      }

      string val = curve != null ? $"CurveID:{curve}"
        : new ForcePerLength((double)stiffness, ForcePerLengthUnit.NewtonPerMeter)
          .ToUnit(DefaultUnits.ForcePerLengthUnit).ToString().Replace(" ", string.Empty);
      return $"{prefix}:{val}";
    }

    private string GeneralRotationalSpringToString(string prefix, int? curve, double? stiffness) {
      if (curve == null && stiffness == null) {
        return string.Empty;
      }

      string val = curve != null ? $"CurveID:{curve}"
        : new RotationalStiffness((double)stiffness, RotationalStiffnessUnit.NewtonMeterPerRadian)
          .ToString().Replace(" ", string.Empty);
      return $"{prefix}:{val}";
    }
  }
}
