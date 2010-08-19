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
namespace Balder.Objects.Geometries
{
	public class GeometryStatistics : NodeStatistics
	{
		private long _verticesStartTime;
		private long _lightingStartTime;
		private long _renderingStartTime;


		private int _renderedFaces;
		public int RenderedFaces
		{
			get { return _renderedFaces; }
			set
			{
				_renderedFaces = value;
#if(SILVERLIGHT)
				OnPropertyChanged("RenderedFaces");
#endif
			}
		}

		private int _renderedLines;
		public int RenderedLines
		{
			get { return _renderedLines; }
			set
			{
				_renderedLines = value;
#if(SILVERLIGHT)
				OnPropertyChanged("RenderedLines");
#endif
			}
		}


		private long _timeSpentInVertices;
		public long TimeSpentInVertices
		{
			get { return _timeSpentInVertices; }
			set
			{
				_timeSpentInVertices = value;
#if(SILVERLIGHT)
				OnPropertyChanged("TimeSpentInVertices");
#endif
			}
		}

		private long	_timeSpentInLighting;
		public long TimeSpentInLighting
		{
			get { return _timeSpentInLighting; }
			set
			{
				_timeSpentInLighting = value;
#if(SILVERLIGHT)
				OnPropertyChanged("TimeSpentInLighting");
#endif
			}
		}

		private long _timeSpentInRendering;
		public long TimeSpentInRendering
		{
			get { return _timeSpentInRendering; }
			set
			{
				_timeSpentInRendering = value;
#if(SILVERLIGHT)
				OnPropertyChanged("TimeSpentInRendering");
#endif
			}
		}


		public void BeginVerticesTiming()
		{
			_verticesStartTime = Stopwatch.ElapsedMilliseconds;
		}

		public void EndVerticesTiming()
		{
			TimeSpentInVertices = Stopwatch.ElapsedMilliseconds - _verticesStartTime;
		}

		public void BeginLightingTiming()
		{
			_lightingStartTime = Stopwatch.ElapsedMilliseconds;
		}

		public void EndLightingTiming()
		{
			TimeSpentInLighting = Stopwatch.ElapsedMilliseconds - _lightingStartTime;
		}

		public void BeginRenderingTiming()
		{
			_renderingStartTime = Stopwatch.ElapsedMilliseconds;
		}

		public void EndRenderingTiming()
		{
			TimeSpentInRendering = Stopwatch.ElapsedMilliseconds - _renderingStartTime;
		}
	}
}