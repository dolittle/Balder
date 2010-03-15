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
using Balder.Core.Content;
using Balder.Core.Exceptions;
using Balder.Core.Imaging;
using Balder.Core.Materials;
using Balder.Core.Objects.Geometries;
using Balder.Core.ReadableRex;
using Balder.Core.ReadableRex.LinqToRegex;


namespace Balder.Core.Assets.AssetLoaders
{

	public delegate void AddPropertyHandler(object scopeObject, string propertyName, string content);

	public class AseParser
	{
		public const string GEOMOBJECT = "GEOMOBJECT";
		public const string NODE_NAME = "NODE_NAME";
		public const string MESH = "MESH";
		public const string MESH_NUMVERTEX = "MESH_NUMVERTEX";
		public const string MESH_NUMFACES = "MESH_NUMFACES";
		public const string MESH_VERTEX = "MESH_VERTEX";
		public const string MESH_FACE = "MESH_FACE";
		public const string MESH_VERTEX_LIST = "MESH_VERTEX_LIST";
		public const string MESH_FACE_LIST = "MESH_FACE_LIST";


		private static readonly Dictionary<string, AddPropertyHandler> AddPropertyHandlers = new Dictionary<string, AddPropertyHandler>
		                                                                     	{
																					{GEOMOBJECT,GeometryScopeHandler},
																					{MESH, MeshScopeHandler},
																					{MESH_VERTEX_LIST, VertexScopeHandler},
																					{MESH_FACE_LIST, FaceScopeHandler}
		                                                                     	};



		public static Geometry[]	Parse(List<string> lines, IAssetLoaderService assetLoaderService, IContentManager contentManager)
		{
			var scopeStack = new Stack<string>();
			var scopeObjectStack = new Stack<object>();
			var currentScope = string.Empty;
			object currentScopeObject = null;

			var geometries = new List<Geometry>();
			
			foreach( var line in lines )
			{
				var trimmedLine = line.Trim();

				if( trimmedLine.StartsWith("*") )
				{
					if (trimmedLine.Contains("{"))
					{
						var startIndex = trimmedLine.IndexOf("*") + 1;
						var endIndex = trimmedLine.IndexOf("{");
						var scope = trimmedLine.Substring(startIndex, endIndex - startIndex).Trim();
						scopeStack.Push(scope);
						currentScope = scope;
						currentScopeObject = GetScopeObject(currentScope, currentScopeObject, assetLoaderService, contentManager);
						scopeObjectStack.Push(currentScopeObject);
						
						if( null != currentScopeObject && 
							currentScopeObject is Geometry &&
							!geometries.Contains(currentScopeObject as Geometry))
						{
							geometries.Add(currentScopeObject as Geometry);
						}
					} else
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
								var propertyName = trimmedLine.Substring(1, contentIndex).Trim();

								contentIndex++;
								var content = trimmedLine.Substring(contentIndex).Trim();
								handler(currentScopeObject, propertyName, content);
							}
						}
					}
				}
				if( trimmedLine.Contains("}"))
				{
					if (scopeStack.Count > 0)
					{
						currentScope = scopeStack.Pop();
						currentScopeObject = scopeObjectStack.Pop();
					} else
					{
						currentScope = string.Empty;
						currentScopeObject = null;
					}
				}
			}

			return geometries.ToArray();
		}


		private static object GetScopeObject(string scope, object currentScopeObject, IAssetLoaderService assetLoaderService, IContentManager contentManager)
		{
			switch (scope)
			{
				case GEOMOBJECT:
					{
						return contentManager.CreateAssetPart<Geometry>();
					}
					break;
			}
			return currentScopeObject;
		}
	


		private static void GeometryScopeHandler(object scopeObject, string propertyName, string content)
		{
			int i = 0;
			i++;
		}

		private static void MeshScopeHandler(object scopeObject, string propertyName, string content)
		{
			var geometry = scopeObject as Geometry;
			switch( propertyName )
			{
				case MESH_NUMVERTEX:
					{
						var numVertices = Convert.ToInt32(content);
						geometry.GeometryContext.AllocateVertices(numVertices);
					}
					break;
				case MESH_NUMFACES:
					{
						var numFaces = Convert.ToInt32(content);
						geometry.GeometryContext.AllocateFaces(numFaces);
					}
					break;
			}
		}

		private static void VertexScopeHandler(object scopeObject, string propertyName, string content)
		{
			var geometry = scopeObject as Geometry;
			switch( propertyName )
			{
				case MESH_VERTEX:
					{
						//*MESH_VERTEX    0	-10.0000	-10.0000	-10.0000
						var elements = content.Split('\t');
						var vertexIndex = Convert.ToInt32(elements[0]);
						var x = float.Parse(elements[1], CultureInfo.InvariantCulture);
						var y = float.Parse(elements[3], CultureInfo.InvariantCulture);
						var z = float.Parse(elements[2], CultureInfo.InvariantCulture);
						var vertex = new Vertex(x, y, z);
						geometry.GeometryContext.SetVertex(vertexIndex,vertex);
					}
					break;

			}
			
		}

		private static void FaceScopeHandler(object scopeObject, string propertyName, string content)
		{
			var geometry = scopeObject as Geometry;
			switch (propertyName)
			{

				case MESH_FACE:
					{
						content = content.Replace(" ", string.Empty);
						var elements = content.Split(':');

						var faceIndex = Convert.ToInt32(elements[0]);
						var a = Convert.ToInt32(elements[2].Substring(0, elements[2].Length - 1));
						var b = Convert.ToInt32(elements[2].Substring(0, elements[3].Length - 1));
						var c = Convert.ToInt32(elements[2].Substring(0, elements[4].Length - 1));
						var face = new Face(a, b, c);
						geometry.GeometryContext.SetFace(faceIndex,face);


						//									*MESH_FACE    0:    A:    0 B:    2 C:    3 AB:    1 BC:    1 CA:    0	 *MESH_SMOOTHING 2 	*MESH_MTLID 1

					}
					break;
			}
		}
	}

	public class AseLoader : AssetLoader<Geometry>
	{
		private readonly IAssetLoaderService _assetLoaderService;


		public AseLoader(IAssetLoaderService assetLoaderService, IFileLoader fileLoader, IContentManager contentManager)
			: base(fileLoader, contentManager)
		{
			_assetLoaderService = assetLoaderService;
		}


		public override Geometry[] Load(string assetName)
		{
			var stream = FileLoader.GetStream(assetName);
			if( null == stream )
			{
				throw new AssetNotFoundException(assetName);
			}
			var streamReader = new StreamReader(stream);

			var lines = new List<string>();
			while( !streamReader.EndOfStream )
			{
				var line = streamReader.ReadLine();
				lines.Add(line);
			}

			var format = CultureInfo.InvariantCulture.NumberFormat;


			var geometries = AseParser.Parse(lines,_assetLoaderService,ContentManager);
			return geometries;
			
		}

		private Material[] LoadMaterials(string aseAssetName, IGeometryContext context, IFormatProvider format, string data)
		{
			var materials = new List<Material>();

			var rootPath = Path.GetDirectoryName(aseAssetName);

			var query = from match in RegexQuery.Against(data)
			            where	match
			            	.RegEx("\t")
			            	.Literal("*MATERIAL")
			            	.WhiteSpace
			            	.Group(Pattern.With.Digit.Repeat.ZeroOrMore)
			            	.WhiteSpace
			            	.Literal("{")
			            	.RegEx("[\n\r]*")
			            	.IsTrue()
			            select match;
			
			foreach( var match in query )
			{
				var materialEnd = data.IndexOf("\n\t}", match.Index);
				if( materialEnd > 0 )
				{
					materialEnd += 2;
					var material = new Material();
					materials.Add(material);
					var materialString = data.Substring(match.Index, materialEnd - match.Index);
					var diffuseIndex = materialString.IndexOf("\t\t*MAP_DIFFUSE");
					if( diffuseIndex > 0 )
					{
						var diffuseEnd = materialString.IndexOf("\n\t\t}", diffuseIndex);
						if( diffuseEnd > 0 )
						{
							var diffuseString = materialString.Substring(diffuseIndex, diffuseEnd - diffuseIndex);
							var bitmapIndex = diffuseString.IndexOf("\t\t\t*BITMAP");
							var eolIndex = diffuseString.IndexOf('\n', bitmapIndex);
							if( eolIndex > 0 )
							{
								var bitmapString = diffuseString.Substring(bitmapIndex, eolIndex - bitmapIndex);
								var fileIndex = bitmapString.IndexOf('"');
								if( fileIndex > 0 )
								{
									var file = bitmapString.Substring(fileIndex + 1).Replace('"', ' ').Trim();
									var relativeFile = Path.GetFileName(file);
									var actualFile = string.IsNullOrEmpty(rootPath)
									                 	? relativeFile
									                 	: string.Format("{0}//{1}", rootPath, relativeFile);
									var loader = _assetLoaderService.GetLoader<Image>(actualFile);
									var frames = loader.Load(actualFile);
									material.DiffuseMap = frames[0];
								}
							}
						}
					}
				}
			}
			return materials.ToArray();
		}


		public override string[] FileExtensions
		{
			get { return new[] { "ase" }; }
		}
	}
}