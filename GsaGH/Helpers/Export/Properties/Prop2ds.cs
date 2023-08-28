using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using Rhino.Geometry;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Helpers.Export {
  internal class Prop2ds {
    internal static void ConvertProp2ds(
      List<GsaProperty2d> prop2Ds,
      ref Properties existingProperties,
      ref GsaIntDictionary<Axis> apiAxes,
      LengthUnit unit) {
      if (prop2Ds == null) {
        return;
      }

      prop2Ds = prop2Ds.OrderByDescending(p => p.Id).ToList();
      foreach (GsaProperty2d prop2d in prop2Ds.Where(prop2d => prop2d != null)) {
        ConvertProp2d(prop2d, ref existingProperties, ref apiAxes, unit);
      }
    }

    internal static int ConvertProp2d(
      GsaProperty2d prop2d,
      ref Properties existingProperties,
      ref GsaIntDictionary<Axis> apiAxes,
      LengthUnit unit) {
      if (prop2d == null) {
        return 0;
      }

      if (prop2d.IsReferencedById || prop2d.ApiProp2d == null) {
        return prop2d.Id;
      }

      return AddProp2d(prop2d, ref existingProperties, ref apiAxes, unit);
    }

    internal static int AddProp2d(
      GsaProperty2d prop, 
      ref Properties existingProperties, 
      ref GsaIntDictionary<Axis> apiAxes,
      LengthUnit unit) {
      Materials.AddMaterial(ref prop, ref existingProperties.Materials);
      if (prop.AxisProperty == -2) {
        if (prop.LocalAxis != null && prop.LocalAxis.IsValid) {
          if (prop.LocalAxis != Plane.WorldXY) {
            var ax = new Axis();
            Plane pln = prop.LocalAxis;
            ax.Origin.X = (unit == LengthUnit.Meter) ? pln.OriginX :
              new Length(pln.OriginX, unit).Meters;
            ax.Origin.Y = (unit == LengthUnit.Meter) ? pln.OriginY :
              new Length(pln.OriginY, unit).Meters;
            ax.Origin.Z = (unit == LengthUnit.Meter) ? pln.OriginZ :
              new Length(pln.OriginZ, unit).Meters;

            ax.XVector.X = pln.XAxis.X;
            ax.XVector.Y = pln.XAxis.Y;
            ax.XVector.Z = pln.XAxis.Z;
            ax.XYPlane.X = pln.YAxis.X;
            ax.XYPlane.Y = pln.YAxis.Y;
            ax.XYPlane.Z = pln.YAxis.Z;

            prop.AxisProperty = apiAxes.AddValue(ax);
          } else {
            prop.AxisProperty = 0;
          }
        }
      }

      if (prop.Id <= 0) {
        return existingProperties.Prop2ds.AddValue(prop.Guid, prop.ApiProp2d);
      }

      existingProperties.Prop2ds.SetValue(prop.Id, prop.Guid, prop.ApiProp2d);
      return prop.Id;
    }
  }
}