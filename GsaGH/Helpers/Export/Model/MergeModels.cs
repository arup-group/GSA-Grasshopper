using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;
using OasysGH.Units;
using OasysUnits.Units;

namespace GsaGH.Helpers.Export
{
  public class MergeModels
  {
    public static GsaModel MergeModel(List<GsaModel> models)
    {
      if (models != null)
      {
        if (models.Count > 1)
        {
          GsaModel model = new GsaModel();
          models.Reverse();
          for (int i = 0; i < models.Count - 1; i++)
          {
            model = MergeModels.MergeModel(model, models[i].Clone());
          }
          GsaModel clone = models[models.Count - 1].Clone();
          clone = MergeModels.MergeModel(clone, model);
          return clone;
        }
        else if (models.Count > 0)
          return models[0].Clone();
      }
      return null;
    }

    public static GsaModel MergeModel(GsaModel mainModel, GsaModel appendModel)
    {
      // open the copyfrom model
      Model model = appendModel.Model;

      // get dictionaries from model
      ConcurrentDictionary<int, Node> nDict = new ConcurrentDictionary<int, Node>(model.Nodes());
      ConcurrentDictionary<int, Element> eDict = new ConcurrentDictionary<int, Element>(model.Elements());
      ConcurrentDictionary<int, Member> mDict = new ConcurrentDictionary<int, Member>(model.Members());
      ConcurrentDictionary<int, Section> sDict = new ConcurrentDictionary<int, Section>(model.Sections());
      ConcurrentDictionary<int, Prop2D> pDict = new ConcurrentDictionary<int, Prop2D>(model.Prop2Ds());
      ConcurrentDictionary<int, Prop3D> p3Dict = new ConcurrentDictionary<int, Prop3D>(model.Prop3Ds());
      ConcurrentDictionary<int, AnalysisMaterial> amDict = new ConcurrentDictionary<int, AnalysisMaterial>(model.AnalysisMaterials());
      ConcurrentDictionary<int, SectionModifier> modDict = new ConcurrentDictionary<int, SectionModifier>(model.SectionModifiers());

      // get nodes
      ConcurrentBag<GsaNodeGoo> goonodes = Import.Nodes.GetNodes(nDict, LengthUnit.Meter);
      // convert from Goo-type
      List<GsaNode> nodes = goonodes.Select(n => n.Value).ToList();
      // change all members in List's ID to 0;
      nodes.Select(c => { c.Id = 0; return c; }).ToList();

      // get elements
      Tuple<ConcurrentBag<GsaElement1dGoo>, ConcurrentBag<GsaElement2dGoo>, ConcurrentBag<GsaElement3dGoo>> elementTuple
          = Import.Elements.GetElements(eDict, nDict, sDict, pDict, p3Dict, amDict, modDict, LengthUnit.Meter);
      // convert from Goo-type
      List<GsaElement1d> elem1ds = elementTuple.Item1.Select(n => n.Value).ToList();
      // change all members in List's ID to 0;
      elem1ds.Select(c => { c.Id = 0; return c; }).ToList();
      // convert from Goo-type
      List<GsaElement2d> elem2ds = elementTuple.Item2.Select(n => n.Value).ToList();
      // change all members in List's ID to 0;
      foreach (var elem2d in elem2ds)
        elem2d.Ids.Select(c => { c = 0; return c; }).ToList();
      // convert from Goo-type
      List<GsaElement3d> elem3ds = elementTuple.Item3.Select(n => n.Value).ToList();
      // change all members in List's ID to 0;
      foreach (var elem3d in elem3ds)
        elem3d.IDs.Select(c => { c = 0; return c; }).ToList();

      // get members
      Tuple<ConcurrentBag<GsaMember1dGoo>, ConcurrentBag<GsaMember2dGoo>, ConcurrentBag<GsaMember3dGoo>> memberTuple
          = Import.Members.GetMembers(mDict, nDict, LengthUnit.Meter, sDict, pDict, p3Dict);
      // convert from Goo-type
      List<GsaMember1d> mem1ds = memberTuple.Item1.Select(n => n.Value).ToList();
      // change all members in List's ID to 0;
      mem1ds.Select(c => { c.Id = 0; return c; }).ToList();
      // convert from Goo-type
      List<GsaMember2d> mem2ds = memberTuple.Item2.Select(n => n.Value).ToList();
      // change all members in List's ID to 0;
      mem2ds.Select(c => { c.Id = 0; return c; }).ToList();

      // get properties
      List<GsaSectionGoo> goosections = Import.Properties.GetSections(sDict, model.AnalysisMaterials(), model.SectionModifiers());
      // convert from Goo-type
      List<GsaSection> sections = goosections.Select(n => n.Value).ToList();
      // change all members in List's ID to 0;
      sections.Select(c => { c.Id = 0; return c; }).ToList();
      List<GsaProp2dGoo> gooprop2Ds = Import.Properties.GetProp2ds(pDict, model.AnalysisMaterials());
      // convert from Goo-type
      List<GsaProp2d> prop2Ds = gooprop2Ds.Select(n => n.Value).ToList();
      // change all members in List's ID to 0;
      prop2Ds.Select(c => { c.ID = 0; return c; }).ToList();
      List<GsaProp3dGoo> gooprop3Ds = Import.Properties.GetProp3ds(p3Dict, model.AnalysisMaterials());
      // convert from Goo-type
      List<GsaProp3d> prop3Ds = gooprop3Ds.Select(n => n.Value).ToList();
      // change all members in List's ID to 0;
      prop3Ds.Select(c => { c.ID = 0; return c; }).ToList();

      // get loads
      List<GsaLoadGoo> gooloads = new List<GsaLoadGoo>();
      gooloads.AddRange(Import.Loads.GetGravityLoads(model.GravityLoads()));
      gooloads.AddRange(Import.Loads.GetNodeLoads(model));
      gooloads.AddRange(Import.Loads.GetBeamLoads(model.BeamLoads()));
      gooloads.AddRange(Import.Loads.GetFaceLoads(model.FaceLoads()));

      IReadOnlyDictionary<int, GridSurface> srfDict = model.GridSurfaces();
      IReadOnlyDictionary<int, GridPlane> plnDict = model.GridPlanes();
      IReadOnlyDictionary<int, Axis> axDict = model.Axes();

      gooloads.AddRange(Import.Loads.GetGridPointLoads(model.GridPointLoads(), srfDict, plnDict, axDict, LengthUnit.Meter));
      gooloads.AddRange(Import.Loads.GetGridLineLoads(model.GridLineLoads(), srfDict, plnDict, axDict, LengthUnit.Meter));
      gooloads.AddRange(Import.Loads.GetGridAreaLoads(model.GridAreaLoads(), srfDict, plnDict, axDict, LengthUnit.Meter));
      List<GsaLoad> loads = gooloads.Select(n => n.Value).ToList();

      // get grid plane surfaces
      List<GsaGridPlaneSurfaceGoo> gpsgoo = new List<GsaGridPlaneSurfaceGoo>();
      foreach (int key in srfDict.Keys)
        gpsgoo.Add(new GsaGridPlaneSurfaceGoo(Import.Loads.GetGridPlaneSurface(srfDict, plnDict, axDict, key, LengthUnit.Meter)));
      // convert from Goo-type
      List<GsaGridPlaneSurface> gps = gpsgoo.Select(n => n.Value).ToList();

      // return new assembled model
      mainModel.Model = AssembleModel.Assemble(mainModel, nodes, elem1ds, elem2ds, elem3ds, mem1ds, mem2ds, null, sections, prop2Ds, prop3Ds, loads, gps, null, null, LengthUnit.Meter, DefaultUnits.Tolerance.Meters, false);
      return mainModel;
    }
  }
}
