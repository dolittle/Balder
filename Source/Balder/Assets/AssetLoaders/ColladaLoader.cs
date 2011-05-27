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
							var faces = polylist.GetFaces(ns);
							geometryDetailLevel.AllocateFaces(faces.Length);
							for (var faceIndex = 0; faceIndex < faces.Length; faceIndex++ )
							{
								geometryDetailLevel.SetFace(faceIndex, faces[faceIndex]);
							}
						}

						GeometryHelper.CalculateNormals(geometryDetailLevel);
					}

					
				}
			}

			return geometriesToReturn.ToArray();
		}



	}
}
