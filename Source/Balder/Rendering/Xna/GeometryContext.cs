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
#if(XNA)
using System;
using System.Collections.Generic;
using Balder.Display;
using Balder.Materials;
using Balder.Objects.Geometries;

namespace Balder.Rendering.Xna
{
    public class GeometryContext : IGeometryContext
    {
        private readonly Dictionary<DetailLevel, IGeometryDetailLevel> _detailLevels;

        public GeometryContext()
        {
            _detailLevels = new Dictionary<DetailLevel, IGeometryDetailLevel>();
        }

        public void SetMaterialForAllFaces(Material material)
        {
            
        }

        public void GenerateDetailLevel(DetailLevel targetLevel, DetailLevel sourceLevel)
        {
            
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
                detailLevel = new GeometryDetailLevel();
                _detailLevels[level] = detailLevel;
            }
            return detailLevel;
        }

        public void Render(Viewport viewport, INode node, DetailLevel detailLevel)
        {
            var geometryDetailLevel = GetDetailLevel(detailLevel);
            if (null != geometryDetailLevel)
            {
                geometryDetailLevel.Render(viewport, node);
            }
            
        }

        public bool HasDetailLevel(DetailLevel level)
        {
            return _detailLevels.ContainsKey(level);
        }
    }
}
#endif