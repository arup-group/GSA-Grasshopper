using GsaAPI;
using GsaGH.Helpers.GsaAPI;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Collections;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

namespace GsaGH.Parameters
{
		/// <summary>
		/// Member1d class, this class defines the basic properties and methods for any Gsa Member 1d
		/// </summary>
		public class GsaMember1d
		{
				#region fields
				internal List<Line> PreviewGreenLines;
				internal List<Line> PreviewRedLines;

				private PolyCurve _crv = new PolyCurve(); // Polyline for visualisation /member1d/member2d
				private List<Point3d> _topo; // list of topology points for visualisation /member1d/member2d
				private List<string> _topoType; //list of polyline curve type (arch or line) for member1d/2d
				private GsaBool6 _rel1;
				private GsaBool6 _rel2;
				private GsaNode _orientationNode;
				private Guid _guid = Guid.NewGuid();
				private int _id = 0;
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
				internal Member ApiMember { get; set; } = new Member();
				public double MeshSize { get; set; } = 0;
				public GsaSection Section { get; set; } = new GsaSection();
				internal GsaLocalAxes LocalAxes { get; set; } = null;
				public List<Point3d> Topology => _topo;
				public List<string> TopologyType => _topoType;
				public PolyCurve PolyCurve
				{
						get
						{
								return _crv;
						}
						set
						{
								Tuple<PolyCurve, List<Point3d>, List<string>> convertCrv = Helpers.GH.RhinoConversions.ConvertMem1dCrv(value);
								_crv = convertCrv.Item1;
								_topo = convertCrv.Item2;
								_topoType = convertCrv.Item3;
								UpdatePreview();
						}
				}
				public GsaBool6 ReleaseStart
				{
						get
						{
								return new GsaBool6(ApiMember.GetEndRelease(0).Releases);
						}
						set
						{
								_rel1 = value;
								if (_rel1 == null)
								{
										_rel1 = new GsaBool6();
								}
								CloneApiObject();
								ApiMember.SetEndRelease(0, new EndRelease(_rel1._bool6));
								UpdatePreview();
						}
				}
				public GsaBool6 ReleaseEnd
				{
						get
						{
								return new GsaBool6(ApiMember.GetEndRelease(1).Releases);
						}
						set
						{
								_rel2 = value;
								if (_rel2 == null)
								{
										_rel2 = new GsaBool6();
								}
								CloneApiObject();
								ApiMember.SetEndRelease(1, new EndRelease(_rel2._bool6));
								UpdatePreview();
						}
				}
				internal Member GetAPI_MemberClone()
				{
						var mem = new Member
						{
								Group = ApiMember.Group,
								IsDummy = ApiMember.IsDummy,
								IsIntersector = ApiMember.IsIntersector,
								LateralTorsionalBucklingFactor = ApiMember.LateralTorsionalBucklingFactor,
								MeshSize = ApiMember.MeshSize,
								MomentAmplificationFactorStrongAxis = ApiMember.MomentAmplificationFactorStrongAxis,
								MomentAmplificationFactorWeakAxis = ApiMember.MomentAmplificationFactorWeakAxis,
								Name = ApiMember.Name.ToString(),
								Offset = ApiMember.Offset,
								OrientationAngle = ApiMember.OrientationAngle,
								OrientationNode = ApiMember.OrientationNode,
								Property = ApiMember.Property,
								Type = ApiMember.Type,
								Type1D = ApiMember.Type1D
						};
						if (ApiMember.Topology != String.Empty)
								mem.Topology = ApiMember.Topology;

						mem.MeshSize = MeshSize;

						mem.SetEndRelease(0, ApiMember.GetEndRelease(0));
						mem.SetEndRelease(1, ApiMember.GetEndRelease(1));

						if ((Color)ApiMember.Colour != Color.FromArgb(0, 0, 0)) // workaround to handle that Color is non-nullable type
								mem.Colour = ApiMember.Colour;

						return mem;
				}
				public Color Colour
				{
						get
						{
								return (Color)ApiMember.Colour;
						}
						set
						{
								CloneApiObject();
								ApiMember.Colour = value;
								UpdatePreview();
						}
				}
				public int Group
				{
						get
						{
								return ApiMember.Group;
						}
						set
						{
								CloneApiObject();
								ApiMember.Group = value;
						}
				}
				public bool IsDummy
				{
						get
						{
								return ApiMember.IsDummy;
						}
						set
						{
								CloneApiObject();
								ApiMember.IsDummy = value;
								UpdatePreview();
						}
				}
				public string Name
				{
						get
						{
								return ApiMember.Name;
						}
						set
						{
								CloneApiObject();
								ApiMember.Name = value;
						}
				}
				public bool MeshWithOthers
				{
						get
						{
								return ApiMember.IsIntersector;
						}
						set
						{
								CloneApiObject();
								ApiMember.IsIntersector = value;
						}
				}
				public GsaOffset Offset
				{
						get
						{
								return new GsaOffset(ApiMember.Offset.X1, ApiMember.Offset.X2, ApiMember.Offset.Y, ApiMember.Offset.Z);
						}
						set
						{
								CloneApiObject();
								ApiMember.Offset.X1 = value.X1.Meters;
								ApiMember.Offset.X2 = value.X2.Meters;
								ApiMember.Offset.Y = value.Y.Meters;
								ApiMember.Offset.Z = value.Z.Meters;
						}
				}
				public Angle OrientationAngle
				{
						get
						{
								return new Angle(ApiMember.OrientationAngle, AngleUnit.Degree).ToUnit(AngleUnit.Radian);
						}
						set
						{
								CloneApiObject();
								ApiMember.OrientationAngle = value.Degrees;
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
				public MemberType Type
				{
						get
						{
								return ApiMember.Type;
						}
						set
						{
								CloneApiObject();
								ApiMember.Type = value;
						}
				}
				public ElementType Type1D
				{
						get
						{
								return ApiMember.Type1D;
						}
						set
						{
								CloneApiObject();
								ApiMember.Type1D = value;
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
				public GsaMember1d()
				{
				}

				public GsaMember1d(Curve crv, int prop = 0)
				{
						ApiMember = new Member
						{
								Type = MemberType.GENERIC_1D,
								Property = prop
						};
						Tuple<PolyCurve, List<Point3d>, List<string>> convertCrv = Helpers.GH.RhinoConversions.ConvertMem1dCrv(crv);
						_crv = convertCrv.Item1;
						_topo = convertCrv.Item2;
						_topoType = convertCrv.Item3;

						UpdatePreview();
				}

				internal GsaMember1d(Member member, int id, List<Point3d> topology, List<string> topo_type, ReadOnlyDictionary<int, Node> nDict,
						ReadOnlyDictionary<int, Section> sDict, ReadOnlyDictionary<int, SectionModifier> modDict, ReadOnlyDictionary<int, AnalysisMaterial> matDict,
						Dictionary<int, ReadOnlyCollection<double>> localAxesDict, LengthUnit modelUnit)
				{
						ApiMember = member;
						MeshSize = new Length(member.MeshSize, LengthUnit.Meter).As(modelUnit);
						_id = id;
						_crv = Helpers.GH.RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(topology, topo_type);
						_topo = topology;
						_topoType = topo_type;
						_rel1 = new GsaBool6(ApiMember.GetEndRelease(0).Releases);
						_rel2 = new GsaBool6(ApiMember.GetEndRelease(1).Releases);
						LocalAxes = new GsaLocalAxes(localAxesDict[id]);
						Section = new GsaSection(sDict, ApiMember.Property, modDict, matDict);
						UpdatePreview();
				}
				#endregion

				#region methods
				public GsaMember1d Duplicate(bool cloneApiMember = false)
				{
						var dup = new GsaMember1d();
						dup.Id = Id;
						dup.MeshSize = MeshSize;
						dup._guid = new Guid(_guid.ToString());
						dup.ApiMember = ApiMember;
						dup.LocalAxes = LocalAxes;
						if (cloneApiMember)
								dup.CloneApiObject();
						dup._crv = (PolyCurve)_crv.DuplicateShallow();
						if (_rel1 != null)
								dup._rel1 = _rel1.Duplicate();
						if (_rel2 != null)
								dup._rel2 = _rel2.Duplicate();
						dup.Section = Section.Duplicate();
						dup._topo = _topo;
						dup._topoType = _topoType;
						if (_orientationNode != null)
								dup._orientationNode = _orientationNode.Duplicate(cloneApiMember);
						dup.UpdatePreview();
						return dup;
				}

				public GsaMember1d Transform(Transform xform)
				{
						GsaMember1d dup = Duplicate(true);
						dup.Id = 0;
						dup.LocalAxes = null;

						var pts = _topo.ToList();
						var xpts = new Point3dList(pts);
						xpts.Transform(xform);
						dup._topo = xpts.ToList();

						if (_crv != null)
						{
								PolyCurve crv = _crv.DuplicatePolyCurve();
								crv.Transform(xform);
								dup._crv = crv;
						}
						dup.UpdatePreview();
						return dup;
				}

				public GsaMember1d Morph(SpaceMorph xmorph)
				{
						GsaMember1d dup = Duplicate(true);
						dup.Id = 0;
						dup.LocalAxes = null;

						var pts = _topo.ToList();
						for (int i = 0; i < pts.Count; i++)
								pts[i] = xmorph.MorphPoint(pts[i]);
						dup._topo = pts;

						if (_crv != null)
						{
								PolyCurve crv = _crv.DuplicatePolyCurve();
								xmorph.Morph(crv);
								dup._crv = crv;
						}
						dup.UpdatePreview();
						return dup;
				}

				public override string ToString()
				{
						string idd = Id == 0 ? "" : "ID:" + Id + " ";
						string type = Mappings.s_memberTypeMapping.FirstOrDefault(x => x.Value == Type).Key + " ";
						string pb = Section.Id > 0 ? "PB" + Section.Id : Section.Profile;
						return string.Join(" ", idd.Trim(), type.Trim(), pb.Trim()).Trim().Replace("  ", " ");
				}

				internal void CloneApiObject()
				{
						ApiMember = GetAPI_MemberClone();
						_guid = Guid.NewGuid();
				}

				internal void UpdateCurveFromTopology()
				{
						if (_crv == null)
								return;
						_crv = Helpers.GH.RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(_topo, _topoType);
				}

				private void UpdatePreview()
				{
						if (_rel1 != null & _rel2 != null)
						{
								if (_rel1.X || _rel1.Y || _rel1.Z || _rel1.Xx || _rel1.Yy || _rel1.Zz || _rel2.X || _rel2.Y || _rel2.Z || _rel2.Xx || _rel2.Yy || _rel2.Zz)
								{
										Tuple<List<Line>, List<Line>> previewCurves = Helpers.Graphics.Display.Preview1D(_crv, ApiMember.OrientationAngle * Math.PI / 180.0, _rel1, _rel2);
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
