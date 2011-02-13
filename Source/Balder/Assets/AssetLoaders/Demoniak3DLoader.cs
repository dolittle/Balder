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
#if(!SILVERLIGHT)
using System.IO;
#endif
using System.Globalization;
using System.Xml.Linq;
using Balder.Content;
using Balder.Exceptions;
using Balder.Imaging;
using Balder.Materials;
using Balder.Objects.Geometries;
using Balder.Rendering;

namespace Balder.Assets.AssetLoaders
{
	/// <summary>
	/// Notes:
	/// 3dsmax plugin can be found at: http://www.ozone3d.net/wak/
	/// </summary>
	public class Demoniak3DLoader : AssetLoader
	{
		private readonly IAssetLoaderService _assetLoaderService;

		public override string[] FileExtensions { get { return new[] {"xml"}; } }
		public override Type SupportedAssetType { get { return typeof (Mesh); } }

		public Demoniak3DLoader(IAssetLoaderService assetLoaderService, IFileLoaderManager fileLoaderManager, IContentManager contentManager)
			: base(fileLoaderManager,contentManager)
		{
			_assetLoaderService = assetLoaderService;
			
		}

		public override IAssetPart[] Load(string assetName)
		{
			var fileLoader = FileLoaderManager.GetFileLoader(assetName);
			var stream = fileLoader.GetStream(assetName);
			if( null == stream )
			{
				throw new AssetNotFoundException(assetName);
			}
#if(XAML)
			var xmlDocument = XDocument.Load(stream);
#else
			var xmlDocument = XDocument.Load(new StreamReader(stream));
#endif
			var materials = LoadMaterials(xmlDocument);
			var geometries = LoadGeometries(xmlDocument,materials);

			return geometries;
		}

		private Material[]	LoadMaterials(XDocument xmlDocument)
		{
			var materials = new List<Material>();

			var rawMaterials = xmlDocument.Root.Elements("material");

			foreach( var rawMaterial in rawMaterials )
			{

				var material = ContentManager.Creator.CreateMaterial();
				var textures = rawMaterial.Elements("texture");
				foreach( var texture in textures )
				{
					var filenameAttribute = texture.Attribute("filename");
					if (null != filenameAttribute)
					{
						var filename = filenameAttribute.Value;
						var loader = _assetLoaderService.GetLoader<Image>(filename);
						var frames = loader.Load(filename);
						var imageMap = new ImageMap(frames[0] as Image);
						material.DiffuseMap = imageMap;
						break;
					}
				}
				materials.Add(material);
			}

			return materials.ToArray();
			
		}

		private Geometry[] LoadGeometries(XDocument xmlDocument, Material[] materials)
		{
			var geometries = new List<Geometry>();
			var meshes = xmlDocument.Root.Elements("mesh");
			foreach( var mesh in meshes )
			{
				var geometry = ContentManager.CreateAssetPart<Geometry>();
				geometries.Add(geometry);

				var numVertices = Convert.ToInt32(mesh.Attribute("num_vertices").Value);
				var numFaces = Convert.ToInt32(mesh.Attribute("num_faces").Value);

				var geometryDetaillevel = geometry.GeometryContext.GetDetailLevel(DetailLevel.Full);

				geometryDetaillevel.AllocateVertices(numVertices);
				geometryDetaillevel.AllocateTextureCoordinates(numVertices);
				geometryDetaillevel.AllocateFaces(numFaces);

				var vertices = mesh.Elements("v");

				foreach( var vertex in vertices )
				{
					var index = int.Parse(vertex.Attribute("i").Value);
					var x = float.Parse(vertex.Attribute("px").Value, CultureInfo.InvariantCulture);
					var y = float.Parse(vertex.Attribute("py").Value, CultureInfo.InvariantCulture);
					var z = float.Parse(vertex.Attribute("pz").Value, CultureInfo.InvariantCulture);
					var u = float.Parse(vertex.Attribute("u0").Value, CultureInfo.InvariantCulture);
					var v = float.Parse(vertex.Attribute("v0").Value, CultureInfo.InvariantCulture);
					
					v = 1f - v;

					var actualVertex = new Vertex(x, y, z);
					geometryDetaillevel.SetVertex(index, actualVertex);

					var textureCoordinate = new TextureCoordinate(u, v);
					geometryDetaillevel.SetTextureCoordinate(index, textureCoordinate);
				}

				var faces = mesh.Elements("f");

				foreach( var face in faces )
				{
					var index = int.Parse(face.Attribute("i").Value);
					var a = int.Parse(face.Attribute("a").Value);
					var b = int.Parse(face.Attribute("b").Value);
					var c = int.Parse(face.Attribute("c").Value);
					var materialIndex = int.Parse(face.Attribute("matidx").Value);

					var actualFace = new Face(c,b,a)
					                 	{
					                 		DiffuseA = a,
					                 		DiffuseB = b,
					                 		DiffuseC = c,
											// TODO : Handle materials!
					                 		//Material = materials[materialIndex]
					                 	};
					geometryDetaillevel.SetFace(index, actualFace);
				}
			}

			return geometries.ToArray();
			
		}
	}
}