﻿using GsaAPI;
using GsaGH.Helpers.GsaAPI;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace GsaGH.Parameters
{
		/// <summary>
		/// Prop2d class, this class defines the basic properties and methods for any <see cref="GsaAPI.Prop2D"/>
		/// </summary>
		public class GsaProp2d
		{
				#region fields
				private int _id = 0;
				private Guid _guid = Guid.NewGuid();
				private GsaMaterial _material = new GsaMaterial();
				private Prop2D _prop2d = new Prop2D();
				private Plane _localAxis = Plane.Unset;
				#endregion

				#region properties
				internal Prop2D API_Prop2d
				{
						get
						{
								return _prop2d;
						}
						set
						{
								_guid = Guid.NewGuid();
								_prop2d = value;
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
								if (_prop2d == null)
										_prop2d = new Prop2D();
								else
										CloneApiObject();

								_prop2d.MaterialType = Helpers.Export.Materials.ConvertType(_material);
								_prop2d.MaterialAnalysisProperty = _material.AnalysisProperty;
								_prop2d.MaterialGradeProperty = _material.GradeProperty;
								IsReferencedByID = false;
						}
				}
				#region GsaAPI members
				public string Name
				{
						get
						{
								return _prop2d.Name;
						}
						set
						{
								CloneApiObject();
								_prop2d.Name = value;
								IsReferencedByID = false;
						}
				}
				public int MaterialID
				{
						get
						{
								return
										_prop2d.MaterialAnalysisProperty;
						}
						set
						{
								CloneApiObject();
								IsReferencedByID = false;
								_prop2d.MaterialAnalysisProperty = value;
								_material.AnalysisProperty = _prop2d.MaterialAnalysisProperty;
						}
				}
				public Length Thickness
				{
						get
						{
								if (_prop2d.Description.Length == 0)
										return Length.Zero;
								if (_prop2d.Description.Last() == ')')
								{
										// thickness could be written as "30.33(in)"
										string unitAbbreviation = _prop2d.Description.Split('(', ')')[1];
										LengthUnit unit = UnitParser.Default.Parse<LengthUnit>(unitAbbreviation);

										double val = double.Parse(_prop2d.Description.Split('(')[0], System.Globalization.CultureInfo.InvariantCulture);

										return new Length(val, unit);
								}
								else
										return new Length(double.Parse(_prop2d.Description, CultureInfo.InvariantCulture), LengthUnit.Millimeter);
						}
						set
						{
								CloneApiObject();
								IQuantity length = new Length(0, value.Unit);
								string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
								_prop2d.Description = value.Value.ToString() + "(" + unitAbbreviation + ")";
								IsReferencedByID = false;
						}
				}
				public string Description
				{
						get
						{
								return _prop2d.Description.Replace("%", " ");
						}
						set
						{
								CloneApiObject();
								_prop2d.Description = value;
								IsReferencedByID = false;
						}
				}
				public int AxisProperty
				{
						get
						{
								return _prop2d.AxisProperty;
						}
						set
						{
								CloneApiObject();
								_prop2d.AxisProperty = value;
								IsReferencedByID = false;
						}
				}
				public Property2D_Type Type
				{
						get
						{
								return _prop2d.Type;
						}
						set
						{
								CloneApiObject();
								_prop2d.Type = value;
								IsReferencedByID = false;
						}
				}
				public Color Colour
				{
						get
						{
								return (Color)_prop2d.Colour;
						}
						set
						{
								CloneApiObject();
								_prop2d.Colour = value;
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
				public Plane LocalAxis
				{
						get
						{
								return _localAxis;
						}
						set
						{
								_localAxis = value;
								CloneApiObject();
								_prop2d.AxisProperty = -2;
								IsReferencedByID = false;
						}
				}
				#endregion

				#region constructors
				public GsaProp2d()
				{
				}

				public GsaProp2d(int id)
				{
						_id = id;
						IsReferencedByID = true;
				}

				public GsaProp2d(Length thickness, int id = 0)
				{
						Thickness = thickness;
						_id = id;
				}

				internal GsaProp2d(ReadOnlyDictionary<int, Prop2D> pDict, int id, ReadOnlyDictionary<int, AnalysisMaterial> matDict, ReadOnlyDictionary<int, Axis> axDict, LengthUnit unit) : this(id)
				{
						if (!pDict.ContainsKey(id))
								return;
						_prop2d = pDict[id];
						IsReferencedByID = false;
						// material
						if (_prop2d.MaterialAnalysisProperty != 0 && matDict.ContainsKey(_prop2d.MaterialAnalysisProperty))
								_material.AnalysisMaterial = matDict[_prop2d.MaterialAnalysisProperty];
						if (_prop2d.AxisProperty > 0)
						{
								if (axDict != null && axDict.ContainsKey(_prop2d.AxisProperty))
								{
										Axis ax = axDict[_prop2d.AxisProperty];
										LocalAxis = new Plane(new Point3d(
												new Length(ax.Origin.X, LengthUnit.Meter).As(unit),
												new Length(ax.Origin.Y, LengthUnit.Meter).As(unit),
												new Length(ax.Origin.Z, LengthUnit.Meter).As(unit)),
												new Vector3d(ax.XVector.X, ax.XVector.Y, ax.XVector.Z),
												new Vector3d(ax.XYPlane.X, ax.XYPlane.Y, ax.XYPlane.Z)
												);
								}
						}
						_material = new GsaMaterial(this);
				}
				#endregion

				#region methods


				public GsaProp2d Duplicate(bool cloneApiElement = false)
				{
						var dup = new GsaProp2d
						{
								_prop2d = _prop2d,
								_id = _id,
								_material = _material.Duplicate(),
								_guid = new Guid(_guid.ToString()),
								_localAxis = new Plane(_localAxis),
								IsReferencedByID = IsReferencedByID
						};
						if (cloneApiElement)
								dup.CloneApiObject();
						return dup;
				}

				public override string ToString()
				{
						string type = Mappings.s_prop2dTypeMapping.FirstOrDefault(x => x.Value == _prop2d.Type).Key + " ";
						string desc = Description.Replace("(", string.Empty).Replace(")", string.Empty) + " ";
						string mat = Mappings.s_materialTypeMapping.FirstOrDefault(x => x.Value == Material.MaterialType).Key + " ";
						string pa = (Id > 0) ? "PA" + Id + " " : "";
						return string.Join(" ", pa.Trim(), type.Trim(), desc.Trim(), mat.Trim()).Trim().Replace("  ", " ");
				}
				internal static Property2D_Type PropTypeFromString(string type)
				{
						try
						{
								return Mappings.GetProperty2D_Type(type);
						}
						catch (ArgumentException)
						{
								type = type.Trim().Replace(" ", "_").ToUpper();
								type = type.Replace("PLANE", "PL");
								type = type.Replace("NUMBER", "NUM");
								type = type.Replace("AXIS_SYMMETRIC", "AXISYMMETRIC");
								type = type.Replace("LOAD_PANEL", "LOAD");
								return (Property2D_Type)Enum.Parse(typeof(Property2D_Type), type);
						}
				}

				private void CloneApiObject()
				{
						_prop2d = GetApiObject();
						_guid = Guid.NewGuid();
				}

				private Prop2D GetApiObject()
				{
						if (_prop2d == null)
								return new Prop2D();
						var prop = new Prop2D
						{
								MaterialAnalysisProperty = _prop2d.MaterialAnalysisProperty,
								MaterialGradeProperty = _prop2d.MaterialGradeProperty,
								MaterialType = _prop2d.MaterialType,
								Name = _prop2d.Name.ToString(),
								Description = _prop2d.Description.ToString(),
								Type = _prop2d.Type, //GsaToModel.Prop2dType((int)m_prop2d.Type),
								AxisProperty = _prop2d.AxisProperty
						};
						if ((Color)_prop2d.Colour != Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
								prop.Colour = _prop2d.Colour;

						return prop;
				}
				#endregion
		}
}
