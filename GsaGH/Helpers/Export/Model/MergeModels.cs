using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Helpers.Export {
  public class MergeModels {

    public static GsaModel MergeModel(List<GsaModel> models, GH_Component owner, Length tolerance) {
      if (models == null) {
        return null;
      }

      if (models.Count <= 1) {
        return models.Count > 0 ? models[0].Clone() : null;
      }

      var model = new GsaModel();
      models.Reverse();
      for (int i = 0; i < models.Count - 1; i++) {
        model = MergeModel(model, models[i].Clone(), owner, tolerance);
      }
      GsaModel clone = models[models.Count - 1].Clone();
      clone = MergeModel(clone, model, owner, tolerance);
      return clone;
    }

    public static GsaModel MergeModel(GsaModel mainModel, GsaModel appendModel, GH_Component owner, Length tolerance) {
      Model model = appendModel.Model;
      ReadOnlyDictionary<int, Node> nDict = model.Nodes();
      ReadOnlyDictionary<int, Element> eDict = model.Elements();
      ReadOnlyDictionary<int, Member> mDict = model.Members();
      ReadOnlyDictionary<int, Section> sDict = model.Sections();
      ReadOnlyDictionary<int, Prop2D> pDict = model.Prop2Ds();
      ReadOnlyDictionary<int, Prop3D> p3Dict = model.Prop3Ds();
      ReadOnlyDictionary<int, AnalysisMaterial> amDict = model.AnalysisMaterials();
      ReadOnlyDictionary<int, SectionModifier> modDict = model.SectionModifiers();
      ReadOnlyDictionary<int, Axis> axDict = model.Axes();
      var localElemAxesDict = eDict.Keys.ToDictionary(id => id, id => model.ElementDirectionCosine(id));
      var localMemAxesDict = mDict.Keys.ToDictionary(id => id, id => model.MemberDirectionCosine(id));
      ConcurrentBag<GsaNodeGoo> goonodes = Import.Nodes.GetNodes(nDict, LengthUnit.Meter);
      var nodes = goonodes.Select(n => n.Value).ToList();
      nodes.Select(c => { c.Id = 0; return c; }).ToList();

      Tuple<ConcurrentBag<GsaElement1dGoo>, ConcurrentBag<GsaElement2dGoo>, ConcurrentBag<GsaElement3dGoo>> elementTuple
          = Import.Elements.GetElements(eDict, nDict, sDict, pDict, p3Dict, amDict, modDict, localElemAxesDict, axDict, LengthUnit.Meter, true);
      var elem1ds = elementTuple.Item1.Select(n => n.Value).ToList();
      elem1ds.Select(c => { c.Id = 0; return c; }).ToList();
      var elem2ds = elementTuple.Item2.Select(n => n.Value).ToList();
      foreach (GsaElement2d elem2d in elem2ds)
        elem2d.Ids.Select(c => { c = 0; return c; }).ToList();
      var elem3ds = elementTuple.Item3.Select(n => n.Value).ToList();
      foreach (GsaElement3d elem3d in elem3ds)
        elem3d.Ids.Select(c => { c = 0; return c; }).ToList();

      Tuple<ConcurrentBag<GsaMember1dGoo>, ConcurrentBag<GsaMember2dGoo>, ConcurrentBag<GsaMember3dGoo>> memberTuple
          = Import.Members.GetMembers(mDict, nDict, sDict, pDict, p3Dict, amDict, modDict, localMemAxesDict, axDict, LengthUnit.Meter, true);
      var mem1ds = memberTuple.Item1.Select(n => n.Value).ToList();
      mem1ds.Select(c => { c.Id = 0; return c; }).ToList();
      var mem2ds = memberTuple.Item2.Select(n => n.Value).ToList();
      mem2ds.Select(c => { c.Id = 0; return c; }).ToList();

      List<GsaSectionGoo> goosections = Import.Properties.GetSections(sDict, model.AnalysisMaterials(), model.SectionModifiers());
      var sections = goosections.Select(n => n.Value).ToList();
      sections.Select(c => { c.Id = 0; return c; }).ToList();
      List<GsaProp2dGoo> gooprop2Ds = Import.Properties.GetProp2ds(pDict, model.AnalysisMaterials(), axDict);
      var prop2Ds = gooprop2Ds.Select(n => n.Value).ToList();
      prop2Ds.Select(c => { c.Id = 0; return c; }).ToList();
      List<GsaProp3dGoo> gooprop3Ds = Import.Properties.GetProp3ds(p3Dict, model.AnalysisMaterials());
      var prop3Ds = gooprop3Ds.Select(n => n.Value).ToList();
      prop3Ds.Select(c => { c.Id = 0; return c; }).ToList();

      var gooloads = new List<GsaLoadGoo>();
      gooloads.AddRange(Import.Loads.GetGravityLoads(model.GravityLoads()));
      gooloads.AddRange(Import.Loads.GetNodeLoads(model));
      gooloads.AddRange(Import.Loads.GetBeamLoads(model.BeamLoads()));
      gooloads.AddRange(Import.Loads.GetFaceLoads(model.FaceLoads()));

      IReadOnlyDictionary<int, GridSurface> srfDict = model.GridSurfaces();
      IReadOnlyDictionary<int, GridPlane> plnDict = model.GridPlanes();

      gooloads.AddRange(Import.Loads.GetGridPointLoads(model.GridPointLoads(), srfDict, plnDict, axDict, LengthUnit.Meter));
      gooloads.AddRange(Import.Loads.GetGridLineLoads(model.GridLineLoads(), srfDict, plnDict, axDict, LengthUnit.Meter));
      gooloads.AddRange(Import.Loads.GetGridAreaLoads(model.GridAreaLoads(), srfDict, plnDict, axDict, LengthUnit.Meter));
      var loads = gooloads.Select(n => n.Value).ToList();

      var gpsgoo = srfDict.Keys.Select(key => new GsaGridPlaneSurfaceGoo(Import.Loads.GetGridPlaneSurface(srfDict, plnDict, axDict, key, LengthUnit.Meter))).ToList();
      var gps = gpsgoo.Select(n => n.Value).ToList();
      mainModel.Model = AssembleModel.Assemble(mainModel, nodes, elem1ds, elem2ds, elem3ds, mem1ds, mem2ds, null, sections, prop2Ds, prop3Ds, loads, gps, null, null, LengthUnit.Meter, tolerance, false, owner);
      return mainModel;
    }
  }
}
