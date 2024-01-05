﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Helpers.Import {
  internal class PropertyQuantities {
    internal IDictionary<int, Length> SectionQuantities { get; private set; }
    internal IDictionary<int, Area> Property2dQuantities { get; private set; }
    private LengthUnit _unit = LengthUnit.Meter;

    internal PropertyQuantities(GsaModel model, Layer layer, string list) {
      _unit = model.ModelUnit;
      switch (layer) {
        case Layer.Analysis:
          CalculateFromElements(new Elements(model, list));
          return;
        case Layer.Design:
          CalculateFromMember(new Members(model, list));
          return;
      }
    }

    private void CalculateFromElements(Elements elements) {
      var sectionQuantities = new ConcurrentDictionary<int, double>();
      var property2dQuantities = new ConcurrentDictionary<int, double>();

      Parallel.ForEach(elements.Element1ds, element => {
        if (element.Value.Section == null || element.Value.Section.IsReferencedById) {
          return;
        }
        double length = element.Value.Line.GetLength();
        double offset1 = 0;
        if (element.Value.ApiElement.Offset.X1 > 0) {
          offset1 = new Length(element.Value.ApiElement.Offset.X1, LengthUnit.Meter).As(_unit);
        }

        double offset2 = 0;
        if (element.Value.ApiElement.Offset.X2 > 0) {
          offset2 = new Length(element.Value.ApiElement.Offset.X1, LengthUnit.Meter).As(_unit);
        }

        sectionQuantities.AddOrUpdate(
          element.Value.Section.Id, length - offset1 - offset2,
          (key, oldValue) => oldValue + length - offset1 - offset2);
      });

      SectionQuantities = sectionQuantities.ToDictionary(
        kvp => kvp.Key, kvp => new Length(kvp.Value, _unit));

      foreach (GsaElement2dGoo element in elements.Element2ds ) {
        if (element.Value.Prop2ds.IsNullOrEmpty()) {
          continue;
        }

        if (element.Value.Prop2ds.Count == 1) {
          double area = AreaMassProperties.Compute(element.Value.Mesh, true, false, false, false).Area;
          property2dQuantities.AddOrUpdate(
              element.Value.Prop2ds[0].Id, area, (key, oldValue) => oldValue + area);
          continue;
        }

        var ids = Enumerable.Range(0, element.Value.Ids.Count).ToList();
        Parallel.ForEach(ids, id => {
          if (element.Value.Prop2ds[id] == null) {
            return;
          }

          Mesh face = element.Value.Mesh.Faces.ExtractFaces(new int[] { id });
          double area = AreaMassProperties.Compute(face, true, false, false, false).Area;
          property2dQuantities.AddOrUpdate(
            element.Value.Prop2ds[id].Id, area, (key, oldValue) => oldValue + area);
        });
      }

      AreaUnit areaUnit = OasysGH.Units.Helpers.UnitsHelper.GetAreaUnit(_unit);
      Property2dQuantities = property2dQuantities.ToDictionary(
        kvp => kvp.Key, kvp => new Area(kvp.Value, areaUnit));
    }

    private void CalculateFromMember(Members members) {
      var sectionQuantities = new ConcurrentDictionary<int, double>();
      var property2dQuantities = new ConcurrentDictionary<int, double>();
      var property3dQuantities = new ConcurrentDictionary<int, double>();

      Parallel.ForEach(members.Member1ds, member => {
        if (member.Value.Section == null || member.Value.Section.IsReferencedById) {
          return;
        }
        double length = member.Value.PolyCurve.GetLength();
        double offset1 = 0;
        if (member.Value.ApiMember.Offset.X1 > 0) {
          offset1 = new Length(member.Value.ApiMember.Offset.X1, LengthUnit.Meter).As(_unit);
        }

        double offset2 = 0;
        if (member.Value.ApiMember.Offset.X2 > 0) {
          offset2 = new Length(member.Value.ApiMember.Offset.X1, LengthUnit.Meter).As(_unit);
        }

        sectionQuantities.AddOrUpdate(
          member.Value.Section.Id, length - offset1 - offset2,
          (key, oldValue) => oldValue + length - offset1 - offset2);
      });

      SectionQuantities = sectionQuantities.ToDictionary(
        kvp => kvp.Key, kvp => new Length(kvp.Value, _unit));

      Parallel.ForEach(members.Member2ds, member => {
        if (member.Value.Prop2d == null || member.Value.Prop2d.IsReferencedById) {
          return;
        }
        double area = member.Value.Brep.GetArea();
        sectionQuantities.AddOrUpdate(
          member.Value.Prop2d.Id, area, (key, oldValue) => oldValue + area);
      });

      AreaUnit areaUnit = OasysGH.Units.Helpers.UnitsHelper.GetAreaUnit(_unit);
      Property2dQuantities = property2dQuantities.ToDictionary(
        kvp => kvp.Key, kvp => new Area(kvp.Value, areaUnit));
    }
  }
}