using System;
using System.Linq;
using System.Collections.Generic;

namespace GsaGH.Helpers.Import
{
  /// <summary>
  /// Class containing functions to import various object types from GSA
  /// </summary>
  internal class Topology
  {
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
    /// <param name="gsa_topology"></param>
    /// <returns></returns>
    internal static Tuple<Tuple<List<int>, List<string>>, Tuple<List<List<int>>, List<List<string>>>,
        Tuple<List<List<int>>, List<List<string>>>, List<int>> Topology_detangler(string gsa_topology)
    {
      List<string> voids = new List<string>();
      List<string> lines = new List<string>();
      List<string> points = new List<string>();
      //string gsa_topology = "7 8 9 a 10 11 7 V(12 13 a 14 15) L(16 a 18 17) 94 P 20 P(19 21 22) L(23 24) 84";
      gsa_topology = gsa_topology.ToUpper();
      char[] spearator = { '(', ')' };

      String[] strlist = gsa_topology.Split(spearator);
      List<String> topos = new List<String>(strlist);

      // first split out anything in brackets and put them into lists for V, L or P
      // also remove those lines so that they dont appear twice in the end
      for (int i = 0; i < topos.Count(); i++)
      {
        if (topos[i].Length > 1)
        {
          if (topos[i].Substring(topos[i].Length - 1, 1) == "V")
          {
            topos[i] = topos[i].Substring(0, topos[i].Length - 1);
            voids.Add(topos[i + 1]);
            topos.RemoveAt(i + 1);
            continue;
          }
        }

        if (topos[i].Length > 1)
        {
          if (topos[i].Substring(topos[i].Length - 1, 1) == "L")
          {
            topos[i] = topos[i].Substring(0, topos[i].Length - 1);
            lines.Add(topos[i + 1]);
            topos.RemoveAt(i + 1);
            continue;
          }
        }

        if (topos[i].Length > 1)
        {
          if (topos[i].Substring(topos[i].Length - 1, 1) == "P")
          {
            topos[i] = topos[i].Substring(0, topos[i].Length - 1);
            points.Add(topos[i + 1]);
            topos.RemoveAt(i + 1);
            continue;
          }
        }
      }

      // then split list with whitespace
      List<String> topolos = new List<String>();
      for (int i = 0; i < topos.Count(); i++)
      {
        List<String> temptopos = new List<String>(topos[i].Split(' '));
        topolos.AddRange(temptopos);
      }

      // also split list of points by whitespace as they go to single list
      List<String> pts = new List<String>();
      for (int i = 0; i < points.Count(); i++)
      {
        List<String> temppts = new List<String>(points[i].Split(' '));
        pts.AddRange(temppts);
      }

      // voids and lines needs to be made into list of lists
      List<List<int>> void_topo = new List<List<int>>();
      List<List<String>> void_topoType = new List<List<String>>();
      for (int i = 0; i < voids.Count(); i++)
      {
        List<String> tempvoids = new List<String>(voids[i].Split(' '));
        List<int> tmpvds = new List<int>();
        List<String> tmpType = new List<String>();
        for (int j = 0; j < tempvoids.Count(); j++)
        {
          if (tempvoids[j] == "A")
          {
            tmpType.Add("A");
            tempvoids.RemoveAt(j);
          }
          else
            tmpType.Add(" ");
          int tpt = Int32.Parse(tempvoids[j]);
          tmpvds.Add(tpt);
        }
        void_topo.Add(tmpvds);
        void_topoType.Add(tmpType);
      }
      List<List<int>> incLines_topo = new List<List<int>>();
      List<List<String>> inclLines_topoType = new List<List<String>>();
      for (int i = 0; i < lines.Count(); i++)
      {
        List<String> templines = new List<String>(lines[i].Split(' '));
        List<int> tmplns = new List<int>();
        List<String> tmpType = new List<String>();
        for (int j = 0; j < templines.Count(); j++)
        {
          if (templines[j] == "A")
          {
            tmpType.Add("A");
            templines.RemoveAt(j);
          }
          else
            tmpType.Add(" ");
          int tpt = Int32.Parse(templines[j]);
          tmplns.Add(tpt);
        }
        incLines_topo.Add(tmplns);
        inclLines_topoType.Add(tmpType);
      }

      // then remove empty entries
      for (int i = 0; i < topolos.Count(); i++)
      {
        if (topolos[i] == null)
        {
          topolos.RemoveAt(i);
          i -= 1;
          continue;
        }
        if (topolos[i].Length < 1)
        {
          topolos.RemoveAt(i);
          i -= 1;
          continue;
        }
      }

      // Find any single inclusion points not in brackets
      for (int i = 0; i < topolos.Count(); i++)
      {
        if (topolos[i] == "P")
        {
          pts.Add(topolos[i + 1]);
          topolos.RemoveAt(i + 1);
          topolos.RemoveAt(i);
          i -= 1;
          continue;
        }
        if (topolos[i].Length < 1)
        {
          topolos.RemoveAt(i);
          i -= 1;
          continue;
        }
      }
      List<int> inclpoint = new List<int>();
      for (int i = 0; i < pts.Count(); i++)
      {
        if (pts[i] != "")
        {
          int tpt = Int32.Parse(pts[i]);
          inclpoint.Add(tpt);
        }
      }

      // write out topology type (A) to list
      List<int> topoint = new List<int>();
      List<String> topoType = new List<String>();
      for (int i = 0; i < topolos.Count(); i++)
      {
        if (topolos[i] == "A")
        {
          topoType.Add("A");
          int tptA = Int32.Parse(topolos[i + 1]);
          topoint.Add(tptA);
          i += 1;
          continue;
        }
        topoType.Add(" ");
        int tpt = Int32.Parse(topolos[i]);
        topoint.Add(tpt);
      }
      Tuple<List<int>, List<string>> topoTuple = new Tuple<List<int>, List<string>>(topoint, topoType);
      Tuple<List<List<int>>, List<List<string>>> voidTuple = new Tuple<List<List<int>>, List<List<string>>>(void_topo, void_topoType);
      Tuple<List<List<int>>, List<List<string>>> lineTuple = new Tuple<List<List<int>>, List<List<string>>>(incLines_topo, inclLines_topoType);

      return new Tuple<Tuple<List<int>, List<string>>, Tuple<List<List<int>>, List<List<string>>>,
      Tuple<List<List<int>>, List<List<string>>>, List<int>>(topoTuple, voidTuple, lineTuple, inclpoint);
    }

    /// <summary>
    /// Method to convert a topology string from a 3D Member
    /// into a list of 3 verticies
    /// </summary>
    /// <param name="gsa_topology">Topology list as string</param>
    /// <returns></returns>
    internal static List<List<int>> Topology_detangler_Mem3d(string gsa_topology)
    {
      // Example input string ‘1 2 4 3; 5 6 8 7; 1 5 2 6 3 7 4 8 1 5’ 
      // we want to create a triangular mesh for Member3D SolidMesh
      List<List<int>> topolist = new List<List<int>>();

      // first split the string by ";"
      char spearator = ';';

      String[] strlist = gsa_topology.Split(spearator);

      // loop through all face lists
      foreach (string stripe in strlist)
      {
        // trim and split list by white space
        string trimmedstripe = stripe.Trim();
        List<String> verticiesString = new List<String>(trimmedstripe.Split(' '));

        // convert string to int
        List<int> tempverticies = new List<int>();
        foreach (string vert in verticiesString)
        {
          int tpt = Int32.Parse(vert);
          tempverticies.Add(tpt);
        }

        while (tempverticies.Count > 2)
        {
          // add the first triangle
          List<int> templist1 = new List<int>();
          templist1.Add(tempverticies[0]);
          templist1.Add(tempverticies[1]);
          templist1.Add(tempverticies[2]);

          // add the list to the main list
          topolist.Add(templist1);

          if (tempverticies.Count > 3)
          {
            // add the second triangle the other way round
            List<int> templist2 = new List<int>();
            templist2.Add(tempverticies[1]);
            templist2.Add(tempverticies[3]);
            templist2.Add(tempverticies[2]);

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
