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
using Balder.Core.Execution;
using Balder.Core.Materials;
using Balder.Core.Objects.Geometries;
using Ninject;

namespace Balder.Core.Content
{
	public class ContentCreator : IContentCreator
	{
		private readonly IKernel _kernel;
		private readonly IIdentityManager _identityManager;

		/// <summary>
		/// Creates a new ContentCreator and provides functionality for creating content
		/// </summary>
		/// <param name="kernel">Kernel that the ContentCreator will use for creating content</param>
		/// <param name="identityManager">IdentityManager used during creation of certain content</param>
		public ContentCreator(IKernel kernel, IIdentityManager identityManager)
		{
			_kernel = kernel;
			_identityManager = identityManager;
		}

		public T CreateGeometry<T>() where T : Geometry
		{
			var geometry = _kernel.Get<T>();
			return geometry;
		}

		public Material CreateMaterial()
		{
			var material = new Material(_identityManager);
			return material;
		}
	}
}
