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
using System.IO;
using System.Text;
using Balder.Content;
using Balder.Exceptions;
using Balder.Math;
using Balder.Objects.Geometries;
using Balder.Rendering;
using Geometry = Balder.Objects.Geometries.Geometry;
using Matrix = Balder.Math.Matrix;

namespace Balder.Assets.AssetLoaders
{
	// Based on code from http://helixtoolkit.codeplex.com/

	public class StudioLoader : AssetLoader
	{
		private readonly IAssetLoaderService _assetLoaderService;

		public StudioLoader(IAssetLoaderService assetLoaderService, IFileLoaderManager fileLoaderManager, IContentManager contentManager)
			: base(fileLoaderManager, contentManager)
		{
			_assetLoaderService = assetLoaderService;
		}

		public override string[] FileExtensions { get { return new[] {"3ds"}; } }

		public override Type SupportedAssetType { get { return typeof(Mesh); }}

		public override IAssetPart[] Load(string assetName)
		{
			var fileLoader = FileLoaderManager.GetFileLoader(assetName);
			var stream = fileLoader.GetStream(assetName);
			if (null == stream)
			{
				throw new AssetNotFoundException(assetName);
			}

			var reader = new BinaryReader(stream);


			var geometries = LoadGeometries(reader);
			return geometries;
		}

		Geometry[] LoadGeometries(BinaryReader reader)
		{
			var geometries = new List<Geometry>();

			var length = reader.BaseStream.Length;
			var headerID = ReadChunkId(reader);
			if (headerID != StudioChunk.MAIN3DS)
				throw new ArgumentException("Unknown 3DS file");

			var headerSize = ReadChunkSize(reader);
			if (headerSize != length)
				throw new ArgumentException("Incomplete file (file length does not match header)");

			while (reader.BaseStream.Position < reader.BaseStream.Length)
			{
				var id = ReadChunkId(reader);
				var size = ReadChunkSize(reader);

				switch (id)
				{
					case StudioChunk.EDIT_MATERIAL:
						ReadMaterial(reader, size);
						break;
					case StudioChunk.EDIT_OBJECT:
						var geometry = ReadObject(reader, size);
						if( geometry != null )
							geometries.Add(geometry);

						break;
					case StudioChunk.EDIT3DS:
					case StudioChunk.OBJ_CAMERA:
					case StudioChunk.OBJ_LIGHT:
					case StudioChunk.OBJ_TRIMESH:
						// don't read the whole chunk, read the sub-defines...
						break;

					default:
						// download the whole chunk
						var bytes = ReadData(reader, size - 6);
						break;
				}
			}

			reader.Close();

			HandleNormals(geometries);

			return geometries.ToArray();
		}

		private void HandleNormals(IEnumerable<Geometry> geometries)
		{
			foreach (var geometry in geometries)
			{
				var geometryDetailLevel = geometry.GeometryContext.GetDetailLevel(DetailLevel.Full);
				GeometryHelper.CalculateNormals(geometryDetailLevel);
			}
		}




		private int ReadChunkSize(BinaryReader reader)
		{
			return (int)reader.ReadUInt32();
		}

		private StudioChunk ReadChunkId(BinaryReader reader)
		{
			return (StudioChunk)reader.ReadUInt16();
		}

		private string ReadString(BinaryReader reader)
		{
			var sb = new StringBuilder();
			while (true)
			{
				var ch = (char)reader.ReadByte();
				if (ch == 0)
					break;
				sb.Append(ch);
			}
			return sb.ToString();
		}

		private byte[] ReadData(BinaryReader reader, int size)
		{
			return reader.ReadBytes(size);
		}

		private Geometry ReadObject(BinaryReader reader, int msize)
		{
			var total = 6;

			var objectName = ReadString(reader);
			total += objectName.Length + 1;

			while (total < msize)
			{
				var id = ReadChunkId(reader);
				var size = ReadChunkSize(reader);
				total += size;
				switch (id)
				{
					case StudioChunk.OBJ_TRIMESH:
						var geometry = ContentManager.CreateAssetPart<Geometry>();
						var geometryDetailLevel = geometry.GeometryContext.GetDetailLevel(DetailLevel.Full);
						ReadTriangularMesh(reader, size, geometry, geometryDetailLevel);
						return geometry;
					// case StudioChunk.OBJ_CAMERA:
					default:
						{
							var bytes = ReadData(reader, size - 6);
							break;
						}
				}
			}

			return null;
		}

		private void ReadTriangularMesh(BinaryReader reader, int chunkSize, Geometry geometry, IGeometryDetailLevel geometryDetailLevel)
		{
			var bytesRead = 6;

			while (bytesRead < chunkSize)
			{
				var id = ReadChunkId(reader);
				var size = ReadChunkSize(reader);
				bytesRead += size;
				switch (id)
				{
					case StudioChunk.TRI_VERTEXL:
						ReadVertices(reader, geometryDetailLevel);
						break;
					case StudioChunk.TRI_FACEL1:
						var faceCount = ReadFaces(reader, geometryDetailLevel);
						size -= (faceCount * 8 + 2);
						var facemat = ReadFaceMaterials(reader, size - 6);
						break;
					case StudioChunk.TRI_TEXCOORD:
						ReadTexCoords(reader, geometryDetailLevel);
						break;
					case StudioChunk.TRI_LOCAL:
						geometry.World = ReadTransformation(reader);	
						break;

					default:
						ReadData(reader, size - 6);
						break;
				}
			}
		}

		private Vector ReadVector(BinaryReader reader)
		{
			return new Vector(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		private void ReadVertices(BinaryReader reader, IGeometryDetailLevel geometryDetailLevel)
		{
			var size = reader.ReadUInt16();

			geometryDetailLevel.AllocateVertices(size);
			for (var i = 0; i < size; i++)
			{
				var x = reader.ReadSingle();
				var z = reader.ReadSingle();
				var y = -reader.ReadSingle();
				var vertex = new Vertex(x, y, z);
				geometryDetailLevel.SetVertex(i, vertex);
			}
		}

		private int ReadFaces(BinaryReader reader, IGeometryDetailLevel geometryDetailLevel)
		{
			var size = reader.ReadUInt16();
			geometryDetailLevel.AllocateFaces(size);
			for (var i = 0; i < size; i++)
			{
				var c = reader.ReadUInt16();
				var b = reader.ReadUInt16();
				var a = reader.ReadUInt16();
				geometryDetailLevel.SetFace(i, new Face(a,b,c));
				float flags = reader.ReadUInt16();
			}
			return size;
		}

		private void ReadTexCoords(BinaryReader reader, IGeometryDetailLevel geometryDetailLevel)
		{
			var size = reader.ReadUInt16();
			geometryDetailLevel.AllocateTextureCoordinates(size);
			
			for (var i = 0; i < size; i++)
			{
				var u = reader.ReadSingle();
				var v = reader.ReadSingle();
				geometryDetailLevel.SetTextureCoordinate(i, new TextureCoordinate(u,v));
			}
		}

		private Matrix ReadTransformation(BinaryReader reader)
		{
			var localx = ReadVector(reader);
			var localy = ReadVector(reader);
			var localz = ReadVector(reader);
			var origin = ReadVector(reader);

			var matrix = new Matrix
			             	{
			             		M11 = localx.X,
			             		M21 = localx.Y,
			             		M31 = localx.Z,
			             		M12 = localy.X,
			             		M22 = localy.Y,
			             		M32 = localy.Z,
			             		M13 = localz.X,
			             		M23 = localz.Y,
			             		M33 = localz.Z,
			             		M41 = origin.X,
			             		M42 = origin.Y,
			             		M43 = origin.Z,
			             		M14 = 0,
			             		M24 = 0,
			             		M34 = 0,
			             		M44 = 1
			             	};

			return matrix;
		}

		private List<FaceMaterial> ReadFaceMaterials(BinaryReader reader, int msize)
		{
			var total = 6;
			var list = new List<FaceMaterial>();
			while (total < msize)
			{
				var id = ReadChunkId(reader);
				int size = ReadChunkSize(reader);
				total += size;
				switch (id)
				{
					case StudioChunk.TRI_FACEMAT:
						{
							var name = ReadString(reader);
							var n = reader.ReadUInt16();
							var c = new List<int>();
							for (var i = 0; i < n; i++)
							{
								c.Add(reader.ReadUInt16());
							}
							var fm = new FaceMaterial { Name = name, Faces = c };
							list.Add(fm);
							break;
						}
					case StudioChunk.TRI_SMOOTH:
						{
							var bytes = ReadData(reader, size - 6);
							break;
						}
					default:
						{
							var bytes = ReadData(reader, size - 6);
							break;
						}
				}
			}
			return list;
		}

		private void ReadMaterial(BinaryReader reader, int msize)
		{
			var total = 6;
			string name = null;

			var luminance = Colors.Transparent;
			var diffuse = Colors.Transparent;
			var specular = Colors.Transparent;
			var shininess = Colors.Transparent;
			string texture = null;

			while (total < msize)
			{
				var id = ReadChunkId(reader);
				var size = ReadChunkSize(reader);
				total += size;

				switch (id)
				{
					case StudioChunk.MAT_NAME01:
						name = ReadString(reader);
						// name = ReadString(size - 6);
						break;

					case StudioChunk.MAT_LUMINANCE:
						luminance = ReadColor(reader,size);
						break;

					case StudioChunk.MAT_DIFFUSE:
						diffuse = ReadColor(reader, size);
						break;

					case StudioChunk.MAT_SPECULAR:
						specular = ReadColor(reader, size);
						break;

					case StudioChunk.MAT_SHININESS:
						var bytes = ReadData(reader, size - 6);
						// shininess = ReadColor(r, size);
						break;

					case StudioChunk.MAT_MAP:
						texture = ReadMatMap(reader, size - 6);
						break;

					case StudioChunk.MAT_MAPFILE:
						ReadData(reader, size - 6);
						break;

					default:
						ReadData(reader, size - 6);
						break;
				}
			}
			int specularPower = 100;
		}

		private string ReadMatMap(BinaryReader reader, int size)
		{
			var id = ReadChunkId(reader);
			var siz = ReadChunkSize(reader);
			var f1 = reader.ReadUInt16();
			var f2 = reader.ReadUInt16();
			var f3 = reader.ReadUInt16();
			var f4 = reader.ReadUInt16();
			size -= 14;
			var cname = ReadString(reader);
			size -= cname.Length + 1;
			var morebytes = ReadData(reader, size);
			return cname;
		}

		private Color ReadColor(BinaryReader reader, int size)
		{
			var type = ReadChunkId(reader);
			int csize = ReadChunkSize(reader);
			size -= 6;
			switch (type)
			{
				case StudioChunk.COL_RGB:
					{
						// not checked...
						var r = reader.ReadSingle();
						var g = reader.ReadSingle();
						var b = reader.ReadSingle();
						return Colors.White;
						//Color.FromScRgb(1, r, g, b);
					}
				case StudioChunk.COL_TRU:
					{
						var r = reader.ReadByte();
						var g = reader.ReadByte();
						var b = reader.ReadByte();
						return Color.FromArgb(0xff, r, g, b);
					}
				default:
					ReadData(reader, csize);
					break;
			}
			return Colors.White;
		}


		private class FaceMaterial
		{
			public string Name { get; set; }
			public List<int> Faces { get; set; }
		}


	}
}
