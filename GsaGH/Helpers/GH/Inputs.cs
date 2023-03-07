using Grasshopper.Kernel.Types;
using Grasshopper.Kernel;
using GsaGH.Parameters;
using System;
using System.Collections.Generic;

namespace GsaGH.Helpers.GH
{
  internal class Inputs
  {
    internal static List<object> GetObjectsForLists(GH_Component owner, IGH_DataAccess DA, int inputid, EntityType type)
    {
      // Get Geometry input
      List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
      List<object> list = new List<object>();
      if (DA.GetDataList(inputid, gh_types))
      {
        for (int i = 0; i < gh_types.Count; i++)
        {
          GH_ObjectWrapper gh_typ = gh_types[i];
          if (gh_typ == null) { owner.AddRuntimeWarning("Input (index: " + i + ") is null and has been ignored"); continue; }

          switch (type)
          {
            case EntityType.Node:
              if (!(gh_typ.Value is GsaNodeGoo))
              {
                owner.AddRuntimeError("Unable to convert " + owner.Params.Input[inputid].NickName + "  input (index: " + i + ") input parameter of type " +
                gh_typ.Value.GetType().Name.Replace("Goo", string.Empty) + Environment.NewLine + " to Node and has been ignored");
                continue;
              }

              GsaNode gsanode = new GsaNode();
              gh_typ.CastTo(ref gsanode);
              list.Add(gsanode.Duplicate());
              break;

            case EntityType.Element:
              if (gh_typ.Value is GsaElement1dGoo)
              {
                GsaElement1d gsaelem1 = new GsaElement1d();
                gh_typ.CastTo(ref gsaelem1);
                list.Add(gsaelem1.Duplicate());
              }
              else if (gh_typ.Value is GsaElement2dGoo)
              {
                GsaElement2d gsaelem2 = new GsaElement2d();
                gh_typ.CastTo(ref gsaelem2);
                list.Add(gsaelem2.Duplicate());
              }
              else if (gh_typ.Value is GsaElement3dGoo)
              {
                GsaElement3d gsaelem3 = new GsaElement3d();
                gh_typ.CastTo(ref gsaelem3);
                list.Add(gsaelem3.Duplicate());
              }
              else
              {
                owner.AddRuntimeError("Unable to convert " + owner.Params.Input[inputid].NickName + " input (index: " + i + ") input parameter of type " +
                gh_typ.Value.GetType().Name.Replace("Goo", string.Empty) + Environment.NewLine + " to Element and has been ignored");
                continue;
              }
              break;

            case EntityType.Member:

              if (gh_typ.Value is GsaMember1dGoo)
              {
                GsaMember1d gsamem1 = new GsaMember1d();
                gh_typ.CastTo(ref gsamem1);
                list.Add(gsamem1.Duplicate());
              }
              else if (gh_typ.Value is GsaMember2dGoo)
              {
                GsaMember2d gsamem2 = new GsaMember2d();
                gh_typ.CastTo(ref gsamem2);
                list.Add(gsamem2.Duplicate());
              }
              else if (gh_typ.Value is GsaMember3dGoo)
              {
                GsaMember3d gsamem3 = new GsaMember3d();
                gh_typ.CastTo(ref gsamem3);
                list.Add(gsamem3.Duplicate());
              }
              else
              {
                owner.AddRuntimeError("Unable to convert " + owner.Params.Input[inputid].NickName + " input (index: " + i + ") input parameter of type " +
                gh_typ.Value.GetType().Name.Replace("Goo", string.Empty) + Environment.NewLine + " to Member and has been ignored");
                continue;
              }
              break;

            case EntityType.Case:
              int caseId = 0;
              if (!GH_Convert.ToInt32(gh_typ.Value, out caseId, GH_Conversion.Both))
              {
                owner.AddRuntimeError("Unable to convert " + owner.Params.Input[inputid].NickName + " input (index: " + i + ") input parameter of type " +
                gh_typ.Value.GetType().Name.Replace("Goo", string.Empty) + Environment.NewLine + " to Integer and has been ignored");
                continue;
              }
              list.Add(caseId);
              break;
          }
        }
      }
      return list;
    }
  }
}
