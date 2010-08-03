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

using Balder.Execution;
using Balder.Materials;
using Balder.Objects.Geometries;

namespace Balder.Input
{
	public class ManipulationDeltaEventArgs : BubbledEventArgs
	{
		public ManipulationDeltaEventArgs()
		{
			
		}

		public ManipulationDeltaEventArgs(
			Material material,
			Face face,
			int faceIndex,
			float faceU,
			float faceV,
			float distance,
			int deltaX,
			int deltaY,
			ManipulationDirection direction)
		{
			Material = material;
			Face = face;
			FaceIndex = faceIndex;
			DeltaX = deltaX;
			DeltaY = deltaY;
			FaceU = faceU;
			FaceV = faceV;
			Distance = distance;
			Direction = direction;
		}

		public Material Material { get; internal set; }
		public int DeltaX { get; internal set; }
		public int DeltaY { get; internal set; }

		public Face Face { get; internal set; }
		public int FaceIndex { get; internal set; }

		public float FaceU { get; internal set; }
		public float FaceV { get; internal set; }

		public float Distance { get; internal set; }

		public ManipulationDirection Direction { get; internal set; }
	}
}