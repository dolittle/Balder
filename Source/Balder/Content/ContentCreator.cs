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

using Balder.Materials;
using Balder.Objects.Geometries;
using Ninject;

namespace Balder.Content
{
	/// <summary>
	/// Represents a <see cref="IContentCreator"/>
	/// </summary>
	public class ContentCreator : IContentCreator
	{
		private readonly IKernel _kernel;

		/// <summary>
		/// Initializes an instance of <see cref="ContentCreator"/>
		/// </summary>
		/// <param name="kernel">Kernel that the ContentCreator will use for creating content</param>
		public ContentCreator(IKernel kernel)
		{
			_kernel = kernel;
		}

#pragma warning disable 1591
		public T CreateGeometry<T>() where T : Geometry
		{
			var geometry = _kernel.Get<T>();
			return geometry;
		}

		public Material CreateMaterial()
		{
			var material = _kernel.Get<Material>();
			return material;
		}
#pragma warning restore 1591
	}
}
