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
using Balder.Math;

namespace Balder.Objects.Geometries
{
	/// <summary>
	/// Helper for working with geometries and its data
	/// </summary>
	public static class GeometryHelper
	{
		/// <summary>
		/// Generate face normals for faces added to a specific <see cref="IGeometryDetailLevel"/>
		/// </summary>
		/// <param name="detailLevel"></param>
		public static void CalculateFaceNormals(IGeometryDetailLevel detailLevel)
		{
			var vertices = detailLevel.GetVertices();
			var faces = detailLevel.GetFaces();

			for( var faceIndex=0; faceIndex<faces.Length; faceIndex++ )
			{
				var v1 = vertices[faces[faceIndex].A].ToVector();
				var v2 = vertices[faces[faceIndex].B].ToVector();
				var v3 = vertices[faces[faceIndex].C].ToVector();

				var cross = (v2 - v1).Cross(v3 - v1);
				cross.Normalize();
				faces[faceIndex].Normal = -cross;
				detailLevel.InvalidateFace(faceIndex);
			}
		}


		/// <summary>
		/// Generate vertex normals for vertices added to a specific <see cref="IGeometryDetailLevel"/>
		/// </summary>
		/// <param name="detailLevel"></param>
		public static void CalculateVertexNormals(IGeometryDetailLevel detailLevel)
		{
			var vertexCount = new Dictionary<int, int>();
			var vertexNormal = new Dictionary<int, Vector>();

			var vertices = detailLevel.GetVertices();
			var faces = detailLevel.GetFaces();

			Func<int, Vector, int> addNormal =
				delegate(int vertex, Vector normal)
					{
						if (!vertexNormal.ContainsKey(vertex))
						{
							vertexNormal[vertex] = normal;
							vertexCount[vertex] = 1;
						}
						else
						{
							vertexNormal[vertex] += normal;
							vertexCount[vertex]++;
						}
						return 0;
					};

			foreach (var face in faces)
			{
				addNormal(face.A, face.Normal);
				addNormal(face.B, face.Normal);
				addNormal(face.C, face.Normal);
			}

			foreach (var vertex in vertexNormal.Keys)
			{
				var addedNormals = vertexNormal[vertex];
				var count = vertexCount[vertex];

				var normal = new Vector(addedNormals.X / count,
				                        addedNormals.Y / count,
				                        addedNormals.Z / count);
				vertices[vertex].NormalX = normal.X;
				vertices[vertex].NormalY = normal.Y;
				vertices[vertex].NormalZ = normal.Z;
				detailLevel.InvalidateVertex(vertex);
			}
		}
	}
}