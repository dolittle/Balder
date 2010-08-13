#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2010, DoLittle Studios
//
// Licensed under the Microsoft Permissive License (Ms-PL), Version 1.1 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the license at 
//
//   http://balder.codeplex.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Balder.Content;
using Balder.Imaging;
using Balder.Materials;
using Balder.Math;
using Balder.Objects.Geometries;
using Balder.Rendering;

namespace Balder.Assets.AssetLoaders
{
	public class AseParser
	{
		// ReSharper disable InconsistentNaming
		public const string GEOMOBJECT = "GEOMOBJECT";

		public const string NODE_NAME = "NODE_NAME";
		public const string MESH = "MESH";

		public const string MATERIAL_LIST = "MATERIAL_LIST";
		public const string MATERIAL_COUNT = "MATERIAL_COUNT";
		public const string MATERIAL = "MATERIAL";
		public const string MAP_DIFFUSE = "MAP_DIFFUSE";
		public const string BITMAP = "BITMAP";

		public const string MESH_NUMVERTEX = "MESH_NUMVERTEX";
		public const string MESH_NUMFACES = "MESH_NUMFACES";
		public const string MESH_NUMTVERTEX = "MESH_NUMTVERTEX";
		public const string MESH_NUMTVFACES = "MESH_NUMTVFACES";
		public const string MESH_NUMCVERTEX = "MESH_NUMCVERTEX";
		public const string MESH_NUMCVFACES = "MESH_NUMCVFACES";

		public const string MESH_VERTEX_LIST = "MESH_VERTEX_LIST";
		public const string MESH_FACE_LIST = "MESH_FACE_LIST";
		public const string MESH_TVERTLIST = "MESH_TVERTLIST";
		public const string MESH_TFACELIST = "MESH_TFACELIST";
		public const string MESH_NORMALS = "MESH_NORMALS";
		public const string MESH_CVERTLIST = "MESH_CVERTLIST";
		public const string MESH_CFACELIST = "MESH_CFACELIST";

		public const string MESH_VERTEX = "MESH_VERTEX";
		public const string MESH_FACE = "MESH_FACE";
		public const string MESH_TVERT = "MESH_TVERT";
		public const string MESH_TFACE = "MESH_TFACE";
		public const string MESH_FACENORMAL = "MESH_FACENORMAL";
		public const string MESH_VERTEXNORMAL = "MESH_VERTEXNORMAL";
		public const string MESH_VERTCOL = "MESH_VERTCOL";
		public const string MESH_CFACE = "MESH_CFACE";

		public const string MESH_SMOOTHING = "MESH_SMOOTHING";

		public const string MATERIAL_REF = "MATERIAL_REF";
		public const string NUMSUBMTLS = "NUMSUBMTLS";
		public const string SUBMATERIAL = "SUBMATERIAL";
		public const string MATERIAL_NAME = "MATERIAL_NAME";


		public const string NODE_TM = "NODE_TM";
		public const string TM_ROW0 = "TM_ROW0";
		public const string TM_ROW1 = "TM_ROW1";
		public const string TM_ROW2 = "TM_ROW2";
		public const string TM_ROW3 = "TM_ROW3";
		public const string TM_POS = "TM_POS";
		public const string TM_ROTAXIS = "TM_ROTAXIS";
		public const string TM_ROTANGLE = "TM_ROTANGLE";
		public const string TM_SCALE = "TM_SCALE";
		// ReSharper restore InconsistentNaming

		private static readonly Dictionary<string, AddPropertyHandler> AddPropertyHandlers = new Dictionary<string, AddPropertyHandler>
		                                                                                     	{
		                                                                                     		{GEOMOBJECT,GeometryScopeHandler},
																									{NODE_TM,GeometryScopeHandler},
		                                                                                     		{MESH, MeshScopeHandler},
		                                                                                     		{MESH_VERTEX_LIST, VertexScopeHandler},
		                                                                                     		{MESH_FACE_LIST, FaceScopeHandler},
		                                                                                     		{MESH_TVERTLIST, TextureCoordinateScopeHandler},
		                                                                                     		{MESH_TFACELIST, TextureCoordinateScopeHandler},
																									{MESH_CVERTLIST, ColorVertexScopeHandler},
																									{MESH_CFACELIST, ColorFaceScopeHandler},
		                                                                                     		{MATERIAL_LIST, MaterialListScopeHandler},
		                                                                                     		{MATERIAL, MaterialScopeHandler},
		                                                                                     		{MAP_DIFFUSE, DiffuseScopeHandler}
		                                                                                     	};



		public static Geometry[] Parse(string assetName, List<string> lines, IAssetLoaderService assetLoaderService, IContentManager contentManager)
		{
			var scopeStack = new Stack<string>();
			var scopeObjectStack = new Stack<object>();
			var currentScope = string.Empty;
			object currentScopeObject = null;

			var geometries = new List<Geometry>();

			var lineNumber = 0;

			var globals = new AseGlobals();
			globals.AssetLoaderService = assetLoaderService;
			globals.RootPath = Path.GetDirectoryName(assetName);
			scopeStack.Push("Globals");
			scopeObjectStack.Push(globals);

			foreach (var line in lines)
			{
				var trimmedLine = line.Trim();

				lineNumber++;

				if (trimmedLine.StartsWith("*"))
				{
					if (trimmedLine.Contains("{"))
					{
						var startIndex = trimmedLine.IndexOf("*") + 1;
						var endIndex = trimmedLine.IndexOf("{");
						var scope = trimmedLine.Substring(startIndex, endIndex - startIndex).Trim();


						var scopeParameter = string.Empty;
						var elements = scope.Split(' ');
						if (elements.Length == 2)
						{
							scope = elements[0];
							scopeParameter = elements[1];
						}

						scopeStack.Push(scope);
						currentScope = scope;
						currentScopeObject = GetScopeObject(globals, currentScope, scopeParameter, currentScopeObject, assetLoaderService, contentManager);
						scopeObjectStack.Push(currentScopeObject);

						if (null != currentScopeObject &&
							currentScopeObject is Geometry &&
							!geometries.Contains(currentScopeObject as Geometry))
						{
							geometries.Add(currentScopeObject as Geometry);
						}
					}
					else
					{

						if (AddPropertyHandlers.ContainsKey(currentScope))
						{
							var handler = AddPropertyHandlers[currentScope];
							if (null != handler && null != currentScopeObject)
							{
								var firstSpace = trimmedLine.IndexOf(' ');
								var firstTab = trimmedLine.IndexOf('\t');

								var contentIndex = 0;

								if ((firstTab < firstSpace || firstSpace < 0) &&
									firstTab > 0)
								{
									contentIndex = firstTab;
								}
								else
								{
									contentIndex = firstSpace;
								}

								if( contentIndex < 1 )
								{
									contentIndex = trimmedLine.Length-1;
								}


								var propertyName = trimmedLine.Substring(1, contentIndex).Trim();

								contentIndex++;
								var content = trimmedLine.Substring(contentIndex).Trim();
								handler(globals, currentScopeObject, propertyName, content);
							}
						}
					}
				}
				if (trimmedLine.Contains("}"))
				{
					scopeStack.Pop();
					scopeObjectStack.Pop();
					if (scopeStack.Count > 0)
					{
						currentScope = scopeStack.Peek();
						currentScopeObject = scopeObjectStack.Peek();
					}
					else
					{
						currentScope = string.Empty;
						currentScopeObject = null;
					}
				}
			}

			return geometries.ToArray();
		}


		private static object GetScopeObject(AseGlobals globals, string scope, string scopeParameter, object currentScopeObject, IAssetLoaderService assetLoaderService, IContentManager contentManager)
		{
			switch (scope)
			{
				case GEOMOBJECT:
					{
						return contentManager.CreateAssetPart<Geometry>();
					}

				case MATERIAL:
					{
						var materialIndex = Convert.ToInt32(scopeParameter);
						var material = contentManager.Creator.CreateMaterial();
						material.Shade = MaterialShade.Gouraud;
						material.Specular = Colors.White;
						globals.Materials[materialIndex] = material;
						return material;
					}
			}
			return currentScopeObject;
		}



		private static void GeometryScopeHandler(AseGlobals globals, object scopeObject, string propertyName, string content)
		{
			var geometry = scopeObject as Geometry;
			switch (propertyName)
			{
				case MATERIAL_REF:
					{
						var materialRef = Convert.ToInt32(content);
						if (null != globals.Materials && materialRef < globals.Materials.Length)
						{
							geometry.Material = globals.Materials[materialRef];
						}
					}
					break;
				case TM_ROW0:
					{
						var elements = content.Split('\t', ' ');
						geometry.World.M11 = float.Parse(elements[0], CultureInfo.InvariantCulture);
						geometry.World.M12 = float.Parse(elements[2], CultureInfo.InvariantCulture);
						geometry.World.M13 = float.Parse(elements[1], CultureInfo.InvariantCulture);
					}
					break;

				case TM_ROW2:
					{
						var elements = content.Split('\t', ' ');
						geometry.World.M21 = float.Parse(elements[0], CultureInfo.InvariantCulture);
						geometry.World.M22 = float.Parse(elements[2], CultureInfo.InvariantCulture);
						geometry.World.M23 = float.Parse(elements[1], CultureInfo.InvariantCulture);
					}
					break;

				case TM_ROW1:
					{
						var elements = content.Split('\t', ' ');
						geometry.World.M31 = float.Parse(elements[0], CultureInfo.InvariantCulture);
						geometry.World.M32 = float.Parse(elements[2], CultureInfo.InvariantCulture);
						geometry.World.M33 = float.Parse(elements[1], CultureInfo.InvariantCulture);
					}
					break;

				case TM_ROW3:
					{
						var elements = content.Split('\t', ' ');
						geometry.World.M41 = float.Parse(elements[0], CultureInfo.InvariantCulture);
						geometry.World.M42 = float.Parse(elements[2], CultureInfo.InvariantCulture);
						geometry.World.M43 = float.Parse(elements[1], CultureInfo.InvariantCulture);
					}
					break;

				
					
				case TM_POS:
					{
						// TODO : Introduce some kinda "out-of-scope" callback mechanism - this is kinda hacky
						globals.CurrentObjectsInvertedMatrix = Matrix.Invert(geometry.World);
					}
					break;

			}
		}

		private static void MeshScopeHandler(AseGlobals globals, object scopeObject, string propertyName, string content)
		{
			var geometry = scopeObject as Geometry;
			var geometryDetailLevel = geometry.GeometryContext.GetDetailLevel(DetailLevel.Full);
			switch (propertyName)
			{
				case MESH_NUMVERTEX:
					{
						var numVertices = Convert.ToInt32(content);
						geometryDetailLevel.AllocateVertices(numVertices);
					}
					break;
				case MESH_NUMFACES:
					{
						var numFaces = Convert.ToInt32(content);
						geometryDetailLevel.AllocateFaces(numFaces);
					}
					break;
				case MESH_NUMTVERTEX:
					{
						var numTVertices = Convert.ToInt32(content);
						geometryDetailLevel.AllocateTextureCoordinates(numTVertices);
					}
					break;
				case MESH_NUMCVERTEX:
					{
						var numCVertices = Convert.ToInt32(content);
						globals.CurrentObjectVertexColors = new Color[numCVertices];
					}
					break;
			}
		}

		private static void VertexScopeHandler(AseGlobals globals, object scopeObject, string propertyName, string content)
		{
			var geometry = scopeObject as Geometry;
			var geometryDetailLevel = geometry.GeometryContext.GetDetailLevel(DetailLevel.Full);
			switch (propertyName)
			{
				case MESH_VERTEX:
					{
						var elements = content.Split('\t', ' ');
						var vertexIndex = Convert.ToInt32(elements[0]);
						var x = float.Parse(elements[1], CultureInfo.InvariantCulture);
						var y = float.Parse(elements[3], CultureInfo.InvariantCulture);
						var z = float.Parse(elements[2], CultureInfo.InvariantCulture);

						var invertedVector = Vector.Transform(x, y, z, globals.CurrentObjectsInvertedMatrix);
						x = invertedVector.X;
						y = invertedVector.Y;
						z = invertedVector.Z;

						var vertex = new Vertex(x, y, z);
						geometryDetailLevel.SetVertex(vertexIndex, vertex);
					}
					break;

			}
		}

		private static void ColorVertexScopeHandler(AseGlobals globals, object scopeObject, string propertyName, string content)
		{
			switch (propertyName)
			{
				case MESH_VERTCOL:
					{
						var elements = content.Split('\t', ' ');
						var colorIndex = Convert.ToInt32(elements[0]);
						var r = float.Parse(elements[1], CultureInfo.InvariantCulture);
						var g = float.Parse(elements[3], CultureInfo.InvariantCulture);
						var b = float.Parse(elements[2], CultureInfo.InvariantCulture);

						var color = new Color((byte)(r*255f), (byte)(g*255f), (byte)(b*255f), 0xff);
						globals.CurrentObjectVertexColors[colorIndex] = color;
					}
					break;
			}
		}


		private static void ColorFaceScopeHandler(AseGlobals globals, object scopeObject, string propertyName, string content)
		{
			
		}
        

		private static void FaceScopeHandler(AseGlobals globals, object scopeObject, string propertyName, string content)
		{
			var geometry = scopeObject as Geometry;
			var geometryDetailLevel = geometry.GeometryContext.GetDetailLevel(DetailLevel.Full);
			switch (propertyName)
			{

				case MESH_FACE:
					{
						var smoothingGroup = 1;
						var meshSmoothingIndex = content.IndexOf(MESH_SMOOTHING);
						if (meshSmoothingIndex > 0)
						{
							var meshSmoothing = content.Substring(meshSmoothingIndex + MESH_SMOOTHING.Length + 1, 2).Trim();
                            meshSmoothing = meshSmoothing.Replace(",", string.Empty);
							if (string.IsNullOrEmpty(meshSmoothing) || meshSmoothing.Contains("*"))
							{
								smoothingGroup = 0;
							}
							else
							{
								smoothingGroup = Convert.ToInt32(meshSmoothing);
							}
						}
						content = content.Replace(" ", string.Empty);
                        var elements = content.Split(':');

						var faceIndex = Convert.ToInt32(elements[0]);
						var a = Convert.ToInt32(elements[2].Substring(0, elements[2].Length - 1));
						var b = Convert.ToInt32(elements[3].Substring(0, elements[3].Length - 1));
						var c = Convert.ToInt32(elements[4].Substring(0, elements[4].Length - 2));
						var face = new Face(a, b, c);
						face.SmoothingGroup = smoothingGroup;
						geometryDetailLevel.SetFace(faceIndex, face);
					}
					break;
			}
		}

		private static void TextureCoordinateScopeHandler(AseGlobals globals, object scopeObject, string propertyName, string content)
		{
			var geometry = scopeObject as Geometry;
			var geometryDetailLevel = geometry.GeometryContext.GetDetailLevel(DetailLevel.Full);
			switch (propertyName)
			{
				case MESH_TVERT:
					{
						var elements = content.Split('\t', ' ');

						var tvertIndex = Convert.ToInt32(elements[0]);
						var u = float.Parse(elements[1], CultureInfo.InvariantCulture);
						var v = float.Parse(elements[2], CultureInfo.InvariantCulture);
						v = 1f - v;
						var textureCoordinate = new TextureCoordinate(u, v);
						geometryDetailLevel.SetTextureCoordinate(tvertIndex, textureCoordinate);
					}
					break;

				case MESH_TFACE:
					{
						var elements = content.Split('\t', ' ');
						var faceIndex = Convert.ToInt32(elements[0]);
						var a = Convert.ToInt32(elements[1]);
						var b = Convert.ToInt32(elements[2]);
						var c = Convert.ToInt32(elements[3]);

						geometryDetailLevel.SetFaceTextureCoordinateIndex(faceIndex, a, b, c);
					}
					break;
			}
		}

		private static void MaterialListScopeHandler(AseGlobals globals, object scopeObject, string propertyName, string content)
		{
			switch (propertyName)
			{
				case MATERIAL_COUNT:
					{
						var materialCount = Convert.ToInt32(content);
						globals.Materials = new Material[materialCount];
					}
					break;
			}
		}

		private static void MaterialScopeHandler(AseGlobals globals, object scopeObject, string propertyName, string content)
		{

		}

		private static void DiffuseScopeHandler(AseGlobals globals, object scopeObject, string propertyName, string content)
		{
			var material = scopeObject as Material;

			switch (propertyName)
			{
				case BITMAP:
					{
						var file = content.Replace("\"", string.Empty);
						var relativeFile = Path.GetFileName(file);
						var rootPath = globals.RootPath;
						var actualFile = string.IsNullOrEmpty(rootPath)
											? relativeFile
											: string.Format("{0}//{1}", rootPath, relativeFile);

						var loader = globals.AssetLoaderService.GetLoader<Image>(actualFile);
						var frames = loader.Load(actualFile);
						var imageMap = new ImageMap(frames[0] as Image);
						material.DiffuseMap = imageMap;
					}
					break;
			}
		}
	}
}