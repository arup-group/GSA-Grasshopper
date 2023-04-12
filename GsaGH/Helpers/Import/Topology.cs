using System;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Helpers.Import {
  /// <summary>
  /// Class containing functions to import various object types from GSA
  /// </summary>
  internal class Topology {

    /// <summary>
    /// Method to split/untangle a topology list from GSA into separate lists for
    /// Topology, Voids, Inclusion lines and Inclusion points with corrosponding list for topology type.
    ///
    /// Output tuple with three sub-tubles for:
    /// - Topology: (Topology integers and topology types)
    /// - Voids: (List of integers and list of topology types)
    /// - Lines: (List of integers and list of topology types)
    /// - Points: (Topology integers)
    ///
    /// Example: gsa_topology =
    /// "7 8 9 a 10 11 7 V(12 13 a 14 15) L(16 a 18 17) 94 P 20 P(19 21 22) L(23 24) 84"
    /// will results in:
    ///
    /// Tuple1, Item1: Topology: (7, 8, 9, 10, 11, 7, 94, 84)
    /// Tuple1, Item2: TopoType: ( ,  ,  ,  a,   ,  ,   ,   )
    ///
    /// Tuple2, Item1: List(Voids): (12, 13, 14, 15)
    /// Tuple2, Item2: List(VType): (  ,   ,  a,   )
    ///
    /// Tuple3, Item1: List(Lines): (16, 18, 17) (23, 24)
    /// Tuple3, Item2: List(LType): (  ,  a,   ) (  ,   )
    ///
    /// Points: (20, 19, 21, 22)
    ///
    /// </summary>
    /// <param name="gsaTopology"></param>
    /// <returns></returns>
    internal static Tuple<Tuple<List<int>, List<string>>, Tuple<List<List<int>>, List<List<string>>>,
        Tuple<List<List<int>>, List<List<string>>>, List<int>> Topology_detangler(string gsaTopology) {
      var voids = new List<string>();
      var lines = new List<string>();
      var points = new List<string>();
      //string gsa_topology = "7 8 9 a 10 11 7 V(12 13 a 14 15) L(16 a 18 17) 94 P 20 P(19 21 22) L(23 24) 84";
      gsaTopology = gsaTopology.ToUpper();
      char[] spearator = { '(', ')' };

      string[] strlist = gsaTopology.Split(spearator);
      var topos = new List<string>(strlist);

      // first split out anything in brackets and put them into lists for V, L or P
      // also remove those lines so that they dont appear twice in the end
      for (int i = 0; i < topos.Count; i++) {
        if (topos[i].Length > 1) {
          if (topos[i].Substring(topos[i].Length - 1, 1) == "V") {
            topos[i] = topos[i].Substring(0, topos[i].Length - 1);
            voids.Add(topos[i + 1]);
            topos.RemoveAt(i + 1);
            continue;
          }
        }

        if (topos[i].Length > 1) {
          if (topos[i].Substring(topos[i].Length - 1, 1) == "L") {
            topos[i] = topos[i].Substring(0, topos[i].Length - 1);
            lines.Add(topos[i + 1]);
            topos.RemoveAt(i + 1);
            continue;
          }
        }

        if (topos[i].Length <= 1) {
          continue;
        }

        if (topos[i].Substring(topos[i].Length - 1, 1) != "P") {
          continue;
        }

        topos[i] = topos[i].Substring(0, topos[i].Length - 1);
        points.Add(topos[i + 1]);
        topos.RemoveAt(i + 1);
      }

      // then split list with whitespace
      var topolos = new List<string>();
      foreach (List<string> temptopos in topos.Select(topo => new List<string>(topo.Split(' ')))) {
        topolos.AddRange(temptopos);
      }

      // also split list of points by whitespace as they go to single list
      var pts = new List<string>();
      foreach (List<string> temppts in points.Select(t => new List<string>(t.Split(' ')))) {
        pts.AddRange(temppts);
      }

      // voids and lines needs to be made into list of lists
      var voidTopo = new List<List<int>>();
      var voidTopoType = new List<List<string>>();
      foreach (string t in voids) {
        var tempvoids = new List<string>(t.Split(' '));
        var tmpvds = new List<int>();
        var tmpType = new List<string>();
        for (int j = 0; j < tempvoids.Count; j++) {
          if (tempvoids[j] == "A") {
            tmpType.Add("A");
            tempvoids.RemoveAt(j);
          }
          else {
            tmpType.Add(" ");
          }

          int tpt = int.Parse(tempvoids[j]);
          tmpvds.Add(tpt);
        }
        voidTopo.Add(tmpvds);
        voidTopoType.Add(tmpType);
      }
      var incLinesTopo = new List<List<int>>();
      var inclLinesTopoType = new List<List<string>>();
      foreach (string line in lines) {
        var templines = new List<string>(line.Split(' '));
        var tmplns = new List<int>();
        var tmpType = new List<string>();
        for (int j = 0; j < templines.Count; j++) {
          if (templines[j] == "A") {
            tmpType.Add("A");
            templines.RemoveAt(j);
          }
          else {
            tmpType.Add(" ");
          }

          int tpt = int.Parse(templines[j]);
          tmplns.Add(tpt);
        }
        incLinesTopo.Add(tmplns);
        inclLinesTopoType.Add(tmpType);
      }

      // then remove empty entries
      for (int i = 0; i < topolos.Count; i++) {
        if (topolos[i] == null) {
          topolos.RemoveAt(i);
          i -= 1;
          continue;
        }

        if (topolos[i].Length >= 1) {
          continue;
        }

        topolos.RemoveAt(i);
        i -= 1;
      }

      // Find any single inclusion points not in brackets
      for (int i = 0; i < topolos.Count; i++) {
        if (topolos[i] == "P") {
          pts.Add(topolos[i + 1]);
          topolos.RemoveAt(i + 1);
          topolos.RemoveAt(i);
          i -= 1;
          continue;
        }

        if (topolos[i].Length >= 1) {
          continue;
        }

        topolos.RemoveAt(i);
        i -= 1;
      }
      var inclpoint = (from t in pts where t != "" select int.Parse(t)).ToList();

      // write out topology type (A) to list
      var topoint = new List<int>();
      var topoType = new List<string>();
      for (int i = 0; i < topolos.Count; i++) {
        if (topolos[i] == "A") {
          topoType.Add("A");
          int tptA = int.Parse(topolos[i + 1]);
          topoint.Add(tptA);
          i += 1;
          continue;
        }
        topoType.Add(" ");
        int tpt = int.Parse(topolos[i]);
        topoint.Add(tpt);
      }
      var topoTuple = new Tuple<List<int>, List<string>>(topoint, topoType);
      var voidTuple = new Tuple<List<List<int>>, List<List<string>>>(voidTopo, voidTopoType);
      var lineTuple = new Tuple<List<List<int>>, List<List<string>>>(incLinesTopo, inclLinesTopoType);

      return new Tuple<Tuple<List<int>, List<string>>, Tuple<List<List<int>>, List<List<string>>>,
      Tuple<List<List<int>>, List<List<string>>>, List<int>>(topoTuple, voidTuple, lineTuple, inclpoint);
    }

    /// <summary>
    /// Method to convert a topology string from a 3D Member
    /// into a list of 3 verticies
    /// </summary>
    /// <param name="gsaTopology">Topology list as string</param>
    /// <returns></returns>
    internal static List<List<int>> Topology_detangler_Mem3d(string gsaTopology) {
      // Example input string ‘1 2 4 3; 5 6 8 7; 1 5 2 6 3 7 4 8 1 5’
      // we want to create a triangular mesh for Member3D SolidMesh
      var topolist = new List<List<int>>();

      char spearator = ';';
      string[] strlist = gsaTopology.Split(spearator);

      foreach (string stripe in strlist) {
        string trimmedstripe = stripe.Trim();
        var verticiesString = new List<string>(trimmedstripe.Split(' '));

        var tempverticies = verticiesString.Select(int.Parse).ToList();

        while (tempverticies.Count > 2) {
          // add the first triangle
          var templist1 = new List<int> { tempverticies[0], tempverticies[1], tempverticies[2] };

          // add the list to the main list
          topolist.Add(templist1);

          if (tempverticies.Count > 3) {
            // add the second triangle the other way round
            var templist2 = new List<int> { tempverticies[1], tempverticies[3], tempverticies[2] };

            // add the list to the main list
            topolist.Add(templist2);

            // remove the first two verticies from list
            tempverticies.RemoveAt(0);
          }
          // put the second remove outside the if to also remove if we only
          // have 3 verticies to bring count below 3 and exit while loop
          tempverticies.RemoveAt(0);
        }
      }
      return topolist;
    }
  }
}
