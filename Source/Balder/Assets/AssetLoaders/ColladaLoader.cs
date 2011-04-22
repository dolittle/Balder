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
#if(!SILVERLIGHT)
using System.IO;
#endif
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Balder.Content;
using Balder.Exceptions;
using Balder.Materials;
using Balder.Objects.Geometries;
using Balder.Rendering;

namespace Balder.Assets.AssetLoaders
{
	public class ColladaLoader : AssetLoader
	{
		readonly IAssetLoaderService _assetLoaderService;

		public override string[] FileExtensions { get { return new[] { "dae" }; } }
		public override Type SupportedAssetType { get { return typeof(Mesh); } }

		public ColladaLoader(IAssetLoaderService assetLoaderService, IFileLoaderManager fileLoaderManager, IContentManager contentManager)
			: base(fileLoaderManager, contentManager)
		{
			_assetLoaderService = assetLoaderService;
		}

		public override IAssetPart[] Load(string assetName)
		{
			var fileLoader = FileLoaderManager.GetFileLoader(assetName);
			var stream = fileLoader.GetStream(assetName);
			if (null == stream)
			{
				throw new AssetNotFoundException(assetName);
			}

#if(SILVERLIGHT)
			var xmlDocument = XDocument.Load(stream);
#else
			var xmlDocument = XDocument.Load(new StreamReader(stream));
#endif
			var materials = LoadMaterials(xmlDocument);
			var geometries = LoadGeometries(xmlDocument, materials);

			return geometries;
		}


		Material[] LoadMaterials(XDocument xmlDocument)
		{
			return null;
		}

		Geometry[] LoadGeometries(XDocument xmlDocument, Material[] materials)
		{
			var geometriesToReturn = new List<Geometry>();

			var ns = xmlDocument.Root.GetDefaultNamespace();

			var library_geometries = xmlDocument.Root.Element(ns + "library_geometries");
			if (library_geometries != null)
			{
				var geometries = library_geometries.Elements(ns + "geometry");
				foreach (var geometry in geometries)
				{
					var mesh = geometry.Element(ns + "mesh");
					if (mesh != null)
					{
						var geometryToAdd = ContentManager.CreateAssetPart<Geometry>();
						geometriesToReturn.Add(geometryToAdd);
						var geometryDetailLevel = geometryToAdd.GeometryContext.GetDetailLevel(DetailLevel.Full);

						var sources = mesh.Elements(ns + "source");
						foreach (var source in sources)
						{
							if (source.IsVertexSource())
							{
								var vertices = source.GetVertices(ns);
								geometryDetailLevel.AllocateVertices(vertices.Length);
								for (var vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++)
									geometryDetailLevel.SetVertex(vertexIndex, vertices[vertexIndex]);
							}
							if (source.IsNormalSource())
							{
								var normals = source.GetNormals(ns);
								geometryDetailLevel.AllocateNormals(normals.Length);
								for (var normalIndex = 0; normalIndex < normals.Length; normalIndex++)
									geometryDetailLevel.SetNormal(normalIndex, normals[normalIndex]);
							}
						}
						var polylist = mesh.Element(ns + "polylist");
						if (polylist != null)
						{
							var p = polylist.Element(ns + "p");
							if (p != null)
							{
								var faces = new List<Face>();
								var indices = p.Value.Split(' ');
								for (var index = 0; index < indices.Length; index += 8)
								{
									var face = new Face(
													int.Parse(indices[index + 4], CultureInfo.InvariantCulture),
													int.Parse(indices[index + 2], CultureInfo.InvariantCulture),
													int.Parse(indices[index + 0], CultureInfo.InvariantCulture))
												{
													NormalA = int.Parse(indices[index + 5], CultureInfo.InvariantCulture),
													NormalB = int.Parse(indices[index + 3], CultureInfo.InvariantCulture),
													NormalC = int.Parse(indices[index + 1], CultureInfo.InvariantCulture)
												};

									faces.Add(face);

									face = new Face(
									       		int.Parse(indices[index + 6], CultureInfo.InvariantCulture),
									       		int.Parse(indices[index + 4], CultureInfo.InvariantCulture),
									       		int.Parse(indices[index + 0], CultureInfo.InvariantCulture))
											{
									       		NormalA = int.Parse(indices[index + 7], CultureInfo.InvariantCulture),
									       		NormalB = int.Parse(indices[index + 5], CultureInfo.InvariantCulture),
									       		NormalC = int.Parse(indices[index + 1], CultureInfo.InvariantCulture)
									       	};

									faces.Add(face);
								}

								geometryDetailLevel.AllocateFaces(faces.Count);
								for (var faceIndex = 0; faceIndex < faces.Count; faceIndex++ )
								{
									geometryDetailLevel.SetFace(faceIndex, faces[faceIndex]);
								}
							}

						}
					}
				}
			}

			return geometriesToReturn.ToArray();
		}



	}


	public static class ColladaExtensions
	{



		public static bool IsNormalSource(this XElement source)
		{
			if (source.Name.LocalName.Equals("source") && source.Attribute("name") != null)
				return source.Attribute("name").Value == "normal";
			return false;
		}

		public static bool IsVertexSource(this XElement source)
		{
			if (source.Name.LocalName.Equals("source") && source.Attribute("name") != null)
				return source.Attribute("name").Value == "position";
			return false;
		}



		public static Vertex[] GetVertices(this XElement source, XNamespace ns)
		{
			var vertices = new List<Vertex>();
			var float_array = source.Element(ns + "float_array");
			if (float_array != null)
			{
				var technique_common = source.Element(ns + "technique_common");
				if (technique_common != null)
				{
					var accessor = technique_common.Element(ns + "accessor");
					if (accessor != null)
					{
						var stride = Int32.Parse(accessor.Attribute("stride").Value, CultureInfo.InvariantCulture);
						var values = float_array.Value.Split(' ');
						for (var valueIndex = 0; valueIndex < values.Length; valueIndex += stride)
						{
							var vertex = new Vertex
											{
												X = float.Parse(values[valueIndex], CultureInfo.InvariantCulture),
												Y = float.Parse(values[valueIndex + 1], CultureInfo.InvariantCulture),
												Z = float.Parse(values[valueIndex + 2], CultureInfo.InvariantCulture),
											};
							vertices.Add(vertex);
						}
					}
				}

			}

			return vertices.ToArray();
		}

		public static Normal[] GetNormals(this XElement source, XNamespace ns)
		{
			var normals = new List<Normal>();
			var float_array = source.Element(ns + "float_array");
			if (float_array != null)
			{
				var technique_common = source.Element(ns + "technique_common");
				if (technique_common != null)
				{
					var accessor = technique_common.Element(ns + "accessor");
					if (accessor != null)
					{
						var stride = Int32.Parse(accessor.Attribute("stride").Value, CultureInfo.InvariantCulture);
						var values = float_array.Value.Split(' ');
						for (var valueIndex = 0; valueIndex < values.Length; valueIndex += stride)
						{
							var vertex = new Normal
							{
								X = float.Parse(values[valueIndex], CultureInfo.InvariantCulture),
								Y = float.Parse(values[valueIndex + 1], CultureInfo.InvariantCulture),
								Z = float.Parse(values[valueIndex + 2], CultureInfo.InvariantCulture),
							};
							normals.Add(vertex);
						}
					}
				}

			}

			return normals.ToArray();
		}

	}
}
