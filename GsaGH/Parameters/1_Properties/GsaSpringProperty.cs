using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers;
using GsaGH.Helpers.GsaApi;

namespace GsaGH.Parameters {
  /// <summary>
  /// A spring is a general type of element which can be used to model both simple springs and more sophisticated types of behaviour. Spring properties describe those behaviours.
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-pr-spring/">Spring Properties</see> to read more.</para>
  /// </summary>
  public class GsaSpringProperty : IGsaProperty {

    // do we need a guid?
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

    public SpringProperty DuplicateApiObject() {
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

      property.Colour = ApiProperty.Colour;
      property.DampingRatio = ApiProperty.DampingRatio;
      property.Name = ApiProperty.Name;

      return property;
    }

    public override string ToString() {
      string sp = (Id > 0) ? "SP" + Id : string.Empty;
      if (IsReferencedById) {
        return (Id > 0) ? $"{sp} (referenced)" : string.Empty; ;
      }

      string name = ApiProperty.Name;
      string type = Mappings.SpringPropertyTypeMapping.FirstOrDefault(x => x.Value == ApiProperty.GetType()).Key;
      return string.Join(" ", sp, type, name, type).TrimSpaces();
    }
  }
}
