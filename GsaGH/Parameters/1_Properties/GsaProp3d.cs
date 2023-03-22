using GsaAPI;
using GsaGH.Helpers.GsaAPI;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

namespace GsaGH.Parameters
{
		/// <summary>
		/// Prop2d class, this class defines the basic properties and methods for any <see cref="GsaAPI.Prop3D"/>
		/// </summary>
		public class GsaProp3d
		{
				#region fields
				private int _id = 0;
				private Guid _guid = Guid.NewGuid();
				private Prop3D _prop3d = new Prop3D();
				private GsaMaterial _material = new GsaMaterial();
				#endregion

				#region properties
				internal Prop3D API_Prop3d
				{
						get
						{
								return _prop3d;
						}
						set
						{
								_guid = Guid.NewGuid();
								_prop3d = value;
								_material = new GsaMaterial(this);
								IsReferencedByID = false;
						}
				}
				public int Id
				{
						get
						{
								return _id;
						}
						set
						{
								_guid = Guid.NewGuid();
								_id = value;
						}
				}
				internal bool IsReferencedByID { get; set; } = false;
				public GsaMaterial Material
				{
						get
						{
								return _material;
						}
						set
						{
								_material = value;
								if (_prop3d == null)
										_prop3d = new Prop3D();
								else
										CloneApiObject();

								_prop3d.MaterialType = Helpers.Export.Materials.ConvertType(_material);
								_prop3d.MaterialAnalysisProperty = _material.AnalysisProperty;
								_prop3d.MaterialGradeProperty = _material.GradeProperty;
								IsReferencedByID = false;
						}
				}
				#region GsaAPI members
				public string Name
				{
						get
						{
								return _prop3d.Name;
						}
						set
						{
								CloneApiObject();
								_prop3d.Name = value;
								IsReferencedByID = false;
						}
				}
				public int MaterialID
				{
						get
						{
								return _prop3d.MaterialAnalysisProperty;
						}
						set
						{
								CloneApiObject();
								_prop3d.MaterialAnalysisProperty = value;
								_material.AnalysisProperty = _prop3d.MaterialAnalysisProperty;
								IsReferencedByID = false;
						}
				}
				public int AxisProperty
				{
						get
						{
								return _prop3d.AxisProperty;
						}
						set
						{
								CloneApiObject();
								value = Math.Min(1, value);
								value = Math.Max(0, value);
								_prop3d.AxisProperty = value * -1;
								IsReferencedByID = false;
						}
				}
				public Color Colour
				{
						get
						{
								return (Color)_prop3d.Colour;
						}
						set
						{
								CloneApiObject();
								_prop3d.Colour = value;
								IsReferencedByID = false;
						}
				}
				#endregion
				public Guid Guid
				{
						get
						{
								return _guid;
						}
				}
				#endregion

				#region constructors
				public GsaProp3d()
				{
				}

				public GsaProp3d(int id)
				{
						_id = id;
						IsReferencedByID = true;
				}

				public GsaProp3d(GsaMaterial material)
				{
						Material = material;
				}

				internal GsaProp3d(ReadOnlyDictionary<int, Prop3D> pDict, int id, ReadOnlyDictionary<int, AnalysisMaterial> matDict) : this(id)
				{
						if (!pDict.ContainsKey(id))
								return;
						_prop3d = pDict[id];
						IsReferencedByID = false;
						// material
						if (_prop3d.MaterialAnalysisProperty != 0 && matDict.ContainsKey(_prop3d.MaterialAnalysisProperty))
								_material.AnalysisMaterial = matDict[_prop3d.MaterialAnalysisProperty];
						_material = new GsaMaterial(this);
				}
				#endregion

				#region methods
				public GsaProp3d Duplicate(bool cloneApiElement = false)
				{
						var dup = new GsaProp3d
						{
								_prop3d = _prop3d,
								_id = _id,
								_material = _material.Duplicate(),
								_guid = new Guid(_guid.ToString()),
								IsReferencedByID = IsReferencedByID
						};
						if (cloneApiElement)
								dup.CloneApiObject();
						return dup;
				}

				public override string ToString()
				{
						string type = Mappings.s_materialTypeMapping.FirstOrDefault(x => x.Value == Material.MaterialType).Key;
						string pa = (Id > 0) ? "PV" + Id + " " : "";
						return string.Join(" ", pa.Trim(), type.Trim()).Trim().Replace("  ", " ");
				}

				private void CloneApiObject()
				{
						var prop = new Prop3D
						{
								MaterialAnalysisProperty = _prop3d.MaterialAnalysisProperty,
								MaterialGradeProperty = _prop3d.MaterialGradeProperty,
								MaterialType = _prop3d.MaterialType,
								Name = _prop3d.Name.ToString(),
								AxisProperty = _prop3d.AxisProperty
						};
						if ((Color)_prop3d.Colour != Color.FromArgb(0, 0, 0)) // workaround to handle that Color is non-nullable type
								prop.Colour = _prop3d.Colour;

						_prop3d = prop;
						_guid = Guid.NewGuid();
				}
				#endregion
		}
}
