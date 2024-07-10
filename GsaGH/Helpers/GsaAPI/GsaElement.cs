using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using GsaAPI;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;
using OasysUnits;
using System.Drawing;
using AngleUnit = OasysUnits.Units.AngleUnit;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
namespace GsaAPI {

  public class GSAElement {
    public static int LOAD_PANEL_TYPE = -1000;
    private GSAElement() { }
    public GSAElement(object element) {
      if ((element as Element) != null) {
        Elelment = element as Element; IsLoadPanel = false;
      }
      else if ((element as LoadPanelElement) != null) {
        LoadPanelElelment = element as LoadPanelElement; IsLoadPanel = true;
      }
      else {
        throw new ArgumentException("Object is not of Element or LoadPanel type");
      }
    }
    public GSAElement(Element element) { Elelment = element; IsLoadPanel = false; }
    public GSAElement(LoadPanelElement element) { LoadPanelElelment = element; IsLoadPanel = true; }

    public bool IsLoadPanel { get; }
    public LoadPanelElement LoadPanelElelment { get; }
    public Element Elelment { get; }

    public string Name {
      get {
        if (IsLoadPanel) {
          return LoadPanelElelment.Name;
        }
        else {
          return Elelment.Name;
        }
      }
      set {
        if (IsLoadPanel) {
          LoadPanelElelment.Name = value;
        }
        else {
          Elelment.Name = value;
        }
      }
    }

    public int Property {
      get {
        if (IsLoadPanel) {
          return LoadPanelElelment.Property;
        }
        else {
          return Elelment.Property;
        }
      }
      set {
        if (IsLoadPanel) {
          LoadPanelElelment.Property = value;
        }
        else {
          Elelment.Property = value;
        }
      }
    }

    public int Group {
      get {
        if (IsLoadPanel) {
          return LoadPanelElelment.Group;
        }
        else {
          return Elelment.Group;
        }
      }
      set {
        if (IsLoadPanel) {
          LoadPanelElelment.Group = value;
        }
        else {
          Elelment.Group = value;
        }
      }
    }

    public ParentMember ParentMember {
      get {
        if (IsLoadPanel) {
          return LoadPanelElelment.ParentMember;
        }
        else {
          return Elelment.ParentMember;
        }
      }
      set {
        if (IsLoadPanel) {
          LoadPanelElelment.ParentMember = value;
        }
        else {
          Elelment.ParentMember = value;
        }
      }
    }

    public bool IsDummy {
      get {
        if (IsLoadPanel) {
          return LoadPanelElelment.IsDummy;
        }
        else {
          return Elelment.IsDummy;
        }
      }
      set {
        if (IsLoadPanel) {
          LoadPanelElelment.IsDummy = value;
        }
        else {
          Elelment.IsDummy = value;
        }
      }
    }

    public Offset Offset {
      get {
        if (IsLoadPanel) {
          return new Element().Offset;
        }
        else {
          return Elelment.Offset;
        }
      }
      set {
        if (!IsLoadPanel) {
          Elelment.Offset = value;
        }
      }
    }

    public double OrientationAngle {
      get {
        if (IsLoadPanel) {
          return LoadPanelElelment.OrientationAngle;
        }
        else {
          return Elelment.OrientationAngle;
        }
      }
      set {
        if (IsLoadPanel) {
          LoadPanelElelment.OrientationAngle = value;
        }
        else {
          Elelment.OrientationAngle = value;
        }
      }
    }

    public int OrientationNode {
      get {
        if (IsLoadPanel) {
          return 0;
        }
        else {
          return Elelment.OrientationNode;
        }
      }
      set {
        if (!IsLoadPanel) {
          Elelment.OrientationNode = value;
        }
      }
    }

    public ReadOnlyCollection<int> Topology {
      get {
        if (IsLoadPanel) {
          return LoadPanelElelment.Topology;
        }
        else {
          return Elelment.Topology;
        }
      }
      set {
        if (IsLoadPanel) {
          LoadPanelElelment.Topology = value;
        }
        else {
          Elelment.Topology = value;
        }
      }
    }

    public ElementType Type {
      get {
        if (IsLoadPanel) {
          return (ElementType)LOAD_PANEL_TYPE;
        }
        else {
          return Elelment.Type;
        }
      }
      set {
        if (!IsLoadPanel) {
          Elelment.Type = value;
        }
      }
    }

    public ValueType Colour {
      get {
        if (IsLoadPanel) {
          return LoadPanelElelment.Colour;
        }
        else {
          return Elelment.Colour;
        }
      }
      set {
        if (IsLoadPanel) {
          LoadPanelElelment.Colour = value;
        }
        else {
          Elelment.Colour = value;
        }
      }
    }

  }

}


