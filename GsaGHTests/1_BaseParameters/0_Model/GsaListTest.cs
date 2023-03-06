using GsaAPI;
using GsaGH.Parameters;
using OasysUnits.Units;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace GsaGHTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GsaListTest
  {
    [Fact]
    public void TestUndefinedList()
    {
      // Arrange
      GsaAPI.Model m = new GsaAPI.Model();

      EntityList apilist = new EntityList();
      apilist.Name = "undef List";
      apilist.Definition = "1 to 5 not 3";
      apilist.Type = GsaAPI.EntityType.Undefined;

      // Act
      m.AddList(apilist);

      GsaModel model = new GsaModel() { Model = m, ModelUnit = LengthUnit.Meter };
      GsaList list = GsaGH.Helpers.Import.Lists.GetLists(model, LengthUnit.Meter)[0];

      Assert.Equal(1, list.Id);
      Assert.Equal("undef List", list.Name);
      Assert.Equal("1 to 5 not 3", list.Definition);
      Assert.Equal(GsaGH.Parameters.EntityType.Undefined, list.EntityType);
      Assert.Equal("1 to 5 not 3", (string)list.GetListObjects()[0]);
    }

    //[Fact]
    //public void TestCaseList()
    //{
    //  // Arrange
    //  GsaAPI.Model m = new GsaAPI.Model();
    //  m.AddAnalysisTask();
    //  m.AddAnalysisCaseToTask(1, "Case numero uno", "L1");
    //  m.AddAnalysisCaseToTask(1, "Case numero due", "A2");

    //  EntityList apilist = new EntityList();
    //  apilist.Name = "case List";
    //  apilist.Definition = "All";
    //  apilist.Type = GsaAPI.EntityType.Case;

    //  // Act
    //  m.AddList(apilist);
    //  ReadOnlyCollection<int> cases1 = m.ExpandList(apilist);

    //  GsaModel model = new GsaModel() { Model = m, ModelUnit = LengthUnit.Meter };
    //  GsaList list = GsaGH.Helpers.Import.Lists.GetLists(model, LengthUnit.Meter)[0];
    //  m.Save();
    //  // Assert
    //  Assert.Equal(1, list.Id);
    //  Assert.Equal("case List", list.Name);
    //  Assert.Equal("all", list.Definition);
    //  Assert.Equal(GsaGH.Parameters.EntityType.Case, list.EntityType);
    //  List<object> cases = list.GetListObjects();
    //  Assert.Equal(2, cases.Count);
    //  Assert.Equal(1, (int)cases[0]);
    //  Assert.Equal(2, (int)cases[1]);
    //}

    [Fact]
    public void TestNodeList()
    {
      // Arrange
      Node n1 = new Node() { Position = new Vector3() { X = 0, Y = 0, Z = 0 } };
      Node n2 = new Node() { Position = new Vector3() { X = 10, Y = 10, Z = 0 } };
      Node n3 = new Node() { Position = new Vector3() { X = 0, Y = 0, Z = 3.5 } };
      GsaAPI.Model m = new GsaAPI.Model();
      m.AddNode(n1);
      m.AddNode(n2);
      m.AddNode(n3);
      EntityList apilist = new EntityList();
      apilist.Name = "node List";
      apilist.Definition = "All NOT 2";
      apilist.Type = GsaAPI.EntityType.Node;

      // Act
      m.AddList(apilist);
      GsaModel model = new GsaModel() { Model = m, ModelUnit = LengthUnit.Meter };
      GsaList list = GsaGH.Helpers.Import.Lists.GetLists(model, LengthUnit.Meter)[0];

      // Assert
      Assert.Equal(1, list.Id);
      Assert.Equal("node List", list.Name);
      Assert.Equal("all not 2", list.Definition);
      Assert.Equal(GsaGH.Parameters.EntityType.Node, list.EntityType);
      List<object> nodes = list.GetListObjects();
      Assert.Equal(2, nodes.Count);
      Assert.Equal(1, ((GsaNodeGoo)nodes[0]).Value.Id);
      Assert.Equal(3, ((GsaNodeGoo)nodes[1]).Value.Id);
      Assert.Equal(3.5, ((GsaNodeGoo)nodes[1]).Value.Point.Z);
    }

    [Fact]
    public void TestElementList()
    {
      // Arrange
      Node n1 = new Node() { Position = new Vector3() { X = 0, Y = 0, Z = 0 } };
      Node n2 = new Node() { Position = new Vector3() { X = 10, Y = 0, Z = 0 } };
      Node n3 = new Node() { Position = new Vector3() { X = 10, Y = 10, Z = 3.5 } };
      Node n4 = new Node() { Position = new Vector3() { X = 0, Y = 10, Z = 3.5 } };
      Element e1 = new Element() { Type = ElementType.BEAM, Topology = new ReadOnlyCollection<int>(new List<int>() { 1, 2 }) };
      Element e2 = new Element() { Type = ElementType.BEAM, Topology = new ReadOnlyCollection<int>(new List<int>() { 2, 3 }) };
      Element e3 = new Element() { Type = ElementType.BEAM, Topology = new ReadOnlyCollection<int>(new List<int>() { 3, 4 }) };
      Element e4 = new Element() { Type = ElementType.QUAD4, Topology = new ReadOnlyCollection<int>(new List<int>() { 1, 2, 3, 4 }) };
      GsaAPI.Model m = new GsaAPI.Model();
      m.AddNode(n1);
      m.AddNode(n2);
      m.AddNode(n3);
      m.AddNode(n4);
      m.AddElement(e1);
      m.AddElement(e2);
      m.AddElement(e3);
      m.AddElement(e4);

      EntityList apilist = new EntityList();
      apilist.Name = "elem List";
      apilist.Definition = "1 2 4";
      apilist.Type = GsaAPI.EntityType.Element;

      // Act
      m.AddList(apilist);
      GsaModel model = new GsaModel() { Model = m, ModelUnit = LengthUnit.Meter };
      GsaList list = GsaGH.Helpers.Import.Lists.GetLists(model, LengthUnit.Meter)[0];

      // Assert
      Assert.Equal(1, list.Id);
      Assert.Equal("elem List", list.Name);
      Assert.Equal("1 2 4", list.Definition);
      Assert.Equal(GsaGH.Parameters.EntityType.Element, list.EntityType);
      List<object> elems = list.GetListObjects();
      Assert.Equal(3, elems.Count);
      Assert.Equal(1, ((GsaElement1dGoo)elems[0]).Value.Id);
      Assert.Equal(2, ((GsaElement1dGoo)elems[1]).Value.Id);
      Assert.Equal(4, ((GsaElement2dGoo)elems[2]).Value.Ids[0]);
      Assert.Equal(10, ((GsaElement1dGoo)elems[1]).Value.Line.PointAtStart.X);
    }

    [Fact]
    public void TestMemberList()
    {
      // Arrange
      Node n1 = new Node() { Position = new Vector3() { X = 0, Y = 0, Z = 0 } };
      Node n2 = new Node() { Position = new Vector3() { X = 10, Y = 0, Z = 0 } };
      Node n3 = new Node() { Position = new Vector3() { X = 10, Y = 10, Z = 3.5 } };
      Node n4 = new Node() { Position = new Vector3() { X = 0, Y = 10, Z = 3.5 } };
      Member m1 = new Member() { Type = MemberType.GENERIC_1D, Topology = "1 2" };
      Member m2 = new Member() { Type = MemberType.GENERIC_2D, Topology = "1 2 3 4" };
      Member m3 = new Member() { Type = MemberType.GENERIC_1D, Topology = "2 3" };
      GsaAPI.Model m = new GsaAPI.Model();
      m.AddNode(n1);
      m.AddNode(n2);
      m.AddNode(n3);
      m.AddNode(n4);
      m.AddMember(m1);
      m.AddMember(m2);
      m.AddMember(m3);

      EntityList apilist = new EntityList();
      apilist.Name = "mem List";
      apilist.Definition = "all not 1";
      apilist.Type = GsaAPI.EntityType.Member;

      // Act
      m.AddList(apilist);
      GsaModel model = new GsaModel() { Model = m, ModelUnit = LengthUnit.Meter };
      GsaList list = GsaGH.Helpers.Import.Lists.GetLists(model, LengthUnit.Meter)[0];

      // Assert
      Assert.Equal(1, list.Id);
      Assert.Equal("mem List", list.Name);
      Assert.Equal("all not 1", list.Definition);
      Assert.Equal(GsaGH.Parameters.EntityType.Member, list.EntityType);
      List<object> mems = list.GetListObjects();
      Assert.Equal(2, mems.Count);
      Assert.Equal(3, ((GsaMember1dGoo)mems[0]).Value.Id);
      Assert.Equal(10, ((GsaMember1dGoo)mems[0]).Value.Topology[0].X);
      Assert.Equal(2, ((GsaMember2dGoo)mems[1]).Value.Id);
      Assert.Equal(3.5, ((GsaMember2dGoo)mems[1]).Value.Topology[3].Z);
    }
  }
}
