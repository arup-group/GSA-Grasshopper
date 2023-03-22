using GsaAPI;
using GsaGH.Helpers.GsaAPI;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GsaGH.Parameters
{
		/// <summary>
		/// Member3d class, this class defines the basic properties and methods for any Gsa Member 3d
		/// </summary>
		public class GsaMember3d
		{
				#region fields
				internal List<Polyline> previewHiddenLines;
				internal List<Line> previewEdgeLines;
				internal List<Point3d> previewPts;

				private int _id = 0;
				private Guid _guid = Guid.NewGuid();
				private Mesh _mesh = new Mesh();
				#endregion

				#region properties
				internal Member ApiMember { get; set; } = new Member();
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
				public double MeshSize
				{
						get; set;
				}
				public GsaProp3d Prop3d { get; set; } = new GsaProp3d();
				public Mesh SolidMesh
				{
						get
						{
								return _mesh;
						}
						set
						{
								_mesh = Helpers.GH.RhinoConversions.ConvertMeshToTriMeshSolid(value);
								_guid = Guid.NewGuid();
								UpdatePreview();
						}
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
				public Guid Guid
				{
						get
						{
								return _guid;
						}
				}
				#endregion

				#region constructors
				public GsaMember3d()
				{
						ApiMember.Type = MemberType.GENERIC_3D;
				}

				public GsaMember3d(Mesh mesh)
				{
						ApiMember = new Member
						{
								Type = MemberType.GENERIC_3D
						};
						_mesh = GsaGH.Helpers.GH.RhinoConversions.ConvertMeshToTriMeshSolid(mesh);
						UpdatePreview();
				}

				public GsaMember3d(Brep brep)
				{
						ApiMember = new Member
						{
								Type = MemberType.GENERIC_3D
						};
						_mesh = GsaGH.Helpers.GH.RhinoConversions.ConvertBrepToTriMeshSolid(brep);
						UpdatePreview();
				}

				internal GsaMember3d(Member member, int id, Mesh mesh, GsaProp3d prop, double meshSize)
				{
						ApiMember = member;
						_id = id;
						_mesh = GsaGH.Helpers.GH.RhinoConversions.ConvertMeshToTriMeshSolid(mesh);
						Prop3d = prop;
						MeshSize = meshSize;
						UpdatePreview();
				}
				#endregion

				#region methods
				public GsaMember3d Duplicate(bool cloneApiMember = false)
				{
						var dup = new GsaMember3d();
						dup.MeshSize = MeshSize;
						dup._mesh = (Mesh)_mesh.DuplicateShallow();
						dup._guid = new Guid(_guid.ToString());
						dup.Prop3d = Prop3d.Duplicate();
						if (cloneApiMember)
								dup.CloneApiObject();
						else
								dup.ApiMember = ApiMember;
						dup.Id = Id;
						dup.UpdatePreview();
						return dup;
				}

				public GsaMember3d UpdateGeometry(Brep brep)
				{
						GsaMember3d dup = Duplicate();
						dup._mesh = GsaGH.Helpers.GH.RhinoConversions.ConvertBrepToTriMeshSolid(brep);
						dup.UpdatePreview();
						return dup;
				}

				public GsaMember3d UpdateGeometry(Mesh mesh)
				{
						GsaMember3d dup = Duplicate();
						dup._mesh = GsaGH.Helpers.GH.RhinoConversions.ConvertMeshToTriMeshSolid(mesh);
						dup.UpdatePreview();
						return dup;
				}

				public GsaMember3d Transform(Transform xform)
				{
						if (SolidMesh == null)
								return null;

						GsaMember3d dup = Duplicate(true);
						dup.Id = 0;
						dup.SolidMesh.Transform(xform);

						return dup;
				}

				public GsaMember3d Morph(SpaceMorph xmorph)
				{
						if (SolidMesh == null)
								return null;

						GsaMember3d dup = Duplicate(true);
						dup.Id = 0;
						xmorph.Morph(dup.SolidMesh.Duplicate());

						return dup;
				}

				public override string ToString()
				{
						string idd = Id == 0 ? "" : "ID:" + Id + " ";
						string type = Mappings.s_memberTypeMapping.FirstOrDefault(x => x.Value == ApiMember.Type).Key;
						return string.Join(" ", idd.Trim(), type.Trim()).Trim().Replace("  ", " ");
				}

				internal void CloneApiObject()
				{
						ApiMember = GetAPI_MemberClone();
						_guid = Guid.NewGuid();
				}

				internal Member GetAPI_MemberClone()
				{
						var mem = new Member
						{
								Group = ApiMember.Group,
								IsDummy = ApiMember.IsDummy,
								MeshSize = ApiMember.MeshSize,
								Name = ApiMember.Name,
								Offset = ApiMember.Offset,
								OrientationAngle = ApiMember.OrientationAngle,
								OrientationNode = ApiMember.OrientationNode,
								Property = ApiMember.Property,
								Type = ApiMember.Type,
						};
						if (ApiMember.Topology != String.Empty)
								mem.Topology = ApiMember.Topology;

						if ((Color)ApiMember.Colour != Color.FromArgb(0, 0, 0)) // workaround to handle that Color is non-nullable type
								mem.Colour = ApiMember.Colour;

						return mem;
				}

				private void UpdatePreview()
				{
						Helpers.Graphics.Display.PreviewMem3d(ref _mesh, ref previewHiddenLines, ref previewEdgeLines, ref previewPts);
				}
				#endregion
		}
}
