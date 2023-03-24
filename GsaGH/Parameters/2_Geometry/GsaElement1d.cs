﻿using GsaAPI;
using GsaGH.Helpers.GsaAPI;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

namespace GsaGH.Parameters
{
		/// <summary>
		/// Element1d class, this class defines the basic properties and methods for any Gsa Element 1d
		/// </summary>
		public class GsaElement1d
		{
				#region fields
				internal List<Line> PreviewGreenLines;
				internal List<Line> PreviewRedLines;

				private int _id = 0;
				private Guid _guid = Guid.NewGuid();
				private LineCurve _line = new LineCurve();
				private GsaBool6 _rel1;
				private GsaBool6 _rel2;
				private GsaNode _orientationNode;
				#endregion

				#region properties
				public int Id
				{
						get
						{
								return _id;
						}
						set
						{
								CloneApiObject();
								_id = value;
						}
				}
				internal GsaLocalAxes LocalAxes { get; set; } = null;
				public LineCurve Line
				{
						get
						{
								return _line;
						}
						set
						{
								_line = value;
								_guid = Guid.NewGuid();
								UpdatePreview();
						}
				}
				public GsaBool6 ReleaseStart
				{
						get
						{
								return new GsaBool6(ApiElement.GetEndRelease(0).Releases);
						}
						set
						{
								_rel1 = value;
								if (_rel1 == null)
								{
										_rel1 = new GsaBool6();
								}
								CloneApiObject();
								ApiElement.SetEndRelease(0, new EndRelease(_rel1._bool6));
								UpdatePreview();
						}
				}
				public GsaBool6 ReleaseEnd
				{
						get
						{
								return new GsaBool6(ApiElement.GetEndRelease(1).Releases);
						}
						set
						{
								_rel2 = value;
								if (_rel2 == null)
								{
										_rel2 = new GsaBool6();
								}
								CloneApiObject();
								ApiElement.SetEndRelease(1, new EndRelease(_rel2._bool6));
								UpdatePreview();
						}
				}
				public GsaSection Section { get; set; } = new GsaSection();
				internal Element ApiElement { get; set; } = new Element();

				public Color Colour
				{
						get
						{
								return (Color)ApiElement.Colour;
						}
						set
						{
								CloneApiObject();
								ApiElement.Colour = value;
						}
				}
				public int Group
				{
						get
						{
								return ApiElement.Group;
						}
						set
						{
								CloneApiObject();
								ApiElement.Group = value;
						}
				}
				public bool IsDummy
				{
						get
						{
								return ApiElement.IsDummy;
						}
						set
						{
								CloneApiObject();
								ApiElement.IsDummy = value;
						}
				}
				public string Name
				{
						get
						{
								return ApiElement.Name;
						}
						set
						{
								CloneApiObject();
								ApiElement.Name = value;
						}
				}
				public GsaOffset Offset
				{
						get
						{
								return new GsaOffset(ApiElement.Offset.X1, ApiElement.Offset.X2, ApiElement.Offset.Y, ApiElement.Offset.Z);
						}
						set
						{
								CloneApiObject();
								ApiElement.Offset.X1 = value.X1.Meters;
								ApiElement.Offset.X2 = value.X2.Meters;
								ApiElement.Offset.Y = value.Y.Meters;
								ApiElement.Offset.Z = value.Z.Meters;
						}
				}
				public Angle OrientationAngle
				{
						get
						{
								return new Angle(ApiElement.OrientationAngle, AngleUnit.Degree).ToUnit(AngleUnit.Radian);
						}
						set
						{
								CloneApiObject();
								ApiElement.OrientationAngle = value.Degrees;
						}
				}
				public GsaNode OrientationNode
				{
						get
						{
								return _orientationNode;
						}
						set
						{
								CloneApiObject();
								_orientationNode = value;
						}
				}
				public int ParentMember
				{
						get
						{
								return ApiElement.ParentMember.Member;
						}
				}
				public ElementType Type
				{
						get
						{
								return ApiElement.Type;
						}
						set
						{
								CloneApiObject();
								ApiElement.Type = value;
						}
				}
				public Guid Guid
				{
						get
						{
								return _guid;
						}
				}
				#endregion

				#region constructors
				public GsaElement1d()
				{
				}

				public GsaElement1d(LineCurve line, int prop = 0, int id = 0, GsaNode orientationNode = null)
				{
						ApiElement = new Element
						{
								Type = ElementType.BEAM,
						};
						_line = line;
						Id = Id;
						Section.Id = prop;
						_orientationNode = orientationNode;
						UpdatePreview();
				}

				internal GsaElement1d(Element elem, LineCurve line, int id, GsaSection section, GsaNode orientationNode)
				{
						ApiElement = elem;
						_line = line;
						_rel1 = new GsaBool6(ApiElement.GetEndRelease(0).Releases);
						_rel2 = new GsaBool6(ApiElement.GetEndRelease(1).Releases);
						Id = id;
						Section = section;
						_orientationNode = orientationNode;
						UpdatePreview();
				}

				internal GsaElement1d(ReadOnlyDictionary<int, Element> eDict, int id, ReadOnlyDictionary<int, Node> nDict,
						ReadOnlyDictionary<int, Section> sDict, ReadOnlyDictionary<int, SectionModifier> modDict, ReadOnlyDictionary<int, AnalysisMaterial> matDict,
						Dictionary<int, ReadOnlyCollection<double>> localAxesDict, LengthUnit modelUnit)
				{
						Id = id;
						ApiElement = eDict[id];
						_rel1 = new GsaBool6(ApiElement.GetEndRelease(0).Releases);
						_rel2 = new GsaBool6(ApiElement.GetEndRelease(1).Releases);
						if (ApiElement.OrientationNode > 0)
								_orientationNode = new GsaNode(Helpers.Import.Nodes.Point3dFromNode(nDict[ApiElement.OrientationNode], modelUnit));
						_line = new LineCurve(new Line(
								Helpers.Import.Nodes.Point3dFromNode(nDict[ApiElement.Topology[0]], modelUnit),
								Helpers.Import.Nodes.Point3dFromNode(nDict[ApiElement.Topology[1]], modelUnit)));
						LocalAxes = new GsaLocalAxes(localAxesDict[id]);
						Section = new GsaSection(sDict, ApiElement.Property, modDict, matDict);
						UpdatePreview();
				}
				#endregion

				#region methods
				public GsaElement1d Duplicate(bool cloneApiElement = false)
				{
						var dup = new GsaElement1d();
						dup.Id = Id;
						dup.ApiElement = ApiElement;
						dup.LocalAxes = LocalAxes;
						dup._guid = new Guid(_guid.ToString());
						if (cloneApiElement)
								dup.CloneApiObject();
						dup._line = (LineCurve)_line.DuplicateShallow();
						if (_rel1 != null)
								dup._rel1 = _rel1.Duplicate();
						if (_rel2 != null)
								dup._rel2 = _rel2.Duplicate();
						dup.Section = Section.Duplicate();
						if (_orientationNode != null)
								dup._orientationNode = _orientationNode.Duplicate();
						UpdatePreview();
						return dup;
				}

				public GsaElement1d Transform(Transform xform)
				{
						GsaElement1d elem = Duplicate(true);
						elem.Id = 0;
						elem.LocalAxes = null;

						LineCurve xLn = elem.Line;
						xLn.Transform(xform);
						elem.Line = xLn;

						return elem;
				}

				public GsaElement1d Morph(SpaceMorph xmorph)
				{
						GsaElement1d elem = Duplicate(true);
						elem.Id = 0;
						elem.LocalAxes = null;

						LineCurve xLn = Line;
						xmorph.Morph(xLn);
						elem.Line = xLn;

						return elem;
				}

				public override string ToString()
				{
						string idd = Id == 0 ? "" : "ID:" + Id + " ";
						string type = Mappings.s_elementTypeMapping.FirstOrDefault(x => x.Value == Type).Key + " ";
						string pb = Section.Id > 0 ? "PB" + Section.Id : Section.Profile;
						return string.Join(" ", idd.Trim(), type.Trim(), pb.Trim()).Trim().Replace("  ", " ");
				}

				internal void CloneApiObject()
				{
						ApiElement = GetApiElementClone();
						_guid = Guid.NewGuid();
				}

				internal Element GetApiElementClone()
				{
						var elem = new Element()
						{
								Group = ApiElement.Group,
								IsDummy = ApiElement.IsDummy,
								Name = ApiElement.Name.ToString(),
								Offset = ApiElement.Offset,
								OrientationAngle = ApiElement.OrientationAngle,
								OrientationNode = ApiElement.OrientationNode,
								ParentMember = ApiElement.ParentMember,
								Property = ApiElement.Property,
								Type = ApiElement.Type //GsaToModel.Element1dType((int)Element.Type)
						};
						elem.Topology = new ReadOnlyCollection<int>(ApiElement.Topology.ToList());
						elem.SetEndRelease(0, ApiElement.GetEndRelease(0));
						elem.SetEndRelease(1, ApiElement.GetEndRelease(1));
						if ((Color)ApiElement.Colour != Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
								elem.Colour = ApiElement.Colour;
						return elem;
				}

				internal void UpdatePreview()
				{
						if (_rel1 != null & _rel2 != null)
						{
								if (_rel1.X || _rel1.Y || _rel1.Z || _rel1.Xx || _rel1.Yy || _rel1.Zz || _rel2.X || _rel2.Y || _rel2.Z || _rel2.Xx || _rel2.Yy || _rel2.Zz)
								{
										var crv = new PolyCurve();
										crv.Append(_line);
										Tuple<List<Line>, List<Line>> previewCurves = Helpers.Graphics.Display.Preview1D(crv, ApiElement.OrientationAngle * Math.PI / 180.0, _rel1, _rel2);
										PreviewGreenLines = previewCurves.Item1;
										PreviewRedLines = previewCurves.Item2;
								}
								else
										PreviewGreenLines = null;
						}
				}
				#endregion
		}
}
