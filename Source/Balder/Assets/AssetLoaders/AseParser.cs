#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2011, DoLittle Studios
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
	public delegate void BeginScopeHandler(AseGlobals globals, object scopeObject, string scopeName, string scopeParameter);
	public delegate void EndScopeHandler(AseGlobals globals, object scopeObject, string scopeName);
	public delegate void AddPropertyHandler(AseGlobals globals, object scopeObject, string propertyName, string content);


	public class AseParser
	{
		private static readonly Dictionary<string, BeginScopeHandler> BeginScopeHandlers = new Dictionary<string, BeginScopeHandler>
		                                                                                   	{
																								{AseConstants.GEOMOBJECT, BeginGeometryScopeHandler},
																								{AseConstants.MATERIAL, BeginMaterialScopeHandler},
																								{AseConstants.SUBMATERIAL, BeginSubMaterialScopeHandler}
		                                                                                   	};

		private static readonly Dictionary<string, EndScopeHandler> EndScopeHandlers = new Dictionary<string, EndScopeHandler>
		                                                                               	{
																							{AseConstants.GEOMOBJECT, GeometryEndScopeHandler}
		                                                                               	};


		private static readonly Dictionary<string, AddPropertyHandler> AddPropertyHandlers = new Dictionary<string, AddPropertyHandler>
		                                                                                     	{
		                                                                                     		{AseConstants.GEOMOBJECT,GeometryScopeHandler},
																									{AseConstants.NODE_TM,GeometryScopeHandler},
		                                                                                     		{AseConstants.MESH, MeshScopeHandler},
		                                                                                     		{AseConstants.MESH_VERTEX_LIST, VertexScopeHandler},
		                                                                                     		{AseConstants.MESH_FACE_LIST, FaceScopeHandler},
		                                                                                     		{AseConstants.MESH_TVERTLIST, TextureCoordinateScopeHandler},
		                                                                                     		{AseConstants.MESH_TFACELIST, TextureCoordinateScopeHandler},
																									{AseConstants.MESH_CVERTLIST, ColorVertexScopeHandler},
																									{AseConstants.MESH_CFACELIST, ColorFaceScopeHandler},
		                                                                                     		{AseConstants.MATERIAL_LIST, MaterialListScopeHandler},
		                                                                                     		{AseConstants.MATERIAL, MaterialScopeHandler},
																									{AseConstants.SUBMATERIAL, MaterialScopeHandler},
																									{AseConstants.MATERIAL_NAME, MaterialScopeHandler},
		                                                                                     		{AseConstants.MAP_DIFFUSE, DiffuseScopeHandler}
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
					if (IsScopeStart(trimmedLine))
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

						HandleBeginScope(globals, scope, scopeParameter, currentScopeObject);

					}
					else
					{
						HandleScopeProperty(globals, trimmedLine, currentScope, currentScopeObject);
					}
				}
				if (IsScopeEnd(trimmedLine))
				{
					if (null != currentScopeObject &&
						currentScopeObject is Geometry &&
						!geometries.Contains(currentScopeObject as Geometry) &&
						null != globals.Faces)
					{
						geometries.Add(currentScopeObject as Geometry);
					}

					HandleEndScope(globals, currentScope, currentScopeObject);
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

		private static void HandleBeginScope(AseGlobals globals, string scope, string scopeParameter, object scopeObject)
		{
			if (BeginScopeHandlers.ContainsKey(scope))
			{
				BeginScopeHandlers[scope](globals, scopeObject, scope, scopeParameter);
			}

		}

		private static void HandleEndScope(AseGlobals globals, string scope, object scopeObject)
		{
			if (EndScopeHandlers.ContainsKey(scope))
			{
				EndScopeHandlers[scope](globals, scopeObject, scope);
			}
		}


		private static void HandleScopeProperty(AseGlobals globals, string trimmedLine, string currentScope, object currentScopeObject)
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

					if (contentIndex < 1)
					{
						contentIndex = trimmedLine.Length - 1;
					}


					var propertyName = trimmedLine.Substring(1, contentIndex).Trim();

					contentIndex++;
					var content = trimmedLine.Substring(contentIndex).Trim();
					handler(globals, currentScopeObject, propertyName, content);
				}
			}
		}


		private static object GetScopeObject(AseGlobals globals, string scope, string scopeParameter, object currentScopeObject, IAssetLoaderService assetLoaderService, IContentManager contentManager)
		{
			switch (scope)
			{
				case AseConstants.GEOMOBJECT:
					{
						return contentManager.CreateAssetPart<Geometry>();
					}

				case AseConstants.MATERIAL:
				case AseConstants.SUBMATERIAL:
					{
						var materialIndex = Convert.ToInt32(scopeParameter);
						var material = contentManager.Creator.CreateMaterial();
						material.Shade = MaterialShade.Gouraud;
						material.Specular = Colors.White;

						if (scope == AseConstants.SUBMATERIAL && null != globals.CurrentParentMaterial)
						{
							var materialId = Convert.ToInt32(scopeParameter);
							globals.CurrentParentMaterial.SubMaterials[materialId] = material;
						}
						else
						{
							globals.Materials[materialIndex] = material;
							globals.CurrentParentMaterial = material;
						}
						globals.CurrentMaterial = material;

						return material;
					}
			}
			return currentScopeObject;
		}


		private static void BeginGeometryScopeHandler(AseGlobals globals, object scopeobject, string scopename, string content)
		{
			
		}

		private static void GeometryScopeHandler(AseGlobals globals, object scopeObject, string propertyName, string content)
		{
			var geometry = scopeObject as Geometry;
			switch (propertyName)
			{
				case AseConstants.NODE_NAME:
					{
						geometry.Label = content;
					}
					break;

				case AseConstants.MATERIAL_REF:
					{
						var materialRef = Convert.ToInt32(content);
						if (globals.Materials.Length > materialRef)
						{
							geometry.Material = globals.Materials[materialRef];
						}
					}
					break;
				case AseConstants.TM_ROW0:
					{
						var elements = GetValuesFromString(content);
						geometry.World.M11 = float.Parse(elements[0], CultureInfo.InvariantCulture);
						geometry.World.M12 = float.Parse(elements[2], CultureInfo.InvariantCulture);
						geometry.World.M13 = float.Parse(elements[1], CultureInfo.InvariantCulture);
					}
					break;

				case AseConstants.TM_ROW2:
					{
						var elements = GetValuesFromString(content);
						geometry.World.M21 = float.Parse(elements[0], CultureInfo.InvariantCulture);
						geometry.World.M22 = float.Parse(elements[2], CultureInfo.InvariantCulture);
						geometry.World.M23 = float.Parse(elements[1], CultureInfo.InvariantCulture);
					}
					break;

				case AseConstants.TM_ROW1:
					{
						var elements = GetValuesFromString(content);
						geometry.World.M31 = float.Parse(elements[0], CultureInfo.InvariantCulture);
						geometry.World.M32 = float.Parse(elements[2], CultureInfo.InvariantCulture);
						geometry.World.M33 = float.Parse(elements[1], CultureInfo.InvariantCulture);
					}
					break;

				case AseConstants.TM_ROW3:
					{
						var elements = GetValuesFromString(content);
						geometry.World.M41 = float.Parse(elements[0], CultureInfo.InvariantCulture);
						geometry.World.M42 = float.Parse(elements[2], CultureInfo.InvariantCulture);
						geometry.World.M43 = float.Parse(elements[1], CultureInfo.InvariantCulture);
					}
					break;

				case AseConstants.TM_POS:
					{
						// TODO : Introduce some kinda "out-of-scope" callback mechanism - this is kinda hacky
						globals.CurrentObjectsInvertedMatrix = Matrix.Invert(geometry.World);
					}
					break;

			}
		}

		private static void GeometryEndScopeHandler(AseGlobals globals, object scopeObject, string scopeName)
		{
			var geometry = scopeObject as Geometry;

			if( null == globals.Faces )
			{
				return;
			}
			var geometryDetailLevel = geometry.GeometryContext.GetDetailLevel(DetailLevel.Full);
			geometryDetailLevel.AllocateFaces(globals.Faces.Length);

			for (var faceIndex = 0; faceIndex < globals.Faces.Length; faceIndex++)
			{
				var face = globals.Faces[faceIndex];
				geometryDetailLevel.SetFace(faceIndex, face);
			}
		}


		private static void MeshScopeHandler(AseGlobals globals, object scopeObject, string propertyName, string content)
		{
			var geometry = scopeObject as Geometry;
			var geometryDetailLevel = geometry.GeometryContext.GetDetailLevel(DetailLevel.Full);
			switch (propertyName)
			{
				case AseConstants.MESH_NUMVERTEX:
					{
						var numVertices = Convert.ToInt32(content);
						geometryDetailLevel.AllocateVertices(numVertices);
					}
					break;
				case AseConstants.MESH_NUMFACES:
					{
						var numFaces = Convert.ToInt32(content);
						globals.Faces = new AseFace[numFaces];
					}
					break;
				case AseConstants.MESH_NUMTVERTEX:
					{
						var numTVertices = Convert.ToInt32(content);
						geometryDetailLevel.AllocateTextureCoordinates(numTVertices);
					}
					break;
				case AseConstants.MESH_NUMCVERTEX:
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
				case AseConstants.MESH_VERTEX:
					{
						var elements = GetValuesFromString(content);
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
				case AseConstants.MESH_VERTCOL:
					{
						var elements = GetValuesFromString(content);
						var colorIndex = Convert.ToInt32(elements[0]);
						var r = float.Parse(elements[1], CultureInfo.InvariantCulture);
						var g = float.Parse(elements[3], CultureInfo.InvariantCulture);
						var b = float.Parse(elements[2], CultureInfo.InvariantCulture);

						var color = new Color((byte)(r * 255f), (byte)(g * 255f), (byte)(b * 255f), 0xff);
						globals.CurrentObjectVertexColors[colorIndex] = color;
					}
					break;
			}
		}


		private static void ColorFaceScopeHandler(AseGlobals globals, object scopeObject, string propertyName, string content)
		{
			switch (propertyName)
			{
				case AseConstants.MESH_CFACE:
					{
						var elements = GetValuesFromString(content);
						var faceIndex = Convert.ToInt32(elements[0]);
						var a = Convert.ToInt32(elements[1]);
						var b = Convert.ToInt32(elements[2]);
						var c = Convert.ToInt32(elements[3]);

						globals.Faces[faceIndex].ColorA = globals.CurrentObjectVertexColors[a];
						globals.Faces[faceIndex].ColorB = globals.CurrentObjectVertexColors[b];
						globals.Faces[faceIndex].ColorC = globals.CurrentObjectVertexColors[c];
					}
					break;

			}

		}


		private static void FaceScopeHandler(AseGlobals globals, object scopeObject, string propertyName, string content)
		{
			switch (propertyName)
			{
				case AseConstants.MESH_FACE:
					{
						var smoothingGroup = GetFaceParameter(content, AseConstants.MESH_SMOOTHING);
						var materialId = GetFaceParameter(content, AseConstants.MESH_MTLID);
						content = content.Replace(" ", string.Empty);
						var elements = content.Split(':');

						var faceIndex = Convert.ToInt32(elements[0]);
						var a = Convert.ToInt32(elements[2].Substring(0, elements[2].Length - 1));
						var b = Convert.ToInt32(elements[3].Substring(0, elements[3].Length - 1));
						var c = Convert.ToInt32(elements[4].Substring(0, elements[4].Length - 2));
						var face = new AseFace(a, b, c);
						face.SmoothingGroup = smoothingGroup;
						face.MaterialId = materialId;
						globals.Faces[faceIndex] = face;
					}
					break;
			}
		}


		private static int GetFaceParameter(string content, string parameterName)
		{
			var parameter = 0;
			var parameterIndex = content.IndexOf(parameterName);
			if (parameterIndex > 0)
			{
				var length = 2;
				var startIndex = parameterIndex + parameterName.Length + 1;
				if (content.Length - startIndex < 2)
				{
					length = 1;
				}
				var parameterContent = content.Substring(startIndex, length).Trim();
				parameterContent = parameterContent.Replace(",", string.Empty);
				if (string.IsNullOrEmpty(parameterContent) || parameterContent.Contains("*"))
				{
					parameter = 0;
				}
				else
				{
					parameter = Convert.ToInt32(parameterContent);
				}
			}
			return parameter;
		}

		private static void TextureCoordinateScopeHandler(AseGlobals globals, object scopeObject, string propertyName, string content)
		{
			var geometry = scopeObject as Geometry;
			var geometryDetailLevel = geometry.GeometryContext.GetDetailLevel(DetailLevel.Full);
			switch (propertyName)
			{
				case AseConstants.MESH_TVERT:
					{
						var elements = GetValuesFromString(content);

						var tvertIndex = Convert.ToInt32(elements[0]);
						var u = float.Parse(elements[1], CultureInfo.InvariantCulture);
						var v = float.Parse(elements[2], CultureInfo.InvariantCulture);
						v = 1f - v;
						var textureCoordinate = new TextureCoordinate(u, v);
						geometryDetailLevel.SetTextureCoordinate(tvertIndex, textureCoordinate);
					}
					break;

				case AseConstants.MESH_TFACE:
					{
						var elements = GetValuesFromString(content);
						var faceIndex = Convert.ToInt32(elements[0]);
						var a = Convert.ToInt32(elements[1]);
						var b = Convert.ToInt32(elements[2]);
						var c = Convert.ToInt32(elements[3]);
						globals.Faces[faceIndex].DiffuseA = a;
						globals.Faces[faceIndex].DiffuseB = b;
						globals.Faces[faceIndex].DiffuseC = c;
					}
					break;
			}
		}

		private static void MaterialListScopeHandler(AseGlobals globals, object scopeObject, string propertyName, string content)
		{
			switch( propertyName)
			{
				case AseConstants.MATERIAL_COUNT:
					{
						int count;
						if( int.TryParse(content,out count) )
						{
							globals.Materials = new Material[count];	
						}
					}
					break;
			}

		}

		private static void BeginMaterialScopeHandler(AseGlobals globals, object scopeobject, string scopename, string content)
		{
		}

		private static void BeginSubMaterialScopeHandler(AseGlobals globals, object scopeobject, string scopename, string content)
		{
		}


		private static void MaterialScopeHandler(AseGlobals globals, object scopeObject, string propertyName, string content)
		{
			switch (propertyName)
			{
				case AseConstants.MATERIAL_NAME:
					{
						globals.CurrentMaterial.Label = content.Trim();
					}
					break;
			}
		}


		private static void DiffuseScopeHandler(AseGlobals globals, object scopeObject, string propertyName, string content)
		{
			var material = scopeObject as Material;

			switch (propertyName)
			{
				case AseConstants.BITMAP:
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

		private static string[] GetValuesFromString(string content)
		{
			var elements = content.Split('\t', ' ');
			var count = 0;
			for( var elementIndex=0; elementIndex<elements.Length; elementIndex++ )
			{
				if( !string.IsNullOrEmpty(elements[elementIndex]) )
				{
					count++;
				}
			}

			var actualElements = new string[count];
			count = 0;
			for (var elementIndex = 0; elementIndex < elements.Length; elementIndex++)
			{
				if (!string.IsNullOrEmpty(elements[elementIndex]))
				{
					actualElements[count++] = elements[elementIndex];
				}
			}
			return actualElements;
		}

		private static bool IsScopeStart(string line)
		{
			return line.EndsWith("{");
		}


		private static bool IsScopeEnd(string line)
		{
			return line.EndsWith("}");
		}

	}
}