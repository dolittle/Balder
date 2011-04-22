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
using System.Linq;
using System.Xml.Linq;
using Balder.Objects.Geometries;

namespace Balder.Assets.AssetLoaders
{
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

		public static string[] SplitAndTrim(this string stringToSplit, char charToSplitOn)
		{
			var query = from s in stringToSplit.Split(charToSplitOn)
						where !string.IsNullOrWhiteSpace(s)
						select s;
			return query.ToArray();
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
						var values = float_array.Value.SplitAndTrim(' ');
						for (var valueIndex = 0; valueIndex < values.Length; valueIndex += stride)
						{
							var vertex = new Vertex
											{
												X = float.Parse(values[valueIndex], CultureInfo.InvariantCulture),
												Y = -float.Parse(values[valueIndex + 1], CultureInfo.InvariantCulture),
												Z = -float.Parse(values[valueIndex + 2], CultureInfo.InvariantCulture),
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
						var values = float_array.Value.SplitAndTrim(' ');
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

		private static int GetFaceIndexSize(this XElement polylist, XNamespace ns)
		{
			var inputs = polylist.Elements(ns + "input");
			if (inputs != null)
			{
				return inputs.Count();
			}
			return 1;
		}

		public static Face[] GetFaces(this XElement polylist, XNamespace ns)
		{
			var faces = new List<Face>();

			var p = polylist.Element(ns + "p");
			if (p != null)
			{
				var faceIndexSize = polylist.GetFaceIndexSize(ns);
				var vertexCounts = (from v in polylist.Element(ns + "vcount").Value.SplitAndTrim(' ')
									select Int32.Parse(v, CultureInfo.InvariantCulture)).ToArray();

				var indices = p.Value.SplitAndTrim(' ');
				var index = 0;
				foreach (var vertexCount in vertexCounts)
				{
					var face = new Face(
						int.Parse(indices[index + (faceIndexSize * 2)], CultureInfo.InvariantCulture),
						int.Parse(indices[index + faceIndexSize], CultureInfo.InvariantCulture),
						int.Parse(indices[index + 0], CultureInfo.InvariantCulture))
					{
						NormalA = int.Parse(indices[index + (faceIndexSize * 2) + 1], CultureInfo.InvariantCulture),
						NormalB = int.Parse(indices[index + faceIndexSize + 1], CultureInfo.InvariantCulture),
						NormalC = int.Parse(indices[index + 1], CultureInfo.InvariantCulture)
					};

					faces.Add(face);
					if (vertexCount == 4)
					{
						face = new Face(
							int.Parse(indices[index + (faceIndexSize * 3)], CultureInfo.InvariantCulture),
							int.Parse(indices[index + (faceIndexSize * 2)], CultureInfo.InvariantCulture),
							int.Parse(indices[index + 0], CultureInfo.InvariantCulture))
								{
									NormalA = int.Parse(indices[index + (faceIndexSize * 3) +1], CultureInfo.InvariantCulture),
									NormalB = int.Parse(indices[index + (faceIndexSize * 2) +1], CultureInfo.InvariantCulture),
									NormalC = int.Parse(indices[index + 1], CultureInfo.InvariantCulture)
								};
					}

					faces.Add(face);

					index += vertexCount * faceIndexSize;
				}




			}
			return faces.ToArray();
		}

	}
}