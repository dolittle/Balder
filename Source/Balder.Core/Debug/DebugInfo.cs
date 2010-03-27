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

namespace Balder.Core.Debug
{
	/// <summary>
	/// Holds all flags for enabling or disabling certain debug information from the
	/// rendering.
	/// </summary>
	public class DebugInfo
	{
		/// <summary>
		/// Creates a debug info object
		/// </summary>
		public DebugInfo()
		{
			Color = Color.FromArgb(0xFF, 0xFF, 0xFF, 0);
		}

		/// <summary>
		/// Get or set if the geometry should be visualized in a debug manner
		/// </summary>
		public bool Geometry { get; set; }

		/// <summary>
		/// Get or set if the normals of every face in a geometry should be rendered
		/// </summary>
		public bool FaceNormals { get; set; }

		/// <summary>
		/// Get or set if the normals of every vertex in a geometry should be rendered
		/// </summary>
		public bool VertexNormals { get; set; }

		/// <summary>
		/// Get or set if the lights should be visualized
		/// </summary>
		public bool Lights { get; set; }

		/// <summary>
		/// Get or set if the bounding boxes for objects should be rendered
		/// </summary>
		public bool BoundingBoxes { get; set; }

		/// <summary>
		/// Get or set if the bounding spheres for objects should be rendered
		/// </summary>
		public bool BoundingSpheres { get; set; }

		/// <summary>
		/// Get or set if the vertices should be rendered
		/// </summary>
		public bool ShowVertices { get; set; }

		/// <summary>
		/// Get or set if the ray that is used for mouse hit detection should show or not
		/// </summary>
		public bool ShowMouseHitDetectionRay { get; set; }

		/// <summary>
		/// Get or set the debug information color used for all debug info
		/// </summary>
		public Color Color { get; set; } 
	}
}