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
		private static void CalculateFaceNormals(IGeometryDetailLevel detailLevel, Vertex[] vertices, Face[] faces)
		{
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


		private static void CalculateVertexNormals(IGeometryDetailLevel detailLevel, Vertex[] vertices, Face[] faces)
		{
			var vertexCount = new Dictionary<int, int>();
			var vertexNormal = new Dictionary<int, Vector>();

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


		public static void CalculateNormals(IGeometryDetailLevel detailLevel)
		{
			var vertices = detailLevel.GetVertices();
			var faces = detailLevel.GetFaces();

			CalculateFaceNormals(detailLevel, vertices, faces);
			CalculateVertexNormals(detailLevel, vertices, faces);

			var vertexCount = new Dictionary<int, Dictionary<int, int>>();
			var vertexNormal = new Dictionary<int, Dictionary<int, Vector>>();

			SummarizeFaceNormals(faces, vertexNormal, vertexCount);
			var normals = CalculateNormalsForSmoothingGroups(faces, vertexNormal, vertexCount);
			detailLevel.AllocateNormals(normals.Count);
			var normalIndex = 0;
			foreach( var normal in normals )
			{
				detailLevel.SetNormal(normalIndex, normal);
				normalIndex++;
			}

		}

		private static List<Normal> CalculateNormalsForSmoothingGroups(Face[] faces, Dictionary<int, Dictionary<int, Vector>> vertexNormal, Dictionary<int, Dictionary<int, int>> vertexCount)
		{
			var normals = new List<Normal>();
			var normalIndex = 0;

			foreach (var vertex in vertexNormal.Keys)
			{
				var countPerSmoothingGroup = vertexCount[vertex];
				var smoothingGroups = vertexNormal[vertex];
				foreach (var smoothingGroupNumber in smoothingGroups.Keys)
				{
					var smoothingGroup = smoothingGroups[smoothingGroupNumber];
					var count = countPerSmoothingGroup[smoothingGroupNumber];
					var normal = smoothingGroup/count;
					normal.Normalize();

					foreach( var face in faces )
					{
						if( face.SmoothingGroup == smoothingGroupNumber )
						{
							if( face.A == vertex )
							{
								face.NormalA = normalIndex;
							}
							if( face.B == vertex )
							{
								face.NormalB = normalIndex;
							}
							if( face.C == vertex )
							{
								face.NormalC = normalIndex;
							}
						}
					}

					normals.Add(new Normal(normal.X, normal.Y, normal.Z));
					normalIndex++;
				}
			}
			return normals;
		}

		private static void SummarizeFaceNormals(Face[] faces, Dictionary<int, Dictionary<int, Vector>> vertexNormal, Dictionary<int, Dictionary<int, int>> vertexCount)
		{
			foreach (var face in faces)
			{
				AddFaceNormalForVertexInSmoothingGroup(vertexNormal, face.A, vertexCount, face);
				AddFaceNormalForVertexInSmoothingGroup(vertexNormal, face.B, vertexCount, face);
				AddFaceNormalForVertexInSmoothingGroup(vertexNormal, face.C, vertexCount, face);
			}
		}

		private static void AddFaceNormalForVertexInSmoothingGroup(Dictionary<int, Dictionary<int, Vector>> vertexNormal, int vertex, Dictionary<int, Dictionary<int, int>> vertexCount, Face face)
		{
			Dictionary<int, Vector> smoothingGroupVertices;
			Dictionary<int, int> smoothingGroupCount;
			if (!vertexNormal.ContainsKey(vertex))
			{
				smoothingGroupVertices = new Dictionary<int, Vector>();
				vertexNormal[vertex] = smoothingGroupVertices;
				smoothingGroupCount = new Dictionary<int, int>();
				vertexCount[vertex] = smoothingGroupCount;
			}
			else
			{
				smoothingGroupVertices = vertexNormal[vertex];
				smoothingGroupCount = vertexCount[vertex];
			}

			if (!smoothingGroupCount.ContainsKey(face.SmoothingGroup))
			{
				smoothingGroupCount[face.SmoothingGroup] = 1;
			}

			if (smoothingGroupVertices.ContainsKey(face.SmoothingGroup))
			{
				smoothingGroupVertices[face.SmoothingGroup] += face.Normal;	
			} else
			{
				smoothingGroupVertices[face.SmoothingGroup] = face.Normal;
			}
			
			smoothingGroupCount[face.SmoothingGroup]++;
		}
	}
}