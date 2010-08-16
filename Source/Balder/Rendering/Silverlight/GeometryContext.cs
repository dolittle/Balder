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
#if(SILVERLIGHT)
using System.Collections.Generic;
using Balder.Display;
using Balder.Lighting;
using Balder.Materials;
using Balder.Math;
using Balder.Objects.Geometries;

namespace Balder.Rendering.Silverlight
{
	public class GeometryContext : IGeometryContext
	{
		private readonly Dictionary<DetailLevel, IGeometryDetailLevel> _detailLevels;
		private readonly ILightCalculator _lightCalculator;
		private readonly ITextureManager _textureManager;

		public GeometryContext(ILightCalculator lightCalculator, ITextureManager textureManager)
		{
			_lightCalculator = lightCalculator;
			_textureManager = textureManager;
			_detailLevels = new Dictionary<DetailLevel, IGeometryDetailLevel>();
		}


		public void SetMaterialForAllFaces(Material material)
		{
			foreach (var detailLevel in _detailLevels.Values)
			{
				detailLevel.SetMaterialForAllFaces(material);
			}
		}

		public void GenerateDetailLevel(DetailLevel targetLevel, DetailLevel sourceLevel)
		{
			if( targetLevel == DetailLevel.BoundingBox )
			{
				GenerateBoundingBoxDetailLevel(sourceLevel);	
			}
		}

		public IGeometryDetailLevel GetDetailLevel(DetailLevel level)
		{
			IGeometryDetailLevel detailLevel = null;
			if (_detailLevels.ContainsKey(level))
			{
				detailLevel = _detailLevels[level];
			}
			else
			{
				detailLevel = new GeometryDetailLevel(_lightCalculator);
				_detailLevels[level] = detailLevel;
			}
			return detailLevel;
		}

		public void Render(Viewport viewport, INode node, DetailLevel detailLevel)
		{
			var geometryDetailLevel = GetDetailLevel(detailLevel);
			if( null != geometryDetailLevel )
			{
				geometryDetailLevel.Render(viewport, node);
			}
		}

		public bool HasDetailLevel(DetailLevel level)
		{
			return _detailLevels.ContainsKey(level);
		}

		private void GenerateBoundingBoxDetailLevel(DetailLevel sourceLevel)
		{
			var sourceDetailLevel = GetDetailLevel(sourceLevel);

			var minimum = new Vector(0, 0, 0);
			var maximum = new Vector(0, 0, 0);
			var vertices = sourceDetailLevel.GetVertices();

			if (null != vertices)
			{
				foreach (var vertex in vertices)
				{
					if (vertex.X < minimum.X)
					{
						minimum.X = vertex.X;
					}
					if (vertex.Y < minimum.Y)
					{
						minimum.Y = vertex.Y;
					}
					if (vertex.Z < minimum.Z)
					{
						minimum.Z = vertex.Z;
					}
					if (vertex.X > maximum.X)
					{
						maximum.X = vertex.X;
					}
					if (vertex.Y > maximum.Y)
					{
						maximum.Y = vertex.Y;
					}
					if (vertex.Z > maximum.Z)
					{
						maximum.Z = vertex.Z;
					}
				}

				var boundingBoxDetailLevel = new BoundingGeometryDetailLevel(minimum, maximum, _lightCalculator);
				_detailLevels[DetailLevel.BoundingBox] = boundingBoxDetailLevel;
			}
		}
	}
}
#endif